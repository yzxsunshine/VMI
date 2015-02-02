using UnityEngine;
using System;
using System.Collections;
using System.Threading;

public class ServerCtrl : MonoBehaviour {

	class ThreadController
	{
		public bool ShouldExecute { get; set; }
	}
	
	private MsgProcess msgProcess;
	private UDPServer server;
	private MusicClip curMC;
	private Thread receiveThread;
	private bool isRunning = false;
	private ThreadController threadController;
	
	// Use this for initialization
	void Start () {
		server = new UDPServer(12005);
		msgProcess = new MsgProcess();
		curMC = new MusicClip();

		threadController = new ThreadController();
		threadController.ShouldExecute = true;
		receiveThread = new Thread(receiveString);
		receiveThread.IsBackground = true;
		receiveThread.Start(threadController);
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Escape)) {
			threadController.ShouldExecute = false;
			Application.Quit();
		}
	}

	void OnGUI() {
		GUI.Label(new Rect(10, 10, 100, 50), "pitch="+curMC.pitch);
		GUI.Label(new Rect(10, 60, 100, 50), "volume="+curMC.volume);
		GUI.Label(new Rect(10, 110, 100, 50), "frame="+curMC.frame);
		GUI.Label(new Rect(10, 160, 100, 50), "status="+curMC.status);
	}

	void receiveString(object data) {
		try {
			ThreadController tc = (ThreadController)data;
			while(tc.ShouldExecute) {
				//Thread.Sleep(10);
				string msg = server.receiveString();
				if(msg.Length < 1)
					continue;
				MusicClip mc = MusicClip.Decode(msg);
				curMC = mc;
			}
		}
		catch(Exception e) {
			Debug.Log("Thread Error");		
		}
	}

	void OnDestroy(){
		threadController.ShouldExecute = false;
	}
}
