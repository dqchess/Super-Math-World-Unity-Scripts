using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SheepTrigger : MonoBehaviour {

//	public static string key = "sheepTriggerKey";
	bool thereWereSheepOnStartup = false;
	public void OnGameStarted(){
		thereWereSheepOnStartup = FindObjectsOfType<Animal_Sheep>().Length > 0;
	}

	public Fraction sheepNeeded = new Fraction(1,1);
	public Sprite sheepIcon; // TODO: icon manager
	Fraction totalCollected = new Fraction(0,1);

	List<Animal_Sheep> sheepsInside = new List<Animal_Sheep>();

	void OnTriggerEnter(Collider other){
		if (LevelBuilder.inst.levelBuilderIsShowing) return;
		Animal_Sheep sheep = other.GetComponent<Animal_Sheep>();
		if (sheep){
			sheepsInside.Add(sheep);
		}

		if (other.tag == "Player"){
			string failString = totalCollected.numerator == 0 ? "There are zero sheep here. Bring sheep totaling "+sheepNeeded.ToString()+" value here." : "You collected some sheep totaling "+totalCollected.ToString()+" value, but you need a total value of "+sheepNeeded.ToString()+".";
			string message = Fraction.Equals(totalCollected,sheepNeeded) ? "Great job! You already collected sheep of exactly "+sheepNeeded.ToString()+" value." : failString;
			PlayerNowMessage.inst.Display(message,Player.inst.transform.position);
		}
	}

	void OnTriggerExit(Collider other){
		if (LevelBuilder.inst.levelBuilderIsShowing) return;
		Animal_Sheep sheep = other.GetComponent<Animal_Sheep>();
		if (sheep){
			if (sheepsInside.Contains(sheep)){
				sheepsInside.Remove(sheep);
			}

		}
	}


	float checkInterval = 1.5f;
	float t = 0;
	bool active = true;


	float userErrorCheckInterval = 20; // did the user destroy all the sheep or are all the sheep in the level here? If so we ask user if they want to restart the level.
	bool optout = false;
	void Update(){
		if (LevelBuilder.inst.levelBuilderIsShowing) return;
		if (active){
			t -= Time.deltaTime;
			if (t < 0){
				t = checkInterval;
				totalCollected = new Fraction(0,1);
				foreach(Animal_Sheep sheep in sheepsInside){
					totalCollected = Fraction.Add(totalCollected,sheep.fraction);
				}
				if (Fraction.Equals(totalCollected,sheepNeeded)){
					AudioManager.inst.PlayItemGetSound();
					transform.root.GetComponentInChildren<ResourceDrop>().DropResource();
					active=false;
					PlayerNowMessage.inst.Display("You did it! You collected exactly "+sheepNeeded.ToString()+" sheep.",Player.inst.transform.position);
				}
			}

		}
		if (active && !LevelBuilder.inst.levelBuilderIsShowing && !optout) {
			
			userErrorCheckInterval -= Time.deltaTime;
			if (userErrorCheckInterval < 0){
				userErrorCheckInterval = 20;
				if (!PlayerDialogue.inst.showing) {
					Fraction totalAvailable = new Fraction(0,1);
					List<Animal_Sheep> sheepsLeft = new List<Animal_Sheep>();
					sheepsLeft.AddRange(FindObjectsOfType<Animal_Sheep>());
//					foreach(Animal_Sheep sheep in sheepsLeft){
//						totalAvailable = Fraction.Add(totalAvailable,sheep.fraction);
//					}
					if (sheepsLeft.Count == 0 && FindObjectsOfType<LevelMachineSheepConverter>().Length == 0 && thereWereSheepOnStartup && !Fraction.Equals(totalCollected,sheepNeeded)){
						// TODO: we shouldn't be assigning delegates to player dialogue if it's already showing, so we should pass delegate targets (list) into the ShowPlayerDialogue function and handle it there
						PlayerDialogue.inst.ShowPlayerDialogue("I'm counting sheep, and there are none left! Do you want to restart?","Uh oh..",sheepIcon);
						PlayerDialogue.inst.playerPressedOKDelegate += PlayerWantedToRestart;
						PlayerDialogue.inst.playerPressedCancelDelegate += PlayerCanceled;
					}
				}

			}
		}
	}

	void PlayerWantedToRestart(){
		PlayerDialogue.inst.playerPressedOKDelegate -= PlayerWantedToRestart;
		GameManager.inst.RestartLevelActual();
	}

	void PlayerCanceled(){
		PlayerDialogue.inst.playerPressedCancelDelegate -= PlayerCanceled;
		optout = true;
	}



}
