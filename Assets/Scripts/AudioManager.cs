using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources & Clips")]
    [Space(14)]
    public AudioMixer MasterMixer;
    public AudioMixerGroup UIMixer;
    public AudioMixerGroup  EffectsMixer;
    public AudioMixerGroup  MusicMixer;
    public AudioMixerGroup  WinningMixer;
    public AudioSource UISource;
    public AudioSource EffectsSource;
    public AudioSource MusicSource;
    public AudioSource WinningsSource;
    public GameManager GameManager;
    public AudioClip[] CheeringCrowd;
    public Slider[] AudioSliders; // 0 - Master / 1 - UI / 2 - Effects / 3 - Music / 4 - Win

    private static AudioManager _instance;
    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<AudioManager>();
            }

            return _instance;
        }
    }

    public enum Sources
    {
        Master = 0,
        UI = 1,
        Effects = 2,
        Music = 3,
        Winnings = 4
    }

    void Start()
    {
        GameManager = GameManager.Instance;
        // Initialize slider values
        foreach (var slider in AudioSliders)
        {
            slider.minValue = -80f; // Minimum attenuation volume
            slider.maxValue = 0f;   // Maximum attenuation volume
            slider.value = 0f;      // Set initial value
        }
    }

    #region Sound Effects & Play Clips
    public void Cheer()
    {
        int rnd = Random.Range(0, CheeringCrowd.Length);
        EffectsSource.PlayOneShot(CheeringCrowd[rnd]);
    }

    public void Play(AudioClip clip, Sources source)
    {
        float rndPitch = Random.Range(0.9f, 1f);
        switch (source)
        {
            case Sources.UI:
                UISource.pitch = rndPitch;
                UISource.PlayOneShot(clip);
                break;
            case Sources.Effects:
                EffectsSource.pitch = rndPitch;
                EffectsSource.PlayOneShot(clip);
                break;
            case Sources.Music:
                MusicSource.PlayOneShot(clip);
                break;
            case Sources.Winnings:
                WinningsSource.PlayOneShot(clip);
                break;
        }
    }

    // Update the volume of the audio mixers based on the slider values
    public void SetMasterVol(float vol)
    {
        MasterMixer.SetFloat("MasterVolume", vol);
    }
        public void SetUIVol(float vol)
    {
        UIMixer.audioMixer.SetFloat("UIVolume", vol);
    }
        public void SetEffectsVol(float vol)
    {
        EffectsMixer.audioMixer.SetFloat("EffectsVolume", vol);
    }
        public void SetMusicVol(float vol)
    {
        MusicMixer.audioMixer.SetFloat("MusicVolume", vol);
    }
        public void SetWinningsVol(float vol)
    {
        WinningMixer.audioMixer.SetFloat("WinningVolume", vol);
    }
    #endregion
}
