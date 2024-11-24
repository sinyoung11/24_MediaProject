using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DetectDir{
    None,
    Up,
    Down,
    Left,
    Right,
}

public class EnemyDetectCollider : MonoBehaviour
{
    [SerializeField] private EnemyController enemyController;
    [SerializeField] private DetectDir detectDir;

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Obstacle")){
            enemyController.ChangeDetectCollider(detectDir, true);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.CompareTag("Obstacle")){
            enemyController.ChangeDetectCollider(detectDir, false);
        }
    }

}
