using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UpdateManager : MonoBehaviour {

	// Handles the "Update" functions of animals (AnimalUpdate) so that animals are only active if within the sqrmagtoawake.

	public List<MonsterAIBase> monsters = new List<MonsterAIBase>();
	public List<MonsterAIBase> awakeMonsters = new List<MonsterAIBase>();
	public List<Animal> animals = new List<Animal>();
	public List<Animal> awakeAnimals = new List<Animal>();
	public List<Animal> alwaysAwakeAnimals = new List<Animal>();
	float distToAwake = 100f;
//	float sqrMagToAwake = 10000f; // 100 world units

	int frameSkip = 30;
	int frames = 0;
	// Update is called once per frame

	public void Start(){
		LevelBuilder.inst.levelBuilderOpenedDelegate += ClearLists;	
		GameManager.inst.onLevelWasRestartedDelegate += ClearLists;
	}
	public void ClearLists(){
		animals.Clear();
		monsters.Clear();
		awakeAnimals.Clear();
		alwaysAwakeAnimals.Clear();
		awakeMonsters.Clear();
	}


	public static UpdateManager inst;
	public void SetInstance(){
		inst = this;
	}



//	bool cleared = false;
	void Update () {
//		if (LevelBuilder.inst.levelBuilderIsShowing) {
//			if (!cleared){
//				monsters.Clear();
//				awakeMonsters.Clear();
//				animals.Clear();
//				awakeAnimals.Clear();
//				cleared = true;
//			}
//			return;
//		}
//		cleared = false;
		if (LevelBuilder.inst.levelBuilderIsShowing) return;
		frames++;
		if (frames > frameSkip){
			frames = 0;
			awakeMonsters.Clear();
			List<MonsterAIBase> toRemove = new List<MonsterAIBase>();
			foreach(MonsterAIBase monster in monsters){
				if (!monster) {
					toRemove.Add(monster);
					continue;
				}

				Rigidbody rb = monster.GetComponent<Rigidbody>();
				if (Vector3.SqrMagnitude(monster.transform.position-Player.inst.transform.position) < distToAwake*distToAwake){
					if (rb){
						rb.isKinematic = false;
					}
					awakeMonsters.Add(monster);	
				} else {
					if (rb){
						rb.isKinematic = true;
					}
//					monster.Sleep(); // lets the monster's rigidbody sleep.
				}
			}
			foreach(MonsterAIBase m in toRemove){
				RemoveMonster(m);
			}
			
			awakeAnimals.Clear();
			List<Animal> toRemove2 = new List<Animal>();
			foreach(Animal animal in animals){
				if (!animal) {
					toRemove2.Add(animal);
					continue;
				} else if (animal.GetAnimalTargetPreference().cannibalize){
					alwaysAwakeAnimals.Add(animal);
				} else if (Vector3.SqrMagnitude(animal.transform.position-Player.inst.transform.position) < distToAwake*distToAwake){
					awakeAnimals.Add(animal);
//					Debug.Log("awake:"+animal.name);
				} else {
					
				}
			}
			foreach(Animal a in toRemove2){
				RemoveAnimal(a);
			}
		}

		foreach(Animal animal in awakeAnimals){
			if (animal) animal.AnimalUpdate();
		}

		foreach(MonsterAIBase monster in awakeMonsters){
			if (monster) monster.MonsterUpdate();
		}

		foreach(Animal animal in alwaysAwakeAnimals){
			if (animal) animal.AnimalUpdate();
		}
	}

	void LateUpdate(){
		if (LevelBuilder.inst.levelBuilderIsShowing) return;
		List<Animal> toRemove = new List<Animal>();
		foreach(Animal animal in awakeAnimals){
			if (animal) animal.LateAnimalUpdate();
			else toRemove.Add(animal);
		}
		foreach(Animal animal in alwaysAwakeAnimals){
			if (animal) animal.AnimalUpdate();
			else toRemove.Add(animal);
		}


		foreach(Animal r in toRemove){
			RemoveAnimal(r);
		}



	}

	public void RemoveAnimal(Animal a){
		if (animals.Contains(a)){
			animals.Remove(a);
		}
		if (awakeAnimals.Contains(a)){
			awakeAnimals.Remove(a);
		}
		if (alwaysAwakeAnimals.Contains(a)){
			alwaysAwakeAnimals.Remove(a);
		}
	}

	public void RemoveMonster(MonsterAIBase m){
		if (monsters.Contains(m)){
			monsters.Remove(m);
		}
		if (awakeMonsters.Contains(m)){
			awakeMonsters.Remove(m);
		}
	}

}
