using UnityEngine;
using UnityEngine.Audio;


[CreateAssetMenu(fileName ="New_sound", menuName = "Audio/New_Sound_Effect")]
public class Sound : ScriptableObject
{
    public enum MusicType { ATTACHED, BACKGROUND_SOLO, BACKGROUND_SFX, WORLDSPACE };

    [Header("Audio Players Components")]
    public AudioClip clip;

    [Tooltip("The Music type used by Audio Mixer")]
    public MusicType myType;

    [Tooltip("Audio Volume")]
    [Range(0f, 1f)]
    public float volume = 1f;

    [Tooltip("Audio Pitch")]
    [Range(.1f,3f)]
    public float pitch =1f;

    [Tooltip("Spatial Blend: 0 means 2D, 1 means 3D")]
    [Range(0f, 1f)]
    public float spatialBlend = 0f;

    [Tooltip("Looping")]
    public bool loop = false;

    [Tooltip("Play On Awake")]
    public bool playOnAwake = false;

    [Tooltip("AudioMixerGroup")]
    public AudioMixerGroup audioMixerGroup;

    [Tooltip("PauseOnGamePause")]
    public bool pauseOnGamePause = false;

    [Header("3d Sound Setting")]

    [Tooltip("Doppler Level")]
    [Range(0f,5f)]
    public float dopplerLevel = 1f;

    [Tooltip("Spread")]
    [Range(0, 360)]
    public int spread;

    [Tooltip("Rolloff Mode")]
    public AudioRolloffMode rolloffMode;

    [Tooltip("Min Distance")]
    public float minDistance = 0.0f;

    [Tooltip("Max Distance")]
    public float maxDistance = 500f;

    [Header("Sound Token")]
    [Tooltip("The parameter that other scripts will call")]
    public AudioManager.SoundResource token;
}
