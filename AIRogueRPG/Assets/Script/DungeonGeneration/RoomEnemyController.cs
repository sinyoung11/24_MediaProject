using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomEnemyController : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GridController grid;
    [SerializeField] private Room room;

    private List<EnemyController> enemyList = new List<EnemyController>();
    private int minEnemyNum = 5;
    private int maxEnemyNum = 10;

    public void GenerateMonster(){
        int randomNum = Random.Range(minEnemyNum, maxEnemyNum);

        for(int i = 0; i < randomNum; i++){
            int randomPos = Random.Range(0, grid.availablePoints.Count - 1 );
            GameObject enemyObject = Instantiate(enemyPrefab, grid.availablePoints[randomPos], Quaternion.identity, transform);
            grid.availablePoints.RemoveAt(randomPos);

            EnemyController enemyController = enemyObject.GetComponent<EnemyController>();
            enemyController.enemyNum = i;
            enemyList.Add(enemyController);
        }
    }

    public void DeleteEnemy(int num){
        enemyList.RemoveAt(num);
        CheckDoor();
    }

    public bool isRemainEnemy(){
        return enemyList.Count > 0;
    }

    private void CheckDoor(){
        if(!isRemainEnemy()){
            room.UnLockDoor();
        }
    }
}
