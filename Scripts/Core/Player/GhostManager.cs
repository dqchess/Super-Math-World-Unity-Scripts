using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GhostManager : MonoBehaviour {


	List<NumberInfo> ghostsDispatched = new List<NumberInfo>();
	public Fraction ghostThreshhold = new Fraction(100,1); // the abs value at which numbers greater than this, ghosts start appearing
	public GameObject ghostPrefab;
	void Start(){
		GameManager.inst.onLevelWasRestartedDelegate += LevelRestarted;
	}

	void LevelRestarted(){
		foreach(NumberInfo ni in ghostsDispatched){
			
			if (ni) {
				ni.GetComponent<NumberGhost>().LevelRestarted();
//				Destroy(ni.gameObject);
			}
		}
		ghostsDispatched.Clear();
	}

	// Update is called once per frame
	float ghostCheck = 0;

	void Update () {
		ghostCheck -= Time.deltaTime;
		if (ghostCheck < 0){
			ghostCheck = Random.Range(2f,4f);
			foreach(NumberInfo ni in NumberManager.inst.GetAllNumbersInScene()){
				if (ni){
					if (Fraction.AbsGreater(ni.fraction,ghostThreshhold)){
						DispatchGhost(ni.fraction);
					}
				}
			}
		}
	}

	void DispatchGhost(Fraction fr){
//		Debug.Log("dispatched:+"+fr);
		foreach(NumberInfo ni in ghostsDispatched){
			if (Fraction.Equals(ni.fraction,fr)) return; // don't make duplicate ghosts!
		}
		float ghostRadius = Random.Range(80f,100f);
		GameObject ghost = (GameObject) Instantiate(ghostPrefab,Player.inst.transform.position + Utils.FlattenVector(Random.insideUnitSphere) * ghostRadius + Vector3.up * Random.Range(5,10f),Quaternion.identity);
		NumberInfo ni2  = ghost.GetComponent<NumberInfo>();
		ghostsDispatched.Add(ni2);
		ni2.SetNumber(fr);
	}
}
