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
        currentMode = SystemUIMode.ImmersiveFullscreen;
        ApplySystemUI(SystemUIMode.ImmersiveFullscreen);
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
            using AndroidJavaObject window = activity.Call<AndroidJavaObject>("getWindow");
            using AndroidJavaObject decorView = window.Call<AndroidJavaObject>("getDecorView");

            const int flagFullscreen = 0x00000400;
            const int flagDrawsSystemBarBackgrounds = unchecked((int)0x80000000);
            const int flagTranslucentStatus = 0x04000000;
            const int systemUiFlagLayoutStable = 0x00000100;
            const int systemUiFlagLayoutFullscreen = 0x00000400;
            const int systemUiFlagLayoutHideNavigation = 0x00000200;
            const int systemUiFlagFullscreen = 0x00000004;
            const int systemUiFlagHideNavigation = 0x00000002;
            const int systemUiFlagImmersiveSticky = 0x00001000;

            window.Call("clearFlags", flagFullscreen);
            window.Call("clearFlags", flagTranslucentStatus);
            window.Call("addFlags", flagDrawsSystemBarBackgrounds);
            window.Call("setStatusBarColor", 0);

            int visibility = mode == SystemUIMode.ImmersiveFullscreen
                ? systemUiFlagLayoutStable
                    | systemUiFlagLayoutFullscreen
                    | systemUiFlagLayoutHideNavigation
                    | systemUiFlagFullscreen
                    | systemUiFlagHideNavigation
                    | systemUiFlagImmersiveSticky
                : systemUiFlagLayoutStable | systemUiFlagLayoutFullscreen;

            decorView.Call("setSystemUiVisibility", visibility);
        }
        catch (System.Exception exception)
        {
            Debug.LogWarning($"AndroidSystemUIController could not apply system UI settings on UI thread: {exception.Message}");
        }
    }
#endif
}
