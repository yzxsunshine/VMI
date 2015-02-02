using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;

public enum CLIP_STATUS {
	CLIP_EMPTY,
	CLIP_BEGIN,
	CLIP_CHANGE,
	CLIP_END
}

public class MusicClip {
	public float pitch;
	public float volume;
	public int frame;
	public List<int> chords;
	public CLIP_STATUS status;

	public MusicClip() {
		chords = new List<int>();
		pitch = 0;
		frame = 0;
		volume = 0;
		status = CLIP_STATUS.CLIP_EMPTY;
	}

	public static string Encode(MusicClip mc) {
		if(mc.chords.Count < 1) {
			return "";
		}
		string msg = String.Format("pitch={0};volume={1};status={2};chords=", mc.pitch, mc.volume, mc.status.ToString("d"));
		msg += mc.chords[0].ToString();
		for(int i=1; i<mc.chords.Count; i++) {
			msg += "#" + mc.chords[i].ToString();
		}
		return msg;
	}

	public static MusicClip Decode(string msg) {

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
				else if(vars[0].Equals("chords")) {
					string[] chords = vars[1].Split('#');
					for (int j=0; j<chords.Length; j++) {
						mc.chords.Add(Convert.ToInt32(chords[j]));
					}
				}
			}
		}
		catch (Exception e) {
		}
		return mc;
	}
}
