using System.Collections;
using System.Collections.Generic;
using Dobro.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LoginScreen : MonoBehaviour
{
    [SerializeField] private TMP_InputField emailField;
    [SerializeField] private TMP_InputField passwordField;
    
    [SerializeField] private TextMeshProUGUI errorText;

    public void Login()
    {
        if (IsValidEmail(emailField.text) && IsValidPassword(passwordField.text))
        {
            //Client.instance.Login()
            LoadMainMenu();
        }
    }
    
    public void Register()
    {
        if (IsValidEmail(emailField.text) && IsValidPassword(passwordField.text))
        {
            //Client.instance.Register()
            LoadMainMenu();
        }
    }

    private bool IsValidPassword(string password)
    {
        switch (password.Length)
        {
            case < 8:
                ShowErrorMessage("The password must be at least 8 characters long.");
                return false;
            case > 32:
                ShowErrorMessage("The password can't be longer than 32 characters.");
                return false;
            default:
                return true;
        }
    }
    
    private bool IsValidEmail(string email)
    {
        if (TestEmail.IsEmail(email))
        {
            return true;
        }
        ShowErrorMessage("Invalid email address.");
        return false;
    }

    private void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void ShowErrorMessage(string error)
    {
        errorText.text = error;
    }
}
