using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class NetworkDebug : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if(!PhotonNetwork.IsConnected)
            Debug.Log("Player has not connected to Photon yet!!!");
        else
        {
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerRacingGame.PlayerSelectionNumber,
                    out object playerSelectionNumber))
            {
                Debug.LogFormat($"Network Debug! {PhotonNetwork.LocalPlayer.NickName}. Player Selection Number: {((int) playerSelectionNumber).ToString()}");
            }
            else
            {
                Debug.LogErrorFormat($"Network Debug! {PhotonNetwork.LocalPlayer.NickName} has not PlayerSelectionNumber!!!");
            }
        }
    }
}
