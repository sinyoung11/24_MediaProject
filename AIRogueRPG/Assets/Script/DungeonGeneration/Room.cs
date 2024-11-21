using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public int Width;
    public int Height;
    public int X;
    public int Y;
    public bool isBossRoom = false;
    public RoomEnemyController roomEnemyController;

    private bool updatedDoors = false;

    public Door leftDoor;
    public Door rightDoor;
    public Door topDoor;
    public Door bottomDoor;

    public List<Door> doors = new List<Door>();

    public Room(int x, int y)
    {
        X = x;
        Y = y;
    }

    // 문을 설정하는 메서드
    public void MakeDoor()
    {
        Door[] ds = GetComponentsInChildren<Door>(); 
        foreach (Door d in ds)
        {
            doors.Add(d);
            d.SetRoom(this); // Door에 Room 설정
            switch (d.doorType)
            {
                case Door.DoorType.right:
                    rightDoor = d;
                    break;
                case Door.DoorType.left:
                    leftDoor = d;
                    break;
                case Door.DoorType.top:
                    topDoor = d;
                    break;
                case Door.DoorType.bottom:
                    bottomDoor = d;
                    break;
            }
        }
        LockDoor();
    }

    public void LockDoor(){
        rightDoor.LockDoor(true);
        leftDoor.LockDoor(true);
        topDoor.LockDoor(true);
        bottomDoor.LockDoor(true);
    }

    public void UnLockDoor(){
        rightDoor.LockDoor(false);
        leftDoor.LockDoor(false);
        topDoor.LockDoor(false);
        bottomDoor.LockDoor(false);
    }


    public void RemoveUnconnectedDoors()
    {
        foreach (Door door in doors)
        {
            switch (door.doorType)
            {
                case Door.DoorType.right:
                    if (GetRight() == null)
                        door.gameObject.SetActive(false);
                    break;
                case Door.DoorType.left:
                    if (GetLeft() == null)
                        door.gameObject.SetActive(false);
                    break;
                case Door.DoorType.top:
                    if (GetTop() == null)
                        door.gameObject.SetActive(false);
                    break;
                case Door.DoorType.bottom:
                    if (GetBottom() == null)
                        door.gameObject.SetActive(false);
                    break;
            }
        }
    }

    public Room GetRight() => RoomController.instance.DoesRoomExist(X + 1, Y) ? RoomController.instance.FindRoom(X + 1, Y) : null;
    public Room GetLeft() => RoomController.instance.DoesRoomExist(X - 1, Y) ? RoomController.instance.FindRoom(X - 1, Y) : null;
    public Room GetTop() => RoomController.instance.DoesRoomExist(X, Y + 1) ? RoomController.instance.FindRoom(X, Y + 1) : null;
    public Room GetBottom() => RoomController.instance.DoesRoomExist(X, Y - 1) ? RoomController.instance.FindRoom(X, Y - 1) : null;

    public Vector3 GetRoomCentre() => new Vector3(X * Width, Y * Height);

    public void MovePlayerPosition(Door.DoorType doorType){
        Vector3 movePosition = Vector3.zero;
        switch(doorType){
            case Door.DoorType.right:
                movePosition = rightDoor.transform.position;
                movePosition.x -= rightDoor.transform.lossyScale.x;
                break;
            case Door.DoorType.left:
                movePosition = leftDoor.transform.position;
                movePosition.x += leftDoor.transform.lossyScale.x;
                break;
            case Door.DoorType.top:
                movePosition = topDoor.transform.position;
                movePosition.y -= topDoor.transform.lossyScale.x;
                break;
            case Door.DoorType.bottom:
                movePosition = bottomDoor.transform.position;
                movePosition.y += bottomDoor.transform.lossyScale.x;
                break;
        }

        PlayerMoveController.Instance.MovePlayerPosition(movePosition);
        
    }

    public void SetCameraBoundary(){
        Vector2 minPoint = this.transform.position;
        minPoint.x -= Width / 2.0f;
        minPoint.y -= Height / 2.0f;
        Vector2 maxPoint = this.transform.position;
        maxPoint.x += Width / 2.0f;
        minPoint.y += Height / 2.0f;

        CameraController.Instance.ChangeCameraBoundary(minPoint, maxPoint);
    }

    public void GenerateMonster(){
        roomEnemyController.GenerateMonster(isBossRoom);
    }
}
