using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DriftGame.UI
{
    public class ConnectWindow : MonoBehaviour
    {
        [SerializeField] private RectTransform _root;
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private Button _buttonClose;
        [SerializeField] private Button _buttonJoinOrCreate;

        public string RoomName => _inputField.text;
        public Button ButtonJoinOrCreate => _buttonJoinOrCreate;

        private void Start()
        {
            _buttonClose.onClick.AddListener(Hide);
            _root.DOScale(Vector3.zero, 0f);
        }

        public void Show()
        {
            _root.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        }

        public void Hide()
        {
            _root.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack);
        }
    }
}
