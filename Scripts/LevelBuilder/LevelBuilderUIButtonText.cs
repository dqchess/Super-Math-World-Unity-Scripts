using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine.UI;

public class LevelBuilderUIButtonText : MonoBehaviour {

	public float min = -99;
	public float max = 99;
	public bool allow360 = false;
//	public bool angle = false;




	public void AddToIntegerText (int amount) {
		int newVal = (MathUtils.IntParse(GetComponent<Text>().text) + amount);
		newVal = Mathf.Clamp(newVal,(int)min,(int)max);
		GetComponent<Text>().text = newVal.ToString();
	}

	public void AddToDegreesText (int amount) {
		
		string text = GetComponent<Text>().text;
		int newVal = (MathUtils.IntParse (text) + amount);
//		int newVal = 0;
//		if (angle) {
		if (newVal < 0) newVal += 360;
		newVal %= 360;
		if (allow360){
			if (newVal == 0){
				newVal = 360;
			}
		}
//		}
		newVal = Mathf.Clamp(newVal,(int)min,(int)max);
		GetComponent<Text>().text = newVal.ToString() + "°";
	}

	public void AddToFloatText(float am){ // amount is 10x
		int amount = Mathf.RoundToInt(am*10);
		int newVal = (Mathf.RoundToInt(float.Parse (GetComponent<Text>().text)*10) + amount);
		newVal = Mathf.Clamp(newVal,(int)(min*10),(int)(max*10));
		decimal newValDec = new decimal(newVal /10f);
		GetComponent<Text>().text = newValDec.ToString();
	}
}
