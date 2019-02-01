using UnityEngine;
using System.Collections;

public class AttackPlayerWithLightning : MonoBehaviour {
	
	
	public float range = 40f;
	float lastFireTime = 0;
	float fireDelay = .3f;
	Material graphics;
	// Use this for initialization
	Color origColor;


	bool gotMesh = false;
	
	void Start () {
		if (transform.Find("mesh")) gotMesh = true; 
		else {	
			// commented Debug.Log("didn't find mesh. mynameL");
			name="WTF";
		}
		
		if (gotMesh) {
			graphics = transform.Find("mesh").GetComponent<Renderer>().material;
			
//			origVig = graphics.GetColor("_VignetteColor");
//			origBase = graphics.GetColor("_BaseColor");
		} 
		else {
			if (transform.Find("CubeChildMesh")) gotMesh = true;
			if (gotMesh){
//				// commented Debug.Log("It's ok, it's ok we got the cube child mesh instead. Stupid cubes don't roll, tho. I didn't sign up for this BS. Check 'Sphere' as the type from NumberGenerator to let me move around.");
				graphics = transform.Find("CubeChildMesh").GetComponent<Renderer>().material;
				
				// dry
				origColor = graphics.color;
//				origBase = graphics.GetColor("_BaseColor");
				
			}
		}
		
		
	}
	
	// Update is called once per frame
	void Update () {
		if (gotMesh) { // TODO: ever lose mesh?
			
			if (Time.time > lastFireTime + fireDelay &&	PlayerInRange()){
				StartCoroutine(GlowRed());
				lastFireTime = Time.time;
			
			}
		}
		
		
	}
	
	bool PlayerInRange(){
		return Vector3.Magnitude(Player.inst.transform.position-transform.position)<range;
	}
	
	IEnumerator GlowRed(){
		AudioManager.inst.PlayLowElectricWarmUp(transform.position);
//		yield return false;
		
		
		
		if (gotMesh){
		
			float timeStarted = Time.time;
			float glowTime = .5f;
			while (Time.time < timeStarted + glowTime){
				
				graphics.color += new Color(.02f,0,0,.02f);

				yield return null;
			}
	
			if (PlayerInRange()) {
				AudioManager.inst.PlayRandomElectricitySound(transform.position);
				SMW_GF.inst.CreateLightning(transform,Player.inst.transform,fireDelay);
				StartCoroutine(ResetPlayerE()); // needs to be delayed .2f so we can see the lightning hit us.
			}
			
			graphics.color = origColor;

		}
		
		
	}
	
	IEnumerator ResetPlayerE (){
		yield return new WaitForSeconds(fireDelay);
//		GlobalVars.inst.gf.FadeInWhitePlane();
//		GlobalVars.inst.gf.KillPlayer();
	}
	
	void ReturningToPool() {
		Destroy(this);	
	}
}
