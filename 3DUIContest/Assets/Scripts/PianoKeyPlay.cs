using UnityEngine;
using System.Collections;

public class PianoKeyPlay : MonoBehaviour {
	public float distToC = 0.0f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseDown() {
		audio.pitch = Mathf.Pow(2.0f, distToC / 12.0f);
		audio.volume = 2.0f;
		audio.Play();
	}
}
