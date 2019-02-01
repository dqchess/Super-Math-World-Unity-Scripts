using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UIValueCommAnimalRules : MonoBehaviour {


	public AnimalType type;
	public Dropdown preference;
	public InputField preferenceInt;

	public Text animalName;
	public Transform animalImages;
	public UIBooleanSlider sliderToggle;
	LevelBuilderUIButton currentButton;
	AnimalType currentAnimalType;
	string xText = "";
	public static UIValueCommAnimalRules inst;

	public void SetInstance(){
		inst = this;
	}

	public void OnEnable(){
		BeginModifyingAnimal();
	}

	void Start(){
		sliderToggle.sliderToggled += SliderToggle;

	}

	void SliderToggle(bool sliderValue){
//		// commented Debug.Log("updating rules from slidertog");
		UpdateRules();
	}

	List<string> optsList = new List<string>();
	List<string> optsListWithSpaces = new List<string>();


	bool suppressUpdateRules = false;
	public void BeginModifyingAnimal(){
		suppressUpdateRules = true;
//		// commented Debug.Log("modifying;"+button.gameObject.name);
		if (!LevelBuilder.inst.currentPiece || !LevelBuilder.inst.currentPiece.GetComponentInChildren<Animal>()){
			// commented Debug.LogError("No animal selected during animal rules dialogue begin");
			return;
		}

		preference.options.Clear();
		int i =0;
		int selectedIndex = -1;

		SetCurrentAnimalType(LevelBuilder.inst.currentPiece.GetComponentInChildren<Animal>().type);
		bool cannibalism = false;
		optsList.Clear();
		optsListWithSpaces.Clear();
//		string debug_optslist = "";

		foreach(AnimalTargetPreference atp in AnimalType.GetValues(typeof(AnimalTargetPreference))){

			string val = atp.ToString();
			optsList.Add(val);
//			debug_optslist += val;
			val = Utils.AddSpacesToSentence(val);
			optsListWithSpaces.Add(val);

			foreach (AnimalTargetPreferenceRel atpr in AnimalBehaviorManager.inst.preferences){
				
				if (atpr.type == currentAnimalType && atpr.preference == atp) {

					cannibalism = atpr.cannibalize;
					if (atpr.preferenceInt.ToString().Length > 0) preferenceInt.text = atpr.preferenceInt.ToString(); // This line causes OnValueChanged to fire on Preference Int, which we want to suppress.
					// Suppress by boolean!
//					// commented Debug.Log("atpr int;"+atpr.preferenceInt+", for:"+atpr.type);
					selectedIndex = i;
//					selectedPreference = atpr;
//					// commented Debug.Log("?");
				} else {
//					// commented Debug.Log("atpr type:"+atpr.type.ToString()+", curantype;"+currentAnimalType+",pref:"+atpr.preference+", atp"+atp.ToString());
				}
			}
//			// commented Debug.Log("val;"+val);
			i++;

		}

//		// commented Debug.Log("Finished optslits for:"+button.levelPiecePrefab.name+": list: "+debug_optslist);
		preference.AddOptions(optsListWithSpaces); 
		preference.value = selectedIndex;
		SetPreferenceIntActive();
//		// commented Debug.Log("set int:"+preferenceInt.text);
		if (sliderToggle.GetSliderValue() != cannibalism) sliderToggle.ToggleSlider();


		animalName.text = currentAnimalType.ToString();
		foreach(Transform t in animalImages){
			t.GetComponent<Image>().sprite = LevelBuilder.inst.currentPiece.GetComponent<UserEditableObject>().icon;
		}


//		// commented Debug.Log("finished;"+button.gameObject.name);
//		// commented Debug.Log(" updating rules from enabled");
		suppressUpdateRules = false;
		UpdateRules();

	}
		

	void SetPreferenceIntActive(){
		int val = preference.GetComponent<Dropdown>().value;
		if (
			val == AnimalBehaviorManager.inst.GetAnimalTargetPreferenceIndexFromPreferenceEnum(AnimalTargetPreference.FactorsOfX) 
			|| val == AnimalBehaviorManager.inst.GetAnimalTargetPreferenceIndexFromPreferenceEnum(AnimalTargetPreference.MultiplesOfX)
			|| val == AnimalBehaviorManager.inst.GetAnimalTargetPreferenceIndexFromPreferenceEnum(AnimalTargetPreference.LessThanX)
			|| val == AnimalBehaviorManager.inst.GetAnimalTargetPreferenceIndexFromPreferenceEnum(AnimalTargetPreference.GreaterThanX)
			|| val == AnimalBehaviorManager.inst.GetAnimalTargetPreferenceIndexFromPreferenceEnum(AnimalTargetPreference.OnlyX)) {
			preferenceInt.gameObject.SetActive(true);
			if (preferenceInt.text == ""){
				preferenceInt.text = "1";
			}
			xText = " where X = "+preferenceInt.text;
		} else {
			preferenceInt.gameObject.SetActive(false);
			xText = "";
		}
	}






	public void SetCurrentAnimalType(AnimalType t){
//		// commented Debug.Log("cur animal type:"+t);
		currentAnimalType = t;
	}

	public void UpdateRules(){
		if (suppressUpdateRules) return;
//		// commented Debug.Log("updating rules, curtype;"+currentAnimalType);
//		// commented Debug.Log("opts list as:"+string.Join(",",optsList.ToArray()));
		SetPreferenceIntActive();
		foreach(AnimalTargetPreferenceRel atpr in AnimalBehaviorManager.inst.preferences){
			if (atpr.type == currentAnimalType){
				atpr.cannibalize = sliderToggle.GetSliderValue();
				foreach(AnimalTargetPreference atp in AnimalType.GetValues(typeof(AnimalTargetPreference))){
//					// commented Debug.Log("pref valu:"+preference.value+" for "+atp+" , "+atpr.type+". OPtslist ct:"+optsList.Count);
					// Unfortunately, UpdateRules is called before BeginModifyingAnimal because of the "OnValueChanged" being fired when the Integer PReference field is enabled in the scene. 
					// Ideally, update rules would never fire before the animal type is set correctly. 
					// Instead let's just check for length?
//					if (atp.ToString() == currentAnimalType == 
					if (atp.ToString() == optsList[preference.value]) {
						atpr.preference = atp;
//						// commented Debug.Log("set pref:"+atp+" for animal "+currentAnimalType.ToString());
					}
				}
				if (preferenceInt.text == "") preferenceInt.text = "1"; // ugly dry
				atpr.preferenceInt = int.Parse(preferenceInt.text);
//				atpr.preference = AnimalTargetPreference(3); // typeof(AnimalTargetPreference). [ AnimalBehaviorManager.inst.target
			}
		}
//		currentButton.extraDescriptionText.text = currentAnimalType.ToString() + " will eat " + optsListWithSpaces[preference.value] + " " + xText;
//		// commented Debug.Log("set extra desription text to:"+ currentButton.extraDescriptionText.text);
	}
}
