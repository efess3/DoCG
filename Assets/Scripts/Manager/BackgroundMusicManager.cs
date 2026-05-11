using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundMusicManager : MonoBehaviour
{
    private const string GameplaySceneName = "GameScene";

    // Must match SettingsToggle.settingKey on the EnableSounds toggle pair
    public const string EnableSoundsKey = "Setting_EnableSounds";

    private static BackgroundMusicManager instance;

    // Cached volume values
    private float masterVolume = 1f;
    private float musicVolume  = 1f;

    private readonly string[] musicPaths =
    {
        "Music/Worn_Path1",
        "Music/Worn_Path2"
    };

    private AudioClip[] tracks;
    private AudioSource audioSource;
    private Coroutine musicQueue;
    private int currentTrackIndex;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Create()
    {
        EnsureInstance();
    }

    public static void PlayGameplayMusic()
    {
        EnsureInstance();
        instance.StartMusic();
    }

    private static void EnsureInstance()
    {
        if (instance != null) return;

        GameObject musicObject = new GameObject(nameof(BackgroundMusicManager));
        instance = musicObject.AddComponent<BackgroundMusicManager>();
        DontDestroyOnLoad(musicObject);
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = false;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
        audioSource.volume = 0.1f;

        LoadTracks();

        // Apply saved setting immediately on creation
        ApplySavedSoundSetting();
        ApplySavedVolumes();

        // Subscribe to toggle (mute) changes
        DoCG.UI.SettingsToggle.OnAnySettingChanged += OnSettingChanged;

        // Subscribe to volume knob changes
        DoCG.UI.VolumeControl.OnVolumeChanged += OnVolumeChannelChanged;
    }

    private void OnDestroy()
    {
        DoCG.UI.SettingsToggle.OnAnySettingChanged -= OnSettingChanged;
        DoCG.UI.VolumeControl.OnVolumeChanged      -= OnVolumeChannelChanged;
    }

    private void OnSettingChanged(string key, bool value)
    {
        if (key == EnableSoundsKey)
            SetSoundsEnabled(value);
    }

    private void OnVolumeChannelChanged(DoCG.UI.VolumeControl.VolumeChannel channel, float value)
    {
        switch (channel)
        {
            case DoCG.UI.VolumeControl.VolumeChannel.Master:
                masterVolume = value;
                // Master volume drives AudioListener — respect mute state
                bool soundsOn = PlayerPrefs.GetInt(EnableSoundsKey, 1) == 1;
                AudioListener.volume = soundsOn ? masterVolume : 0f;
                break;

            case DoCG.UI.VolumeControl.VolumeChannel.Music:
                musicVolume = value;
                if (audioSource != null)
                    audioSource.volume = musicVolume * 0.1f; // 0.1 is the base music level
                break;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != GameplaySceneName)
        {
            StopMusic();
        }

        // Re-apply the saved sound setting on every scene load.
        // This ensures sounds are muted/unmuted correctly in both Menu and Game scenes.
        ApplySavedSoundSetting();
    }

    /// <summary>
    /// Mutes or unmutes ALL game audio (music + SFX + attacks + everything).
    /// Called automatically by SettingsToggle.OnSettingChanged when the user
    /// clicks EnableSoundsOn or EnableSoundsOff in the settings panel.
    /// Uses AudioListener.volume which is a global multiplier for every AudioSource.
    /// </summary>
    public void SetSoundsEnabled(bool isEnabled)
    {
        // When unmuting, restore the saved master volume (not hardcoded 1f)
        AudioListener.volume = isEnabled ? masterVolume : 0f;
    }

    private void ApplySavedSoundSetting()
    {
        bool soundsEnabled = PlayerPrefs.GetInt(EnableSoundsKey, 1) == 1;
        AudioListener.volume = soundsEnabled ? masterVolume : 0f;
    }

    private void ApplySavedVolumes()
    {
        masterVolume = PlayerPrefs.GetFloat("Volume_Master", 1f);
        musicVolume  = PlayerPrefs.GetFloat("Volume_Music",  1f);

        bool soundsOn = PlayerPrefs.GetInt(EnableSoundsKey, 1) == 1;
        AudioListener.volume = soundsOn ? masterVolume : 0f;

        if (audioSource != null)
            audioSource.volume = musicVolume * 0.1f;
    }

    private void StartMusic()
    {
        if (musicQueue != null || tracks == null || tracks.Length == 0) return;

        currentTrackIndex = Random.Range(0, tracks.Length);
        musicQueue = StartCoroutine(PlayMusicQueue());
    }

    private void StopMusic()
    {
        if (musicQueue != null)
        {
            StopCoroutine(musicQueue);
            musicQueue = null;
        }

        if (audioSource != null)
        {
            audioSource.Stop();
            audioSource.clip = null;
        }
    }

    private void LoadTracks()
    {
        tracks = new AudioClip[musicPaths.Length];

        for (int i = 0; i < musicPaths.Length; i++)
        {
            tracks[i] = Resources.Load<AudioClip>(musicPaths[i]);

            if (tracks[i] == null)
            {
                Debug.LogWarning($"Background music clip not found at Resources/{musicPaths[i]}");
            }
        }
    }

    private IEnumerator PlayMusicQueue()
    {
        while (true)
        {
            AudioClip track = tracks[currentTrackIndex];

            if (track != null)
            {
                audioSource.clip = track;
                audioSource.Play();

                yield return new WaitWhile(() => audioSource.isPlaying);
            }

            currentTrackIndex = (currentTrackIndex + 1) % tracks.Length;
            yield return null;
        }
    }
}
