using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkyCtrl : MonoBehaviour {
	private Rect skyRect = new Rect(-1200, -600, 2400, 1200);
	private float noteWidth = 100.0f;
	private float beatHeight = 40.0f;
	private int[] notePitches;
	private bool[] hasSharp;
	//private Rect[] noteBoxes;
	private int noteNum = 24;
	private bool isDragging = false;

	private float baseHeight;
	private float currentTopHeight;

	private int pitchNum;
	private int basePitch;
	private List<int>[] playNoteList;
	private GameObject[] noteChannels;

	private bool isPlay;
	private int curFrame = 0;
	private int totalFrame = 0;
	private int timer = 0;
	// Use this for initialization
	void Start () {
		baseHeight = skyRect.y;
		currentTopHeight = baseHeight;
		noteNum = 24;
		noteWidth = skyRect.width / noteNum;
		notePitches = new int[noteNum];
		hasSharp = new bool[noteNum];
		/*noteBoxes = new Rect[noteNum];
		for(int i=0; i<noteNum; i++) {
			noteBoxes[i] = new Rect(skyRect.x, skyRect.y, skyRect.x + noteWidth * i, skyRect.height);
		}*/
		notePitches[0] = -8;	hasSharp[0] = false;	// E0
		notePitches[1] = -7;	hasSharp[1] = true;		// F0
		notePitches[2] = -5;	hasSharp[2] = true;		// G0
		notePitches[3] = -3;	hasSharp[3] = true;		// A0
		notePitches[4] = -1;	hasSharp[4] = false;	// B0
		notePitches[5] = 0;		hasSharp[5] = true;		// C1
		notePitches[6] = 2;		hasSharp[6] = true;		// D1
		notePitches[7] = 4;		hasSharp[7] = false;	// E1
		notePitches[8] = 5;		hasSharp[8] = true;		// F1
		notePitches[9] = 7;		hasSharp[9] = true;		// G1
		notePitches[10] = 9;	hasSharp[10] = true;	// A1
		notePitches[11] = 11;	hasSharp[11] = false;	// B1
		notePitches[12] = 12;	hasSharp[12] = true;	// C2
		notePitches[13] = 14;	hasSharp[13] = true;	// D2
		notePitches[14] = 16;	hasSharp[14] = false;	// E2
		notePitches[15] = 17;	hasSharp[15] = true;	// F2
		notePitches[16] = 19;	hasSharp[16] = true;	// G2
		notePitches[17] = 21;	hasSharp[17] = true;	// A2
		notePitches[18] = 23;	hasSharp[18] = false;	// B2
		notePitches[19] = 24;	hasSharp[19] = true;	// C3
		notePitches[20] = 26;	hasSharp[20] = true;	// D3
		notePitches[21] = 28;	hasSharp[21] = false;	// E3
		notePitches[22] = 29;	hasSharp[22] = true;	// F3
		notePitches[23] = 31;	hasSharp[23] = true;	// G3

		if(StarCtrl.audioNum <= 0) {
			StarCtrl.InitStatic();
		}
		pitchNum = notePitches[noteNum-1] - notePitches[0] + 1;
		basePitch = notePitches[0];
		playNoteList = new List<int>[pitchNum];
		noteChannels = new GameObject[pitchNum];
		for(int i=0; i<pitchNum; i++) {
			noteChannels[i] = Instantiate(Resources.Load("Prefab/note_channel", typeof(GameObject)), Vector3.zero, Quaternion.identity) as GameObject; 
			for (int j=0; j<StarCtrl.audioNum; j++) {
				if(StarCtrl.audioRange[j, 0] <= basePitch + i && basePitch + i < StarCtrl.audioRange[j, 1]) {
					int pitch = basePitch + i - j * 12;
					noteChannels[i].audio.clip = Resources.Load(StarCtrl.audioSources[j], typeof(AudioClip)) as AudioClip;
					noteChannels[i].audio.pitch = Mathf.Pow(2.0f, pitch / 12.0f);
					noteChannels[i].audio.volume = 1.0f;
					noteChannels[i].audio.playOnAwake = false;
					break;
				}
			}
			noteChannels[i].name = "note" + i;
			noteChannels[i].transform.parent = transform;
			playNoteList[i] = new List<int>();
		}
		for(int i=0; i<1000; i++) {
			int pitchIdx = Random.Range(0, pitchNum);
			AddOneBeatSpace(i, pitchIdx, 0);
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(isPlay) {
			if(timer > 10) {
				if(curFrame > totalFrame)
					return;
				for (int i=0; i<pitchNum; i++) {
					if(playNoteList[i][curFrame] == 1) {
						noteChannels[i].audio.Play();
					}
					else if(playNoteList[i][curFrame] == 0) {
						//noteChannels[i].audio.Stop();
					}
				}
				curFrame++;
				timer=0;
			}
			timer++;
		}

		if(Input.GetKey(KeyCode.Space)) {
			isPlay = ! isPlay;
		}
	}

	public void SetStar(StarCtrl star) {
		Vector3 starPos = star.GetScreenPosition();
		int noteID = (int) ((starPos.x - skyRect.x) / noteWidth);
		int pitch = notePitches[noteID];
		Vector3 targetlPos = new Vector3((skyRect.x + noteWidth * (noteID + 0.5f)) / Screen.width, beatHeight / Screen.height, 1.0f);
		if(noteID > 0 && noteID < noteNum - 1) {
			Vector3 pos = new Vector3((skyRect.x + noteWidth * noteID), beatHeight * (1 + 0.3f), 1.0f);
			float dist = (starPos - pos).magnitude;
			if(dist < noteWidth * 0.25 && hasSharp[noteID - 1] == true) {
				pitch = notePitches[noteID] - 1;
				targetlPos.x = pos.x / Screen.width;
				targetlPos.y = pos.y / Screen.height;
			}
			else {
				pos = new Vector3((skyRect.x + noteWidth * (noteID + 1)), beatHeight * (1 + 0.3f), 1.0f);
				dist = (starPos - pos).magnitude;
				if(dist < noteWidth * 0.25 && hasSharp[noteID] == true) {
					pitch = notePitches[noteID] + 1;
					targetlPos.x = pos.x / Screen.width;
					targetlPos.y = pos.y / Screen.height;
				}
			}
		}
		star.SetPitch(pitch, targetlPos);
	}

	public void SetDraggingStatus(bool drag) {	// to show small dots
		if(isDragging == drag)
			return;
		if(isDragging) {	// if previously, it was dragging, destroy dots
			GameObject[] dots = GameObject.FindGameObjectsWithTag("dots");
			foreach (GameObject dot in dots) {
				Destroy(dot);
			}
		}
		else{	// create dots
			for(int i=0; i<noteNum; i++) {
				Vector3 pos = new Vector3((skyRect.x + noteWidth * (i + 0.5f)) / Screen.width, beatHeight / Screen.height, 1.0f);
				Instantiate(Resources.Load("Prefab/dotGUI", typeof(GameObject)), pos, Quaternion.identity);
				if(i < noteNum - 1) {
					if(notePitches[i+1] - notePitches[i] > 1) {	// sharp
						pos = new Vector3((skyRect.x + noteWidth * (i + 1.0f)) / Screen.width, beatHeight * (1 + 0.3f) / Screen.height, 1.0f);
						Instantiate(Resources.Load("Prefab/dotGUI", typeof(GameObject)), pos, Quaternion.identity);
					}
				}
			}
		}
		isDragging = drag;
	}

	void AddOneBeatSpace(int targetFrame, int pitchIdx, int type) {
		totalFrame += targetFrame + 1 - playNoteList[0].Count;
		for(int i=0; i<pitchNum; i++) {
			for(int j=playNoteList[i].Count; j<targetFrame+1; j++) {
				playNoteList[i].Add(0);
			}
		}
		playNoteList[pitchIdx][targetFrame] = 1;
	}
}
