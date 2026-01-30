using UnityEngine;

public static class FrameRateInitializer
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        Debug.Log($"帧率已设置为{Application.targetFrameRate}FPS");
    }
}
