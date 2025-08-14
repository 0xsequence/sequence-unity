using UnityEngine;

namespace Sequence.Utils
{
    public static class ChromeTabs
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        private const string PluginClass = "xyz.sequence.ChromeTabsPlugin";
#endif
        /// <summary>
        /// Open a URL in Chrome Custom Tabs (Android), or Application.OpenURL elsewhere.
        /// rgb24 is optional 0xRRGGBB; will be applied as opaque toolbar color.
        /// </summary>
        public static void Open(string url, uint? rgb24 = null, bool enableShare = false, bool showTitle = true)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            int colorInt = 0; // 0 = use default theme color in Java
            if (rgb24.HasValue)
            {
                // Convert 0xRRGGBB into Android ARGB (opaque)
                colorInt = unchecked((int)(0xFF000000u | rgb24.Value));
            }

            using (var jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (var activity = jc.GetStatic<AndroidJavaObject>("currentActivity"))
            using (var plugin = new AndroidJavaClass(PluginClass))
            {
                plugin.CallStatic("openUrl", url, colorInt, enableShare, showTitle);
            }
#else
            Application.OpenURL(url);
#endif
        }
        
        public static bool IsSupported()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            using (var plugin = new AndroidJavaClass(PluginClass))
            {
                return plugin.CallStatic<bool>("isCustomTabsSupported");
            }
#else
            return false;
#endif
        }
    }
}