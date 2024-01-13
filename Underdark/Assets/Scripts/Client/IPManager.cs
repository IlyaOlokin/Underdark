using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using UnityEngine;

public class IPManager
{
    public static string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new Exception("No network adapters with an IPv4 address in the system!");
    }
    
    public static string GetGlobalIPAddress()
    {
        var url = "https://api.ipify.org/";

        WebRequest request = WebRequest.Create(url);
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();

        Stream dataStream = response.GetResponseStream();

        using StreamReader reader = new StreamReader(dataStream);

        var ip = reader.ReadToEnd();
        reader.Close();

        return ip;
    }
}