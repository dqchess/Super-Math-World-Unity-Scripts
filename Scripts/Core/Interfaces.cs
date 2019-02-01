public interface IMyUpdateable
{
	void Update();
}

public interface IMyPickupable
{
	void OnPlayerPickup();
}

public interface IMuteDestroySound
{
	void MuteDestroy();
}

public interface IDestroyedByPlayer
{
	void DestroyedByPlayer();
}

public interface IMyDragEnded
{
	void DragEnded(Slot s);
}

public interface IMyGameStarted
{
	void GameStarted();
}

public interface IMyPlayerPickedUp
{
	void PlayerPickedUp();
}
public interface IMyPlayerDropped
{
	void PlayerDropped();
}


//public interface IOnPlayerThrow
//{
//	void OnPlayerThrow();
//}