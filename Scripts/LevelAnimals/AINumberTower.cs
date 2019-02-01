using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum NumberTowerType {
	Simple, // stack is { 1, 1, 1 ( 1,1) } or {2,2,2, (2,2)}
	Addition, // stack is { 1, 2, 3 ( 4,5 ) } or {2,4,8, (16,32)}
	Multiplication // stack is { 1, 2, 4 ( 8,16) } or {2,4,8, (16,32)}
}
[System.Serializable]
public class NumberTowerInfo {
	public NumberTowerType type = NumberTowerType.Addition;
	public Color color = Color.yellow;

}

public class AINumberTower : MonsterAIBase {

	[SerializeField] public NumberTowerInfo info;
	public static string towerHeightKey = "numberTowerHeightKey";
	public int towerHeightMax = 5; // max tower size

	public int towerHeightMin = 3; // starting tower size
	public Fraction baseFraction = new Fraction(2,1);
	public Dictionary<int,NumberInfo> towerPieces = new Dictionary<int,NumberInfo>();
	GameObject playerFollower = null;
	float pieceSize = 2.5f;
	float randomOffset = 0;
	public override void Start(){
		base.Start();
		ResetReplaceTimer();
		foreach(Transform t in transform){ // we had some "fake" tower pieces that were visible for the prefab. get rid of them.
			Destroy(t.gameObject);
		}
		BuildTower();
		randomOffset = Random.Range(0,5f);
	}

	public int SetMaxTowerHeight(int newHeight){
		switch(newHeight){
			case 0: 
			case 1:
			case 2: 
				break; // because minimum 3
			case 3: 
			case 4:
				towerHeightMax = newHeight;
				towerHeightMin = newHeight - 1;
				break;
			case 5:
			case 6:
			case 7:
			case 8:
			case 9:
				towerHeightMax = newHeight;
				towerHeightMin = 3; // max start at 3..
				break;
			default: 	
				break;
		}

		return towerHeightMax;
	}

	public void SetBaseFraction(Fraction f){
		baseFraction = f;
	}

	public void DestroyTower(){
//		Debug.Log("Trying to destroy. TP ct:"+towerPieces.Count);
		foreach(KeyValuePair<int,NumberInfo> kvp in towerPieces){
			NumberInfo ni = kvp.Value;
			if (ni && ni.gameObject) {
//				Debug.Log("destroyed:"+kvp.Key+" piece.");
				NumberManager.inst.DestroyOrPool(ni);
			}
		}

		towerPieces.Clear();
	}

	bool building = false;
	public void BuildTower (float interval = .11f, bool wait = true){
		StopAllCoroutines();
		ResetReplaceTimer();

		// for addition, fraction tower is 2, 4, 6, 8
		// for multiply, fraction tower is 3, 9, 27, 81
		DestroyTower();
		StartCoroutine(BuildTowerE(interval,wait));
	}

	void ResetReplaceTimer(){
		replaceTimer = 14f;
	}

	IEnumerator BuildTowerE(float interval, bool wait = true){
		building = true;
		float randWaitOffset = wait ? Random.Range(0,1.2f) : .01f;
		yield return new WaitForSeconds(randWaitOffset);
		for (int i=0;i<towerHeightMin;i++){
			CreateTowerPiece(i);
			yield return new WaitForSeconds(interval);
		}
		building = false;
	}


	// This ensures that if a child in my stack is changed, i take longer to revert (otherwise its too hard)
	void ChildChanged(NumberInfo ni){
		ResetReplaceTimer();
	}

	void RemoveChildChanged(NumberInfo ni){
		// todos possible memory leak
		ni.numberChangedDelegate -= ChildChanged;
	}

	void CreateTowerPiece(int i){
		Fraction fr = baseFraction;
		if (info.type == NumberTowerType.Simple){
			// fr is always base fraction.
		}
		if (info.type == NumberTowerType.Addition){
			fr = Fraction.Add(baseFraction,Fraction.Multiply(new Fraction(i,1),baseFraction));
		} else if (info.type == NumberTowerType.Multiplication){
			fr = new Fraction(Mathf.Pow(baseFraction.numerator,i),baseFraction.denominator);
		}

		GameObject towerPiece = null;
		if (i == towerHeightMin - 1) {
//			Debug.Log("fr:"+fr);
			if (info.type == NumberTowerType.Simple){
				towerPiece = NumberManager.inst.CreateNumber(fr, transform.position, NumberShape.Face);
			} else if (info.type == NumberTowerType.Addition){
				towerPiece = NumberManager.inst.CreateNumber(fr, transform.position, NumberShape.SimpleHat);
			} else if (info.type == NumberTowerType.Multiplication){ 
				towerPiece = NumberManager.inst.CreateNumber(fr, transform.position, NumberShape.Hat);
			}
		} else {
			towerPiece = NumberManager.inst.CreateNumber(fr, transform.position, NumberShape.Sphere);
		}
		towerPiece.GetComponent<NumberInfo>().numberChangedDelegate += ChildChanged;
		towerPiece.GetComponent<NumberInfo>().numberDestroyedDelegate += RemoveChildChanged;

		towerPiece.transform.parent = transform;
		UserEditableObject ueo = towerPiece.GetComponent<UserEditableObject>();

		ueo.isSerializeableForSceneInstance = false;

		NumberInfo ni = towerPiece.GetComponent<NumberInfo>();
		Destroy(ni.GetComponent<PickUppableObject>());
		ni.GetComponent<Rigidbody>().isKinematic = true;
		ni.GetComponent<Rigidbody>().useGravity = false;
		ni.transform.localScale = Vector3.one * pieceSize;
		MonsterAIRevertNumber mairn = towerPiece.AddComponent<MonsterAIRevertNumber>();
		mairn.SetNumber(ni.fraction);
		if (towerPieces.ContainsKey(i)){
//			if (towerPieces[i] && towerPieces[i].gameObject){
//				Destroy(towerPieces[i].gameObject);
//			}
		}
		towerPieces.Add(i,ni);
		float pitch = 0.5f + 0.5f * ((i+1f)/(towerHeightMin));
//		Debug.Log("pitch "+pitch+" for i:"+i);
		if (!LevelBuilder.inst.levelBuilderIsShowing) AudioManager.inst.PlayBubblePop(transform.position,pitch);
//		ni.childMeshRenderer.enabled = false;
//		ni.outlineMesh.enabled = false; // these are invisible by default, we want to enable them 
		SetPositionForTowerPiece(i);
		EffectsManager.inst.RevertSparks(ni.transform.position,2);

	}

	Dictionary<int,float> numsToReplace = new Dictionary<int,float>(); // the int is the index to replace, the float is seconds which goes to 0 and when reaches 0 the num is replaced.
	float replaceTimer = 0f;


	void SetPositionForTowerPiece(int i){
		GameObject tp = towerPieces[i].gameObject;
		float wobbleSpeed = 2;
		float wobbleAmplitude = 0.5f;
		int missingPiecesCount = 0;
		for (int j=0;j<i;j++){
			if (towerPieces[j] == null || towerPieces[i].gameObject == null){
				missingPiecesCount ++;
			}
		}
		Vector3 offFloorOffset = Vector3.up * pieceSize * 0.25f;
		Vector3 pieceOffset = Vector3.up * pieceSize * (i - missingPiecesCount);
		Vector3 pos = transform.position + offFloorOffset + pieceOffset;
		tp.transform.position = pos + transform.right * (Mathf.Sin(Time.time * wobbleSpeed + i + randomOffset) * wobbleAmplitude);
	}

	float attackPlayerWithLightningRange = 13f;
	void AttackPlayerWithLightning(){
		AINumberTowerGroup aitg = transform.root.GetComponentInChildren<AINumberTowerGroup>();
		if (aitg && !aitg.mpb){
			for(int i=towerPieces.Count - 1;i> -1;i--){ // start at highest piece
				if (towerPieces[i]) {
					SMW_GF.inst.CreateLightning(towerPieces[i].transform,Player.inst.pivot,.4f);
					AudioManager.inst.PlayLightning(transform.position);
					aitg.MovePlayerBackAfterSeconds(0.1f);
					return;
				}
			}
		}
	}
	float lightningTimer =0;
	public override void MonsterUpdate(){
//		if (destroyed) { Destroy(this); return; }
//		if (LevelBuilder.inst.levelBuilderIsShowing) return;
		base.MonsterUpdate();
		lightningTimer -= Time.deltaTime;
		if (lightningTimer < 0 && Vector3.Magnitude(Player.inst.transform.position-transform.position) < attackPlayerWithLightningRange){
			lightningTimer = Random.Range(1.75f,2.5f);
			AttackPlayerWithLightning();
		}
//		// Replace numbers
//		List<int> numsToReplaceThisFrame = new List<int>();
//		List<int> valsToSubtractThisFrame = new List<int>(); // counting down to 0
//		foreach(KeyValuePair<int,float> kvp in numsToReplace){
//			
//			valsToSubtractThisFrame.Add(kvp.Key);
////			numsToReplace[kvp.Key] = newVal;
//			if (kvp.Value < 0){
//				numsToReplaceThisFrame.Add(kvp.Key);
//			}
//		}
//		foreach(int i in valsToSubtractThisFrame){
//			numsToReplace[i] -= Time.deltaTime;
//		}
//		foreach(int i in numsToReplaceThisFrame){
//			numsToReplace.Remove(i);
//			Debug.Log("removed "+i+" from nums to replace");
//			towerPieces.Remove(i);
//			CreateTowerPiece(i);
//			AudioManager.inst.PlayBubblePop(transform.position,Random.Range(.6f,1.1f));
//
//		}
//		transform.LookAt(Player.inst.transform,Vector3.up);
		// Numbers follow a base object which follow the player
		// Numbers swoon left and right on a sine wave
		bool missingPiece = false;
		bool destroyable = true; // everything dead?
		if (towerPieces.Count == 0) destroyable = false; // don't destroy if we haven't created the tower yet (no pieces to check)
		foreach (KeyValuePair<int,NumberInfo> kvp in towerPieces){
			int i = kvp.Key;
			if (towerPieces[i] == null) {
				if (!numsToReplace.ContainsKey(i)){
					// We were missing a piece. Start the replacement timer countodown.
					missingPiece = true;

				}
				continue; 
			}
			destroyable = false; // at least one thing wasn't dead
			SetPositionForTowerPiece(i);

		}
		if (destroyable){
			Destroy(gameObject);
		}
		if (missingPiece){
			replaceTimer -= Time.deltaTime;
			if (replaceTimer < 0){
				if (!building){ // at least wait until last tower was rebuilt
					towerHeightMin++; // hahahaha 
					if (info.type == NumberTowerType.Addition){
						towerHeightMin = Mathf.Min(towerHeightMin,towerHeightMax); // ok..
					} else if (info.type == NumberTowerType.Multiplication){
						towerHeightMin = Mathf.Min(towerHeightMin,towerHeightMax); // ok..
					}
					bool wait =false; // we don't want the tower to have a delay, it should begin creating right away..
					BuildTower(0.4f,wait);
				}
			}
		}
		transform.LookAt(Player.inst.transform,Vector3.up);
		KeepMinimumYPosition();
	}

	void KeepMinimumYPosition(){
//		transform.position = new Vector3(transform.position.x,Mathf.Max(myZone.transform.position.y,transform.position.y),transform.position.z);
	}


	public override void OnDestroy(){
		
		base.OnDestroy();
	}

}
