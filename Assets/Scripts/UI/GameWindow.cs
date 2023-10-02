using DG.Tweening;
using DriftGame.Managers;
using DriftGame.Singleton;
using TMPro;
using UnityEngine;

namespace DriftGame.UI
{
    public class GameWindow : Singleton<GameWindow>
    {
        [SerializeField] private TextMeshProUGUI _textSpeed;
        [SerializeField] private TextMeshProUGUI _textCurrentDriftPoints;
        [SerializeField] private TextMeshProUGUI _textTotalDriftPoints;
        [SerializeField] private CompletedWindow _completedWindow;
        [SerializeField] private CanvasGroup _canvasGroup;

        private int _totalDriftPoints;

        private void Start()
        {
            TimerManager.Instance.OnComplete += CompleteGame;
        }

        public void SetSpeed(float speed)
        {
            float absoluteCarSpeed = Mathf.Abs(speed);
            _textSpeed.text = $"{Mathf.RoundToInt(absoluteCarSpeed)}";
        }

        public void SetDriftPoint(int driftPoints)
        {
            _textCurrentDriftPoints.text = $"{driftPoints}";
        }

        public void UpdateTotalDriftPoints(int lastDriftPoints)
        {
            _totalDriftPoints += lastDriftPoints;
            _textTotalDriftPoints.text = $"{_totalDriftPoints}";
        }

        private void CompleteGame()
        {
            var currencySystem = GameplayEntryPoint.Instance.CurrencySystem;
            currencySystem.AddMoney(_totalDriftPoints);
            _completedWindow.ShowWindow(_totalDriftPoints);
            _canvasGroup.DOFade(0f, 0.5f);
        }
    }
}
