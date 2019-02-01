using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIFlipBool : MonoBehaviour {

	public void FlipBool(GameObject o){
		o.SetActive(!o.activeSelf);
	}
}
