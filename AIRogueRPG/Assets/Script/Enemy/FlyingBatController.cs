using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FlyingBatTrackingController : EnemyController
{
    private Color originalColor;
    private float originalAnimSpeed;
    private Vector3 playerPosition;
    private Transform childSpriteTransform;
    private Coroutine flyingCoroutine = null;
    private bool touchObstacle = false;
    private Rigidbody2D rb;

    public float chargingTime = 1.0f;
    public float flyingTime = 0.3f;

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
        if(GameController.Instance.GetPlayTime() < 0.5f) return;

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
                if(flyingCoroutine == null){
                    flyingCoroutine = StartCoroutine(ChargeAndFly());
                }
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
            if (direction.x > 0)
            {
                spriteRenderer.flipX = false;
            }
            else if (direction.x < 0)
            {
                spriteRenderer.flipX = true;
            }

        }
    }

    protected override void Follow()
    {
        canAttack = true;
        Vector2 newPosition = Vector2.MoveTowards(rb.position, player.transform.position, speed * Time.deltaTime);
        rb.MovePosition(newPosition);

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

    private IEnumerator ChargeAndFly()
    {
        canAttack = true;
        // Stop animation and set charging sprite
        playerPosition = player.transform.position;

        Vector3 direction = playerPosition - transform.position;

        // Wait for charging time
        animator.SetBool("chargeStart", true);
        yield return new WaitForSeconds(chargingTime);
        animator.SetBool("chargeStart", false);
        animator.SetBool("flyStart", true);

        // Set flying sprite and start flying
        direction = playerPosition - transform.position;
        if (direction.x > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (direction.x < 0)
        {
            spriteRenderer.flipX = true;
        }

        float elapsedTime = 0f;

        Vector2 startPos = rb.position;
        while (elapsedTime < flyingTime)
        {
            Vector2 newPosition = Vector2.Lerp(startPos, playerPosition, elapsedTime / flyingTime);
            rb.MovePosition(newPosition);
            elapsedTime += Time.deltaTime;

            if(touchObstacle){
                break;
            }

            yield return null;
        }

        animator.SetBool("flyStart", false);

        animator.speed = originalAnimSpeed;
        // Wait for 1 second before returning to charging state
        yield return new WaitForSeconds(chargingTime);
        flyingCoroutine = null;
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        // Don't Use Trigger
        // if (canAttack && other.CompareTag("Player"))
        // {
        //     Attack();
        // }
        // if(other.CompareTag("Obstacle")){
        //     touchObstacle = true;
        // }
    }

    private void OnTriggerExit2D(Collider2D other) {
        // if(other.CompareTag("Obstacle")){
        //     touchObstacle = false;
        // }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (canAttack && other.transform.CompareTag("Player"))
        {
            Attack();
        }
    }

    protected override void Attack()
    {
        if (!coolDownAttack)
        {
            PlayerStatManager.Instance.DamagePlayer(true);
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
