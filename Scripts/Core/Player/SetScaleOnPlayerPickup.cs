using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetScaleOnPlayerPickup : MonoBehaviour {

	public float scaleToBe = 2f;
	public void OnPlayerCollect(){
		transform.localScale = Vector3.one * scaleToBe;
	}
	public void OnPlayerThrow(){
		transform.localScale = Vector3.one * scaleToBe;
	}
}
