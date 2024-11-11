using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    public static RoomController instance;
    [SerializeField] private RoomGenerator roomGenerator;
    [SerializeField] private GameObject roomPrefabObject;

    public Room currRoom;

    public List<Room> loadedRooms = new List<Room>();
    private Queue<Vector2Int> roomQueue = new Queue<Vector2Int>(); 

    bool spawnedBossRoom = false;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        roomQueue = roomGenerator.GenerateRoomQueue();

        // roomQueue에 있는 각 위치에 방을 생성
        while (roomQueue.Count > 0)
        {
            Vector2Int roomPosition = roomQueue.Dequeue();
            CreateRoom(roomPosition.x, roomPosition.y);
        }

        ///// load 된 룸에게 다 문 업데이트 적용
        foreach(Room _room in loadedRooms){
            _room.RemoveUnconnectedDoors();
        }

        // 게임 시작 시 (0, 0) 위치의 방에서 시작
        currRoom = FindRoom(0, 0);
        if (currRoom != null)
        {
            OnPlayerEnterRoom(currRoom);
        }
    }

    // 지정된 좌표에 Room을 생성하는 메서드
    private void CreateRoom(int x, int y)
    {
        Vector3 roomPosition = new Vector3(x * roomPrefabObject.GetComponent<Room>().Width, 
                                           y * roomPrefabObject.GetComponent<Room>().Height, 
                                           0);
        
        // Room 프리팹을 인스턴스화하여 생성
        GameObject newRoomObj = Instantiate(roomPrefabObject, roomPosition, Quaternion.identity);
        newRoomObj.name = $"Room ({x}, {y})";
        Room newRoom = newRoomObj.GetComponent<Room>();

        // Room의 좌표 설정
        newRoom.X = x;
        newRoom.Y = y;
        newRoom.MakeDoor();

        // 방을 로드된 방 리스트에 추가
        loadedRooms.Add(newRoom);
    }

    // 방이 존재하는지 확인하는 함수
    public bool DoesRoomExist(int x, int y)
    {
        return loadedRooms.Exists(room => room.X == x && room.Y == y);
    }

    // 특정 좌표에 있는 방을 찾는 함수
    public Room FindRoom(int x, int y)
    {
        return loadedRooms.Find(room => room.X == x && room.Y == y);
    }

    // 플레이어가 방에 들어갈 때 호출되는 함수
    public void OnPlayerEnterRoom(Room room)
    {
        currRoom = room;
        currRoom.roomEnemyController.GenerateMonster();
        currRoom.SetCameraBoundary();
        Debug.Log("Player entered room at: " + room.X + ", " + room.Y);

        // 카메라나 UI 등을 업데이트하는 추가적인 작업을 여기에 추가할 수 있습니다.
    }

}
