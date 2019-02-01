using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInWhenPlayerClose : MonoBehaviour {

	// Use this for initialization
	Renderer r;
	void Start () {
		r = GetComponent<Renderer>();
	}

	public float transparentCutoff = 18f;
	public float opaqueCutoff = 4f;
	
	// Update is called once per frame
	void Update () {
		float playerDist = Vector3.Distance(Player.inst.transform.position,transform.position);
		float alpha = 0f;
		float lerpRange = transparentCutoff - opaqueCutoff;
		if (playerDist > transparentCutoff){
			alpha = 0f;
		} else if (playerDist > opaqueCutoff){
			alpha = ((-playerDist + lerpRange ) + opaqueCutoff)/ lerpRange;
		} else alpha = 1;
		r.material.color = new Color(r.material.color.r,r.material.color.g,r.material.color.b,alpha);
	}
}
