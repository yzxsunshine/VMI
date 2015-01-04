using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class VMIClient : MonoBehaviour {
	UdpClient udpc;
	IPEndPoint ep = null;
	public string serverIP = "192.168.1.7";
	// Use this for initialization
	void Start () {
		udpc = new UdpClient(serverIP, 12055);
	}
	
	// Update is called once per frame
	void SendMsg (int pitch, int frame) {
		string msg = String.Format("{0} {0}", pitch,frame);
		byte[] sdata = Encoding.ASCII.GetBytes(msg);
		udpc.Send(sdata,sdata.Length);
	}
}
