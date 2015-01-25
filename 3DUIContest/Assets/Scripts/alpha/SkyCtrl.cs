using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkyCtrl : MonoBehaviour {
	public static Rect skyRect;	// range of screen
	private float noteWidth = 100.0f;
	private float beatHeight = 40.0f;
	private int[] notePitches;
	private bool[] hasSharp;
	private string[] noteText;
	//private Rect[] noteBoxes;
	private int noteNum = 21;
	private int notePerScreen;
	private bool isDragging = false;

	private float baseHeight;
	private float currentTopHeight;

	private int pitchNum;
	private int basePitch;
	private List<int>[] playNoteList;
	private GameObject[] noteChannels;

	private bool isPlay;
	private int curFrame = 0;
	private int curComposeFrame = 0;
	private int totalFrame = 0;
	private int timer = 0;

	private string[] audioSources;
	private int[,] audioRange; 
	private int audioNum = 0;

	private string[] skyTexNames = {"Prefab/sky1", "Prefab/sky2", "Prefab/sky3"};
	//private GameObject[] skyTextures;
	private Rect[][,] skyScaleRect;	// each scale have 7 notes, 12 pitch
	private int tileNumVertical;
	private int tileNumHorizontal;

	private int editCurBeat = 0;

	// deal with page and beats
	private int curPage = 0;
	private int beatsPerPage = 12;
	public Vector3 lastMousePosition;
	public float[] pitchXPoses;
	public float[] beatYPoses;
	public bool[] isSharp;

	private float keyHeight = 0.0f;
	// Use this for initialization
	void Start () {


		currentTopHeight = baseHeight;

		audioNum = 3;
		audioSources = new string[audioNum];
		audioSources[0] = "audio/c0";
		audioSources[1] = "audio/c1";
		audioSources[2] = "audio/c2";
		audioRange = new int[audioNum, 2];	// include left, exclude right
		audioRange[0, 0] = -3;	audioRange[0, 1] = 9;
		audioRange[1, 0] = 9;	audioRange[1, 1] = 21;
		audioRange[2, 0] = 21;	audioRange[2, 1] = 32;
		noteNum = 21;
		notePerScreen = 10;
//#if UNITY_EDITOR
//		keyHeight = 0.0f*Screen.height;
//#elif
		keyHeight = 0.2f*Screen.height;
//#endif
		skyRect = new Rect(-Screen.width / notePerScreen * noteNum / 2, keyHeight, Screen.width / notePerScreen * noteNum, Screen.height - keyHeight);
		baseHeight = skyRect.y;

		beatsPerPage = 12;
		noteWidth = skyRect.width / noteNum;
		beatHeight = skyRect.height / beatsPerPage;

		notePitches = new int[noteNum];
		hasSharp = new bool[noteNum];
		noteText = new string[noteNum];
		/*noteBoxes = new Rect[noteNum];
		for(int i=0; i<noteNum; i++) {
			noteBoxes[i] = new Rect(skyRect.x, skyRect.y, skyRect.x + noteWidth * i, skyRect.height);
		}*/
		notePitches[0] = -3;	hasSharp[0] = true;		noteText[0]  = "A0"; // A0
		notePitches[1] = -1;	hasSharp[1] = false;	noteText[1]  = "B0"; // B0
		notePitches[2] = 0;		hasSharp[2] = true;		noteText[2]  = "C1"; // C1
		notePitches[3] = 2;		hasSharp[3] = true;		noteText[3]  = "D1"; // D1
		notePitches[4] = 4;		hasSharp[4] = false;	noteText[4]  = "E1"; // E1
		notePitches[5] = 5;		hasSharp[5] = true;		noteText[5]  = "F1"; // F1
		notePitches[6] = 7;		hasSharp[6] = true;		noteText[6]  = "G1"; // G1
		notePitches[7] = 9;		hasSharp[7]  = true;	noteText[7]  = "A1"; // A1
		notePitches[8] = 11;	hasSharp[8]  = false;	noteText[8]  = "B1"; // B1
		notePitches[9] = 12;	hasSharp[9]  = true;	noteText[9]  = "C2"; // C2
		notePitches[10] = 14;	hasSharp[10] = true;	noteText[10] = "D2"; // D2
		notePitches[11] = 16;	hasSharp[11] = false;	noteText[11] = "E2"; // E2
		notePitches[12] = 17;	hasSharp[12] = true;	noteText[12] = "F2"; // F2
		notePitches[13] = 19;	hasSharp[13] = true;	noteText[13] = "G2"; // G2
		notePitches[14] = 21;	hasSharp[14] = true;	noteText[14] = "A2"; // A2
		notePitches[15] = 23;	hasSharp[15] = false;	noteText[15] = "B2"; // B2
		notePitches[16] = 24;	hasSharp[16] = true;	noteText[16] = "C3"; // C3
		notePitches[17] = 26;	hasSharp[17] = true;	noteText[17] = "D3"; // D3
		notePitches[18] = 28;	hasSharp[18] = false;	noteText[18] = "E3"; // E3
		notePitches[19] = 29;	hasSharp[19] = true;	noteText[19] = "F3"; // F3
		notePitches[20] = 31;	hasSharp[20] = false;	noteText[20] = "G3"; // G3

		pitchNum = notePitches[noteNum-1] - notePitches[0] + 1;

		pitchXPoses = new float[pitchNum];
		beatYPoses = new float[beatsPerPage];
		isSharp = new bool[pitchNum];

		basePitch = notePitches[0];
		playNoteList = new List<int>[pitchNum];

		/*for(int i=0; i<1000; i++) {
			int pitchIdx = Random.Range(0, pitchNum);
			AddOneBeatSpace(i, pitchIdx, 0);
		}*/
		skyScaleRect = new Rect[audioNum][,];
		//skyTextures = new Texture[audioNum];
		tileNumHorizontal = 7;
		int tileSize = (int) skyRect.width / (tileNumHorizontal * 3);
		tileNumVertical = (int) (skyRect.height * 2 / tileSize);
		for(int i=0; i<audioNum; i++) {
			//skyTextures[i].pixelInset = new Vector2(skyRect.x + skyRect.width * i / 3, skyRect.y, skyRect.width / 3, skyRect.height);
			skyScaleRect[i] = new Rect[tileNumVertical, tileNumHorizontal];
			for(int r=0; r<tileNumVertical; r++) {
				for(int c=0; c<tileNumHorizontal; c++) {
					skyScaleRect[i][r, c] = new Rect(skyRect.x + skyRect.width * i / 3 + c*tileSize, skyRect.y + r * tileSize, tileSize, tileSize);
					Vector3 center = new Vector3(skyScaleRect[i][r, c].x, skyScaleRect[i][r, c].y, 0.0f);
					center = Camera.main.ScreenToViewportPoint(center);
					GameObject obj = Resources.Load(skyTexNames[i], typeof(GameObject)) as GameObject;
					obj.guiTexture.pixelInset = new Rect(skyScaleRect[i][r, c].x, skyScaleRect[i][r, c].y, skyScaleRect[i][r, c].width*2.0f, skyScaleRect[i][r, c].height*2.0f);
					Instantiate(obj, center, Quaternion.identity);
				}
			}
		}

		int pitchCount = 0;
		for(int i=0; i<noteNum; i++) {
			pitchXPoses[pitchCount] = skyRect.x + noteWidth * (i + 0.5f);
			isSharp[pitchCount] = false;
			pitchCount++;
			if(hasSharp[i]) {
				pitchXPoses[pitchCount] = skyRect.x + noteWidth * (i + 1.0f);
				isSharp[pitchCount] = true;
				pitchCount++;
			}
		}
		for(int i=0; i<beatsPerPage; i++) {
			beatYPoses[i] = keyHeight + beatHeight * (i + 0.5f);
		}

		noteChannels = new GameObject[pitchNum];
		int pitchCounter = 0;
		Vector3 whiteKeySize = Camera.main.ScreenToWorldPoint(new Vector3(noteWidth, keyHeight, 1.0f));
		Vector3 blackKeySize = new Vector3(whiteKeySize.x*0.8f, whiteKeySize.y*0.6f, 1.0f);
		for(int i=0; i<noteNum; i++) {
			Vector3 pos = new Vector3(pitchXPoses[pitchCounter], 0.2f * Screen.height, 1.0f);
			pos = Camera.main.ScreenToWorldPoint(pos);
			noteChannels[pitchCounter] = Instantiate(Resources.Load("Prefab/key", typeof(GameObject)), pos, Quaternion.identity) as GameObject; 
			noteChannels[pitchCounter].transform.localScale = new Vector3(1.4f, whiteKeySize.y*1.2f, 1.0f);
			noteChannels[pitchCounter].renderer.material.color = Color.white;
			noteChannels[pitchCounter].GetComponent<PianoKeyPlay>().SendMessage("SetPitchID", pitchCounter);
			pitchCounter++;
			if(hasSharp[i]) {
				pos = new Vector3(pitchXPoses[pitchCounter], 0.28f * Screen.height, 0.9f);
				pos = Camera.main.ScreenToWorldPoint(pos);
				noteChannels[pitchCounter] = Instantiate(Resources.Load("Prefab/key", typeof(GameObject)), pos, Quaternion.identity) as GameObject; 
				noteChannels[pitchCounter].transform.localScale = new Vector3(1.2f, blackKeySize.y*1.2f, 1.0f);
				noteChannels[pitchCounter].renderer.material.color = Color.black;
				noteChannels[pitchCounter].GetComponent<PianoKeyPlay>().SendMessage("SetPitchID", pitchCounter);
				pitchCounter++;
			}
		}

		for(int i=0; i<pitchNum; i++) {
			for (int j=0; j<audioNum; j++) {
				if(audioRange[j, 0] <= basePitch + i && basePitch + i < audioRange[j, 1]) {
					int pitch = basePitch + i - j * 12;
					noteChannels[i].audio.clip = Resources.Load(audioSources[j], typeof(AudioClip)) as AudioClip;
					noteChannels[i].audio.pitch = Mathf.Pow(2.0f, pitch / 12.0f);
					noteChannels[i].audio.volume = 1.0f;
					noteChannels[i].audio.playOnAwake = false;
					break;
				}
			}
			noteChannels[i].name = "note" + i;
			//noteChannels[i].transform.parent = transform;
			playNoteList[i] = new List<int>();
		}

		AddNewPage();
	}

	// Update is called once per frame
	void FixedUpdate () {
		if(isPlay) {
			if(timer > 10) {
				if(curFrame >= totalFrame)
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
			curFrame = 0;
		}

		if(isDragging) {
			Vector3 mousePos = GetClampedMousePosition();
			int curBeat = (int) ((mousePos.y - keyHeight)/ beatHeight);
			curBeat = (int) Mathf.Clamp(curBeat, 0f, beatsPerPage - 1);
			if(editCurBeat != curBeat) {
				editCurBeat = curBeat;
				GameObject[] dots = GameObject.FindGameObjectsWithTag("dots");
				int pitchIdx = 0;
				for(int i=0; i<noteNum; i++) {
					Vector3 pos = new Vector3((skyRect.x + noteWidth * (i + 0.5f)), beatYPoses[editCurBeat], 1.0f);
					pos = Camera.main.ScreenToViewportPoint(pos);
					dots[pitchIdx].transform.position = pos;
					dots[pitchIdx].GetComponentInChildren<GUIText>().text = noteText[i];
					pitchIdx++;
					if(i < noteNum - 1) {
						if(notePitches[i+1] - notePitches[i] > 1) {	// sharp
							pos = new Vector3((skyRect.x + noteWidth * (i + 1.0f)),  beatYPoses[editCurBeat] + beatHeight * 0.3f, 1.0f);
							pos = Camera.main.ScreenToViewportPoint(pos);
							dots[pitchIdx].transform.position = pos;
							dots[pitchIdx].GetComponentInChildren<GUIText>().text = noteText[i] + "#";
							pitchIdx++;
						}
					}
				}
			}
		}
		else {
			if(Input.GetMouseButtonDown(0)) {
				if(Input.mousePosition.y > keyHeight) {
					lastMousePosition = Input.mousePosition;
				}
#if (UNITY_EDITOR_WIN || UNITY_STANDALONE)
				else {
					Vector2 pt = Input.mousePosition;
					Vector2 ptInScreen = new Vector2(pt.x, Screen.height - pt.y);
					Ray ray = Camera.main.ScreenPointToRay (pt);
					RaycastHit hit = new RaycastHit();
					if(Physics.Raycast(ray, out hit, Mathf.Infinity)) {
						hit.collider.gameObject.SendMessage("BeginTouch");
						int noteID = (int) ((pt.x - skyRect.x) / noteWidth);
						int pitch = notePitches[noteID] - notePitches[0];
						AddStar(pitch, curComposeFrame);
						NextFrame();
					}
				}
#endif
			}
			else if(Input.GetMouseButtonUp(0)) {
				if(Input.mousePosition.y > keyHeight) {
					float dragDist = Input.mousePosition.y - lastMousePosition.y;
					if(dragDist < -0.3f * skyRect.height) {
						NextPage();
					}
					else if(dragDist > 0.3f * skyRect.height) {
						PrevPage();
					}
				}
				else {
					Vector2 pt = Input.mousePosition;
					Vector2 ptInScreen = new Vector2(pt.x, Screen.height - pt.y);
					Ray ray = Camera.main.ScreenPointToRay (pt);
					RaycastHit hit = new RaycastHit();
					if(Physics.Raycast(ray, out hit, Mathf.Infinity)) {
						hit.collider.gameObject.SendMessage("EndTouch");
					}
				}
			}


#if !UNITY_EDITOR //(UNITY_IPHONE || UNITY_ANDROID)
			int touchID = 0;
			bool isHit = false;
			while (touchID < Input.touchCount) {
				Debug.Log("Touch Count = " + Input.touchCount);
				Vector2 pt = Input.GetTouch(touchID).position;
				if(pt.y > keyHeight) {
					touchID++;
					continue;
				}
				Vector2 ptInScreen = new Vector2(pt.x, Screen.height - pt.y);
				Ray ray = Camera.main.ScreenPointToRay (pt);
				RaycastHit hit = new RaycastHit();
				if(Physics.Raycast(ray, out hit, Mathf.Infinity)) {
					switch (Input.GetTouch(touchID).phase) {
					case TouchPhase.Ended:
						hit.collider.gameObject.SendMessage("EndTouch");
						break;
					//case TouchPhase.Moved:
					case TouchPhase.Began:
						hit.collider.gameObject.SendMessage("BeginTouch");
						int noteID = (int) ((pt.x - skyRect.x) / noteWidth);
						int pitch = notePitches[noteID] - notePitches[0];
						AddStar(pitch, curComposeFrame);
						isHit = true;
						//GetComponent<VMINetwork>().SendMsg(pitch, curComposeFrame);
						break;
					}
				}
				touchID++;
			}

			if(isHit) {
				NextFrame();
			}
#endif
		}
	}

	void OnGUI() {
		//GUI.Label(new Rect(10, 10, 100, 50), Input.mousePosition.ToString());
	}

	Vector3 GetClampedMousePosition()
	{
		Vector3 mousePosition = Input.mousePosition;
		mousePosition.x = Mathf.Clamp(mousePosition.x, 0f, Screen.width);
		mousePosition.y = Mathf.Clamp(mousePosition.y, 0f, Screen.height);
		
		return mousePosition;
	}

	public void SetStar(StarCtrl star) {
		Vector3 starPos = star.GetScreenPosition();
		int noteID = (int) ((starPos.x - skyRect.x) / noteWidth);
		int pitch = notePitches[noteID] - notePitches[0];
		Vector3 targetlPos = new Vector3((skyRect.x + noteWidth * (noteID + 0.5f)), beatYPoses[editCurBeat], 1.0f);
		targetlPos = Camera.main.ScreenToViewportPoint(targetlPos);
		if(noteID > 0 && noteID < noteNum - 1) {
			Vector3 pos = new Vector3(skyRect.x + noteWidth * noteID, beatYPoses[editCurBeat] + beatHeight * 0.3f, 1.0f);
			float dist = (starPos - pos).magnitude;
			if(dist < noteWidth * 0.25f && hasSharp[noteID - 1] == true) {
				pitch = notePitches[noteID] - 1 - notePitches[0];
				//targetlPos.x = pos.x / Screen.width;
				//targetlPos.y = pos.y / Screen.height;
				targetlPos = Camera.main.ScreenToViewportPoint(pos);
			}
			else {
				pos = new Vector3(skyRect.x + noteWidth * (noteID + 1.0f), beatYPoses[editCurBeat] + beatHeight * 0.3f, 1.0f);
				dist = (starPos - pos).magnitude;
				if(dist < noteWidth * 0.25f && hasSharp[noteID] == true) {
					pitch = notePitches[noteID] + 1 - notePitches[0];
					//targetlPos.x = pos.x / Screen.width;
					//targetlPos.y = pos.y / Screen.height;
					targetlPos = Camera.main.ScreenToViewportPoint(pos);
				}
			}
		}
		star.SetPitch(pitch, targetlPos);
		noteChannels[pitch].audio.Play();
		int beatID = curPage * beatsPerPage + (editCurBeat);
		//AddOneBeatSpace(beatID, pitch, 0);
		playNoteList[pitch][beatID] = 1;
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
			editCurBeat = (int) (Input.mousePosition.y / beatHeight);
			for(int i=0; i<noteNum; i++) {
				Vector3 pos = new Vector3((skyRect.x + noteWidth * (i + 0.5f)) / Screen.width, beatHeight * (editCurBeat + 0.5f) / skyRect.height, 1.0f);
				GameObject dot = Instantiate(Resources.Load("Prefab/dotGUI", typeof(GameObject)), pos, Quaternion.identity) as GameObject;
				dot.GetComponentInChildren<GUIText>().text = noteText[i];
				if(i < noteNum - 1) {
					if(notePitches[i+1] - notePitches[i] > 1) {	// sharp
						pos = new Vector3((skyRect.x + noteWidth * (i + 1.0f)) / Screen.width, beatHeight * (editCurBeat + 0.8f) / skyRect.height, 1.0f);
						GameObject dotSharp = Instantiate(Resources.Load("Prefab/dotGUI", typeof(GameObject)), pos, Quaternion.identity) as GameObject;
						dotSharp.GetComponentInChildren<GUIText>().text = noteText[i] + "#";
					}
				}
			}
		}

		isDragging = drag;
		if(!isDragging) {
			editCurBeat = -1;
		}
	}

	void NextFrame() {
		curComposeFrame++;
		if(curComposeFrame >= (curPage + 1) * beatsPerPage) {
			NextPage();
		}
	}

	void AddNewPage() {
		totalFrame = beatsPerPage * (curPage + 1);
		for(int i=0; i<pitchNum; i++) {
			for(int j=playNoteList[i].Count; j<totalFrame; j++) {
				playNoteList[i].Add(0);
			}
		}
	}

	void NextPage() {
		curPage++;
		GameObject[] stars = GameObject.FindGameObjectsWithTag("star");
		foreach (GameObject star in stars) {
			Destroy(star);
		}
		
		if(curPage * beatsPerPage < totalFrame) {
			for(int i=0; i<pitchNum; i++) {
				for(int j=0; j<beatsPerPage; j++) {
					if(playNoteList[i][curPage * beatsPerPage+ j] == 1) {
						Vector3 pos = new Vector3(pitchXPoses[i], beatYPoses[j], 1.0f);
						if(isSharp[i]) {
							pos.y += beatHeight * 0.3f;
						}
						pos = Camera.main.ScreenToViewportPoint(pos);
						GameObject star = Instantiate(Resources.Load("Prefab/starGUI", typeof(GameObject)), pos, Quaternion.identity) as GameObject;
						star.tag = "star";
						star.guiTexture.pixelInset.Set(-25, -25, 50, 50);
					}
				}
			}
		}
		else {
			AddNewPage();
		}
	}

	void PrevPage() {
		curPage-- ;
		if(curPage < 0) 
			curPage = 0;
		GameObject[] stars = GameObject.FindGameObjectsWithTag("star");
		foreach (GameObject star in stars) {
			Destroy(star);
		}
		for(int i=0; i<pitchNum; i++) {
			for(int j=0; j<beatsPerPage; j++) {
				if(playNoteList[i][curPage * beatsPerPage+ j] == 1) {
					Vector3 pos = new Vector3(pitchXPoses[i], beatYPoses[j], 1.0f);
					if(isSharp[i]) {
						pos.y += beatHeight * 0.3f;
					}
					pos = Camera.main.ScreenToViewportPoint(pos);
					GameObject star = Instantiate(Resources.Load("Prefab/starGUI", typeof(GameObject)), pos, Quaternion.identity) as GameObject;
					star.tag = "star";
					star.guiTexture.pixelInset.Set(-25, -25, 50, 50);
				}
			}
		}
	}

	public void AddStar(int pitch, int frame) {
		Vector3 pos = new Vector3(pitchXPoses[pitch], beatYPoses[beatsPerPage - 1 - (frame - curPage*beatsPerPage)], 1.0f);
		pos = Camera.main.ScreenToViewportPoint(pos);
		GameObject star = Instantiate(Resources.Load("Prefab/starGUI", typeof(GameObject)), pos, Quaternion.identity) as GameObject;
		star.tag = "star";
		star.guiTexture.pixelInset.Set(-25, -25, 50, 50);
		playNoteList[pitch][frame] = 1;
	}

	void AddOneBeatSpace(int targetFrame, int pitchIdx, int type) {
		totalFrame = beatsPerPage * curPage + targetFrame + 1 - playNoteList[0].Count;
		for(int i=0; i<pitchNum; i++) {
			for(int j=playNoteList[i].Count; j<targetFrame+1; j++) {
				playNoteList[i].Add(0);
			}
		}
		playNoteList[pitchIdx][targetFrame] = 1;
	}
}
