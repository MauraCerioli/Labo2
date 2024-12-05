using MyAttribute;

namespace AnotherLibrary {
    /*
    public class NoConstructorParams {
        public NoConstructorParams() { Console.WriteLine("Creating an OkInvocationsNoConstructorParams"); }
        [ExecuteMePlus(null, 1)]
        [ExecuteMePlus(null, 2)]
        [ExecuteMePlus(null, 0, 7)]
        [ExecuteMePlus(null)]
        [ExecuteMePlus(null, "pippo")]
        public void OneParameterSameType(int a) {
            Console.WriteLine($"{nameof(OneParameterSameType)} a={a} with ExecuteMePlus without constructor arguments");
        }
        [ExecuteMePlus(null, 1)]
        public void OneParameterSuperType(int? a) {
            Console.WriteLine($"{nameof(OneParameterSuperType)} a={a} with ExecuteMePlus without constructor arguments");
        }
        [ExecuteMePlus(null, true, new[] { 42, 73 })]
        [ExecuteMePlus(null, false, 11, 22)]
        [ExecuteMePlus(null, false, 0, "pippo")]
        [ExecuteMePlus(null, false)]
        public void NonOptionalAndParams(bool a, params int[] anyNumber) {
            Console.WriteLine($"{nameof(NonOptionalAndParams)} a={a} and anyNumber={((anyNumber == null) ? "null" : String.Join(',', anyNumber))} with ExecuteMePlus without constructor arguments");
        }

        [ExecuteMePlus(null, false, 6, 3.14)]
        [ExecuteMePlus(null, false, 666)]
        [ExecuteMePlus(null, false)]
        [ExecuteMePlus(null)]
        public void OnlyOptional(bool a = true, int b = 7, double c = 8.9) {
            Console.WriteLine($"{nameof(OnlyOptional)} a={a} and b={b} and c={c} with ExecuteMePlus without constructor arguments");
        }

        [ExecuteMePlus(null, 3, 5.6, false, 6, 3.14)]
        [ExecuteMePlus(null, 42, 6.7, false, 666)]
        [ExecuteMePlus(null, 111, 5.5, false)]
        [ExecuteMePlus(null, 17, 13.3)]
        public void MandatoryAndOptional(int x, double y, bool a = true, int b = 7, double c = 8.9) {
            Console.WriteLine($"{nameof(MandatoryAndOptional)} x={x}, y={y}, a={a} and b={b} and c={c} with ExecuteMePlus without constructor arguments");
        }

        [ExecuteMePlus(null, 17, true)]
        public int ByRef(int x, out bool b) {
            Console.WriteLine($"{nameof(ByRef)} x={x} with ExecuteMePlus without constructor arguments");
            b = true;
            return 42;
        }
    }
    */
    public class ConstructorParams {
        public ConstructorParams(int x) {
            Console.WriteLine($"Constructor with one int parameter = {x}");
        }
        public ConstructorParams(bool b) {
            Console.WriteLine($"Constructor with one bool parameter = {b}");
        }
        public ConstructorParams(bool b, int n=42, double d=3.14, params string[] s) {
            Console.WriteLine($"Constructor with parameters b = {b}, n={n}, d={d}, s={((s==null)?"null":string.Join(',',s))}");
        }
        [ExecuteMePlus(new object[]{37})]
        [ExecuteMePlus(new object[] { true })]
        [ExecuteMePlus(new object[] { false, 3 })]
        [ExecuteMePlus(new object[] { false, 31,7.7 })]
        [ExecuteMePlus(new object[] { false,333,6.5,"ciao","ciao","bambina" })]
        public void M(){Console.WriteLine("M");}
        [ExecuteMePlus(new object[] { 37 },4)]
        [ExecuteMePlus(new object[] { true },24)]
        [ExecuteMePlus(new object[] { false, 3 },435)]
        [ExecuteMePlus(new object[] { false, 31, 7.7 },0)]
        [ExecuteMePlus(new object[] { false, 333, 6.5, "ciao", "ciao", "bambina" },-4)]
        public void M1(int z) { Console.WriteLine($"M1 with z={z}"); }
    }
}
