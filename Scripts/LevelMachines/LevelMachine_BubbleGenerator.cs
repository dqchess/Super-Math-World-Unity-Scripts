using UnityEngine;
using System.Collections;

public class LevelMachine_BubbleGenerator : MonoBehaviour {

	public GameObject gears;
	public float radius = 60;


	void Start(){
		foreach(NumberWallCreatorSquare nwc in GetComponentsInChildren<NumberWallCreatorSquare>()){
			FindObjectOfType<SceneTools>().DropNumberWall(nwc.gameObject);
		}
		FindObjectOfType<SceneTools>().DropObjectToTerrain(gears);
	}

	float lightningTimerFX = 0;
	void Update(){
		lightningTimerFX -= Time.deltaTime;
		if (lightningTimerFX < 0){
			lightningTimerFX = Random.Range(.5f,1f);
			GameObject randomG = new GameObject();
			randomG.transform.position = transform.position + Random.insideUnitSphere*radius;
			TimedObjectDestructor tod = randomG.AddComponent<TimedObjectDestructor>(); // what is default seconds.. and why..ugh
			float randomDuration = Random.Range (1.2f,1.5f);
			SMW_GF.inst.CreateLightning(transform,randomG.transform,randomDuration);
		}
	}

	
}
