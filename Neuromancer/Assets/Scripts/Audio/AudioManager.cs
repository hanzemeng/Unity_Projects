using UnityEngine.Audio;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Assertions;


public class AudioManager : MonoBehaviour
{

    //User will use this enum to select music
    public enum SoundResource{
        NONE = 0,
        PLAYER_WALK = 1,
        PLAYER_WALK1 = 2,
        PLAYER_SPELL_CHARGE =3,
        PLAYER_SPELL_HIT =4,
        PLAYER_GOT_HIT= 5,
        PLAYER_DEATH = 6,
        PLAYER_SPELL_ACTIVE = 7,
        PLAYER_SWITCH_SPELL = 8,
        PLAYER_MISSILE_LAUNCH = 9,
        PLAYER_MISSILE_EXPLOSION = 10,
        PLAYER_SPELL_DENY = 11,

        NEURON_COLLECT = 1000,

        ENEMY1_GROWL = 50,
        ENEMY1_WALK1 =51,
        ENEMY1_WALK2 = 55,
        ENEMY1_ATTACK =52,
        ENEMY1_DEATH = 53,
        ENEMY1_GOT_HIT = 54,


        ENEMY_DRAGON_WALK1 = 100,
        ENEMY_DRAGON_WALK2 = 105,
        ENEMY_DRAGON_GROWL = 101,
        ENEMY_DRAGON_GOT_HIT = 102,
        ENEMY_DRAGON_DEATH = 103,
        ENEMY_DRAGON_ATTACK =104,
        ENEMY_DRAGON_SPECIAL_ATTACK =106,
        ENEMY_SPIDER_WALK = 150,
        ENEMY_SPIDER_GROWL = 151,
        ENEMY_SPIDER_DEATH = 152,
        EMEMY_SPIDER_ATTACK = 153,
        ENEMY_SPIDER_SPECIAL_ATTACK =154,
        ENEMY_SPIDER_GOT_HIT = 155,

        ENEMY_GOLEM_WALK1 = 200,
        ENEMY_GOLEM_WALK2 = 201,
        ENEMY_GOLEM_GROWL = 202,
        ENEMY_GOLEM_GOT_HIT = 203,
        ENEMY_GOLEM_DEATH = 204,
        ENEMY_GOELM_ATTACK = 205,
        ENEMY_GOLEM_SPECIAL_ATTACK = 206,


        BOSS1_WALK = 2000,
        BOSS1_GROWL = 2001,
        BOSS1_ATTACK = 2002,
        BOSS1_DEATH = 2003,

        AXORATH_PROJECTILE_1 = 2100,
        AXORATH_PROJECTILE_2 = 2101,

        CORROSIVE_SLIME_PROJECTILE = 2200,

        BGM_VILLIAGE = 3000,
        BGM_WIZARD = 3001,
        BGM_BATTLE1 = 3002,
        BGM_BOSS1 = 3003,
        BGM_HELL_TRENCH = 3004,
        BGM_SWAMP = 3005,
        BGM_VERDANT_CASTLE = 3006,
        BGM_ENDING = 3007,

        PRESSURE_PLATE_PRESSED = 3045,
        PRESSURE_PLATE_RELEASED = 3046,
        FIRE_LIT = 3047,
        FIRE_EXTINGUISH = 3048,
        CARRIABLE_PICKUP = 3049,
        CARRIABLE_DROP = 3050,
        DOOR_OPEN = 3051,
        PUZZLE_SOLVED = 3052,
        ROCK_SMASH = 3053,
        SPIKE_MOVEUP = 3054,
        SPIKE_MOVEDOWN = 3055,
        TREE_HIT = 3056,
        TREE_DESTROY = 3057,
        INTERACTABLE_DENY = 3058,
        INTERACTABLE_SUCCESS = 3059,
        KEYLOCK_CHAIN_BREAK = 3060,
        KEYLOCK_KEY_BREAK = 3061,   
        MINI_PUZZLE_SOLVED = 3062,
        TO_NEXT_SCENE_DEFAULT = 3064,


        TEXT_CRAWLING_SFX = 4000,
        UI_SELECT_SFX = 4001,
        UI_TARGET_SFX = 4002,
        STAR_POWERUPP_SFX = 4003,
        MONSTER_GROWL = 4004,
        DESERT_WIND = 4005,
        SPAWN_SMOKE = 4006,
        UI_GENERIC_CLICK_SFX = 4007,
        UI_UPGRADE_SFX = 4008,
        UI_BUFF_SFX = 4009,
        ALTAR_SFX = 4010,
        UI_PING_SFX = 4011,
        UI_DENY_SFX = 4012,
        SAVE_SFX = 4013,
        UI_DEBUFF_SFX = 4014

    }


    [SerializeField]
    private List<Sound> _sounds;

    [SerializeField]
    private Transform attachedFolder;

    [SerializeField]
    private Transform backGroundSFXFolder;


    [SerializeField]
    private Transform backGroundMusicFolder;

    [SerializeField]
    private Transform worldSpaceFolder;

    public static AudioManager instance { get; private set; }

    private Dictionary<SoundResource, GameObject> _audioDictionary;
    private Dictionary<SoundResource, Sound> _soundDictionary;

    private SoundResource currentBGM;

    [SerializeField] private AudioMixer mixer;
    public const string MUSIC_KEY = "MusicVolume";
    public const string SFX_KEY = "SFXVolume";
    public const string UI_KEY = "UIVolume";


    private void Awake()
    {
        //singleton pattern
        if (instance == null)
        {
            instance = this;
        }
        else
        {   
            Destroy(gameObject);
        }
        _audioDictionary = new Dictionary<SoundResource, GameObject>();
        _soundDictionary = new Dictionary<SoundResource, Sound>();

        //add audio to dictionary and create their settings
        foreach (var sound in _sounds)
        {
            GameObject obj = new GameObject(sound.clip.name);
            switch(sound.myType){
                case Sound.MusicType.ATTACHED:
                    obj.transform.SetParent(attachedFolder);
                    break;
                case Sound.MusicType.BACKGROUND_SFX:
                    obj.transform.SetParent(backGroundSFXFolder);
                    break;
                case Sound.MusicType.WORLDSPACE:
                    obj.transform.SetParent(worldSpaceFolder);
                    break;
                case Sound.MusicType.BACKGROUND_SOLO:
                    obj.transform.SetParent(backGroundMusicFolder);
                    break;
                default:
                    Debug.Log("The selected Music Type does not correlate with any defined type");
                    obj.transform.SetParent(transform);
                    break;
            }
            AudioSource source = obj.AddComponent<AudioSource>();
            source.clip = sound.clip;
            source.volume = sound.volume;
            source.pitch = sound.pitch;
            source.playOnAwake = sound.playOnAwake;
            source.loop = sound.loop;
            source.spatialBlend = sound.spatialBlend;
            source.outputAudioMixerGroup = sound.audioMixerGroup;
            source.dopplerLevel = sound.dopplerLevel;
            source.spread = sound.spread;
            source.rolloffMode = sound.rolloffMode;
            source.minDistance = sound.minDistance;
            source.maxDistance = sound.maxDistance;
            if (sound.pauseOnGamePause)
            {
                obj.AddComponent<AudioPauseScreenBehavior>();
            }

            if (sound.myType == Sound.MusicType.BACKGROUND_SOLO)
            {
                //only one BGM should be playonAwake, otherwise there is a problem.
                if (sound.playOnAwake)
                {
                    source.Play();
                    currentBGM = sound.token;
                }
            }

            _audioDictionary.Add(sound.token, obj);
            _soundDictionary.Add(sound.token, sound);
        }

        LoadVolume();
    }

    ///<summary>
    /// Attach a gameobject with the requested audiosource as a child of the calling gameobject.
    /// </summary>
    /// <param name="tokenName">Sound you are requesting.</param>
    /// <param name="attachedObjTransform">The transform of the calling object.</param>
    /// <returns>Return the gameobject with the requested audiosource if success. If fail, it would return null.</returns>
    public GameObject MusicAttached(SoundResource tokenName, Transform attachedObjTransform)
    {
        if (!instance._audioDictionary.ContainsKey(tokenName))
        {
            Debug.Log("Music Does Not Exist");
            return null;
        }
  
        GameObject tmp = Instantiate(instance._audioDictionary[tokenName]);
        tmp.transform.SetParent(attachedObjTransform);
        return tmp;
        
    }


    ///<summary>
    /// Play the targeted music if gameObject has audiosource component, do nothing if none are find. If multipleShot is false,
    /// then the music will only play if the clip is not playing. If multipleShot is true, the music can be played if the clip is playing.
    /// </summary>
    /// <param name="gameObject">The child gameObject that contain audisource</param>
    /// <param name="multipleShot">Optional,false if nothing is passed. multipleShot allow you to play the same clips multipletimes, NOTE: if multipleShot option is avaible, both fadeIn and fadeOut will be disabled</param>
    /// <param name="isfadeIn">Optional, set to false if nothing is passed. introduce fadeIn effect for attachedMusicType</param>
    /// <param name="fadeInDuration">Optional, <b>Only work when isfadeIn set to TRUE</b>, set to 2s second if nothing is passed. the fadein time for the selected music to reach its target volume.</param>
    /// <param name="isfadeOut">Optional, set to false if nothing is passed. introduce fadeOut effect for attachedMusicType</param>
    /// <param name="fadeoutPercent">Optional, <b>only WORK when isfadeOut set to TRUE</b>, set to 0.1 if nothing is passed. allow you to decide when will fadeout effect come in. For instance, 0.1 means that the music will fadeout when the audio reach the 90% of its clip, the fadeoutPercent value should be between 0 and 1</param>
    /// <param name="fadeoutDuration">Optional, <b>only WORK when isfadeOut set to TRUE</b>, set to 2f if nothing is passed. allow you to decide the fadeout duration when the clip is about to fadeout.</param>
    /// <param name="fadeOutModeStop">Optional, <b>only WORK when isfadeOut set to TRUE</b>, set to true if nothing is passed. when set to true, the audio will be stopped. when set to false, the audio will be paused</param>
    /// <returns>Void</returns>
    public void PlayMusic(GameObject gameObject, bool multipleShot = false, bool isfadeIn = false, float fadeInDuration = 2f, bool isfadeOut = false, float fadeoutPercent = 0.1f, float fadeoutDuration = 2f, bool fadeOutModeStop = true)
    {
        if (!gameObject) { return;  }
        AudioSource audio = gameObject.GetComponent<AudioSource>();
        if(audio == null) { Debug.Log("target does not have audio"); return; }
        if (multipleShot)
        {
            audio.PlayOneShot(audio.clip);
        }
        else
        {
            if (!audio.isPlaying)
            {

                ProcessPlayMusic(audio, isfadeIn, fadeInDuration, isfadeOut, fadeoutPercent, fadeoutDuration, fadeOutModeStop:fadeOutModeStop);
            }
        }
        
    }

    ///<summary>
    /// Pause the requested music that attach to the calling script.
    /// </summary>
    /// <param name="gameObject">the gameObject that contain audiosource</param>
    /// <returns>Void</returns>
    public void PauseMusic(GameObject gameObject, bool isfadeOut = false, float fadeoutDuration = 2f, bool fadeOutModeStop = true)
    {
        if (!gameObject) { return; }
        AudioSource audio = gameObject.GetComponent<AudioSource>();
        if (audio == null) { Debug.Log("target does not have audio"); return; }
        if (audio.isPlaying)
        {
            if (!isfadeOut)
                audio.Pause();
            else {
                FadeOutEffect(audio, fadeoutDuration, 0, fadeOutModeStop);
            }  
        }
    }

    ///<summary>
    /// Play the requested background sound effect.
    /// </summary>
    /// <param name="tokenName">Sound you are requesting to play</param>
    /// <returns>Void</returns>
    public void PlayBackgroundSFX(SoundResource tokenName) {
        if (!instance._audioDictionary.ContainsKey(tokenName))
        {
            Debug.Log("Music does not exist");
            return;
        }
        AudioSource audio = _audioDictionary[tokenName].GetComponent<AudioSource>();

        if(tokenName == SoundResource.TEXT_CRAWLING_SFX)
        {
            audio.Play();
        }
        else
        {
            audio.PlayOneShot(audio.clip);
        }
    }

    ///<summary>
    /// Pause the selected background sound effect.
    /// </summary>
    /// <param name="tokenName">Sound you are requesting to pause</param>
    /// <returns>Void</returns>
    public void PauseBackgroundSFX(SoundResource tokenName){
        if (!instance._audioDictionary.ContainsKey(tokenName))
        {
            Debug.Log("Music does not exist");
            return;
        }
        AudioSource audio = _audioDictionary[tokenName].GetComponent<AudioSource>();
        if (audio.isPlaying)
        {
            // require fadeout effect, need to implement in the future
            if(tokenName == SoundResource.TEXT_CRAWLING_SFX)
            {
                FadeOutEffect(audio, 0.1f);
            }
            else
            {
                audio.Pause();
            }
        }
    }

    ///<summary>
    /// Pause the curret in-game BGM if any and play the requested background music.
    /// </summary>
    /// <param name="tokenName">Sound you are requesting to play</param>
    /// <param name="isfadeIn">Optional, set to false if nothing is passed. introduce fadeIn effect for BGM</param>
    /// <param name="fadeInDuration">Optional, <b>Only work when isfadeIn set to TRUE</b>, set to 2s second if nothing is passed. the fadein time for the selected BGM to reach its target volume.</param>
    /// <param name="isfadeOutCurrent">Optional, set to false if nothing is passed. fadeout the current playing BGM. Notice that the fadeout duration will be equal to the fadein Duration of the selected BGM</param>
    /// <returns>Void</returns>
    public void PlayBackgroundMusic(SoundResource tokenName, bool isfadeIn = true, float fadeInDuration = 2f, bool isfadeOutCurrent = true)
    {
        if (!instance._audioDictionary.ContainsKey(tokenName) || tokenName == currentBGM)
        {
            return;
        }
        if (currentBGM != SoundResource.NONE)
        {
            PauseBackgroundMusic(currentBGM, isfadeOutCurrent, fadeInDuration);
        }
        AudioSource audio = _audioDictionary[tokenName].GetComponent<AudioSource>();
        currentBGM = tokenName;
        ProcessPlayMusic(audio, isfadeIn: isfadeIn, fadeInDuration: fadeInDuration);
       
    }


    ///<summary>
    /// Pause the selected background music.
    /// </summary>
    /// <param name="tokenName">Sound you are requesting to pause</param>
    /// <returns>Void</returns>
    public void PauseBackgroundMusic(SoundResource tokenName, bool isfadeOut = true, float fadeOutDuration = 2f)
    {
        if (!instance._audioDictionary.ContainsKey(tokenName))
        {
            Debug.Log("Music does not exist");
            return;
        }
        AudioSource audio = _audioDictionary[tokenName].GetComponent<AudioSource>();
        if (audio.isPlaying)
        {
            // require fadeout effect, need to implement in the future

            if (isfadeOut)
            {
                FadeOutEffect(audio, fadeOutDuration, fadeOutModeStop:false);
            }
            else
            {
                audio.Pause();
            }
            
        }
    }

    ///<summary>
    /// Force stops the current background music.
    /// </summary>
    /// <returns>Void</returns>
    public void StopBackgroundMusic() {
        AudioSource audio = _audioDictionary[currentBGM].GetComponent<AudioSource>();
        if (audio.isPlaying) { audio.Stop(); }
        currentBGM = SoundResource.NONE;
    }

    ///<summary>
    /// Played the requested audioclip at a given position in world space.
    /// </summary>
    /// <param name="tokenName">Sound you are requesting to Play</param>
    /// <param name="position">Position in world space from which sound originates.</param>
    /// <returns>Void</returns>
    public void PlayMusicAtVector(SoundResource tokenName, Vector3 position)
    {
        if (!instance._audioDictionary.ContainsKey(tokenName))
        {
            Debug.Log("Music does not exist");
            return;
        }
        AudioSource audio = _audioDictionary[tokenName].GetComponent<AudioSource>();
        AudioSource.PlayClipAtPoint(audio.clip, position, audio.volume);
    }

    private IEnumerator FadeOutCalculated(AudioSource audioSource, float duration, float delayTime, float targetVolume = 0f, bool fadeOutModeStop = true)
    {
        yield return new WaitForSeconds(delayTime);
        FadeOutEffect(audioSource, duration, targetVolume, fadeOutModeStop);
    }

    //Manage fadein, fadeout effect for attached music type. 
    private void ProcessPlayMusic(AudioSource audioSource, bool isfadeIn = false, float fadeInDuration = 2f, bool isfadeOut = false, float fadeoutPercent = 0.1f, float fadeoutDuration = 2f, float targetVolume = 0f, bool fadeOutModeStop = true)
    {
        if (isfadeOut)
        {
            float audioRemainTime = GetClipRemainingTime(audioSource);
            Assert.IsTrue(fadeoutPercent > 0.0f && fadeoutPercent <= 1.0f);
            StartCoroutine(FadeOutCalculated(audioSource, fadeoutDuration, (1.0f - fadeoutPercent) * audioRemainTime, targetVolume, fadeOutModeStop));
        }
        if (isfadeIn)
        {
            FadeInEffect(audioSource, fadeInDuration, start:targetVolume ,audioSource.volume);
        }
        else
        {
            audioSource.Play();
        }

    }

    private void FadeInEffect(AudioSource audioSource, float duration, float start = 0f, float targetVolume = 1f)
    {

        audioSource.volume = 0;
        Tweener audioVolumeTweener = new Tweener(this, x => { audioSource.volume = x; },  ()=> { audioSource.volume = targetVolume; });

        audioSource.Play();
        audioVolumeTweener.TweenWithTime(start, targetVolume, duration, Tweener.LINEAR);
    }

    private void FadeOutEffect(AudioSource audioSource, float duration, float targetVolume = 0f, bool fadeOutModeStop = true)
    {

        float start = audioSource.volume;
        //float currentTime = 0;
        Tweener audioVolumeTweener = new Tweener(this, x => { audioSource.volume = x; }, ()=> {
            if (fadeOutModeStop)
            {
                audioSource.Stop();
            }
            else
            {
                audioSource.Pause();
            }
            audioSource.volume = start;
        });

        audioVolumeTweener.TweenWithTime(start, targetVolume, duration, Tweener.LINEAR);
    }

    private bool IsReversePitch(AudioSource source)
    {
        return source.pitch < 0f;
    }


    private float GetClipRemainingTime(AudioSource source)
    {
        // Calculate the remainingTime of the given AudioSource,
        // if we keep playing with the same pitch.
        float remainingTime = (source.clip.length - source.time) / source.pitch;
        return IsReversePitch(source) ?
            (source.clip.length + remainingTime) :
            remainingTime;
    }

    private void LoadVolume()
    {
        float musicVolume = PlayerPrefs.GetFloat(MUSIC_KEY, 1f);
        float sfxVolume = PlayerPrefs.GetFloat(SFX_KEY, 1f);
        float uiVolume = PlayerPrefs.GetFloat(UI_KEY, 1f);


        mixer.SetFloat(SoundSettings.MIXER_MUSIC, Mathf.Log10(musicVolume) * 20);
        mixer.SetFloat(SoundSettings.MIXER_SFX, Mathf.Log10(sfxVolume) * 20);
        mixer.SetFloat(SoundSettings.MIXER_UI, Mathf.Log10(uiVolume) * 20);


    }
}
