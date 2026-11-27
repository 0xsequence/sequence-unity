using System;
using System.IO;
using Sequence.Config;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Sequence.DeviceTest.Editor
{
    /// <summary>
    /// Builds the DualSessionDeviceTest scene as an Android APK.
    ///
    ///   Unity -batchmode -projectPath . -buildTarget Android -executeMethod Sequence.DeviceTest.Editor.BuildDualSessionDeviceTest.Android
    ///
    /// Changes the project it runs in, so run it in a copy: writes the keys below into Assets/Resources/SequenceConfig.asset,
    /// turns on StoreSessionPrivateKeyInSecureStorage, switches signing to the debug keystore (ProjectSettings), and the SDK's
    /// CheckUrlScheme build step adds the URL scheme to Assets/Plugins/Android/AndroidManifest.xml.
    /// Android secure storage needs AndroidKeyBridge.java; in this repo the package's Samples symlink already includes it
    /// (a game importing the package imports the "Android Secure Storage" sample instead).
    /// Env: SEQ_WAAS_CONFIG_KEY, SEQ_BUILDER_KEY, SEQ_GOOGLE_CLIENT_ID, SEQ_URL_SCHEME (each optional, overrides the config),
    ///      DEVICE_TEST_APK (output path, default Builds/dual-session-test.apk).
    /// </summary>
    public static class BuildDualSessionDeviceTest
    {
        private const string ScenePath = "Assets/DualSessionDeviceTest/DualSessionDeviceTest.unity";

        public static void Android()
        {
            var config = AssetDatabase.LoadAssetAtPath<SequenceConfig>("Assets/Resources/SequenceConfig.asset");
            if (config == null)
            {
                Fail("Assets/Resources/SequenceConfig.asset not found");
                return;
            }
            config.WaaSConfigKey = Env("SEQ_WAAS_CONFIG_KEY") ?? config.WaaSConfigKey;
            config.BuilderAPIKey = Env("SEQ_BUILDER_KEY") ?? config.BuilderAPIKey;
            config.GoogleClientId = Env("SEQ_GOOGLE_CLIENT_ID") ?? config.GoogleClientId;
            config.UrlScheme = Env("SEQ_URL_SCHEME") ?? config.UrlScheme;
            config.StoreSessionPrivateKeyInSecureStorage = true;
            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
            Debug.Log($"[DeviceTestBuild] UrlScheme='{config.UrlScheme}' GoogleClientId set={!string.IsNullOrEmpty(config.GoogleClientId)}");

            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            new GameObject("DualSessionDeviceTest").AddComponent<DualSessionDeviceTest>();
            EditorSceneManager.SaveScene(scene, ScenePath);

            string output = Env("DEVICE_TEST_APK") ?? "Builds/dual-session-test.apk";
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(output)));
            EditorUserBuildSettings.buildAppBundle = false;
            // The project points at a release keystore that isn't in the repo; sign with Unity's debug key.
            PlayerSettings.Android.useCustomKeystore = false;
            BuildReport report = BuildPipeline.BuildPlayer(new[] { ScenePath }, output, BuildTarget.Android, BuildOptions.Development);
            Debug.Log($"[DeviceTestBuild] result {report.summary.result}, errors {report.summary.totalErrors}, output {Path.GetFullPath(output)}");
            EditorApplication.Exit(report.summary.result == BuildResult.Succeeded ? 0 : 1);
        }

        private static string Env(string name)
        {
            string value = Environment.GetEnvironmentVariable(name);
            return string.IsNullOrEmpty(value) ? null : value;
        }

        private static void Fail(string message)
        {
            Debug.LogError("[DeviceTestBuild] " + message);
            EditorApplication.Exit(1);
        }
    }
}
