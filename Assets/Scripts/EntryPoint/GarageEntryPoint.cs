using DriftGame.Singleton;
using DriftGame.System;
using DriftGame.UI;
using UnityEngine;

namespace DriftGame
{
    public class GarageEntryPoint : Singleton<GarageEntryPoint>
    {
        [SerializeField] private GarageMenu _garageMenu; 

        private ISaveSystem _saveSystem;
        private CurrencySystem _currencySystem;

        public ISaveSystem SaveSystem => _saveSystem;
        public CurrencySystem CurrencySystem => _currencySystem;

        private bool _isFirstLaunch;

        private void Start()
        {
            _isFirstLaunch = !PlayerPrefs.HasKey("FIRST_LAUNCH");

            _saveSystem = new JsonSaveSystem();
            _currencySystem = new CurrencySystem(_saveSystem);
            _currencySystem.Init(_isFirstLaunch);
            _garageMenu.Init(_saveSystem, _isFirstLaunch);
        }
    }
}
