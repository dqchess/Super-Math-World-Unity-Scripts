using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelMachineSheepConverter : UserEditableObject {


	#region user_editable

	public override void SetProperties (SimpleJSON.JSONClass N)
	{
		base.SetProperties(N);
	}



	public override SimpleJSON.JSONClass GetProperties ()
	{
		return base.GetProperties();
	}

	public override GameObject[] GetUIElementsToShow(){
		return new GameObject[] {
			LevelBuilder.inst.POCMcopyButton,
			LevelBuilder.inst.POCMheightButton
		};
	}

	#endregion

	public Transform startPos;
	public Transform endPos;
	public Transform sheepOutputPos;
	public GameObject sheepPrefab;
	public List<NumberInfo> grabbingNumbers = new List<NumberInfo>();

	float grabTimer = 0;
	float grabDuration = 0.2f;
	float grabSpeed = 5f;
	void Trigger(GameObject other){
//		if (grabbing) return;
		NumberInfo ni = other.GetComponent<NumberInfo>();
		if (ni){
			AudioManager.inst.PlayCartoonEat(ni.transform.position,0.8f);
			ni.transform.position = startPos.position;
			ni.GetComponent<Rigidbody>().isKinematic = true;
			ni.GetComponent<Collider>().enabled = false;
			grabbingNumbers.Add(ni);

			grabTimer = 0;

		}
	}

	void Update(){
		List<NumberInfo> toRemove = new List<NumberInfo>();
		if (grabbingNumbers.Count > 0){
			grabTimer += Time.deltaTime;

			foreach(NumberInfo grabbingNumber in grabbingNumbers){
				if (grabbingNumber == null) {
					toRemove.Add(grabbingNumber);
					continue;
				}
				grabbingNumber.transform.position = Vector3.Lerp(grabbingNumber.transform.position,endPos.position,Time.deltaTime * grabSpeed);
				if (Vector3.Distance(grabbingNumber.transform.position,endPos.position) < .1f){
					GameObject sheep = (GameObject)Instantiate(sheepPrefab,sheepOutputPos.position,sheepOutputPos.rotation);
					float outputForce = 1200f;
					sheep.GetComponent<Rigidbody>().AddForce(sheepOutputPos.forward * outputForce);
					sheep.GetComponent<NumberInfo>().SetNumber(grabbingNumber.fraction);
					AudioManager.inst.PlayPlungerSuck(sheepOutputPos.position,1);
					AudioManager.inst.PlayRandomSheepSound(sheepOutputPos.position);
					toRemove.Add(grabbingNumber);
					Destroy(grabbingNumber.gameObject);
	//				grabbingNumber.transform.position = firePos.position;
	//				AttemptToFireNumber(grabbingNumber);

				}
			}

		}
		foreach(NumberInfo rm in toRemove){
			grabbingNumbers.Remove(rm);
		}
	}

//	void AttemptToFireNumber(NumberInfo fireNumber){
//		grabbingNumber = null;
//		if (fireNumber.fraction.numerator % 10 == 0 && fireNumber.fraction.denominator == 1){
//			SMW_GF.inst.FireRocketFromTo(firePos.position,firePos.forward,fireNumber.fraction,1,18,200,null,true);
//			EffectsManager.inst.CreateSmokePuffBig(firePos.position,firePos.forward);
//			Destroy(fireNumber.gameObject);
//		} else {
//			fireNumber.GetComponent<Rigidbody>().isKinematic = false;
//			fireNumber.GetComponent<Collider>().enabled = true;
//			fireNumber.transform.position = firePos.position;
//			float fireForce = 3000f;
//			fireNumber.GetComponent<Rigidbody>().AddForce(firePos.forward * fireForce);
//			AudioManager.inst.PlayDepressingCannonSound(firePos.position,1f);
//			EffectsManager.inst.CreateSmokePuffBig(firePos.position,firePos.forward);
//		}
//	}
}
