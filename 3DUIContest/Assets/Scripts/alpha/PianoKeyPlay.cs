using UnityEngine;
using System.Collections;

public class PianoKeyPlay : MonoBehaviour {
	// Use this for initialization
	private bool isPress = false;
	private int pitchID = 0;
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		int i = 0;
		bool hited = false;
		#if (UNITY_EDITOR_WIN || UNITY_STANDALONE)
		if(Input.GetMouseButtonDown(0)) {
			Vector2 pt = Input.mousePosition;
			//RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(pt), Vector2.zero);
			Ray ray = Camera.main.ScreenPointToRay (pt);
			RaycastHit hit = new RaycastHit();
			if(Physics.Raycast(ray, out hit, Mathf.Infinity) && hit.transform == this.transform) {
				hited = true;
			}
		}
		#elif (UNITY_IPHONE || UNITY_ANDROID)
		while (i < Input.touchCount) {
			Vector2 pt = Input.GetTouch(i).position;
			Ray ray = Camera.main.ScreenPointToRay (pt);
			RaycastHit hit = new RaycastHit();
			if(Physics.Raycast(ray, out hit, Mathf.Infinity) && hit.transform == this.transform) {
				hited = true;
				break;
			}
			i++;
		}
		#endif
		
		if(hited == false) {
			EndTouch();
		}
		else {
			//BeginTouch();
		}
	}

	public void BeginTouch () {//(PLAY_MODE mode) {
		if(!isPress) {
			StopSound();
			//chordMode = mode;
			Debug.Log("Press");
			PlaySound();
			isPress = true;
		}
	}
	
	public void EndTouch() {
		//StopSound();
		isPress = false;
	}

	void PlaySound() {
		audio.Play();

	}
	void StopSound() {
		audio.Stop();
	}
	//void OnMouseDown() {
	//	audio.Play();
	//}
	public void SetPitchID(int pid) {
		pitchID = pid;
	}
}
