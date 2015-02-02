using UnityEngine;
using System.Collections;

public class PersistObj : MonoBehaviour {
	public string remoteIP = "130.215.28.125";
	public string remotePort = "12005";
	void Awake() {
		DontDestroyOnLoad(transform.gameObject);
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
