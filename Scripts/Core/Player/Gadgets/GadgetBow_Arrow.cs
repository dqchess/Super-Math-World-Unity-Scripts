using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GadgetBow_Arrow : MonoBehaviour {

	NumberInfo ni0;
	RecordPosition rp;
	void Start(){
		ni0 = GetComponent<NumberInfo>(); // all arrows are numberinfo .. should probably inherit?
		rp = GetComponent<RecordPosition>();
	}

	void OnCollisionEnter(Collision hit){
		Collider other = hit.collider;
		NumberInfo ni = other.GetComponent<NumberInfo>();

		if (!ni){
			// hit the ground or something. Stick in the ground, become noninteractable (or can you still pick up?)
//			Debug.Log("hit;"+other.name);	
			GetComponent<Rigidbody>().isKinematic = true;
			transform.parent = GetComponent<Collider>().transform;
			TimedObjectDestructor tod = gameObject.AddComponent<TimedObjectDestructor>();
			tod.DestroyNow(2);
		} else if (ni.myShape == NumberShape.Sphere) {
			// Divide the number.
			int degreesToComplete = 360;
			float radius = 4f;
			float scale = radius/3.2f/3.2f;
			int count = Mathf.Abs(ni0.fraction.numerator);
			int startingDegrees = Mathf.FloorToInt(Player.inst.transform.rotation.eulerAngles.y+90);
			Vector3[] circlePoints = MathUtils.GetPointsOfACircle(degreesToComplete,radius,scale,count,startingDegrees); // why is there a 19 here?
			float splashForce = 340f; // after the arrow splits this number, how much force does each piece move away with?
			Fraction result = Fraction.Multiply(ni.fraction,ni0.fraction.GetReciprocal());
			for (int i=0;i<circlePoints.Length;i++){
				Vector3 pos = hit.contacts[0].point + circlePoints[i] + Vector3.up * 2;
				ni.SetNumber(result);
				ni.gameObject.AddComponent<BowSplitNumber>();
				ni.ForbidCombinationsForSeconds(5); // don't collide with nearby stuff right away.
				GameObject piece = (GameObject)Instantiate(ni.gameObject,pos,ni.transform.rotation); // NumberManager.inst.CreateNumber(result),pos,NumberShape.Sphere);
				Animal an = piece.GetComponent<Animal>();
				if (an){
					// in case we split a frog while it's tonguing.
					if (an.target != null){
						if (an.target.transform) 	Destroy(an.target.transform.gameObject);
					}
					an.LoseTarget();

				}
//				ScaleUpAndStop sc = piece.AddComponent<ScaleUpAndStop>();
//				sc.stopScale = piece.transform.localScale;
//				piece.transform.localScale = Vector3.one * 0.4f;

//				piece.GetComponent<NumberInfo>().InitSinGrowAttributes();
				Vector3 dir = Vector3.Normalize(pos - other.transform.position) + Vector3.up;
				Rigidbody rb = piece.GetComponent<Rigidbody>();
				if (!rb){
					rb = piece.AddComponent<Rigidbody>();
					rb.freezeRotation=true;
					rb.drag = 1f;
				}
				rb.mass = Mathf.Max(10,rb.mass);
				rb.drag = Mathf.Max(.05f,rb.drag);
				rb.AddForce(dir * splashForce);
			}
			NumberManager.inst.DestroyOrPool(ni);
			Destroy(gameObject);

		}
	}

	void Update(){
		Vector3 dir = rp.nowPosition-rp.lastPosition;
		if (Vector3.SqrMagnitude(dir) > 1){
			transform.forward = dir;
		}
	}
}
