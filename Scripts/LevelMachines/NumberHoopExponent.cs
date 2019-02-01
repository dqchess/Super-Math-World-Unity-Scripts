using UnityEngine;
using System.Collections;

public class NumberHoopExponent : NumberHoop {


	override public void UseHoop(GameObject obj, RecordPosition record, bool direction){
		base.UseHoop(obj,record,direction);
		GetComponent<AudioSource>().Play();
		if (obj.GetComponent<NumberInfo>()){
			AlgebraInfo ai = obj.GetComponent<AlgebraInfo>();
			if (!direction){
				if (ai){
					ai.SetExponent(ai.exponent-1);
					if (ai.exponent == 0) {
						Destroy(ai);
						return;
					}
				}
			} else {
				if (!ai) {
					ai = obj.AddComponent<AlgebraInfo>();
					ai.SetExponent(1);
				} else {
					ai.SetExponent(ai.exponent+1);
				}
			}
		}
	}
}
