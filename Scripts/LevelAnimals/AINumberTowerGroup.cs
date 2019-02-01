using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AINumberTowerGroup : UEO_SimpleObject {

	public Transform back;
	public List<AINumberTower> towers = new List<AINumberTower>();
	public Fraction frac = new Fraction(0,1); // for "storing" the fraction here on the parent object of the group, because we want to know what fraction this tower was for OnDestroy.givegems
	public int gemValueMultiplier = 5;
	#region userEditable
	public override void SetProperties(SimpleJSON.JSONClass N){
		base.SetProperties(N);
		if (N.GetKeys().Contains(Fraction.fractionKey)){
			foreach(AINumberTower tw in towers){
				Fraction f = JsonUtil.ConvertJsonToFraction(Fraction.fractionKey,N);
				frac = f;
				tw.SetBaseFraction(f);
				tw.BuildTower();
			}
		}
		if (N.GetKeys().Contains(AINumberTower.towerHeightKey)){
			foreach(AINumberTower tw in towers){
				tw.SetMaxTowerHeight(N[AINumberTower.towerHeightKey].AsInt);
			}
		}
	}

	public override SimpleJSON.JSONClass GetProperties(){
		SimpleJSON.JSONClass N = base.GetProperties();
		N = JsonUtil.ConvertFractionToJson(Fraction.fractionKey,towers[0].baseFraction,N);
		N[AINumberTower.towerHeightKey].AsInt = towers[0].towerHeightMax; // note that this is fragile, depending on there only being one tower in the group OR depending on all towers in group being identical heights
		return N;
	}

	public override void OnGameStarted(){
		//		foreach(NumberInfo ni in GetComponentsInChildren<NumberInfo>()){
		//			ni.GetComponent<Rigidbody>().isKinematic = false;
		//			ni.GetComponent<Collider>().enabled = true;
		//		}
		base.OnGameStarted();
	}

	public override void OnLevelBuilderObjectPlaced(){
		foreach(AINumberTower tw in towers){
			tw.BuildTower();
		}
		base.OnLevelBuilderObjectPlaced();
	}

	public override GameObject[] GetUIElementsToShow(){
		List<GameObject> show = new List<GameObject>();
		show.AddRange(base.GetUIElementsToShow());
		show.Add(LevelBuilder.inst.POCMFractionButton);
		show.Add(LevelBuilder.inst.POCMmodTowerHeight);
		return show.ToArray();
	}

	#endregion

	// Use this for initialization

	void Start () {
		PlayerGadgetController.inst.PlayerTouched += PlayerTouched;
	}

	void PlayerTouched(GameObject o){
		NumberInfo ni = o.GetComponent<NumberInfo>();
		if (ni != null && GetComponentsInChildren<NumberInfo>().ToList().Contains(ni)){
			MovePlayerBack();
		}
	}
	public override void OnDestroy(){
		base.OnDestroy();
		PlayerGadgetController.inst.PlayerTouched -= PlayerTouched;
//		Debug.Log("Destroyed");
		EffectsManager.inst.DropGemsProbability(transform.position+Vector3.up*5f,Mathf.RoundToInt(frac.GetAsFloat())*gemValueMultiplier,1);
	}
	[System.NonSerialized] public bool mpb = false;
	float mpbs = 0;
	public void MovePlayerBackAfterSeconds(float s){
		if (!mpb){
//			Debug.Log("movepl:"+s);
			mpb = true;
			mpbs = s;
		}

	}
	public void MovePlayerBack(){
		Player.inst.FlashWhite();
		RaycastHit hit = new RaycastHit();
		float distToTerrain=0f;
		if (Physics.Raycast(new Ray(back.position,Vector3.down),out hit)){
			distToTerrain = hit.distance;
		}
		Player.inst.SetPosition(back);
		Inventory.inst.DropSomeNumbers();
		AudioManager.inst.PlayElectricDischarge1(Player.inst.transform.position);

	}


	// Update is called once per frame
	float checkDeadTimer = 0;
	void Update () {
		if (mpb){
			mpbs -= Time.deltaTime;
//			Debug.Log("s:"+mpbs);
			if (mpbs <0 ){
				MovePlayerBack();
				mpb = false;
			}
		}
		checkDeadTimer -= Time.deltaTime;
		if (checkDeadTimer < 0){
			checkDeadTimer = Random.Range(1,2f);
			int dead = 0;
			foreach (AINumberTower nt in towers){
				if (nt == null) dead++;
			}
			if (dead == towers.Count) {
				//				Debug.Log("destroyed.");
				GetComponentInChildren<ResourceDrop>().DropResource();
				Destroy(gameObject);
			}
		}
	}
}
