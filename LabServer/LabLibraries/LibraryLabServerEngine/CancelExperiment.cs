using System;

namespace Library.LabServerEngine
{
    public class CancelExperiment
    {
        //
        // Local variables
        //
        private object cancelLock;

        //
        // Properties
        //
        private bool cancelled;

        public bool IsCancelled
        {
            get
            {
                lock (this.cancelLock)
                {
                    return this.cancelled;
                }
            }
        }

        //-------------------------------------------------------------------------------------------------//

        public CancelExperiment()
        {
            this.cancelLock = new object();
            this.cancelled = false;
        }

        //-------------------------------------------------------------------------------------------------//

        public void Cancel()
        {
            lock (this.cancelLock)
            {
                this.cancelled = true;
            }
        }

    }
}
