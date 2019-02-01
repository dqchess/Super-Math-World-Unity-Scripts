using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {


	public static AudioManager inst;

	public AudioClip activateSound;
	public AudioClip arrowFired;
	public AudioClip bellDing;
	public AudioClip bigElectricArc;
	public AudioClip boom1;
	public AudioClip bossDie;
	public AudioClip bounce1;
	public AudioClip bubble;
	public AudioClip bubbles;
	public AudioClip cameraShutter;
	public AudioClip cameraWindDown;
	public AudioClip cartoonEat;
	public AudioClip cartoonEatLow;
	public AudioClip checkpoint;
	public AudioClip chestOpen;
	public AudioClip chomp;
	public AudioClip clearClink;
	public AudioClip closeMenu;
	public AudioClip click2;
	public AudioClip click3;
	public AudioClip clunk2;
	public AudioClip comboLose;
	public AudioClip comboPlus;
	public AudioClip comboWin;
	public AudioClip crash;
	public AudioClip crystalThump1;
	public AudioClip deathSound;
	public AudioClip depressingCannonSound;
	public AudioClip dirtyClink;
	public AudioClip doorLever;
	public AudioClip doorOpen;
	public AudioClip electricDischarge1;
	public AudioClip electricDischarge2;
	public AudioClip electricDischarge3;
	public AudioClip elevatorArrivedDing;
	public AudioClip equipGadget;
	public AudioClip equipNumber;
	public AudioClip equipSword;
	public AudioClip explosion1;
	public AudioClip exhaustWhoosh;
	public AudioClip fanfare;
	public AudioClip fizzle;
	public AudioClip frogStickSound;
	public AudioClip gadgetGetSound;
	public AudioClip gateOpen;
	public AudioClip gemZoneEnter;
	public AudioClip gemZoneExit;
	public AudioClip glassBounce;
	public AudioClip heartPickup;
	public AudioClip heal1;
	public AudioClip heavyClick;
	public AudioClip hum;
	public AudioClip iceClink;
	public AudioClip inventoryCollect;
	public AudioClip inventoryOpen;
	public AudioClip inventoryClose;
	public AudioClip itemGetSound;
	public AudioClip itemCollect;
	public AudioClip jetpackClip;
	public AudioClip jetpackOutOfFuel;
	public AudioClip jump;
	public AudioClip laserBlast;
	public AudioClip levelBuilderItemPlace;
	public AudioClip levelBuilderOpened;
	public AudioClip levelBuilderPlay;
	public AudioClip levelBuilderSaved;
	public AudioClip lightning;
	public AudioClip lowElectricWarmUp;
	public AudioClip machineRumble;
	public AudioClip monsterPain1;
	public AudioClip monsterPain2;
	public AudioClip munch;
	public AudioClip notify1;
	public AudioClip notify2;
	public AudioClip numberBounce;
	public AudioClip numberEat;
	public AudioClip numberFire;
	public AudioClip numberImpact;
	public AudioClip shatter;
	public AudioClip numberThrow;
	public AudioClip pickup1; // gem sound
	public AudioClip pickup2_appGame;
	public AudioClip plantOpen;
	public AudioClip plungerSuck;
	public AudioClip poof;
	public AudioClip rain1;
	public AudioClip riserArrived;
	public AudioClip rocketLaunch;
	public AudioClip rocketLaunch3d;
	public AudioClip sawBuzz;

	public AudioClip shortBop;
	public AudioClip shortElectricBurst;

	public AudioClip simpleShortShot;
	public AudioClip simpleShortShotLaser;
	public AudioClip slideSound;
	public AudioClip sonarSound;
	public AudioClip sonicBoom;
	public AudioClip sparkle1;
	public AudioClip staticDischarge;
	public AudioClip successBeep;
	public AudioClip swish;

	public AudioClip takeDamage;
	public AudioClip thunder1;
	public AudioClip timerDing;
	public AudioClip tickticktick;
	public AudioClip trainSound;
	public AudioClip underwaterBubbles;
	public AudioClip vehicleBoatStart;
	public AudioClip vehicleBoatRunning;
	public AudioClip vehicleBoatStop;
	public AudioClip vehicleCarStart;
	public AudioClip vehicleCarRunning;
	public AudioClip vehicleCarStop;
	public AudioClip waterIn;
	public AudioClip waterOut;
	public AudioClip waterSplash1;
	public AudioClip weirdAlarm;
	public AudioClip whoosh;
	public AudioClip wrongAnswer;
	public AudioClip wrongAnswerError;
	public AudioClip zap1;
	
	
	
	public AudioClip[] spikeySounds = new AudioClip[6];
	public AudioClip[] sheepSounds = new AudioClip[4];
	public AudioClip[] frogSounds = new AudioClip[3];
	public AudioClip[] cartoonVoices = new AudioClip[5];
	public AudioClip[] wingFlap =new AudioClip[2];
	public AudioClip[] iceCrackle;
	public AudioClip[] drips;
	
	// old
	public AudioSource jetpackSound;
	public AudioClip[] monsterFootsteps;
	AudioSource sonarSource;
	public AudioClip[] clunk;
	public AudioClip[] vaporize;
	
	public AudioClip music;
	AudioSource[] audios = new AudioSource[50];
	int audiosIndex=0;
	AudioSource musicSource;
	AudioSource footstepsSource;
	AudioSource gemSoundSource;
	AudioSource slideSoundSource;
	
	public AudioClip[] footstepsConcrete;
	public AudioClip[] footstepsGrass;
	public AudioClip[] footstepsWood;
	public AudioClip[] footstepsGlass;
	public AudioClip[] footstepsSponge;
	public AudioClip[] munchSound;
	
	public AnimationCurve rolloffCurve;


	public void SetInstance(){
		inst = this;
	}
	void  Start (){
		
		jetpackSound =gameObject.AddComponent<AudioSource>(); // we need a one-off for this one so it isn't lost in the rolling audio sources, since we need to turn it on and off at will. 
		jetpackSound.playOnAwake=false;
		jetpackSound.clip=jetpackClip;
		sonarSource = gameObject.AddComponent<AudioSource>(); // we need a one-off for this one so it isn't lost in the rolling audio sources, since we need to turn it on and off at will.
		sonarSource.playOnAwake=false;
		sonarSource.loop=true;
		sonarSource.clip=sonarSound;
		
		gemSoundSource = gameObject.AddComponent<AudioSource>(); // we need a one-off for this one so it isn't lost in the rolling audio sources, since we need to turn it on and off at will.
		gemSoundSource.playOnAwake=false;
		gemSoundSource.loop=false;
		gemSoundSource.clip=pickup1;
		gemSoundSource.volume = 0.25f;

		slideSoundSource = gameObject.AddComponent<AudioSource>();
		slideSoundSource.playOnAwake = false;
		slideSoundSource.loop = false;
		slideSoundSource.clip = slideSound;
		slideSoundSource.volume = 0.4f;
		// Note that this is needed to play separate audio clips at the same time (overlapping).
		// Init all 30 or 512 audio sources. 
		
		for (int i=0;i<audios.Length;i++){
			GameObject go = new GameObject();
			go.transform.parent = transform;
			audios[i] = go.AddComponent<AudioSource>();
			audios[i].GetComponent<AudioSource>().loop=false;
			audios[i].dopplerLevel=0;
			audios[i].minDistance=0.5f;
			audios[i].maxDistance=100.5f;
			audios[i].spread=0;
			audios[i].rolloffMode = AudioRolloffMode.Custom;// AudioRolloffMode.Logarithmic;
			audios[i].spatialBlend = 0.75f;
			audios[i].SetCustomCurve(AudioSourceCurveType.CustomRolloff,rolloffCurve);
		}
		footstepsSource = gameObject.AddComponent<AudioSource>();
		footstepsSource.volume=.5f;
		
		/*musicSource = gameObject.AddComponent<AudioSource>();
		musicSource.audio.loop=true;
		musicSource.clip = music;
		musicSource.Play();*/
		// audioSourceFart.playMode = PlayMode.Once;
	}
	
	public void StopAudioAt(int audioIndexUsed){
		if (audios[audioIndexUsed]) audios[audioIndexUsed].Stop();
	}
	
	
	public void WaterSplash1(Vector3 pos, float vol=.2f) {
//		// commented Debug.Log("eh?");
		PlayLocationSound(waterSplash1, pos, vol,1);
	}
	
	public void PlayFlatSound(AudioClip clip, float vol=1, float pitch=1){
		PlayLocationSound(clip,Vector3.zero,vol,pitch,0);
	}
	public void PlayLocationSound(AudioClip clip, Vector3 pos, float vol, float pitch, float spatialBlend = 1, float cutoffDist = 150f) {
		//		return;
		//		// commented Debug.Log ("play location sound :" +clip);
		if(clip == null) { return; }
		AudioIndexPlus(); 
//		// commented Debug.Log("playing " +clip.name+" with linear:"+linearRolloff+", vol:"+vol);
		if (pos == Vector3.zero) pos = Player.inst.transform.position;
		
		// manual cutoff
		// unity sucks apparently and Log sounds are STILL AUDIBLE despite me being 2x the cutoff dist away.

		if (Vector3.SqrMagnitude(Player.inst.transform.position-pos)>cutoffDist*cutoffDist) {
			if (pos != Vector3.zero){ // for 2D sounds only
				return;
			}
		}

		
		
		
		if (!audios[audiosIndex]) return;
		audios[audiosIndex].rolloffMode = AudioRolloffMode.Custom; // assume log rolloff unless linear is called
		audios[audiosIndex].spatialBlend = spatialBlend;
		audios[audiosIndex].clip = clip;
		pos = Utils.UnNan(pos); // this is Nan or Infinite sometimes because some foolish gameobject made a call to AudioManager and passed a Vector3 with a NaN
		// Fixing this because it breaks entire webgl thread if pass a NaN to AudioPanner (non finite)
		audios[audiosIndex].gameObject.transform.position = pos;
		audios[audiosIndex].volume = vol;
		audios[audiosIndex].pitch = pitch;
		//		// commented Debug.Log ("pitch: "+pitch);
		
		audios[audiosIndex].Play();


		if (VideoRecorder.inst.replayState == ReplayState.Recording){
			float cutDist = 75f;
			float playerDist = Vector3.Distance(Player.inst.transform.position,pos);
			if (playerDist > cutDist) return;
//			float effectiveVol = rolloffCurve. audios[audiosIndex].roll
			float effectiveVol = (vol * rolloffCurve.Evaluate(playerDist) * spatialBlend ) + (vol * (1 - spatialBlend));
			VideoRecorder.inst.RegisterSoundClip(clip.name,effectiveVol,pitch,clip.length);
		}

	}
	
	public void PlayLocationSound(AudioClip clip) {
		PlayLocationSound(clip, Player.inst.transform.position, 1.0f, 1.0f,0);
	}
	
	public void PlayLocationSound(AudioClip clip, Vector3 pos) {
		PlayLocationSound(clip, pos, 1.0f, 1.0f);
	}
	
	public void PlayGemZoneEnter(){
		PlayLocationSound(gemZoneEnter,Player.inst.transform.position,1f,1);
		PlayLocationSound(waterIn,Player.inst.transform.position,1,1);
	}
	
	public void PlayGemZoneExit(){
		PlayLocationSound(waterOut,Player.inst.transform.position,1,1);
		PlayLocationSound(gemZoneExit,Player.inst.transform.position,.3f,1);
	}
	
	public void EquipSword(){
		PlayLocationSound(equipSword,Player.inst.transform.position,.09f,Random.Range(.9f,1.1f),0);
	}

	public void PlayLevelBuilderItemPlace(){
		PlayLocationSound(levelBuilderItemPlace,Player.inst.transform.position,.35f,Random.Range(.9f,1.1f),0);
	}


	public void PlayEquipNumber(){
		
		PlayLocationSound(equipNumber,Player.inst.transform.position,.5f,Random.Range(.9f,1.1f),0);
	}
	public void PlayCloseMenuSound(string source="def"){
//		#if !UNITY_EDITOR 

//		#endif
//		Debug.Log("close menu;"+source);
		PlayLocationSound(closeMenu,Player.inst.transform.position,0.6f,1f,0);
	}

	public void EquipGadget(){
		if (gadgetGetCooldown < 0){
			PlayLocationSound(equipGadget,Player.inst.transform.position,.5f,Random.Range(.9f,1.1f),0);
		} else {
//			// commented Debug.Log("gadgetcool:"+gadgetGetCooldown);
		}
	}

	public void EquipGadget3D(Vector3 p){
		PlayLocationSound(equipGadget,p,.5f,Random.Range(.9f,1.1f),0);
	}





	public void PlayPlayerFootsteps(FootstepSoundType stepType, float vol){
//		int ind = 0;
		if (FPSInputController.inst.motor.slidingNow) return;
		vol += Random.Range(0.0f,0.1f);
		float pitch = Random.Range(0.85f,1.10f);
		AudioClip clip = null;
		switch(stepType){
		case FootstepSoundType.Concrete:
			clip = footstepsConcrete[Random.Range(0,footstepsConcrete.Length)];
			vol *= 0.41f;
			break;
		case FootstepSoundType.Grass:
			vol = Random.Range(0.1f,0.15f);
			clip = footstepsGrass[Random.Range(0,footstepsGrass.Length)];
			break;
		case FootstepSoundType.Wood:
			clip = footstepsWood[Random.Range(0,footstepsWood.Length)];
			break;
		case FootstepSoundType.Glass:
			clip = footstepsGlass[Random.Range(0,footstepsGlass.Length)];
			pitch = Random.Range(0.95f,1f); //Random.Range(0.7f,.9f);
			break;
		case FootstepSoundType.Sponge:
			clip = footstepsSponge[Random.Range(0,footstepsSponge.Length)];
			vol += 0.25f;
			break;
		default:break;
		}
		if (clip) PlayLocationSound(clip,Player.inst.transform.position,vol,pitch,0.5f);
	}

	public void PlayBigFootsteps(Vector3 pos){
		int ind= Random.Range(0,monsterFootsteps.Length);
		float vol = Random.Range(0.5f,0.6f);
		PlayLocationSound(monsterFootsteps[ind],pos,1,vol,0.5f,200);
	}
	
	public void PlaySmallFootsteps(Vector3 pos){
		int ind = Random.Range(0,monsterFootsteps.Length);
		float pitch = Random.Range(1.9f,2f);
		if (ind == 2 || ind == 3 || ind == 4) pitch = 1.2f;Random.Range(1.9f,2f);
		PlayLocationSound(monsterFootsteps[ind],pos,1,pitch,0.5f,400);
		//		// commented Debug.Log("CLOMP: "+Time.time);
	}
	
	public void PlayNumberFire(Vector3 pos, float pitch=1){
		PlayLocationSound(numberFire,pos,1.0f,pitch);
	}
	
	public void PlayIceClink(Vector3 pos, float vol=1, float pitch=1){
		PlayLocationSound(iceClink,pos,vol,pitch,1,100);
	}
	
	public void PlayGlassBounce(Vector3 pos, float vol=1, float pitch=1){
		PlayLocationSound(glassBounce,pos,vol,pitch,1,100);
	}
	
	public void PlayBubblePop(Vector3 pos, float pitch){
//		Debug.Log("bubble at:"+pos+" with pitch;"+pitch);
		PlayLocationSound(bubble,pos,1.0f,pitch,0.8f);
	}
	
	public void PlayCartoonEat(Vector3 pos, float pitch, float vol=0.3f){
		PlayLocationSound(cartoonEat,pos,vol,pitch);

	}

	public void PlayCartoonEatLow(Vector3 pos, float pitch, float vol=0.3f, float spatialBlend = 1){
		PlayLocationSound(cartoonEatLow,pos,vol,pitch,spatialBlend);

	}

	public void PlayPlungerSuck(Vector3 pos, float pitch, float vol=.4f, float spatialBlend = 0.9f){
		PlayLocationSound(plungerSuck,pos,vol,pitch,spatialBlend);
	}
	
	
	
	public void PlayElectricArc(Vector3 pos, float vol, float pitch){
		PlayLocationSound(bigElectricArc,pos,vol,pitch,0.5f);
	}
	
	public void PlayStaticDischarge(Vector3 pos, float vol, float pitch){
		PlayLocationSound(staticDischarge,pos,vol,pitch,0.5f);
	}
	
	
	public void PlayBossDie(){
		PlayLocationSound(bossDie,transform.position);
	}
	
	public void PlayWrongAnswerError(Vector3 pos, float vol, float pitch){
		PlayLocationSound(wrongAnswerError,pos,vol,pitch,0.5f);
	}
	
	public void PlayRandomSheepSound(Vector3 pos, float pitch=1){
//		// commented Debug.Log("baa!");
		float vol = Random.Range(.7f,1f);
		pitch = Random.Range(pitch,pitch*1.3f);
		PlayLocationSound(sheepSounds[Random.Range(0,sheepSounds.Length)],pos,vol,pitch);

	}

	public void PlayFrogStick(Vector3 pos){
		float pitch = Random.Range(0.8f,1f);
		float vol = Random.Range(0.8f,1f);
		PlayLocationSound(frogStickSound,pos,vol,pitch);
	}

	public void PlayRandomFrogSound(Vector3 pos, float pitch=1){
		//		// commented Debug.Log("baa!");
		float vol = Random.Range(.7f,1f);
		pitch = Random.Range(pitch,pitch*1.3f);
		PlayLocationSound(frogSounds[Random.Range(0,frogSounds.Length)],pos,vol,pitch);

	}
	
	
	public void PlayShortElectricBurst(Vector3 pos) {
		PlayLocationSound(shortElectricBurst, pos, 1f, UnityEngine.Random.Range(0.95f,1.05f));
	}
	
	public void PlayBoom1(Vector3 pos) {
		PlayLocationSound(boom1, pos, .5f, UnityEngine.Random.Range(0.95f,1.05f),0.5f);
	}
	
	public void PlayNumberImpact(Vector3 pos) {
		PlayLocationSound(numberImpact, pos, .1f, UnityEngine.Random.Range(0.95f,1.05f),0.5f);
	}
	

	public void PlayDeathSound(Vector3 p){
		PlayLocationSound(deathSound,p);

	}	
	
	public void PlayItemGetSound(){
		AudioIndexPlus();
		PlayLocationSound(itemGetSound, Player.inst.transform.position);
	}

	public void PlayItemCollect(){
		AudioIndexPlus();
		PlayLocationSound(itemCollect, Player.inst.transform.position);
	}

	public void PlayNotify1(){
		AudioIndexPlus();
		PlayLocationSound(notify1, Player.inst.transform.position);
	}
	
	public void PlaySheepMunch(Vector3 pos){
		PlayLocationSound(munchSound[Random.Range(0,munchSound.Length-1)], pos,1,Random.Range(.9f,1.1f),0.98f);
	}
	
	public void PlayNotify2(){
		AudioIndexPlus(); // Wrong answer
		PlayLocationSound(notify2, Player.inst.transform.position);
	}
	
	
	public void PlaySonarSound(){
		//		// commented Debug.Log("hi");
		sonarSource.Play();
	}
	public void StopSonarSound(){
		sonarSource.Stop();
	}
	
	
	public void PlayWrongAnswer(Vector3 pos) {
		PlayLocationSound(wrongAnswer, pos, 1f, UnityEngine.Random.Range(0.95f,1.05f)); //TODO: this is inaudible in game
	}
	
	public void PlayClearClink(Vector3 pos){
		PlayLocationSound(clearClink, pos, 1f, UnityEngine.Random.Range(0.95f,1.05f)); 
	}
	
	
	public void PlayDirtyClink(Vector3 pos){
		PlayLocationSound(dirtyClink, pos, 1f, UnityEngine.Random.Range(0.95f,1.05f)); 
	}
	
	
	public void PlayCrystalThump1(Vector3 pos) {
		float vol = Random.Range(0.4f,.6f);
		PlayLocationSound(crystalThump1, pos, vol, UnityEngine.Random.Range(0.85f,1.05f),0.5f);
	}
	
	public void PlaySimpleShortShot(Vector3 pos) {
		PlayLocationSound(simpleShortShot, pos, 1f, UnityEngine.Random.Range(0.95f,1.05f));
	}
	
	public void PlayShortBop(Vector3 pos) {
		PlayLocationSound(shortBop, pos, 0.4f, UnityEngine.Random.Range(0.95f,1.05f));
	}
	
	
	
	public void PlaySimpleShortShotLaser(Vector3 pos) {
		PlayLocationSound(simpleShortShotLaser, pos, 1f, UnityEngine.Random.Range(0.95f,1.05f));
	}
	
	public void PlayLowElectricWarmUp(Vector3 p, float v=1f) {
		PlayLocationSound(lowElectricWarmUp, p, v, UnityEngine.Random.Range(0.75f,1.15f));
	}
	
	public void PlaySpikeySound(Vector3 p, float v = 0.34f){
		//		// commented Debug.Log ("playing spikey sound at distance : "+Vector3.Distance(Player.inst.transform.position,p));
//		float v = Random.Range(.2f,.45f);
		PlayLocationSound(spikeySounds[Random.Range(0,spikeySounds.Length)], p,  v, UnityEngine.Random.Range(0.95f,1.05f));
	}

	public void PlayCartoonVoice(Vector3 p, float v = 1, float pitch=1){
		//		// commented Debug.Log ("playing spikey sound at distance : "+Vector3.Distance(Player.inst.transform.position,p));
		//		float v = Random.Range(.2f,.45f);
		PlayLocationSound(cartoonVoices[Random.Range(0,cartoonVoices.Length)], p,  v, pitch);
	}
	
	public void PlayNumberShatter(Vector3 pos) {
		float vol = 0.25f;
		float pitch = UnityEngine.Random.Range(0.95f,1.05f);
		PlayLocationSound(shatter, pos, vol, pitch);
	}
	
	public void PlayNumberEat(Vector3 pos, float v =1) {
		//		PlayDrip(pos);
		float p = Random.Range(0.7f,1.1f);
		v *= Random.Range(0.8f,1f);
		PlayLocationSound(numberEat, pos, v, p,.9f);
//		// commented Debug.Log("ate");
		
	}
	
	public void PlayNumberBounce(Vector3 pos, float speed) {
		float vol = UnityEngine.Mathf.Clamp(speed / 20.0f, 0.0f, 1.0f) * 1.1f;
		//		// commented Debug.Log("vol:"+vol);
		PlayLocationSound(numberBounce, pos, vol, UnityEngine.Random.Range(0.53f,.59f));
	}
	
	public void PlayTickTickTick(Vector3 pos){
		PlayLocationSound(tickticktick, pos, 1f, UnityEngine.Random.Range(0.53f,.59f));
	}
	
	public void PlayActivate(Vector3 pos){
		PlayLocationSound(activateSound, pos, 1f, UnityEngine.Random.Range(0.53f,.59f));
	}
	
	public void PlayLightning(Vector3 pos){
		PlayLocationSound(lightning, pos, 1f, UnityEngine.Random.Range(0.9f,1.1f));
	}
	
	
	// short beeps 
	// shouldn't happen all at once
	float beepCooldown=0;
	void Update(){
		if (beepCooldown > 0) beepCooldown -= Time.deltaTime;
		gadgetGetCooldown-=Time.deltaTime;
		invSoundTimeout -= Time.deltaTime;
	}
	
	
	public void PlayShortBeep(){
		
		gemSoundSource.Stop();
		gemSoundSource.Play();
	}
	
	float gadgetGetCooldown = 0;
	public void PlayGadgetPickup(Vector3 pos){
		PlayLocationSound(gadgetGetSound, Player.inst.transform.position,1, 1f);
		gadgetGetCooldown=.2f;
		//		PlayLocationSound(pickup1,pos);
		
		
	}
	
	
	public void PlayPickup2(){
		PlayLocationSound(pickup2_appGame, Player.inst.transform.position,.5f, 1f);
	}
	
	public void PlayShortLaser(float v){
		v = Random.Range(0.9f*v,1f*v);
		float p = Random.Range(0.95f,1.05f);
		PlayFlatSound(laserBlast, v,p); 
	}
	
	
	
	
	public void PlayHeavyClick(Vector3 pos){
		PlayLocationSound(heavyClick, pos, 1f, UnityEngine.Random.Range(0.53f,.59f));
	}
	
	public void PlayWeirdAlarm(Vector3 pos){
		PlayLocationSound(weirdAlarm, pos, 1f, UnityEngine.Random.Range(0.53f,.59f));
	}
	
	public void PlayCameraShutter(){
		PlayLocationSound(cameraShutter, Player.inst.transform.position, 1f,1);
	}
	
	public void PlayCameraWindDown(){
		PlayLocationSound(cameraWindDown, Player.inst.transform.position, 1f,1);
	}
	
	public void PlayBounce1(Vector3 pos){
		PlayLocationSound(bounce1,pos,1,1,0.5f);
		
	}

	public void PlayPoof(Vector3 p, float vol=1){
		PlayLocationSound(poof, p, vol, UnityEngine.Random.Range(0.6f,1.4f),0.5f);
	}
	
	
	public void PlayElectricDischarge1(Vector3 pos){ // wrong names TODO swap lightningwhump and electridischarge
		PlayLocationSound(electricDischarge1,pos,.7f,1,0.5f);
	}
	
	
	public void PlayElectricDischarge2(Vector3 pos, float vol=0.7f){
		float pitch = Random.Range(0.8f,1.1f);
		PlayLocationSound(electricDischarge2,pos,vol,pitch,0.5f);
		
		
	}
	
	public void PlayElectricDischarge3(Vector3 pos){ // wrong names TODO swap lightningwhump and electridischarge
		PlayLocationSound(electricDischarge3,pos,.7f,1,0.5f);
	}
	
	public void PlayRandomElectricitySound(Vector3 pos){
		int i=(int)Random.Range(0,2);
		switch(i){
		case 0: PlayElectricDischarge1(pos); break;
		case 1: PlayElectricDischarge2(pos); break;
		case 2: PlayElectricDischarge3(pos); break;
		default: break;
		}
	}
	
	
	
	
	
	
	
	
	
	
	/// <summary>
	/// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	
	
	
	
	/// </summary>
	
	
	public void PlayNumberThrow(float vol=0.3f){
		//		// commented Debug.Log("throw");
		PlayFlatSound(numberThrow,vol);
	}

	public void PlayNumberThrow3D(Vector3 p,float vol=0.3f){
		PlayLocationSound(numberThrow,p,vol,1,.95f);
	}
	
	public void PlayRain1(float seconds){
		AudioIndexPlus();
		audios[audiosIndex].clip=rain1;
		audios[audiosIndex].volume = .4f;
		audios[audiosIndex].pitch = 1f;
		audios[audiosIndex].Play();
	}
	
	
	
	
	public void PlayThunder1(){
		PlayLocationSound(thunder1);
		//		AudioIndexPlus();
		//		audios[audiosIndex].clip=thunder1;
		//		audios[audiosIndex].volume = 1f;
		//		audios[audiosIndex].pitch = 1f;
		//		audios[audiosIndex].Play();
	}
	
	
	void  AudioIndexPlus (){
		audiosIndex = (audiosIndex+1)%audios.Length;// +=1;if(audiosIndex>=audios.Length)audiosIndex=0;
	}
	
	public void PlayClick2(){
		PlayLocationSound(click2,Player.inst.transform.position,0.3f,Random.Range(0.8f,1.1f));

	}
	public void PlayClick3(){
		PlayLocationSound(click3,Player.inst.transform.position,0.3f,Random.Range(0.8f,1.1f));

	}
	
	public void PlayRocketLaunch(Vector3 pos, float vol=0.2f){
//		// commented Debug.Log("rock:"+pos);
//		Debu
		PlayLocationSound(rocketLaunch, pos, vol, 1,1);
	}
	
	public void PlayRocketLaunch3D(Vector3 pos){
		
		PlayLocationSound(rocketLaunch3d, pos, .8f, Random.Range(.6f,.8f),0.5f,150);
	}
	
	public void PlayExplosion1(Vector3 pos) {
//		Debug.Log("exp:"+pos);
		return;
		float vol = UnityEngine.Random.Range(0.02f,.03f);
		float pitch = UnityEngine.Random.Range(0.95f,1.05f);
		PlayLocationSound(explosion1, pos, vol, pitch,1);

	}
	public void PlayExhaustWhoosh(Vector3 pos, float vol =1, float pitch=1) {
		//		Debug.Log("exp:"+pos);
	
		PlayLocationSound(exhaustWhoosh, pos, vol, pitch,1);

	}
	
	
	public void PlayPlantOpen(Vector3 p){
		PlayLocationSound(plantOpen,p,1,Random.Range(0.9f,1.1f),0.5f);
	}
	
	
	public void PlaySuccessBeep(float vol = 0.3f){
		float pitch = 1f;
		PlayLocationSound(successBeep,Player.inst.transform.position,vol,pitch);
	}
	
	
	
	void  PlayFootstepStandard (){
		// footstepsSource.clip=footstepsConcrete[Random.Range(0,footstepsConcrete.Length)];
		footstepsSource.PlayOneShot(footstepsConcrete[Random.Range(0,footstepsConcrete.Length)]);	
	}
	
	public void PlayWingFlap(Vector3 p){
		PlayLocationSound(wingFlap[Random.Range(0,wingFlap.Length)],p,1,Random.Range(.9f,1.1f));
	}
	
	
	int comboCounter = 0;
	public void PlayComboWin(){
		
		PlayLocationSound(comboWin);
		comboCounter=0;
	}
	
	public void PlayComboPlus(){
		comboCounter++;
		float pitch = 0.6f + comboCounter/10f;
		PlayLocationSound(comboPlus,Vector3.zero,1,pitch);
	}
	
	public void PlayComboLose(){
		PlayLocationSound(comboLose);
		comboCounter=0;
	}
	
	public void PlayBellDing(Vector3 p){
		PlayLocationSound (bellDing,p,1,1,0.5f);
	}
	
	public void PlayDoorLever(Vector3 p){
		float vol = 0.5f;
		float pitch = Random.Range(0.9f,1f);
		PlayLocationSound(doorLever,p,vol,pitch,0.5f);
	}
	
	public void PlayTrainSound(){
		PlayLocationSound(trainSound);
	}
	
	public void PlayChomp(Vector3 p){
		//		// commented Debug.Log ("?");
		PlayLocationSound(chomp,p,1,1,0.5f);
	}
	
	public void PlayJump(){
		//		// commented Debug.Log ("?");
		//		PlayLocationSound(jump,Vector3.zero,1,1,0.5f);
	}
	
	public void PlayDrip(Vector3 pos, float p=.5f, float v=.2f){
		//		// commented Debug.Log ("playing drip");
		
		if(drips.Length > 0) {
			PlayLocationSound(drips[Random.Range (0,drips.Length)],pos,v,p,0.5f,70);
		}
		
	}
	
	public void PlayBubbles(Vector3 pos, float p=1, float v=.5f){
		PlayLocationSound(bubbles,pos,p,v,0.5f,70);
		
	}
	
	public void PlaySwish(Vector3 pos, float p=1, float v=.5f){
		
		PlayLocationSound(swish,pos,p,v,0.5f,270);
		
	}
	
	public void PlayClunk(Vector3 pos, float p=1, float v=.5f){
		
		PlayLocationSound(clunk2,pos,p,v,0.5f,270);
		
	}
	
	public void PlayCrash(Vector3 pos, float p=1, float v=.5f){
		
		PlayLocationSound(crash,pos,p,v,0.5f,970);
		
	}
	
	public void PlayGateOpen(Vector3 pos, float p=1, float v=.5f){
		PlayLocationSound(gateOpen,pos,v,p,0.5f,970);
	}

	public void PlayDoorOpen(Vector3 pos, float p=1, float v=.5f){
		PlayLocationSound(doorOpen,pos,v,p,0.5f,970);
	}
	
	public int PlayIceCrackle(Vector3 pos, float p=1, float v=0.4f){
		PlayLocationSound(iceCrackle[Random.Range (0,iceCrackle.Length)],pos,v,p);
		return audiosIndex; // give back a ref to which audio was played, so we can stop it remotely if the object was destroyed before audio finish
	}
	
	public void PlayFanfare(){
		PlayLocationSound(fanfare,Vector3.zero,.69f,1);
		//		return audiosIndex; // give back a ref to which audio was played, so we can stop it remotely if the object was destroyed before audio finish
	}
	
	public void PlayChestOpen(){
		PlayLocationSound(chestOpen,Vector3.zero,.4f,1);
	}
	
	public void PlaySparkle1(){
		PlayLocationSound(sparkle1,Vector3.zero,.4f,1);
	}
	
	public void MonsterPain1(){
		PlayLocationSound(monsterPain1,Vector3.zero,.4f,1);
	}
	public void MonsterPain2(){
		PlayLocationSound(monsterPain2,Vector3.zero,.4f,1);
	}
	public void Zap1(Vector3 p){
		PlayLocationSound(zap1,p,1,1);
	}
	
	public void PlaySawBuzz(Vector3 p){
		PlayLocationSound(sawBuzz,p);	
	}
	
	public void PlayFizzle(Vector3 p){
		PlayLocationSound(fizzle,p);	
	}
	
	public void PlayInventoryCollect(Vector3 p){
		//		// commented Debug.Log ("HI");
		p = Player.inst.transform.position;
		PlayLocationSound(inventoryCollect,p);	
	}


	float invSoundTimeout = 0f;
	public void PlayInventoryOpen(float vol = 0.5f){
		if (invSoundTimeout < 0){
			invSoundTimeout = 0.4f;
			PlayFlatSound(inventoryOpen,vol);
		}

	}

	public void PlayInventoryClose(float vol = 0.5f){
		if (invSoundTimeout < 0){
			invSoundTimeout = 0.4f;
			PlayFlatSound(inventoryClose,vol);
		}

	}
	
	public void JetpackOutOfFuel(){
		
		PlayLocationSound(jetpackOutOfFuel);	
	}
	
	
	public void ElevatorArrivedDing(Vector3 p){
		//		// commented Debug.Log ("HI");
		//		p = Player.inst.transform.position;
		PlayLocationSound(elevatorArrivedDing,p);	
	}
	
	public void PlayHeartPickup(){
		PlayLocationSound(heartPickup);
	}
	
	public void PlayTakeDamage(){
		PlayLocationSound(takeDamage);
	}
	
	public void UnderwaterBubbles(){
		Vector3 pos = Player.inst.transform.position;
		float vol = Random.Range (0.7f,1f);
		float pitch = Random.Range (0.8f,1.1f);
		PlayLocationSound(underwaterBubbles,pos,vol,pitch);
	}

	public void PlayDepressingCannonSound(Vector3 pos, float pitch=1){
		float vol = Random.Range (0.7f,1f);
//		float pitch = Random.Range (0.8f,1.1f);
		PlayLocationSound(depressingCannonSound,pos,vol,pitch);
	}

	public void PlayVehicleCarStart(){
		PlayLocationSound(vehicleCarStart);
	}

	public void PlayVehicleCarRunning(float pitch){
		PlayLocationSound(vehicleCarRunning,Player.inst.transform.position,0.9f,pitch,0);
	}

	public void PlayVehicleCarStop(){
		if (LevelBuilder.inst.levelBuilderIsShowing) return;
		PlayLocationSound(vehicleCarStop,Player.inst.transform.position,0.5f,1);
	}


	public void PlayCheckpointSound(){
		PlayLocationSound(checkpoint,Player.inst.transform.position,0.7f,1,0);
	}

	public void PlayVehicleBoatStart(){
		PlayLocationSound(vehicleBoatStart,Player.inst.transform.position,.4f,1);
	}

	public void PlayVehicleBoatRunning(float pitch){
		PlayLocationSound(vehicleBoatRunning,Player.inst.transform.position,0.2f,pitch,0);
	}

	public void PlayVehicleBoatStop(){
		if (LevelBuilder.inst.levelBuilderIsShowing) return;
		PlayLocationSound(vehicleBoatStop,Player.inst.transform.position,.4f,1);
	}

	public void LevelBuilderSave(){
		PlayLocationSound(levelBuilderSaved,Vector3.zero,.55f,1,0);
	}

	public void PlayTimerDing(){
		PlaySound(timerDing);
	}

	public void PlayMachineRumble(Vector3 p, float vol=1,float pitch=1){
		PlayLocationSound(machineRumble,p,vol,pitch);
	}


	public void LevelBuilderPreview(){
		PlayLocationSound(levelBuilderPlay,Vector3.zero,.75f,1,0);
	}
	public void LevelBuilderOpened(){
		PlayLocationSound(levelBuilderOpened,Vector3.zero,.55f,1,0);
	}


	void PlaySound(AudioClip c, float vol=1,float pitch=1){
		PlayLocationSound(c,Vector3.zero,vol,pitch);
	}

	public void PlaySonicBoom(Vector3 p, float vol=1,float pitch=1){
		PlayLocationSound(sonicBoom,p,vol,pitch);
	}

	public void PlayElevatorArrived(Vector3 p){
		
		PlayLocationSound(elevatorArrivedDing,p,1,1,.9f);
	}

	public void PlayRiserArrived(Vector3 p){
		PlayLocationSound(riserArrived,p);
	}

	public void PlayArrowFire(Vector3 p){
		PlayLocationSound(arrowFired,p);
	}

	public void PlayMunch(Vector3 pos, float v=1, float p=1) {
//		Debug.Log("munch:"+pos);
		PlayLocationSound(munch,pos,v,p,0.8f);
	}

	public void PlayWhoosh(Vector3 pos, float v=1, float p=1) {
		//		Debug.Log("munch:"+pos);
		PlayLocationSound(whoosh,pos,v,p,0.8f);
	}

	public void PlayHeal1(Vector3 pos){
		//		Debug.Log("munch:"+pos);
		PlayLocationSound(heal1,pos,1,1,0.8f);
	}

	public void StartSlide(){
		slideSoundSource.Stop();
		slideSoundSource.Play();
	}

	public void StopSlide(){
		slideSoundSource.Stop();
	}
}
