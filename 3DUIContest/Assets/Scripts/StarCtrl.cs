using UnityEngine;
using System.Collections;

// part of the code is from http://wiki.unity3d.com/index.php?title=DraggableGUIElement

public enum STAR_STATUS {
	IN_PANEL,
	FOLLOWING,
	IN_SCENE
};

public class StarCtrl : MonoBehaviour {
	Vector3 lastMousePosition;
	Rect border;
	STAR_STATUS status;

	public static Rect scenePanel;
	// Use this for initialization
	void Start () {
		border = new Rect(0, 0, Screen.width, Screen.height);
		status = STAR_STATUS.IN_PANEL;
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	void OnMouseDown() {
		lastMousePosition = GetClampedMousePosition();
		status = STAR_STATUS.FOLLOWING;
		Instantiate(Resources.Load("Prefab/starGUI", typeof(GameObject)), this.transform.position, this.transform.rotation);
	}

	void OnMouseUp() {
		if(scenePanel.Contains(Input.mousePosition)) {
			status = STAR_STATUS.IN_SCENE;
		}
		else {
			Destroy(this.gameObject);
		}
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
		
		delta = Camera.main.ScreenToViewportPoint(delta);
		
		transform.position += delta;
		
		Vector3 position = transform.position;
		position.x = Mathf.Clamp(position.x, border.x, border.x + border.width);
		position.y = Mathf.Clamp(position.y, border.y, border.y + border.height);
		
		transform.position = position;
		
		lastMousePosition = GetClampedMousePosition();
	}
}
