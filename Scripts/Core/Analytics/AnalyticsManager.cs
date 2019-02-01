using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;



//public class UserData {
//	public float playing = 0;
//	public float editing = 0;
//	public float paused = 0;
////	public float idle = 0;
//	public float mouseMovedDistance = 0;
//	public float worldDistanceMoved = 0;
//	public int numbersPickedUp = 0;
//	public int numbersThrown = 0;
//	public int timesMultiblasterFired = 0;
//	public int timesZookaFired = 0;
//	public int timesSwordSwung = 0;
//	public int numberChopped = 0;
//	public int zerosCreated = 0;
//	public int timesItemDroppedOnInventorySlot = 0;
//	public int timesBeltSpaceSelected = 0;
//	public int timesSaveButtonPressed = 0;
//	public int timesSuccessfullySaved = 0;
//	public int timesMarkerMenuShown = 0;
//	public List<string> levelBuilderButtonsPressed = new List<string>();
//
//}



public class AnalyticsManager : MonoBehaviour {
	public static class Keys {
		public static string secondsPlayed = "seconds_played";
		public static string secondsEditing = "seconds_editing";
		public static string secondsPaused = "seconds_paused";
		public static string mouseMovedDistance = "mouse_moved_distance";
		public static string worldDistanceMoved = "world_distance_moved";
		public static string levelBuilderButtonPress = "level_builder_button_press";
		public static string zerosCreated = "zeros_created";
		public static string timesMultiblasterFired = "times_multiblaster_fired";
		public static string numberChopped = "times_sword_chopped_successful";
		public static string timesSwordSwung = "times_sword_chopped";
		public static string numbersThrown = "numbers_thrown";
		public static string timesMarkerMenuShown = "times_marker_menu_shown";
		public static string timesSaveButtonPressed = "times_save_button_pressed";
		public static string timesSuccessfullySaved = "times_successfully_saved";
		public static string timesZookaFired = "times_zooka_fired";
		public static string timesBeltSpaceSelected = "times_belt_space_selected";
		public static string timesItemDroppedOnInventorySlot = "times_item_dragged_inventory";
		public static string numbersPickedUp = "numbers_picked_up";
		public static string showMouseRetrapHelp = "show_mouse_retrap_help";
		public static string droppedInventoryItem = "dropped_inventory_item";
		public static string heatmap = "heatmap";
		public static string pos = "pos";
		public static string fps = "fps";
		public static string blocksDestroyed = "blocks_destroyed";
		public static string checkpointReached = "num_checkpoint_reached";
		public static string levelCompleted = "level_completed";
		public static string monstersDestroyed = "monsters_destroyed";
		public static string animalsDestroyed = "animals_destroyed";
	}

//
//	public float worldDistanceMoved = 0;
//	public int numbersPickedUp = 0;
//	public int numbersThrown = 0;
//	public int timesMultiblasterFired = 0;
//	public int timesZookaFired = 0;
//	public int timesSwordSwung = 0;
//	public int numberChopped = 0;
//	public int zerosCreated = 0;
//	public int timesItemDroppedOnInventorySlot = 0;
//	public int timesBeltSpaceSelected = 0;
//	public int timesSaveButtonPressed = 0;
//	public int timesSuccessfullySaved = 0;
//	public int timesMarkerMenuShown = 0;
//	public List<string> levelBuilderButtonsPressed = new List<string>();

//	public static Keys keys;


	public void SetInstance(){
		inst = this;
//		keys = 
	}

	void Start(){
		WebGLComm.inst.SendAnalytics(""); // inits the session for Unity
	}



	public static AnalyticsManager inst;

	/*
	 * 
	 * What user behaviors do we want to track and store? Balancing our time to read/interpret the data, the cost of sending/storing the data, difficulty of implementation, and future usefulness of it?
	 * We should update every 30 seconds the following information
	 * 		MOVEMENT TIME While playing, For how many seconds was a W,A,S,D, or arrow key pressed? 
	 * 		MOUSE LOOK AMOUNT While playing, how far was the mouse moved?
	 * 		STATIONARY TIME For how many seconds was the player sitting still and not moving keys?
	 * 		IDLE TIME For how many seconds was there NO INPUT AT ALL?
	 * 		Editor time For how many second was the editor open and user was not idle?
	 * 		Number of editor opens How many times did player pause, open editor, place an item? We should just track um 
	 *		Did the user change maps?
	 *		Did the user create a new map? Did they save this map?
	 *
	 *
	 *		Does the Json object grow over time and we're passing bigger and bigger objects to the js? That feels wrong
	 *		Or do we send a chunk each time and the js updates the database with just the latest chunk, putting serial chunks into studentsession.json like a [json{},json{},json{}]? Feels ok
	 *		Or do we have js parse the json, no
	 *		Or do we use a totally separate database model for unity sessions vs student sessions and just keep a rel?
	 *		We definitely want these 30 second updates to NOT get bigger w each update
	 *		We definitely want to avoid doing extra database work for every 30 second push
	 *		So, it sounds like json chunks in a rel is the way to go.
	 *		
	 *		
	 * 
	 * */

	SimpleJSON.JSONClass N = new SimpleJSON.JSONClass();

	float time = 0;
	float sessionInterval = 30; // seconds between each report


	float heatTimer = 0f; // count to 0 then set to interval
	void Update(){
		if (LevelBuilder.inst.levelBuilderIsShowing){
			RecordEvent(Keys.secondsEditing,Time.deltaTime);
		} else if (PauseMenu.paused){
			RecordEvent(Keys.secondsPaused,Time.deltaTime);
		} else {
			RecordEvent(Keys.secondsPlayed,Time.deltaTime);
		}

		heatTimer -= Time.deltaTime;
		if (heatTimer < 0){
			heatTimer = HeatmapManager.heatmapTimeInterval;
			RecordHeatmapEvent(JsonUtil.GetTruncatedPosition(Player.inst.transform));
		}
		// Timer that sends a session event every sessionInterval seconds.
		time += Time.deltaTime;
		if (time > sessionInterval){
			SendAnalytics();
		}
	}

	public void SendAnalytics(bool force=false){ 
		// "force" if we want to initiate something like alevel change when there may be no data.
		UpdateSession();
		ResetSession();
	}

	void UpdateSession(bool force=false){
//		// commented Debug.Log("sending analytics;"); //+N.ToString());
		if (!force && ((N[Keys.worldDistanceMoved] == null 
			|| N[Keys.mouseMovedDistance] == null 
			|| N[Keys.worldDistanceMoved].AsFloat == 0 
			|| N[Keys.mouseMovedDistance].AsFloat == 0
		) && (N[Keys.levelBuilderButtonPress] == null 
			|| N[Keys.levelBuilderButtonPress].AsArray.Count == 0 ))) {

			// Worldpos and mousepos zero distance, and also no editor buttons pressed -- oh my we must be idle!

			// Idle is now handled in JS. If a report is not sent (repeatedly) the js will recognize idle, so Unity doesn't need to detect idle
			// This way is better for a variety of reasons but mainly beacuse Unity will "pause" if user is not focused so no reports will be sent nor will an idle alert be sent.

		} else {
			StatsMonitor.StatsMonitor stats = FindObjectOfType<StatsMonitor.StatsMonitor>();
			int fps = -1;
			if (stats) fps = stats.fps;
			N[Keys.fps].AsInt = fps;
			#if UNITY_EDITOR
//			Debug.Log("sending:"+N.ToString());
			#else
			WebGLComm.inst.SendAnalytics(N.ToString());	
			#endif
		}
	}

	public void RecordHeatmapEvent(string s){ // for recording heatmaps
		if (N[Keys.heatmap] == null) N[Keys.heatmap] = new SimpleJSON.JSONArray();

		SimpleJSON.JSONClass item = new SimpleJSON.JSONClass();
		item[Keys.pos] = s;
		// note that time is not recorded, heatmaps are in a giant list in order and associated with a StudentSession with start and stop times
		N[Keys.heatmap].Add(item);
	}

	public void RecordEvent(string key, float amount){
		if (N[key] == null) N[key].AsFloat = 0;
		N[key].AsFloat += amount;
	}

	public void RecordEvent(string key, int amount){
		if (N[key] == null) N[key].AsInt = 0;
		N[key].AsInt += amount;
	}

	public void RecordButtonPressEvent(string buttonName){
		if (N[Keys.levelBuilderButtonPress] == null) N[Keys.levelBuilderButtonPress] = new SimpleJSON.JSONArray();
		N[Keys.levelBuilderButtonPress].Add(new SimpleJSON.JSONData(buttonName));
//		N[key]
	}



	void ResetSession(){
		
		N = new SimpleJSON.JSONClass(); // clears all variables

		time = 0;
	}


}
