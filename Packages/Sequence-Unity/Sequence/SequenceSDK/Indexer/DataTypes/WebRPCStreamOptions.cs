using System;

namespace Sequence
{
    public class WebRPCStreamOptions<T>
    {
        public Action<T> onMessage;
        public Action<WebRPCError> onError;

        public WebRPCStreamOptions(Action<T> onMessage, Action<WebRPCError> onError)
        {
            this.onMessage = onMessage;
            this.onError = onError;
        }
    }
}