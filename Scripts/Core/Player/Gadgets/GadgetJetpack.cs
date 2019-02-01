using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GadgetJetpack : Gadget {
	public GameObject ok;
//	asdf;

	
//	public JetpackWeapon(GameObject player) : base(player) {
////		graphi = myPlayer.GetComponent<PlayerNumberController>().jetpackGraphics;
//		SetFire(false);
////		NumberGenerator ng = myPlayer.GetComponent<PlayerNumberController>().magnetGraphics.GetComponent<NumberGenerator>();
////		foreach (SwarmBehavior sb in ng.swarm){
////			sb.home = ng.transform;
//////			Debug.Log("sb: "+sb.name);
////		}
//
//	}
//
//	public override void OnSelect(){
//		GlobalVars.inst.pnc.jetpackGraphics.SetActive(true);
//		base.OnSelect();
//		SetFire(false);
//		myPlayer.GetComponent<PlayerNumberController>().weaponsAnims.DefaultStatic();
//
//	}
//
//	public void SetFire(bool flag){
//		ParticleEmitter[] pems = myPlayer.GetComponent<PlayerNumberController>().jetpackFire.GetComponentsInChildren<ParticleEmitter>();
//		foreach(ParticleEmitter pem in pems){
//			pem.emit=flag;
//		}
//	}
//
//	float jetpackTimer=.3f;
//
//	public override void OnDeselect(){
//		base.OnDeselect();
//		if (GlobalVars.inst.am.jetpackSound) GlobalVars.inst.am.jetpackSound.Stop();
//	}
//
//	bool flying=false;
//	public override void OnMouseHoldLeft() {
//		if (GlobalVars.inst.playerFrozen) return;
//		if (!GlobalVars.inst.am.jetpackSound.isPlaying) GlobalVars.inst.am.jetpackSound.Play();
//		if (jetpackTimer < 0){
//			if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
//		    && (GlobalVars.inst.gf.PurchaseForGems(10))){
//				jetpackTimer=.3f;
//				GlobalVars.inst.fpswe.JetpackShift();
//				SetFire(true);
//				flying=true;
//
//			} else if (GlobalVars.inst.gf.PurchaseForGems(5)){
//				jetpackTimer=.3f;
//				GlobalVars.inst.fpswe.Jetpack();
//				SetFire(true);
//				flying=true;
//			} else {
//				flying=false;
//				SetFire(false);
//				GlobalVars.inst.am.jetpackSound.Stop();
//			}
//		}
//		
//	}
//
//
//
//	public override void OnLeftMouseButtonUp(){
//		GlobalVars.inst.am.jetpackSound.Stop();
//		SetFire(false);
//	}
//
//	public override void WeaponUpdate(){
//		jetpackTimer-=Time.deltaTime;
//	}
//
//	public override void Fire(){
//		if (flying) SetFire(true);
//	}
}
