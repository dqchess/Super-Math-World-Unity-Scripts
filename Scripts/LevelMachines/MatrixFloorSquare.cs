using UnityEngine;
using System.Collections;

public class MatrixFloorSquare : MonoBehaviour {

	public int positionX;
	public int positionZ;
	public bool planted = false;
	public Color targetColor;
	public bool blinking = false;
	public int flowerIndex;
	GameObject flower;
	LevelMachineMatrixFloor matrixFloor;
	// Use this for initialization
	void Start () {
	
	}

	public void Init(LevelMachineMatrixFloor lmf){
		matrixFloor = lmf;
		name = positionX + ","+positionZ;
	}

	public void GrowFlower(int i){
		// The second iteration of plant life.
		// It grows a number
		StartCoroutine(GrowFlowerE(i));
	}
 	IEnumerator GrowFlowerE(int i){

		// We yield in a while loop because we want to let the flower finish growing first (never more than 3 seconds?)
		yield return new WaitForSeconds(.1f);
		while (!finishedGrowing){
			yield return new WaitForSeconds(.1f);
		}
		// Finally, grow the flower after a random number of seconds and "pop" sound
		yield return new WaitForSeconds(Random.Range(0f,4f));
		AudioManager.inst.PlayCrystalThump1(transform.position);
		EffectsManager.inst.CreateSmallPurpleExplosion(transform.position,2f,1f);

		GameObject newFlower = NumberManager.inst.CreateNumber(new Fraction(i,1),flower.transform.position);
		newFlower.transform.localScale *= 1.1f;
		Destroy(flower);
		flower = newFlower;
		flower.transform.parent = transform;
		flower.GetComponent<Rigidbody>().isKinematic = true;

		flower.AddComponent<SetScaleOnPlayerPickup>(); // because the floor tile is scale 3, the number is actually 1/3 which is awkward on pickup. get around this by force scale on player interaction
//		flower.AddComponent<UnparentOnPlayerPickup>(); // don't want to stay a parent of this object once player touches it.
	}

	public bool HasNumberFlower(){
		// returns true if we "have" a number flower . Returns false if flower is never planted, not a number, or was a number but is no longer our child (picked up by player).
		return flower && flower.activeSelf && flower.GetComponent<NumberInfo>() && flower.transform.parent == transform && !(flower.GetComponent<Rigidbody>() && !flower.GetComponent<Rigidbody>().isKinematic);
	}


	public void Plant(int flowerPrefabIndex, Color c){
		// The first iteration of plant life. 
		// It grows a number
		blinking = false;
		targetColor = c;
		flower = (GameObject)Instantiate(FlowerManager.inst.flowerPrefabs[flowerPrefabIndex]);
		flowerIndex = flowerPrefabIndex;
		flower.transform.position = transform.position;
		flower.transform.parent = transform;
		planted = true;
//		Debug.Log("planted true on:"+name);
		plantSize = Random.Range(.4f,.7f);
		stem = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
		stem.transform.position = transform.position;
		stem.transform.parent = transform;
		flower.GetComponent<Renderer>().material.color = targetColor + Utils.RandomColorDelta(-.05f,.1f);
		stem.GetComponent<Renderer>().material.color = new Color(.3f,.35f,.2f,1) + Utils.RandomColorDelta(0,.05f);;
	}

	float plantSize = 0;
	bool finishedGrowing = false;
	float growTimer = 0;
	float finishedGrowingTimer = 0;
	GameObject stem;

	void Update(){
		if (finishedGrowingTimer > 3) {
			return;
		}
		if (planted && !finishedGrowing && flower){
			
			float delta = Random.Range(1,1.6f) * Time.deltaTime * plantSize; 
			flower.transform.position += Vector3.up * delta;
			flower.transform.localScale += Vector3.one * delta*.03f;
			flower.transform.Rotate(Vector3.up * Random.Range(1,1.2f),Space.World);

			stem.transform.position = transform.position + Vector3.up * (flower.transform.position.y-transform.position.y)/2f;
			stem.transform.localScale = new Vector3(.3f,(stem.transform.position.y-transform.position.y),.3f)/transform.localScale.x;
			growTimer += Time.deltaTime;
			if (growTimer > 5){
				finishedGrowing = true;
			}
		}
		if (finishedGrowing){
			finishedGrowingTimer += Time.deltaTime;
		}
		bool coloring = true;
		if (Vector4.SqrMagnitude(GetComponent<Renderer>().material.color-targetColor) < .01f){
			coloring = false;
		}
		if (coloring){
			Renderer r = GetComponent<Renderer>();
			Color lerpColor; 
			float colorTransitionSpeed =10f;

			lerpColor = Color.Lerp(r.material.color,targetColor,Time.deltaTime * colorTransitionSpeed);
			if (blinking){
				float alphaSpeed = 5.5f;
				//				float blinkInterval = 0.5f;
				//				if (Mathf.FloorToInt(Time.time/blinkInterval) % 2 ==0){
				float alpha = 0.5f + ( Mathf.Sin(Time.time * alphaSpeed)  + 1f) / 4f; // sin between some 0.5-1
				Color tc = new Color(matrixFloor.floorColor.r,matrixFloor.floorColor.g,matrixFloor.floorColor.b,alpha);
				lerpColor = Color.Lerp(r.material.color,tc,Time.deltaTime * colorTransitionSpeed);
				//				} 
			}
			r.material.color = lerpColor;
		}
	}

	public void ResetColor(Color c){
		blinking = false;
		targetColor = c;
	}

	public void Die(){
		if (stem) {
			stem.transform.parent = null;
			stem.AddComponent<Rigidbody>();
			TimedObjectDestructor tod = stem.AddComponent<TimedObjectDestructor>();
			stem.GetComponent<Rigidbody>().AddForce(Random.insideUnitSphere * 500f);
			tod.DestroyNow(Random.Range(5f,10f));
		}
//		Debug.Log("I dieded.:"+name);
		Destroy(gameObject);
	}
}
