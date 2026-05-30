using UnityEngine;

[DisallowMultipleComponent]
public sealed class AndroidSystemUIController : MonoBehaviour
{
    private enum SystemUIMode
    {
        NormalEdgeToEdge,
        ImmersiveFullscreen
    }

    private SystemUIMode currentMode = SystemUIMode.NormalEdgeToEdge;

    private void Awake()
    {
        currentMode = SystemUIMode.NormalEdgeToEdge;
        ReapplyCurrentMode();
    }

    private void Start()
    {
        ReapplyCurrentMode();
        Invoke(nameof(ReapplyCurrentMode), 0.25f);
        Invoke(nameof(ReapplyCurrentMode), 1f);
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            ReapplyCurrentMode();
        }
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus)
        {
            ReapplyCurrentMode();
        }
    }

    public void ApplyNormalEdgeToEdgeMode()
    {
        currentMode = SystemUIMode.NormalEdgeToEdge;
        ApplySystemUI(SystemUIMode.NormalEdgeToEdge);
    }

    public void ApplyImmersiveFullscreenMode()
    {
        Debug.Log("AndroidSystemUIController: immersive fullscreen request ignored for 033A native gate validation.");
        currentMode = SystemUIMode.NormalEdgeToEdge;
        ApplySystemUI(SystemUIMode.NormalEdgeToEdge);
    }

    public void ReapplyCurrentMode()
    {
        ApplySystemUI(currentMode);
    }

    private void ApplySystemUI(SystemUIMode mode)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            using AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            using AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            if (activity == null)
            {
                return;
            }

            activity.Call("runOnUiThread", new AndroidJavaRunnable(() => ApplySystemUIOnUiThread(mode)));
        }
        catch (System.Exception exception)
        {
            Debug.LogWarning($"AndroidSystemUIController could not apply system UI settings: {exception.Message}");
        }
#endif
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    private static void ApplySystemUIOnUiThread(SystemUIMode mode)
    {
        try
        {
            using AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            using AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            if (activity == null)
            {
                Debug.LogWarning("AndroidSystemUIController v03: currentActivity is null on UI thread.");
                return;
            }

            using AndroidJavaObject window = activity.Call<AndroidJavaObject>("getWindow");
            using AndroidJavaObject decorView = window.Call<AndroidJavaObject>("getDecorView");
            using AndroidJavaObject activityClass = activity.Call<AndroidJavaObject>("getClass");

            int sdkInt = GetAndroidSdkInt();
            string activityClassName = activityClass.Call<string>("getName");
            const int flagFullscreen = 0x00000400;
            const int flagDrawsSystemBarBackgrounds = unchecked((int)0x80000000);
            const int flagTranslucentStatus = 0x04000000;
            const int flagTranslucentNavigation = 0x08000000;
            const int systemUiFlagLayoutStable = 0x00000100;
            const int systemUiFlagLayoutFullscreen = 0x00000400;

            Debug.Log($"AndroidSystemUIController v03: SDK_INT={sdkInt}, activity={activityClassName}, requestedMode={mode}");

            window.Call("clearFlags", flagFullscreen);
            window.Call("clearFlags", flagTranslucentStatus);
            window.Call("clearFlags", flagTranslucentNavigation);
            window.Call("addFlags", flagDrawsSystemBarBackgrounds);
            Debug.Log("AndroidSystemUIController v03: applying transparent statusBarColor.");
            window.Call("setStatusBarColor", 0);
            Debug.Log("AndroidSystemUIController v03: applying transparent navigationBarColor.");
            window.Call("setNavigationBarColor", 0);

            if (sdkInt >= 29)
            {
                Debug.Log("AndroidSystemUIController v03: disabling status/navigation bar contrast enforcement.");
                window.Call("setStatusBarContrastEnforced", false);
                window.Call("setNavigationBarContrastEnforced", false);
            }

            if (sdkInt >= 30)
            {
                Debug.Log("AndroidSystemUIController v03: applying decorFitsSystemWindows=false.");
                window.Call("setDecorFitsSystemWindows", false);
            }
            else
            {
                Debug.Log("AndroidSystemUIController v03: decorFitsSystemWindows=false skipped because SDK_INT < 30.");
            }

            int visibility = systemUiFlagLayoutStable | systemUiFlagLayoutFullscreen;
            Debug.Log($"AndroidSystemUIController v03: applying systemUiVisibility={visibility} without hiding system bars.");
            decorView.Call("setSystemUiVisibility", visibility);
        }
        catch (System.Exception exception)
        {
            Debug.LogWarning($"AndroidSystemUIController v03 could not apply system UI settings on UI thread: {exception.Message}");
        }
    }

    private static int GetAndroidSdkInt()
    {
        try
        {
            using AndroidJavaClass version = new AndroidJavaClass("android.os.Build$VERSION");
            return version.GetStatic<int>("SDK_INT");
        }
        catch (System.Exception exception)
        {
            Debug.LogWarning($"AndroidSystemUIController v03 could not read SDK_INT: {exception.Message}");
            return 0;
        }
    }
#endif
}
