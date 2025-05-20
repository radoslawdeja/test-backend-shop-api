using Microsoft.Extensions.Primitives;
using System.Security.Cryptography;

namespace Test.Shop.Api.AppStart.FileProvider
{
    /// <summary>
    /// A custom implementation of IChangeToken that uses a timer to detect file changes by computing and comparing file checksums.
    /// </summary>
    public sealed class ConfigMapFileProviderChangeToken : IChangeToken, IDisposable
    {
        /// <summary>
        /// A list of all registered change callbacks.
        /// </summary>
        List<CallbackRegistration> registeredCallbacks;

        /// <summary>
        /// Root directory for file monitoring.
        /// </summary>
        private readonly string rootPath;

        /// <summary>
        /// The relative path to the file being monitored.
        /// </summary>
        private readonly string filter;

        /// <summary>
        /// How frequently the file is checked (in milliseconds).
        /// </summary>
        private readonly int detectChangeIntervalMs;

        /// <summary>
        /// Timer used to periodically check the file.
        /// </summary>
        private Timer timer;

        /// <summary>
        /// Indicates whether the file has changed.
        /// </summary>
        private bool hasChanged;

        /// <summary>
        /// Stores the most recent checksum to compare changes.
        /// </summary>
        private string lastChecksum;

        /// <summary>
        /// Synchronization object for timer initialization/disposal.
        /// </summary>
        private readonly object timerLock = new();

        /// <summary>
        /// Initializes the change token with the file to monitor and the polling interval.
        /// </summary>
        /// <param name="rootPath"></param>
        /// <param name="filter"></param>
        /// <param name="detectChangeIntervalMs"></param>
        public ConfigMapFileProviderChangeToken(string rootPath, string filter, int detectChangeIntervalMs = 30_000)
        {
            Console.WriteLine($"new {nameof(ConfigMapFileProviderChangeToken)} for {filter}");
            registeredCallbacks = new List<CallbackRegistration>();
            this.rootPath = rootPath;
            this.filter = filter;
            this.detectChangeIntervalMs = detectChangeIntervalMs;
        }

        /// <summary>
        /// Starts the timer for periodic file checking if it’s not already started.
        /// </summary>
        internal void EnsureStarted()
        {
            lock (timerLock)
            {
                if (timer == null)
                {
                    var fullPath = Path.Combine(rootPath, filter);
                    if (File.Exists(fullPath))
                    {
                        timer = new Timer(CheckForChanges);
                        timer.Change(0, detectChangeIntervalMs);
                    }
                }
            }
        }

        /// <summary>
        /// Called by the timer to check whether the file content has changed based on checksum comparison.
        /// </summary>
        /// <param name="state"></param>
        private void CheckForChanges(object state)
        {
            var fullPath = Path.Combine(rootPath, filter);

            Console.WriteLine($"Checking for changes in {fullPath}");

            var newCheckSum = GetFileChecksum(fullPath);
            var newHasChangesValue = false;
            if (lastChecksum != null && lastChecksum != newCheckSum)
            {
                Console.WriteLine($"File {fullPath} was modified!");

                // changed
                NotifyChanges();

                newHasChangesValue = true;
            }

            hasChanged = newHasChangesValue;
            lastChecksum = newCheckSum;
        }

        /// <summary>
        /// Invokes all registered callbacks to notify them of the detected change.
        /// </summary>
        private void NotifyChanges()
        {
            var localRegisteredCallbacks = registeredCallbacks;
            if (localRegisteredCallbacks != null)
            {
                var count = localRegisteredCallbacks.Count;
                for (int i = 0; i < count; i++)
                {
                    localRegisteredCallbacks[i].Notify();
                }
            }
        }

        /// <summary>
        /// Computes and returns the MD5 checksum of the file's contents.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private static string GetFileChecksum(string filename)
        {
            using var md5 = MD5.Create();
            using var stream = File.OpenRead(filename);

            return BitConverter.ToString(md5.ComputeHash(stream));
        }

        #region IChangeToken, IDisposable

        /// <summary>
        /// Returns true if a change was detected.
        /// </summary>
        public bool HasChanged => hasChanged;

        /// <summary>
        /// Always returns true to indicate that callbacks are supported.
        /// </summary>
        public bool ActiveChangeCallbacks => true;

        /// <summary>
        /// Disposes of the timer and prevents further change detection or callback execution.
        /// </summary>
        public void Dispose()
        {
            Interlocked.Exchange(ref registeredCallbacks, null);

            Timer localTimer = null;
            lock (timerLock)
            {
                localTimer = Interlocked.Exchange(ref timer, null);
            }

            if (localTimer != null)
            {
                localTimer.Dispose();
            }
        }

        /// <summary>
        /// Registers a callback to be triggered when the file changes.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        /// <exception cref="ObjectDisposedException"></exception>
        public IDisposable RegisterChangeCallback(Action<object> callback, object state)
        {
            var localRegisteredCallbacks = registeredCallbacks;
            if (localRegisteredCallbacks == null)
                throw new ObjectDisposedException(nameof(registeredCallbacks));

            var cbRegistration = new CallbackRegistration(callback, state, (cb) => localRegisteredCallbacks.Remove(cb));
            localRegisteredCallbacks.Add(cbRegistration);

            return cbRegistration;
        }

        #endregion
    }

    internal class CallbackRegistration : IDisposable
    {
        private object state;
        private Action<object> callback;
        private Action<CallbackRegistration> unregister;

        public CallbackRegistration(Action<object> callback, object state, Action<CallbackRegistration> unregister)
        {
            this.callback = callback;
            this.state = state;
            this.unregister = unregister;
        }

        public void Notify()
        {
            var localState = state;
            var localCallback = callback;
            localCallback?.Invoke(localState);
        }

        public void Dispose()
        {
            var localUnregister = Interlocked.Exchange(ref unregister, null);
            if (localUnregister != null)
            {
                localUnregister(this);
                callback = null;
                state = null;
            }
        }
    }
}
