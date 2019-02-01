using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
	public static GameObject itemBeingDragged;
	Vector3 startPosition;
	Transform startParent;

	#region IBeginDragHandler implementation

	public void OnBeginDrag (PointerEventData eventData)
	{
		itemBeingDragged = gameObject;
//		itemBeingDragged.transform.position += Vector3.forward * 1;
		startPosition = transform.position;
		startParent = transform.parent;
		transform.SetParent(Inventory.inst.draggingObjectParent);
		GetComponent<CanvasGroup>().blocksRaycasts = false;
//		GetComponent<Canvas>().sortingOrder  =2;
	}

	#endregion

	#region IDragHandler implementation

	public void OnDrag (PointerEventData eventData)
	{
//		// commented Debug.Log("inv inst ui cam screenray mousep:"+Inventory.inst.uiCam.ScreenToWorldPoint(Input.mousePosition));
//		// commented Debug.Log("mousepos:"+Input.mousePosition);
		Vector3 p = new Vector3(eventData.position.x,eventData.position.y,-1);
		Vector3 pp = Inventory.inst.uiCam.ScreenToWorldPoint(p);
		Vector3 ppp = new Vector3(pp.x,pp.y,-10);
//		// commented Debug.Log("p:"+p);
		transform.position = ppp; //eventData.position; //new Vector3(eventData.position.x,eventData.position.y,-10);
	}

	#endregion

	#region IEndDragHandler implementation

	public void OnEndDrag (PointerEventData eventData)
	{
		DragEnded();
	}

	public void DragEnded(){
		//		GetComponent<Canvas>().sortingOrder  = 0;
		GetComponent<CanvasGroup>().blocksRaycasts = true;
		if(transform.parent == Inventory.inst.draggingObjectParent){
			transform.position = startPosition;
			transform.SetParent(startParent);
		} else {

		}

		itemBeingDragged = null;
		//		Debug.Log("ended.");
		GameObject item = GetComponent<InventoryItem>().item3d;
		if (item){
			//			Debug.Log("item.");
			foreach(IMyDragEnded c in item.GetComponents(typeof(IMyDragEnded))){
				c.DragEnded(transform.parent.GetComponent<Slot>()); // update wont run on these disabled objects otherwise.
			}
		}


	}
	#endregion



}