using MyAttribute;

namespace MyLibrary {
    public class Experiment {
        public void M0(TheClass refTypeByValue, out TheClass refTypeByRef, in int numberByIn, ref bool boolByRef,
            int optional = 73, params Foo[] last) {
            refTypeByRef = new TheClass(42);
            var byIn = 77;
            var bbb = true;
            M0(new TheClass(4), out var xxx, in byIn, ref bbb);
        }
    }
    public class TheClass {
        private const int ctorArg = 42;
        public TheClass(int x = 8) {

        }
        [ExecuteMePlus(null, new object?[] { 3, "pip", "f" })]
        [ExecuteMePlus(new Object?[] { ctorArg }, new Object?[] { 0, new string[] { "pip", "f" } })]
        [ExecuteMePlus(new Object?[] { ctorArg }, 1, "pip", "f")]
        [ExecuteMePlus(new Object?[] { ctorArg }, 2, "pip", "f")]
        [ExecuteMePlus(new Object?[] { ctorArg }, new Object?[] { 3, "pip" })]
        [ExecuteMePlus(new Object?[] { ctorArg }, new Object?[] { "pip", "f" })]
        public void MM1(int x, params string[] whatever) {
            Console.WriteLine($"MM1 x={x}, whatever = {string.Join(",", whatever)}");
        }
    }
    public class Foo {
        [ExecuteMe(1, "pip", "f")]
        public void M0(int x = 9, params string[] whatever) {
            Console.WriteLine($"M0 x={x}, whatever = {string.Join(",", whatever)}");
        }
        [ExecuteMe()]
        public void MDefault(int x = 9) {
            Console.WriteLine($"MDefault x={x}");
        }
        [ExecuteMe]
        public void M1() {
            Console.WriteLine("M1");
        }

        [ExecuteMe(1)]
        [ExecuteMe(0, 7)]
        [ExecuteMe(2)]
        [ExecuteMe()]
        [ExecuteMe(3)]
        [ExecuteMe("pippo")]
        [ExecuteMe(4)]
        public void M2(int a) {
            Console.WriteLine("M2 a={0}", a);
        }

        [ExecuteMe("hello", "reflection")]
        public void M3(string s1, string s2) {
            Console.WriteLine("M3 s1={0} s2={1}", s1, s2);
        }
    }

    public class MyClass {
        private const int ctorArg = 42;
        public MyClass(int x = 8) {

        }
        [ExecuteMePlus(null, new object?[] { 3, "pip", "f" })]
        [ExecuteMePlus(new Object?[] { ctorArg }, new Object?[] { 3, "pip", "f" })]
        public void MM1(int x = 9, params string[] whatever) {
            Console.WriteLine($"MM1 x={x}, whatever = {whatever}");
            var sdg = new ExecuteMePlusAttribute(null, new object?[] { 3, "pip", "f" });
        }
        [ExecuteMePlus(new Object?[] { ctorArg })]
        public void MM2() {
            Console.WriteLine($"MM2");
        }
    }

}


