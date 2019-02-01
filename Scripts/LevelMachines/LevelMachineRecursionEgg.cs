using UnityEngine;
using System.Collections;

public class LevelMachineRecursionEgg : MonoBehaviour {




	NumberInfo ni;
	void Start(){
		ni = GetComponent<NumberInfo>();
	}

	float t = 0; // countdown to revert
	bool hatching = false;
	bool hatched = false;
	void Update(){
		if (Input.GetKeyDown(KeyCode.H)){
			HatchAfterSeconds(1);
		}

		if (hatching && !hatched){
			float mt = totalTimeToHatch - t;
			t -= Time.deltaTime;
			ni.childMeshRenderer.material.SetColor("_Glow", new Color(0.5f, 0.5f, 0.5f, t/5 + 0.1f));

			if (t < 0){
				Hatch ();
			}
		}


	}

	public void TurnIntoEgg(){
//		ni.childMeshRenderer.gameObject.SetActive(
	}

	float totalTimeToHatch = 1;
	public void HatchAfterSeconds(float s){
		t = s;
		totalTimeToHatch = s;
		AudioManager.inst.PlayIceCrackle(transform.position,1,.4f);
		hatching = true;
	}

	public void Hatch(){
		AudioManager.inst.PlayCrystalThump1(transform.position);
		hatched = true;
		int numChildrenToHatch = Mathf.FloorToInt(ni.fraction.GetAsFloat());
		GameObject hatchedParent = new GameObject("Hatch Parent"); // to prohibit combinations per the parent rule
		for (int i = 0; i < numChildrenToHatch; i++){
			float deltaAngle = 360 / numChildrenToHatch * i;
			Vector3 dirFromCenter = Quaternion.Euler (0,deltaAngle,0) * Vector3.forward;
			float distFromCenter = 5;
			Vector3 dest = transform.position + dirFromCenter * distFromCenter;
			GameObject hatchling = (GameObject)Instantiate (gameObject,dest,Quaternion.identity);
		}
		Destroy (gameObject);
	}

}
