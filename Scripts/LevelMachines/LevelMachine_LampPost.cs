using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelMachine_LampPost : MonoBehaviour {

	public Fraction frac;

	void Start(){
		GetComponentInChildren<NumberInfo>().SetNumber(frac);
	}

}
