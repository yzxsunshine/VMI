using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneControl : MonoBehaviour {
	private SensorFusion senfu = null;
	private Rect ctrlPanel;
	private Rect scenePanel;
	private List<Vector2> starList;
	private Vector3 defaultOrient = new Vector3(0, 0, 0);
	private Vector3 defaultCameraPos;
	private Vector3 debugOrientation;
	// Use this for initialization
	void Start () {
		defaultCameraPos = Camera.main.transform.position;
		debugOrientation = new Vector3(0, 0, 0);
#if !UNITY_EDITOR
		senfu = new SensorFusion();
		senfu.Start();
#endif
		CreateScenePanel();
		CreateCtrlPanel();
	}

	// Update is called once per frame
	void Update () {
		//senfu.Update();
#if !UNITY_EDITOR
		senfu.Update();
		Vector3 diffVec = senfu.Orientation - defaultOrient;
		//KeyTouch.SetVolume((int)(50 + 30 * diffVec.x));
		if(Input.touchCount <= 0) {
			Camera.main.transform.position = new Vector3(defaultCameraPos.x + 10.0f * diffVec.y, defaultCameraPos.y, defaultCameraPos.z);
		}
#else
		if(Input.GetKeyDown(KeyCode.UpArrow)) {
			debugOrientation.x += 0.1f;
		}
		if(Input.GetKeyDown(KeyCode.DownArrow)) {
			debugOrientation.x -= 0.1f;
		}
		if(Input.GetKeyDown(KeyCode.LeftArrow)) {
			debugOrientation.y += 0.1f;
		}
		if(Input.GetKeyDown(KeyCode.RightArrow)) {
			debugOrientation.y -= 0.1f;
		}
		Vector3 diffVec = debugOrientation - defaultOrient;
		//KeyTouch.SetVolume((int)(50 + 30 * diffVec.x));
		if(Input.touchCount <= 0) {
			Camera.main.transform.position = new Vector3(defaultCameraPos.x + 10.0f * diffVec.y, defaultCameraPos.y, defaultCameraPos.z);
		}
#endif
	}

	void OnGUI ()
	{
		//GUI.Label(new Rect(10, 10, 500, 20), senfu.Orientation.ToString());
		// End the Groups and Area	
		//GUI.Box(ctrlPanel, " ");
		Event e = Event.current;
		if (e.isKey)
			Debug.Log("Detected key code: " + e.keyCode);	
	}

	void CreateCtrlPanel() {
		float widthUnit = Screen.width * 1.0f / 16;
		float heightUnit = Screen.height * 1.0f / 9;
		ctrlPanel = new Rect(0,0,0,0);//(widthUnit * 11.5f, heightUnit * 0.5f, widthUnit * 4, heightUnit * 8);
		Vector3 pos = new Vector3((ctrlPanel.x + 50) / Screen.width, (ctrlPanel.y + ctrlPanel.height - 50) / Screen.height, 1);
		//GameObject star = Instantiate(Resources.Load("Prefab/starGUI", typeof(GameObject)), pos, Quaternion.identity) as GameObject;
		//star.guiTexture.pixelInset.Set(-25, -25, 50, 50);
	}

	void DrawCtrlPanel() {
		GUI.Box(ctrlPanel, "haha");

	}

	void CreateScenePanel() {
		float widthUnit = Screen.width * 1.0f / 16;
		float heightUnit = Screen.height * 1.0f / 9;
		scenePanel = new Rect(0, 0, widthUnit * 11.5f, Screen.height);
		StarCtrl.scenePanel = scenePanel;
	}
}
