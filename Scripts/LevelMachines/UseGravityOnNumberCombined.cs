using UnityEngine;
using System.Collections;

public class UseGravityOnNumberCombined : MonoBehaviour {


	// Use this for initialization
	NumberInfo ni;
	void Start () {
		ni = GetComponent<NumberInfo>();
		if (!ni) Destroy(this);
		else {
			ni.numberChanged += OnCombine;
		}
	}

	void OnCombine(Fraction f){
		if (GetComponent<Rigidbody>()){
			GetComponent<Rigidbody>().useGravity = true;
			if (ni) ni.numberChanged -= OnCombine;
			Destroy(this);
		}	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
