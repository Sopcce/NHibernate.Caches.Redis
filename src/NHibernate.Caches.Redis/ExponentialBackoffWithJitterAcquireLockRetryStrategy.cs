using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NHibernate.Caches.Redis
{
    /// <summary>
    /// 
    /// </summary>
    public class ExponentialBackoffWithJitterAcquireLockRetryStrategy : IAcquireLockRetryStrategy
    {
     
        private static readonly INHibernateLogger Log = NHibernateLogger.For(typeof(ExponentialBackoffWithJitterAcquireLockRetryStrategy));


        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <param name="attempt"></param>
        /// <param name="sleep"></param>
        public delegate void BackoffEventHandler(ShouldRetryAcquireLockArgs args, int attempt, int sleep);

        public event BackoffEventHandler Backoff;

        private const int SleepBase = 5;
        private const int SleepMax = 500;

        public ShouldRetryAcquireLock GetShouldRetry()
        {
            var firstAttempt = DateTime.UtcNow;
            var attempt = 0;
            // Ensure a unique seed per-thread.
            var random = new Random(Guid.NewGuid().GetHashCode());
            var onBackoff = Backoff;

            return (ShouldRetryAcquireLockArgs args) =>
            {
                attempt++;

                var hasNotTimedOut = DateTime.UtcNow - firstAttempt < args.AcquireLockTimeout;

                if (hasNotTimedOut)
                {
                    // Use an exponential backoff with jitter (randomness) to
                    // prevent multiple concurrent retries and reduce work.
                    //
                    // This is the "Full Jitter" algorithm from:
                    // http://www.awsarchitectureblog.com/2015/03/backoff.html
                    // https://github.com/awslabs/aws-arch-backoff-simulator/blob/master/src/backoff_simulator.py
                    // However, the algorithm is modified to use a minimum sleep
                    // instead of "0" to prevent a wasted sleep.
                    var v = (int)Math.Min(SleepMax, Math.Pow(2, attempt) * SleepBase);
                    var sleep = random.Next(SleepBase, v);

                    if (Log.IsDebugEnabled())
                    {
                        Log.Debug("sleep back off for {0}ms", sleep);
                    }

                    if (onBackoff != null)
                    {
                        onBackoff(args, attempt, sleep);
                    }

                    Thread.Sleep(sleep);
                }

                var shouldRetry = hasNotTimedOut;
                return shouldRetry;
            };
        }
    }
}
