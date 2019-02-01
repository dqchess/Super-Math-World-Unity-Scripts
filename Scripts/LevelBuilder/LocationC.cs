using UnityEngine;
using System.Collections;

public class LocationC : Location {





	public override void OnLevelBuilderObjectPlaced(){
		DestroyDuplicates<LocationC>();
	}



}
