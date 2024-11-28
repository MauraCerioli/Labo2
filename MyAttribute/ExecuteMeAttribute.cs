namespace MyAttribute {
    [AttributeUsage(AttributeTargets.Method,AllowMultiple = true)]
    public class ExecuteMeAttribute:Attribute {
        public object[] Arguments { get; }
        public ExecuteMeAttribute(params object[] arguments) {
            Arguments = arguments;
        }
    }
}
