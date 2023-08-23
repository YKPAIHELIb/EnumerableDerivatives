namespace EnumerableDerivatives.Implementations;

using System.Collections;
using EnumerableDerivatives.Interfaces;

public sealed class AnyAwareEnumerable<T> : IAnyAwareEnumerable<T>
{
    private readonly IEnumerable<T> source;
    private AnyAwareEnumerator enumerator;
    private bool getEnumeratorCalled = false;

    public AnyAwareEnumerable(IEnumerable<T> source)
    {
        this.source = source ?? throw new ArgumentNullException(nameof(source));
    }

    public bool IsAny => GetOrCreateEnumerator().IsAny;

    public IEnumerator<T> GetEnumerator()
    {
        if (!getEnumeratorCalled)
        {
            getEnumeratorCalled = true;
            return GetOrCreateEnumerator();
        }
        return CreateAndOverwriteEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Dispose()
    {
        // If enumerator was not requested via GetEnumerator then it'll be disposed there
        if (!getEnumeratorCalled)
        {
            enumerator?.Dispose();
        }
    }

    private AnyAwareEnumerator GetOrCreateEnumerator() => enumerator ?? CreateAndOverwriteEnumerator();
    private AnyAwareEnumerator CreateAndOverwriteEnumerator() => enumerator = new AnyAwareEnumerator(source.GetEnumerator());


    private sealed class AnyAwareEnumerator : IEnumerator<T>
    {
        private readonly IEnumerator<T> _sourceEnumerator;
        private bool? _isAny;
        private bool _moveNextWasCalled = false;

        public AnyAwareEnumerator(IEnumerator<T> sourceEnumerator)
        {
            _sourceEnumerator = sourceEnumerator ?? throw new ArgumentNullException(nameof(sourceEnumerator));
        }

        public bool IsAny => _isAny ?? MoveNextFirstTime();

        public T Current => _moveNextWasCalled ? _sourceEnumerator.Current : default;

        object IEnumerator.Current => Current;

        private bool MoveNextFirstTime()
        {
            if (_isAny.HasValue)
            {
                return _isAny.Value;
            }

            _isAny = _sourceEnumerator.MoveNext();
            return _isAny.Value;
        }

        public bool MoveNext()
        {
            if (!_isAny.HasValue)
            {
                _moveNextWasCalled = true;
                return MoveNextFirstTime();
            }

            if (!_moveNextWasCalled)
            {
                _moveNextWasCalled = true;
                return _isAny.Value;
            }

            return _sourceEnumerator.MoveNext();
        }

        public void Reset()
        {
            _sourceEnumerator.Reset();
            _isAny = null;
            _moveNextWasCalled = false;
        }

        public void Dispose() => _sourceEnumerator.Dispose();
    }
}
