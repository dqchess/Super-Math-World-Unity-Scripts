using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class NumberFaucetHold : NumberFaucet {



	public override void DripFX(){
		if (dripNumber.transform.parent == null){
			dripNumber = null;
			return;
		}

		dripNumber.transform.position = dripStartT.position + (Vector3.up * -dripTime);
		dripTime += Time.deltaTime;
		if (dripTime > maxDripTime){
			gameObject.SendMessage ("OnNumberDrip",dripNumber,SendMessageOptions.DontRequireReceiver);
			dripNumber.transform.position += Random.insideUnitSphere * .4f;

			dripNumber.GetComponent<Rigidbody>().useGravity = false;
			dripNumber.GetComponent<Rigidbody>().velocity = Vector3.zero;
			dripNumber.transform.position += Vector3.up * -1;
			dripNumber.AddComponent<UseGravityOnNumberCombined>();
			dripNumber.AddComponent<UseGravityOnPlayerPickup>();
			dripNumber.transform.parent = transform;
			
			dripNumber=null;
			AudioManager.inst.PlayDrip(transform.position,Random.Range (.4f,.6f),Random.Range (.4f,.5f));
		}
	}
}
