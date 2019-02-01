using UnityEngine;
using System.Collections;

public class EnergyBall : MonoBehaviour {

	public GameObject energySprite;
	public GameObject energyParticles;
	Transform digitsParent;
	void Start(){
		
	}

	public void RemoveEnergyBallProperties(){
		
		Destroy(energyParticles);
		Destroy(this);
		if (!	digitsParent) SetDigitsParent();
		if (digitsParent.GetComponent<AlwaysFacePlayer>()){
			Destroy(digitsParent.GetComponent<AlwaysFacePlayer>());
			digitsParent.gameObject.AddComponent<SometimesFacePlayer>();
		}
	}

	public void AddEnergyBallProperties(){
		if (!digitsParent) 	SetDigitsParent();
		if (!energyParticles){
			energyParticles = (GameObject)Instantiate(NumberManager.inst.energyBallParticlesPrefab);
			energyParticles.transform.parent = transform;
			energyParticles.transform.localPosition = Vector3.zero;
		}
		if (digitsParent.GetComponent<SometimesFacePlayer>()){
			Destroy(digitsParent.GetComponent<SometimesFacePlayer>());
			AlwaysFacePlayer af = digitsParent.gameObject.AddComponent<AlwaysFacePlayer>();
			af.framesToSkip = 0;
		}
	}

	public void SetDigitsParent(){
		digitsParent = GetComponent<NumberInfo>().texts[0].denominator.transform.parent;


	}

//	public void OnNumberChanged(){
////		// commented Debug.Log("on number changed energy ball");
//		EnergyBallHolder hold = GetComponentInParent<EnergyBallHolder>();
//		if (hold){
//			hold.OnNumberChanged();
//		}
////		i	f (GetComponent<NumberInfo>().fraction.numerator > 0){
////			energySprite.GetComponent<SpriteRenderer>().color = Color.white;
//////			// commented Debug.Log("white");
////		}else {
////			energySprite.GetComponent<SpriteRenderer>().color = new Color(0.3f,0.3f,0.3f,1);
////		}
//	}
}
