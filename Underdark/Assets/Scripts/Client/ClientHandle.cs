using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{
    private static TextMeshProUGUI logStatic;
    [SerializeField] private TextMeshProUGUI log;
    
    private void Awake()
    {
        logStatic = log;
    }

    public static void Welcome(Packet _packet)
    {
        string _msg = _packet.ReadString();
        int _myId = _packet.ReadInt();

        logStatic.text += $"Message from server: {_msg}\n";
        Debug.Log($"Message from server: {_msg}");
        Client.instance.myId = _myId;
        ClientSend.WelcomeReceived();

        Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
    }

    public static void UDPTest(Packet _packet)
    {
        string _msg = _packet.ReadString();

        logStatic.text += $"Received packet via UDP. Contains message: {_msg}\n";
        Debug.Log($"Received packet via UDP. Contains message: {_msg}");
        ClientSend.UDPTestReceived();
    }
    
    public static void Register(Packet _packet)
    {
        string _msg = _packet.ReadString();
        bool isRegistrationValid = bool.Parse(_msg);

        LoginScreen.Instance.RegisterCallBack(isRegistrationValid);
        
        logStatic.text += $"Register" + isRegistrationValid;
        Debug.Log($"Register"+ isRegistrationValid);
    }
    
    public static void Login(Packet _packet)
    {
        string _msg = _packet.ReadString();
        bool isRegistrationValid = bool.Parse(_msg);

        LoginScreen.Instance.LoginCallBack(isRegistrationValid);

        logStatic.text += $"Login";
        Debug.Log($"Login");
    }
}
