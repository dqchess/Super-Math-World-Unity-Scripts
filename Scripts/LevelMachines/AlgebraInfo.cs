using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum AlgebraBallType {
	X,
	Y,
	Z
}



public class AlgebraInfo : MonoBehaviour {

	public AlgebraBallType type = AlgebraBallType.X;
	public int exponent = 2;
	float orbScale = .3f;
	public NumberInfo ni;
	float orbRotateSpeed = 220f;
	float orbitRadius = 1.2f;
	public List<GameObject> exponentBalls = new List<GameObject>();

	public void SetExponent(int exp){
		exponent = exp;
		CreateBalls();
	}

	void Start(){
		ni = GetComponent<NumberInfo>();
		CreateBalls();

	}
	public void ClearBalls(){
		foreach(GameObject o in exponentBalls){
			Destroy(o);
		}
		exponentBalls.Clear();
//		// commented Debug.Log("destroyed exp balls");
	}
	void CreateBalls(){
//		// commented Debug.Log("creating exp balls");
		exponentBalls.Clear();
		for (int i=0;i<exponent;i++){
			exponentBalls.Add(CreateExponentBall());
//			// commented Debug.Log("added");
		}
	}

	public void OnPlayerCollect(){
		
//		foreach(GameObject o in exponentBalls){
//			Destroy(o);
//		}
//		exponentBalls.Clear();
//		// commented Debug.Log("destroyed exp balls");
	}

	void OnPlayerThrow(){
//		CreateBalls();
	}

	void FixedUpdate(){
		
		for (int i=0; i<exponent; i++){

		}
		int rotateIndex = 0;
		foreach(GameObject orb in exponentBalls){
			Vector3 dir = Vector3.zero;
			if (rotateIndex == 0) dir = Vector3.up;
			if (rotateIndex == 1) dir = Vector3.right;
			if (rotateIndex == 2) dir = Vector3.forward;
//				Vector3 dir = new Vector3((rotateIndex),1 + rotateIndex % 1,rotateIndex % 2);

			orb.transform.Rotate(dir * orbRotateSpeed*Time.deltaTime,Space.World);
			rotateIndex ++;
//			// commented Debug.Log("rotating!");
			// All the orbs are "pointing" in a way they will rotata.

			// So, it will have a 
		}
	}

	public GameObject CreateExponentBall(){
		GameObject pivot = new GameObject("orbitoer pivot");
		GameObject orb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		Destroy(orb.GetComponent<Collider>());
		orb.transform.localScale = orbScale * Vector3.one;
		orb.transform.parent = pivot.transform;
		orb.transform.localPosition = new Vector3(0,0,orbitRadius);
		orb.GetComponent<Renderer>().material.color = Color.red;
		orb.GetComponent<Renderer>().material.shader = Shader.Find("Toon/Lit Outline");
		pivot.transform.parent = transform;
		pivot.transform.localPosition = Vector3.zero;
		pivot.transform.localRotation = Quaternion.identity;
		return pivot;
	}
}
