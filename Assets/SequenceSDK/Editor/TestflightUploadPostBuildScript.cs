using System;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.Diagnostics;
using System.IO;
using Debug = UnityEngine.Debug;

namespace Sequence.Editor
{
    public class TestflightUploadPostBuildScript : IPostprocessBuildWithReport
    {
        public int callbackOrder => 999;

        public void OnPostprocessBuild(BuildReport report)
        {
            if (report.summary.platform != BuildTarget.iOS ||
                Environment.GetEnvironmentVariable("ENABLE_TESTFLIGHT_UPLOAD") != "TRUE")
                return;

            var exportPath = report.summary.outputPath;
            var ipaPath = Path.GetFullPath(Path.Combine(exportPath, "../.build/last/ios-production"));
            var ipaFile = Path.Combine(ipaPath, "build.ipa");
            
            Debug.Log($"Uploading iOS binary to TestFlight from {ipaFile}");

            var keyId = Environment.GetEnvironmentVariable("APPSTORE_CONNECT_KEY_ID");
            var issuerId = Environment.GetEnvironmentVariable("APPSTORE_CONNECT_ISSUER_ID");
            var privateKey = Environment.GetEnvironmentVariable("APPSTORE_CONNECT_P8");
            privateKey = privateKey?.Replace("\\n", "\n");
            
            var keyDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".appstoreconnect/private_keys");
            
            Directory.CreateDirectory(keyDir);
            var keyPath = Path.Combine(keyDir, $"AuthKey_{keyId}.p8");
            File.WriteAllText(keyPath, privateKey);

            var cmd = $"xcrun altool --upload-app -f \"{ipaFile}\" -t ios --apiKey {keyId} --apiIssuer {issuerId}";
            RunCommand("/bin/bash", $"ls {ipaPath}");
            RunCommand("/bin/bash", $"-c \"{cmd}\"");
        }

        static void RunCommand(string cmd, string args)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = cmd,
                    Arguments = args,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            Debug.Log(process.StandardOutput.ReadToEnd());
            Debug.LogError(process.StandardError.ReadToEnd());
            process.WaitForExit();
        }
    }
}