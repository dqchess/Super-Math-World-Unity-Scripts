using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIRestrictInputCustom : MonoBehaviour {

	[Header("Retrict Inclusive")]
	public string allowedCharacters = "1234567890,";
	public bool forceToLower = false;
	public bool allowZero = true;
	public bool allowEmpty = false;
	[Header("Exclusive option")]
	public bool allowAllCharactersExcept = false;
	public string disallowedCharacters = "\\";


	public void RestrictInput(InputField inpf){
		if (allowAllCharactersExcept){
//			// commented Debug.Log("inpf:"+inpf.text);
//			return;
			inpf.text = inpf.text.Trim(disallowedCharacters.ToCharArray());
//			// commented Debug.Log("niew;"+inpf.text);
		} else {
			string ret = "";
			string last = ",";
			for (int i=0;i<inpf.text.Length;i++){
				string c = inpf.text[i].ToString();
				if (forceToLower) c = c.ToLower();
				if (allowedCharacters.Contains(c)){
					if ( c != "," || (c == "," && last != ",") ){ // Prevent double , or leading ,
						ret += c;
						last = c;
					}
				}
			}
			if (!allowZero){
				if (MathUtils.IntParse(inpf.text)==0){
					ret = "1";
				}
			}
			if (!allowEmpty){
				if (inpf.text == ""){
					ret = allowedCharacters.ToCharArray()[0].ToString();
				}
			}
//			Debug.Log("allowed:"+ret);
			inpf.text = ret;
		}
	}

	public void RestrictFinalInput(InputField inpf){
		if (allowAllCharactersExcept){
			//			// commented Debug.Log("inpf:"+inpf.text);
			inpf.text = inpf.text.Trim(disallowedCharacters.ToCharArray());
		} else if (!allowZero){
			if (MathUtils.IntParse(inpf.text)==0){
				inpf.text = "1";
			}
		}
		if (!allowEmpty){
			if (inpf.text == ""){
				if (allowedCharacters.Length > 0){
					inpf.text = allowedCharacters.ToCharArray()[0].ToString();
				}
			}
		}
		if (inpf.text.Length == 0) return;
		while (inpf.text[inpf.text.Length-1].ToString() == ",") { // Don't allow comma at end
			inpf.text = inpf.text.TrimEnd(inpf.text[inpf.text.Length - 1]);
			if (inpf.text.Length == 0) return;
		}
	}
}
