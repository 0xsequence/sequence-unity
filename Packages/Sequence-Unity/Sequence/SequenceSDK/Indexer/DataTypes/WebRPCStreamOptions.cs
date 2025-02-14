using System;

namespace Sequence
{
    public class WebRPCStreamOptions<T>
    {
        public Action<T> onMessage;
        public Action<WebRPCError> onError;
        public Action onOpen;
        public Action onClose;

        public WebRPCStreamOptions(Action<T> onMessage, Action<WebRPCError> onError, Action onOpen = null, Action onClose = null)
        {
            this.onMessage = onMessage;
            this.onError = onError;
            this.onOpen = onOpen;
            this.onClose = onClose;
        }
    }
}