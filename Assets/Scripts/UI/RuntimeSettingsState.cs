using UnityEngine;

public static class RuntimeSettingsState
{
    public static bool AudioEnabled { get; private set; } = true;
    public static bool HapticsEnabled { get; private set; } = true;

    public static void SetAudioEnabled(bool isEnabled)
    {
        AudioEnabled = isEnabled;
        AudioListener.volume = AudioEnabled ? 1f : 0f;
        // TODO: Persist audio settings in the Phase 4 save/progression system.
    }

    public static void SetHapticsEnabled(bool isEnabled)
    {
        HapticsEnabled = isEnabled;
        // TODO: Wire this to a real haptics service and persist it in Phase 4.
    }

    public static void ToggleAudio()
    {
        SetAudioEnabled(!AudioEnabled);
    }

    public static void ToggleHaptics()
    {
        SetHapticsEnabled(!HapticsEnabled);
    }
}
