using UnityEngine;

namespace Sequence.Demo.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Color Scheme", menuName = "Sequence/Color Scheme")]
    public class ColorScheme : ScriptableObject
    {
        public Color backgroundColor;
        public Color textColor;
        public Color buttonColor;
    }
}