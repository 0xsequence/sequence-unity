using UnityEditor;
using UnityEngine;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Sequence.Sidekick.Config;
public static class SidekickDockerUtility
{
    private static SidekickConfig config;

    private static string SidekickPath
    {
        get
        {
            if (config == null)
            {
                config = Resources.Load<SidekickConfig>("SidekickConfig");
                if (config == null)
                {
                    UnityEngine.Debug.LogError("Could not load SidekickConfig from Resources.");
                    return string.Empty;
                }
            }
            return config.SidekickPath;
        }
    }

    private static string dockerDesktopPath
    {
        get
        {
            if (config == null)
            {
                config = Resources.Load<SidekickConfig>("SidekickConfig");
                if (config == null)
                {
                    UnityEngine.Debug.LogError("Could not load SidekickConfig from Resources.");
                    return string.Empty;
                }
            }
            return config.DockerDesktopPath;
        }
    }


    [MenuItem("Sequence Sidekick/Start", false, 0)]
    private static void StartSidekick()
    {
        if (IsSidekickRunning()) return;

        EnsureDockerDesktopRunning();

        RunCommand("pnpm docker:restart", SidekickPath);
    }

    [MenuItem("Sequence Sidekick/Start", true)]
    private static bool ValidateStart() => !IsSidekickRunning();

    [MenuItem("Sequence Sidekick/Stop", false, 1)]
    private static void StopSidekick()
    {
        if (!IsSidekickRunning()) return;

        RunCommand("pnpm docker:stop", SidekickPath);
    }

    [MenuItem("Sequence Sidekick/Stop", true)]
    private static bool ValidateStop() => IsSidekickRunning();

    private static void RunCommand(string command, string workingDirectory)
    {
        if (string.IsNullOrEmpty(workingDirectory) || !Directory.Exists(workingDirectory))
        {
            UnityEngine.Debug.LogError($"Sidekick path not set or invalid: {workingDirectory}");
            return;
        }

        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = $"/c {command}",
            WorkingDirectory = workingDirectory,
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        Process process = new Process { StartInfo = psi };

        process.OutputDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
                UnityEngine.Debug.Log($"[Docker] {e.Data}");
        };

        process.ErrorDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
                UnityEngine.Debug.LogWarning($"[Docker] {e.Data}");
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
    }
    private static void EnsureDockerDesktopRunning()
    {
        const int maxWaitTimeSeconds = 120;
        const int pollIntervalMs = 2000;

        System.Threading.Tasks.Task.Run(() =>
        {
            if (!Process.GetProcessesByName("Docker Desktop").Any())
            {
                if (File.Exists(config.DockerDesktopPath))
                {
                    Process.Start(config.DockerDesktopPath);
                }
                else
                {
                    UnityEngine.Debug.LogWarning("[Docker] Docker Desktop not found at Sidekick Config set path.");
                    return;
                }
            }

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            while (stopwatch.Elapsed.TotalSeconds < maxWaitTimeSeconds)
            {
                if (IsDockerDaemonReady())
                {
                    UnityEngine.Debug.Log("[Docker] Docker is ready.");
                    return;
                }

                System.Threading.Thread.Sleep(pollIntervalMs);
            }

            UnityEngine.Debug.LogError("[Docker] Timed out waiting for Docker to become ready.");
        });
    }

    private static bool IsDockerDaemonReady()
    {
        try
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/c docker info",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                UseShellExecute = false
            };

            using (Process process = Process.Start(psi))
            {
                process.WaitForExit(3000);

                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                return process.ExitCode == 0 && output.Contains("Server Version");
            }
        }
        catch
        {
            return false;
        }
    }

    private static bool IsSidekickRunning()
    {
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/c docker ps --format \"{{.Names}}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = Process.Start(psi))
            {
                process.WaitForExit();
                var output = process.StandardOutput.ReadToEnd();

                return output.Contains("sidekick"); 
            }
        }
        catch
        {
            return false;
        }
    }
}
