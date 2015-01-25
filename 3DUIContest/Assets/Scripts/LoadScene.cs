using UnityEngine;
using System.Collections;

public class LoadScene : MonoBehaviour {

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
		if(GUI.Button(new Rect(10, 10, 100, 50), "Connect to Server")){
			Application.LoadLevel("SwingMobileClient");
		}
#endif
	}
}
