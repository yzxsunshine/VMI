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

public class UDPServer
{
    private static int localPort;

    // prefs
   	public int port = 12005;  // define in init

    // "connection" things
    IPEndPoint ipep;
	IPEndPoint receiver;
	UdpClient server = null;

	public UDPServer (int sport) {
		SetPort (port);
	}
    // init
    public void init()
    {
		// ----------------------------
        // Senden
        // ----------------------------

		ipep = new IPEndPoint(IPAddress.Any, port);
		receiver = new IPEndPoint(IPAddress.Any, 0);
		if(server != null) {
			server.Close();
		}
		server = new UdpClient(ipep);

        // status
		Debug.Log("Server started at port=" + port);
    }

	public void SetPort(int sPort) {
		port = sPort;
		init();
	}

	public string receiveString() {
		byte[] rdata = server.Receive(ref receiver);
		string msg = Encoding.ASCII.GetString(rdata);
		return msg;
	}

    // sendData
    public void sendString(string message)
    {
        try
        {
            // Daten mit der UTF8-Kodierung in das Binärformat kodieren.
			byte[] data = Encoding.ASCII.GetBytes(message);

            // Den message zum Remote-Client senden.
			server.Send(data, data.Length, receiver);
            //}
        }
        catch (Exception err)
        {
            Debug.Log(err.ToString());
        }
    }
}