using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    const float PITCH_SHIFT_THRESHOLD = 1.5f;
    const float PITCH_SHIFT = 0.05f;
    public enum SoundEffectType{
        PlayOnce,
        Loop,
        StartLoopEnd,
        Random,
        PitchShift,
        Index
    }
    public static AudioManager instance;
    [SerializeField] AudioSource musicSource = null;
    public enum Track{
        MainMenu,
        Gameplay,
    }
    [SerializeField,Tooltip("Must have the same order as Track enum in AudioManager")] AudioClip[] tracks = null;

    float trackPlaybackTime;
    public SoundEffect[] soundEffects = null;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
            foreach (SoundEffect s in soundEffects)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clips[0];
                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
                if (s.type == SoundEffectType.Loop)
                    s.source.loop = true;
                else
                    s.source.loop = false;
                s.source.playOnAwake = false;
            }
        }
        else
        {
            DestroyImmediate(gameObject);
            return;
        }
    }

    void Start()
    {
        if(GameStateManager.instance)
        {
            SetSoundEffectsActive(GameStateManager.instance.GetSoundsEnabled());
            SetMusicActive(GameStateManager.instance.GetMusicEnabled());
        }
        else
        {
            SetSoundEffectsActive(true);
            SetMusicActive(true);
        }

        PlayMusic(Track.MainMenu);
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.S))
        //StopSoundEffect("SportsCar");
        //if (Input.GetKeyDown(KeyCode.P))
            //PlaySoundEffect("Iap");
    }

    public void PlayMusic(Track track,bool resumeLastTrack = false)
    {
        trackPlaybackTime = musicSource.time;
        musicSource.clip = tracks[(int)track];
        if(resumeLastTrack)
        {
            musicSource.time = trackPlaybackTime;
        }
        musicSource.Play();

    }

    public void PlayMusic(int index, bool resumeLastTrack = false)
    {
        if(index<0)
        {
            musicSource.Stop();
            return;
        }

        trackPlaybackTime = musicSource.time;
        musicSource.clip = tracks[index];
        if (resumeLastTrack)
        {
            musicSource.time = trackPlaybackTime;
        }
        musicSource.Play();
    }

    public void PlaySoundEffect(string name, int index)
    {
        SoundEffect s = System.Array.Find(soundEffects, sound => sound.name == name);
        if(s == null)
        {
            Debug.LogWarning("Sound : " + name + " not found.");
            return;
        }
        if(s.type == SoundEffectType.Index)
        {
            index -= 1;
            s.source.clip = s.clips[index];
            s.source.Play();
            Debug.Log("Play sound");
        }
    }

    public void PlaySoundEffect(string name)
    {
        if (GameplayManager.instance == null)
            return;

        if(name=="Jump")
        {
            if(GameplayManager.instance.CurrentCharacterIndex==0)
            {
                name = "JumpBoy";
            }
            else if (GameplayManager.instance.CurrentCharacterIndex == 1)
            {
                name = "JumpGirl";
            }
        }

        SoundEffect s = System.Array.Find(soundEffects, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound : " + name + " not found.");
            return;
        }
        if(s.type == SoundEffectType.StartLoopEnd)
        {
            if (!s.source.isPlaying)
                StartCoroutine(StartLoopStopCoroutine(s));
            else
                Debug.LogError("Failed to Play SoundEffect");
        }
        else if(s.type == SoundEffectType.Random)
        {
            s.source.clip = s.clips[Random.Range(0, s.clips.Length)];
            s.source.Play();
        }
        else if(s.type == SoundEffectType.PitchShift)
        {
            if (Time.time >= s.lastPitchShiftTime + PITCH_SHIFT_THRESHOLD)
            {
                s.source.pitch = s.pitch;
            }
            else
            {
                s.source.pitch += PITCH_SHIFT;
            }
            s.lastPitchShiftTime = Time.time;
            s.source.Play();
        }
        else{
            s.source.Play();
        }

    }

    public void PlayAmbientSound(int currentIndex, int previousIndex)
    {
        StopSoundEffect("Piece"+previousIndex);
        PlaySoundEffect("Piece"+currentIndex);
    }

    public void SetSoundEffectVolume(string name, float volumeToSet)
    {
        SoundEffect s = System.Array.Find(soundEffects, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound : " + name + " not found.");
            return;
        }
        s.source.volume = volumeToSet;
    }

    IEnumerator StartLoopStopCoroutine(SoundEffect s){
        s.source.clip = s.clips[0];
        s.source.loop = false;
        s.source.Play();
        yield return new WaitWhile(() => s.source.isPlaying);
        s.source.loop = true;
        s.source.clip = s.clips[1];
        s.source.Play();
    }

    public void StopAllSounds()
    {
        //StopSoundEffect("CopSiren");
        //StopSoundEffect("SportsCar");
        //StopSoundEffect("Headstart");
        StopAllCoroutines();
        SetSoundEffectsActive(false);
        SetMusicActive(false);
    }

    public void ResumeAllSounds()
    {
        SetSoundEffectsActive(GameStateManager.instance.GetSoundsEnabled());
        SetMusicActive(GameStateManager.instance.GetMusicEnabled());
    }

    public void StopSoundEffect(string name)
    {
        SoundEffect s = System.Array.Find(soundEffects, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound : " + name + " not found.");
            return;
        }
        if (s.type == SoundEffectType.StartLoopEnd)
        {
            if (s.source.loop)
            {
                s.source.clip = s.clips[2];
                s.source.loop = false;
                s.source.Play();
            }
            else{
                Debug.LogWarning("Failed to Stop SoundEffect");
            }
        }
        else
            s.source.Stop();
    
    }

    public AudioSource GetSoundEffectSource(string name)
    {
        SoundEffect s = System.Array.Find(soundEffects, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound : " + name + " not found.");
            return null;
        }
        return s.source;
    }

    public SoundEffect GetSoundEffect(string name)
    {
        SoundEffect s = System.Array.Find(soundEffects, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound : " + name + " not found.");
            return null;
        }
        return s;
    }

    public void SetMusicActive(bool isActive)
    {
        musicSource.enabled = isActive;
    }

    public void SetSoundEffectsActive(bool isActive)
    {
        for (int i = 0; i < soundEffects.Length; i++)
        {
            soundEffects[i].source.enabled = isActive;
        }
    }

    [System.Serializable]
    public class SoundEffect
    {
        public string name;
        public SoundEffectType type;
        public AudioClip[] clips;

        [Range(0f, 2f)]
        public float volume;
        [Range(0.1f, 3f)]
        public float pitch;

        [HideInInspector]
        public AudioSource source;

        [HideInInspector] public float lastPitchShiftTime;
    }


}
