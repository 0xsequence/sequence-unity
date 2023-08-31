using System.Collections.Generic;
using Sequence.Demo.ScriptableObjects;
using UnityEditor;
using UnityEngine;

namespace Sequence.Demo.EditorExtensions
{
    [CustomEditor(typeof(NetworkIcons))]
    public class NetworkIconsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            NetworkIcons icons = (NetworkIcons)target;
            if (icons.NetworkIconMapping == null)
            {
                icons.NetworkIconMapping = new List<SerializableKeyValuePair<Chain, Sprite>>();
            }

            foreach (var mapping in icons.NetworkIconMapping)
            {
                EditorGUILayout.BeginHorizontal();
                mapping.Key = (Chain)EditorGUILayout.EnumPopup(mapping.Key);
                mapping.Value = (Sprite)EditorGUILayout.ObjectField(mapping.Value, typeof(Sprite), false);
                EditorGUILayout.EndHorizontal();
            }

            DrawDefaultInspector();
        }
    }
}