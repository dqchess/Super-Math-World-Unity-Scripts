using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterAISpikey1 : MonsterAIBase
{

	
	public bool preparingToJump = false;
	public float jumpWaitTimer = 1.0f;
	public float jumpCooldown = 0.0f;
	
	Color origBase;
	Color origVignette;
	public Color chargedBase = new Color(1, 1, 0, .25f);
	public Color chargedVignette = new Color(1, 1, 1, .1f);
	Material graphics;

	public override void Start ()
	{
		base.Start();

	
		graphics = transform.Find("mesh").GetComponent<Renderer>().material;

	}


	void ReturningToPool() {
		Destroy (this);
	}
	
	void JumpAtPlayer(){

	}


	float jumpTimer = 1f;



	float spikeySound = 0;
	public override void MonsterUpdate ()
	{

		base.MonsterUpdate ();

		if (LevelBuilder.inst.levelBuilderIsShowing) return;
		if (ignoringPlayer){
			ignorePlayerSeconds -= Time.deltaTime;
			if (ignorePlayerSeconds < 0){
				ignoringPlayer = false;
			}
		}
		spikeySound -= Time.deltaTime;

		if (spikeySound < 0){
			spikeySound = Random.Range(4f,12f);
			AudioManager.inst.PlaySpikeySound(transform.position);
		}
		stopped = false;
	}
	
	void RestoreColor() {
//		graphics.SetColor("_BaseColor", origBase);
//		graphics.SetColor("_VignetteColor", origVignette);
	}
	
//	public override void Idle ()
//	{
//		base.Idle ();
//		stopped = false;
//		jumpWaitTimer = 1;
//		preparingToJump = false;
//		RestoreColor();
//	}

	public override void OnCollisionEnter(Collision hit){
		
		MonsterAISpikey1 sp = hit.collider.GetComponent<MonsterAISpikey1>();
		if (sp){
			//Ignore other spikeys of the same sign. 
			if (sp.fraction.numerator * fraction.numerator < 0){
				base.OnTouchedSomething(hit.collider); 
			} else {
				return;
			}
		}
		if (hit.collider.gameObject.tag == "Player"){
			if (GetComponentInParent<SpikeyGroup>()) GetComponentInParent<SpikeyGroup>().MovePlayerBack();
		}
	}
		
	float ignorePlayerSeconds = 0;
	public void IgnorePlayerForSeconds(float s){
		Debug.Log("ignore;"+s);
		ignoringPlayer = true;
	}

	public override void OnDestroy(){
		base.OnDestroy();
		if (!muteDestroy){
			AudioManager.inst.PlaySpikeySound(transform.position,.6f);
		}
	}
}

