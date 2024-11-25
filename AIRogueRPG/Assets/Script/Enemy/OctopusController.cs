using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class OctopusController : EnemyController
{
    [SerializeField]private Color originalColor;
    private float originalAnimSpeed;
    private Transform childSpriteTransform;
    private Rigidbody2D rb;
    
    public float bulletSpeed;

    protected override void Start()
    {
        base.Start();
        originalColor = spriteRenderer.color;
        originalAnimSpeed = animator.speed;
        childSpriteTransform = spriteRenderer.transform;
        rb = GetComponent<Rigidbody2D>();
    }

    protected override void Update()
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
                Wander();
                Attack();
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

        Vector3 direction = player.transform.position - transform.position;
        if (direction.x > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (direction.x < 0)
        {
            spriteRenderer.flipX = true;
        }
    }

    protected override bool IsPlayerInRange(float range)
    {
        return Vector3.Distance(transform.position, player.transform.position) <= range;
    }

    protected override IEnumerator ChooseTargetPosition()
    {
        chooseDir = true;
        yield return new WaitForSeconds(Random.Range(2f, 5f));
        Vector3 randomOffset = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0);
        targetPosition = transform.position + randomOffset;
        chooseDir = false;
    }

    public override void ChangeDetectCollider(DetectDir detectDir, bool isActive)
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

    protected override void Wander()
    {
        canAttack = false;
        animator.speed = originalAnimSpeed;
        if (!chooseDir)
        {
            StartCoroutine(ChooseTargetPosition());
        }
        else
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            Vector2 newPosition = rb.position + (Vector2)(direction * speed * 0.5f * Time.deltaTime);
            rb.MovePosition(newPosition);

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

    protected override void ChooseAlternativeDirection()
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

    protected override void Follow()
    {
        canAttack = true;
        Vector2 newPosition = Vector2.MoveTowards(rb.position, player.transform.position, speed * Time.deltaTime);
        rb.MovePosition(newPosition);
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (canAttack && other.CompareTag("Player"))
        {
            
        }
        if(other.CompareTag("Obstacle")){
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.CompareTag("Obstacle")){
        }
    }

    protected override void Attack()
    {
        if (!coolDownAttack)
        {
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity) as GameObject;
            bullet.GetComponent<BulletController>().GetPlayer(player.transform);
            bullet.AddComponent<Rigidbody2D>().gravityScale = 0;
            bullet.GetComponent<BulletController>().GetBulletSpeed(bulletSpeed);
            StartCoroutine(CoolDown());
        }
    }

    protected override IEnumerator CoolDown()
    {
        coolDownAttack = true;
        yield return new WaitForSeconds(coolDown);
        coolDownAttack = false;
    }

    public override void Damaged(float amount)
    {
        StartCoroutine(FlashRed());
        healthPoint -= amount;
        if (healthPoint <= 0.0f)
        {
            Death();
        }
    }

    private IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.3f);
        spriteRenderer.color = originalColor;
    }

    public override void Death()
    {
        roomEnemyController.DeleteEnemy(enemyNum);
        Destroy(gameObject);
    }
}
