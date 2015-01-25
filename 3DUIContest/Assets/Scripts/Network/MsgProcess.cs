using UnityEngine;
using System.Collections;
using System;
using System.Text;

public class MsgProcess : MonoBehaviour {
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public string Encode(float pitch, float volume, CLIP_STATUS status) {
		string msg = String.Format("pitch={0};volume={1};status={2}", pitch, volume, status.ToString("d"));
		return msg;
	}

	public MusicClip Decode(string msg) {
		string[] strs = msg.Split(';');
		MusicClip mc = new MusicClip();
		try {
			for(int i=0; i<strs.Length; i++) {
				string[] vars = strs[i].Split('=');
				if(vars[0].Equals("pitch")) {
					mc.pitch = (float)Convert.ToDouble(vars[1]);
				}
				else if(vars[0].Equals("volume")) {
					mc.volume = (float)Convert.ToDouble(vars[1]);
				}
				else if(vars[0].Equals("status")) {
					int status = Convert.ToInt32(vars[1]);
					switch(status) {
					case 0:
						mc.status = CLIP_STATUS.CLIP_BEGIN;
						break;
					case 1:
						mc.status = CLIP_STATUS.CLIP_CHANGE;
						break;
					case 2:
						mc.status = CLIP_STATUS.CLIP_END;
						break;
					}
				}
				else if(vars[0].Equals("frame")) {
					mc.frame = Convert.ToInt32(vars[1]);
				}
			}
		}
		catch (Exception e) {
		}
		return mc;
	}
}
