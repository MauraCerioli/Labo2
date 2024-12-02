using MyAttribute;

namespace MyLibrary {
    public class TheClass {
        private const int ctorArg = 42;
        public TheClass(int x = 8) {

        }
        [ExecuteMePlus(null, new object?[] { 3, "pip", "f" })]
        [ExecuteMePlus(new Object?[] { ctorArg }, new Object?[] { 0, new string[] { "pip", "f" } })]
        [ExecuteMePlus(new Object?[] { ctorArg }, new Object?[] { 1,  "pip", "f" })]
        [ExecuteMePlus(new Object?[] { ctorArg },  2, "pip", "f" )]
        [ExecuteMePlus(new Object?[] { ctorArg }, new Object?[] { 3,  "pip" })]
        [ExecuteMePlus(new Object?[] { ctorArg }, new Object?[] { "pip", "f" })]
        public void MM1(int x = 9, params string[] whatever) {
            Console.WriteLine($"MM1 x={x}, whatever = {whatever}");
            var sdg = new ExecuteMePlusAttribute(null, new object?[] { 3, "pip", "f" });
        }
    }
    public class Foo {
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
        public MyClass(int x=8) {

        }
        [ExecuteMePlus(null, new object?[]{3,"pip","f"})]
        [ExecuteMePlus(new Object?[]{ctorArg}, new Object?[]{3, "pip", "f"})]
        public void MM1(int x = 9, params string[] whatever) {
            Console.WriteLine($"MM1 x={x}, whatever = {whatever}");
            var sdg = new ExecuteMePlusAttribute(null, new object?[] { 3, "pip", "f" });
        }
        [ExecuteMePlus(new Object?[] { ctorArg })]
        public void MM2() {
            Console.WriteLine($"MM2");
        }
    }
        public class MyLibrary {
            public MyLibrary() { Console.WriteLine("Creating a MyLibrary"); }
            [ExecuteMePlus(null,1)]
            [ExecuteMePlus(null, 0, 7)]
            [ExecuteMePlus(null, 2)]
            [ExecuteMePlus(null)]
            [ExecuteMePlus(null,3)]
            [ExecuteMePlus(null, "pippo")]
            [ExecuteMePlus(null, 4)]
            public void M2(int a) {
                Console.WriteLine("M2 a={0} with ExecuteMePlus without constructor arguments", a);
            }

        }
    }


