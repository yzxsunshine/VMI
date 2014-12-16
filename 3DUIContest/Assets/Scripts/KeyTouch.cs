using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CSharpSynth.Effects;
using CSharpSynth.Sequencer;
using CSharpSynth.Synthesis;
using CSharpSynth.Midi;

public enum PLAY_MODE {
	SINGLE_NOTE,
	CHORD_MAJOR,
	CHORD_MINOR,
	SEQUENCE_MAJOR_1_2,
	SEQUENCE_MINOR_1_2,
	SEQUENCE_MAJOR_1_4,
	SEQUENCE_MINOR_1_4,
	SEQUENCE_MAJOR_1_8,
	SEQUENCE_MINOR_1_8
};

[RequireComponent (typeof(AudioSource))]
public class KeyTouch : MonoBehaviour {
	public int distToMidC = 0;
	public static int noteVolume = 100;
	public Vector3 defaultPosition;
	public Quaternion defaultRotation;
	public Vector3 pressPosition;
	public Quaternion pressRotation;
	private bool isPress = false;

	private StreamSynthesizer midiStreamSynthesizer;
	private static int midCNote;
	private static int instrument;
	private PLAY_MODE chordMode;

	private int passTime = 0;
	private int deltaTime = 0;
	private int currentNode = 0;
	private bool isPlaySeq = false;	// whether is playing a sequence

	private static int[] majorChordOffset = {0, 4, 7};
	private static int[] minorChordOffset = {0, 3, 7};

	// Use this for initialization
	void Start () {
		defaultPosition = transform.position;
		defaultRotation = transform.rotation;
		pressPosition = defaultPosition + new Vector3(0, -0.005f, 0);
		pressRotation = Quaternion.Euler(defaultRotation.eulerAngles + new Vector3(5.0f, 0, 0));
		
		BoxCollider bc = gameObject.AddComponent("BoxCollider") as BoxCollider;
		int divVal = distToMidC / 12;
		int modVal = (distToMidC + 12 * 10) % 12;	// to deal with the negative numbers
		if(distToMidC < 0 && distToMidC % 12 != 0) {
			divVal--;
			Debug.Log("distToMidC: " + distToMidC + ", divVal: " + divVal + ", modVal" + modVal);
		}
		switch (modVal) {
		case 0:
			bc.size = new Vector3(0.00907f, 0.01f, 0.019f);
			bc.center = new Vector3((divVal * 7) * -0.00907f - 0.2683f, 0.0035f, -0.0093f);
			break;
		case 1:
			bc.size = new Vector3(0.006f, 0.014f, 0.036f);
			bc.center = new Vector3((divVal * 7) * -0.00907f - 0.272f, 0.006f, -0.038f);
			break;
		case 2:
			bc.size = new Vector3(0.00907f, 0.01f, 0.019f);
			bc.center = new Vector3((divVal * 7 + 1) * -0.00907f - 0.2683f, 0.0035f, -0.0093f);
			break;
		case 3:
			bc.size = new Vector3(0.006f, 0.014f, 0.036f);
			bc.center = new Vector3((divVal * 7) * -0.00907f - 0.272f - 0.011f, 0.006f, -0.038f);
			break;
		case 4:
			bc.size = new Vector3(0.00907f, 0.01f, 0.019f);
			bc.center = new Vector3((divVal * 7 + 2) * -0.00907f - 0.2683f, 0.0035f, -0.0093f);
			break;
		case 5:
			bc.size = new Vector3(0.00907f, 0.01f, 0.019f);
			bc.center = new Vector3((divVal * 7 + 3) * -0.00907f - 0.2683f, 0.0035f, -0.0093f);
			break;
		case 6:
			bc.size = new Vector3(0.006f, 0.014f, 0.036f);
			bc.center = new Vector3((divVal * 7) * -0.00907f - 0.272f - 0.0267f, 0.006f, -0.038f);
			break;
		case 7:
			bc.size = new Vector3(0.00907f, 0.01f, 0.019f);
			bc.center = new Vector3((divVal * 7 + 4) * -0.00907f - 0.2683f, 0.0035f, -0.0093f);
			break;
		case 8:
			bc.size = new Vector3(0.006f, 0.014f, 0.036f);
			bc.center = new Vector3((divVal * 7) * -0.00907f - 0.272f - 0.0371f, 0.006f, -0.038f);
			break;
		case 9:
			bc.size = new Vector3(0.00907f, 0.01f, 0.019f);
			bc.center = new Vector3((divVal * 7 + 5) * -0.00907f - 0.2683f, 0.0035f, -0.0093f);
			break;
		case 10:
			bc.size = new Vector3(0.006f, 0.014f, 0.036f);
			bc.center = new Vector3((divVal * 7) * -0.00907f - 0.272f - 0.0474f, 0.006f, -0.038f);
			break;
		case 11:
			bc.size = new Vector3(0.00907f, 0.01f, 0.019f);
			bc.center = new Vector3((divVal * 7 + 6) * -0.00907f - 0.2683f, 0.0035f, -0.0093f);
			break;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(isPress) {
			transform.position = pressPosition;
			transform.rotation = pressRotation;

			int i=0; 

			if(isPlaySeq) {
				passTime += (int)(Time.deltaTime * 1000);
				if(passTime > (currentNode + 1) * deltaTime) {
					int offset = majorChordOffset[currentNode];
					switch (chordMode) {
					case PLAY_MODE.SEQUENCE_MAJOR_1_2:
					case PLAY_MODE.SEQUENCE_MAJOR_1_4:
					case PLAY_MODE.SEQUENCE_MAJOR_1_8:
						offset = majorChordOffset[currentNode];
						break;
					case PLAY_MODE.SEQUENCE_MINOR_1_2:
					case PLAY_MODE.SEQUENCE_MINOR_1_4:
					case PLAY_MODE.SEQUENCE_MINOR_1_8:
						offset = minorChordOffset[currentNode];
						break;
					}
					midiStreamSynthesizer.NoteOff (1, midCNote + distToMidC + offset);
					currentNode++;
					if(currentNode > 2) {
						passTime = passTime - currentNode * deltaTime;
						currentNode = 0;
					}
					switch (chordMode) {
					case PLAY_MODE.SEQUENCE_MAJOR_1_2:
					case PLAY_MODE.SEQUENCE_MAJOR_1_4:
					case PLAY_MODE.SEQUENCE_MAJOR_1_8:
						offset = majorChordOffset[currentNode];
						break;
					case PLAY_MODE.SEQUENCE_MINOR_1_2:
					case PLAY_MODE.SEQUENCE_MINOR_1_4:
					case PLAY_MODE.SEQUENCE_MINOR_1_8:
						offset = minorChordOffset[currentNode];
						break;
					}
					midiStreamSynthesizer.NoteOn (1, midCNote + distToMidC + offset, noteVolume, instrument);
				}
			}

			bool hited = false;
#if (UNITY_EDITOR_WIN || UNITY_STANDALONE)
			Vector2 pt = Input.mousePosition;
			//RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(pt), Vector2.zero);
			Ray ray = Camera.main.ScreenPointToRay (pt);
			RaycastHit hit = new RaycastHit();
			if(Physics.Raycast(ray, out hit, Mathf.Infinity) && hit.transform == this.transform) {
				hited = true;
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
				isPlaySeq = false;
				EndTouch();
			}
		}
		else {
			transform.position = defaultPosition;
			transform.rotation = defaultRotation;
		}
	}

	public void BeginTouch (PLAY_MODE mode) {
		if(!isPress) {
			StopSound();
			chordMode = mode;
			Debug.Log("Press");
			PlaySound();
			isPress = true;
		}
	}

	public void EndTouch() {
		StopSound();
		isPress = false;
	}

	public void SetStreamTynthesizer(StreamSynthesizer ss) {
		midiStreamSynthesizer = ss;
	}

	public static void SetMiddleCNode(int midC) {
		midCNote = midC;
	}

	public static void SetVolume(int v) {
		noteVolume = v;
	}

	public static void SetInstrument(int i) {
		instrument = i;
	}

	void PlaySound() {
		isPlaySeq = false;
		switch (chordMode) {
		case PLAY_MODE.SINGLE_NOTE:
			midiStreamSynthesizer.NoteOn (1, midCNote + distToMidC, noteVolume, instrument);
			break;
		case PLAY_MODE.CHORD_MAJOR:
			midiStreamSynthesizer.NoteOn (1, midCNote + distToMidC + majorChordOffset[0], noteVolume, instrument);
			midiStreamSynthesizer.NoteOn (1, midCNote + distToMidC + majorChordOffset[1], noteVolume, instrument);
			midiStreamSynthesizer.NoteOn (1, midCNote + distToMidC + majorChordOffset[2], noteVolume, instrument);
			break;
		case PLAY_MODE.CHORD_MINOR:
			midiStreamSynthesizer.NoteOn (1, midCNote + distToMidC + minorChordOffset[0], noteVolume, instrument);
			midiStreamSynthesizer.NoteOn (1, midCNote + distToMidC + minorChordOffset[1], noteVolume, instrument);
			midiStreamSynthesizer.NoteOn (1, midCNote + distToMidC + minorChordOffset[2], noteVolume, instrument);
			break;
		case PLAY_MODE.SEQUENCE_MAJOR_1_2:
		case PLAY_MODE.SEQUENCE_MINOR_1_2:
			midiStreamSynthesizer.NoteOn (1, midCNote + distToMidC, noteVolume, instrument);
			deltaTime = 500;
			passTime = 0;
			currentNode = 0;
			isPlaySeq = true;
			break;
		case PLAY_MODE.SEQUENCE_MAJOR_1_4:
		case PLAY_MODE.SEQUENCE_MINOR_1_4:
			midiStreamSynthesizer.NoteOn (1, midCNote + distToMidC, noteVolume, instrument);
			deltaTime = 250;
			passTime = 0;
			currentNode = 0;
			isPlaySeq = true;
			break;
		case PLAY_MODE.SEQUENCE_MAJOR_1_8:
		case PLAY_MODE.SEQUENCE_MINOR_1_8:
			midiStreamSynthesizer.NoteOn (1, midCNote + distToMidC, noteVolume, instrument);
			deltaTime = 125;
			passTime = 0;
			currentNode = 0;
			isPlaySeq = true;
			break;
		}
	}

	void StopSound() {
		switch (chordMode) {
		case PLAY_MODE.SINGLE_NOTE:
			midiStreamSynthesizer.NoteOff (1, midCNote + distToMidC);
			break;
		case PLAY_MODE.CHORD_MAJOR:
			midiStreamSynthesizer.NoteOff (1, midCNote + distToMidC);
			midiStreamSynthesizer.NoteOff (1, midCNote + distToMidC + 4);
			midiStreamSynthesizer.NoteOff (1, midCNote + distToMidC + 7);
			break;
		case PLAY_MODE.CHORD_MINOR:
			midiStreamSynthesizer.NoteOff (1, midCNote + distToMidC);
			midiStreamSynthesizer.NoteOff (1, midCNote + distToMidC + 3);
			midiStreamSynthesizer.NoteOff (1, midCNote + distToMidC + 7);
			break;
		case PLAY_MODE.SEQUENCE_MAJOR_1_2:
		case PLAY_MODE.SEQUENCE_MAJOR_1_4:
		case PLAY_MODE.SEQUENCE_MAJOR_1_8:
			midiStreamSynthesizer.NoteOff (1, midCNote + distToMidC + majorChordOffset[currentNode]);
			passTime = 0;
			currentNode = 0;
			break;
		case PLAY_MODE.SEQUENCE_MINOR_1_2:
		case PLAY_MODE.SEQUENCE_MINOR_1_4:
		case PLAY_MODE.SEQUENCE_MINOR_1_8:
			midiStreamSynthesizer.NoteOff (1, midCNote + distToMidC + minorChordOffset[currentNode]);
			passTime = 0;
			currentNode = 0;
			break;
		}
	}
}
