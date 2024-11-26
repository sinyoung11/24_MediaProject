using UnityEngine;

public class Grid : MonoBehaviour
{
    public bool isObstacle = false;
    public bool isMonster = false;

    [SerializeField] private GameObject obstacle;

    public void ActiveObstacle(){
        obstacle.SetActive(true);
        isObstacle = true;
    }

    public void ActiveMonster(){
        isMonster = true;
    }
}
