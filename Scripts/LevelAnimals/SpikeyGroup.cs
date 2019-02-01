using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SpikeyGroup : UEO_SimpleObject {

	public Transform back;
	public List<NumberInfo> spikeyChildren = new List<NumberInfo>();
	public Fraction frac = new Fraction(0,1); 
	int gemValueMultiplier = 2;
	#region userEditable
	public override void SetProperties(SimpleJSON.JSONClass N){
//		// commented Debug.Log("setprop:"+N.ToString());
		base.SetProperties(N);
		if (N.GetKeys().Contains(Fraction.fractionKey)){
			foreach(NumberInfo ni in spikeyChildren){
				Fraction f = JsonUtil.ConvertJsonToFraction(Fraction.fractionKey,N);
				frac = f;
				ni.SetNumber(f);
				MonsterAIRevertNumber mairn = ni.GetComponent<MonsterAIRevertNumber>();
				if (mairn) mairn.SetNumber(f); 
//				// commented Debug.Log("set spikey num;"+ni.fraction);
			}
		}

	}

	public override SimpleJSON.JSONClass GetProperties(){
		SimpleJSON.JSONClass N = base.GetProperties();
		N = JsonUtil.ConvertFractionToJson(Fraction.fractionKey,spikeyChildren[0].fraction,N);
		return N;
	}

	public override void OnGameStarted(){
		Debug.Log("Gamestarted1");
		foreach(NumberInfo ni in spikeyChildren){
			if (ni){
				ni.GetComponent<Rigidbody>().isKinematic = false;
				ni.GetComponent<Collider>().enabled = true;
				ni.SetNumber(ni.fraction); // needs to be initialized, here doesn't seem like the optimal place ..
			}
		}
		base.OnGameStarted();
	}

	public override void OnLevelBuilderObjectPlaced(){
		foreach(NumberInfo ni in spikeyChildren){
			ni.SetNumber(ni.fraction); // needs to be initialized, here doesn't seem like the optimal place ..
		}
		base.OnLevelBuilderObjectPlaced();
	}

	#endregion

	// Use this for initialization

	void Start () {
		


		PlayerGadgetController.inst.PlayerTouched += PlayerTouched;

	}

	public override void OnDestroy(){
		base.OnDestroy();
		PlayerGadgetController.inst.PlayerTouched -= PlayerTouched;
		EffectsManager.inst.DropGemsProbability(transform.position+Vector3.up*5f,frac.GetAsInt()*gemValueMultiplier,1);

	}

	void PlayerTouched(GameObject o){
		NumberInfo ni = o.GetComponent<NumberInfo>();
		if (ni != null && spikeyChildren.Contains(ni)){
			MovePlayerBack();
		}
	}

	public void MovePlayerBack(){
		Player.inst.FlashWhite();
		RaycastHit hit = new RaycastHit();
//		float distToTerrain=0f;
//		if (Physics.Raycast(new Ray(back.position,Vector3.down),out hit)){
//			distToTerrain = hit.distance;
//		}
		if (Physics.Raycast(new Ray(back.position,Vector3.down),out hit)){
			Player.inst.SetPosition(hit.point,back.rotation);	
		} else {
			Player.inst.SetPosition(back);
		}
		Inventory.inst.DropSomeNumbers();
		AudioManager.inst.PlayElectricDischarge1(Player.inst.transform.position);

	}

	public void IgnorePlayerForSeconds(float s){
		foreach(NumberInfo ni in spikeyChildren){
			ni.GetComponent<MonsterAISpikey1>().IgnorePlayerForSeconds(s);
		}
	}

	// Update is called once per frame
	float checkDeadTimer = 0;
	void Update () {
		if (SMW_CHEATS.inst.cheatsEnabled && Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.P)) {
			bool en = GetComponentInChildren<MonsterAISpikey1>().GetComponent<Collider>().enabled;
			WebGLComm.inst.Debug("Spikey debug 1: flip collider. Collider going from "+en+"to "+!en);
			foreach(NumberInfo ni in spikeyChildren){
				ni.GetComponent<Collider>().enabled = !ni.GetComponent<Collider>().enabled;
			}
		}
		checkDeadTimer -= Time.deltaTime;
		if (checkDeadTimer < 0){
			checkDeadTimer = Random.Range(1,2f);
			int dead = 0;
			foreach (NumberInfo ni in spikeyChildren){
				if (ni == null) dead++;
			}
			if (dead == spikeyChildren.Count) {
//				Debug.Log("destroyed.");
				GetComponentInChildren<ResourceDrop>().DropResource();
				Destroy(gameObject);
				Debug.Log("Destroyed all spikeys.");

			}
		}
	}
}
