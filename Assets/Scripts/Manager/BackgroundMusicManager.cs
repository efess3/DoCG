using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundMusicManager : MonoBehaviour
{
    private const string GameplaySceneName = "GameScene";
    private static BackgroundMusicManager instance;

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
