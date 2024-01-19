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
    public static LoginScreen Instance;
    
    [SerializeField] private TMP_InputField emailField;
    [SerializeField] private TMP_InputField passwordField;
    
    [SerializeField] private TextMeshProUGUI errorText;
    
    [Header("Buttons")]
    [SerializeField] private Button registerButton;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button tryConnectButton;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    private void Start()
    {
        ConnectToSever();
        
        if (DataLoader.metaGameData == null) return;
        
        if (!DataLoader.metaGameData.Email.Equals(""))
            emailField.text = DataLoader.metaGameData.Email;
        if (!DataLoader.metaGameData.Password.Equals(""))
            passwordField.text = DataLoader.metaGameData.Password;
    }

    public void Login()
    {
        SetInputFieldsActive(false);
        DataLoader.SaveMetaData(emailField.text, passwordField.text);
        
        if (IsValidEmail(emailField.text) && IsValidPassword(passwordField.text))
        {
            try
            {
                ClientSend.LoginReceived(emailField.text, passwordField.text);
            }
            catch
            {
                ActivateReconnectMenu();
                throw;
            }
        }
    }
    
    public void Register()
    {
        SetInputFieldsActive(false);
        DataLoader.SaveMetaData(emailField.text, passwordField.text);
        
        if (IsValidEmail(emailField.text) && IsValidPassword(passwordField.text))
        {
            try
            {
                ClientSend.RegisterReceived(emailField.text, passwordField.text);
            }
            catch
            {
                ActivateReconnectMenu();
                throw;
            }
        }
    }

    public void RegisterCallBack(bool isRegistrationValid)
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
    
    public void LoginCallBack(bool isLoginValid)
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

    private void SetInputFieldsActive(bool isActive)
    {
        emailField.interactable = isActive;
        passwordField.interactable = isActive;
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
        StaticSceneLoader.LoadScene("MainMenu");
    }

    private void ShowErrorMessage(string error)
    {
        SetInputFieldsActive(true);
        errorText.text = error;
    }

    public void ConnectToSever()
    {
        Client.instance.ConnectToServer();
    }

    public void ActivateReconnectMenu()
    {
        ShowErrorMessage("Connection error");
        registerButton.gameObject.SetActive(false);
        loginButton.gameObject.SetActive(false);

        tryConnectButton.gameObject.SetActive(true);
    }
    public void DeactivateReconnectMenu()
    {
        ShowErrorMessage("");
        registerButton.gameObject.SetActive(true);
        loginButton.gameObject.SetActive(true);

        tryConnectButton.gameObject.SetActive(false);
    }
    
}
