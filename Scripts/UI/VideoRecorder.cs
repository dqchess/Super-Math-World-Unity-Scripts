using UnityEngine;
using UnityEngine.UI;
using System; // for audio 
using System.Collections;
using System.Collections.Generic;

public enum ReplayState {
	Ready,
	Initializing,
	Recording,
	Saving
}

public class VideoRecorder : MonoBehaviour {

	public static VideoRecorder inst;
	public Sprite camIcon;
	public Text recordText;
	public Image recordButton;
	public GameObject blinkFuzz;
	public Camera skyboxCamera;
	string defaultRecordText = "VIDEO";
	public void SetInstance(){
	
		inst = this;
	}

	public ReplayState replayState = ReplayState.Ready;
//	public RenderTexture rawSource;
	// Use this for initialization

	int framesToSkip = 1;
	int frames = 0;
	int blinkFrames = 0;
	int blinkInterval=20;
	int resW = 512;
	int resH = 288;
//	int resW = 768;
//	int resH = 432;
//	int resW  = 384;
//	int resH = 216;
	int shotIndex = 0;
	Color offColor = new Color(1,0.5f,0.5f);


	float videoTimer = 0;

//	int audBufferSize = 0;
//	int audNumBuffers = 0;
//	int outputRate = 44100;
//	int headerSize = 44; //default for uncompressed wav

	void Awake(){
//		// commented Debug.Log("video awake");
	}
	RenderTexture rt;
	Texture2D shot;
	void Start(){
		recordText.text = defaultRecordText;
		rt = new RenderTexture(resW, resH,24);
		shot = new Texture2D(resW, resH, TextureFormat.RGB24, true);
//		// commented Debug.Log("video start");
//		AudioSettings.outputSampleRate = outputRate;
//		AudioSettings.GetDSPBufferSize(out audBufferSize,out audNumBuffers);
	}



	void Update () {
		if (LevelBuilder.inst.levelBuilderIsShowing) return;
		if (GameManager.inst.CanDisplayDialogue() && Input.GetKeyDown(KeyCode.R)){
			WebGLComm.inst.Debug("Reocrd attempt during state;"+replayState.ToString());
			if (replayState == ReplayState.Ready){
				PlayerDialogue.inst.playerPressedOKDelegate += StartRecording;
				PlayerDialogue.inst.playerPressedCancelDelegate += PlayerCanceled;
				PlayerDialogue.inst.ShowPlayerDialogue("Do you want to record yourself playing? Your video will be shared on VIMEO.COM","Record replay!",camIcon);
			} else {
				StopRecording();
			}
			
		}
		if (replayState == ReplayState.Recording) {
			videoTimer += Time.deltaTime;
			frames++;
			if (frames > framesToSkip){
				frames = 0;
				RecordShot(shotIndex);
				shotIndex++;
				if (shotIndex > 300) {
					StopRecording();
				}

			}
			blinkFrames++;
			if (blinkFrames%blinkInterval==0){
				ToggleBlink();
			}
		}
	}

//	void LateUpdate(){
//		if (recordShotThisFrame){
//			recordShotThisFrame = false; 
//
//		}
//	}


	void ToggleBlink(){
		bool blinkWasOn = recordButton.color == Color.red;
		recordButton.color = blinkWasOn ? offColor : Color.red;
		blinkFuzz.SetActive(!blinkWasOn);
	}


	void RecordShot(int i){
		skyboxCamera.targetTexture = rt;
		skyboxCamera.Render();
		Camera.main.targetTexture = rt;
//		rt.useMipMap = true;
		//		// commented Debug.Log("render texture stats. mipmap?"+rt.useMipMap+", renderbuffer:"+rt.colorBuffer);
		Camera.main.Render();
		RenderTexture.active = rt;
		shot.ReadPixels(new Rect(0, 0, resW, resH), 0, 0); // Readpixels doesn't take the RenderTexture singleton as an argument but assume the pixels are being read to the current RenderTexture.
//		byte[] bArray = shot.EncodeToJPG(80);
		byte[] bArray = shot.EncodeToJPG(55);
		string data = System.Convert.ToBase64String(bArray);
		#if UNITY_EDITOR
		System.IO.File.WriteAllBytes("screen_replay_"+i+".jpg",bArray);
		#else
		WebGLComm.inst.SaveSingleJpgForVideo(i,data);

		#endif
		RenderTexture.active = null;
		Camera.main.targetTexture = null;
		skyboxCamera.targetTexture = null;

////		Camera.main.Render();
//		RenderTexture.active = null;
//		Destroy(rt);
		
	}



//	void ToggleRecording(){
//		
//		if (replayState == ReplayState.Recording) StopRecording();
//		else if (replayState == ReplayState.Ready) StartRecording();
//	}

	void StopRecording(){
		replayState = ReplayState.Saving;
		recordText.text = "Replay is saving ... ";
		// commented Debug.Log("Finished! Frame: "+shotIndex+", total time:"+videoTimer);

		blinkFuzz.SetActive(false);
		AudioManager.inst.PlayCameraShutter();
		recordButton.color = offColor;
		string fps = ((float)shotIndex/(float)videoTimer).ToString("n2");
		WebGLComm.inst.FinishedVideoRecording(audioN.ToString(),fps);


	}

	public void RecordingCompleted(){
		replayState = ReplayState.Ready;
		recordText.text = defaultRecordText;
	}

	void PlayerCanceled(){
		PlayerDialogue.inst.playerPressedOKDelegate -= StartRecording;
		PlayerDialogue.inst.playerPressedCancelDelegate -= PlayerCanceled;
	}

	void StartRecording(){
//		if (replayState == ReplayState.Ready) {
		PlayerDialogue.inst.playerPressedOKDelegate -= StartRecording;
		PlayerDialogue.inst.playerPressedCancelDelegate -= PlayerCanceled;
		replayState = ReplayState.Initializing;
		#if UNITY_EDITOR
		VideoRecordingInitialized();
		#else 
		WebGLComm.inst.InitVideoRecording();

		#endif
//		}
	}

	
	public void VideoRecordingInitialized(){
		AudioManager.inst.PlayHeavyClick(Player.inst.transform.position);
		recordButton.color = Color.red;
		blinkFuzz.SetActive(true);
		recordText.text = "Recording! Press R to STOP";
		shotIndex = 0;
		videoTimer = 0;
		replayState = ReplayState.Recording;

//		// audio
		audioN = new SimpleJSON.JSONClass();

	}

	SimpleJSON.JSONClass audioN = new SimpleJSON.JSONClass();

	public void RegisterSoundClip(string name, float vol, float pitch, float length){
		if (length < .1f){
//			// commented Debug.Log("not registering;"+name);
			return;
		}
//		// commented Debug.Log("name;"+name);
//		// commented Debug.Log("exist?"+audioN[shotIndex.ToString()]);
		string si = shotIndex.ToString("D5");
		if (audioN[si] == null || audioN[si] == "") audioN[si] = new SimpleJSON.JSONArray();
		SimpleJSON.JSONClass n = new SimpleJSON.JSONClass();
		n["name"] = name;
		n["vol"].AsFloat = 0.1f + vol * 0.5f; // WAS normalize to 0.2-1 with 0.2f + vol * 0.8f. Now wtf, it's too loud so this should quiet the soudns a bit
		n["pitch"].AsFloat = pitch;
		audioN[si].Add(n);
//		N[shotIndex][
	}



}
