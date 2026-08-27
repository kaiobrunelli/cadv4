using System;
using System.Threading;
using System.Threading.Tasks;

namespace PlataformaOperacional.Service.Cobranca.Async
{
    public static class AsyncValueRunner
    {
        public static async Task RunAsync<T>(
            AsyncValue<T> current,
            Func<CancellationToken, Task<T>> load,
            Action<AsyncValue<T>> onChange,
            CancellationToken ct = default)
        {
            var previous = current.GetValueOrDefault();

            onChange(new AsyncValue<T>.Loading(previous));

            try
            {
                var value = await load(ct).ConfigureAwait(false);
                onChange(new AsyncValue<T>.Success(value));
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                onChange(previous is null
                    ? new AsyncValue<T>.Idle()
                    : new AsyncValue<T>.Success(previous));
            }
            catch (Exception ex)
            {
                onChange(new AsyncValue<T>.Failure(ex, previous));
            }
        }
    }
}
