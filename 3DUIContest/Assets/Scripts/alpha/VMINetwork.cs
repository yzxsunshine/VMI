using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Configuration;

public class VMINetwork : MonoBehaviour {
	UdpClient udpc;
	IPEndPoint ep = null;
	public string serverIP = "130.215.28.28";
	private int port = 12055;
	// Use this for initialization
	void Start () {
#if (UNITY_EDITOR || UNITY_STANDALONE)
		udpc = new UdpClient(12055);
		Debug.Log("Server started, servicing on port 12055");
#elif UNITY_ANDROID
		udpc = new UdpClient(serverIP, 12055);
#endif
	}
	
	// Update is called once per frame
	void Update () {
#if (UNITY_EDITOR || UNITY_STANDALONE)
		byte[] rdata = udpc.Receive(ref ep);
		string data = Encoding.ASCII.GetString(rdata);
		var items = data.Split(' ');
		int pitch = Convert.ToInt32(items[0]);
		int frame = Convert.ToInt32(items[1]);
		GameObject.Find("sky").GetComponent<SkyCtrl>().AddStar(pitch, frame);
#endif
	}
	

	public void SendMsg (int pitch, int frame) {
		string msg = String.Format("{0} {0}", pitch,frame);
		byte[] sdata = Encoding.ASCII.GetBytes(msg);
		udpc.Send(sdata,sdata.Length);
	}
}
