using DriftGame.Singleton;
using System;
using TMPro;
using UnityEngine;

namespace DriftGame.Managers
{
    public class TimerManager : Singleton<TimerManager>
    {
        [SerializeField] private TextMeshProUGUI _textTimer;

        private float _timeLeft = 120f;
        private bool _isGameEnd;
        private bool _playerCreated;

        public event Action OnComplete;

        private void Start()
        {
            UpdateTimer();
        }

        public void Setup()
        {
            _playerCreated = true;
            _isGameEnd = false;
        }

        private void Update()
        {
            if (!_playerCreated)
            {
                return;
            }

            _timeLeft -= Time.deltaTime;

            if (_timeLeft > 0)
            {
                UpdateTimer();   
            }

            if (_timeLeft <= 0 && !_isGameEnd)
            {
                _isGameEnd = true;
                OnComplete?.Invoke();
            }
        }

        private void UpdateTimer()
        {
            var hours = Mathf.FloorToInt(_timeLeft / 3600f);
            var minutes = Mathf.FloorToInt((_timeLeft - hours * 3600f) / 60f);
            var seconds = Mathf.FloorToInt(_timeLeft - hours * 3600f - minutes * 60f);

            _textTimer.text = $"{minutes.ToString("00")}:{seconds.ToString("00")}";
        }
    }
}