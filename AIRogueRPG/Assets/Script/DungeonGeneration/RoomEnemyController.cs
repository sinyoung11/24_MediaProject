using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomEnemyController : MonoBehaviour
{
    [SerializeField] private GameObject batTrackingPrefab;
    [SerializeField] private GameObject batFlyingPrefab;
    [SerializeField] private GameObject OctopusPrefab;

    private int[] batTrackingBound = {6, 9};
    private int[] batFlyingBound = {6, 9};
    private int[] OctopusBound = {6, 9};

    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private GridController grid;
    [SerializeField] private Room room;

    private List<EnemyController> enemyList = new List<EnemyController>();
    private int enemyCount = -1;
    private int minEnemyNum = 5;
    private int maxEnemyNum = 9;

    public void GenerateMonster(bool isBossRoom){
        
        if(isBossRoom){
            int centerPos = grid.availableGrids.Count / 2;
            GameObject bossObject = Instantiate(bossPrefab, grid.availableGrids[centerPos].transform.position, Quaternion.identity, transform);
            grid.availableGrids.RemoveAt(centerPos);

            BossController bossController = bossObject.GetComponent<BossController>();
            bossController.roomEnemyController = this;
            bossController.enemyNum = 1;
        }
        else{

            float batTrackingRatio = Random.Range(batTrackingBound[0], batTrackingBound[1]);
            float batFlyingRatio = Random.Range(batFlyingBound[0], batFlyingBound[1]);
            float octopusRatio = Random.Range(OctopusBound[0], OctopusBound[1]);
            float ratioSum = batTrackingRatio + batFlyingRatio + octopusRatio;

            if(GameController.Instance.GetLevelData() != null && GameController.Instance.GetLevelData().enemy_count != 0){
                minEnemyNum = (int)(GameController.Instance.GetLevelData().enemy_count * 0.75f);
                maxEnemyNum = (int)(GameController.Instance.GetLevelData().enemy_count * 1.25f);
            }
            else{
                Debug.LogError("Can not Take GetLevelData From GameController");
            }
            

            int randomNum = Random.Range(minEnemyNum, maxEnemyNum);

            int batTrackingCount = (int)(randomNum * (batTrackingRatio / ratioSum));
            int batFlyingCount = (int)(randomNum * (batFlyingRatio / ratioSum));
            int octopusCount = (int)(randomNum * (octopusRatio / ratioSum));

            int index = 0;
            for(int i = 0; i < batTrackingCount; i++){
                GenerateMonster(batTrackingPrefab, index);
                index++;
                randomNum--;
            }

            for(int i = 0; i < batFlyingCount; i++){
                GenerateMonster(batFlyingPrefab, index);
                index++;
                randomNum--;
            }

            for(int i = 0; i < octopusCount; i++){
                GenerateMonster(OctopusPrefab, index);
                index++;
                randomNum--;
            }
            
            for(int i = 0; i < randomNum; i++){
                GenerateMonster(batTrackingPrefab, index);
                index++;
            }
            

            enemyCount = enemyList.Count;
        }
        
    }

    private void GenerateMonster(GameObject enemyPrefab, int index){
        int randomPos = Random.Range(0, grid.availableGrids.Count - 1 );
        GameObject enemyObject = Instantiate(enemyPrefab, grid.availableGrids[randomPos].transform.position, Quaternion.identity, transform);
        grid.availableGrids.RemoveAt(randomPos);

        EnemyController enemyController = enemyObject.GetComponent<EnemyController>();
        enemyController.roomEnemyController = this;
        enemyController.enemyNum = index;
        enemyList.Add(enemyController);
    }

    public void DeleteEnemy(int num){
        enemyCount--;
        CheckDoor();
    }


    private void CheckDoor(){
        if(enemyCount <=0){
            room.UnLockDoor();
            room.SpawnItem();
        }
    }
}
