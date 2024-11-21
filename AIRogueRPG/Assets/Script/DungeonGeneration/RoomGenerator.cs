using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    public int minRooms = 5;  // 최소 방 갯수
    public int maxRooms = 15; // 최대 방 갯수
    private int totalRooms;
    public int randomFootSteps = 5; // 랜덤 풋 함수가 한 번에 이동하는 최대 횟수
    private Queue<Vector2Int> roomQueue = new Queue<Vector2Int>(); // 생성된 방 큐
    private HashSet<Vector2Int> visitedRooms = new HashSet<Vector2Int>(); // 방문한 방 좌표

    public Queue<Vector2Int> GenerateRoomQueue()
    {
        totalRooms = Random.Range(minRooms, maxRooms); // 생성할 방의 총 갯수
        Debug.Log("Generate Rooms: " + totalRooms);
        
        roomQueue.Enqueue(Vector2Int.zero);
        visitedRooms.Add(Vector2Int.zero);

        while (roomQueue.Count < totalRooms)
        {
            Vector2Int startPosition = FindClosestUnfilledPosition(Vector2Int.zero);
            RandomFoot(startPosition, randomFootSteps);
        }

        return roomQueue;
    }

    private void RandomFoot(Vector2Int start, int steps)
    {
        Vector2Int currentPosition = start;
        roomQueue.Enqueue(currentPosition);
        visitedRooms.Add(currentPosition);
        steps--;

        for (int i = 0; i < steps; i++)
        {
            // 4방향 중 랜덤하게 선택
            Vector2Int direction = GetRandomDirection();
            currentPosition += direction;

            // 처음 가는 위치라면 큐에 추가하고 방문 표시
            if (!visitedRooms.Contains(currentPosition))
            {
                roomQueue.Enqueue(currentPosition);
                visitedRooms.Add(currentPosition);

                // 생성할 방의 목표 수를 달성하면 중단
                if (roomQueue.Count >= totalRooms)
                    break;
            }
        }
    }

    private Vector2Int GetRandomDirection()
    {
        List<Vector2Int> directions = new List<Vector2Int>
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };
        return directions[Random.Range(0, directions.Count)];
    }

    private Vector2Int FindClosestUnfilledPosition(Vector2Int origin) {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(origin);

        List<Vector2Int> directions = new List<Vector2Int>
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right,
            Vector2Int.up + Vector2Int.left,
            Vector2Int.up + Vector2Int.right,
            Vector2Int.down + Vector2Int.left,
            Vector2Int.down + Vector2Int.right
        };

        HashSet<Vector2Int> visited = new HashSet<Vector2Int> { origin };

        while (queue.Count > 0)
        {
            Vector2Int currentPosition = queue.Dequeue();

            if (!visitedRooms.Contains(currentPosition))
            {
                return currentPosition;
            }

            for(int i = 0; i < directions.Count; i++){
                Vector2Int nextPosition = currentPosition + directions[i];

                // 좌표가 맵 범위를 넘는지 확인하고, 범위를 넘는 경우 큐에 추가하지 않음
                if (!visited.Contains(nextPosition) && 
                    Mathf.Abs(nextPosition.x) <= maxRooms && 
                    Mathf.Abs(nextPosition.y) <= maxRooms)
                {
                    queue.Enqueue(nextPosition);
                    visited.Add(nextPosition);
                }
            }
        }

        // 가능한 위치를 찾지 못한 경우
        Debug.Log("Error Can't Find Unfilled Position!");
        return origin;
    }
}
