using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    public List<Grid> availableGrids = new List<Grid>();
    private List<Grid> delGrids = new List<Grid>();
    public GameObject bossObstacles;
    private int gridWidth = 14;
    private int gridHeight = 6;
    public int obstacleCount = 8;

    public void GenerateObstacle(bool isBossRoom){
        if(isBossRoom){
            bossObstacles.SetActive(true);
            return;
        }

        int centerMinX = gridWidth / 2 - 2;
        int centerMaxX = gridWidth / 2 + 2;
        int centerMinY = gridHeight / 2 - 1;
        int centerMaxY = gridHeight / 2 + 1;

        while(obstacleCount > 0){
            int index = Random.Range(0, availableGrids.Count);

            int x = index & gridWidth;
            int y = index / gridWidth;

            // Skip the central area
            if (x >= centerMinX && x < centerMaxX && y >= centerMinY && y < centerMaxY)
            {
                continue;
            }

            availableGrids[index].ActiveObstacle();
            delGrids.Add(availableGrids[index]);
            obstacleCount--;
        }

        foreach(Grid grid in delGrids){
            availableGrids.Remove(grid);
        }
    }

    

}
