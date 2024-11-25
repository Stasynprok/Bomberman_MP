using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ButtonScript : MonoBehaviour
{
    [SerializeField] private GameObject _mainMenuCanvas;
    public void ChangeScene() {
        SceneManager.LoadScene(sceneBuildIndex: 1);
    }

    public void QuitApplication() {
        Application.Quit();
    }

    public void DeactivateMainMenuCanvas()
    {
        _mainMenuCanvas.SetActive(false);
    }
}
