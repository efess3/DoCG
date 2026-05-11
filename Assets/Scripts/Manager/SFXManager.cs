using UnityEngine;
using DoCG.UI;

/// <summary>
/// Manages SFX volume via a static property.
/// Persists across scenes with DontDestroyOnLoad.
///
/// HOW TO USE YOUR SFX AUDIOSOURCES:
///   Instead of: audioSource.PlayOneShot(clip);
///   Use:        SFXManager.Play(audioSource, clip);
///
///   Or set the volume before playing:
///   audioSource.volume = SFXManager.Volume;
///   audioSource.Play();
/// </summary>
public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }

    // Current SFX volume (0.0 – 1.0). Read this when playing any SFX.
    public static float Volume { get; private set; } = 1f;

    private const string SFXKey = "Volume_SFX";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Create()
    {
        if (Instance != null) return;
        var go = new GameObject(nameof(SFXManager));
        Instance = go.AddComponent<SFXManager>();
        DontDestroyOnLoad(go);
    }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Load saved volume
        Volume = PlayerPrefs.GetFloat(SFXKey, 1f);

        // Subscribe to VolumeControl static event
        VolumeControl.OnVolumeChanged += OnVolumeChanged;
    }

    private void OnDestroy()
    {
        VolumeControl.OnVolumeChanged -= OnVolumeChanged;
    }

    private void OnVolumeChanged(VolumeControl.VolumeChannel channel, float value)
    {
        if (channel == VolumeControl.VolumeChannel.SFX)
        {
            Volume = value;
        }
    }

    // ---------------------------------------------------------------
    // Convenience helpers — use these anywhere you play SFX
    // ---------------------------------------------------------------

    /// <summary>Plays a clip at the correct SFX volume.</summary>
    public static void Play(AudioSource source, AudioClip clip)
    {
        if (source == null || clip == null) return;
        source.volume = Volume;
        source.PlayOneShot(clip);
    }

    /// <summary>Plays a clip at a position in world space.</summary>
    public static void PlayAtPoint(AudioClip clip, Vector3 position)
    {
        if (clip == null) return;
        AudioSource.PlayClipAtPoint(clip, position, Volume);
    }
}
