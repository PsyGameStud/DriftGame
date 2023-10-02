using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DriftGame.System
{
    public class CurrencySystem
    {

        private ISaveSystem _saveSystem;
        private CurrencyData _currencyData;
        private int _currency;

        private const string Key = "CURRENCY_DATA";

        public int Currency => _currency;

        public CurrencySystem(ISaveSystem saveSystem)
        {
            _saveSystem = saveSystem;
        }

        public void Init(bool isFirstLaunch)
        {
            if (isFirstLaunch)
            {
                SaveCurrency();
                PlayerPrefs.SetString("FIRST_LAUNCH", "");
            }

            _saveSystem.Load<CurrencyData>(Key, data =>
            {
                _currencyData = data;
            });


            _currency = _currencyData.Currency;
        }

        public void AddMoney(int reward)
        {
            _currency += reward;
            SaveCurrency();
        }

        public bool SpendMoney(int price)
        {
            if (price > _currency)
            {
                return false;
            }

            _currency -= price;
            SaveCurrency();
            return true;
        }

        private void SaveCurrency()
        {
            _saveSystem.Save(Key, new CurrencyData()
            {
                Currency = _currency
            });
        }
    }

    public class CurrencyData
    {
        public int Currency;
    }
}
