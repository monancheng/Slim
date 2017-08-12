using UnityEngine;

[ExecuteInEditMode]
public class ScreenOptions : MonoBehaviour
{
    public int FrameRate = 60;
    public float GameHeight = 9f;
    public float GameWidth = 16f;
    public bool IsChangeCameraSize = true;
    public float GameAspect { get; private set; }

    public static float ScreenWidth { get; private set; }
    public static float ScreenHeight { get; private set; }
    public static float ScreenAspect { get; private set; }

    public static float ScreenWidthAspect { get; private set; }
    public static float ScreenHeightAspect { get; private set; }

    private void Awake()
    {
        SetScreenSettings();
        if (IsChangeCameraSize) SetScreenSize();
    }

#if UNITY_EDITOR
    private void OnRenderObject()
    {
        if (Application.isPlaying) return;
        SetScreenSettings();
        if (IsChangeCameraSize) SetScreenSize();
    }
#endif

    private void SetScreenSettings()
    {
        //Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.targetFrameRate = FrameRate;
    }

    private void SetScreenSize()
    {
        GameAspect = GameWidth / GameHeight;
        ScreenAspect = Camera.main.aspect;

        if (ScreenAspect > GameAspect)
        {
            ScreenWidth = GameHeight * ScreenAspect;
            ScreenHeight = GameHeight;
        }
        else
        {
            ScreenWidth = GameWidth;
            ScreenHeight = GameWidth / ScreenAspect;
        }

        ScreenWidthAspect = ScreenWidth / GameWidth;
        ScreenHeightAspect = ScreenHeight / GameHeight;

        Camera.main.orthographicSize = ScreenHeight / 2f;
    }
}