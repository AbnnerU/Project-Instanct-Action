using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scripts.Sound
{
    public class SIS_SoundsManager : Singleton<SIS_SoundsManager>
    {
        [Header("Audio Source Pool")]
        [SerializeField] private GameObject _emitterPrefab;
        [SerializeField] private int _initialSize;

        [Header("Channel")]
        [SerializeField] private AudioChannel _musicAudioChannel;
        [SerializeField] private AudioChannel _soundEffectsChannel;
        [SerializeField] private AudioChannel _ambientEffectsChannel;

        [Header("General")]
        [SerializeField] private bool stopMainTrackWhenNewScene = true;

        private List<SoundEmitter> _list_SoundEmitters = new List<SoundEmitter>();

        protected override void Awake()
        {
            base.Awake();
            if (Instance != this)
                return;

            _musicAudioChannel.OnAudioRequest += MusicChannel_AudioRequest;
            _musicAudioChannel.OnStopAudioRequest += MusicChannel_StopAudioRequest;

            _soundEffectsChannel.OnAudioRequest += SoundEffect_AudioRequest;
            _soundEffectsChannel.OnStopAudioRequest += SoundEffect_StopAudioRequest;

            if (_ambientEffectsChannel != null)
            {
                _ambientEffectsChannel.OnAudioRequest += AmbientEffect_AudioRequest;
                _ambientEffectsChannel.OnStopAudioRequest += AmbientEffect_StopAudioRequest;
            }

            Prewarm();

            SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        }

        private void OnDestroy()
        {
            if (_musicAudioChannel != null)
            {
                _musicAudioChannel.OnAudioRequest -= MusicChannel_AudioRequest;
                _musicAudioChannel.OnStopAudioRequest -= MusicChannel_StopAudioRequest;
            }

            if (_soundEffectsChannel != null)
            {
                _soundEffectsChannel.OnAudioRequest -= SoundEffect_AudioRequest;
                _soundEffectsChannel.OnStopAudioRequest -= SoundEffect_StopAudioRequest;
            }

            SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
            if (_ambientEffectsChannel != null)
            {
                _ambientEffectsChannel.OnAudioRequest -= AmbientEffect_AudioRequest;
                _ambientEffectsChannel.OnStopAudioRequest -= AmbientEffect_StopAudioRequest;
            }
        }

        private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
        {
            if (stopMainTrackWhenNewScene)
                StopMainTrack();

            StopSfXAudios();
            StopAmbientEffects();
        }

        private void SoundEffect_StopAudioRequest(AudioRequestInfo request)
        {
            StopAudioRequest(request.audioConfig, request.audioPosition, EmitterType.SOUNDEFFECTS);
        }

        private void SoundEffect_AudioRequest(AudioRequestInfo request)
        {
            float volumePercentage = SoundsSettings.GetSoundEffectVolume(); //Value between 0 and 100

            for (int i = 0; i < _list_SoundEmitters.Count; i++)
            {
                if (_list_SoundEmitters[i].InUse() == false)
                {
                    _list_SoundEmitters[i].PlayAudio(request.audioConfig, EmitterType.SOUNDEFFECTS, volumePercentage, request.audioPosition, request.clipOverride);

                    return ;
                }
            }

            GameObject obj = Instantiate(_emitterPrefab, Vector3.zero, Quaternion.identity, this.transform);

            obj.GetComponent<SoundEmitter>().PlayAudio(request.audioConfig, EmitterType.SOUNDEFFECTS, volumePercentage, request.audioPosition, request.clipOverride);

            _list_SoundEmitters.Add(obj.GetComponent<SoundEmitter>());
        }

        private void MusicChannel_StopAudioRequest(AudioRequestInfo request)
        {
            StopAudioRequest(request.audioConfig, request.audioPosition, EmitterType.MAiNTRACK);
        }

        private void MusicChannel_AudioRequest(AudioRequestInfo request)
        {
            float volumePercentage = SoundsSettings.GetMusicVolume(); //Value between 0 and 100

            for (int i = 0; i < _list_SoundEmitters.Count; i++)
            {
                if (_list_SoundEmitters[i].InUse() == false)
                {
                   

                    _list_SoundEmitters[i].PlayAudio(request.audioConfig, EmitterType.MAiNTRACK, volumePercentage, request.audioPosition, request.clipOverride);

                    return;
                }
            }

            GameObject obj = Instantiate(_emitterPrefab, Vector3.zero, Quaternion.identity, this.transform);

            obj.GetComponent<SoundEmitter>().PlayAudio(request.audioConfig, EmitterType.MAiNTRACK, volumePercentage, request.audioPosition, request.clipOverride);

            _list_SoundEmitters.Add(obj.GetComponent<SoundEmitter>());
        }

        private void StopSfXAudios()
        {
            foreach (SoundEmitter s in _list_SoundEmitters)
            {
                if (s.GetEmitterType() == EmitterType.SOUNDEFFECTS && s.InUse())
                {
                    s.Stop();
                }
            }
        }

        private void AmbientEffect_AudioRequest(AudioRequestInfo request)
        {
            float volumePercentage = SoundsSettings.GetAmbientEffectVolume();
            foreach (SoundEmitter emitter in _list_SoundEmitters)
            {
                if (!emitter.InUse())
                {
                    emitter.PlayAudio(request.audioConfig, EmitterType.AMBIENTEFFECTS, volumePercentage, request.audioPosition, request.clipOverride);
                    return;
                }
            }

            GameObject obj = Instantiate(_emitterPrefab, Vector3.zero, Quaternion.identity, transform);
            SoundEmitter newEmitter = obj.GetComponent<SoundEmitter>();
            newEmitter.PlayAudio(request.audioConfig, EmitterType.AMBIENTEFFECTS, volumePercentage, request.audioPosition, request.clipOverride);
            _list_SoundEmitters.Add(newEmitter);
        }

        private void AmbientEffect_StopAudioRequest(AudioRequestInfo request)
        {
            StopAudioRequest(request.audioConfig, request.audioPosition, EmitterType.AMBIENTEFFECTS);
        }

        public void StopAmbientEffects()
        {
            foreach (SoundEmitter emitter in _list_SoundEmitters)
            {
                if (emitter.GetEmitterType() == EmitterType.AMBIENTEFFECTS && emitter.InUse())
                    emitter.Stop();
            }
        }

        public void SetNewAmbientEffectVolume(float newValue)
        {
            newValue = Mathf.Clamp(newValue, 0f, 100f);
            foreach (SoundEmitter emitter in _list_SoundEmitters)
            {
                if (emitter.GetEmitterType() == EmitterType.AMBIENTEFFECTS)
                    emitter.ChangeVolume(newValue);
            }

            SoundsSettings.SaveAmbientEffectsVolume(newValue);
        }

        private void StopAudioRequest(AudioConfig audioConfig, Vector3 positionRef, EmitterType emitterType)
        {
            foreach (SoundEmitter s in _list_SoundEmitters)
            {
                if (s.GetEmitterType() == emitterType && s.IsUsingAudioConfig(audioConfig, positionRef))
                {
                    s.Stop();
                    return;
                }
            }
        }

        public void SetNewMusicVolume(float newValue)
        {
            foreach (SoundEmitter s in _list_SoundEmitters)
            {
                if (s.GetEmitterType() == EmitterType.MAiNTRACK)
                    s.ChangeVolume(newValue);
            }

            SoundsSettings.SaveMusicVolume(newValue);
        }

        public void SetNewSoundEffectVolume(float newValue)
        {
            foreach (SoundEmitter s in _list_SoundEmitters)
            {
                if (s.GetEmitterType() == EmitterType.SOUNDEFFECTS)
                    s.ChangeVolume(newValue);
            }

            SoundsSettings.SaveSoundEffectsVolume(newValue);
        }

        public void StopMainTrack()
        {
            foreach (SoundEmitter s in _list_SoundEmitters)
            {
                if (s.GetEmitterType() == EmitterType.MAiNTRACK)
                {
                    if (s.InUse())
                    {
                        s.StopWhitFadeOut();
                        break;
                    }
                }
            }
        }

        private void Prewarm()
        {
            for(int i = 0; i < _initialSize; i++)
            {
                GameObject obj = Instantiate(_emitterPrefab, Vector3.zero, Quaternion.identity, this.transform);

                _list_SoundEmitters.Add(obj.GetComponent<SoundEmitter>());
            }
        }
    }
}
