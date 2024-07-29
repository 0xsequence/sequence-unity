using System;

namespace Sequence.WaaS.DataTypes
{
    public class TimeMismatchException : Exception
    {
        public long CurrentTime { get; private set;  }
        
        public TimeMismatchException(string message, long currentTime) : base(message)
        {
            CurrentTime = currentTime;
        }
    }
}