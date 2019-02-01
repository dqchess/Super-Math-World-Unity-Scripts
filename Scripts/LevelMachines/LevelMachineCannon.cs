using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelMachineCannon : UserEditableObject {


	#region user_editable

	public override void SetProperties (SimpleJSON.JSONClass N)
	{
		base.SetProperties(N);
	}



	public override SimpleJSON.JSONClass GetProperties ()
	{
		SimpleJSON.JSONClass N = base.GetProperties();
		return N;
	}

	public override GameObject[] GetUIElementsToShow(){
		return new GameObject[] {
			LevelBuilder.inst.POCMcopyButton,
			LevelBuilder.inst.POCMheightButton
		};
	}



	#endregion
	void Start(){
		GameManager.inst.onLevelWasRestartedDelegate += ResetIgnoredNumbers;
	}
	Fraction cannonFrac = new Fraction(10,1);
	void ResetIgnoredNumbers(){
		ignoredNumbers.Clear();
	}
	public Transform startPos;
	public Transform endPos;
	public Transform firePos;
	public NumberInfo grabbingNumber;
	bool grabbing = false;
	float grabTimer = 0;
	float grabDuration = 1.2f;
	float grabSpeed = 2f;
	void Trigger(GameObject other){
		if (grabbing) return;

		NumberInfo ni = other.GetComponent<NumberInfo>();
		if (ni){
			if (ignoredNumbers.Contains(ni)){
				return;
			}


			AudioManager.inst.PlayCartoonEat(ni.transform.position,0.85f,0.2f);
			ni.transform.position = startPos.position;
			ni.GetComponent<Rigidbody>().isKinematic = true;
			ni.GetComponent<Collider>().enabled = false;
			grabbingNumber = ni;
			grabbing = true;
			grabTimer = 0;

		}
	}

	List<NumberInfo> ignoredNumbers = new List<NumberInfo>(); 
	float checkIgnoredTimer = 0;
	void Update(){
		grabTimer += Time.deltaTime;
		if (grabbing){
			if (grabbingNumber && grabbingNumber.transform){
				grabbingNumber.transform.position = Vector3.Lerp(grabbingNumber.transform.position,endPos.position,Time.deltaTime * grabSpeed);
				if (grabTimer > grabDuration){
					grabbingNumber.transform.position = firePos.position;
					grabbing = false;

					AttemptToFireNumber(grabbingNumber);

				}
			} else {
				grabbing = false;
			}

		}
		checkIgnoredTimer -= Time.deltaTime;
		if (checkIgnoredTimer <0 ){
			List<NumberInfo> toRemove = new List<NumberInfo>();
			checkIgnoredTimer = Random.Range(1.5f,2f);
			ignoredNumbers.Clear();
//			foreach(NumberInfo ni in ignoredNumbers){
//				toRemove.Add(ni);
//			}
//			foreach(NumberInfo ni in toRemove){
//				ignoredNumbers.Remove(ni);
//			}
		}
	}

	void AttemptToFireNumber(NumberInfo fireNumber){
		grabbingNumber = null;
		if (fireNumber.fraction.IsMultipleOf(cannonFrac)){ //.numerator % cannonNumber == 0 && fireNumber.fraction.denominator == 1){
			// success! Fire the number.
			GameObject rocket = SMW_GF.inst.FireRocketFromTo(firePos.position,firePos.forward,fireNumber.fraction,1,18,200,null,true);
			ScaleUpAndStop sup = rocket.AddComponent<ScaleUpAndStop>();
			sup.stopScale = Vector3.one * 3.5f;
			EffectsManager.inst.CreateSmokePuffBig(firePos.position,firePos.forward);
			Destroy(fireNumber.gameObject);
		} else {
			if (!ignoredNumbers.Contains(fireNumber)){
				ignoredNumbers.Add(fireNumber);
				checkIgnoredTimer = Random.Range(1.5f,2.5f);
			}
			fireNumber.GetComponent<Rigidbody>().isKinematic = false;
			fireNumber.GetComponent<Collider>().enabled = true;
			fireNumber.transform.position = startPos.position;
			float fireForce = 3000f;

			fireNumber.GetComponent<Rigidbody>().AddForce((-startPos.forward + startPos.right*1.5f) * fireForce); // reject number off and to the right. This prevents it rejecing into the next incoming number
			AudioManager.inst.PlayWrongAnswerError(firePos.position,1,1);
			AudioManager.inst.PlayDepressingCannonSound(firePos.position,1f);
			EffectsManager.inst.CreateSmokePuffBig(startPos.position,-startPos.forward);
			if (Vector3.Distance(Player.inst.transform.position,transform.position) < 20f) PlayerNowMessage.inst.Display("This cannon can only use multiples of 10, like 10, 20, or 30.");

		}
	}
}
