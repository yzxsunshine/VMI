using UnityEngine;
using System.Collections;

public class MobileCtrl : MonoBehaviour {
	private Rect ribbonRect;
	private string txt = "";
	private Vector3 defaultOrient = new Vector3(0, 0, 0);
	private Vector3 debugOrientation = new Vector3(0, 0, 0);
	// Use this for initialization
	private SensorFusion senfu;
	private UDPSend udpClient;
	private MsgProcess msgProcess;
	void Start () {
		ribbonRect = new Rect(0, Screen.height * 0.6f, Screen.width, Screen.height / 5);
		Vector3 pos = new Vector3(0, 0, 1);
		GameObject ribbon = Instantiate(Resources.Load("Prefab/ribbon", typeof(GameObject)), pos, Quaternion.identity) as GameObject;
		ribbon.guiTexture.pixelInset = ribbonRect;

		senfu = new SensorFusion();
		senfu.Start();
		udpClient = new UDPSend();
		udpClient.SetIPPort("130.215.28.28", 12005);
		msgProcess = new MsgProcess();
	}
	
	// Update is called once per frame
	void Update () {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN 

		Vector3 diffVec = debugOrientation - defaultOrient;

		if(Input.GetMouseButtonDown(0)) {
			Vector3 pt = Input.mousePosition;
			if(ribbonRect.Contains(pt)) {
				txt = "click";

			}
		}
		 
#elif UNITY_ANDROID

		txt = "none";
		senfu.Update();
		txt = "update";
		Vector3 diffVec = senfu.Orientation - defaultOrient;

		for (int i=0; i < Input.touchCount; i++) {
			Vector2 pt = Input.GetTouch(i).position;
			if(ribbonRect.Contains(pt)) {
				CLIP_STATUS status = CLIP_STATUS.CLIP_BEGIN;
				switch (Input.GetTouch(i).phase) {
				case TouchPhase.Began:
					status = CLIP_STATUS.CLIP_BEGIN;
					break;
				case TouchPhase.Ended:
					status = CLIP_STATUS.CLIP_END;
					break;
				case TouchPhase.Moved:
					status = CLIP_STATUS.CLIP_CHANGE;
					break;
				}
				float pitch = diffVec.y;
				float volume = pt.x/Screen.width;
				string msg = msgProcess.Encode(pitch, volume, status);
				udpClient.sendString(msg);
				txt = msg;
				break;
			}
		}
#endif

	}

	void OnGUI() {
		GUI.Label(new Rect(10, 10, 100, 30), txt);
	}
}
