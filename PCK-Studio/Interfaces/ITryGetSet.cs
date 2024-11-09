namespace PckStudio.Interfaces
{
    public delegate bool TryGetDelegate<in TKey, TValue>(TKey key, out TValue value);
    public delegate bool TrySetDelegate<in TKey, TValue>(TKey key, TValue value);

    internal sealed class TryGet<TKey, TValue> : ITryGet<TKey, TValue>
    {
        private TryGetDelegate<TKey, TValue> _tryGetDelegate;

        public static ITryGet<TKey, TValue> FromDelegate(TryGetDelegate<TKey, TValue> tryGetDelegate) => new TryGet<TKey, TValue>(tryGetDelegate);

        bool ITryGet<TKey, TValue>.TryGet(TKey key, out TValue value) => _tryGetDelegate(key, out value);

        private TryGet(TryGetDelegate<TKey, TValue> tryGetDelegate)
        {
            _tryGetDelegate = tryGetDelegate;
        }
    }

    internal sealed class TrySet<TKey, TValue> : ITrySet<TKey, TValue>
    {
        private TrySetDelegate<TKey, TValue> _trySetDelegate;

        public static ITrySet<TKey, TValue> FromDelegate(TrySetDelegate<TKey, TValue> trySetDelegate) => new TrySet<TKey, TValue>(trySetDelegate);

        bool ITrySet<TKey, TValue>.TrySet(TKey key, TValue value) => _trySetDelegate(key, value);

        private TrySet(TrySetDelegate<TKey, TValue> trySetDelegate)
        {
            _trySetDelegate = trySetDelegate;
        }
    }

    internal sealed class TryGetSet<TKey, TValue> : ITryGetSet<TKey, TValue>
    {
        public static ITryGetSet<TKey, TValue> FromDelegates(TryGetDelegate<TKey, TValue> tryGetDelegate, TrySetDelegate<TKey, TValue> trySetDelegate) => new TryGetSet<TKey, TValue>(tryGetDelegate, trySetDelegate);

        public bool TryGet(TKey key, out TValue value) => _tryGetDelegate(key, out value);

        public bool TrySet(TKey key, TValue value) => _trySetDelegate(key, value);
        
        private TryGetDelegate<TKey, TValue> _tryGetDelegate;
        private TrySetDelegate<TKey, TValue> _trySetDelegate;

        private TryGetSet(TryGetDelegate<TKey, TValue> tryGetDelegate, TrySetDelegate<TKey, TValue> trySetDelegate)
        {
            _tryGetDelegate = tryGetDelegate;
            _trySetDelegate = trySetDelegate;
        }
    }

    interface ITryGet<in TKey, TValue>
    {
        bool TryGet(TKey key, out TValue value);
    }    
    
    interface ITrySet<in TKey, TValue>
    {
        bool TrySet(TKey key, TValue value);
    }

    interface ITryGetSet<in TKey, TValue> : ITryGet<TKey, TValue>, ITrySet<TKey, TValue>
    {
    }
}
