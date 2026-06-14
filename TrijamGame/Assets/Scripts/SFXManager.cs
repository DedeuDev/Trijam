using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    [Header("Settings")]
    [SerializeField] private float sfxVolume = 1f;

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

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.volume = sfxVolume;
        audioSource.spatialBlend = 0f;
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;

        audioSource.PlayOneShot(clip, sfxVolume);
    }

    public void PlaySFX(AudioClip clip, float volumeMultiplier)
    {
        if (clip == null) return;

        audioSource.PlayOneShot(clip, sfxVolume * volumeMultiplier);
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;

        if (audioSource != null)
        {
            audioSource.volume = sfxVolume;
        }
    }
}