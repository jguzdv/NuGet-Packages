namespace JGUZDV.CQRS
{
    /// <summary>
    /// Flags a class as a "Decorator".
    /// Decorators won't be automatically registered as ICommandHandler&lt;&gt; or IQueryHandler&lt;&gt; during DI setup.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class CQRSDecoratorAttribute : Attribute { }
}
