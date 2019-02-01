using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DetectMouseClickUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IEndDragHandler, IBeginDragHandler, IDragHandler {


	public UIInputType type;
//	public string type;

	public void OnPointerDown (PointerEventData eventData)
	{
//		// commented Debug.Log("pointerdown");
		if (type == UIInputType.MouseDown) {
//			// commented Debug.Log("mousedown");
			TutorialManager.inst.currentTutorial.UserFollowedInstruction(gameObject);
		}
	}


	public void OnBeginDrag(PointerEventData eventData){}
	public void OnDrag(PointerEventData eventData){}
	public void OnEndDrag (PointerEventData eventData)
	{
//		// commented Debug.Log("end drag");
//		if (type == UIInputType.Drag) FindObjectOfType<UIInstructionsHelper>().UserFollowedInstruction(gameObject);
	}

	public void OnPointerUp (PointerEventData eventData) {
//		// commented Debug.Log("up here.");
		if (type == UIInputType.MouseUp) {
//			// commented Debug.Log("pointer up here.");
			List<RaycastResult> objectsHit = LevelBuilder.inst.GetUIObjectsUnderCursor();
			if (objectsHit.Count > 0){
//				// commented Debug.Log("obj hit >0, first one was"+objectsHit[0].gameObject.name);
				foreach(RaycastResult rr in objectsHit){
//					// commented Debug.Log("hit:"+rr.gameObject.name);
					if (rr.gameObject.GetComponent<DetectMouseClickUI>() // Only pass a successful instruction followed event if the mouse up happened while cursor was in fact still on this object.
						|| rr.gameObject.transform.parent.GetComponent<DetectMouseClickUI>() // awkwardly search for parents..
						|| rr.gameObject.transform.parent.parent.GetComponent<DetectMouseClickUI>()){ // two deep. ugh. Because buttons DO receive clicks but DONT pass this raycast test.
						// This is done because if there isn't this raycast check, the user can "mouse up" over somewhere that ISNT the tutorial target button and the button itself will NOT fire
						// and open the right window, but the tutorial will THINK it has fired because it DID receive a mouseup event. Solved this by checking via raycast if we mouse-upd over the actual correct element.
						// Better way: Pointer up over this object?! But the button doesn't receive raycasts, its children do! (no image over button, images are all in children).
						// TODO a better way to check this is to.. what.. 
//						// commented Debug.Log("obj hit had the thing!");
						TutorialManager.inst.currentTutorial.UserFollowedInstruction(gameObject);
	//					FindObjectOfType<UIInstructionsHelper>().UserFollowedInstruction(gameObject); // not good, TODO make this call TutorialManager.inst.currentInstructionSet
						return;
					}
				}
			}
		} 
//		else if (type == UIInputType.MouseUpAnywhere){
////			// commented Debug.Log("pointer up anywher");
//			TutorialManager.inst.currentTutorial.UserFollowedInstruction(gameObject);
//		}
	}
}