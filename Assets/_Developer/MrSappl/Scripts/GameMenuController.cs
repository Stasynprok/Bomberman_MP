using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenuController : MonoBehaviour
{
    [SerializeField] private GameObject _menu;
    [SerializeField] private Button _menuButton;
    [SerializeField] private Button _mainMenuButton;
    private GameParameters _gameParameters;


    private void OnEnable()
    {
        _menuButton.onClick.AddListener(ToggleMenuHandler);
        _mainMenuButton.onClick.AddListener(MainMenuHandler);
    }

    private void OnDisable()
    {
        _menuButton.onClick.RemoveListener(ToggleMenuHandler);
        _mainMenuButton.onClick.RemoveListener(MainMenuHandler);
    }
    public void ToggleMenuHandler()
    {
        _gameParameters.ToggleMenu = !_gameParameters.ToggleMenu;
        UpdateMenuWindow();
    }

    private void UpdateMenuWindow()
    {
        _menu.SetActive(_gameParameters.ToggleMenu || _gameParameters.GameOver);
    }

    public void MainMenuHandler()
    {
        SceneManager.LoadScene(sceneBuildIndex: 0);
        NetworkClient.Disconnect();
    }
}
