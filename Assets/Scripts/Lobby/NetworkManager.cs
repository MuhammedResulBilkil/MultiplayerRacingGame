using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Lobby
{
    public class NetworkManager : MonoBehaviourPunCallbacks
    {
        [Header("Connection Status")] [SerializeField]
        private TextMeshProUGUI _connectionStatusText;

        [Header("Login UI Panel")] [SerializeField]
        private TMP_InputField _playerNameInput;

        [SerializeField] private GameObject _loginUIPanel;

        [Header("Connecting Info Panel")] [SerializeField]
        private GameObject _connectingInfoUIPanel;

        [Header("Game Options UI Panel")] [SerializeField]
        private GameObject _gameOptionsUIPanel;

        [Header("Create Room UI Panel")] [SerializeField]
        private GameObject _createRoomUIPanel;

        [SerializeField] private GameObject _creatingRoomUIPanel;
        [SerializeField] private TMP_InputField _roomNameInputField;

        [Header("Inside Room UI Panel")] [SerializeField]
        private GameObject _insideRoomUIPanel;

        [SerializeField] private TextMeshProUGUI _roomInfoText;
        [SerializeField] private GameObject _playerListPrefab;
        [SerializeField] private GameObject _playerListContent;
        [SerializeField] private GameObject _startGameButton;
        [SerializeField] private TextMeshProUGUI _gameModeText;
        [SerializeField] private Image _insideRoomPanelBackground;
        [SerializeField] private Sprite _racingBackground;
        [SerializeField] private Sprite _deathRaceBackground;

        [Header("Room List UI Panel")] [SerializeField]
        private GameObject _roomListUIPanel;

        [SerializeField] private GameObject _roomListParentGameObject;
        [SerializeField] private GameObject _roomListEntryPrefab;

        [Header("Join Random Room UI Panel")] [SerializeField]
        private GameObject _joinRandomRoomUIPanel;

        private string _gameModeName;

        private List<GameObject> _panels = new List<GameObject>();
        private Dictionary<string, RoomInfo> _cachedRoomList = new Dictionary<string, RoomInfo>();
        private Dictionary<string, GameObject> _roomListGameObjects = new Dictionary<string, GameObject>();
        private Dictionary<int, GameObject> _playerListGameObjects = new Dictionary<int, GameObject>();

        #region Unity Methods

        private void Awake()
        {
            _panels.Add(_loginUIPanel);
            _panels.Add(_gameOptionsUIPanel);
            _panels.Add(_createRoomUIPanel);
            _panels.Add(_creatingRoomUIPanel);
            _panels.Add(_insideRoomUIPanel);
            _panels.Add(_connectingInfoUIPanel);
            //_panels.Add(_roomListUIPanel);
            _panels.Add(_joinRandomRoomUIPanel);

            PhotonNetwork.AutomaticallySyncScene = true;
        }

        // Start is called before the first frame update
        void Start()
        {
            ActivatePanel(_loginUIPanel);
        }

        // Update is called once per frame
        void Update()
        {
            _connectionStatusText.text = $"Connection Status: {PhotonNetwork.NetworkClientState}";
        }

        #endregion

        #region UI Callback Methods

        public void OnLoginButtonClicked()
        {
            string playerName = _playerNameInput.text;

            if (!string.IsNullOrEmpty(playerName))
            {
                ActivatePanel(_connectingInfoUIPanel);

                if (!PhotonNetwork.IsConnected)
                {
                    PhotonNetwork.LocalPlayer.NickName = playerName;
                    PhotonNetwork.ConnectUsingSettings();
                }
            }
            else
            {
                Debug.LogError("Player Name is invalid!");
            }
        }

        public void OnCreateRoomButtonClicked()
        {
            if (!string.IsNullOrEmpty(_gameModeName))
            {
                ActivatePanel(_creatingRoomUIPanel);

                string roomName = _roomNameInputField.text;

                if (string.IsNullOrEmpty(roomName))
                    roomName = $"Room {Random.Range(0, 100000)}";

                RoomOptions roomOptions = new RoomOptions();
                roomOptions.MaxPlayers = 3;
                string[] roomPropsInLobby = { MultiplayerRacingGame.GameModePropKey }; // gm = Game Mode
                //Two Game Modes
                //1. Racing = "rc"
                //2. Death Race = "dr"

                Hashtable customRoomProperties = new Hashtable
                    { { MultiplayerRacingGame.GameModePropKey, _gameModeName } };

                roomOptions.CustomRoomProperties = customRoomProperties;
                roomOptions.CustomRoomPropertiesForLobby = roomPropsInLobby;

                PhotonNetwork.CreateRoom(roomName, roomOptions);
            }
            else
            {
                Debug.LogError("You must select a game mode first!!!");
            }
        }

        public void OnCancelButtonClicked()
        {
            ActivatePanel(_gameOptionsUIPanel);
        }

        public void OnShowRoomListButtonClicked()
        {
            if (!PhotonNetwork.InLobby)
                PhotonNetwork.JoinLobby();

            ActivatePanel(_roomListUIPanel);
        }

        public void OnBackButtonClicked()
        {
            if (PhotonNetwork.InLobby)
                PhotonNetwork.LeaveLobby();

            ActivatePanel(_gameOptionsUIPanel);
        }

        public void OnLeaveGameButtonClicked()
        {
            PhotonNetwork.LeaveRoom();
        }

        public void OnJoinRandomRoomButtonClicked(bool isGameModeRacing)
        {
            SetGameMode(isGameModeRacing);

            if (!string.IsNullOrEmpty(_gameModeName))
            {
                Hashtable expectedCustomRoomProperties = new Hashtable
                    { { MultiplayerRacingGame.GameModePropKey, _gameModeName } };
                PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
            }
            else
            {
                Debug.LogError("Game Mode Name is Empty!!!");
            }
        }

        public void OnStartGameButtonClicked()
        {
            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(MultiplayerRacingGame.GameModePropKey))
            {
                if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue(MultiplayerRacingGame.RacingModeName))
                {
                    //Racing Mode
                    PhotonNetwork.LoadLevel(MultiplayerRacingGame.RacingSceneName);
                }
                else if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue(MultiplayerRacingGame
                             .DeathRaceModeName))
                {
                    //Death Race Mode
                    PhotonNetwork.LoadLevel(MultiplayerRacingGame.DeathRaceSceneName);
                }
            }
        }

        #endregion

        #region Photon Callbacks

        public override void OnConnected()
        {
            Debug.Log("OnConnected! Connected to Internet!");
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log(
                $"OnConnectedToMaster! {PhotonNetwork.LocalPlayer.NickName} is connected to Photon Master Server!");

            ActivatePanel(_gameOptionsUIPanel);
        }

        public override void OnCreatedRoom()
        {
            Debug.LogFormat($"OnCreatedRoom! {PhotonNetwork.CurrentRoom.Name} is created!");
        }

        public override void OnJoinedRoom()
        {
            Debug.LogFormat(
                $"OnJoinedRoom! {PhotonNetwork.LocalPlayer.NickName} joined to {PhotonNetwork.CurrentRoom.Name}! Player Count: {PhotonNetwork.CurrentRoom.PlayerCount}");

            ActivatePanel(_insideRoomUIPanel);

            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(MultiplayerRacingGame.GameModePropKey))
            {
                if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(MultiplayerRacingGame.GameModePropKey,
                        out object gameModeName))
                {
                    Debug.LogFormat($"Game Mode Name = {gameModeName}");

                    _roomInfoText.text =
                        $"Room Name: {PhotonNetwork.CurrentRoom.Name} Players/Max.Players: {PhotonNetwork.CurrentRoom.PlayerCount} / {PhotonNetwork.CurrentRoom.MaxPlayers}";

                    if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue(MultiplayerRacingGame.RacingModeName))
                    {
                        _insideRoomPanelBackground.sprite = _racingBackground;
                        _gameModeText.text = "Let's Race!";
                    }
                    else if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue(MultiplayerRacingGame
                                 .DeathRaceModeName))
                    {
                        _insideRoomPanelBackground.sprite = _deathRaceBackground;
                        _gameModeText.text = "Death Race!";
                    }

                    //Instantiate Player List GameObjects
                    foreach (Player player in PhotonNetwork.PlayerList)
                    {
                        GameObject playerListGameObject = Instantiate(_playerListPrefab, _playerListContent.transform);
                        playerListGameObject.transform.localScale = Vector3.one;

                        PlayerListEntryInitializer playerListEntryInitializer =
                            playerListGameObject.GetComponent<PlayerListEntryInitializer>();
                        playerListEntryInitializer.Initializer(player.ActorNumber, player.NickName);

                        if (player.CustomProperties.TryGetValue(MultiplayerRacingGame.IsPlayerReady,
                                out object isPlayerReady))
                        {
                            playerListEntryInitializer.SetPlayerReady((bool)isPlayerReady);
                        }

                        _playerListGameObjects.Add(player.ActorNumber, playerListGameObject);
                    }
                }
            }

            _startGameButton.SetActive(false);
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            Debug.LogFormat($"OnPlayerPropertiesUpdate! Target Player Name: {targetPlayer.NickName}");

            if (_playerListGameObjects.TryGetValue(targetPlayer.ActorNumber, out GameObject playerListGameObject))
            {
                if (changedProps.TryGetValue(MultiplayerRacingGame.IsPlayerReady, out object isPlayerReady))
                {
                    PlayerListEntryInitializer playerListEntryInitializer =
                        playerListGameObject.GetComponent<PlayerListEntryInitializer>();

                    playerListEntryInitializer.SetPlayerReady((bool)isPlayerReady);
                }
            }

            _startGameButton.SetActive(CheckPlayersReady());
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.LogFormat($"OnJoinRandomRoomFailed! Message: {message}");

            string roomName = _roomNameInputField.text;

            if (string.IsNullOrEmpty(roomName))
                roomName = $"Room {Random.Range(0, 100000)}";

            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 3;
            string[] roomPropsInLobby = { MultiplayerRacingGame.GameModePropKey }; // gm = Game Mode
            //Two Game Modes
            //1. Racing = "rc"
            //2. Death Race = "dr"

            Hashtable customRoomProperties = new Hashtable { { MultiplayerRacingGame.GameModePropKey, _gameModeName } };

            roomOptions.CustomRoomProperties = customRoomProperties;
            roomOptions.CustomRoomPropertiesForLobby = roomPropsInLobby;

            PhotonNetwork.CreateRoom(roomName, roomOptions);
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Debug.LogFormat($"OnPlayerEnteredRoom! Entered Player Name: {newPlayer.NickName}");

            //Update Room Info Text
            _roomInfoText.text =
                $"Room Name: {PhotonNetwork.CurrentRoom.Name} Players/Max.Players: {PhotonNetwork.CurrentRoom.PlayerCount} / {PhotonNetwork.CurrentRoom.MaxPlayers}";

            GameObject playerListGameObject = Instantiate(_playerListPrefab, _playerListContent.transform);
            playerListGameObject.transform.localScale = Vector3.one;

            PlayerListEntryInitializer playerListEntryInitializer =
                playerListGameObject.GetComponent<PlayerListEntryInitializer>();
            playerListEntryInitializer.Initializer(newPlayer.ActorNumber, newPlayer.NickName);

            _playerListGameObjects.Add(newPlayer.ActorNumber, playerListGameObject);

            _startGameButton.SetActive(CheckPlayersReady());
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug.LogFormat($"OnPlayerLeftRoom! Left Player Name: {otherPlayer.NickName}");

            //Update Room Info Text
            _roomInfoText.text =
                $"Room Name: {PhotonNetwork.CurrentRoom.Name} Players/Max.Players: {PhotonNetwork.CurrentRoom.PlayerCount} / {PhotonNetwork.CurrentRoom.MaxPlayers}";

            Destroy(_playerListGameObjects[otherPlayer.ActorNumber].gameObject);
            _playerListGameObjects.Remove(otherPlayer.ActorNumber);

            _startGameButton.SetActive(CheckPlayersReady());
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            Debug.LogFormat($"OnMasterClientSwitched! New Master Client Name: {newMasterClient.NickName}");

            if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
                _startGameButton.SetActive(CheckPlayersReady());
        }

        public override void OnLeftRoom()
        {
            Debug.Log("OnLeftRoom!");

            ActivatePanel(_gameOptionsUIPanel);

            foreach (GameObject playerListGameObject in _playerListGameObjects.Values)
                Destroy(playerListGameObject);

            _playerListGameObjects.Clear();
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            ClearRoomListView();

            foreach (RoomInfo roomInfo in roomList)
            {
                if (!roomInfo.IsOpen || !roomInfo.IsVisible || roomInfo.RemovedFromList)
                {
                    if (_cachedRoomList.ContainsKey(roomInfo.Name))
                        _cachedRoomList.Remove(roomInfo.Name);
                }
                else
                {
                    if (_cachedRoomList.ContainsKey(roomInfo.Name))
                        _cachedRoomList[roomInfo.Name] = roomInfo;
                    else
                        _cachedRoomList.Add(roomInfo.Name, roomInfo);
                }

                //_cachedRoomList[roomInfo.Name] = roomInfo;

                Debug.LogFormat($"In {PhotonNetwork.CurrentLobby.Name} Lobby - Room Name = {roomInfo.Name}");
            }

            foreach (RoomInfo roomInfo in _cachedRoomList.Values)
            {
                GameObject roomListEntryGameObject =
                    Instantiate(_roomListEntryPrefab, _roomListParentGameObject.transform);
                roomListEntryGameObject.transform.localScale = Vector3.one;

                roomListEntryGameObject.transform.Find("Text_RoomName").GetComponent<TextMeshProUGUI>().text =
                    roomInfo.Name;
                roomListEntryGameObject.transform.Find("Text_RoomPlayers").GetComponent<TextMeshProUGUI>().text =
                    $"{roomInfo.PlayerCount} / {roomInfo.MaxPlayers}";
                roomListEntryGameObject.transform.Find("Button_JoinRoom").GetComponent<Button>().onClick
                    .AddListener(() => OnJoinRoomButtonClicked(roomInfo.Name));

                _roomListGameObjects.Add(roomInfo.Name, roomListEntryGameObject);
            }
        }

        public override void OnLeftLobby()
        {
            Debug.LogFormat($"{PhotonNetwork.LocalPlayer.NickName} left lobby!");

            ClearRoomListView();
            _cachedRoomList.Clear();
        }

        public override void OnJoinedLobby()
        {
            string currentLobbyName = PhotonNetwork.CurrentLobby.Name;
            if (string.IsNullOrEmpty(currentLobbyName))
                currentLobbyName = "Default";

            Debug.LogFormat($"{PhotonNetwork.LocalPlayer.NickName} is joined to {currentLobbyName} lobby!");
        }

        #endregion

        #region Public Methods

        public void ActivatePanel(GameObject panelToBeActivated)
        {
            if (panelToBeActivated == null) return;

            string panelToBeActivatedName = panelToBeActivated.name;

            foreach (GameObject panel in _panels)
                panel.SetActive(panelToBeActivatedName.Equals(panel.name));
        }

        public void SetGameMode(bool isRacingModeSelected)
        {
            _gameModeName = isRacingModeSelected
                ? MultiplayerRacingGame.RacingModeName
                : MultiplayerRacingGame.DeathRaceModeName;
        }

        #endregion

        #region Private Methods

        private bool CheckPlayersReady()
        {
            if (!PhotonNetwork.IsMasterClient)
                return false;

            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (player.CustomProperties.TryGetValue(MultiplayerRacingGame.IsPlayerReady, out object isPlayerReady))
                {
                    if (!(bool)isPlayerReady)
                        return false;
                }
                else
                {
                    Debug.LogErrorFormat($"{player.NickName} has no IsPlayerReady CustomProperties!!!");
                    return false;
                }
            }

            return true;
        }

        private void OnJoinRoomButtonClicked(string roomName)
        {
            if (PhotonNetwork.InLobby)
                PhotonNetwork.LeaveLobby();

            PhotonNetwork.JoinRoom(roomName);
        }

        private void ClearRoomListView()
        {
            foreach (GameObject roomListGameObject in _roomListGameObjects.Values)
                Destroy(roomListGameObject);

            _roomListGameObjects.Clear();
        }

        #endregion
    }
}