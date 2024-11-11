using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomEnemyController : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GridController grid;
    [SerializeField] private Room room;

    private List<EnemyController> enemyList = new List<EnemyController>();
    private int enemyCount = -1;
    private int minEnemyNum = 5;
    private int maxEnemyNum = 10;

    public void GenerateMonster(){
        int randomNum = Random.Range(minEnemyNum, maxEnemyNum);

        for(int i = 0; i < randomNum; i++){
            int randomPos = Random.Range(0, grid.availablePoints.Count - 1 );
            GameObject enemyObject = Instantiate(enemyPrefab, grid.availablePoints[randomPos], Quaternion.identity, transform);
            grid.availablePoints.RemoveAt(randomPos);

            EnemyController enemyController = enemyObject.GetComponent<EnemyController>();
            enemyController.roomEnemyController = this;
            enemyController.enemyNum = i;
            enemyList.Add(enemyController);
        }
        enemyCount = enemyList.Count;
    }

    public void DeleteEnemy(int num){
        enemyCount--;
        CheckDoor();
    }


    private void CheckDoor(){
        if(enemyCount <=0){
            room.UnLockDoor();
        }
    }
}
