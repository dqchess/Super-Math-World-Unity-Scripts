/*
 This script lets you enable and disable per camera.
 By enabling or disabling the script in the title of the inspector, you can turn fog on or off per camera.
*/
 
private var revertFogState = false;
public var showFog = false;
function OnPreRender () {
	revertFogState = RenderSettings.fog;
	RenderSettings.fog = showFog;
//	Debug.Log("off");
}
 
function OnPostRender () {
	RenderSettings.fog = revertFogState;
//	Debug.Log("on");
}
 
@script AddComponentMenu ("Rendering/Fog Layer")
@script RequireComponent (Camera)