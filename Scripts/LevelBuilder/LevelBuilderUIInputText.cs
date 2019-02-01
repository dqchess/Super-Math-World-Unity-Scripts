using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine.UI;

public class LevelBuilderUIInputText : MonoBehaviour {

	public float min = -99;
	public float max = 99;
	public bool allowZero = false;
//	public bool angle = false;



	public void AddToIntegerText (int amount) {
		int oldval = int.Parse (GetComponent<InputField>().text);
		int newVal = (int.Parse (GetComponent<InputField>().text) + amount);
		newVal = Mathf.Clamp(newVal,(int)min,(int)max);
//		// commented Debug.Log("newval:"+newVal);

		// Handle "skipping" of zero in cases where user pressed "increase" and value was already -1, or vice versa for decrease / 1
		// since we don't want the text to be able to be zero.
		if (newVal == 0 && !allowZero){
			if (oldval == 1){
				if (-1 >= min) {
					newVal = -1;
//					// commented Debug.Log("set newval:"+newVal);
				} else {
					newVal = 1;
//					// commented Debug.Log("set newval:"+newVal);
				}
			} else if (oldval == -1) {
				if (1 <= max){
					newVal = 1;
				} else {
					newVal = -1;
				}
			}
		} 
		GetComponent<InputField>().text = newVal.ToString();
	}

	public void AddToDegreesText (int amount) {
		
		string text = Regex.Replace(GetComponent<InputField>().text, "[^0-9]", "");
		int newVal = (int.Parse (text) + amount);
//		if (angle) {
		if (newVal < 0) newVal += 360;
		newVal %= 360;
//		}
		newVal = Mathf.Clamp(newVal,(int)min,(int)max);
		GetComponent<InputField>().text = newVal.ToString() + "°";
	}

	public void AddToFloatText(float am){ // amount is 10x
		int amount = Mathf.RoundToInt(am*10);
		int newVal = (Mathf.RoundToInt(float.Parse (GetComponent<InputField>().text)*10) + amount);
		newVal = Mathf.Clamp(newVal,(int)(min*10),(int)(max*10));
		decimal newValDec = new decimal(newVal /10f);
		GetComponent<InputField>().text = newValDec.ToString();


	}
}
