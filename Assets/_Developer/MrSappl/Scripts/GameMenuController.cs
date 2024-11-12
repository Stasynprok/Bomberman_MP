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
        GameParameters.Instance.ToggleMenu = !GameParameters.Instance.ToggleMenu;
        UpdateMenuWindow();
    }

    private void UpdateMenuWindow()
    {
        _menu.SetActive(GameParameters.Instance.ToggleMenu || GameParameters.Instance.GameOver);
    }

    public void MainMenuHandler()
    {
        SceneManager.LoadScene(sceneBuildIndex: 0);
        NetworkClient.Disconnect();
    }
}
