	using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeParticleSystemListener : MonoBehaviour {

	public Transform cubeToScaleWith;
	// Use this for initialization
	public float scaleFactor = 10f;
	ParticleSystem ps;
	void Start () {
		ps = GetComponent<ParticleSystem>();
	}


	// Update is called once per frame
	void Update () {
		if (Utils.IntervalElapsed(5f)){
			ResizeParticleSystemBasedOnScale();
		}	
	}

	void ResizeParticleSystemBasedOnScale(){
		float dim = cubeToScaleWith.transform.lossyScale.x;
//		ParticleSystem.ShapeModule pss = ps.shape;
//		pss.box = new Vector3();
//		ParticleSystem.EmissionModule mod = new ParticleSystem.EmissionModule();
		float newRate = Mathf.Pow((dim/10f),2)*scaleFactor;
		ps.emissionRate = newRate;
//		ps.emission.rate = Mathf.Pow((dim/10f),2)*10f;
	}
}
