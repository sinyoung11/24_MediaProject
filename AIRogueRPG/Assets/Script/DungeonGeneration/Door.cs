using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
public class Door : MonoBehaviour
{
    public enum DoorType
    {
        left, right, top, bottom
    }

    public bool isDoorActive = false;
    public DoorType doorType;
    public GameObject doorCollider;
    public SpriteRenderer doorSprite;

    private GameObject player;
    private float widthOffset = 4f;
    private Room connectedRoom;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void SetRoom(Room room)
    {
        connectedRoom = room;
    }

    public void LockDoor(bool goLock){
        Color color = doorSprite.color;
        if(goLock){
            color = Color.black;
            isDoorActive = false;
        }
        else{
            color = Color.red;
            isDoorActive = true;
        }
        doorSprite.color = color;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(!isDoorActive) return;
        Room nextRoom = null;
        if (other.tag == "Player")
        {
            Vector2 newPlayerPosition = transform.position;

            switch (doorType)
            {
                case DoorType.bottom:
                    newPlayerPosition.y -= widthOffset;
                    nextRoom = connectedRoom?.GetBottom();
                    break;
                case DoorType.left:
                    newPlayerPosition.x -= widthOffset;
                    nextRoom = connectedRoom?.GetLeft();
                    break;
                case DoorType.right:
                    newPlayerPosition.x += widthOffset;
                    nextRoom = connectedRoom?.GetRight();
                    break;
                case DoorType.top:
                    newPlayerPosition.y += widthOffset;
                    nextRoom = connectedRoom?.GetTop();
                    break;
            }

            // 플레이어 위치 업데이트
            player.transform.position = newPlayerPosition;

            // 방 이동 처리
            if (nextRoom != null)
            {
                RoomController.instance.OnPlayerEnterRoom(nextRoom);
            }
        }
    }
}
