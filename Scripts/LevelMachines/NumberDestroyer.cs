using UnityEngine;
using System.Collections;

public class NumberDestroyer: MonoBehaviour {


	public bool destroyNumbers=true;
	public bool destroyPlayerNumbers=true;
	public bool destroyGadgets = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter(Collider other){
		if (LevelBuilder.inst.levelBuilderIsShowing) return;
		if (destroyNumbers){
			NumberInfo ni = other.GetComponent<NumberInfo>();
			if (ni){
				EffectsManager.inst.EmitLightningBall(ni.transform.position);
				Destroy (ni.gameObject);
				AudioManager.inst.PlayElectricDischarge2(ni.transform.position,.2f);
//					other.SendMessage("SetStability",false);
//					other.SendMessage("SetInstabilityTimer",.2f);
			}

			else if (other.tag=="Player" && destroyPlayerNumbers){
//				GlobalVars.inst.pnc.DropAmmo();
//				PlayerInventory.inst.ClearInventory();
				Inventory.inst.ClearNumbersFromInventory();
				EffectsManager.inst.EmitLightningBall(Player.inst.transform.position+Vector3.up);
				EffectsManager.inst.EmitLightningBall(Player.inst.transform.position+Vector3.up*2);
				EffectsManager.inst.EmitLightningBall(Player.inst.transform.position+Vector3.up*3);
				AudioManager.inst.PlayElectricDischarge2(Player.inst.transform.position,.2f);
			}
		}
		if (destroyGadgets){
			if (other.tag=="Player"){
				Inventory.inst.ClearGadgetsFromInventory();
			}
		}
	}
}
