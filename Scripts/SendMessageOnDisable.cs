using UnityEngine;
using System.Collections;

public class SendMessageOnDisable : MonoBehaviour {

	public GameObject objToSend;
	public string messageToSend;

	void OnEnable(){
		if (!gameObject.activeSelf) SendNow();
	}
	void OnDisable(){
		if (!gameObject.activeSelf) SendNow();
	}

	void SendNow(){
		objToSend.SendMessage(messageToSend,SendMessageOptions.DontRequireReceiver);
	}


}
