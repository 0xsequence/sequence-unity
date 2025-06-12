using UnityEngine;

[CreateAssetMenu(fileName = "SidekickConfig", menuName = "Sequence/SidekickConfig")]
public class SidekickConfig : ScriptableObject
{
    public string secretKey;
    public string sidekickPath;
    public string dockerDesktopPath;
}