using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using DriftGame.Managers;

namespace DriftGame.UI
{
    public class MainMenu : MonoBehaviour
    {
        [Space]
        [SerializeField] private PhotonManager _photonManager;

        [Space]
        [SerializeField] private GarageMenu _garageMenu;
        [SerializeField] private TextMeshProUGUI _textCurrency;

        [Header("Main Buttons")]
        [SerializeField] private Button _buttonStart;
        [SerializeField] private Button _buttonSettings;
        [SerializeField] private Button _buttonGarage;
        [SerializeField] private Button _buttonExit;

        [Space]
        [SerializeField] private RectTransform _buttonsRect;
        [SerializeField] private RectTransform _garageRect;

        [Header("Garage Buttons")]
        [SerializeField] private Button _buttonCloseGarage;

        [Space]
        [SerializeField] private ConnectWindow _connectWindow;


        private void Start()
        {
            _buttonExit.onClick.AddListener(Application.Quit);
            _buttonGarage.onClick.AddListener(ShowGarage);
            _buttonStart.onClick.AddListener(StartGame);
            _buttonSettings.onClick.AddListener(OpenSettings);
            _buttonCloseGarage.onClick.AddListener(HideGarage);
            _garageRect.DOAnchorPosX(450, 0f);

            _garageMenu.OnBuyBoost += UpdateCurrency;
            _textCurrency.text = $"Currency: {GarageEntryPoint.Instance.CurrencySystem.Currency}";

            _photonManager.SetWindowConnect(_connectWindow);
        }

        private void StartGame()
        {
            _connectWindow.Show();
        }

        private void OpenSettings()
        {

        }

        private void ShowGarage()
        {
            _buttonsRect.DOAnchorPosX(450, 0.5f).SetEase(Ease.OutBack);
            _garageRect.DOAnchorPosX(-30, 0.5f).SetEase(Ease.InBack);
        }

        private void HideGarage()
        {
            _buttonsRect.DOAnchorPosX(-50, 0.5f).SetEase(Ease.InBack);
            _garageRect.DOAnchorPosX(450, 0.5f).SetEase(Ease.OutBack);
        }

        private void UpdateCurrency(int currency)
        {
            _textCurrency.text = $"Currency: {currency}";
        }
    }
}
