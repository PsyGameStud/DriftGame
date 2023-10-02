using DriftGame.System;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace DriftGame.UI
{
    public class GarageMenu : MonoBehaviour
    {
        [SerializeField] private Button _buttonBoostSpeed;
        [SerializeField] private Button _buttonBoostAcceleration;

        [Space]
        [SerializeField] private Material _carMaterial;
        [SerializeField] private Material _carMaterialLight;

        [SerializeField] private Button _buttonRed;
        [SerializeField] private Button _buttonBlue;
        [SerializeField] private Button _buttonYellow;

        [SerializeField] private Texture _redTexture;
        [SerializeField] private Texture _blueTexture;
        [SerializeField] private Texture _yellowTexture;

        private ISaveSystem _saveSystem;

        public event Action<int> OnBuyBoost;
        private int _additionalSpeed;
        private int _additionalAcceleration;
        
        public void Init(ISaveSystem saveSystem, bool isFirstLaunch)
        {
            _buttonRed.onClick.AddListener(() =>
            {
                _carMaterial.mainTexture = _redTexture;
            });

            _buttonBlue.onClick.AddListener(() =>
            {
                _carMaterial.mainTexture = _blueTexture;
            });

            _buttonYellow.onClick.AddListener(() =>
            {
                _carMaterial.mainTexture = _yellowTexture;
            });

            _buttonBoostSpeed.onClick.AddListener(OnClickBoostSpeed);
            _buttonBoostAcceleration.onClick.AddListener(OnClickBoostAcceleration);

            _saveSystem = saveSystem;

            if (isFirstLaunch)
            {
                SaveData();
            }
        }

        private void OnClickBoostSpeed()
        {
            if (GarageEntryPoint.Instance.CurrencySystem.SpendMoney(10000))
            {
                OnBuyBoost?.Invoke(GarageEntryPoint.Instance.CurrencySystem.Currency);
                _additionalSpeed += 5;
                SaveData();
            }
        }

        private void OnClickBoostAcceleration()
        {
            if (GarageEntryPoint.Instance.CurrencySystem.SpendMoney(10000))
            {
                OnBuyBoost?.Invoke(GarageEntryPoint.Instance.CurrencySystem.Currency);
                _additionalAcceleration += 1;
                SaveData();
            }
        }

        private void SaveData()
        {
            var newGarageData = new GarageData()
            {
                AdditionalSpeed = _additionalSpeed,
                AdditionalAcceleration = _additionalAcceleration
            };

            _saveSystem.Save("GARAGE_DATA", newGarageData);
        }
    }

    public class GarageData
    {
        public int AdditionalSpeed;
        public int AdditionalAcceleration;
    }
}