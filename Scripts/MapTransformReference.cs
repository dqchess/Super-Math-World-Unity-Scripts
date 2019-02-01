using UnityEngine;
using System.Collections;

public class MapTransformReference : MonoBehaviour {

	// This is needed because we like to define our maps in only one place, e.g. the "Maps List" item in the LEvelEditor selection GUI for
	// selecting a map. However, setting the argument of the button to switch the map is not accessible,
	// Therefore if we only want to change the map in one place we change it here.
	// The editor script that automatically populates the MapManager.maps object grabs the map transform from this variable,
	// And we also set the map button argument to this variable so that the user can select this map by the "Map List" map select menu.

	public Transform map;
}
