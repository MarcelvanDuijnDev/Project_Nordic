using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Audio;

public class AudioHandler : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Only used for testing disable it for the final build to improve performance")]
    [SerializeField] private bool _RefreshSettingsOnUpdate = false;

    [Header("AudioMixer/Audio")]
    [SerializeField] private AudioMixerGroup _AudioMixer = null;
    [SerializeField] private List<AudioHandler_Sound> _Sound = new List<AudioHandler_Sound>();

    private string _CurrentScene;

    public float AudioVolume = 0.5f;

    //You can call AudioHandler.AUDIO from every script as long as you have the script in the scene
    public static AudioHandler AUDIO;

    void Start()
    {
        AUDIO = this;

        //PlayOnStart
        for (int i = 0; i < _Sound.Count; i++)
        {
            //AudioSource
            if (_Sound[i].Settings.CreateAudioSource)
            {
                _Sound[i].Settings.AudioSource = this.gameObject.AddComponent<AudioSource>();
                _Sound[i].Settings.AudioSource.outputAudioMixerGroup = _AudioMixer;

                //AudioGroup
                if (_Sound[i].Settings.AudioGroup != null)
                    _Sound[i].Settings.AudioSource.outputAudioMixerGroup = _Sound[i].Settings.AudioGroup;
            }

            //AudioClip
            _Sound[i].Settings.AudioSource.clip = _Sound[i].Settings.AudioClip;

            //Settings
            if (_Sound[i].AudioSettings.PlayOnStart)
            {
                _Sound[i].Settings.AudioSource.playOnAwake = _Sound[i].AudioSettings.PlayOnStart;
                _Sound[i].Settings.AudioSource.Play();
            }
            if (_Sound[i].AudioEffects.FadeIn)
            {
                _Sound[i].Settings.AudioSource.volume = 1;
                _Sound[i].AudioEffects.FadeInSpeed = _Sound[i].AudioSettings.Volume / _Sound[i].AudioEffects.FadeInDuration;
            }
            if (_Sound[i].AudioEffects.FadeOut)
            {
                _Sound[i].AudioEffects.FadeOutSpeed = _Sound[i].AudioSettings.Volume / _Sound[i].AudioEffects.FadeOutDuration;
            }
        }

        RefreshSettings();
    }

    void Update()
    {
        CheckNewScene();

        if (_RefreshSettingsOnUpdate)
            RefreshSettings();

        for (int i = 0; i < _Sound.Count; i++)
        {
            //FadeIn
            if (_Sound[i].AudioEffects.FadingIn)
            {
                if (_Sound[i].AudioEffects.FadeIn && !_Sound[i].AudioEffects.FadeInDone)
                {
                    if (_Sound[i].Settings.AudioSource.volume < _Sound[i].AudioSettings.Volume)
                    {
                        _Sound[i].Settings.AudioSource.volume += _Sound[i].AudioEffects.FadeInSpeed * Time.deltaTime;
                    }
                    else
                    {
                        _Sound[i].AudioEffects.FadeInDone = true;
                        _Sound[i].Settings.AudioSource.volume = _Sound[i].AudioSettings.Volume;
                    }
                }
            }
            //FadeOut
            if (_Sound[i].AudioEffects.FadingOut)
            {
                if (_Sound[i].AudioEffects.FadeOutAfterTime > -0.1f)
                {
                    _Sound[i].AudioEffects.FadeOutAfterTime -= 1 * Time.deltaTime;
                }
                else
                {
                    if (_Sound[i].AudioEffects.FadeOut && !_Sound[i].AudioEffects.FadeOutDone)
                    {
                        if (_Sound[i].Settings.AudioSource.volume > 0)
                        {
                            _Sound[i].Settings.AudioSource.volume -= _Sound[i].AudioEffects.FadeOutSpeed * Time.deltaTime;
                        }
                        else
                        {
                            _Sound[i].AudioEffects.FadeOutDone = true;
                            _Sound[i].Settings.AudioSource.volume = 0;
                            _Sound[i].Settings.AudioSource.Stop();
                        }
                    }
                }
            }

            //Set Volume
            _Sound[i].AudioSettings.Volume = AudioVolume;
            _Sound[i].Settings.AudioSource.volume = _Sound[i].AudioSettings.Volume;
        }
    }

    private void CheckNewScene()
    {
        if (_CurrentScene != SceneManager.GetActiveScene().name)
        {
            _CurrentScene = SceneManager.GetActiveScene().name;
            for (int i = 0; i < _Sound.Count; i++)
            {
                for (int o = 0; o < _Sound[i].AudioControl.StartAudioOnScene.Count; o++)
                {
                    if (_Sound[i].AudioControl.StartAudioOnScene[o] == _CurrentScene)
                    {
                        //FadeIn
                        if (_Sound[i].AudioEffects.FadeIn)
                        {
                            _Sound[i].AudioEffects.FadingOut = false;
                            _Sound[i].AudioEffects.FadeInDone = false;
                            _Sound[i].AudioEffects.FadingIn = true;
                        }
                        _Sound[i].Settings.AudioSource.Play();
                    }
                }
                for (int o = 0; o < _Sound[i].AudioControl.StopAudioOnScene.Count; o++)
                {
                    if (_Sound[i].AudioControl.StopAudioOnScene[o] == _CurrentScene)
                    {
                        //FadeOut
                        if (_Sound[i].AudioEffects.FadeOut && !_Sound[i].AudioEffects.FadingOut)
                        {
                            _Sound[i].AudioEffects.FadingIn = false;
                            _Sound[i].AudioEffects.FadeOutDone = false;
                            _Sound[i].AudioEffects.FadingOut = true;
                        }
                        else
                            _Sound[i].Settings.AudioSource.Stop();
                    }
                }
            }
        }
    }

    /// <summary>Plays the audiotrack.</summary>
    public void PlayTrack(string trackname)
    {
        for (int i = 0; i < _Sound.Count; i++)
        {
            if (_Sound[i].AudioTrackName == trackname)
                AudioHandler_PlayTrack(i);
        }
    }
    /// <summary>Plays the audiotrack if it's not playing yet.</summary>
    public void StartTrack(string trackname)
    {
        for (int i = 0; i < _Sound.Count; i++)
        {
            if (_Sound[i].AudioTrackName == trackname)
                if (!_Sound[i].Settings.AudioSource.isPlaying)
                    AudioHandler_PlayTrack(i);
        }
    }
    public void StopTrack(string trackname)
    {
        for (int i = 0; i < _Sound.Count; i++)
        {
            if (_Sound[i].AudioTrackName == trackname)
                _Sound[i].Settings.AudioSource.Stop();
        }
    }
    public string Get_Track_AudioFileName(string trackname)
    {
        for (int i = 0; i < _Sound.Count; i++)
        {
            if (_Sound[i].AudioTrackName == trackname)
                return _Sound[i].Settings.AudioClip.name;
        }
        return "No AudioClip detected";
    }

    private void AudioHandler_PlayTrack(int trackid)
    {
        _Sound[trackid].Settings.AudioSource.Play();
    }
    public void RefreshSettings()
    {
        for (int i = 0; i < _Sound.Count; i++)
        {
            //SetClip
            if (_Sound[i].Settings.AudioSource.clip != _Sound[i].Settings.AudioClip)
                _Sound[i].Settings.AudioSource.clip = _Sound[i].Settings.AudioClip;
            //SetEffects
            if (!_Sound[i].AudioEffects.FadeIn || _Sound[i].AudioEffects.FadeIn && _Sound[i].AudioEffects.FadeInDone)
                _Sound[i].Settings.AudioSource.volume = _Sound[i].AudioSettings.Volume;
            _Sound[i].Settings.AudioSource.loop = _Sound[i].AudioSettings.Loop;
        }
    }

    public void SetAudioSource(string trackname, AudioSource audiosource)
    {
        for (int i = 0; i < _Sound.Count; i++)
        {
            if (_Sound[i].AudioTrackName == trackname)
                _Sound[i].Settings.AudioSource = audiosource;
        }
    }
}

[System.Serializable]
public class AudioHandler_Sound
{
    public string AudioTrackName;
    public AudioHandler_Settings Settings;
    public AudioHandler_AudioSettings AudioSettings;
    public AudioHandler_Control AudioControl;
    public AudioHandler_Effects AudioEffects;
}

[System.Serializable]
public class AudioHandler_Settings
{
    [Header("AudioClip/AudioMixerGroup")]
    public AudioClip AudioClip;
    public AudioMixerGroup AudioGroup;

    [Header("AudioSource")]
    public AudioSource AudioSource;
    public bool CreateAudioSource;
}

[System.Serializable]
public class AudioHandler_AudioSettings
{
    [Header("AudioSettings")]
    [Range(0, 1)] public float Volume;
    public bool Loop;
    public bool PlayOnStart;
}

[System.Serializable]
public class AudioHandler_Control
{
    [Header("Enable/Disable Song")]
    public List<string> StartAudioOnScene = new List<string>();
    public List<string> StopAudioOnScene = new List<string>();
    public bool StopOnNextScene;
    [HideInInspector] public int SceneEnabled;
}

[System.Serializable]
public class AudioHandler_Effects
{
    [Header("FadeIn")]
    public bool FadeIn;
    public float FadeInDuration;
    [HideInInspector] public float FadeInSpeed;
    [HideInInspector] public bool FadeInDone;
    [HideInInspector] public bool FadingIn;
    [Header("FadeOut")]
    public bool FadeOut;
    public float FadeOutAfterTime;
    public float FadeOutDuration;
    [HideInInspector] public float FadeOutSpeed;
    [HideInInspector] public bool FadeOutDone;
    [HideInInspector] public bool FadingOut;
}
