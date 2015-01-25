/*
 
    -----------------------
    UDP-Send
    -----------------------
    // [url]http://msdn.microsoft.com/de-de/library/bb979228.aspx#ID0E3BAC[/url]
   
    // > gesendetes unter
    // 127.0.0.1 : 8050 empfangen
   
    // nc -lu 127.0.0.1 8050
 
        // todo: shutdown thread at the end
*/
using UnityEngine;
using System.Collections;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UDPSend
{
    private static int localPort;

    // prefs
    public string IP = "127.0.0.1";  // define in init
    public int port = 12005;  // define in init

    // "connection" things
    IPEndPoint remoteEndPoint;
	UdpClient client = null;


    // init
    public void init()
    {
		// ----------------------------
        // Senden
        // ----------------------------

		remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), port);
		if(client != null) {
			client.Close();
		}
		client = new UdpClient();

        // status
		Debug.Log("connected to IP=" + IP + ", port=" + port);
    }

	public void SetIPPort(string sIP, int sPort) {
		IP = sIP;
		port = sPort;
		init();
	}

	public string receiveString() {
		byte[] rdata = client.Receive(ref remoteEndPoint);
		string msg = Encoding.ASCII.GetString(rdata);
		return msg;
	}

    // sendData
    public void sendString(string message)
    {
        try
        {
            // Daten mit der UTF8-Kodierung in das Binärformat kodieren.
            byte[] data = Encoding.UTF8.GetBytes(message);

            // Den message zum Remote-Client senden.
            client.Send(data, data.Length, remoteEndPoint);
            //}
        }
        catch (Exception err)
        {
            Debug.Log(err.ToString());
        }
    }
}