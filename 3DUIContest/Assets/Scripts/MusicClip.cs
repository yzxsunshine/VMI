using System.Collections;

public enum CLIP_STATUS {
	CLIP_BEGIN,
	CLIP_CHANGE,
	CLIP_END
}

public class MusicClip {
	public float pitch;
	public float volume;
	public int frame;
	public CLIP_STATUS status;
}
