using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BatTrackingController : EnemyController
{
    private Color originalColor;
    private float originalAnimSpeed;
    private Rigidbody2D rb;

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        originalColor = spriteRenderer.color;
        originalAnimSpeed = animator.speed;
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

        }
    }



    protected override void Follow()
    {
        canAttack = true;
        animator.speed = originalAnimSpeed * 1.25f;
        Vector2 newPosition = Vector2.MoveTowards(rb.position, player.transform.position, speed * Time.deltaTime);
        rb.MovePosition(newPosition);
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        // Didn't Use Trigger More
        // if (canAttack && other.CompareTag("Player"))
        // {
        //     Attack();
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
