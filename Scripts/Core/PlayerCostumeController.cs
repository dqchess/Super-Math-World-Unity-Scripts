using UnityEngine;
using System.Collections;

[System.Serializable]
public class CharacterInfo {
	public CharacterType characterType;
	public Animator myAnimator;
	public Transform root;
	public Transform playerRightArm;
	public Transform playerBody;
	public Transform playerHead;
	public SkinnedMeshRenderer playerBodyGraphics;
	public Transform playerMeshes;
	public MascotAnimatorController animController;
	public GadgetThrow gadgetThrow;
}

public enum CharacterType {
	Simple_Boy,
	Simple_Girl
}

public class PlayerCostumeController : MonoBehaviour {

	public Transform playerHeadObjects;
	public static PlayerCostumeController inst;
	[SerializeField] public CharacterInfo[] characters;

	public CharacterType currentCharacter;
	public CharacterInfo curCharInfo;
	public Renderer playerHeadGraphics;


//	public Transform[] hats;
	public Transform[] hair;
	public Material[] allMaterials;

	public void GetCostumeJsonArray(){
		SimpleJSON.JSONClass N = new SimpleJSON.JSONClass();
//		N["Hats"] = new SimpleJSON.JSONArray();
		N["Hair"] = new SimpleJSON.JSONArray();
		N["Materials"] = new SimpleJSON.JSONArray();
//		foreach(Transform t in hats){
//			N["Hats"].Add(t.name);
//		}
		foreach(Transform t in hair){
			N["Hair"].Add(t.name);
		}
		foreach(Material m in allMaterials){
			N["Materials"].Add(m.name);
		}
		// commented Debug.Log(N.ToString());
	}

	public void SetInstance(){
		inst = this;
	}
	public bool initialized = false;
	void Start(){
		#if UNITY_EDITOR
		if (mainPlayer){
//			var c = SetRandomBody();
//			var  c = "{\"HairColorIndex\": 6, \"BodyColorIndex\": 7, \"BodyIndex\": 0, \"HairIndex\": 3, \"HeadColorIndex\": 5 }";
			characterJson = SetRandomBody();
			WebGLComm.inst.SetCharacterCostume(characterJson);
//			InitCharacter(c);
//			SetCharacterMaterials(c);
		}
		#endif

	}

	public string characterJson = "";
	string SetRandomBody(){
		int bodInd = Random.Range(0,2);
		SimpleJSON.JSONClass N = new SimpleJSON.JSONClass();
//		N["HatIndex"].AsInt = Random.Range(0,4);
		N["HairIndex"].AsInt = Random.Range(0,4);
		N["BodyIndex"].AsInt = bodInd;
		N["HairColorIndex"].AsInt = Random.Range(0,allMaterials.Length);
		N["BodyColorIndex"].AsInt = Random.Range(0,allMaterials.Length);
		N["HeadColorIndex"].AsInt = Random.Range(0,allMaterials.Length);
		return N.ToString();
	}


	void Update(){
		
//		if (Input.GetKeyDown(KeyCode.B)){
//			CycleBodyColor();
//		} else if (Input.GetKeyDown(KeyCode.R)) {
//			SetRandomBody();
//		}
	}

	int bi = 0;
	void CycleBodyColor(){
		curCharInfo.playerBodyGraphics.material = allMaterials[bi];
		bi ++;
		bi = bi % allMaterials.Length;
	}


	int bodyColorIndex = 0;
	int headColorIndex = 0;
	int bodyIndex = 0;
	int hairIndex = 0;
	int hairColorIndex = 0;

	public bool mainPlayer = false; // only the actual player has this boolean checked, others are heatmap clones
	//		

	public void InitCharacter(string jsonString){
		if (initialized) return;
		characterJson = jsonString; // for resetting character to originally selected materials later -- because at times we make the character transparent for camera wall manager.
		DeactivateAllCharacterObjects();
		SimpleJSON.JSONClass N = new SimpleJSON.JSONClass();
		N = (SimpleJSON.JSONClass)SimpleJSON.JSONNode.Parse(jsonString);
		bodyIndex = N["BodyIndex"].AsInt;
		hairIndex = N["HairIndex"].AsInt;
		characters[bodyIndex].root.gameObject.SetActive(true);
		curCharInfo = characters[bodyIndex];
		currentCharacter = bodyIndex == 0 ? CharacterType.Simple_Boy : CharacterType.Simple_Girl;
		if (mainPlayer) GameManager.inst.SetThrowGadgetInstance(curCharInfo.gadgetThrow);
		if (mainPlayer) MascotAnimatorController.inst.Init(curCharInfo.myAnimator);
		hair[hairIndex].gameObject.SetActive(true);
		initialized = true;
	}

	public void SetCharacterMaterials(string jsonString){
//		// commented Debug.Log("set char! jsonstring:"+jsonString);
		SimpleJSON.JSONClass N = new SimpleJSON.JSONClass();
		N = (SimpleJSON.JSONClass)SimpleJSON.JSONNode.Parse(jsonString);

		//		int hatIndex = N["HatIndex"].AsInt;
		bodyColorIndex = N["BodyColorIndex"].AsInt;
		headColorIndex = N["HeadColorIndex"].AsInt;
		bodyIndex = N["BodyIndex"].AsInt;
		hairIndex = N["HairIndex"].AsInt;
		hairColorIndex = N["HairColorIndex"].AsInt;
		// Set current player body and activate

		// Move head object to current active player
		playerHeadObjects.transform.parent = curCharInfo.playerHead;
		playerHeadObjects.transform.localPosition = Vector3.zero;
		playerHeadObjects.transform.localRotation = Quaternion.identity;
		// Init animator for current player

//		 Set hat and hair style based on index, note that 0 is None



		// Set head and body color based on index

		Material[] mats = playerHeadGraphics.materials;
		for (int i=0;i<mats.Length;i++){
			mats[i] = allMaterials[headColorIndex];
		}
		playerHeadGraphics.materials = mats;
		curCharInfo.playerBodyGraphics.material = allMaterials[bodyColorIndex];

		// Set hair color based on index
		if (hair[hairIndex].GetComponent<Renderer>()){
			mats = hair[hairIndex].GetComponent<Renderer>().materials;
			for (int i=0;i<mats.Length;i++){
				mats[i] = allMaterials[hairColorIndex];
			}
			hair[hairIndex].GetComponent<Renderer>().materials = mats;
		}

	}

	public Material transpMaterial; // for when the camera wall manager pushes the camera too close to the player, need to render transparent so view isn't blocked.
	public void SetPlayerTransparent(){
		if (initialized){
			Material[] mats;
			if (hair[hairIndex].GetComponent<Renderer>()){
				mats = hair[hairIndex].GetComponent<Renderer>().materials;
				for (int i=0;i<mats.Length;i++){
					mats[i] = transpMaterial;
				}
				hair[hairIndex].GetComponent<Renderer>().materials = mats;
			}
			mats = playerHeadGraphics.materials;
			for (int i=0;i<mats.Length;i++){
				mats[i] = transpMaterial;
			}
			playerHeadGraphics.materials = mats;
			curCharInfo.playerBodyGraphics.material = transpMaterial;
		}

	}

	public void SetPlayerOpaque(){
		if (characterJson != "" && initialized){ // prevent setting to null since savedjson needs to be populated by the js SetCharacter() before there's any data to set
			SetCharacterMaterials(characterJson);
		}
	}

	void DeactivateAllCharacterObjects(){
		for (int i=0;i<characters.Length;i++){
			characters[i].root.gameObject.SetActive(false);
		}
//		foreach(Transform t in hats){
//			t.gameObject.SetActive(false);
//		}
		foreach(Transform t in hair){
			t.gameObject.SetActive(false);
		}
	}

	public void SetGrounded(bool flag){
		if (curCharInfo != null && curCharInfo.animController != null) curCharInfo.animController.SetGrounded(flag);
	}


}
