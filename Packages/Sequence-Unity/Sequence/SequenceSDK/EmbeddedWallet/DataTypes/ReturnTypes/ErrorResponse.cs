using Sequence.Utils;

namespace Sequence.EmbeddedWallet
{
    [Preserve]
    public class ErrorResponse
    {
        public string error;
        public int code;
        public string msg;
        public string cause;
        public int status;

        [Preserve]
        public ErrorResponse(string error, int code, string msg, string cause, int status)
        {
            this.error = error;
            this.code = code;
            this.msg = msg;
            this.cause = cause;
            this.status = status;
        }
    }
}