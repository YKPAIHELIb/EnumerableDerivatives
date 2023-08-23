namespace EnumerableDerivatives.Interfaces;
public interface IAnyAwareEnumerable<T> : IEnumerable<T>, IDisposable
{
    bool IsAny { get; }
}
