using UnityEngine;

namespace Sequence.Sidekick.Config
{
    [CreateAssetMenu(fileName = "SidekickConfig", menuName = "Sequence/SidekickConfig")]
    public class SidekickConfig : ScriptableObject
    {
        [Tooltip("Your secret key from your Sidekick project")]
        public string SecretKey;

        [Tooltip("The local file path to the cloned Sidekick repository on your machine. For example: 'C:/Projects/sidekick'.")]
        public string SidekickPath;

        [Tooltip("The local installation path to Docker Desktop. For example: 'C:/Program Files/Docker/Docker'.")]
        public string DockerDesktopPath;
    }
}
