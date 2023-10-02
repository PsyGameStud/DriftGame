using DG.Tweening;
using DriftGame.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DriftGame.UI
{
    public class CompletedWindow : MonoBehaviour
    {
        [SerializeField] private RectTransform _root;
        [SerializeField] private TextMeshProUGUI _textTotalReward;
        [SerializeField] private Button _buttonQuit;
        [SerializeField] private PhotonManager _photonManager;

        public void ShowWindow(int totalReward)
        {
            _root.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
            _textTotalReward.text = $"{totalReward}";
            _buttonQuit.onClick.AddListener(() => 
            {
                GameplayEntryPoint.Instance.DestoryPlayer();
                _photonManager.LeftRoom(); 
            });
        }
    }
}