using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListEntryInitializer : MonoBehaviour
{
    [Header("UI References")] 
    public TextMeshProUGUI _playerNameText;
    public Button _playerReadyButton;
    public Image _playerReadyImage;

    public void Initializer(int playerID, string playerName)
    {
        _playerNameText.text = playerName;
    }

}
