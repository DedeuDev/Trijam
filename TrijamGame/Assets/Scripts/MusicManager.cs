using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Music Clips")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip gameplayMusic;

    [Header("Scene Names")]
    [SerializeField] private string menuSceneName = "Menu";
    [SerializeField] private string gameplaySceneName = "Gameplay";

    [Header("Settings")]
    [SerializeField] private float musicVolume = 0.7f;

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();

        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = musicVolume;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    private void Start()
    {
        PlayMusicForCurrentScene();
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForCurrentScene();
    }

    private void PlayMusicForCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (currentSceneName == menuSceneName)
        {
            PlayMusic(menuMusic);
        }
        else if (currentSceneName == gameplaySceneName)
        {
            PlayMusic(gameplayMusic);
        }
    }

    private void PlayMusic(AudioClip newClip)
    {
        if (newClip == null) return;

        if (audioSource.clip == newClip && audioSource.isPlaying)
        {
            return;
        }

        audioSource.clip = newClip;
        audioSource.Play();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;

        if (audioSource != null)
        {
            audioSource.volume = musicVolume;
        }
    }
}