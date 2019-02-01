using UnityEngine;
using System.Collections;

public class NumberHoopEnergyBall : NumberHoop {

	override public void UseHoop(GameObject obj, RecordPosition record, bool direction){
		base.UseHoop(obj,record,direction);
		if (obj.GetComponent<NumberInfo>()){
			if (!direction){
				if (obj.GetComponent<EnergyBall>()){
					obj.GetComponent<EnergyBall>().RemoveEnergyBallProperties();
				}
			} else {
				GetComponent<AudioSource>().Play();
				if (!obj.GetComponent<EnergyBall>()){
					obj.AddComponent<EnergyBall>();
				}
				obj.GetComponent<EnergyBall>().AddEnergyBallProperties();
			}
			
		}
	}
}
