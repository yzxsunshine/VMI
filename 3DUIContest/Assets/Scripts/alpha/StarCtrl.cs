using UnityEngine;
using System.Collections;
using UnityEngine.Events;
// part of the code is from http://wiki.unity3d.com/index.php?title=DraggableGUIElement

public enum STAR_STATUS {
	EDIT_IN_PANEL,
	EDIT_DRAGGING,
	EDIT_IN_SCENE,
	EDIT_IN_SCENE_SELECTED,
	PLAY_CURRENT,
	PLAY_NEXT,
	PLAY_PAST,
	PLAY_FUTURE
};

public class StarCtrl : MonoBehaviour {
	Vector3 lastMousePosition;
	Rect border;
	STAR_STATUS status;
	bool isSharp;	// is one degree higher for some notes
	Vector3 targetPosition;
	bool autoMove = false;
	float maxPosChange = 0.01f;
	Texture[] starTex;

	public static Rect scenePanel;

	// Use this for initialization
	void Start () {
		border = new Rect(0, 0, Screen.width, Screen.height);
		status = STAR_STATUS.EDIT_IN_SCENE;
		starTex = new Texture[3];	// 0: single note; 1: selected single note; 2: past star; 3: future star; 
		starTex[0] = Resources.Load("Textures/star", typeof(Texture)) as Texture;
		starTex[1] = Resources.Load("Textures/star_selected", typeof(Texture)) as Texture;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 screenPt = Camera.main.ViewportToScreenPoint(this.transform.position);
		Rect collisionBox = new Rect(screenPt.x + guiTexture.pixelInset.x, screenPt.y + guiTexture.pixelInset.y
		                             , guiTexture.pixelInset.width, guiTexture.pixelInset.height);
		if (Input.GetMouseButtonDown(0) )
		{
			if (!collisionBox.Contains(Input.mousePosition)){
				if(status == STAR_STATUS.EDIT_IN_SCENE_SELECTED) {
					status = STAR_STATUS.EDIT_IN_SCENE;
					guiTexture.texture = starTex[0]; 
				}
			}
		}

		if(autoMove) {
			if(transform.position.Equals(targetPosition)) {
				autoMove = false;
			}
			else {
				Vector3 vel = (targetPosition - transform.position);
				if(vel.magnitude > maxPosChange) {
					vel.Normalize();
					vel = vel * maxPosChange;
				}
				transform.position = transform.position + vel;
			}
		}
	}


	void OnMouseDown() {
		lastMousePosition = GetClampedMousePosition();
		if(status == STAR_STATUS.EDIT_IN_PANEL) {
			// do nothing here, as user may just click and not dragging, which will create a copy of star although will delete finally
		}
		else if(status == STAR_STATUS.EDIT_IN_SCENE){
			status = STAR_STATUS.EDIT_IN_SCENE_SELECTED;
			guiTexture.texture = starTex[1];
		}
		else if(status == STAR_STATUS.EDIT_IN_SCENE_SELECTED){
			status = STAR_STATUS.EDIT_IN_SCENE;
			guiTexture.texture = starTex[0];
		}
		Debug.Log(status.ToString());
	}

	void OnMouseUp() {
		lastMousePosition = GetClampedMousePosition();
		if(status == STAR_STATUS.EDIT_DRAGGING) {
			if(scenePanel.Contains(Input.mousePosition)) {
				status = STAR_STATUS.EDIT_IN_SCENE;
				guiTexture.texture = starTex[0];
				GameObject.Find("sky").GetComponent<SkyCtrl>().SendMessage("SetStar", this);
			}
			else {
				Destroy(this.gameObject);
			}
			GameObject.Find("sky").GetComponent<SkyCtrl>().SendMessage("SetDraggingStatus", false);
		}
		Debug.Log(status.ToString());
	}

	Vector3 GetClampedMousePosition()
	{
		Vector3 mousePosition = Input.mousePosition;
		mousePosition.x = Mathf.Clamp(mousePosition.x, 0f, Screen.width);
		mousePosition.y = Mathf.Clamp(mousePosition.y, 0f, Screen.height);
		
		return mousePosition;
	}

	void OnMouseDrag()
	{
		Vector3 delta = GetClampedMousePosition() - lastMousePosition;
			
		if(!delta.Equals(Vector3.zero)) {
			if(status == STAR_STATUS.EDIT_IN_SCENE) {	// previously is selected
				status = STAR_STATUS.EDIT_DRAGGING;
				GameObject.Find("sky").GetComponent<SkyCtrl>().SendMessage("SetDraggingStatus", true);
			}
			else if(status == STAR_STATUS.EDIT_IN_PANEL) {
				GameObject star = Instantiate(Resources.Load("Prefab/starGUI", typeof(GameObject)), this.transform.position, this.transform.rotation) as GameObject;
				star.tag = "ctrl";
				this.tag = "star";
				status = STAR_STATUS.EDIT_DRAGGING;
				GameObject.Find("sky").GetComponent<SkyCtrl>().SendMessage("SetDraggingStatus", true);
			}
			if(status == STAR_STATUS.EDIT_DRAGGING) {
				delta = Camera.main.ScreenToViewportPoint(delta);
				
				transform.position += delta;
				
				Vector3 position = transform.position;
				position.x = Mathf.Clamp(position.x, border.x, border.x + border.width);
				position.y = Mathf.Clamp(position.y, border.y, border.y + border.height);
				
				lastMousePosition = GetClampedMousePosition();
			}
		}
		Debug.Log(status.ToString());
	}

	public Vector3 GetScreenPosition() {
		return lastMousePosition;
	}

	public void SetPitch(int pitch, Vector3 targetPos) {
		/*for (int i=0; i<audioNum; i++) {
			if(audioRange[i, 0] <= pitch && pitch < audioRange[i, 1]) {
				pitch = pitch - i * 12;
				audio.clip = Resources.Load(audioSources[i], typeof(AudioClip)) as AudioClip;
				audio.pitch = Mathf.Pow(2.0f, pitch / 12.0f);
				audio.volume = 1.0f;
				audio.Play();
				break;
			}
		}*/
		targetPosition = targetPos;
		autoMove = true;
	}
}
