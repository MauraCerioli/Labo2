using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata;
using MyAttribute;

namespace Executer {
    internal class Program {
        static bool ArgumentsAreAcceptable(object?[] providedArguments, MethodBase candidate, [NotNullWhen(true)] out object?[]? actualParameters) {
            var formalParameters = candidate.GetParameters();
            actualParameters = new object?[formalParameters.Length];
            bool typesAsExpected = true;
            bool paramsFound = false;
            var j = 0;
            while (j < formalParameters.Length && !formalParameters[j].HasDefaultValue) {
                if (formalParameters[j].ParameterType.IsByRef) {
                    AbortTypeCheck(out actualParameters, out typesAsExpected, "reference parameter expected. No way");
                    return false;
                }
                if (formalParameters[j].GetCustomAttributes<ParamArrayAttribute>().Any()) {
                    paramsFound = true;
                    Debug.WriteLine("Found a params");
                    break;
                }
                if (j >= providedArguments.Length) {
                    AbortTypeCheck(out actualParameters, out typesAsExpected, "missing parameter.No way");
                    return false;
                }
                if (!OneTypeOk(providedArguments[j]?.GetType(), formalParameters[j].ParameterType)) {
                    AbortTypeCheck(out actualParameters, out typesAsExpected, "wrong parameter type. No way");
                    return false;
                }
                actualParameters[j] = providedArguments[j];
                ++j;
            }
            //non-optional providedArguments are ok
            //default parameters cannot be by reference nor params
            while (j < formalParameters.Length && formalParameters[j].HasDefaultValue) {
                if (j >= providedArguments.Length)
                    actualParameters[j] = formalParameters[j].DefaultValue;
                else {
                    if (!OneTypeOk(providedArguments[j]?.GetType(), formalParameters[j].ParameterType)) {
                        AbortTypeCheck(out actualParameters, out typesAsExpected, "wrong parameter type. No way");
                        return false;
                    }
                    actualParameters[j] = providedArguments[j];
                }
                ++j;
            }
            //possibly a final params parameter to check
            if (j < formalParameters.Length) {
                Debug.Assert(j + 1 == formalParameters.Length && formalParameters[j].GetCustomAttributes<ParamArrayAttribute>().Any());
                var baseParamsType = formalParameters[j].ParameterType.GetElementType()!;//params parameter typ are collections=>they have an element type
                if (j >= providedArguments.Length) {
                    actualParameters[j] = null;
                    return true;
                }
                if (j == providedArguments.Length-1) {
                    if (OneTypeOk(providedArguments[j]?.GetType(), formalParameters[j].ParameterType)) {
                        actualParameters[j] = providedArguments[j];//the last parameter is explicitly packed as an array
                        return true;
                    }
                }
                var paramsValuesLength = providedArguments.Length - j;
                var paramsValues = Array.CreateInstance(baseParamsType, paramsValuesLength);
                for (var k = 0; k < paramsValuesLength; k++) {
                    if (!OneTypeOk(providedArguments[j + k]?.GetType(), baseParamsType)) {
                        AbortTypeCheck(out actualParameters, out typesAsExpected, "unacceptable type for an element of the params parameter. No way");
                        return false;
                    }
                    paramsValues.SetValue(providedArguments[j + k], k);//Array type has no indexer
                }
                actualParameters[j] = paramsValues;
            }
            else if(providedArguments.Length>j){
                AbortTypeCheck(out actualParameters, out typesAsExpected, "too many arguments. No way");
                return false;
            }


            return true;
            /* https://stackoverflow.com/questions/3622381/how-to-check-assignability-of-types-at-runtime-in-c
              There are actually three ways that a type can be “assignable” to another.              
               - Class hierarchy, interface implementation, covariance and contravariance. This is what .IsAssignableFrom already checks for. (This also includes permissible boxing operations, e.g. int to object or DateTime to ValueType.)
               - User-defined implicit conversions.
               - Built-in implicit conversions which are defined in the C# language specification. Unfortunately Reflection does not show these.
               Furthermore, a user-defined implicit conversion can be chained with a built-in implicit conversion.
             */
            bool OneTypeOk(Type? argType, Type parType) {
                bool b;
                if (argType == null)//the element generating argType is null=>it is assignable to any reference or nullable type
                    b = parType.IsClass ||
                        parType.IsInterface ||
                        Nullable.GetUnderlyingType(parType) != null;
                else b = parType.IsAssignableFrom(argType);//Given the context it's like using equality :-(
                return b;
            }

        }

        static bool FoundConstructor(object?[] providedArguments, Type t, [NotNullWhen(true)] out ConstructorInfo? c,
            [NotNullWhen(true)] out object?[]? actualParameters) {
            var constructors = t.GetConstructors();
            foreach (var candidate in constructors) {
                var formalParameters = candidate.GetParameters();
                if (ArgumentsAreAcceptable(providedArguments, candidate, out actualParameters)) {
                    c = candidate;
                    return true;
                }
            }
            c=null;
            actualParameters = null;
            return false;
        }

        static void ExtendedSolution(Assembly assembly) {
            Console.WriteLine("Alternative approach: Execute me chooses its constructor");
            foreach (var t in assembly.GetTypes()) {
                if (!t.IsClass)
                    continue;
                Console.WriteLine(t.Name);
                var methods = t.GetMethods();
                foreach (var m in methods) {
                    Console.WriteLine($"====================={m.Name}");
                    foreach (var a in m.GetCustomAttributes<ExecuteMePlusAttribute>()) {
                        Console.WriteLine("+++++++++++++++");
                        object? o;
                        try {
                            if (a.ConstructorArguments != null)
                                if (FoundConstructor(a.ConstructorArguments, t,
                                        out var c, out var actualParameters))
                                    o = c.Invoke(actualParameters);
                                else continue;
                            else o = Activator.CreateInstance(t);
                            Console.WriteLine("Created object");
                        }
                        catch (Exception e) {//to be improved with different catches for the possible significant exceptions
                            Console.WriteLine(e);
                            continue;
                        }
                        try {
                            if (!ArgumentsAreAcceptable(a.Arguments,m,out var processedArguments))
                                continue;
                            m.Invoke(o, processedArguments);
                        }
                        catch (TargetParameterCountException ex) {
                            Console.WriteLine($"Wrong arguments' number ({m.GetParameters().Length} expected, {a.Arguments.Length} received)");
                        }
                        catch (ArgumentException argEx) {
                            Console.WriteLine($"Unacceptable Arguments ({argEx.Message})");
                        }
                        catch (Exception exception) {
                            Console.WriteLine($"Something went wrong ({exception.Message})");
                        }
                    }
                }
            }

            /*
            object CreateInstance(Type t) {
                try {
                    var instance = Activator.CreateInstance(t);
                    if (instance!=null) return instance;
                    throw new ArgumentNullException();
                }
                catch (Exception e) {
                    Console.WriteLine(e);
                }
                var constructors = t.GetConstructors();
                foreach (var c in constructors) {
                    var expected =c.GetParameters();
                    var arguments = new object?[expected.Length];
                    var i = 0;
                    foreach (var p in expected) {
                        if (p.HasDefaultValue) {
                            arguments[i++] = p.DefaultValue;
                            continue;
                        }

                        if (p.ParameterType.H) {
                            
                        }
                    }
                }
            }*/
        }
        private static void AbortTypeCheck(out object?[]? actualParameters, out bool typesAsExpected, string debugMessage) {
            Debug.WriteLine(debugMessage);
            actualParameters = null;
            typesAsExpected = false;
            return;
        }


        static void Main(string[] args) {
            var path = @"..\..\..\..\AnotherLibrary\bin\Debug\net8.0\AnotherLibrary.dll";
            var a = Assembly.LoadFrom(path);
            /*var t = a.GetType("MyLibrary.Experiment");
            var m = t?.GetMethod("M0");
            var mParameters = m?.GetParameters();
            var thriller = mParameters[mParameters.Length - 1].GetCustomAttributes<ParamArrayAttribute>();
            Console.WriteLine("Finito");
            */
            //BasicSolution(a);
            ExtendedSolution(a);
            /*
             * GetInstance sort of works, but its design goes against logical expectations:
             * If using default value creates a usable object, why the constructor without parameters
             * is missing?
             * To use it replace o = Activator.CreateInstance(t); by o = GetInstance(t);
             *
             */

        }
        object? GetInstance(Type t) {
            object? result = null;
            var constructors = t.GetConstructors();
            try {
                return Activator.CreateInstance(t);
            }
            catch (MissingMethodException e) {
                Console.WriteLine($"No empty constructor: {e.Message}");
            ConstructorLoop: foreach (var c in constructors) {
                    var parameterInfos = c.GetParameters();
                    var arguments = new object?[parameterInfos.Length];
                    for (int i = 0; i < parameterInfos.Length; i++) {
                        var p = parameterInfos[i];
                        if (p.HasDefaultValue) {
                            arguments[i] = p.DefaultValue;
                        }
                        else goto ConstructorLoop;
                    }
                    return c.Invoke(null, arguments);
                }
            }
            catch (Exception e) {
                Console.WriteLine($"Something else went wrong ({e})");
            }
            return result;
        }
        private static void BasicSolution(Assembly a) {
            foreach (var t in a.GetTypes()) {
                if (!t.IsClass) continue;
                foreach (var m in t.GetMethods())
                    foreach (var e in m.GetCustomAttributes<ExecuteMeAttribute>()) {
                        object? o;
                        try {
                            o = Activator.CreateInstance(t);
                        }
                        catch (Exception exception) {
                            goto TypeLoop;
                        }
                        try {
                            m.Invoke(o, e.Arguments);
                        }
                        catch (TargetParameterCountException ex) {
                            Console.WriteLine($"Wrong arguments' number ({m.GetParameters().Length} expected, {e.Arguments.Length} received)");
                        }
                        catch (ArgumentException argEx) {
                            Console.WriteLine($"Unacceptable Arguments ({argEx.Message})");
                        }
                        catch (Exception exception) {
                            Console.WriteLine($"Something went wrong ({exception.Message})");
                        }
                    }
                TypeLoop: Console.WriteLine(t.FullName);
            }
        }
    }
}
