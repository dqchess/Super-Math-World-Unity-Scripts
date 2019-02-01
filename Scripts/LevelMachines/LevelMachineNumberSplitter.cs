using UnityEngine;
using System.Collections;

public class LevelMachineNumberSplitter : UserEditableObject {


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
	public Transform outputLeft;
	public Transform outputRight;

	public NumberInfo grabbingNumber;
	bool grabbing = false;
	float grabTimer = 0;
	float grabDuration = 1.2f;
	float grabSpeed = 2f;
	void Trigger(GameObject other){
		if (grabbing) return;
		NumberInfo ni = other.GetComponent<NumberInfo>();
		if (ni){
			AudioManager.inst.PlayCartoonEat(ni.transform.position,0.8f);
			ni.transform.position = startPos.position;
			ni.GetComponent<Rigidbody>().isKinematic = true;
			ni.GetComponent<Collider>().enabled = false;
			grabbingNumber = ni;
			grabbing = true;
			grabTimer = 0;

		}
	}

	void Update(){
		if (grabbing){
			grabTimer += Time.deltaTime;
			grabbingNumber.transform.position = Vector3.Lerp(grabbingNumber.transform.position,endPos.position,Time.deltaTime * grabSpeed);
			if (grabTimer > grabDuration){
				Fraction result1 = Fraction.Divide(grabbingNumber.fraction,new Fraction(2,1));	
				float spitForce = 200;
				grabbingNumber.GetComponent<NumberInfo>().SetNumber(result1);
				GameObject leftNum = (GameObject) Instantiate(grabbingNumber.gameObject,outputLeft.transform.position,grabbingNumber.transform.rotation); 
				GameObject rightNum = (GameObject) Instantiate(grabbingNumber.gameObject,outputRight.transform.position,grabbingNumber.transform.rotation); 
//				leftNum.GetComponent<NumberInfo>().SetNumber(result1); // half value per split so total "number" is preserved.
//				rightNum.GetComponent<NumberInfo>().SetNumber(result1); 
				leftNum.GetComponent<Collider>().enabled = true;
				rightNum.GetComponent<Collider>().enabled = true;
				leftNum.GetComponent<Rigidbody>().isKinematic = false;
				rightNum.GetComponent<Rigidbody>().isKinematic = false;
				leftNum.GetComponent<Rigidbody>().AddForce(outputLeft.transform.forward * spitForce);
				rightNum.GetComponent<Rigidbody>().AddForce(outputRight.transform.forward * spitForce);

				if (!grabbingNumber.GetComponent<Animal>()){
					float range = 8f;
					// Check for numbers nearby (not animals tho). Greedily combine with them for Pascal.
					foreach(Collider c in Physics.OverlapSphere(leftNum.transform.position,range)){
						if (c == leftNum.GetComponent<Collider>()) continue;
						if (c.GetComponent<NumberInfo>() && c.GetComponent<GameObject>() != rightNum){
							leftNum.GetComponent<NumberInfo>().Eat(c.GetComponent<NumberInfo>());
						}
					}

					foreach(Collider c in Physics.OverlapSphere(rightNum.transform.position,range)){
						if (c == rightNum.GetComponent<Collider>()) continue;
						if (c.GetComponent<NumberInfo>() && c.GetComponent<GameObject>() != leftNum){
							rightNum.GetComponent<NumberInfo>().Eat(c.GetComponent<NumberInfo>());
						}
					}
				}

				Destroy(grabbingNumber.gameObject);
//				grabbingNumber.transform.position = firePos.position;
				grabbing = false;
//				AttemptToFireNumber(grabbingNumber);

			}

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
