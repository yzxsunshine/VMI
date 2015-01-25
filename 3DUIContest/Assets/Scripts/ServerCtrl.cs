using UnityEngine;
using System;
using System.Collections;
using System.Threading;


public class ServerCtrl : MonoBehaviour {
	private MsgProcess msgProcess;
	private UDPServer server;
	private MusicClip curMC;
	private Thread receiveThread;
	private bool isRunning = false;
	
	// Use this for initialization
	void Start () {
		server = new UDPServer(12005);
		msgProcess = new MsgProcess();
		curMC = new MusicClip();
		receiveThread = new Thread(
			new ThreadStart(receiveString));
		receiveThread.IsBackground = true;
		isRunning = true;
		receiveThread.Start();
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnGUI() {
		GUI.Label(new Rect(10, 10, 100, 50), "pitch="+curMC.pitch);
		GUI.Label(new Rect(10, 60, 100, 50), "volume="+curMC.volume);
		GUI.Label(new Rect(10, 110, 100, 50), "frame="+curMC.frame);
		GUI.Label(new Rect(10, 160, 100, 50), "status="+curMC.status);
	}

	void receiveString() {
		try {
			while(isRunning) {
				string msg = server.receiveString();
				MusicClip mc = msgProcess.Decode(msg);
				curMC = mc;
			}
		}
		catch(Exception e) {
			Debug.Log("Thread Error");		
		}
	}

	void OnDestroy(){
		receiveThread.Abort();
	}
}
