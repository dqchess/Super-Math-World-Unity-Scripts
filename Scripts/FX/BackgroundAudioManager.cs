using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class BackgroundAudio {
	// Note that paths are hard-coded to a location on supermathworld.com server ..
	public string name = "Visible Name";
	public string serverPath = "/media/environment_audio/name_here.mp3";
	public BackgroundAudio (string _name,string _serverPath){
		name = _name;
		serverPath = _serverPath;
	}
}



public class BackgroundAudioManager : MonoBehaviour {

	[SerializeField] public BackgroundAudio[] environmentAudios;
	[SerializeField] public BackgroundAudio[] editorAudios;
	public BackgroundAudio gameAudio = null;
	public BackgroundAudio editorAudio = null;
	public Text editorAudioText;

	public AudioClip underwater;


	public static BackgroundAudioManager inst;

	public void SetInstance(){
		inst = this;
	}

	void Start(){
		WebGLComm.inst.GetEnvironmentAudiosFromBrowser();
	}

	public void PopulateBackgroundAudiosFromBrowser(SimpleJSON.JSONClass N){
		environmentAudios = new BackgroundAudio[N["tracks"].AsArray.Count];
		WebGLComm.inst.Debug("populating " +N["tracks"].AsArray.Count+" audios.");
		int i = 0;
		foreach(SimpleJSON.JSONClass item in N["tracks"].AsArray.Childs ){
			BackgroundAudio ea = new BackgroundAudio(item["name"],item["file"]);
			environmentAudios[i] = ea;
			i++;
		}

	}

	public void EnableUnderwaterSound(){
		if (GetComponent<AudioSource>().clip != underwater){
			GetComponent<AudioSource>().clip = underwater;
			GetComponent<AudioSource>().Play ();
		}
	}

	public void DisableUnderwaterSound(){
		GetComponent<AudioSource>().Stop ();
	}

	// For playing backgorund audio in game.
	public void SetGameBackgroundAudio(string n){
		foreach(BackgroundAudio env in environmentAudios){
			if (n == env.name){
				gameAudio = env;
//				WebGLComm.inst.PlayEnvironmentAudio(env.serverPath);
			}
		}
	}
	public void PlayGameBackgroundAudio(){
		if (gameAudio != null){
			WebGLComm.inst.PlayEnvironmentAudio(gameAudio.serverPath);
		}
	}
	public void PauseEnvironmentAudio(){
		WebGLComm.inst.PauseEnvironmentAudio();
	}

	// For playing audio in editor
	public void SelectEditorAudio(string n){
		foreach(BackgroundAudio env in environmentAudios){
			if (n == env.name){
				editorAudio = env;
				WebGLComm.inst.PlayEnvironmentAudio(env.serverPath);
			}
		}
	}
	int editorAudioIndex = 0;
	public void PlayEditorAudio(){
		editorAudioIndex = Random.Range(1,editorAudios.Length);
		editorAudio = editorAudios[editorAudioIndex];
//		if (editorAudio == null){
//		}
		WebGLComm.inst.PlayEditorAudio(editorAudio.serverPath);
		editorAudioText.text = editorAudio.name;
		if (editorAudio.name == "None") editorAudioText.color = Color.white;
		else editorAudioText.color = Color.yellow;

	}
	public void PauseEditorAudio(){
		editorAudioText.text = "None";
		editorAudioText.color = Color.white;
		editorAudio = editorAudios[0]; // "none" won't play when ed opens.
		WebGLComm.inst.PauseEditorAudio();
	}
	public void NextEditorAudio(){
		editorAudioIndex++;
		editorAudioIndex %= editorAudios.Length;
		if (editorAudioIndex == 0) editorAudioIndex = 1; // skip "none", that's only for stoping.
		editorAudio = editorAudios[editorAudioIndex];
//		Debug.Log("next:"+editorAudio.name);
		WebGLComm.inst.PlayEditorAudio(editorAudio.serverPath);
		editorAudioText.text = editorAudio.name;
		editorAudioText.color = Color.yellow;
	}
}
