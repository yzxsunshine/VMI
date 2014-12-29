using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneControl : MonoBehaviour {
	//private SensorFusion senfu = null;
	private Rect ctrlPanel;
	private Rect scenePanel;
	private List<Vector2> starList;
	// Use this for initialization
	void Start () {
		//senfu = new SensorFusion();
		//senfu.Start();
		CreateScenePanel();
		CreateCtrlPanel();
	}

	// Update is called once per frame
	void Update () {
		//senfu.Update();
	}

	void OnGUI ()
	{
		//GUI.Label(new Rect(10, 10, 500, 20), senfu.Orientation.ToString());
		// End the Groups and Area	
		Event e = Event.current;
		if (e.isKey)
			Debug.Log("Detected key code: " + e.keyCode);	
		DrawCtrlPanel();
	}

	void CreateCtrlPanel() {
		float widthUnit = Screen.width * 1.0f / 16;
		float heightUnit = Screen.height * 1.0f / 9;
		ctrlPanel = new Rect(widthUnit * 11.5f, heightUnit * 0.5f, widthUnit * 4, heightUnit * 8);
		Vector3 pos = new Vector3((ctrlPanel.x + 50) / Screen.width, (ctrlPanel.y + ctrlPanel.height - 50) / Screen.height, 0);
		GameObject star = Instantiate(Resources.Load("Prefab/starGUI", typeof(GameObject)), pos, Quaternion.identity) as GameObject;
		star.guiTexture.pixelInset.Set(-25, -25, 50, 50);
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
