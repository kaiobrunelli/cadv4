using System;

namespace PlataformaOperacional.Service.Cobranca.Async
{
    public abstract record AsyncValue<T>
    {
        private AsyncValue() { }

        public sealed record Idle : AsyncValue<T>;

        public sealed record Loading(T? Previous = default) : AsyncValue<T>;

        public sealed record Success(T Value) : AsyncValue<T>;

        public sealed record Failure(Exception Error, T? LastGood = default) : AsyncValue<T>;


        public bool IsIdle    => this is Idle;
        public bool IsLoading => this is Loading;
        public bool IsSuccess => this is Success;
        public bool IsFailure => this is Failure;

        public T? GetValueOrDefault() => this switch
        {
            Success s              => s.Value,
            Loading l              => l.Previous,
            Failure f              => f.LastGood,
            _                      => default
        };

        public TResult Match<TResult>(
            Func<TResult> onIdle,
            Func<T?, TResult> onLoading,
            Func<T, TResult> onSuccess,
            Func<Exception, T?, TResult> onFailure) => this switch
            {
                Idle              => onIdle(),
                Loading l         => onLoading(l.Previous),
                Success s         => onSuccess(s.Value),
                Failure f         => onFailure(f.Error, f.LastGood),
                _                 => throw new InvalidOperationException("Estado impossível")
            };
    }
}
