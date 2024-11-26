using UnityEngine;

public class Door : MonoBehaviour
{
    public enum DoorType { left, right, top, bottom }

    public bool isDoorActive = false;
    public DoorType doorType;
    // public GameObject doorCollider;
    public SpriteRenderer doorSprite;

    private float widthOffset = 4f;
    private Room connectedRoom;
    
    public void SetRoom(Room room)
    {
        connectedRoom = room;
    }

    public void LockDoor(bool goLock){
        Color color = doorSprite.color;
        if(goLock){
            color.a = 0.0f;
            isDoorActive = false;
        }
        else{
            color.a = 1.0f;
            isDoorActive = true;
        }
        doorSprite.color = color;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(!isDoorActive) return;
        Room nextRoom = null;
        if (other.CompareTag("Player"))
        {
            switch (doorType)
            {
                case DoorType.bottom:
                    nextRoom = connectedRoom?.GetBottom();
                    nextRoom.MovePlayerPosition(DoorType.top);
                    break;
                case DoorType.left:
                    nextRoom = connectedRoom?.GetLeft();
                    nextRoom.MovePlayerPosition(DoorType.right);
                    break;
                case DoorType.right:
                    nextRoom = connectedRoom?.GetRight();
                    nextRoom.MovePlayerPosition(DoorType.left);
                    break;
                case DoorType.top:
                    nextRoom = connectedRoom?.GetTop();
                    nextRoom.MovePlayerPosition(DoorType.bottom);
                    break;
            }

            // 방 이동 처리
            if (nextRoom != null)
            {
                RoomController.instance.OnPlayerEnterRoom(nextRoom);
            }
        }
    }
}
