using System.Collections.Generic;
using UnityEngine;


public class PlayerSelection : MonoBehaviour
{
    [SerializeField] private List<GameObject> _selectablePlayers = new List<GameObject>();

    private int _playerSelectionNumber = 0;

    private void ActivatePlayer(int index)
    {
        foreach (GameObject selectablePlayer in _selectablePlayers)
            selectablePlayer.SetActive(false);

        _selectablePlayers[index].SetActive(true);
    }
    
    public void NextPlayer()
    {
        _playerSelectionNumber++;

        if (_playerSelectionNumber >= _selectablePlayers.Count)
            _playerSelectionNumber = 0;

        ActivatePlayer(_playerSelectionNumber);
    }

    public void PreviousPlayer()
    {
        _playerSelectionNumber--;

        if (_playerSelectionNumber < 0)
            _playerSelectionNumber = _selectablePlayers.Count - 1;

        ActivatePlayer(_playerSelectionNumber);
    }
}