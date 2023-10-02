using DriftGame.Singleton;
using System;
using UnityEngine;

namespace DriftGame.Managers
{
    public class SoundManager : Singleton<SoundManager>
    {
        [Space(10)]
        [SerializeField] private bool _useSounds = false;
        [SerializeField] private AudioSource _carEngineSound;
        [SerializeField] private AudioSource _tireScreechSound;
        private float _initialCarEngineSoundPitch;

        private void Start()
        {
            _initialCarEngineSoundPitch = _carEngineSound.pitch;
        }

        public void CarSounds(bool isDrifting, bool isTractionLocked, float carSpeed, Rigidbody carRigidbody)
        {

            if (_useSounds)
            {
                try
                {
                    if (_carEngineSound != null)
                    {
                        float engineSoundPitch = _initialCarEngineSoundPitch + (Mathf.Abs(carRigidbody.velocity.magnitude) / 25f);
                        _carEngineSound.pitch = engineSoundPitch;
                    }
                    if ((isDrifting) || (isTractionLocked && Mathf.Abs(carSpeed) > 12f))
                    {
                        if (!_tireScreechSound.isPlaying)
                        {
                            _tireScreechSound.Play();
                        }
                    }
                    else if ((!isDrifting) && (!isTractionLocked || Mathf.Abs(carSpeed) < 12f))
                    {
                        _tireScreechSound.Stop();
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning(ex);
                }
            }
            else if (!_useSounds)
            {
                if (_carEngineSound != null && _carEngineSound.isPlaying)
                {
                    _carEngineSound.Stop();
                }
                if (_tireScreechSound != null && _tireScreechSound.isPlaying)
                {
                    _tireScreechSound.Stop();
                }
            }
        }

        public void StopSound()
        {
            _carEngineSound.Stop();
            _tireScreechSound.Stop();
        }
    }
}