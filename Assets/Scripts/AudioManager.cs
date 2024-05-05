using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources & Clips")]
    [Space(14)]
    public AudioSource UISource;
    public AudioSource EffectsSource;
    public AudioSource MusicSource;
    public AudioSource WinningsSource;
    public GameManager GameManager;
    public AudioClip[] CheeringCrowd;
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
        UI = 0,
        Effects = 1,
        Music = 2,
        Winnings = 3
    }

    void Start()
    {
        GameManager = GameManager.Instance;
    }

    #region Handle Music
    public void FadeInOutMusic(){

    }

    public void PlayMusic(){
        
    }
    #endregion
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
    #endregion
}
