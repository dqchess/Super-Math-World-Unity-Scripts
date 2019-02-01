using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CamWallManagerAlwaysDetect : MonoBehaviour {
	// This helps CamWallManager identify exceptions, such as meshes which have no renderer but are substituting for a rendered mesh, so that camwallmgr still works and moves cam for thsi collider.
}
