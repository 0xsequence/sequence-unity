using System.Threading.Tasks;

namespace Sequence.Utils
{
    public static class AsyncExtensions
    {
        /// <summary>
        /// Similar to Task.Delay but is thread safe
        /// Task.Delay creates a separate thread. This makes Task.Delay incompatible with WebGL builds
        /// This method does not create a separate thread and therefore works with WebGL builds
        /// </summary>
        /// <param name="delayInSeconds"></param>
        public static async Task DelayTask(float delayInSeconds)
        {
            float currentTime = AppEnvironment.Time;
            float elapsedTime = 0;
            while (elapsedTime < delayInSeconds)
            {
                elapsedTime = AppEnvironment.Time - currentTime;
                await Task.Yield();
            }
        }
    }
}