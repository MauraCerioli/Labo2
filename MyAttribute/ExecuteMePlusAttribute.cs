namespace MyAttribute {
    [AttributeUsage(AttributeTargets.Method,AllowMultiple = true)]
    public class ExecuteMeAttribute:Attribute {
        public object?[] Arguments { get; }
        public ExecuteMeAttribute(params object?[] arguments) {
            Arguments = arguments;
        }
    }
    public class ExecuteMePlusAttribute : ExecuteMeAttribute {
        public object?[]? ConstructorArguments { get; }

        public ExecuteMePlusAttribute(object?[]? constructorArguments, params object?[] arguments):base(arguments) {
            ConstructorArguments = constructorArguments;
        }
        //public ExecuteMePlusAttribute(params object?[] arguments) : this(null,arguments) { }
    }
}
