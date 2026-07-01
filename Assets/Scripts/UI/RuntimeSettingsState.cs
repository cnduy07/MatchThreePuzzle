using UnityEngine;

public static class RuntimeSettingsState
{
    static bool s_isInitialized;
    static bool s_audioEnabled = true;
    static bool s_hapticsEnabled = true;

    public static bool AudioEnabled
    {
        get
        {
            EnsureInitialized();
            return s_audioEnabled;
        }
    }

    public static bool HapticsEnabled
    {
        get
        {
            EnsureInitialized();
            return s_hapticsEnabled;
        }
    }

    public static void EnsureInitialized()
    {
        if (s_isInitialized)
        {
            return;
        }

        PlayerProgress.EnsureLoaded();
        s_audioEnabled = PlayerProgress.AudioEnabled;
        s_hapticsEnabled = PlayerProgress.HapticsEnabled;
        ApplyAudioVolume();
        s_isInitialized = true;
    }

    public static void SetAudioEnabled(bool isEnabled)
    {
        EnsureInitialized();
        s_audioEnabled = isEnabled;
        ApplyAudioVolume();
        PlayerProgress.SetAudioEnabled(s_audioEnabled);
    }

    public static void SetHapticsEnabled(bool isEnabled)
    {
        EnsureInitialized();
        s_hapticsEnabled = isEnabled;
        PlayerProgress.SetHapticsEnabled(s_hapticsEnabled);
        // TODO: Wire this persisted setting to a real haptics service.
    }

    public static void ToggleAudio()
    {
        EnsureInitialized();
        SetAudioEnabled(!s_audioEnabled);
    }

    public static void ToggleHaptics()
    {
        EnsureInitialized();
        SetHapticsEnabled(!s_hapticsEnabled);
    }

    static void ApplyAudioVolume()
    {
        AudioListener.volume = s_audioEnabled ? 1f : 0f;
    }
}
