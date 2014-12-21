using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(AudioSource))]
public class KeyManage : MonoBehaviour {
	//Try also: "FM Bank/fm" or "Analog Bank/analog" for some different sounds
	public string bankFilePath = "GM Bank/gm"; // "Analog Bank/analog";	//"FM Bank/fm";
	public int bufferSize = 1024;
	public int MidCNote = 60;
	public int instrument = 1;

	//Check the Midi's file folder for different songs
	public string midiFilePath = "Midis/Groove.mid";
	//Private 
	private float[] sampleBuffer;
	
	private float gain = 1f;
	private float maxSliderValue = 255.0f;
	
	public GameObject keyboard = null;
	private Vector3 defaultOrient = new Vector3(0, 0, 0);
	private Vector3 debugOrientation = new Vector3(0, 0, 0);

	private Vector2 touchPos = new Vector2(0,0);
	SensorFusion senfu = null;

	public Texture chordPadText = null;

	private Rect chordPadRect;
	private Rect[] chordsRect;
	private Rect[] sequenceRect;

	private PLAY_MODE mode;
	/*
	void Awake ()
	{

		midiStreamSynthesizer = new StreamSynthesizer (44100, 2, bufferSize, 40);
		sampleBuffer = new float[midiStreamSynthesizer.BufferSize];		
		
		midiStreamSynthesizer.LoadBank (bankFilePath);

		midiSequencer = new MidiSequencer (midiStreamSynthesizer);
		midiSequencer.LoadMidi (midiFilePath, false);
		//These will be fired by the midiSequencer when a song plays. Check the console for messages
		midiSequencer.NoteOnEvent += new MidiSequencer.NoteOnEventHandler (MidiNoteOnHandler);
		midiSequencer.NoteOffEvent += new MidiSequencer.NoteOffEventHandler (MidiNoteOffHandler);	
		senfu = new SensorFusion();

		chordPadRect = new Rect(0, 0, Screen.width * 0.5f, Screen.height * 0.3f);
		chordsRect = new Rect[2];
		chordsRect[0] = new Rect(Screen.width * 0.2f, Screen.height * 0.15f, Screen.width * 0.12f, Screen.height * 0.15f);
		chordsRect[1] = new Rect(Screen.width * 0.32f, Screen.height * 0.15f, Screen.width * 0.12f, Screen.height * 0.15f);
		//chordsRect[2] = new Rect(Screen.width * 0.5f, Screen.height * 0.2f, Screen.width * 0.1f, Screen.height * 0.1f);

		sequenceRect = new Rect[4];
		sequenceRect[0] = new Rect(Screen.width * 0.2f, 0, Screen.width * 0.12f, Screen.height * 0.15f);
		sequenceRect[1] = new Rect(Screen.width * 0.32f, 0, Screen.width * 0.12f, Screen.height * 0.15f);
		//sequenceRect[2] = new Rect(Screen.width * 0.2f, Screen.height * 0.1f, Screen.width * 0.1f, Screen.height * 0.1f);
		//sequenceRect[3] = new Rect(Screen.width * 0.35f, Screen.height * 0.1f, Screen.width * 0.1f, Screen.height * 0.1f);
		//sequenceRect[4] = new Rect(Screen.width * 0.4f, Screen.height * 0.1f, Screen.width * 0.1f, Screen.height * 0.1f);
		//sequenceRect[5] = new Rect(Screen.width * 0.5f, Screen.height * 0.1f, Screen.width * 0.1f, Screen.height * 0.1f);

	}
	// Use this for initialization
	void Start () {
		foreach(KeyTouch kt in GameObject.Find("Group1").GetComponentsInChildren<KeyTouch>()) {
			kt.SetStreamTynthesizer(midiStreamSynthesizer);
		}
		KeyTouch.SetMiddleCNode(MidCNote);
		KeyTouch.SetInstrument(instrument);
		KeyTouch.SetVolume(100);
		senfu.Start();
	}
	
	// Update is called once per frame
	void Update () {
#if (UNITY_EDITOR_WIN || UNITY_STANDALONE)
		mode = PLAY_MODE.SINGLE_NOTE;
		if(Input.GetMouseButtonDown(0)) {
			Vector2 pt = Input.mousePosition;
			//RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(pt), Vector2.zero);
			Vector2 ptInScreen = new Vector2(pt.x, Screen.height - pt.y);
			if(chordPadRect.Contains(ptInScreen)) {
				// chord pad;
				//Vector2 diff = pt - 
				if(chordsRect[0].Contains(ptInScreen)) {
					mode = PLAY_MODE.CHORD_MAJOR;
				}
				else if(chordsRect[1].Contains(ptInScreen)) {
					mode = PLAY_MODE.CHORD_MINOR;
				}
				else if(sequenceRect[0].Contains(ptInScreen)) {
					mode = PLAY_MODE.SEQUENCE_MAJOR_1_2;
				}
				else if(sequenceRect[1].Contains(ptInScreen)) {
					mode = PLAY_MODE.SEQUENCE_MINOR_1_2;
				}
				else if(sequenceRect[2].Contains(ptInScreen)) {
					mode = PLAY_MODE.SEQUENCE_MAJOR_1_4;
				}
				else if(sequenceRect[3].Contains(ptInScreen)) {
					mode = PLAY_MODE.SEQUENCE_MINOR_1_4;
				}	
			}
			Ray ray = Camera.main.ScreenPointToRay (pt);
			RaycastHit hit = new RaycastHit();
			if(!Physics.Raycast(ray, out hit, Mathf.Infinity))
			//if(hit.collider == null)
				return;
			hit.collider.gameObject.SendMessage("BeginTouch", mode);
		}
		else if(Input.GetMouseButtonUp(0)) {
			Vector2 pt = Input.mousePosition;
			//RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(pt), Vector2.zero);
			Ray ray = Camera.main.ScreenPointToRay (pt);
			//RaycastHit hit = Physics.Raycast(ray, Mathf.Infinity);
			RaycastHit hit = new RaycastHit();
			if(!Physics.Raycast(ray, out hit, Mathf.Infinity))
				//if(hit.collider == null)
				return;
			hit.collider.gameObject.SendMessage("EndTouch");
		}
		else if(Input.GetMouseButton(0)) {
			Vector2 pt = Input.mousePosition;
			//RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(pt), Vector2.zero);
			Ray ray = Camera.main.ScreenPointToRay (pt);
			RaycastHit hit = new RaycastHit();
			if(!Physics.Raycast(ray, out hit, Mathf.Infinity))
				//if(hit.collider == null)
				return;
			hit.collider.gameObject.SendMessage("BeginTouch", PLAY_MODE.SINGLE_NOTE);
		}

		if(Input.GetKeyDown(KeyCode.UpArrow)) {
			debugOrientation.x += 0.1f;
		}
		if(Input.GetKeyDown(KeyCode.DownArrow)) {
			debugOrientation.x -= 0.1f;
		}
		if(Input.GetKeyDown(KeyCode.LeftArrow)) {
			debugOrientation.y += 0.1f;
		}
		if(Input.GetKeyDown(KeyCode.RightArrow)) {
			debugOrientation.y -= 0.1f;
		}

		Vector3 diffVec = debugOrientation - defaultOrient;
		//KeyTouch.SetVolume((int)(50 + 30 * diffVec.x));
		Camera.main.transform.position = new Vector3(18 + 5 * diffVec.y, 0, 0);

#elif (UNITY_IPHONE || UNITY_ANDROID)
		int i = 0;
		mode = PLAY_MODE.SINGLE_NOTE;
		while (i < Input.touchCount) {
			Debug.Log("Touch Count = " + Input.touchCount);
			Vector2 pt = Input.GetTouch(i).position;
			Vector2 ptInScreen = new Vector2(pt.x, Screen.height - pt.y);
			touchPos = pt;
			if(chordPadRect.Contains(ptInScreen)) {
				// chord pad;
				//Vector2 diff = pt - 
				if(chordsRect[0].Contains(ptInScreen)) {
					mode = PLAY_MODE.CHORD_MAJOR;
				}
				else if(chordsRect[1].Contains(ptInScreen)) {
					mode = PLAY_MODE.CHORD_MINOR;
				}
				else if(sequenceRect[0].Contains(ptInScreen)) {
					mode = PLAY_MODE.SEQUENCE_MAJOR_1_4;
				}
				else if(sequenceRect[1].Contains(ptInScreen)) {
					mode = PLAY_MODE.SEQUENCE_MINOR_1_4;
				}
			//	else if(sequenceRect[2].Contains(ptInScreen)) {
			//		mode = PLAY_MODE.SEQUENCE_MAJOR_1_4;
			//	}
			//	else if(sequenceRect[3].Contains(ptInScreen)) {
			//		mode = PLAY_MODE.SEQUENCE_MINOR_1_4;
			//	}
				i++;
				continue;
			}
			i++;
		}
		i = 0;
		while (i < Input.touchCount) {
			Vector2 pt = Input.GetTouch(i).position;
			Vector2 ptInScreen = new Vector2(pt.x, Screen.height - pt.y);
			if(chordPadRect.Contains(ptInScreen)) {
				++i;
				continue;
			}
			Ray ray = Camera.main.ScreenPointToRay (pt);
			RaycastHit hit = new RaycastHit();
			if(!Physics.Raycast(ray, out hit, Mathf.Infinity))
				return;
			switch (Input.GetTouch(i).phase) {
			case TouchPhase.Began:
				hit.collider.gameObject.SendMessage("BeginTouch", mode);
				break;
			case TouchPhase.Ended:
				hit.collider.gameObject.SendMessage("EndTouch");
				break;
			case TouchPhase.Moved:
				hit.collider.gameObject.SendMessage("BeginTouch", mode);
				break;
			}
			++i;
		}
		senfu.Update();
		Vector3 diffVec = senfu.Orientation - defaultOrient;
		//KeyTouch.SetVolume((int)(50 + 30 * diffVec.x));
		if(Input.touchCount <= 0) {
			Camera.main.transform.position = new Vector3(18 + 10 * diffVec.y, 0, 0);
		}
#endif
	}
	// This function is called when the object
	// becomes enabled and active.
	void OnEnable ()
	{
		
	}
	
	// This function is called when the behaviour
	// becomes disabled () or inactive.
	void OnDisable ()
	{
		
	}
	
	// Reset to default values.
	void Reset ()
	{
		
	}

	// OnGUI is called for rendering and handling
	// GUI events.
	void OnGUI ()
	{
		// Make a background box
		GUI.Box(chordsRect[0], "Major Chord");
		GUI.Box(chordsRect[1], "Minor Chord");

		GUI.Box(sequenceRect[0], "Major Seq 1/4");
		GUI.Box(sequenceRect[1], "Minor Seq 1/4");
		//GUI.Box(sequenceRect[2], "Major Seq 1/4");
		//GUI.Box(sequenceRect[3], "Minor Seq 1/4");
		GUI.Label(new Rect(10, 10, 500, 20), new Vector2(touchPos.x, Screen.height - touchPos.y).ToString());
		GUI.Label(new Rect(10, 40, 500, 20), touchPos.ToString());
		GUI.Label(new Rect(10, 70, 500, 20), mode.ToString());
		// End the Groups and Area	
		Event e = Event.current;
		if (e.isKey)
			Debug.Log("Detected key code: " + e.keyCode);		
	}

	private void OnAudioFilterRead (float[] data, int channels)
	{
		
		//This uses the Unity specific float method we added to get the buffer
		midiStreamSynthesizer.GetNext (sampleBuffer);
		
		for (int i = 0; i < data.Length; i++) {
			data [i] = sampleBuffer [i] * gain;
		}
	}
	
	public void MidiNoteOnHandler (int channel, int note, int velocity)
	{
		Debug.Log ("NoteOn: " + note.ToString () + " Velocity: " + velocity.ToString ());
	}
	
	public void MidiNoteOffHandler (int channel, int note)
	{
		Debug.Log ("NoteOff: " + note.ToString ());
	}
*/
}
