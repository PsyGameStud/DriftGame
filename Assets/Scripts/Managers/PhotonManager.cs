using DriftGame.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace DriftGame.Managers
{
    public class PhotonManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private string _region;
        private ConnectWindow _connectWindow;

        private void Awake()
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.ConnectToRegion(_region);
        }

        public void SetWindowConnect(ConnectWindow connectWindow)
        {
            _connectWindow = connectWindow;
            _connectWindow.ButtonJoinOrCreate.onClick.AddListener(JoinOrCreateRoom);
        }

        public override void OnConnectedToMaster()
        {
            Debug.LogError($"CONNECT TO REGION: {PhotonNetwork.CloudRegion}");
            if (!PhotonNetwork.InLobby)
            {
                PhotonNetwork.JoinLobby();
            }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogError($"DISCONNECT");
        }

        private void JoinOrCreateRoom()
        {
            if (!PhotonNetwork.IsConnected)
            {
                return;
            }

            var roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 2;
            PhotonNetwork.JoinOrCreateRoom(_connectWindow.RoomName, roomOptions, TypedLobby.Default);
            PhotonNetwork.LoadLevel(1);
        }

        public override void OnCreatedRoom()
        {
            Debug.LogError($"Room Created: {PhotonNetwork.CurrentRoom.Name}");
        }

        public void LeftRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        public override void OnLeftRoom()
        {
            PhotonNetwork.LoadLevel(0);
        }
    }
}