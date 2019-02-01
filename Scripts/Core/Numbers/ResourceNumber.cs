using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResourceNumber : MonoBehaviour, IMyUpdateable {

	public Vector3 startPos;

	// Use this for initialization
	void Start () {
		
		startPos = transform.position;
	}


	List<ResourceReceptacle> receptacles = new List<ResourceReceptacle>();

	// Update is called once per frame
	float indicatorDistance = 20f;
	public void Update () {
		if (Inventory.inst.equippedItem && Inventory.inst.equippedItem == this.gameObject){
			if (Utils.IntervalElapsed(2f)){
				receptacles = new List<ResourceReceptacle>();
				receptacles.AddRange(FindObjectsOfType<ResourceReceptacle>());
			}
			foreach(ResourceReceptacle res in receptacles){
				if (Vector3.Distance(res.transform.position,Player.inst.transform.position) < indicatorDistance){
					EffectsManager.inst.DrawDottedLine(GadgetThrow.inst.transform.position+GadgetThrow.inst.playerHoldingPos,res.transform.position, Color.blue);
					return;
				}
			}
		}
	}



}
