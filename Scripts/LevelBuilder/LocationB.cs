using UnityEngine;
using System.Collections;

public class LocationB : Location {



	public override void OnLevelBuilderObjectPlaced(){
		DestroyDuplicates<LocationB>();
	}
}
