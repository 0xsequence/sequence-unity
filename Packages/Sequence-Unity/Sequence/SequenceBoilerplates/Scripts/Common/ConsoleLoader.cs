using UnityEngine;

namespace Sequence.Boilerplates
{
    // Let's instantiate the IngameDebugConsole from Resources only if it exists.
    public class ConsoleLoader : MonoBehaviour
    {
        private void Start()
        {
            var consolePrefab = Resources.Load<GameObject>("IngameDebugConsole");
            if (!consolePrefab)
                return;
            
            Instantiate(consolePrefab);
        }
    }
}
