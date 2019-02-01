using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;

public class UIBooleanSlider : MonoBehaviour {

	public delegate void SliderToggled(bool sliderValue);
	public  SliderToggled sliderToggled;
	public Transform slider;
	public Transform sliderOnPosition;
	public Transform sliderOffPosition;
	Transform target;
	public Image image; 
	bool moving = false;
	public bool sliderValue = false;
	Color targetColor = Color.gray;

	public bool GetSliderValue(){
		return sliderValue;
	}

//	public void ToggleSliderWithCallback(GameObject callbackObj=null){
//		ToggleSlider();
//		if (callbackObj) callbackObj.SendMessage("SliderToggleCallback",SendMessageOptions.DontRequireReceiver);
//	}

	public void ToggleSlider(){
		if (moving) return;
		if (sliderValue == true){
			TurnOff();
		} else {
			TurnOn();
		}
		if (sliderToggled != null) sliderToggled(sliderValue);
	}

	public void TurnOn(){
		sliderValue = true;
		target = sliderOnPosition;
		targetColor = Color.green;
		moving = true;
		onSliderStateChanged.Invoke(true);
	}

	public void TurnOff(){
		sliderValue = false;
		target = sliderOffPosition;
		targetColor = Color.gray;
		moving = true;
		onSliderStateChanged.Invoke(false);
	}

	public class BooleanEvent : UnityEvent<bool> {} //empty class; just needs to exist

	public BooleanEvent onSliderStateChanged = new BooleanEvent();



	// Update is called once per frame
	void Update () {
		float dt = Time.unscaledDeltaTime;
		if (moving){
			float fadeSpeed = 13;
			float moveSpeed = 15;
			image.color = Color.Lerp(image.color,targetColor,dt * fadeSpeed);
			slider.position = Vector3.Lerp(slider.position,target.position,dt * moveSpeed);
			if (Vector3.Magnitude(slider.position - target.position) < .1f){
				image.color = targetColor;
				slider.position = target.position;
				moving = false;
			}
//		if 
		}
	}
}
