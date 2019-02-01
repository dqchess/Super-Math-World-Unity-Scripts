using UnityEngine;
using System.Collections;

public class PlayerWalkDustParticles : MonoBehaviour {

	ParticleSystem ps;
	public static PlayerWalkDustParticles inst;


	public void SetInstance(){
		inst = this;
	}

	RecordPosition rp;
	float nowSpeed = 0;
	void Start () {
		ps = GetComponentInChildren<ParticleSystem>();
		rp = Player.inst.GetComponent<RecordPosition>();
	}
	
	// Update is called once per frame
	void Update () {
		if (FPSInputController.inst.motor.grounded && PlayerVehicleController.inst.currentVehicle == null){
			nowSpeed = Vector3.Magnitude(rp.nowPosition - rp.lastPosition); // 0.2, 0.5, 2.4
//			// commented Debug.Log("nowspeed:"+nowSpeed);
			float mag = FPSInputController.inst.motor.inputMoveDirection.magnitude;


			float offset = 10f;
			float speedNeededForParticles = FPSInputController.inst.motor.movement.maxForwardSpeed * 1.5f;
//			
			if (nowSpeed > 1f){
				for (int i=0;i<10;i++){
					Emit ();
				}
			}
		} 
	}

	float tMinus = 0;
	Vector2 particleIntervalRange = new Vector2(0.5f, 1.6f);
	Vector2 particleAmountRange = new Vector2(1,2.1f);
	public void Emit (float interval = .02f){
//		// commented Debug.Log("Tminus:"+tMinus);
		tMinus -= interval;
		if (tMinus <= 0){
			tMinus = Random.Range (particleIntervalRange.x,particleIntervalRange.y);
			ps.Emit(Mathf.FloorToInt(Random.Range (particleAmountRange.x,particleAmountRange.y)));

		}

	}
}
