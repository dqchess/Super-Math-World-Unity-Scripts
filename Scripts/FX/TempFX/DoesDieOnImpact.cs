using UnityEngine;
using System.Collections;

public class DoesDieOnImpact : MonoBehaviour {

	void OnCollisionEnter(Collision hit){
		ShrinkAndDisappear sad = gameObject.AddComponent<ShrinkAndDisappear>();	
	}
}
