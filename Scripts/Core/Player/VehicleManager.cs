using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VehicleManager : MonoBehaviour {
	public static VehicleManager inst;
	public void SetInstance(){
		inst = this; // soo dry by now.. superclass these already!
	}

	void Start(){
		GameManager.inst.onLevelWasRestartedDelegate += GameRestarted;
		LevelBuilder.inst.levelBuilderOpenedDelegate += GameRestarted;
	}
	public List<Vehicle> vehicles = new List<Vehicle>();

	void GameRestarted(){
		vehicles.Clear();
//		Debug.Log("game restarted. veh list:"+vehicles.Count);
	}
}
