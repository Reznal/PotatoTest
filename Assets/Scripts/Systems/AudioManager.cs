using UnityEngine;
using PotatoFarm.Core;

namespace PotatoFarm.Systems
{
    public class AudioManager : MonoBehaviour
    {
        [Header("Audio Sources")]
        public AudioSource musicSource;
        public AudioSource sfxSource;
        
        [Header("Music")]
        public AudioClip backgroundMusic;
        public float musicVolume = 0.7f;
        
        [Header("Sound Effects")]
        public AudioClip tapSound;
        public AudioClip upgradeSound;
        public AudioClip prestigeSound;
        public AudioClip processSound;
        public AudioClip cashSound;
        public AudioClip eventSound;
        public float sfxVolume = 0.8f;
        
        public static AudioManager Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            
            SetupAudioSources();
        }
        
        private void Start()
        {
            PlayBackgroundMusic();
            SubscribeToEvents();
        }
        
        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }
        
        private void SetupAudioSources()
        {
            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
            }
            
            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
            }
            
            musicSource.loop = true;
            musicSource.volume = musicVolume;
            
            sfxSource.loop = false;
            sfxSource.volume = sfxVolume;
        }
        
        private void SubscribeToEvents()
        {
            if (GameManager.Instance?.farmManager != null)
            {
                GameManager.Instance.farmManager.OnPotatoHarvested += OnPotatoHarvested;
            }
            
            if (GameManager.Instance?.upgradeManager != null)
            {
                GameManager.Instance.upgradeManager.OnUpgradePurchased += OnUpgradePurchased;
            }
            
            if (GameManager.Instance?.prestigeManager != null)
            {
                GameManager.Instance.prestigeManager.OnPrestigePerformed += OnPrestigePerformed;
            }
        }
        
        private void UnsubscribeFromEvents()
        {
            if (GameManager.Instance?.farmManager != null)
            {
                GameManager.Instance.farmManager.OnPotatoHarvested -= OnPotatoHarvested;
            }
            
            if (GameManager.Instance?.upgradeManager != null)
            {
                GameManager.Instance.upgradeManager.OnUpgradePurchased -= OnUpgradePurchased;
            }
            
            if (GameManager.Instance?.prestigeManager != null)
            {
                GameManager.Instance.prestigeManager.OnPrestigePerformed -= OnPrestigePerformed;
            }
        }
        
        public void PlayBackgroundMusic()
        {
            if (backgroundMusic != null && musicSource != null)
            {
                musicSource.clip = backgroundMusic;
                musicSource.Play();
            }
        }
        
        public void PlaySFX(AudioClip clip)
        {
            if (clip != null && sfxSource != null)
            {
                sfxSource.PlayOneShot(clip);
            }
        }
        
        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            if (musicSource != null)
            {
                musicSource.volume = musicVolume;
            }
        }
        
        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            if (sfxSource != null)
            {
                sfxSource.volume = sfxVolume;
            }
        }
        
        public void ToggleMusic()
        {
            if (musicSource != null)
            {
                if (musicSource.isPlaying)
                {
                    musicSource.Pause();
                }
                else
                {
                    musicSource.UnPause();
                }
            }
        }
        
        public void ToggleSFX()
        {
            sfxSource.mute = !sfxSource.mute;
        }
        
        // Event handlers
        private void OnPotatoHarvested(double amount)
        {
            PlaySFX(tapSound);
        }
        
        private void OnUpgradePurchased(int upgradeIndex)
        {
            PlaySFX(upgradeSound);
        }
        
        private void OnPrestigePerformed()
        {
            PlaySFX(prestigeSound);
        }
        
        public void PlayProcessSound()
        {
            PlaySFX(processSound);
        }
        
        public void PlayCashSound()
        {
            PlaySFX(cashSound);
        }
        
        public void PlayEventSound()
        {
            PlaySFX(eventSound);
        }
        
        // Create default audio clips programmatically for demonstration
        private void CreateDefaultAudioClips()
        {
            // In a real project, you would assign actual audio files
            // This creates simple tone clips for demonstration
            
            tapSound = CreateSimpleTone(0.1f, 800f);
            upgradeSound = CreateSimpleTone(0.3f, 1200f);
            prestigeSound = CreateSimpleTone(0.5f, 1600f);
            processSound = CreateSimpleTone(0.2f, 600f);
            cashSound = CreateSimpleTone(0.15f, 1000f);
            eventSound = CreateSimpleTone(0.4f, 1400f);
        }
        
        private AudioClip CreateSimpleTone(float duration, float frequency)
        {
            int sampleRate = 44100;
            int samples = Mathf.RoundToInt(duration * sampleRate);
            float[] audioData = new float[samples];
            
            for (int i = 0; i < samples; i++)
            {
                float time = (float)i / sampleRate;
                audioData[i] = Mathf.Sin(2 * Mathf.PI * frequency * time) * 0.1f;
            }
            
            AudioClip clip = AudioClip.Create("GeneratedTone", samples, 1, sampleRate, false);
            clip.SetData(audioData, 0);
            return clip;
        }
        
        // Create simple background music
        private void CreateBackgroundMusic()
        {
            float duration = 30f; // 30 second loop
            int sampleRate = 44100;
            int samples = Mathf.RoundToInt(duration * sampleRate);
            float[] audioData = new float[samples];
            
            // Create a simple melody
            float[] frequencies = { 440f, 493.88f, 523.25f, 587.33f, 659.25f }; // A, B, C, D, E
            
            for (int i = 0; i < samples; i++)
            {
                float time = (float)i / sampleRate;
                int noteIndex = Mathf.FloorToInt((time % duration) / (duration / frequencies.Length));
                float frequency = frequencies[noteIndex];
                
                audioData[i] = Mathf.Sin(2 * Mathf.PI * frequency * time) * 0.05f;
                
                // Add some harmony
                audioData[i] += Mathf.Sin(2 * Mathf.PI * frequency * 1.5f * time) * 0.03f;
                
                // Add gentle fade in/out for each note
                float noteTime = (time % (duration / frequencies.Length)) / (duration / frequencies.Length);
                float envelope = Mathf.Sin(Mathf.PI * noteTime);
                audioData[i] *= envelope;
            }
            
            backgroundMusic = AudioClip.Create("BackgroundMusic", samples, 1, sampleRate, false);
            backgroundMusic.SetData(audioData, 0);
        }
        
        [ContextMenu("Create Default Audio")]
        public void CreateDefaultAudio()
        {
            CreateDefaultAudioClips();
            CreateBackgroundMusic();
            Debug.Log("Default audio clips created!");
        }
    }
}