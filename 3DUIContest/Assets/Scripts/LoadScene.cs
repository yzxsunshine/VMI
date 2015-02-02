using UnityEngine;
using System.Collections;

public class LoadScene : MonoBehaviour {
	public string remoteIP = "130.215.28.125";
	public string remotePort = "12005";
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI() {
#if UNITY_EDITOR || UNITY_STANDALONE
		if(GUI.Button(new Rect(10, 10, 100, 50), "Start Server")){
			Application.LoadLevel("SwingServerTest");
		}
#elif UNITY_ANDROID

		remoteIP = GUI.TextField(new Rect(10, 50, 300, 80), remoteIP);
		remotePort = GUI.TextField(new Rect(10, 150, 300, 80), remotePort);
		if(GUI.Button(new Rect(10, 250, 300, 80), "Connect To Server")) {
			GameObject networkInfo = GameObject.Find("NetworkInfo");
			networkInfo.GetComponent<PersistObj>().remoteIP = remoteIP;
			networkInfo.GetComponent<PersistObj>().remotePort = remotePort;
			Application.LoadLevel("SwingMobileClient");
		}
#endif
	}
}
