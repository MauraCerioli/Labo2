using System.Linq.Expressions;
using System.Reflection;
using MyAttribute;

namespace Executer {
    internal class Program {
        static void Main(string[] args) {
            //tirare assembly in memoria
            var path = @"..\..\..\..\MyLibrary\bin\Debug\net8.0\MyLibrary.dll";
            var a = Assembly.LoadFrom(path);
           TypeLoop: foreach (var t in a.GetTypes()) {
                if (!t.IsClass) continue;
                foreach (var m in t.GetMethods()) {
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
                            Console.WriteLine($"Wrong arguments' number ({m.GetParameters().Length} expected, {e.Arguments.Length} received");
                        }
                        catch (ArgumentException argEx) {
                            Console.WriteLine($"Unacceptable Arguments ({argEx.Message}");
                        }
                        catch (Exception exception) {
                            Console.WriteLine($"Something went wrong ({exception.Message})");
                        }
                    }
                }
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
