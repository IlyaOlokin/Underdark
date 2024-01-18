﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend
{
    private static void SendTCPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.tcp.SendData(_packet);
    }

    private static void SendUDPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.udp.SendData(_packet);
    }

    #region Packets
    public static void WelcomeReceived()
    {
        using (Packet _packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            _packet.Write(Client.instance.myId);
            _packet.Write("text");

            SendTCPData(_packet);
        }
    }

    public static void UDPTestReceived()
    {
        using (Packet _packet = new Packet((int)ClientPackets.updTestReceived))
        {
            _packet.Write("Received a UDP packet.");

            SendUDPData(_packet);
        }
    }
    
    public static void RegisterReceived(string email, string password)
    {
        using (Packet _packet = new Packet((int)ClientPackets.registerReceived))
        {
            _packet.Write(email);
            _packet.Write(password);

            SendTCPData(_packet);
        }
    }
    
    public static void LoginReceived(string email, string password)
    {
        using (Packet _packet = new Packet((int)ClientPackets.loginReceived))
        {
            _packet.Write(email);
            _packet.Write(password);

            SendTCPData(_packet);
        }
    }
    #endregion
}
