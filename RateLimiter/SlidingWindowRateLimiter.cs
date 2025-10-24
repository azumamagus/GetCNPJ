using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GetCNPJ.Interfaces;

namespace GetCNPJ.RateLimiter
{
    /// <summary>
    /// Implementação de Rate Limiter usando Sliding Window
    /// Limita o número de requisições por intervalo de tempo
    /// </summary>
    public class SlidingWindowRateLimiter : IRateLimiter
    {
        private readonly int _maxRequests;
        private readonly TimeSpan _timeWindow;
        private readonly ConcurrentDictionary<string, Queue<DateTime>> _requestTimestamps;
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _semaphores;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="maxRequests">Número máximo de requisições no intervalo</param>
        /// <param name="timeWindow">Intervalo de tempo (padrão: 1 minuto)</param>
        public SlidingWindowRateLimiter(int maxRequests = 3, TimeSpan? timeWindow = null)
        {
            _maxRequests = maxRequests;
            _timeWindow = timeWindow ?? TimeSpan.FromMinutes(1);
            _requestTimestamps = new ConcurrentDictionary<string, Queue<DateTime>>();
            _semaphores = new ConcurrentDictionary<string, SemaphoreSlim>();
        }

        /// <inheritdoc/>
        public async Task WaitIfNeededAsync(string providerName, CancellationToken cancellationToken = default)
        {
            var semaphore = _semaphores.GetOrAdd(providerName, _ => new SemaphoreSlim(1, 1));

            await semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                var timestamps = _requestTimestamps.GetOrAdd(providerName, _ => new Queue<DateTime>());

                // Remove timestamps antigos fora da janela de tempo
                CleanOldTimestamps(timestamps);

                // Se atingiu o limite, aguarda até que uma requisição antiga expire
                while (timestamps.Count >= _maxRequests)
                {
                    var oldestRequest = timestamps.Peek();
                    var waitTime = oldestRequest.Add(_timeWindow) - DateTime.UtcNow;

                    if (waitTime > TimeSpan.Zero)
                    {
                        await Task.Delay(waitTime, cancellationToken).ConfigureAwait(false);
                    }

                    CleanOldTimestamps(timestamps);
                }
            }
            finally
            {
                semaphore.Release();
            }
        }

        /// <inheritdoc/>
        public void RecordRequest(string providerName)
        {
            var timestamps = _requestTimestamps.GetOrAdd(providerName, _ => new Queue<DateTime>());
            timestamps.Enqueue(DateTime.UtcNow);
        }

        /// <inheritdoc/>
        public void Reset(string providerName)
        {
            _requestTimestamps.TryRemove(providerName, out _);
        }

        /// <inheritdoc/>
        public int GetAvailableRequests(string providerName)
        {
            if (!_requestTimestamps.TryGetValue(providerName, out var timestamps))
                return _maxRequests;

            CleanOldTimestamps(timestamps);
            return Math.Max(0, _maxRequests - timestamps.Count);
        }

        /// <summary>
        /// Remove timestamps fora da janela de tempo
        /// </summary>
        private void CleanOldTimestamps(Queue<DateTime> timestamps)
        {
            var cutoffTime = DateTime.UtcNow.Subtract(_timeWindow);

            while (timestamps.Count > 0 && timestamps.Peek() < cutoffTime)
            {
                timestamps.Dequeue();
            }
        }
    }
}
