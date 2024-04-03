#if UNITY_IOS
using AppleAuth.Editor;
#endif
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

namespace Sequence.Editor
{
    public static class SignInWithApplePostprocessor
    {
#if UNITY_IOS // From https://github.com/lupidan/apple-signin-unity?tab=readme-ov-file#programmatic-setup-with-a-script
        [PostProcessBuild(1)]
        public static void OnPostProcessBuild(BuildTarget target, string path)
        {
            if (target != BuildTarget.iOS)
                return;

            var projectPath = PBXProject.GetPBXProjectPath(path);
        
            // Adds entitlement depending on the Unity version used
#if UNITY_2019_3_OR_NEWER
            var project = new PBXProject();
            project.ReadFromString(System.IO.File.ReadAllText(projectPath));
            var manager = new ProjectCapabilityManager(projectPath, "Entitlements.entitlements", null, project.GetUnityMainTargetGuid());
            manager.AddSignInWithAppleWithCompatibility(project.GetUnityFrameworkTargetGuid());
            manager.WriteToFile();
#else
            var manager = new ProjectCapabilityManager(projectPath, "Entitlements.entitlements", PBXProject.GetUnityTargetName());
            manager.AddSignInWithAppleWithCompatibility();
            manager.WriteToFile();
#endif
        }
#endif
    }
}