using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum AnimalTargetPreference { // Need to use Preference Index because Dropdown value (where user selects this) is reduced to int before passing. Ugh
	Nothing,
	GreaterThanItself,
	LessThanItself,
	GreaterThanX,
	LessThanX,
	PrimesOnly,
	FactorsOfItself,
	MultiplesOfItself,
	FactorsOfX,
	MultiplesOfX,
	OnlyX,
	Everything,
	EverythingButItsOwnNumber
}

public enum AnimalType {
	Bee,
	Bird,
	Sheep,
	Fish,
	Frog,
	Toad,
	Pollywog,
	Croaker,
	FrogQueen
}

[System.Serializable]
public class AnimalTargetPreferenceRel {
	public AnimalType type;
	public AnimalTargetPreference preference;
	public int preferenceInt; // Only relevant for "X" target types
	public bool cannibalize;
}

public class AnimalBehaviorManager : MonoBehaviour {

	/*
	 * Animals only prefer certain types of targets, which are selectable by the creator.
	 * For example the creator may decide that BIRDS only eat PRIME numbers.
	 * That information is stored here when the creator selects it, so that when the player
	 * interacts with the game the birds will look to this script to validate their preferred targets.
	 * The result is that when birds interact in the game they follow the target preference rules listed here.
	 * */

	[SerializeField] public AnimalTargetPreferenceRel[] preferences;
	public static AnimalBehaviorManager inst;
	public void SetInstance(){
		inst = this;
		
	}



	public int GetAnimalTargetPreferenceIndexFromPreferenceEnum(AnimalTargetPreference pref){
//		// commented Debug.Log("pref string:"+pref.ToString());

		List<AnimalTargetPreference> vals = System.Enum.GetValues(typeof(AnimalTargetPreference)).Cast<AnimalTargetPreference>().ToList();
		for (int i=0; i<vals.Count; i++){
			if (pref == vals[i]) return i;
		}
		// commented Debug.Log("this shouldn't happen.");
		return -1;

//		switch (pref){
//		case AnimalTargetPreference.PrimesOnly: return 0; break;
//		case AnimalTargetPreference.FactorsOfMe: return 1; break;
//		case AnimalTargetPreference.MultiplesOfMe: return 2; break;
//		case AnimalTargetPreference.FactorsOfX: return 3; break;
//		case AnimalTargetPreference.MultiplesOfX: return 4; break;
//		case AnimalTargetPreference.OnlyX: return 5; break;
//		default: return 0; break;
//		}
	}

	public AnimalTargetPreferenceRel GetAnimalTargetPreferenceRelFromAnimalType(AnimalType type){
		foreach(AnimalTargetPreferenceRel tpr in preferences){
			if (tpr.type == type){
//				Debug.Log("TPR returned type:"+type+", pref was:"+tpr.preference);
				return tpr;
			}
		}
		return null;
	}



	bool TargetAndAnimalAreSameType(Animal an, Transform target){
		if (target.GetComponent(an.GetType())) {
//			// commented Debug.Log("can't eat beacuse same:"+an.name+","+target.name+". antype:"+an.GetType());
			return true; // birds don't target other birds.
		}
//		// commented Debug.Log("diff type, it's ok");
		return false;
	}

	public bool CheckValidTarget(AnimalTargetPreferenceRel tpr, Animal an, NumberInfo targetNi, bool careAboutRigidbodies=true){ // Transform target) {
		if (targetNi.myShape == NumberShape.Tetrahedron || targetNi.myShape == NumberShape.Ghost) { return false; }
		if (careAboutRigidbodies && !targetNi.GetComponent<Animal>()){
			if (targetNi.GetComponent<Rigidbody>()){
				if (targetNi.GetComponent<Rigidbody>().isKinematic) return false;
			} else if (!targetNi.transform.parent){ // if we found a non-rigidbody with no parent ignore it
				return false;
			} else if (!targetNi.transform.parent.GetComponent<LevelMachinePrefab_NumberFlower>()){ // if the parent was a flower, let it be targeted, else return false
				return false;
			}
		}
//		Debug.Log("here1");
		Animal selfCheck = targetNi.GetComponent<Animal>();
		if (selfCheck){
			if (selfCheck == an){
				return false;
			}
		}
		if (!targetNi) return false;
//		Debug.Log("here2");
		Fraction targetFrac;
//		if (targetNi.transform.tag == "Player"){
//			targetFrac = Player.inst.PlayerHoldingNumberInfo();
//		} else {
		targetFrac = targetNi.fraction;
//		}
		if (!tpr.cannibalize && TargetAndAnimalAreSameType(an,targetNi.transform)) {
			return false;
//			Debug.Log("!! here2");
		}
//		Debug.Log("here");
		return CheckValidTarget(tpr, an,targetFrac);
		
	}
	public bool CheckValidTarget(AnimalTargetPreferenceRel tpr, Animal an, Fraction targetFrac){ // Transform target) {
		
		if (targetFrac == null) {  return false; }


		if (!an) return false;


		NumberInfo animalNi = an.GetComponent<NumberInfo>();




//		Debug.Log("hi..");

		bool ret = false;
//		 Debug.Log("ret!");
		switch(tpr.preference){
		case AnimalTargetPreference.Nothing:
			ret = false;
			break;
		case AnimalTargetPreference.Everything:
			ret = true;
			break;
		case AnimalTargetPreference.GreaterThanItself:
			ret = Fraction.Greater(targetFrac,animalNi.fraction);
			break;
		case AnimalTargetPreference.LessThanItself:
			ret = Fraction.Greater(animalNi.fraction,targetFrac);
			break;
		case AnimalTargetPreference.GreaterThanX:
			ret = Fraction.Greater(targetFrac,new Fraction(tpr.preferenceInt,1));
			break;
		case AnimalTargetPreference.LessThanX:
			ret = Fraction.Greater(new Fraction(tpr.preferenceInt,1),targetFrac);
			break;
		case AnimalTargetPreference.PrimesOnly:
			ret = MathUtils.isPrime(targetFrac.numerator);
			break;
		case AnimalTargetPreference.FactorsOfItself: 
			ret = MathUtils.AIsFactorOfB(targetFrac,animalNi.fraction);
			break;
		case AnimalTargetPreference.MultiplesOfItself: 
			ret = MathUtils.AIsFactorOfB(animalNi.fraction,targetFrac);
//			// commented Debug.Log("ret false for "+animalNi.fraction.numerator+" is a factor of "+targetNi.fraction.numerator+".");
			break;
		case AnimalTargetPreference.FactorsOfX: 
			ret = MathUtils.AIsFactorOfB(targetFrac,new Fraction(tpr.preferenceInt,1));
			break;
		case AnimalTargetPreference.MultiplesOfX:
			ret = MathUtils.AIsFactorOfB(new Fraction(tpr.preferenceInt,1),new Fraction(Mathf.Abs(targetFrac.numerator),targetFrac.denominator));
			break;
		case AnimalTargetPreference.OnlyX: 
			ret = Fraction.Equals(targetFrac,new Fraction(tpr.preferenceInt,1));
			break;
		case AnimalTargetPreference.EverythingButItsOwnNumber:
			ret = !Fraction.Equals(targetFrac,animalNi.fraction);
			break;
		default:
			break;
		}
//		Debug.Log("ret final:"+ret);
//		// commented Debug.Log("hmm:"+ret);
//		// commented Debug.Log("Target preference wa:"+tpr.preference.ToString()+", anni:"+animalNi.fraction.numerator+",targtni:"+targetNi.fraction.numerator+"..__result__:"+ret);
		return ret;

	}

	public SimpleJSON.JSONClass GetAnimalRules(){
		SimpleJSON.JSONClass N = new SimpleJSON.JSONClass();
		foreach(AnimalTargetPreferenceRel atpr in preferences){
			string animalType = atpr.type.ToString();
			N[animalType] = new SimpleJSON.JSONClass();
			N[animalType]["cannibalize"].AsBool = atpr.cannibalize;
			N[animalType]["preference"].AsInt = GetAnimalTargetPreferenceIndexFromPreferenceEnum(atpr.preference);
			N[animalType]["preferenceInt"].AsInt = atpr.preferenceInt;
		}
		return N;
	}

	public void SetAnimalRules(SimpleJSON.JSONNode N){
		foreach(AnimalTargetPreferenceRel atpr in preferences){
			string animalType = atpr.type.ToString();
			atpr.cannibalize = N[animalType]["cannibalize"].AsBool;
			atpr.preference =  (AnimalTargetPreference)N[animalType]["preference"].AsInt;
			atpr.preferenceInt = N[animalType]["preferenceInt"].AsInt;
		}
	}

}
