using System;

namespace Flux.SDK.DataTableAPI.DatatableTypes
{
    /// <summary>Report progress to the specified UpperLimit.</summary>
    [Obsolete("Use progress stream to track progress.")]
    public class ProgressReporter
    {
        private const int MAX_VALUE = 100;
        private int upperLimit;

        /// <summary>Reports progress change.</summary>
        public Action<int> OnProgressChanged;

        /// <summary>Initializes new ProgressReporter instance</summary>
        /// <param name="upperLimit">The max value of the Progress.</param>
        public ProgressReporter(int upperLimit)
        {
            this.upperLimit = upperLimit;
        }

        /// <summary>Finalizes Progress reporter. Reports max progress value (100).</summary>
        public void Finalize()
        {
            Report(MAX_VALUE);
        }
        
        /// <summary>The max value of the Progress.</summary>
        public int UpperLimit
        {
            get { return upperLimit; }
        }

        /// <summary>Reports a progress update.</summary>
        /// <param name="value">The value of the updated progress.</param>
        public void Report(int value)
        {
            RaiseOnProgressChanged(value);
        }

        /// <summary>Reports a progress update.</summary>
        /// <param name="position">The current position in the progress stream</param>
        /// <param name="length">The length of the progress stream</param>
        public void Report(long position, long length)
        {
            var percent = (int)(UpperLimit * position / length);
            RaiseOnProgressChanged(percent);
        }

        private void RaiseOnProgressChanged(int i)
        {
            if (OnProgressChanged != null)
                OnProgressChanged(i);
        }
    }
}
