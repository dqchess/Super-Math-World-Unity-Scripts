using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraResolutionController : MonoBehaviour {

	// Use this for initialization
	int origW = 1024;
	int origH = 576;
	void Start () {
//		origW = 1024l
			
	}
	
	// Update is called once per frame
	void Update () {
		if (SMW_CHEATS.inst.cheatsEnabled && Input.GetKey(KeyCode.LeftControl)){
			if (Input.GetKeyDown(KeyCode.R)){
				SwapResolutionBoth();
			}
			if (Input.GetKeyDown(KeyCode.T)){
				SwapResolutionScreen();
			}
			if (Input.GetKeyDown(KeyCode.Y)){
				SwapResolutionDisplay();
			}
		}
	}




	int swap = 0;
	void SwapResolutionBoth(){
		WebGLComm.inst.Debug("R hit, Swapping both. opts: R,T,Y");
		Display.main.SetRenderingResolution(origW/(2-swap%2),origH/(2-swap%2));
		Screen.SetResolution(origW/(2-swap%2),origH/(2-swap%2),false);
		swap++;
		WebGLComm.inst.Debug("display render w,h:"+Display.main.renderingWidth+","+Display.main.renderingHeight);
		WebGLComm.inst.Debug("display native w,h:"+Display.main.systemWidth+","+Display.main.systemHeight);
		WebGLComm.inst.Debug("Screen w,h:"+Screen.width+","+Screen.height);
		Application.ExternalEval("Swap('"+swap+"');");
	}

	void SwapResolutionScreen(){
		WebGLComm.inst.Debug("T hit, Swapping screen only. opts: R,T,Y");
//		Display.main.SetRenderingResolution(origW/(2-swap%2),origH/(2-swap%2));
		Screen.SetResolution(origW/(2-swap%2),origH/(2-swap%2),false);
		swap++;
		WebGLComm.inst.Debug("display render w,h:"+Display.main.renderingWidth+","+Display.main.renderingHeight);
		WebGLComm.inst.Debug("display native w,h:"+Display.main.systemWidth+","+Display.main.systemHeight);
		WebGLComm.inst.Debug("Screen w,h:"+Screen.width+","+Screen.height);
		Application.ExternalEval("Swap('"+swap+"');");
	}

	void SwapResolutionDisplay(){
		WebGLComm.inst.Debug("U hit, Swapping display only. opts: R,T,Y");
		Display.main.SetRenderingResolution(origW/(2-swap%2),origH/(2-swap%2));
//		Screen.SetResolution(origW/(2-swap%2),origH/(2-swap%2));
		swap++;
		WebGLComm.inst.Debug("display render w,h:"+Display.main.renderingWidth+","+Display.main.renderingHeight);
		WebGLComm.inst.Debug("display native w,h:"+Display.main.systemWidth+","+Display.main.systemHeight);
		WebGLComm.inst.Debug("Screen w,h:"+Screen.width+","+Screen.height);
		Application.ExternalEval("Swap('"+swap+"');");
	}
}
