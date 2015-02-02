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

	public string Encode(MusicClip[] mcs) {
		string msg = "";
		msg += String.Format("pitch={0};volume={1};status={2}", mcs[0].pitch, mcs[0].volume, mcs[0].status.ToString("d"));
		for (int i=1; i<mcs.Length; i++) {
			msg += String.Format("#pitch={0};volume={1};status={2}", mcs[0].pitch, mcs[0].volume, mcs[0].status.ToString("d"));
		}
		return msg;
	}

	public MusicClip[] Decode(string msg) {
		string[] clips = msg.Split('#');
		MusicClip[] mcs = new MusicClip[5];
		try {
			for (int i=0; i<5 && i<clips.Length; i++) {
				string[] strs = clips[i].Split(';');
				MusicClip mc = new MusicClip();

				for(int j=0; j<strs.Length; j++) {
					string[] vars = strs[j].Split('=');
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
				mcs[i] = mc;
			}
		}
		catch (Exception e) {
		}
		return mcs;
	}
}
