using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Sequence.Editor
{
    internal class SequenceWarningPopup : EditorWindow
    {
        private class Warning
        {
            public string WarningMessage;
            public string RelatedDocsUrl;

            public Warning(string warningMessage, string relatedDocsUrl)
            {
                WarningMessage = warningMessage;
                RelatedDocsUrl = relatedDocsUrl;
            }
        }
        
        private static List<Warning> warnings = new List<Warning>();

        public static void ShowWindow(List<string> warningsToShow, string docsUrl)
        {
            if (warningsToShow == null || warningsToShow.Count == 0)
            {
                return;
            }

            List<Warning> warningsToAdd = new List<Warning>();
            foreach (var warning in warningsToShow)
            {
                warningsToAdd.Add(new Warning(warning, docsUrl));
            }

            if (warnings.Count > 0)
            {
                warnings.AddRange(warningsToAdd);
            }
            else
            {
                warnings = warningsToAdd;
            }
            var window = GetWindow<SequenceWarningPopup>("Sequence Build Warnings");
            window.position = new Rect(Screen.width / 2, Screen.height / 2, 400, 200);
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.Label("Warnings Detected", EditorStyles.boldLabel);

            foreach (Warning warning in warnings)
            {
                EditorGUILayout.HelpBox(warning.WarningMessage, MessageType.Warning);

                if (GUILayout.Button("Learn More", GUILayout.Width(100)))
                {
                    Application.OpenURL(warning.RelatedDocsUrl);
                }
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Dismiss", GUILayout.Height(30)))
            {
                warnings = new List<Warning>();
                Close();
            }
        }
    }
}