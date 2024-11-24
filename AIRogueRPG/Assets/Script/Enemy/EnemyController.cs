using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState
{
    Idle,
    Wander,
    Follow,
    Die,
    Attack
};

public enum EnemyType
{
    Melee,
    Ranged
};

public class EnemyController : MonoBehaviour
{
    GameObject player;
    public int enemyNum = -1;
    public RoomEnemyController roomEnemyController;
    public EnemyState currState = EnemyState.Idle;
    public EnemyType enemyType;
    private float healthPoint = 100.0f;
    public float range;
    public float speed;
    public float attackRange;
    public float bulletSpeed;
    public float coolDown;

    private bool upDetect = false;
    private bool downDetect = false;
    private bool leftDetect = false;
    private bool rightDetect = false;

    private bool chooseDir = false;
    private bool dead = false;
    private bool canAttack = false;
    private bool coolDownAttack = false;
    public bool notInRoom = false;
    private Vector3 targetPosition;
    public GameObject bulletPrefab;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        switch (currState)
        {
            case (EnemyState.Wander):
                Wander();
                break;
            case (EnemyState.Follow):
                Follow();
                break;
            case (EnemyState.Die):
                break;
            case (EnemyState.Attack):
                Follow();
                break;
        }

        if (!notInRoom)
        {
            if (IsPlayerInRange(range) && currState != EnemyState.Die)
            {
                currState = EnemyState.Follow;
            }
            else if (!IsPlayerInRange(range) && currState != EnemyState.Die)
            {
                currState = EnemyState.Wander;
            }
            if (Vector3.Distance(transform.position, player.transform.position) <= attackRange)
            {
                currState = EnemyState.Attack;
            }
        }
        else
        {
            currState = EnemyState.Idle;
        }
    }

    private bool IsPlayerInRange(float range)
    {
        return Vector3.Distance(transform.position, player.transform.position) <= range;
    }

    private IEnumerator ChooseTargetPosition()
    {
        chooseDir = true;
        yield return new WaitForSeconds(Random.Range(2f, 5f));
        Vector3 randomOffset = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0);
        targetPosition = transform.position + randomOffset;
        chooseDir = false;
    }

    public void ChangeDetectCollider(DetectDir detectDir, bool isActive)
    {
        switch (detectDir)
        {
            case DetectDir.Up:
                upDetect = isActive;
                break;
            case DetectDir.Down:
                downDetect = isActive;
                break;
            case DetectDir.Left:
                leftDetect = isActive;
                break;
            case DetectDir.Right:
                rightDetect = isActive;
                break;
        }
    }

    void Wander()
    {
        canAttack = false;
        if (!chooseDir)
        {
            StartCoroutine(ChooseTargetPosition());
        }
        else
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            // Check for collisions and change direction if needed
            if (upDetect && direction.y > 0)
            {
                ChooseAlternativeDirection();
            }
            else if (downDetect && direction.y < 0)
            {
                ChooseAlternativeDirection();
            }
            else if (leftDetect && direction.x < 0)
            {
                ChooseAlternativeDirection();
            }
            else if (rightDetect && direction.x > 0)
            {
                ChooseAlternativeDirection();
            }
        }
    }

    void ChooseAlternativeDirection()
    {
        List<Vector3> possibleDirections = new List<Vector3>();

        if (!upDetect)
        {
            possibleDirections.Add(Vector3.up);
        }
        if (!downDetect)
        {
            possibleDirections.Add(Vector3.down);
        }
        if (!leftDetect)
        {
            possibleDirections.Add(Vector3.left);
        }
        if (!rightDetect)
        {
            possibleDirections.Add(Vector3.right);
        }

        if (possibleDirections.Count > 0)
        {
            Vector3 chosenDirection = possibleDirections[Random.Range(0, possibleDirections.Count)];
            targetPosition = transform.position + chosenDirection * 5f;
        }
    }

    void Follow()
    {
        canAttack = true;
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(canAttack){
            Attack();
        }
    }

    private void Attack()
    {
        if (!coolDownAttack)
        {
            switch (enemyType)
            {
                case (EnemyType.Melee):
                    PlayerStatManager.Instance.DamagePlayer(true);
                    StartCoroutine(CoolDown());
                    break;
                case (EnemyType.Ranged):
                    GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity) as GameObject;
                    bullet.GetComponent<BulletController>().GetPlayer(player.transform);
                    bullet.AddComponent<Rigidbody2D>().gravityScale = 0;
                    bullet.GetComponent<BulletController>().isEnemyBullet = true;
                    StartCoroutine(CoolDown());
                    break;
            }
        }
    }

    private IEnumerator CoolDown()
    {
        coolDownAttack = true;
        yield return new WaitForSeconds(coolDown);
        coolDownAttack = false;
    }

    public void Damaged(float amount)
    {
        healthPoint -= amount;
        if (healthPoint <= 0.0f)
        {
            Death();
        }
    }

    public void Death()
    {
        roomEnemyController.DeleteEnemy(enemyNum);
        Destroy(gameObject);
    }
}
