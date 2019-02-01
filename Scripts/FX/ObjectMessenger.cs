using UnityEngine;
using System.Collections;

public class ObjectMessenger : MonoBehaviour {

	public GameObject objToSendMessage;
	public string message;


	virtual public void SendMessage(){
		objToSendMessage.SendMessage(message);
		MascotAnimatorController.inst.PointForwards();
	}
}
