using System;
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
    private static TextMeshProUGUI errorTextStatic;

    private void Awake()
    {
        errorTextStatic = errorText;
    }

    public void Login()
    {
        if (IsValidEmail(emailField.text) && IsValidPassword(passwordField.text))
        {
            ClientSend.LoginReceived(emailField.text, passwordField.text);
        }
    }
    
    public void Register()
    {
        if (IsValidEmail(emailField.text) && IsValidPassword(passwordField.text))
        {
            ClientSend.RegisterReceived(emailField.text, passwordField.text);
        }
    }

    public static void RegisterCallBack(bool isRegistrationValid)
    {
        if (isRegistrationValid)
        {
            LoadMainMenu();
        }
        else
        {
            ShowErrorMessage("This email is already registered.");
        }
    }
    
    public static void LoginCallBack(bool isLoginValid)
    {
        if (isLoginValid)
        {
            LoadMainMenu();
        }
        else
        {
            ShowErrorMessage("Incorrect email or password.");
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
        return true;
        
        if (TestEmail.IsEmail(email))
        {
            return true;
        }
        ShowErrorMessage("Invalid email address.");
        return false;
    }

    private static void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private static void ShowErrorMessage(string error)
    {
        errorTextStatic.text = error;
    }
}
