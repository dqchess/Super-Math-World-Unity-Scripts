using UnityEngine;
using System.Collections;

public class PlayerStart : Location {

	public Transform playerStartT;

	public void Awake(){
//		WebGLComm.inst.Debug("Player start awake.");
		Player.inst.AddPlayerStartPriority(PlayerStartType.StartObject,playerStartT,"playerstart obj");	
	}

	#region UserEditable
	public override void OnGameStarted(){
		base.OnGameStarted();
	}

	#endregion


	float playerOffGroundOffset = 5; // keep in mind player is scaled UP when we're placing him for level builder. Super stupid and will break. Oh well.




	void Start(){
		if (LevelBuilder.inst.levelBuilderIsShowing){
			SetPlayerStart();
		}
	}

	public override void OnLevelBuilderObjectPlaced(){
		if (LevelBuilder.inst.levelBuilderIsShowing){
			SetPlayerStart();
		}
		base.OnLevelBuilderObjectPlaced();
	}


	public void SetPlayerStart(){
		DestroyDuplicates<PlayerStart>();
		Player.inst.SetPosition(playerStartT);

	}

	void Update(){
		if (LevelBuilder.inst.levelBuilderIsShowing){
			Player.inst.SetPosition(playerStartT);
		}
	}
}
