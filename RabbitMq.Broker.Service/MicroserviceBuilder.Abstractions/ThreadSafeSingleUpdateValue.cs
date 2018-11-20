using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Microservice.Abstractions
{
    public class ThreadSafeSingleUpdateValue
    {
        private const int Init = 0;
        private const int Success = 1;
        private const int Failure = 2;
        private int _state = Init;
        /// <summary>Explicit call to check and set if this is the first call</summary>
        public bool CheckAndSetSuccessFirstCall => Interlocked.Exchange(ref _state, Success) == Success;

        public void UpdateValue(bool success)
        {
            Interlocked.Exchange(ref _state, success?Success:Failure);
        }

        public bool CheckAndSetFailureFirstCall => Interlocked.Exchange(ref _state, Failure) == Success;

        public bool? Successful => _state==Init?(bool?)null:_state==Success;
    }
}
