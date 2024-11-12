using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputNicknameController : MonoBehaviour
{
    public TMP_InputField InputNickname;
    [SerializeField] private GameObject _errorNickNameObject;

    private void OnEnable()
    {
        _errorNickNameObject.SetActive(false);
    }
    public void ErrorNickname()
    {
        _errorNickNameObject.SetActive(true);
    }
}
