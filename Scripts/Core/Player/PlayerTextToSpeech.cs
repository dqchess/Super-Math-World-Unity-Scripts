using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTextToSpeech : MonoBehaviour {

	public static PlayerTextToSpeech inst;
	public UIBooleanSlider slider;
	private UnityWebGLSpeechSynthesis.ISpeechSynthesisPlugin _mSpeechSynthesisPlugin = null;
	private UnityWebGLSpeechSynthesis.SpeechSynthesisUtterance _mSpeechSynthesisUtterance = null;

	IEnumerator Start() {
		slider.onSliderStateChanged.AddListener(SliderStateChanged);

		_mSpeechSynthesisPlugin = UnityWebGLSpeechSynthesis.WebGLSpeechSynthesisPlugin.GetInstance();

				// get singleton instance
		_mSpeechSynthesisPlugin = UnityWebGLSpeechSynthesis.WebGLSpeechSynthesisPlugin.GetInstance();
		if (null == _mSpeechSynthesisPlugin)
		{
			Debug.LogError("WebGL Speech Synthesis Plugin is not set!");
			yield break;
		}

		// wait for proxy to become available
		while (!_mSpeechSynthesisPlugin.IsAvailable())
		{
			yield return null;
		}

		// Get voices from plugin
//			StartCoroutine(GetVoices());

		// Create an instance of SpeechSynthesisUtterance
		// This populates the default utterance (voice)
		_mSpeechSynthesisPlugin.CreateSpeechSynthesisUtterance((utterance) =>
			{
				//Debug.LogFormat("Utterance created: {0}", utterance._mReference);
				_mSpeechSynthesisUtterance = utterance;


			});



			
	}

	public void Speak(string s){
//		Debug.Log("speak!");
		_mSpeechSynthesisPlugin.Cancel(); // stop voice if playing
		_mSpeechSynthesisPlugin.SetText(_mSpeechSynthesisUtterance, s); // set text
		_mSpeechSynthesisPlugin.Speak(_mSpeechSynthesisUtterance); // speak
	}

	void SliderStateChanged(bool f){
		speechEnabled = f;
	}
	public void SetInstance(){
		inst = this;
	}
	public static bool speechEnabled = false;
//	public void TurnSpeechOn(){
//		speechEnabled = true;
//	}
//
//	public void TurnSpeechOff(){
//		speechEnabled = false;
//	}
}
