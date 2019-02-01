using UnityEngine;
using System.Collections;

public enum AxisType{
	X,
	Y,
	Z
}

[System.Serializable]
public class XyzAxis {
	public AxisType axisType;
	public Transform inputStart;
	public Transform inputFinal;
	public Transform heldPosition;
	public bool moving = false;
	public NumberInfo heldNumber;
}

public class LevelMachine_XYZ : MonoBehaviour {

	[SerializeField]
	public XyzAxis[] axis;
	Vector3 targetPosition;
	bool moving = false;
	Vector3 startPosition;
	void Start(){
		startPosition = transform.position;
//		targetPosition = transform.position;
		// commented Debug.Log("startpos;"+targetPosition);
	}


	public void ReceiveNumberX(GameObject o){
		NumberInfo ni = o.GetComponent<NumberInfo>();
		if (ni){
			ReceiveNumberInput(axis[0],ni);
		}
	}
	public void ReceiveNumberY(GameObject o){
		NumberInfo ni = o.GetComponent<NumberInfo>();
		if (ni){
			ReceiveNumberInput(axis[1],ni);
		}
	}
	public void ReceiveNumberZ(GameObject o){
		NumberInfo ni = o.GetComponent<NumberInfo>();
		if (ni){
			ReceiveNumberInput(axis[2],ni);
		}
	}

	public void ReceiveNumberInput(XyzAxis a, NumberInfo ni){
		// commented Debug.Log("rec:"+ni);
		if (a.heldNumber){
			Destroy(a.heldNumber);
		}
		AudioManager.inst.PlayCartoonEat(ni.transform.position,0.8f);
		a.heldNumber = ni;
		a.heldNumber.GetComponent<Collider>().enabled = false;
		a.heldNumber.GetComponent<Rigidbody>().isKinematic = true;
		a.moving = true;
		a.heldNumber.transform.position = a.inputStart.position;
		a.heldNumber.transform.parent = a.heldPosition;


	}

	void FixedUpdate(){

		foreach( XyzAxis a in axis){
			if (a.moving){
//				// commented Debug.Log("a moving");
				float lerpSpeed = 5f;
				a.heldNumber.transform.position = Vector3.Lerp(a.heldNumber.transform.position,a.inputFinal.position,Time.deltaTime* lerpSpeed);
				if (Vector3.Distance(a.heldNumber.transform.position,a.inputFinal.position) < 0.5f){
					moving = true;
					a.moving = false;
					float x = a.heldNumber.fraction.GetAsFloat();
					AudioManager.inst.PlayPlungerSuck(transform.position,Mathf.Log(x+1)+0.2f);
					a.heldNumber.transform.position = a.heldPosition.position;
					float gridScale = 5;
					float amount = a.heldNumber.fraction.GetAsFloat() * gridScale; // 5 is per line
					switch(a.axisType){
					case AxisType.X: targetPosition = new Vector3(amount,targetPosition.y,targetPosition.z); break;
					case AxisType.Y: targetPosition = new Vector3(targetPosition.x,amount,targetPosition.z); break;
					case AxisType.Z: targetPosition = new Vector3(targetPosition.x,targetPosition.y,amount); break;
					default:break;
					}
					// commented Debug.Log("targetpos;"+targetPosition);
				}
			}
		}
		if (moving) {
			float lerpSpeed = 5.5f;
			transform.position = Vector3.MoveTowards(transform.position,startPosition+targetPosition,Time.deltaTime * lerpSpeed);
			if (Vector3.Distance(transform.position,startPosition+targetPosition) < .1f){
				transform.position = startPosition+targetPosition;
				AudioManager.inst.PlayDoorLever(transform.position);
				moving = false;
			}
		}
	}

	public void ResetPosition(){
		targetPosition = Vector3.zero;
		moving = true;
		foreach( XyzAxis a in axis){
			if (a.heldNumber){
				Destroy(a.heldNumber.gameObject);
				a.moving = false;
				EffectsManager.inst.CreateShards(a.heldNumber.transform.position);
				AudioManager.inst.PlayNumberShatter(a.heldNumber.transform.position);
			}
		}
	}
}
