using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    [System.Serializable]
    public class SoundClip
    {
        public string id;
        public AudioClip clip;
        public float volume = 1f;
    }

    [SerializeField] private List<SoundClip> soundClips = new();
    private Dictionary<string, AudioClip> soundDictionary = new();
    private AudioSource audioSource;

    public void Initialize()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        // Build dictionary
        foreach (var sound in soundClips)
        {
            if (!soundDictionary.ContainsKey(sound.id))
            {
                soundDictionary[sound.id] = sound.clip;
            }
        }

        // For MVP, we'll use placeholder sounds
        if (soundDictionary.Count == 0)
        {
            Debug.Log("No audio clips configured. Audio will be placeholder beeps.");
        }
    }

    public void PlaySound(string soundId)
    {
        if (soundDictionary.ContainsKey(soundId) && soundDictionary[soundId] != null)
        {
            audioSource.PlayOneShot(soundDictionary[soundId]);
        }
        else
        {
            // Placeholder: play default UI sound
            PlayPlaceholderSound(soundId);
        }
    }

    private void PlayPlaceholderSound(string soundId)
    {
        // In a real game, these would be actual audio clips
        // For MVP, we just log that sound would play
        Debug.Log($"[Audio] Sound '{soundId}' would play here");
    }

    public void StopSound()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = Mathf.Clamp01(volume);
    }

    public bool IsMuted()
    {
        return audioSource.volume <= 0f;
    }

    // Sound IDs for MVP
    public static class SoundIds
    {
        public const string SALE = "sale";
        public const string CASH = "cash";
        public const string COMBO = "combo";
        public const string UPGRADE = "upgrade";
        public const string MISS = "miss";
        public const string TRUCK_LOOP = "truck_loop";
        public const string BUTTON_TAP = "button_tap";
        public const string UNLOCK = "unlock";
    }
}
