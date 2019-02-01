using UnityEngine;
using System.Collections;

public class EnergyBallHolder : MonoBehaviour {


//	GameObject currentBall;
//	float heldBallScale = 0.58f;
//	public Material glowMaterial;
//	void OnTriggerEnter(Collider other){
//		if (other.GetComponent<EnergyBall>() && other.GetComponent<Rigidbody>()){
//			
//			if (!currentBall){
//				Destroy(other.GetComponent<Rigidbody>());
//				currentBall = other.gameObject;
//				currentBall.transform.parent = transform;
////				GetComponent<Renderer>().material = glowMaterial;
//				currentBall.transform.localScale = Vector3.one * heldBallScale;
//				grabbing = true;
//				if (currentBall.GetComponent<PickUppableObject>()){
//					Destroy(currentBall.GetComponent<PickUppableObject>());
//				}
//			} else {
//				currentBall.GetComponent<NumberInfo>().Eat(other.GetComponent<NumberInfo>());
//			}		
//			UpdateCurrentEnergy(currentBall.GetComponent<NumberInfo>().fraction);
//		}
//	}
//
//	public void OnNumberChanged(){
////		// commented Debug.Log("on num changed");
//		if (currentBall){
//			UpdateCurrentEnergy(currentBall.GetComponent<NumberInfo>().fraction);
//		}
//	}
//
//	void UpdateCurrentEnergy(Fraction f){
//		Vehicle v = GetComponentInParent<Vehicle>();
//		if (v){
//			v.EnergyNumberCollected(currentBall.GetComponent<NumberInfo>()); //GiveEnergy(currentBall.GetComponent<NumberInfo>().fraction);
//		}
//		bool pos = f.numerator > 0 ? true : false;
//		if (pos) GetComponent<AudioSource>().Play();
//		else GetComponent<AudioSource>().Stop();
//		GetComponent<Rotate>().enabled = pos;
//		GetComponent<SinColor>().enabled = pos;
//	}
//
//	bool grabbing = false;
//	void Update(){
//		if (currentBall && grabbing){
//			float grabSpeed = 1f;
//			currentBall.transform.position = Vector3.Lerp(currentBall.transform.position,transform.position,Time.deltaTime * grabSpeed);
//			if (Vector3.SqrMagnitude(currentBall.transform.position - transform.position) < .1f){
//				currentBall.transform.position = transform.position;	
//				grabbing = false;
////				currentBall = null;
//			}
//		}
//	}
}
