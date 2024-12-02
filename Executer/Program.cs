using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using MyAttribute;

namespace Executer {
    internal class Program {
        static void AlternativeMain() {
            var assembly = Assembly.LoadFrom("..\\..\\..\\..\\MyLibrary\\bin\\Debug\\net8.0\\MyLibrary.dll");
            foreach (var t in assembly.GetTypes()) {
                if (!t.IsClass)
                    continue;
                Console.WriteLine(t.Name);
                var methods = t.GetMethods();
                foreach (var m in methods) {
                    foreach (var a in m.GetCustomAttributes<ExecuteMePlusAttribute>()) {
                        object? o;
                        try {
                            if (a.ConstructorArguments != null)
                                if (FoundConstructor(a.ConstructorArguments.Select(arg => arg?.GetType()).ToArray(), t,
                                        out var c))
                                    o = c.Invoke(a.ConstructorArguments);
                                else break;
                            else o = Activator.CreateInstance(t);
                        }
                        catch (Exception e) {//to be improved with different catches for the possible significant exceptions
                            Console.WriteLine(e);
                            continue;
                        }
                        try {
                            m.Invoke(o, a.Arguments);
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

            bool FoundConstructor(Type?[] actualTypes, Type t, [NotNullWhen(true)] out ConstructorInfo? c) {
                var constructors = t.GetConstructors();
                foreach (var candidate in constructors) {
                    var expectedTypes = candidate.GetParameters();
                    var min = Math.Min(expectedTypes.Length, actualTypes.Length);
                    bool typesAsExpected = true;
                    typesAsExpected = TypesAsExpected(min - 1, actualTypes, expectedTypes);
                    if (!typesAsExpected) continue;
                    switch (expectedTypes.Length - actualTypes.Length) {
                        case < 0://wrong unless last parameters is a params
                            if (!expectedTypes[min - 1].GetCustomAttributes<ParamArrayAttribute>().Any()) typesAsExpected = false;
                            else {
                                var paramsType = expectedTypes[min - 1].ParameterType.GetElementType()!;//the last parameters is params, its type must be an array
                                for (int i = min - 1; i < actualTypes.Length && typesAsExpected; i++) typesAsExpected = OneTypeOk(actualTypes[i], paramsType);
                            }
                            break;
                        case 0:
                            typesAsExpected = OneTypeOk(actualTypes[min - 1], expectedTypes[min - 1].ParameterType);
                            break;
                        case > 0: //wrong unless the extra parameters have default value
                            for (int i = min - 1; i < expectedTypes.Length && typesAsExpected; i++) typesAsExpected = expectedTypes[i].HasDefaultValue;
                            break;
                    }

                    if (typesAsExpected) {
                        c = candidate;
                        return true;
                    }
                }

                c = null;
                return false;

                bool OneTypeOk(Type? argType, Type parType) {
                    bool b;
                    if (argType == null)
                        b = parType.IsClass ||
                            parType.IsInterface ||
                            Nullable.GetUnderlyingType(parType) != null;
                    else b = parType.IsAssignableFrom(argType);
                    return b;
                }

                bool TypesAsExpected(int min, Type?[] types, ParameterInfo[] expectedTypes) {
                    var typesAsExpected = true;
                    for (int i = 0; i < min && typesAsExpected; i++) {
                        typesAsExpected = OneTypeOk(types[i], expectedTypes[i].ParameterType);
                    }

                    return typesAsExpected;
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


        static void Main(string[] args) {
            var path = @"..\..\..\..\MyLibrary\bin\Debug\net8.0\MyLibrary.dll";
            var a = Assembly.LoadFrom(path);
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
            Console.WriteLine("Alternative approach: Execute me chooses its constructor");
           AlternativeMain();
            /*
             * GetInstance sort of works, but its design goes against logical expectations:
             * If using default value creates a usable object, why the constructor without parameters
             * is missing?
             * To use it replace o = Activator.CreateInstance(t); by o = GetInstance(t);
             *
             */
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
                        for (int i=0;i< parameterInfos.Length;i++) {
                            var p = parameterInfos[i];
                            if (p.HasDefaultValue) {
                                arguments[i] = p.DefaultValue;
                            } else goto ConstructorLoop;
                        }
                        return c.Invoke(null, arguments);
                    }
                }
                catch (Exception e) {
                    Console.WriteLine($"Something else went wrong ({e})");
                }
                return result;
            }

        }
    }
}
