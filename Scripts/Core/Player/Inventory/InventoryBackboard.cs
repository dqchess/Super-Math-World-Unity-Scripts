using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;



public class InventoryBackboard : MonoBehaviour, IDropHandler {


	#region IDropHandler implementation
	public void OnDrop (PointerEventData eventData)
	{

		AnalyticsManager.inst.RecordEvent(AnalyticsManager.Keys.droppedInventoryItem,1);
		GameObject draggedItem3d = DragHandler.itemBeingDragged.GetComponent<InventoryItem>().item3d;
		Vector3 dropPosition = Player.inst.transform.position + Player.inst.transform.forward * 2 + Vector3.up * 5 + Player.inst.transform.right * (Input.mousePosition.x - Screen.width/2f) / Screen.width * 25f;

		



		Destroy(DragHandler.itemBeingDragged); // we dropped it so lose the icon

		ExecuteEvents.ExecuteHierarchy<IHasChanged>(gameObject,null,(x,y) => x.HasChanged ()); // prevent the item from returning to original slot maybe?

		// place in front


		draggedItem3d.SetActive(true);
		draggedItem3d.transform.position = dropPosition;
		Collider c = draggedItem3d.GetComponent<Collider>();
		if (c){
			c.enabled = true;
			c.isTrigger = false;
		}
		Rigidbody rb = draggedItem3d.GetComponent<Rigidbody>();
		if (rb && !rb.isKinematic){
			rb.useGravity = true;
			rb.velocity = Vector3.zero;
		}
		if (draggedItem3d.GetComponent<Gadget>()){
			draggedItem3d.AddComponent<Rigidbody>();
		}
		Inventory.inst.UpdateBeltSelection(true);
		foreach(IMyPlayerDropped drop in draggedItem3d.GetComponents<IMyPlayerDropped>()){
			drop.PlayerDropped();
		}
		foreach(IMyDragEnded m in draggedItem3d.GetComponents(typeof(IMyDragEnded))){
			m.DragEnded(transform.parent.GetComponent<Slot>()); // update wont run on these disabled objects otherwise.
		}
	}

	#endregion
}