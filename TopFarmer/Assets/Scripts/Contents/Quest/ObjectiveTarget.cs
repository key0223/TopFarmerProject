public abstract class ObjectiveTarget
{
    public abstract object Value { get; }
    public abstract bool IsEquals(object target);
}
