using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SMW_GF : MonoBehaviour {
	
	// Some useful functions that other scripts can call arbitrarily.
	
	// Rotate
	// Move obj to
	public static SMW_GF inst;

	public GameObject skybox;
	GameObject player;
	public GameObject whiteoutCube;
	
	GameObject[] textEffectPool;
	int textEffectPoolCur = 0;
	

	public bool laserFiring=false;
	
	WWW www;
	IEnumerator DataBaseUpdate(){
		
		yield return new WaitForSeconds(1);
	}
	
	// Use this for initialization
	GameObject gc;
	void Start () {
		inst = this;
//		gc = GameObject.Find("GUI Gem Count"); // todo: no
		if (skybox) skybox.SetActive(true);

//		UpdateGemCount();
		
		player = GameObject.FindWithTag("Player");
	
	}
	
	
	
	
	public IEnumerator SeriesOfExplosionSounds(Vector3 pos){
		for (int i=0;i<10;i++){
			yield return new WaitForSeconds(Random.Range(.2f,.5f));
//			AudioManager.inst.PlayExplosion1(pos);
		}
	}
	
	public void CreateScorchDecal(Vector3 pos){
		//		GameObject decal = (GameObject)Instantiate(GlobalVars.inst.decalProjector,pos,Quaternion.identity);
		//		decal.transform.parent = GlobalVars.inst.decalProjector.transform.parent;
	}
	
	public GameObject FireRocketFromTo(Vector3 startPos, Vector3 dir, Fraction f, float scale, float radius=18, float rocketPower=600,Transform excludeObjectsUnderParent = null, bool useGravity = false){
		AudioManager.inst.PlayRocketLaunch(startPos,.6f);
		GameObject rocket = NumberManager.inst.CreateNumber(f,startPos,NumberShape.Sphere);
		DoesExplodeOnImpact de = rocket.AddComponent<DoesExplodeOnImpact>();
		rocket.layer = LayerMask.NameToLayer("DontCollideWithPlayer"); // don't collide with player

		if (excludeObjectsUnderParent != null) {
			de.excludeRootFromRocketExplosion = excludeObjectsUnderParent;

		}

		rocket.transform.localScale = Vector3.one*scale;
		rocket = NumberManager.inst.MakeIntoRocket(rocket);
		//		float rocketPower = 600;
		rocket.GetComponent<Rigidbody>().mass = .1f; // light rockets
		rocket.GetComponent<Rigidbody>().AddForce(dir*rocketPower);
		rocket.GetComponent<Rigidbody>().useGravity = useGravity;
//		NumberManager.inst.ApplyDerpEyes(rocket);
		return rocket;
	}
	
	
	//	public void AlertApproval(string s,string levelToLoad){
	//		Time.timeScale=0;
	////		Screen.lockCursor=false;
	////		Screen.showCursor=true;
	//		GlobalVars.inst.alertApprovalWindow.SetActive(true);
	//		GlobalVars.inst.alertApprovalText.Text=s;
	////		StartCoroutine(AlertApprovalE(levelToLoad));
	//	}
	////	public IEnumerator AlertApprovalE(string levelToLoad){
	//		bool bLoop=true;
	//		while (bLoop){
	//			yield return null;
	//			Screen.lockCursor=false;
	//			Screen.showCursor=true;
	//			RaycastHit hit;
	//			if (Input.GetMouseButtonDown(0)){
	//				if (Physics.Raycast(FindObjectOfType<CamGUI>().ScreenPointToRay(Input.mousePosition),out hit)){
	//					if (hit.collider.name == "Button_YesApprove"){
	//						bLoop=false;
	//						GlobalVars.inst.alertApprovalWindow.SetActive(false);
	//						Time.timeScale=1;
	//						Application.LoadLevel(levelToLoad);
	//
	//					} else if (hit.collider.name == "Button_NoCancel"){
	//						bLoop=false;
	//						Time.timeScale=1;
	//						GlobalVars.inst.alertApprovalWindow.SetActive(false);
	//
	//					}
	//				}
	//			}
	//		}
	//		Screen.lockCursor=true;
	//		Screen.showCursor=false;
	//		Time.timeScale=1;
	//		yield return null;
	//	}
	
	
	
	
	
	
	public void UseGravityAfterSeconds(GameObject go, float seconds){
		StartCoroutine(UGASE(go,seconds));
	}
	
	public IEnumerator UGASE(GameObject go, float seconds){
		yield return new WaitForSeconds(seconds);
		if (go) {
			if (go.GetComponent<Rigidbody>()){
				go.GetComponent<Rigidbody>().useGravity=true;
				//				// commented Debug.Log("set rigidbody grav to true at "+Time.time);
			}
		}
		
	}
	
	//	public void CreateRandomMonster(){
	//		AudioManager.inst.PlayCrystalThump1(Player.inst.transform.position);
	//		GameObject randomMonster = new GameObject();
	//		Vector3 newpos=Player.inst.transform.position+Player.inst.transform.forward*33f;
	//		EffectsManager.inst.CreateSmallPurpleExplosion(newpos,5,5);
	//		EffectsManager.inst.CreateSomeStars(newpos);
	//		randomMonster.transform.position=newpos;
	//		NumberGenerator ng = randomMonster.AddComponent<NumberGenerator>();
	//		ng.numbersAreMonsters=true;
	//		ng.number= new Fraction[Random.Range(1,4)];
	//		int r=Random.Range(-4,33);
	//		for (int i=0;i<ng.number.Length;i++){
	//			ng.number[i]=new Fraction(r*(i+1),1);
	//		}
	//		ng.maxToGenerate=Random.Range(3,15);
	//		ng.numberScaleMin=Random.Range(1,2);
	//		ng.numberScaleMax=Random.Range(3,5);
	//		int mti = Random.Range(0,5);
	//
	//		ng.spawnForever=true;
	//		ng.spawnFromExistingPool=true;
	//		ng.stopSpawningIfNoSiblings=true;
	//		switch(mti){
	//			case 0: ng.monsterType = MonsterType.Dumbell; ng.maxToGenerate+=4; break;
	//			case 1: ng.monsterType = MonsterType.Spikey; break;
	////			case 2: ng.monsterType = MonsterType.Guard; ng.transform.position+=new Vector3(0,5,0); ng.spawnFromExistingPool=false; break;
	//			case 2: ng.monsterType = MonsterType.NumberTower; break;
	//			case 3: ng.monsterType = MonsterType.SwarmSpikey; break;
	//			case 4: ng.monsterType = MonsterType.NumberTower; break;
	//			default: ng.monsterType = MonsterType.NumberTower; break;
	//		}
	//		ng.ellipsoid = new Vector3(10,0,10);
	//		GameObject enemyCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
	//		enemyCube.GetComponent<Collider>().isTrigger=true;
	//		enemyCube.transform.position = randomMonster.transform.position;
	//		enemyCube.transform.localScale *= 10;
	//		enemyCube.layer = LayerMask.NameToLayer("EnemyZone");
	//		enemyCube.tag="EnemyZone";
	//		enemyCube.GetComponent<Renderer>().enabled=false;
	//
	//	}
	
	
	public GameObject DefuseRocket(NumberInfo ni, GameObject other){
		//		other.GetComponent<NumberInfo>().forbidCombinations=true;
		Destroy(other);
		GameObject newRocketDefused = NumberManager.inst.CreateNumber(ni.fraction,other.transform.position,NumberShape.Sphere);
		newRocketDefused.name = "RocketDefused";
		ni = newRocketDefused.GetComponent<NumberInfo>();
		return newRocketDefused;
	}
	
	
	float failedPurchaseTimer=0;
	public bool PurchaseForGems(int cost){
//		
//		Vector3 textPos = Camera.main.transform.position + Camera.main.transform.forward*10  +Camera.main.transform.up*1;
//		if (GlobalVars.inst.gemCount > cost){
////			EndLevelTrigger end = FindObjectOfType<EndLevelTrigger>();
////			if (end) end.GemReimbursement(cost);
//			GlobalVars.inst.gemCount-=cost;
//			SMW_GF.inst.UpdateGemCount();
//			//GameObject textEffect1 = SMW_GF.inst.CreateTextEffect(textPos,"Used "+cost+" gems!");
//			//textEffect1.transform.parent = Camera.main.transform;
//			//			textEffect1.AddComponent<TimedObjectDestructor>();
//			return true;
//		} else {
//			if (failedPurchaseTimer > 0){
//				return false;
//			}
//			failedPurchaseTimer = 1;
//			GameObject textEffect = SMW_GF.inst.CreateTextEffect(textPos,"You don't have "+cost+" gems!");
//			if (!textEffect) return false;
//			textEffect.transform.parent = Player.inst.transform;
//			textEffect.AddComponent<TimedObjectDestructor>();
//			return false;
//		}
		return false;
	}
	
	public bool PlayerInsideBoundsOf(Bounds bounds){
		return bounds.Contains(Player.inst.transform.position);
	}
	
	
//	int detailIndex = 0;
//	public void CycleDetail(){
//		return;
//		detailIndex++;
//		if (detailIndex > 3) detailIndex=0;
//		//		// commented Debug.Log("cycle detail : "+detailIndex);
//		if (!GlobalVars.inst.dirLight) {
//			//			// commented Debug.Log("oop");
//			return;
//		}
//		switch (detailIndex){
//		case 0: 
//			GlobalVars.inst.dirLight.GetComponent<Light>().enabled=true;
//			GlobalVars.inst.dirLight.GetComponent<Light>().shadows=LightShadows.Hard;
//			Camera.main.farClipPlane=500;
//			break;
//		case 1:
//			GlobalVars.inst.dirLight.GetComponent<Light>().enabled=true;
//			GlobalVars.inst.dirLight.GetComponent<Light>().shadows=LightShadows.None;
//			Camera.main.farClipPlane=500;
//			break;
//		case 2:
//			GlobalVars.inst.dirLight.GetComponent<Light>().enabled=false;
//			GlobalVars.inst.dirLight.GetComponent<Light>().shadows=LightShadows.None;
//			Camera.main.farClipPlane=500;
//			break;
//		case 3:
//			GlobalVars.inst.dirLight.GetComponent<Light>().enabled=false;
//			GlobalVars.inst.dirLight.GetComponent<Light>().shadows=LightShadows.None;
//			Camera.main.farClipPlane=250;
//			break;
//		default:
//			break;
//			
//		}
//		
//	}
	
	public void RemoveDynamicNumberComponentsFromChildren(Transform p){
		if (p){
			//			// commented Debug.Log("parent length: "+p.childCount);
			int i=0;
			foreach (Transform t in p){
				//				SMW_GF.inst.ForbidCombinationsForSeconds(t.gameObject);
				SMW_GF.inst.RemoveAllDynamicComponents(t.gameObject);
				// commented Debug.Log(t.name+" was neutered");
				//				t.parent=null;
				//				SMW_GF.inst.RemoveAllDynamicComponents(gameObject);
				i++;
			}
			//			// commented Debug.Log("i:" +i);
			//			if (transform.parent.GetComponent<NumberGenerator>()){
			//				Destroy(transform.parent.GetComponent<NumberGenerator>());
			//			}
		}
	}
	
	public void RemoveAllDynamicComponents(GameObject go){
//		if (go.GetComponent<MonsterAINumberTower>()) {
//			Destroy(go.GetComponent<MonsterAINumberTower>());		
//			Destroy (go.transform.Find("DerpEye").gameObject);
//
//			EffectsManager.inst.CreateShards(go.transform.position);
//		}
//		if (go.GetComponent<NumberInfo>()) {
//			//			go.GetComponent<NumberInfo>().numberIsAlive=false;
//			//			go.GetComponent<NumberInfo>().killPlayerOnContact=false;
//		}
//		if (go.GetComponent<MonsterSound>()) Destroy(go.GetComponent<MonsterSound>());
//		if (go.GetComponent<MonsterAIDumbell>()) Destroy(go.GetComponent<MonsterAIDumbell>());
//		if (go.GetComponent<AINumberSheep>()) Destroy(go.GetComponent<AINumberSheep>());
//		if (go.GetComponent<AlwaysFaceCameraY>()) Destroy(go.GetComponent<AlwaysFaceCameraY>());
//		if (go.GetComponent<NumberHoverArea>()) Destroy(go.GetComponent<NumberHoverArea>());
//		if (go.GetComponent<MonsterAIRevertNumber>()) Destroy(go.GetComponent<MonsterAIRevertNumber>());
//		if (go.GetComponent<MonsterAIBase>()) Destroy (go.GetComponent<MonsterAIBase>());
//		if (go.GetComponent<MonsterAISpikey1>()) {
//			Destroy (go.GetComponent<MonsterAISpikey1>());
//			Destroy (go.transform.Find("DerpEyes").gameObject);
//			Destroy (go.transform.Find("Spikes").gameObject);
//			EffectsManager.inst.CreateShards(go.transform.position);
//		}
//		
//		if (go.GetComponent<EnemyBrick>()) Destroy(go.GetComponent<EnemyBrick>());		
//		if (go.GetComponent<AttackPlayerWithLightning>()) Destroy(go.GetComponent<AttackPlayerWithLightning>());
//		//		if (go.GetComponent<BoxCollider>()) Destroy (go.GetComponent<BoxCollider>());
//		//		if (go.GetComponent<SphereCollider>()) Destroy (go.GetComponent<SphereCollider>());
//		// etc;
		
	}
	
	
	public Color RandomColor(){
		switch(Random.Range(0,6)){
		case 0: return Color.yellow; break;
		case 1: return Color.green; break;
		case 2: return Color.gray; break;
		case 3: return Color.blue; break;
		case 4: return Color.red; break;
		case 5: return Color.cyan; break;
		default: return Color.black; break;
		}
		return Color.yellow;
	}
	
	public Vector3 CheckForAltRoutes(Vector3 origin, Vector3 destination){
		//		RaycastHit hit;
		//		
		//		// explanation
		//		float vertLimit = 5; // defintion
		//		float downDistance = 0;
		//		float upDistance = 0;
		//		if (Mathf.Abs(origin.y - destination.y) < vertLimit){
		//			if (Physics.Raycast(origin,Vector3.down,out hit)){		downDistance = hit.distance;	}
		//			if (Physics.Raycast(origin,Vector3.up,out hit)){		upDistance = hit.distance;		}	
		//			if (downDistance > upDistance){ // nothing below us, let's pursue downwards!
		//				return origin - new Vector3(0,-10,0);
		//			}
		//			
		//		}
		
		return Vector3.zero; // nevermind this will take too long to feel right		
		
		
	}
	
	
	
	
	
	
	public GameObject CreatePlusTrail(Vector3 pos){
		return (GameObject)Instantiate(EffectsManager.inst.plusTrail1,pos,Quaternion.identity);
	}
	

	
	public void SendMessageAfterSeconds(GameObject go, string m, float t){
		StartCoroutine(SendMessageAfterSecondsE(go,m,t));
	}
	
	IEnumerator SendMessageAfterSecondsE(GameObject go, string m, float t){
		yield return new WaitForSeconds(t);
		go.SendMessage(m,SendMessageOptions.DontRequireReceiver);
	}
	
	public void DestroyAfterSeconds(GameObject go, float t){
		StartCoroutine(DestroyAfterSecondsE(go,t));
	}
	
	
	public IEnumerator DestroyAfterSecondsE(GameObject go, float t){
		yield return new WaitForSeconds(t);

		Destroy(go);
	}
	
	public bool ColliderWasNumber(Collider other){
		return (other.GetComponent<NumberInfo>() != null);
		//		return ( (GlobalVars.inst.numberLayerMask & 1 << other.gameObject.layer)>0 ||
		//			other.gameObject.layer == LayerMask.NameToLayer("DontCollideWithPlayer")  // include rockets
		//			);
	}
	
	
	
	//	void UpdateHearts(){
	//		FindObjectOfType<HeartDrawer>().DrawHearts(GlobalVars.inst.hearts);
	//	}
	//	
	public void SetGodMode(bool flag){
		//		GlobalVars.inst.godMode=flag;
		//		GlobalVars.il.DisplayInstructionsText("Godmode: "+flag,1.5f);
	}
	
	
//	public void GemDrop(Vector3 pos, int numGems, bool useGrav = true) {
//		
//		List<GameObject> allGems = new List<GameObject>();
//		
//		int size100gems=0;
//		int size10gems=0;
//		int size1gems=0;
//		
//		size100gems = (int)numGems/100;
//		size10gems = (int)((numGems % 100)/10);
//		size1gems = numGems % 10;
//		
//		for (int i=0;i<size100gems;i++){
//			allGems.Add ((GameObject)Instantiate(NumberManager.inst.gemPrefabx100));
//		}
//		for (int i=0;i<size10gems;i++){
//			allGems.Add ((GameObject)Instantiate(NumberManager.inst.gemPrefabx10));
//		}
//		for (int i=0;i<size1gems;i++){
//			allGems.Add ((GameObject)Instantiate(NumberManager.inst.gemPrefab));
//		}
//		
//		foreach(GameObject g in allGems){
//			g.transform.position=pos + Random.onUnitSphere * 3;
//			Vector3 randVel = new Vector3(Random.Range(-1f,1f),Random.Range(-1f,1f),Random.Range(-1f,1f))*3;
//			if (useGrav) {
//				g.GetComponent<Rigidbody>().velocity=new Vector3(0,3,0);
//				g.GetComponent<Rigidbody>().AddForce(randVel,ForceMode.VelocityChange);	
//			} else {
//				g.GetComponent<Rigidbody>().useGravity= false;
//			}
//		}
//		
//	}
//	
//	public void RandomGemDrop(Vector3 pos, int val) {
//		int randVal = (int)Random.Range (1,Mathf.Pow (Mathf.Abs (val),0.399f)); // logarithmic growth for number of gems dropped as number's integer increases linearlly
//		//		// commented Debug.Log ("gem val / randval: "+val +" / " +randVal);
//		if(Random.Range(0, 4) == 0) {
//			GemDrop(pos,1);
//			if(Random.Range(0, 4) == 0) {
//				GemDrop(pos,2*randVal);
//				if(Random.Range(0, 4) == 0) {
//					GemDrop(pos,3*randVal);
//					
//				}				
//			}
//		}
//	}
	
//	public GameObject CreateGem(NumberInfo ni){
//		Vector3 velocityOfNumber = Vector3.zero;
//		if (ni.GetComponent<Rigidbody>()){
//			velocityOfNumber = ni.GetComponent<Rigidbody>().velocity;
//		}
//		Vector3 loc = ni.transform.position;
//		AudioManager.inst.PlayNumberShatter(loc);
//		EffectsManager.inst.CreateShards(loc);
//		GameObject gem = (GameObject)Instantiate(NumberManager.inst.gemPrefab);
////		Powerup_Gem powg = gem.GetComponent<Powerup_Gem>();
//		
//		if (ni.fraction.denominator ==0 || ni.fraction.numerator ==0) powg.num = 0;
//		else powg.num = ni.fraction.numerator / ni.fraction.denominator;
//		
//		MathProblem mp = ni.GetComponent<MathProblem>();
//		if (mp){
//			switch (mp.topic){
//			case MathProblemTopic.AdditionSubtraction:
//				gem.GetComponent<Renderer>().material.color=Color.yellow;
//				powg.num=1;
//				break;
//			case MathProblemTopic.Multiplication:
//			case MathProblemTopic.Fractions:
//				gem.GetComponent<Renderer>().material.color=Color.green;
//				powg.num=5;
//				break;
//			case MathProblemTopic.Squares:
//				gem.GetComponent<Renderer>().material.color=Color.red;
//				powg.num=10;
//				break;
//			case MathProblemTopic.Factoring:
//			case MathProblemTopic.Logic:
//				gem.GetComponent<Renderer>().material.color=Color.blue;
//				powg.num=25;
//				break;
//			default:
//				gem.GetComponent<Renderer>().material.color=Color.yellow;
//				powg.num=1;
//				break;
//			}
//		} else {
//			gem.GetComponent<Renderer>().material.color=Color.yellow;
//			powg.num=1;
//		}
//		
//		
//		
//		
//		//		GameObject gemText=  NumberManager.inst.CreateDigits(ni.fraction);
//		//		gemText.transform.parent=gem.transform;
//		//		gemText.transform.localPosition=new Vector3(0,0,1);
//		//		gemText.transform.localScale = Vector3.one*.5f;
//		//		
//		//		powg.pow1.Text = ""; // This will be used for "power gems" later, like 3^6 which is a lot of freaking gems
//		//		powg.pow2.Text = "";
//		//		ni.fraction.numerator.ToString();
//		//		if (ni.fraction.denominator > 1) powg.gemText.Text += "/"+ni.fraction.denominator.ToString();
//		
//		gem.GetComponent<Rigidbody>().mass = 100;
//		gem.transform.position=loc+new Vector3(0,1,0);
//		Vector3 randVel = new Vector3(Random.Range(-1f,1f),Random.Range(-1f,1f),Random.Range(-1f,1f))*3000f;
//		gem.GetComponent<Rigidbody>().velocity=new Vector3(velocityOfNumber.x,velocityOfNumber.y+5,velocityOfNumber.z)*.3f;
//		gem.GetComponent<Rigidbody>().AddForce(randVel);
//		return gem;
//	}
	
	public bool RaycastCheck(Transform a, Transform b){ // was there line of sight? // layermask!??
		
		RaycastHit hit;
		Vector3 dir = b.position - a.position;
		if (Physics.Raycast(new Ray(a.position,dir),out hit)){
			// commented Debug.Log("hit "+hit.collider.name+" with raycast from "+a.name+" to "+b.name);
			if (hit.collider.name == b.name){
				return true;
			} else {
				return false;
			}
		} else return false;
		
	}
	
	
	
	//	public bool IsNumber(Collider collider){
	//
	////		if (collider.GetComponent<ResourceNumber>()) return false;
	////		if (collider.gameObject.layer==18) return true;
	////		if (collider.gameObject.layer==8) return true;
	////
	////		return ((GlobalVars.inst.numberLayerMask.value & 1<<collider.gameObject.layer)>0);
	//	}
	//	
	IEnumerator LightningTimeout(LightningBoltShuriken lb,float seconds){
		
		yield return new WaitForSeconds(seconds);
		if (lb) {
			Destroy(lb.gameObject);
			Destroy(lb.target.gameObject);
		}
	}
	
	
	
	
	public void CreateLightning(Transform origin, Transform dest,float duration,bool redLightning=false){
		GameObject newLightning;
		// create lightning bolt and get ref to its scriptie
		if (redLightning) {
			newLightning = (GameObject)Instantiate(EffectsManager.inst.lightningPrefabRed);
			newLightning.GetComponent<LightningBoltShuriken>().endLight.color=Color.red;
//			newLightning.GetComponent<LightningBolt>().endLight.range;
		}
		else newLightning = (GameObject)Instantiate(EffectsManager.inst.lightningPrefab);
		
		//		newLightning.name="newlightning";
		//		// commented Debug.Log("lightning much?");
		if (!origin || !dest) return;
		AudioManager.inst.PlayLightning((origin.position+dest.position)/2);
		LightningBoltShuriken lb = newLightning.GetComponent<LightningBoltShuriken>();
		
		// set beginning point for lightning bolt
		newLightning.transform.position=origin.position;
		newLightning.transform.parent = origin;
		
		// Set end point for lightning bolt
		lb.target.position = dest.position;
		lb.target.parent = dest;
		
		// timeout the lightning bolt, duration currently hardcoded 
		StartCoroutine(LightningTimeout(lb,duration));
	}
	
	public void DestroyInSeconds(GameObject g, float time) {
		TimedObjectDestructor tod = g.AddComponent<TimedObjectDestructor>();
		tod.autoDestruct = false;
		tod.DestroyNow(time);
	}
	
	public void SetActiveRecursively(GameObject obj, bool flag){
		if (!obj) return; 
		obj.SetActive(flag);
		foreach (Transform child in obj.transform){
			SetActiveRecursively(child.gameObject,flag); // recurse
		}
	}
	
	// Checks the collision matrix for layers. Note: This is already done in ProjectSettings > Physics, however NavMeshAgents need this manual check to pass navmeshes.
	public bool CanNumberPassThisWall(int numberLayer, int wallLayer){
		bool canPass = false;
		if (
			(wallLayer == 29 && numberLayer == 8) ||
			(wallLayer == 27 && numberLayer == 25) ||
			(wallLayer == 28 && numberLayer == 26)
			) 
			canPass=true;
		//			// commented Debug.Log("Checked number layer "+numberLayer+" against wall layer "+wallLayer);
		return canPass;
		
	}
	


	
	public void Stop(string coroutine){
		StopCoroutine(coroutine);
	}
	
	IEnumerator ScaleObjectOverTimeC(GameObject obj, Vector3 newScale, float time){
		
		bool bNeedsScale = true;
		float speed = 1f/time; // speed good?
		while (bNeedsScale){
			obj.transform.localScale = Vector3.Lerp(obj.transform.localScale,newScale,Time.deltaTime*speed);
			yield return new WaitForSeconds(.01f); // not as good as update?
			if (null == obj) yield break        ; // sometimes they are destroyed by GemZoneMonsterAI before finished scaling
			if (Vector3.Magnitude(obj.transform.localScale-newScale)<.1f){
				bNeedsScale=false;
				obj.transform.localScale = newScale; // snap
			}
		}
	}
	
	public void ScaleObjectOverTime(GameObject obj, Vector3 newScale, float time){
		StartCoroutine(ScaleObjectOverTimeC(obj,newScale,time));
	}
	
	
	public void CreateExplosionForce(Vector3 position, int radius, int force){
		Collider[] colliders = Physics.OverlapSphere(position,radius);
		foreach (Collider collider in colliders){
			if (collider.GetComponent<Rigidbody>()){
				Vector3 newForce = Vector3.Normalize(collider.transform.position - position)*force;
				collider.GetComponent<Rigidbody>().AddForce(newForce);
			}
		}
	}
	
	
	
	
	
	
	
	
	
	
	public GameObject DrawLineFromTo(Transform f, Transform t, Color c, float widthScale=1){
		GameObject r = new GameObject();
		MovingLaser m = r.AddComponent<MovingLaser>();
		m.DrawMovingLaserFromTo(f,t,c,widthScale);
		return r;
	}
	
	public GameObject DrawCoolLaserFromTo(Transform f, Transform t,Color c, Color sc, float widthScale=1){
		GameObject r = new GameObject();
		MovingLaser m = r.AddComponent<MovingLaser>();
		m.DrawMovingLaserFromTo(f,t,c,widthScale);
		SineLaser sl = r.AddComponent<SineLaser>();
		sl.DrawSineLaserFromTo(f,t,sc, widthScale);
		return r;
	}
	
	
	
	
	
	
	
	
	//	public IEnumerator FadeLaserOut(LineRenderer opLaser){
	//
	//		laserFiring=false;
	//		
	//		if (laserFiring) {
	//
	//			yield break; // early out in case laser was fired again since timeout was started
	//		}
	//		
	//		float startTime = Time.time;
	//		float fadeTime = 1.7f;
	//		float fadeVal = 1;
	//		
	//		Color opColor = Color.yellow;
	//		while (Time.time < startTime + fadeTime && !laserFiring){
	//			fadeVal -= .05f; // fade out every frame
	//			Color fadeColor = new Color(opColor.r,opColor.g,opColor.b,fadeVal);// define fade color to only impact alpha
	//			opLaser.SetColors(fadeColor,fadeColor); // set the fade color
	//			yield return null;
	//			
	//		}
	//		if (!laserFiring) opLaser.SetVertexCount(0); // done
	//	
	//	}
	
	
	
	
	
	
	
	
	public float RandomFloat(float range){
		return Random.Range(-range,range);
	}
	
	public Vector3 RandomVector3(float range){
		return new Vector3(RandomFloat(range),RandomFloat(range),RandomFloat(range));
	}
	
	public GameObject SetUpCenteredCCTextObject(bool flip){
		GameObject statusTextObjectParent = new GameObject();
		GameObject statusTextObject= new GameObject();
//		statusTextObject.transform.parent = statusTextObjectParent.transform;
//		statusTextObject.name = "StatusTextObjectChild";
//		if (flip) statusTextObject.transform.eulerAngles = new Vector3(0,180,0);
//		CCText statusText = statusTextObject.AddComponent<CCText>();
////		statusText.Font = GlobalVars.inst.standardCCFont;
//		statusText.Color = Color.white;
//		statusTextObject.GetComponent<Renderer>().material = GlobalVars.inst.standardFontMaterial;
//		//		statusText.Text = "Hi!";
//		statusText.Alignment=CCText.AlignmentMode.Center;
//		statusText.HorizontalAnchor=CCText.HorizontalAnchorMode.Center;
//		statusText.VerticalAnchor=CCText.VerticalAnchorMode.Middle;
//		statusTextObject.transform.localScale *=2;
//		statusTextObjectParent.name = "Status Text (parent)";
		return statusTextObjectParent;
	}
	
	
	public void SetRendererRecursively(GameObject obj,bool flag){
		if (obj.GetComponent<Renderer>()) obj.GetComponent<Renderer>().enabled=flag; // disable my renderer
		foreach(Transform child in obj.transform){
			if (child.GetComponent<Renderer>()) child.GetComponent<Renderer>().enabled=flag; // disable child renderers too
			SetRendererRecursively(child.gameObject,flag); // recurse through all children
		}
	}
	
	//	public void UpdateAllGravityFields(){
	//		GameObject[] fields = GameObject.FindGameObjectsWithTag("GravityField");
	//		foreach (GameObject field in fields){
	//			GravityField gf = field.GetComponent<GravityField>();
	////			gf.CheckGravity();
	////			// commented Debug.Log("Checking gravity from update all");
	//		}
	//	}
	
	public GameObject GetClosestObjectWithLayer(Vector3 receivedPosition, string thisLayer){
		Collider[] colliders = Physics.OverlapSphere(receivedPosition,700);
		
		if (colliders.Length > 0) {
			GameObject closest = colliders[0].gameObject; // default to the center of the whole machine. Don't worry, this will get replaced in the foreach loop.
			
			float lastDistance = Mathf.Infinity; // prep the last distance -- this is how we compare between terms as we enumerate through all children.
			
			
			foreach (Collider col in colliders){ // Enumerate through all term receiver children to find which one our target was closest to.
				if (col.gameObject.layer == LayerMask.NameToLayer(thisLayer)){
					float curDistance = Vector3.SqrMagnitude(col.transform.position-receivedPosition);
					if (lastDistance > curDistance){
						lastDistance = curDistance;
						closest = col.gameObject;
					}
				}
			}
			
			
			return closest; // This should be the closest target.
		} else {
			//// commented Debug.Log("ERROR : No objects of tag "+thisTag+" were found.");
			return null;
		}
		
	}	
	
	
	public static GameObject GetClosest(Vector3 from, List<GameObject> others) {
		GameObject closest = null;
		float min = float.PositiveInfinity;
		foreach(GameObject g in GameObject.FindGameObjectsWithTag("EnemyZone")) {
			float dist = (g.transform.position - from).sqrMagnitude;
			if(dist < min) { 
				min = dist;
				closest = g;
			}
		}
		return closest;
	}
	
	// TODO: Don't ever use this function, dummy. Use tags
	public GameObject GetClosestObjectWithName(Vector3 receivedPosition, string nameSought){
		Collider[] colliders = Physics.OverlapSphere(receivedPosition,70);
		
		if (colliders.Length > 0) {
			GameObject closest = colliders[0].gameObject; // default to the center of the whole machine. Don't worry, this will get replaced in the foreach loop.
			
			float lastDistance = Mathf.Infinity; // prep the last distance -- this is how we compare between terms as we enumerate through all children.
			
			
			foreach (Collider col in colliders){ // Enumerate through all term receiver children to find which one our target was closest to.
				if (col.gameObject.name == nameSought){
					float curDistance = Vector3.SqrMagnitude(col.transform.position-receivedPosition);
					if (lastDistance > curDistance){
						lastDistance = curDistance;
						closest = col.gameObject;
					}
				}
			}
			
			
			return closest; // This should be the closest target.
		} else {
			//// commented Debug.Log("ERROR : No objects of tag "+thisTag+" were found.");
			return null;
		}
		
	}	
	
	
	public GameObject GetClosestObjectWithTag(Vector3 receivedPosition, string thisTag){
		GameObject[] allWithTag = GameObject.FindGameObjectsWithTag(thisTag);
		if (allWithTag.Length > 0) {
			GameObject closest = allWithTag[0]; // default to the center of the whole machine. Don't worry, this will get replaced in the foreach loop.
			
			float lastDistance = Mathf.Infinity; // prep the last distance -- this is how we compare between terms as we enumerate through all children.
			
			
			foreach (GameObject child in allWithTag){ // Enumerate through all term receiver children to find which one our target was closest to.
				float curDistance = Vector3.SqrMagnitude(child.transform.position-receivedPosition);
				if (lastDistance > curDistance){
					lastDistance = curDistance;
					closest = child;
				}
			}
			
			
			return closest; // This should be the closest target.
		} else {
			//// commented Debug.Log("ERROR : No objects of tag "+thisTag+" were found.");
			return null;
		}
		
	}
	
//	public void AddGems(int count) {
//		
//		GlobalVars.inst.gemCount += Mathf.Abs(count);
//		foreach(GemGUIEffects fx in FindObjectsOfType<GemGUIEffects>()){fx.GotGem();} // Morgan GUI
//		UpdateGemCount();	
//	}
	
	
	
//	public void UpdateGemCount() {
//		//		// commented Debug.Log ("update gem count. existing: "+PlayerPrefs.GetInt("GemCount") + "; new: "+GlobalVars.inst.gemCount);
//		PlayerPrefs.SetInt("GemCount",GlobalVars.inst.gemCount);
//		
//		if(gc != null)
//		{
//			foreach(MeshRenderer mr in gc.transform.parent.GetComponentsInChildren<MeshRenderer>()) { 
//				mr.enabled = true;
//			}
//			gc.GetComponent<CCText>().Text = GlobalVars.inst.gemCount.ToString();
//		}
//		
//	}
//	
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
	

//	public void CreateFormulaEffect(Vector3 pos, Fraction a, Fraction b, string op) {
//		Fraction ans;
//		switch(op) {
//		case "+":
//			ans = Fraction.Add(a, b);
//			if(b.numerator < 0) {
//				CreateTextEffect(pos, a.ToString() + " - " + new Fraction(-b.numerator, b.denominator).ToString() + " = " + ans.ToString());
//			} else {
//				CreateTextEffect(pos, a.ToString() + " + " + b.ToString() + " = " + ans.ToString());
//			}
//			break;
//		case "-":
//			ans = Fraction.Add(a, new Fraction(-b.numerator, b.denominator));
//			if(b.numerator < 0) {
//				CreateTextEffect(pos, a.ToString() + " + " + new Fraction(-b.numerator, b.denominator).ToString() + " = " + ans.ToString());
//			} else {
//				CreateTextEffect(pos, a.ToString() + " - " + b.ToString() + " = " + ans.ToString());
//			}
//			break;
//		case "*":
//			ans = Fraction.Multiply(a, b);
//			CreateTextEffect(pos, a.ToString() + " * " + b.ToString() + " = " + ans.ToString());
//			break;
//		case "/":
//			ans = Fraction.Multiply(a, b.GetReciprocal());
//			CreateTextEffect(pos, a.ToString() + " / " + b.ToString() + " = " + ans.ToString());
//			break;
//		default:
//			CreateTextEffect(pos, a.ToString() + " " + op + " " + b.ToString() + " = ?");
//			break;
//		}
//	}
//	

	
//	public GameObject SetupFollowText(GameObject objToFollow, string textToCreate, Color color){
//		GameObject textFollow = new GameObject();
//		textFollow.name = "tf2";
//		GameObject textFollowParent = new GameObject(); // needed to flip the child 180° abuot y axis so CCText is visible when facing away from player 
//		textFollowParent.name = textToCreate;
//		
//		SmoothFollow sf = textFollowParent.AddComponent<SmoothFollow>();
//		sf.objectToFollow = objToFollow.transform; // make it "follow" the reaminder
//		textFollow.transform.parent=textFollowParent.transform;
//		textFollow.transform.eulerAngles = new Vector3(0,180,0);
//		textFollowParent.transform.position = objToFollow.transform.position+ new Vector3(0,1.5f,0); // position it just above the number it marks
//		textFollowParent.AddComponent<AlwaysFacePlayer>();
//		
//		
//		
//		CCText cct = textFollow.AddComponent<CCText>();
//		cct = SetupText(cct,GlobalVars.inst.standardCCFont);
//		cct.Text = textToCreate;
//		cct.Color = color;
//		textFollow.GetComponent<Renderer>().material = GlobalVars.inst.standardFontMaterial;
//		textFollow.transform.position = textFollowParent.transform.position;
//		return textFollowParent;
//	}
//	
	//	IEnumerator TempDisablePlayer(){
	//		yield return new WaitForSeconds(.15f); // todo: No waiting for starting functions!
	//		if (null != GlobalVars.inst.camFlythruObj) {
	//			SMW_GF.inst.SetActiveRecursively(Player,false);
	//		}
	//	
	//	}
	
	
	bool fadeInWhitePlane=false;
	public void FadeInWhitePlane(){
		fadeInWhitePlane=true;
	}
	
	
	

	float playerDamagedTimer = 0;
	void Update () {
		
		
		failedPurchaseTimer-=Time.deltaTime;

		if (bNeedsWhiteout){
			
			
			float whiteoutTime = 2.5f; // maximum (will snap)
			float lerpSpeed = 3.0f;
			
			if (Time.time < whiteoutStartTime + whiteoutTime){
				lerpAlpha = Mathf.Lerp(lerpAlpha,0,Time.deltaTime*lerpSpeed);
				//				whiteoutCube.GetComponent<Renderer>().material.color = new Color(1,1,1,lerpAlpha);
			} else {
				//			if (Time.time > whiteoutStartTime + whiteoutTime){
				bNeedsWhiteout = false;
				//				whiteoutCube.GetComponent<Renderer>().material.color = new Color(1,1,1,0);
			} 
		}
		
		if (fadeInWhitePlane){
			float lerpSpeed = 5.0f;
			lerpAlpha = Mathf.Lerp(lerpAlpha,1,Time.deltaTime*lerpSpeed);
			//			whiteoutCube.GetComponent<Renderer>().material.color = new Color(1,1,1,lerpAlpha);
			
		}
		
	}
	
	public string ReplaceFirst(string text, string search, string replace) {
		int pos = text.IndexOf(search);
		if (pos < 0)
		{
			return text;
		}
		return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
	}
	
	
	public CCText SetupText(CCText textToSetup,CCFont useThisFont){
		
		textToSetup.HorizontalAnchor = CCText.HorizontalAnchorMode.Center;
		textToSetup.VerticalAnchor = CCText.VerticalAnchorMode.Middle;
		textToSetup.Alignment = CCText.AlignmentMode.Center;
		
		textToSetup.Font = useThisFont;
		textToSetup.Color = Color.white;
		return textToSetup;
	}
	
	
	bool bNeedsWhiteout=false;
	float whiteoutStartTime = 0;
	float lerpAlpha ;

	
	public GameObject GetClosestObjectInArray(GameObject[] objs, Vector3 pos){
		float nearestDist = float.PositiveInfinity;
		GameObject nearest = null;
		Vector3 offset = Vector3.zero;
		foreach(GameObject t in objs) {
			if(t) {
				offset = t.transform.position - pos;
				if(offset.sqrMagnitude < nearestDist) {
					nearestDist = offset.sqrMagnitude;
					nearest = t;
				}
			}
		}
		return nearest;
	}
	
	
	public void FlashColor(GameObject go, int materialIndex, Color flashColor){
		StartCoroutine(FlashColorE(go, materialIndex,flashColor));
	}
	
	public IEnumerator FlashColorE(GameObject go, int materialIndex, Color flashColor){
		yield return null;
		// check if obj is already flashing; if so, refuse to flash again.
		if (go.name != "FLASHING") {
			//			// commented Debug.Log("attempting to set go "+go.name+" mat index "+materialIndex+" to "+flashColor);
			string origName = go.name;
			go.name="FLASHING"; 
			bool em = false;
			//			// commented Debug.Log("Flashing "+go.name+" index "+materialIndex);
			Color baseColor = go.GetComponent<Renderer>().materials[materialIndex].color;
			Color baseEmissionColor = Color.black;
			if (go.GetComponent<Renderer>().materials[materialIndex].HasProperty("_Emission")) {
				em=true; // only attempt emission assignemnts if the object's material has that property
				baseEmissionColor = go.GetComponent<Renderer>().materials[materialIndex].GetColor("_Emission");
				
			}
			float startTime = Time.time;
			float blinkDuration  = 1.2f;
			float blinkInterval = .1f;
			while (Time.time < startTime+blinkDuration){
				if (go.GetComponent<Renderer>().materials[materialIndex].color == flashColor){
					go.GetComponent<Renderer>().materials[materialIndex].color = baseColor;
					if (em) go.GetComponent<Renderer>().materials[materialIndex].SetColor("_Emission",baseEmissionColor);
				} else {
					go.GetComponent<Renderer>().materials[materialIndex].color = flashColor;
					if (em) go.GetComponent<Renderer>().materials[materialIndex].SetColor("_Emission",flashColor);
				}
				yield return new WaitForSeconds(blinkInterval);
			}
			yield return new WaitForSeconds(blinkDuration);
			go.GetComponent<Renderer>().materials[materialIndex].color = baseColor;
			if (em) go.GetComponent<Renderer>().materials[materialIndex].SetColor("_Emission",baseEmissionColor);
			go.name = origName; //// commented Debug.Log("reset name to "+go.name+" at time "+Time.time);
		}
	}
	
	public void SlowRigidbodyVelocityToZero(GameObject obj, float s){
//		// commented Debug.Log("hi");
		while (s>0){
			s-=Time.deltaTime;
			obj.GetComponent<Rigidbody>().velocity=Vector3.Lerp(obj.GetComponent<Rigidbody>().velocity,Vector3.zero,Time.deltaTime*(1f/s)/10f);
			// commented Debug.Log("obj vel: "+obj.GetComponent<Rigidbody>().velocity);
		}
	}
	
	public void MoveObjTo(GameObject obj, Transform dest, float moveSpeed,bool destroyFlag=false, float cutoffTime=3){
		StartCoroutine(MoveObjToE(obj,dest,moveSpeed,destroyFlag,cutoffTime));
	}
	
	public void MoveObjTo(GameObject obj, Vector3 dest, float moveSpeed,bool destroyFlag=false, float cutoffTime=3){
		StartCoroutine(MoveObjToE(obj,dest,moveSpeed,destroyFlag,cutoffTime));
	}
	//
	//
	//	public void MoveObjConstantSpeed(Transform t, Vector3 dest, float moveSpeed, float cutoffTime=3){
	//		
	//		while (cutoffTime > 0){
	////			// commented Debug.Log("moving "+cutoffTime);
	//			cutoffTime -= Time.deltaTime;
	//			Vector3 norm = Vector3.Normalize(dest - t.position);
	//			t.position += norm*.1f;//*moveSpeed*Time.deltaTime*1f;
	////			// commented Debug.Log("t pos: "+t.position+"; norm: "+norm+";");
	//			if (Vector3.SqrMagnitude(t.position-dest)<10 || cutoffTime <= 0){
	//				cutoffTime=0;
	////				t.position=dest;
	//			}
	//		}
	//		
	//		
	//	}
	
	public IEnumerator MoveObjToE(GameObject obj, Vector3 dest, float moveSpeed,bool destroyFlag,float cutoffTime){
		var rotSpeed = 34;
		bool bNeedsMove=true;
		float timeStarted = Time.time;
		Vector3 origin = obj.transform.position;
		//		// commented Debug.Log ("obj moving start: "+obj.name);
		while(obj && bNeedsMove){
			//			// commented Debug.Log ("obj moving: "+obj.name);
			if (Vector3.SqrMagnitude(obj.transform.position-dest)<.02f) bNeedsMove=false;
			else if (Time.time > timeStarted + cutoffTime) bNeedsMove=false;
			else if (!obj) bNeedsMove=false;
			// slowly lerp to new pos and rot
			//			obj.transform.position = Vector3.Lerp(origin,dest,Time.deltaTime*moveSpeed);
			obj.transform.position = Vector3.Lerp(obj.transform.position,dest,Time.deltaTime*moveSpeed);
			//			Vector3 dir = Vector3.Normalize(dest - transform.position);
			//			obj.transform.position += dir * moveSpeed; // * Time.deltaTime;
			//			// commented Debug.Log ("move: "+obj.transform.position);
			//			obj.transform.rotation = Quaternion.identity;//Quaternion.Slerp(obj.transform.rotation,Quaternion.identity,Time.deltaTime*rotSpeed);
			if(obj.GetComponent<Rigidbody>()) { if (!obj.GetComponent<Rigidbody>().isKinematic) obj.GetComponent<Rigidbody>().velocity = Vector3.zero; }
			yield return null; // don't allow loop to overflow
		}
		
		// Lerp finished, now reposition object exactly
		if (obj){
			obj.transform.position = dest; // snap to destination after lerping almost all the way there
			obj.transform.rotation = Quaternion.identity;
			if(obj.GetComponent<Rigidbody>()) { 
				if (!obj.GetComponent<Rigidbody>().isKinematic)
					obj.GetComponent<Rigidbody>().velocity = Vector3.zero; 
			}
			
		}
		if (destroyFlag){
			Destroy(obj);
			
		}
		yield return true; // dummy return?
	}
	

	public IEnumerator MoveObjToE(GameObject obj, Transform dest, float moveSpeed,bool destroyFlag,float cutoffTime){
		
		var rotSpeed = 34;
		bool bNeedsMove=true;
		float timeStarted = Time.time;
		
		while(obj && bNeedsMove){
			if(obj.GetComponent<Rigidbody>()) { obj.GetComponent<Rigidbody>().velocity = Vector3.zero; }	
			// early out for destination disappearaed
			if (!dest) {
				transform.parent = null;
//				// commented Debug.Log("giving life:"+obj.name);
//				NumberManager.inst.GiveLife(obj.transform.gameObject);
				yield return false;
			}
			
			if (Vector3.SqrMagnitude(obj.transform.position-dest.position)<.2f) bNeedsMove=false;
			else if (Time.time > timeStarted + cutoffTime) bNeedsMove=false;
			else if (!obj) bNeedsMove=false;
			// slowly lerp to new pos and rot
			obj.transform.position = Vector3.Lerp(obj.transform.position,dest.position,Time.deltaTime*moveSpeed);
			obj.transform.rotation = Quaternion.Slerp(obj.transform.rotation,dest.rotation,Time.deltaTime*rotSpeed);
			yield return null; // don't allow loop to overflow
		}
		
		// Lerp finished, now reposition object exactly
		if (obj){
			obj.transform.position = dest.position; // snap to destination after lerping almost all the way there
			obj.transform.rotation = dest.rotation;
		}
		if (destroyFlag){
			Destroy(obj);
			
		}
		yield return true; // dummy return?
	}
	
	
	bool bNeedsRotate = false;
	public void RotateExactly(Transform pivotPoint, Vector3 globalDir, float degrees, float speed){
		StartCoroutine(RotateExactlyE(pivotPoint,globalDir,degrees,speed));
	}
	
	IEnumerator RotateExactlyE(Transform pivotPoint, Vector3 globalDir, float degrees, float speed){
		yield return null;
		bNeedsRotate=true;
		degrees = degrees % 360f;
		
		
		if (Mathf.Abs(pivotPoint.localRotation.eulerAngles.z-degrees) > 180){
			// make it not spin the long way
		}
		
		// Quaternion.FromToRotation
		// // commented Debug.Log("global dir: "+globalDir);
		//		float newRotZ = pivotPoint.localRotation.eulerAngles.z + degrees;
		
		// NOTE: when moving farther than 180° it's unclear which direction to move.
		
		Quaternion newRot = new Quaternion();
		float startedTime = Time.time;
		float maxTimeAllotedForRotation=2;
		
		
		// Check for local degrees vs desired degrees, to see if it's closer to 360 or 0.
		if (pivotPoint.localRotation.z==degrees) bNeedsRotate=false; // skip if we rotate to same degrees
		//		else if (Mathf.Abs(pivotPoint.localRotation.eulerAngles.z-degrees)>180) degrees += 360;
		
		while (bNeedsRotate){
			//			// commented Debug.Log("local Z: "+pivotPoint.localRotation.eulerAngles.z+"; degrees: "+degrees);
			
			float lerpDegrees = Mathf.LerpAngle(pivotPoint.localRotation.eulerAngles.z,degrees,Time.deltaTime*speed);
			
			newRot.eulerAngles = new Vector3(pivotPoint.localRotation.eulerAngles.x,pivotPoint.localRotation.eulerAngles.y,lerpDegrees);
			pivotPoint.localRotation = newRot;
			
			if (Mathf.Abs(pivotPoint.localRotation.eulerAngles.z-degrees)<.1f){
				bNeedsRotate=false; // if it overshoots by .1f or less, we can catch it here to prevent "double flipping" (the lerp function appears to be one directional, so when it crosses the 0/360 mark, it tries to flip all the way around a 2nd time)
			}
			
			// escape the loop when following conditions are met
			//			if (Mathf.Abs(pivotPoint.localRotation.eulerAngles.z - degrees) > .1f) {
			//				bNeedsRotate = false;
			//				// commented Debug.Log("degrees condition met");
			//			}
			if (Time.time > startedTime + maxTimeAllotedForRotation) {
				bNeedsRotate = false;
				//// commented Debug.Log("Time exceeded for rotation");
			}
			yield return null;
		}
		
		// loop finished, now set rotation to exactly destination
		newRot.eulerAngles = new Vector3(pivotPoint.localRotation.eulerAngles.x,pivotPoint.localRotation.eulerAngles.y,degrees);
		pivotPoint.localRotation = newRot;
		
		
		
		
	}
	
	
	
	
	//	public void ForbidCombinationsForSeconds(GameObject go,float seconds=2){
	//		StartCoroutine(ForbidCombinationsForSecondsE(go,seconds));
	//	}
	//		
	//	IEnumerator ForbidCombinationsForSecondsE(GameObject go, float seconds){
	//
	//		NumberInfo ni = go.GetComponent<NumberInfo>();
	//		if (!ni) yield return false;
	////		ni.forbidCombinations=true;
	//		yield return new WaitForSeconds(seconds);
	////		ni.forbidCombinations=false;
	//
	//	}
	
	public void  SetLayerRecursively (GameObject obj,int newLayer){
		obj.layer = newLayer;
		foreach( Transform child in obj.transform ) {
			SetLayerRecursively( child.gameObject, newLayer );
		}
	}
	
	public void SetTagRecursively(GameObject o,string s){
		o.tag=s;
		foreach(Transform t in o.transform){
			SetTagRecursively(t.gameObject,s);
		}
	}
	
	public GameObject GetClosestChildToPlayer(Transform t){
		float dist = Mathf.Infinity;
		GameObject o = null;
		if (!t) return null;
		foreach(Transform c in t){
			float diff = Vector3.SqrMagnitude(c.transform.position-Player.inst.transform.position);
			if (diff < dist){
				o = c.gameObject;
				dist=diff;
			}
		}
		return o;
	}
	
	
	
	public bool CanCompleteLevelChecks(){
		
		
		// all sheep collected?
		if (!PlayerPrefs.HasKey("CollectSheepThisLevel")){			PlayerPrefs.SetInt("CollectSheepThisLevel",0);		}
		if (!PlayerPrefs.HasKey("SheepCollected")){					PlayerPrefs.SetInt("SheepCollected",0);		}
		if (!PlayerPrefs.HasKey("SheepToCollect")){					PlayerPrefs.SetInt("SheepToCollect",0);		}
		if (PlayerPrefs.GetInt("CollectSheepThisLevel")==1){
			if (PlayerPrefs.GetInt("SheepCollected")>=PlayerPrefs.GetInt("SheepToCollect")){
				return true;
			} else {
				int diff = PlayerPrefs.GetInt("SheepToCollect") - PlayerPrefs.GetInt("SheepCollected");
//				GlobalVars.inst.il.DisplayInstructionsText("You still need to collect "+diff+" sheep.",5);
				return false;
			}
		}
		return true;
	}
	

	
	
//	public void GiveGems(int r,Vector3 p,int value=10){
//		for(int i=0;i<r;i++){
//			GameObject gem = (GameObject)Instantiate(NumberManager.inst.gemPrefab,p+new Vector3(Random.Range(-4,4),Random.Range(-4,4),Random.Range(-4,4)),transform.rotation);
//			gem.GetComponent<Renderer>().material.color = SMW_GF.inst.RandomColor();
//			gem.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-100,100),Random.Range(200,500),Random.Range(-100,100)));
//			gem.GetComponent<Powerup_Gem>().num = value;
//		}
//		
//	}
	
	public bool PlayerIsCloseToAndPointingAt(Transform t, float d){
		Vector3 dirToT = Vector3.Normalize(t.position-Player.inst.transform.position);
		if (Vector3.Distance(Player.inst.transform.position,t.position)<d
		    && Vector3.Angle (Player.inst.transform.forward,dirToT)<90){
			return true;
		}
		return false;
	}
	
	public IEnumerator DropAllRigidbodies(GameObject o, float s){
		yield return new WaitForSeconds(s);
		//		// commented Debug.Log ("dropall:" +o.name);
		AudioManager.inst.PlayCrash(o.transform.position,1,1);
		int i=0;
		GameObject temp = new GameObject();
		if (o.transform.childCount==0) yield return false;
		temp.transform.position = o.transform.GetChild(0).position;
		//		List<GameObject> tempL = new List<GameObject>(); // for speed, group objects by 10.
		int objectSkip = 20;
		List<Transform> ts = new List<Transform>();
		foreach(Transform t in o.transform){
			ts.Add (t);
		}
		foreach(Transform t in ts){
			if (i % objectSkip == 0){
				
				EffectsManager.inst.CreateSmokePuffBig(t.position,Vector3.up*10,20);
				temp.gameObject.AddComponent<Rigidbody>();
				//				temp.rigidbody.isKinematic=false;
				//				temp.rigidbody.useGravity=true;
				temp.GetComponent<Rigidbody>().AddForce(Random.insideUnitSphere*1000);
				temp.GetComponent<Rigidbody>().AddTorque(Random.insideUnitSphere*1000);
				
				temp.AddComponent<ConstantForce>().force = Vector3.up * -50;
				temp.name = "temp :"+i;
				// commented Debug.Log ("dropped :"+temp.name);
				temp = new GameObject(); // create a new temp
				temp.AddComponent<TimedObjectDestructor>();
				temp.transform.position = t.position;
			}
			if (t.GetComponent<Rigidbody>()){
				Destroy (t.GetComponent<Rigidbody>());
			}
			t.parent = temp.transform;
			
			i++;
			//DropAllRigidbodies(t.gameObject,0); // recurse
		}
		temp.AddComponent<TimedObjectDestructor>();
		temp.AddComponent<Rigidbody>();
	}
	

	
	
	
	public void SinGrow(GameObject o){
		if (o.GetComponent<SinGrow>()) Destroy (o.GetComponent<SinGrow>());
		o.AddComponent<SinGrow>();
	}
	

	
	public void CreateCircleOfNumbers(Vector3 startPos){
		
		int degreesToComplete = 360;
		float radius = 16f;
		float scale = radius/3.2f/3.2f;
		Vector3[] circlePoints = MathUtils.GetPointsOfACircle(degreesToComplete,radius,scale,19);
		for (int i=0;i<circlePoints.Length;i++){
			Vector3 pos = startPos + transform.TransformDirection(circlePoints[i]) + Vector3.up * 5;
			GameObject num = NumberManager.inst.CreateNumber(new Fraction(i-9,1),pos,NumberShape.Sphere,false);
			num.transform.localScale = Vector3.one * 2;
//			if (Mathf.Abs(num.GetComponent<NumberInfo>().fraction.numerator) > 4) num.GetComponent<Rigidbody>().AddForce(Utils.FlattenVector(Random.onUnitSphere) * 500f);
		}


	}
	
}



