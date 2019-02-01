using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class EffectsManager : MonoBehaviour {


	public static EffectsManager inst;

	public Material[] coloredMaterials;
	public Material[] naturalMaterials;
	public Material[] spongeMaterials;

	public Text fadeLetter;
	public GameObject arenaParticles;
	public GameObject arenaSmoke;
	public GameObject blueCircles;
	public GameObject blueRing;

	ParticleSystem blueRingInstance;
	public GameObject blueFlash;
	ParticleEmitter blueFlashInstance;
	public GameObject blueStars;
	public GameObject blueStarsLite;
	public GameObject bounceParticles;
	ParticleSystem bounceParticlesInstance;
	public GameObject bubbles;
	ParticleSystem bubblesInstance;
	public GameObject cloudParticles;
	ParticleSystem cloudParticlesInstance;
	public GameObject dust;
	ParticleSystem dustInstance;
	public GameObject dragonFireNumber;
	public GameObject explosionBigPurple;
	ParticleEmitter explosionBigPurpleInstance;
	public GameObject explosionSmallPurple;
	ParticleEmitter explosionSmallPurpleInstance;
	public GameObject flame;
	ParticleEmitter flameInstance;
	public GameObject flyingCheckmark;
	public GameObject flyingCancel;
	public GameObject fireworks1;
	public GameObject fireworks2;
	public GameObject justAteFX;
	public GameObject gemx1;
	public GameObject gemx10;
	public GameObject gemx50;
	public GameObject gemx2000;
	public GameObject heatmapRing;
	public GameObject heatmapTrail;
	public GameObject lightningBall;
	ParticleSystem lightningBallInstance;
	public GameObject lightningPrefab;
	public GameObject lightningPrefabRed;
	public GameObject littleBlueExplosion;
	public GameObject multiplyTrail;
	public GameObject multiplyParticle;
	public GameObject mathparticles;
	ParticleEmitter mathparticlesinstance;
	public GameObject mediumExplosion;
	public GameObject negativeRain;
	public GameObject plusSign;
	ParticleEmitter plusSignInstance;
	public GameObject plusTrail1;
	public GameObject radiatingRays;
	public GameObject ringEffect;
	ParticleEmitter ringEffectInstance;
	public GameObject rocketAOE;
	public GameObject smokeTrail;
	public GameObject sparkleFall1;
	ParticleEmitter sparkleFall1Instance;
	public GameObject sparkleFall2;
	ParticleEmitter sparkleFall2Instance;
	public GameObject sparkles3;
	ParticleEmitter sparkles3Instance;
	public GameObject spikeyTrail;
	public GameObject spikesFall;
	ParticleEmitter spikesFallInstance;
	public GameObject smokePuff;
	ParticleSystem smokePuffInstance;
	public GameObject revertSparks;
	ParticleSystem revertSparksInstance;
	public GameObject waterSplash;
	public GameObject ringEffect2;
	ParticleEmitter ringEffectInstance2;
	public GameObject sparkle;
	ParticleEmitter sparkleInstance;
	public GameObject shards;
	ParticleEmitter shardsInstance;
	public GameObject whiteSmoke;
	ParticleEmitter whiteSmokeInstance;
	public GameObject whiteFlash;
	ParticleEmitter whiteFlashInstance;
	public GameObject zeroEffect;

	int textEffectPoolCur = 0;
	GameObject[] textEffectPool;
	// Use this for initialization

	public void SetInstance(){
		
		inst = this;
	}

	public void MakeLittleBlueExplosion(Vector3 pos){
		GameObject exp = (GameObject)Instantiate(littleBlueExplosion,pos,Quaternion.identity);
		TimedObjectDestructor tod = exp.AddComponent<TimedObjectDestructor>();
		tod.DestroyNow(2);
	}


	void Start () {

		
		textEffectPool = new GameObject[10];
		for(int i = 0; i < textEffectPool.Length; i++) {
			GameObject risingParent = new GameObject();
//			risingParent.layer=LayerMask.NameToLayer("GUI");
			risingParent.name="risingparent";
			risingParent.transform.parent = transform; // TODO: remove from final build. This is to reduce scene clutter duing playtime
			risingParent.transform.position = new Vector3(0,0,0);
			//MoveAlongAxis maa = risingParent.AddComponent<MoveAlongAxis>();
			//maa.dir = Vector3.up;
			//maa.speed = 3f;
			
			SetupParentedText(risingParent,"",Color.black);
//			risingParent.GetComponentInChildren<Renderer>().material.shader = Shader.Find ("Catlike Coding/Text Box/Alpha Blend Cycle Color");
			textEffectPool[i] = risingParent;
			//followText.renderer.enabled = false;
		}
		
		
		Vector3 particlesLocation = new Vector3(10000,-10000,10000);
		
		GameObject instt = (GameObject)Instantiate(explosionSmallPurple,particlesLocation,Quaternion.identity);
		explosionSmallPurpleInstance = instt.GetComponent<ParticleEmitter>();
		explosionSmallPurpleInstance.emit=false;
		instt.transform.parent = transform;
		
		instt = (GameObject)Instantiate(ringEffect2,particlesLocation,Quaternion.identity);
		ringEffectInstance2 = instt.GetComponent<ParticleEmitter>();
		ringEffectInstance2.emit=false;
		instt.transform.parent = transform;
		
		instt = (GameObject)Instantiate(explosionBigPurple,particlesLocation,Quaternion.identity);
		explosionBigPurpleInstance = instt.GetComponent<ParticleEmitter>();
		explosionBigPurpleInstance.emit=false;
		instt.transform.parent = transform;
		
		instt = (GameObject)Instantiate(smokePuff,particlesLocation,Quaternion.identity);
		smokePuffInstance = instt.GetComponent<ParticleSystem>();
		smokePuffInstance.emissionRate=0;
		instt.transform.parent = transform;
		
		instt = (GameObject)Instantiate(sparkle,particlesLocation,Quaternion.identity);
		sparkleInstance = instt.GetComponent<ParticleEmitter>();
		sparkleInstance.emit=false;
		instt.transform.parent = transform;
		
		// This one has a WorldParticleCollider on it, does this work with Emit()?
		instt = (GameObject)Instantiate(shards,particlesLocation,Quaternion.identity);
		shardsInstance = instt.GetComponent<ParticleEmitter>();
		shardsInstance.emit=false;
		instt.transform.parent = transform;
		
		instt = (GameObject)Instantiate(plusSign,particlesLocation,Quaternion.identity);
		plusSignInstance = instt.GetComponent<ParticleEmitter>();
		plusSignInstance.emit=false;
		instt.transform.parent = transform;
		
		instt = (GameObject)Instantiate(sparkleFall1,particlesLocation,Quaternion.identity);
		sparkleFall1Instance=instt.GetComponent<ParticleEmitter>();
		sparkleFall1Instance.emit=false;
		instt.transform.parent = transform;
		
		instt = (GameObject)Instantiate(sparkleFall2,particlesLocation,Quaternion.identity);
		sparkleFall2Instance=instt.GetComponent<ParticleEmitter>();
		sparkleFall2Instance.emit=false;
		instt.transform.parent = transform;
		
		instt = (GameObject)Instantiate(sparkles3,particlesLocation,Quaternion.identity);
		sparkles3Instance=instt.GetComponent<ParticleEmitter>();
		sparkles3Instance.emit=false;
		instt.transform.parent = transform;
		
		//		instt = (GameObject)Instantiate(plusTrail1,particlesLocation,Quaternion.identity);
		//		plusTrail1Instance=instt.GetComponent<ParticleEmitter>();
		
		instt = (GameObject)Instantiate(spikesFall,particlesLocation,Quaternion.identity);
		spikesFallInstance=instt.GetComponent<ParticleEmitter>();
		spikesFallInstance.emit=false;
		instt.transform.parent = transform;
		
		instt = (GameObject)Instantiate(whiteFlash,particlesLocation,Quaternion.identity);
		whiteFlashInstance=instt.GetComponent<ParticleEmitter>();
		whiteFlashInstance.emit=false;
		instt.transform.parent = transform;

		instt = (GameObject)Instantiate(blueFlash,particlesLocation,Quaternion.identity);
		blueFlashInstance=instt.GetComponent<ParticleEmitter>();
		blueFlashInstance.emit=false;
		instt.transform.parent = transform;
		
		instt = (GameObject)Instantiate(ringEffect,particlesLocation,Quaternion.identity);
		ringEffectInstance=instt.GetComponent<ParticleEmitter>();
		ringEffectInstance.emit=false;
		instt.transform.parent = transform;

		instt = (GameObject)Instantiate(flame,particlesLocation,Quaternion.identity);
		flameInstance=instt.GetComponent<ParticleEmitter>();
		flameInstance.emit=false;
		instt.transform.parent = transform;

		instt = (GameObject)Instantiate(lightningBall,particlesLocation,Quaternion.identity);
		lightningBallInstance=instt.GetComponent<ParticleSystem>();
		lightningBallInstance.emissionRate=0;
		instt.transform.parent = transform;


		instt = (GameObject)Instantiate(revertSparks,particlesLocation,Quaternion.identity);
		revertSparksInstance=instt.GetComponent<ParticleSystem>();
		revertSparksInstance.emissionRate=0;
		instt.transform.parent = transform;

		instt = (GameObject)Instantiate(blueRing,particlesLocation,Quaternion.identity);
		blueRingInstance=instt.GetComponent<ParticleSystem>();
		blueRingInstance.emissionRate=0;
		instt.transform.parent = transform;
		
		instt = (GameObject)Instantiate(whiteSmoke,particlesLocation,Quaternion.identity);
		whiteSmokeInstance=instt.GetComponent<ParticleEmitter>();
		whiteSmokeInstance.emit=false;
		instt.transform.parent = transform;
		instt = (GameObject)Instantiate(bubbles,particlesLocation,Quaternion.identity);
		bubblesInstance=instt.GetComponent<ParticleSystem>();
		bubblesInstance.emissionRate = 0;
		instt.transform.parent = transform;
		instt = (GameObject)Instantiate(dust,particlesLocation,Quaternion.identity);
		dustInstance=instt.GetComponent<ParticleSystem>();
		dustInstance.emissionRate = 0;
		instt.transform.parent = transform;
		instt = (GameObject)Instantiate (bounceParticles,particlesLocation,Quaternion.identity);
		instt.name ="binst";
		bounceParticlesInstance = instt.GetComponent<ParticleSystem>();
		bounceParticlesInstance.emissionRate = 0;
		instt.transform.parent = transform;

//		instt = (GameObject)Instantiate (cloudParticles,particlesLocation,Quaternion.identity);
//		cloudParticlesInstance = instt.GetComponent<ParticleSystem>();
//		cloudParticlesInstance.emissionRate = 0;
//
		////		instt = (GameObject)Instantiate(mathparticles,particlesLocation,Quaternion.identity);
		//		mathparticlesinstance=instt.GetComponent<ParticleEmitter>();
		//		mathparticlesinstance.emit=false;
		
		
	}

	public GameObject SetupParentedText(GameObject objToFollow, string textToCreate, Color color){
		GameObject textFollow = new GameObject("textfollow");
//		textFollow.name = "textFollow";
		GameObject textFollowParent = new GameObject("textfollowparent"); // needed to flip the child 180° abuot y axis so CCText is visible when facing away from player 
		textFollow.transform.parent = textFollowParent.transform;
		textFollowParent.transform.parent =transform;
//		textFollowParent.name = textToCreate;
//		
//		
//		// double parenting to make rotation always face player
//		textFollow.transform.parent=textFollowParent.transform;
//		textFollowParent.transform.parent = objToFollow.transform;
//		
//		textFollow.transform.localPosition=Vector3.zero;
//		textFollow.transform.localScale *= 1.2f;
//		textFollow.transform.eulerAngles = new Vector3(0,180,0);
//		textFollowParent.transform.position = objToFollow.transform.position+ new Vector3(0,1.5f,0); // position it just above the number it marks
//		textFollowParent.AddComponent<SometimesFacePlayer>();
//		textFollowParent.transform.LookAt(Camera.main.transform.position);
//		textFollowParent.AddComponent<EquationEffects>();
//		CCText cct = textFollow.AddComponent<CCText>();
////		cct = SetupText(cct,GlobalVars.instt.standardCCFont);
//		cct.name="textobj";
//		
//		cct.Text = textToCreate;
//		cct.Color = color;
////		textFollow.GetComponent<Renderer>().material = GlobalVars.instt.standardFontMaterial;
		
		return textFollowParent;
	}
	
	public GameObject CreateTextEffect(Vector3 effectPos,string effectText, float scale=1.2f) {
		
//		textEffectPoolCur = (textEffectPoolCur + 1) % textEffectPool.Length;
//		if (textEffectPool[textEffectPoolCur] == null) { return null; }
//		textEffectPool[textEffectPoolCur].transform.position = effectPos;
//		CCText t = textEffectPool[textEffectPoolCur].GetComponentInChildren<CCText>();
//		
//		t.Text = effectText;
//		
//		textEffectPool[textEffectPoolCur].GetComponentInChildren<EquationEffects>().Reset();
//		textEffectPool[textEffectPoolCur].name = "fx!!";
//		textEffectPool[textEffectPoolCur].transform.localScale = Vector3.one*scale;
//		StartCoroutine (HideTextEffect(textEffectPool[textEffectPoolCur]));
		return textEffectPool[textEffectPoolCur];
		
	}
	IEnumerator HideTextEffect(GameObject o){
		yield return new WaitForSeconds(3f);
		if (o) o.transform.position = Vector3.zero;
		else {}  // commented Debug.Log ("onamel"+o.name);
	}

	
	// Update is called once per frame
	void Update () {
		if (fadingLetter){
			float fadeSpeed = 3;
			fadeLetter.transform.localScale -= Vector3.one * fadeSpeed * Time.deltaTime;
			if (fadeLetter.transform.localScale.x < .1f){
				fadingLetter = false;
				fadeLetter.gameObject.SetActive(false);
			}
		}


		//newline for asset server debug june 20
	}
	
	//	[RPC]
	//	public void MakeRocketExplosionRPC(Vector3 pos){
	//		RocketExplosionEffect.MakeRocketExplosion(new Fraction(0,1), pos,15);
	//		GlobalVars.instt.am.PlayExplosion1(pos);	
	//	}
	
	public void GeneratePlusSign(Vector3 pos){
		plusSignInstance.Emit(pos,new Vector3(0,5,0),2,.2f,Color.white);
	}
	
	public void GenerateRingEffect(Vector3 pos, float size, float energy, Color color){
		StartCoroutine(GenerateRingEffectE(pos, size, energy, color));
		
	}
	
	IEnumerator GenerateRingEffectE(Vector3 pos, float size, float energy, Color color){
		for (int i=0;i<3;i++){
			yield return new WaitForSeconds(.07f);
			ringEffectInstance.Emit(pos,Vector3.zero,size,energy,color);
		}
	}
	// oh yeah i added this.
	public void CreateSmallPurpleExplosion(Vector3 pos,float size,float energy){
		explosionSmallPurpleInstance.Emit(pos,Vector3.zero,7,.2f,Color.white);
		for (int i=0;i<7;i++){
			
			//			Vector3 vel = new Vector3(Random.Range(-10f,10f),Random.Range(-1f,1f),Random.Range(-10f,10f));
			//			size += Random.Range(-.1f,.1f)
			//					
			//					energy
			//					color
			//
			//			sparkleFall1Instance.Emit(pos,
			//			sparkleFall1Instance.Emit(pos,vel,size,1.5f+Random.Range(-.4f,.4f),new Color(1,0,1));
			//			sparkleFall2Instance.Emit(pos,new Vector3(Random.Range(-10f,10f),2+Random.Range(-1f,1f),Random.Range(-10f,10f)),size+Random.Range(-.2f,.2f),energy+Random.Range(-.4f,.4f),new Color(1,0,1));
			
		}
	}
	
	//	[RPC]
	//	public void CreateSmallPurpleExplosionRPC(Vector3 pos, float size, float energy) {
	//		CreateSmallPurpleExplosion(pos, size, energy);	
	//	}
	//	
	//	[RPC]
	//	public void CreateShardsRPC(Vector3 pos) {
	//		CreateShards(pos);	
	//	}	
	
	public void CreateSomeStars(Vector3 pos){
		CreateSomeStars(pos,1,.6f); // default size to 1, energy to .6f
		
	}
	
	public void CreateSomeStars(Vector3 pos, float size, float energy){
		for (int i=0;i<7;i++){
			
			sparkleFall1Instance.Emit(
				pos,
				new Vector3(Random.Range(-10f,10f),Random.Range(4f,10f),Random.Range(-10f,10f)),
				size*Random.Range(0.8f,1.2f),
				1.5f+Random.Range(-.4f,.4f),
				new Color(1,0,1));
			
			sparkleFall2Instance.Emit(
				pos,
				new Vector3(Random.Range(-10f,10f),2+Random.Range(-1f,1f),Random.Range(-10f,10f)),
				size*Random.Range(0.8f,1.2f),
				energy+Random.Range(-.4f,.4f),
				new Color(1,0,1));
		}
		
	}
	
	public void CreateSingleSlowSparkle(Vector3 pos){
		sparkles3Instance.Emit(pos,Vector3.zero,Random.Range(1.5f,2f),Random.Range(1f,2.5f),Color.white);
	}
	
	public void SpikesFall(Vector3 pos){
		for (int i=0;i<7;i++){
			float size=Random.Range(1f,2f);
			float energy=Random.Range(2f,3f);
			if (!spikesFallInstance) return;
			spikesFallInstance.Emit(pos,
			                        new Vector3(
				Random.Range(-10f,10f),
				Random.Range(4f,10f),
				Random.Range(-10f,10f)),
			                        
			                        size+Random.Range(-.1f,.1f), // 
			                        energy+Random.Range(-.4f,.4f),
			                        new Color(0,0,0),
			                        Random.Range(0,360), // random rotation to start
			                        0 // angular velocity
			                        );
		}
	}
	
	public void CreateShards(Vector3 pos){
		int max = Random.Range(15,20);
		for (int i=0;i<max;i++){
			float force = 7f;
			float yForce = 5;
			Vector3 vel = new Vector3(Random.Range(-force,force),Random.Range(-force,force)+yForce,Random.Range(-force,force));
			
			float size = Random.Range(0.4f,0.9f);
			float energy = Random.Range(.6f,1.5f);
			Color color = Color.white;//new Color(Random.Range(0f,1f),Random.Range(0f,1f),Random.Range(0f,1f));
			if (Random.Range (0,2) == 1) color = Color.black;
			
			int rot = Random.Range(-180,180);
			int angVel = Random.Range(-10,10);
			shardsInstance.Emit(pos,vel,size,energy,color,rot,angVel);
			
		}
	}
	
	// oh yeah i added this.
	public void CreateBigPurpleExplosion(Vector3 pos,float size,float energy){
		explosionBigPurpleInstance.Emit(pos,Vector3.zero,size,energy,Color.white);
	}
	
	public void CreateSmokePuff(Vector3 pos, Vector3 dir){
		smokePuffInstance.Emit(pos,dir,0.7f,0.5f,Color.white);
	}
	
	
	public void CreateWhiteFlash(Vector3 pos, float size, float duration){
		whiteFlashInstance.Emit(pos,Vector3.zero,size,duration,Color.white);
	}
	
	public void CreateBlueFlash(Vector3 pos, float size, float duration){
		blueFlashInstance.Emit(pos,Vector3.zero,size,duration,Color.blue);
	}
	
	public void CreateRingEffect2(Vector3 pos, float size, float duration,Color color){
		ringEffectInstance2.Emit(pos,Vector3.zero,size,duration,color);
	}
	
	public void CreateSmokePuffBig(Vector3 pos, Vector3 dir, float size=6, float energy=1.5f){
//		ParticleSystem.Particle
//		ParticleSystem.Particle p = new ParticleSystem.Particle();
		ParticleSystem.EmitParams p = new ParticleSystem.EmitParams();
		p.position = pos;
		p.velocity = dir;
		p.startSize = size;
		p.startLifetime = energy;
		smokePuffInstance.Emit(p,1);
	}

	public void BlueRing(Vector3 pos, Vector3 dir, float size=30, float energy=3.5f){
		//		ParticleSystem.Particle
		//		ParticleSystem.Particle p = new ParticleSystem.Particle();
		ParticleSystem.EmitParams p = new ParticleSystem.EmitParams();
		p.position = pos;
		p.velocity = dir;
		p.startSize = size;
		p.startLifetime = energy;
		blueRingInstance.Emit(p,1);
	}

	public void EmitLightningBall(Vector3 pos){
//		ParticleSystem.EmitParams p = new ParticleSystem.EmitParams();
//		p.position = pos;
//		p.startSize = size;
//		p.startLifetime = energy;
		lightningBallInstance.transform.position = pos;
		lightningBallInstance.Emit(20);
	}

	public void CreateMathParticles(Vector3 pos,float size,float energy){
		mathparticlesinstance.Emit(pos,Vector3.zero,7,.2f,Color.white);
		//		for (int i=0;i<7;i++){
	}
	
	public void CreateSingleRandomSparkle(Vector3 pos){
		//		while (!high){
		//			int rand = Random.Range (2,5);
		//			Weeds.SmokePuffs(rand);
		//		}
		return;
		// Randomize each sparkle!
		float force = 1.1f;
		float yForce = 1;
		Vector3 vel = new Vector3(Random.Range(-force,force),Random.Range(-force,force)+yForce,Random.Range(-force,force));
		
		float size = Random.Range(1.5f,2.5f);
		float energy = Random.Range(.6f,1.5f);
		Color color = new Color(Random.Range(0f,1f),Random.Range(0f,1f),Random.Range(0f,1f));
		
		int rot = Random.Range(15,80);
		int angVel = Random.Range(15,80);
		sparkleInstance.Emit(pos,vel,size,energy,color,rot,angVel);
	}
	
	
	public void CreateBigPurpleExplosion(Vector3 pos){
		//		explosionBigPurpleInstance.Emit( Emit(pos,Vector3.zero,1.5f,.3f,Color.white);
	}
	
	public void ArenaParticles(Vector3 pos){
		Instantiate (arenaParticles,pos,Quaternion.identity);
	}
	public void ArenaSmoke(Vector3 pos){
		Instantiate (arenaSmoke,pos,Quaternion.identity);
	}
	
	public void CreateFlame(Vector3 pos ,float size=8, float energy = 1){
		// commented Debug.Log ("FLAME");
		Vector3 vel = Vector3.zero;
		flameInstance.Emit (pos,vel,size,energy,Color.yellow);
		
	}
	
	public void WhiteSmoke(Vector3 pos, int amount){
		
	}
	
	public void EmitParticlesForBounce(Vector3 pos, Vector3 speed, float size, float lifetime, Color color){
		float spread = 1;
		Vector3 randXZ = new Vector3(Random.Range (-spread,spread),0,Random.Range (-spread,spread));
		pos += randXZ;
		bounceParticlesInstance.Emit(pos,speed,size,lifetime,color);
		//		// commented Debug.Log ("no particles here?"+pos);
	}
	
	public void CreateZeroEffect(Vector3 pos){
		Instantiate(zeroEffect,pos,Quaternion.identity);
	}
	
	public void Bubbles(Vector3 pos){
		Color color = Color.white;
		int r = Random.Range (3,7);
		Vector3 vel = Vector3.zero;
		for (int i=0;i<r;i++){
			float life = Random.Range (2,3f);
			float size = Random.Range (2,3f);
			bubblesInstance.Emit(pos,vel,size,life,color);
		}
	}

	public void MakeDust(Vector3 pos, float size){
		Color color = Color.white;
		Vector3 vel = Vector3.up;
		float life = Random.Range (2,3f);
//		size = Random.Range (size * 0.66f, size * 1.5f);
		dustInstance.Emit(pos,vel,size,life,color);
	}

	public void MakeCloudParticle(Vector3 pos, float size, float life){
		ParticleSystem.EmitParams p = new ParticleSystem.EmitParams();
		p.position = pos;
		p.velocity = Vector3.zero;
		p.startSize = size;
		p.startLifetime = life;
		cloudParticlesInstance.Emit(p,1);
	}

	public GameObject JustAteFX(Vector3 p, NumberInfo ni){
		return null;
		GameObject ret = (GameObject)Instantiate(justAteFX);
		ret.transform.position = p;
		if (ni.icon) {
			ret.GetComponentInChildren<JustAteFX>().justAteIcon.material.mainTexture = ni.icon.texture;
		} 
		ret.GetComponentInChildren<JustAteFX>().numberSphereIcon.material.mainTexture = ni.fraction.GetAsFloat() > 0 ? NumberManager.inst.positiveSphereIcon : NumberManager.inst.negativeSphereIcon;
		ret.GetComponentInChildren<CCText>().Text = ni.fraction.ToString();
		switch(ni.fraction.ToString().Length){
		case 0: break;
		case 1: ret.GetComponentInChildren<CCText>().transform.localScale = Vector3.one * 0.75f; break;
		case 2: ret.GetComponentInChildren<CCText>().transform.localScale = Vector3.one * 0.55f; break;
		case 3: ret.GetComponentInChildren<CCText>().transform.localScale = Vector3.one * 0.45f; break;
//		case 4: 
		default: ret.GetComponentInChildren<CCText>().transform.localScale = Vector3.one * 0.4f; break;
		}
		return ret;

	}

	public void RevertSparks(Vector3 pos, float scale){
		int max = Random.Range(15,20);
		for (int i=0;i<max;i++){
			float force = scale * 2.5f;
			float yForce = 7f;
			Vector3 vel = new Vector3(Random.Range(-force,force),Random.Range(-force,force)+yForce,Random.Range(-force,force));
			float size = Random.Range(.1f,.6f) * scale/3f;
			float energy = Random.Range(1f,1.5f);
			Color color = Color.white;
			switch(Random.Range(0,7)){
			case 0: color = Color.red; break;
			case 1: color = new Color(1,.5f,0); break;
			case 2: color = Color.yellow; break;
			case 3: color = Color.green; break;
			case 4: color = Color.blue; break;
			case 5: color = new Color(0.5f,0,1); break;
			case 6: color = new Color(1,0,1); break;
			default: break;
			}
			int rot = Random.Range(-180,180);
			int angVel = Random.Range(-10,10);
			Vector3 p = pos + Random.insideUnitSphere * scale / 5f;
			//			Debug.Log("spark at:"+p+",vel:"+vel+",size;"+size+",energy:"+energy+",color;"+color);
			ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
			emitParams.startSize = size;
			emitParams.velocity = vel;
			emitParams.position = p;
			emitParams.startLifetime = energy;
			emitParams.startColor = color;
//			emitParams.
			revertSparksInstance.Emit(emitParams,1);
		}
//		Debug.Log("rev sparks");
	}

	bool fadingLetter = false;
	public void FadeLetter(string s){
		fadeLetter.gameObject.SetActive(true);
		fadeLetter.text = s;
		fadingLetter = true;
		fadeLetter.transform.localScale = Vector3.one * 2.5f;
	}



	public void GemDropX1(Vector3 p){
		GameObject gem = (GameObject)Instantiate(gemx1,p+MathUtils.RandomInsideHalfSphere*.1f,Quaternion.identity);
		gem.AddComponent<TimedObjectDestructorPlayerDistance>();
		GemFX(gem);
	}
	public void GemDropX10(Vector3 p){
		GameObject gem = (GameObject)Instantiate(gemx10,p+MathUtils.RandomInsideHalfSphere*.1f,Quaternion.identity);
		gem.AddComponent<TimedObjectDestructorPlayerDistance>();
		GemFX(gem);
	}
	public void GemDropX50(Vector3 p){
		GameObject gem = (GameObject)Instantiate(gemx50,p+MathUtils.RandomInsideHalfSphere*.1f,Quaternion.identity);
		gem.AddComponent<TimedObjectDestructorPlayerDistance>();
		GemFX(gem);
	}
	public void GemDropX2000(Vector3 p){
		GameObject gem = (GameObject)Instantiate(gemx2000,p+MathUtils.RandomInsideHalfSphere*.1f,Quaternion.identity);
		gem.AddComponent<TimedObjectDestructorPlayerDistance>();
		GemFX(gem);
	}


	void GemFX(GameObject gem){
		gem.GetComponent<Rigidbody>().useGravity = true;
		gem.GetComponent<Rigidbody>().isKinematic = false;
		float gemFlyForce = Random.Range(25f,150f);
		float gemUpForce = 100f;
		gem.GetComponent<Rigidbody>().AddForce(MathUtils.RandomInsideHalfSphere * gemFlyForce + Vector3.up * gemUpForce);
	}

	bool CanDropGems(){
		return !(LevelBuilder.inst.levelBuilderIsShowing || JsonLevelLoader.inst.levelIsBeingLoaded);
	}
	public void DropGemsProbability(Vector3 p, int val, float probability){
//		Debug.Log("dropping:");
		if (CanDropGems()){
//			Debug.Log("can");
			if (Random.value <= probability){
//				Debug.Log("val");
//				Debug.Log("prob");
				DropGemCombo(val,p);

			}
		}
	}

	public void DropGemCombo(int singles, Vector3 p){
		int twothousands = Mathf.FloorToInt(singles / 2000f);
		singles %= 2000;
		int fifties = Mathf.FloorToInt(singles / 50f);
		singles %= 50;
		int tens = Mathf.FloorToInt(singles / 10f);
		singles %= 10;
//		Debug.Log("s, t,f:"+singles+","+tens+","+fifties);
		for(var i=0;i<singles;i++){
//			Debug.Log("singles:"+i+"at:"+p);
			EffectsManager.inst.GemDropX1(p);
		}
		for(var i=0;i<tens;i++){
			EffectsManager.inst.GemDropX10(p);
		}
		for(var i=0;i<fifties;i++){
			EffectsManager.inst.GemDropX50(p);
		}
		for(var i=0;i<twothousands;i++){
			EffectsManager.inst.GemDropX2000(p);
		}
	}

	public GameObject WaterSplash(Vector3 p){
		GameObject ret = (GameObject)Instantiate(EffectsManager.inst.waterSplash,p,Quaternion.identity);
		return ret;
	}

	public void DrawDottedLine(Vector3 origin, Vector3 dest, Color c){
		GameObject mover = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		mover.transform.position = origin;
		mover.transform.localScale = Vector3.one * .3f;
		mover.GetComponent<Renderer>().material.color = c;
		Destroy(mover.GetComponent<Collider>());
		SMW_GF.inst.MoveObjTo(mover,dest,5f,true);
	}



	public static Dictionary<GameObject,Vector3> NumberGrid (int factor1, int factor2,Transform anchor, float scale){
		GameObject combineParent = new GameObject();
		combineParent.transform.position = anchor.position;
		combineParent.transform.LookAt(Player.inst.transform);
		combineParent.transform.parent = anchor;
		Dictionary<GameObject,Vector3> poss = new Dictionary<GameObject,Vector3>();

		for (int i=0;i<factor1;i++){
			for (int j=0;j<factor2;j++){
				GameObject one = NumberManager.inst.CreateNumberAmmo(new Fraction(1,1));
				one.transform.position = anchor.position;
				one.transform.parent = combineParent.transform;
				Vector3 newPos = new Vector3((i-(i/2f))*scale,(j-(j/2f))*scale,0);
				//				toCombine.Add (one);
				poss.Add (one,newPos);
			}
		}
		return poss;
	}


	public Material squareMat;

	public void StartAnimationSequence (NumberInfo ni){
		StartCoroutine(StartAnimationSequenceE(ni));
	}
	IEnumerator StartAnimationSequenceE (NumberInfo ni){

		yield return new WaitForSeconds(0.01f);

//		ni.gameObject.SetActive(false);
		ni.enabled = false;
		ni.childMeshRenderer.gameObject.SetActive(false);

		int count = Mathf.RoundToInt (ni.fraction.GetAsFloat());

		List<Fraction> factors = ni.fraction.GetFactors();

		int factor1=0;
		int factor2=0;
		bool square = false;
		float result = Mathf.Sqrt(ni.fraction.GetAsFloat());
		//		// commented Debug.Log ("result:" + result);
		if (Mathf.Round(result) == result){

			factor1 = (int)result;
			factor2 = (int)result;
			square = true;
		} else {
			int delta= (int)Mathf.Pow (2,15);
			for (int i=0;i<factors.Count-1;i+=2){
				int d1 = factors[i+1].numerator - factors[i].numerator;
				//				// commented Debug.Log ("i:"+i+"...." +factors[i].numerator+","+factors[i+1].numerator);
				if (d1  < delta){
					delta = factors[i+1].numerator - factors[i].numerator;
					factor1 = factors[i].numerator;
					factor2 = factors[i+1].numerator;

				} 
			}
		}


		float scale = 3.5f;
		Dictionary<GameObject,Vector3> poss = EffectsManager.NumberGrid(factor1,factor2,ni.transform,scale);

		List<GameObject> toCombine = new List<GameObject>();
		foreach(KeyValuePair<GameObject,Vector3> kvp in poss){
			toCombine.Add (kvp.Key);
		}

		//		float scale = 3.5f;
		//		for (int i=0;i<factor1;i++){
		//			for (int j=0;j<factor2;j++){
		//				GameObject one = NumberManager.inst.CreateNumber(new Fraction(1,1),ni.transform.position);
		//				Destroy(one.GetComponent<Rigidbody>());
		//				Destroy(one.GetComponent<Collider>());
		//				one.transform.parent = combineParent.transform;
		//				Vector3 newPos = new Vector3((i-(i/2f))*scale,(j-(j/2f))*scale,0);
		//				toCombine.Add (one);
		//				poss.Add (one,newPos);
		//			}
		//		}



		Transform combineParent = toCombine[0].transform.parent;
		combineParent.transform.parent = ni.transform;
		foreach(GameObject o in toCombine){
			Vector3 dest = ni.transform.position + combineParent.TransformVector(poss[o]);
			SMW_GF.inst.MoveObjTo(o,dest,3);
		}

		// wait for stuff to get moved
		yield return new WaitForSeconds(2);
		if (!combineParent) yield break;
		if (square){
			GameObject squareGraphic = GameObject.CreatePrimitive(PrimitiveType.Cube);

			squareGraphic.GetComponent<Renderer>().material = squareMat;
			squareGraphic.transform.parent = combineParent.transform;
			squareGraphic.transform.localPosition = new Vector3(scale*(factor1-1)/4f,scale*(factor2-1)/4f,0);
			squareGraphic.transform.LookAt(Camera.main.transform);
			squareGraphic.transform.localRotation = Quaternion.identity;


			squareGraphic.transform.localScale = new Vector3(factor1*2,factor2*2,.1f);
//			squareGraphic.transform.parent = ni.transform;
			AudioManager.inst.PlayComboWin();
			yield return new WaitForSeconds(2);

			for (int i=0;i<factor1;i++){
				toCombine[i].GetComponent<NumberInfo>().childMeshRenderer.GetComponent<Renderer>().material.color = new Color(0,1,0);
				AudioManager.inst.PlayComboPlus();
				yield return new WaitForSeconds (0.4f);
			}
			yield return new WaitForSeconds(1.5f);


			for (int i=factor1;i<toCombine.Count;i++){
				Destroy(toCombine[i]);
			}
			yield return new WaitForSeconds(0.7f);


			for (int i=0;i<toCombine.Count;i++){
				if (toCombine[i] != null) SMW_GF.inst.MoveObjTo(toCombine[i],ni.transform.position,3);
			}
			yield return new WaitForSeconds(1f);

			ni.enabled = true;
			ni.childMeshRenderer.gameObject.SetActive(true);
			ni.SetNumber(new Fraction(result,1));
			MonsterAIRevertNumber rev = ni.gameObject.GetComponent<MonsterAIRevertNumber>();
			if (rev){
				rev.SetNumber(new Fraction(result,1)); 
			}
		} else {
			yield return new WaitForSeconds(2);
			AudioManager.inst.PlayWrongAnswerError(transform.position,1,1);
			foreach(GameObject o in toCombine){
				SMW_GF.inst.MoveObjTo(o,ni.transform.position,3);
			}
			yield return new WaitForSeconds(2);
			// commented Debug.Log ("4");
			if (combineParent) Destroy (combineParent.gameObject);
			ni.enabled = true;
			ni.childMeshRenderer.gameObject.SetActive(true);
		}
		if (combineParent) Destroy (combineParent.gameObject);
	}
}

