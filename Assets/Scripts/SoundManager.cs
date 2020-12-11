using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Equation
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }

        [SerializeField] AudioMixer _audioMixer;
        [SerializeField] AudioMixerGroup _masterGroup;
        [SerializeField] AudioMixerGroup _soundsGroup;
        [SerializeField] AudioMixerGroup _musicsGroup;

        [SerializeField] string _soundsGroupVolumeParam;
        [SerializeField] string _musicsGroupVolumeParam;


        [SerializeField] AudioSource _musicAudio;

        void Start()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                // _musicAudio.PlayDelayed(2);
                return;
            }
            else if(Instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }

        public void MuteSounds()
        {
            _audioMixer.GetFloat(_soundsGroupVolumeParam, out float volume);
            PlayerPrefs.SetFloat(_soundsGroupVolumeParam, volume);
            _audioMixer.SetFloat(_soundsGroupVolumeParam, -80);
        }
        
        public void UnMuteSounds()
        {
            var volume = PlayerPrefs.GetFloat(_soundsGroupVolumeParam);
            _audioMixer.SetFloat(_soundsGroupVolumeParam, volume);
        }
        
        public void MuteMusics()
        {
            _audioMixer.GetFloat(_musicsGroupVolumeParam, out float volume);
            PlayerPrefs.SetFloat(_musicsGroupVolumeParam, volume);
            _audioMixer.SetFloat(_musicsGroupVolumeParam, -80);
        }
        
        public void UnMuteMusics()
        {
            var volume = PlayerPrefs.GetFloat(_musicsGroupVolumeParam);
            _audioMixer.SetFloat(_musicsGroupVolumeParam, volume);
        }

    }
}