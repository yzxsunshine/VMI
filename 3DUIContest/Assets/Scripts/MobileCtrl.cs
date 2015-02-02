using UnityEngine;
using System.Collections;
using System;
using System.Text;

public class MobileCtrl : MonoBehaviour {
	private Rect ribbonRect;
	private Rect menuRect;
	private Rect minus1Rect;
	private Rect minus2Rect;
	private Rect minus3Rect;
	private Rect plus1Rect;
	private Rect plus2Rect;
	private Rect plus3Rect;
	private Rect majorChordRect;
	private Rect minorChordRect;
	private string txt = "";
	private Vector3 defaultOrient = new Vector3(0, 0, 0);
	private Vector3 debugOrientation = new Vector3(0, 0, 0);
	// Use this for initialization
	private SensorFusion senfu;
	private UDPSend udpClient;
	private string remoteIP;
	private string remotePort;
	void Start () {
		InitGUIRects();

		senfu = new SensorFusion();
		senfu.Start();
		udpClient = new UDPSend();
		GameObject networkInfo = GameObject.Find("NetworkInfo");
		remoteIP = networkInfo.GetComponent<PersistObj>().remoteIP;
		remotePort = networkInfo.GetComponent<PersistObj>().remotePort;
		udpClient.SetIPPort(remoteIP, Convert.ToInt32(remotePort));
		DestroyObject(networkInfo);
	}

	void InitGUIRects() {
		float widthUnit = Screen.width / 13;
		float heightUnit = Screen.height / 30;
		ribbonRect = new Rect(0, heightUnit * 19, Screen.width, heightUnit * 6);
		menuRect = new Rect(widthUnit, heightUnit*27, widthUnit * 5, heightUnit * 3);
		minus1Rect = new Rect(widthUnit, heightUnit * 13, widthUnit * 3, heightUnit * 4);
		minus2Rect = new Rect(widthUnit * 5, heightUnit * 13, widthUnit * 3, heightUnit * 4);
		minus3Rect = new Rect(widthUnit * 9, heightUnit * 13, widthUnit * 3, heightUnit * 4);
		plus1Rect = new Rect(widthUnit, heightUnit * 7, widthUnit * 3, heightUnit * 4);
		plus2Rect = new Rect(widthUnit * 5, heightUnit * 7, widthUnit * 3, heightUnit * 4);
		plus3Rect = new Rect(widthUnit * 9, heightUnit * 7, widthUnit * 3, heightUnit * 4);
		majorChordRect = new Rect(widthUnit, heightUnit, widthUnit * 5f, heightUnit * 4);
		minorChordRect = new Rect(widthUnit * 7, heightUnit, widthUnit * 5f, heightUnit * 4);

		Vector3 pos = new Vector3(0, 0, 1);
		GameObject ribbon = Instantiate(Resources.Load("Prefab/ribbon", typeof(GameObject)), pos, Quaternion.identity) as GameObject;
		ribbon.guiTexture.pixelInset = ribbonRect;

		GameObject menuObj = Instantiate(Resources.Load("Prefab/menu", typeof(GameObject)), pos, Quaternion.identity) as GameObject;
		menuObj.guiTexture.pixelInset = menuRect;

		GameObject minus1 = Instantiate(Resources.Load("Prefab/minus1", typeof(GameObject)), pos, Quaternion.identity) as GameObject;
		minus1.guiTexture.pixelInset = minus1Rect;

		GameObject minus2 = Instantiate(Resources.Load("Prefab/minus2", typeof(GameObject)), pos, Quaternion.identity) as GameObject;
		minus2.guiTexture.pixelInset = minus2Rect;

		GameObject minus3 = Instantiate(Resources.Load("Prefab/minus3", typeof(GameObject)), pos, Quaternion.identity) as GameObject;
		minus3.guiTexture.pixelInset = minus3Rect;

		GameObject plus1 = Instantiate(Resources.Load("Prefab/plus1", typeof(GameObject)), pos, Quaternion.identity) as GameObject;
		plus1.guiTexture.pixelInset = plus1Rect;

		GameObject plus2 = Instantiate(Resources.Load("Prefab/plus2", typeof(GameObject)), pos, Quaternion.identity) as GameObject;
		plus2.guiTexture.pixelInset = plus2Rect;

		GameObject plus3 = Instantiate(Resources.Load("Prefab/plus3", typeof(GameObject)), pos, Quaternion.identity) as GameObject;
		plus3.guiTexture.pixelInset = plus3Rect;

		GameObject major = Instantiate(Resources.Load("Prefab/major", typeof(GameObject)), pos, Quaternion.identity) as GameObject;
		major.guiTexture.pixelInset = majorChordRect;

		GameObject minor = Instantiate(Resources.Load("Prefab/minor", typeof(GameObject)), pos, Quaternion.identity) as GameObject;
		minor.guiTexture.pixelInset = minorChordRect;

	}
	
	// Update is called once per frame
	void Update () {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN 

		Vector3 diffVec = debugOrientation - defaultOrient;
		int clipIdx = 0;
		MusicClip mc = new MusicClip();

		if(Input.GetMouseButtonDown(0)) {
			Vector3 pt = Input.mousePosition;
			if(ribbonRect.Contains(pt)) {
				txt = "click";
			}
			else if(menuRect.Contains(pt)) {
				
			}
			else if(minus1Rect.Contains(pt)) {
				mc.chords.Add(-1);
			}
			else if(minus2Rect.Contains(pt)) {
				mc.chords.Add(-2);
			}
			else if(minus3Rect.Contains(pt)) {
				mc.chords.Add(-3);
			}
			else if(plus1Rect.Contains(pt)) {
				mc.chords.Add(1);
			}
			else if(plus2Rect.Contains(pt)) {
				mc.chords.Add(2);
			}
			else if(plus3Rect.Contains(pt)) {
				mc.chords.Add(3);
			}
			else if(majorChordRect.Contains(pt)) {
				mc.chords.Add(0);
				mc.chords.Add(4);
				mc.chords.Add(7);
			}
			else if(minorChordRect.Contains(pt)) {
				mc.chords.Add(0);
				mc.chords.Add(3);
				mc.chords.Add(7);
			}
			if(mc.chords.Count == 0) {
				mc.chords.Add(0);
			}
		}
		 
#elif UNITY_ANDROID

		txt = "none";
		senfu.Update();
		txt = "update";
		Vector3 diffVec = senfu.Orientation - defaultOrient;
		MusicClip mc = new MusicClip();
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
				if (mc.status == CLIP_STATUS.CLIP_EMPTY) {
					mc.status = status;
					mc.pitch = pitch;
					mc.volume = volume;
				}
			}
			else if(menuRect.Contains(pt)) {
				
			}
			else if(minus1Rect.Contains(pt)) {
				mc.chords.Add(-1);
			}
			else if(minus2Rect.Contains(pt)) {
				mc.chords.Add(-2);
			}
			else if(minus3Rect.Contains(pt)) {
				mc.chords.Add(-3);
			}
			else if(plus1Rect.Contains(pt)) {
				mc.chords.Add(1);
			}
			else if(plus2Rect.Contains(pt)) {
				mc.chords.Add(2);
			}
			else if(plus3Rect.Contains(pt)) {
				mc.chords.Add(3);
			}
			else if(majorChordRect.Contains(pt)) {
				mc.chords.Add(0);
				mc.chords.Add(4);
				mc.chords.Add(7);
			}
			else if(minorChordRect.Contains(pt)) {
				mc.chords.Add(0);
				mc.chords.Add(3);
				mc.chords.Add(7);
			}
			if(mc.chords.Count == 0) {
				mc.chords.Add(0);
			}
		}
		if(mc.chords.Count == 0) {
			mc.chords.Add(0);
		}
		string msg = MusicClip.Encode(mc);
		udpClient.sendString(msg);
		txt = msg;
#endif

	}

	void OnGUI() {
		GUI.Label(new Rect(10, 10, 100, 30), txt);

	}
}
