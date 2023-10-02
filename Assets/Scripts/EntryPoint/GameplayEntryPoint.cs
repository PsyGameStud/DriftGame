using Cinemachine;
using DriftGame.Car;
using DriftGame.Managers;
using DriftGame.Singleton;
using DriftGame.System;
using Photon.Pun;
using System.Collections;
using UnityEngine;

namespace DriftGame
{
    public class GameplayEntryPoint : Singleton<GameplayEntryPoint>
    {
        [SerializeField] private CarController _carPrefab;
        [SerializeField] private Transform[] _startPoints;
        [SerializeField] private CinemachineVirtualCamera _virtualCamera;
        [SerializeField] private TimerManager _timerManager;

        private ISaveSystem _saveSystem;
        private CurrencySystem _currencySystem;
        private GameObject _player;

        public ISaveSystem SaveSystem => _saveSystem;
        public CurrencySystem CurrencySystem => _currencySystem;

        public override void Awake()
        { 
            base.Awake();
        
            _saveSystem = new JsonSaveSystem();
            _currencySystem = new CurrencySystem(_saveSystem);
            _currencySystem.Init(false);
        }

        private void Start()
        {
            StartCoroutine(CreatePlayer());
        }

        private IEnumerator CreatePlayer()
        {
            yield return new WaitUntil(() => PhotonNetwork.IsConnectedAndReady);
            var newCar = PhotonNetwork.Instantiate(_carPrefab.name,
                PhotonNetwork.IsMasterClient ? _startPoints[0].position : _startPoints[1].position, Quaternion.identity);
            _virtualCamera.LookAt = newCar.transform;
            _virtualCamera.Follow = newCar.transform;
            _player = newCar;
            _timerManager.Setup();
        }

        public void DestoryPlayer()
        {
            PhotonNetwork.Destroy(_player);
        }
    }
}