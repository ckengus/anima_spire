using UnityEngine;

[DisallowMultipleComponent]
public sealed class AndroidSystemUIController : MonoBehaviour
{
    private void Awake()
    {
        ApplySystemUI();
    }

    private void Start()
    {
        ApplySystemUI();
        Invoke(nameof(ApplySystemUI), 0.25f);
        Invoke(nameof(ApplySystemUI), 1f);
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            ApplySystemUI();
        }
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus)
        {
            ApplySystemUI();
        }
    }

    private void ApplySystemUI()
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

            activity.Call("runOnUiThread", new AndroidJavaRunnable(ApplySystemUIOnUiThread));
        }
        catch (System.Exception exception)
        {
            Debug.LogWarning($"AndroidSystemUIController could not apply system UI settings: {exception.Message}");
        }
#endif
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    private static void ApplySystemUIOnUiThread()
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

            window.Call("clearFlags", flagFullscreen);
            window.Call("clearFlags", flagTranslucentStatus);
            window.Call("addFlags", flagDrawsSystemBarBackgrounds);
            decorView.Call("setSystemUiVisibility", systemUiFlagLayoutStable | systemUiFlagLayoutFullscreen);
            window.Call("setStatusBarColor", 0);
        }
        catch (System.Exception exception)
        {
            Debug.LogWarning($"AndroidSystemUIController could not apply system UI settings on UI thread: {exception.Message}");
        }
    }
#endif
}
