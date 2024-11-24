using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : EnemyController
{
    [SerializeField]private Color originalColor;
    private float originalAnimSpeed;
    private Transform childSpriteTransform;
    
    public float bulletSpeed;


    public GameObject missilePrefab;

    private bool usingMissile = false;
    private bool canUseMissile = false;
    private float missileTime = 8.0f;
    private int missileCount = 7;
    private bool isGroggy = false;
    private float groggyTime = 3.0f;

    protected override void Start()
    {
        base.Start();
        originalColor = spriteRenderer.color;
        originalAnimSpeed = animator.speed;
        childSpriteTransform = spriteRenderer.transform;
        StartCoroutine(CoolDownMissile());
    }

    protected override void Update()
    {
        if(isGroggy) return;

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
            transform.position += direction * speed * 0.5f * Time.deltaTime;
            if (direction.x > 0)
            {
                spriteRenderer.flipX = false;
            }
            else if (direction.x < 0)
            {
                spriteRenderer.flipX = true;
            }

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
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);

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
        if(usingMissile){
            return;
        }

        if(canUseMissile){
            StartCoroutine(ShootMissile());
            return;
        }

        if (!coolDownAttack)
        {
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity) as GameObject;
            bullet.GetComponent<BulletController>().GetPlayer(player.transform);
            bullet.AddComponent<Rigidbody2D>().gravityScale = 0;
            bullet.GetComponent<BulletController>().GetBulletSpeed(bulletSpeed);
            bullet.transform.localScale *= 2f;
            StartCoroutine(CoolDown());
        }
        Wander();
    }

    IEnumerator ShootMissile(){
        usingMissile = true;
        canUseMissile = false;

        for(int i = 0;  i < missileCount; i++){
            GameObject missile_1 = Instantiate(missilePrefab, transform.position, Quaternion.identity) as GameObject;
            missile_1.GetComponent<MissileController>().SetTarget(player.transform);
            missile_1.GetComponent<MissileController>().missileSpeed = bulletSpeed * 0.75f;
            missile_1.GetComponent<MissileController>().FireUpwards();
            missile_1.transform.localScale *= 2f;
            // 2

            GameObject missile_2 = Instantiate(missilePrefab, transform.position, Quaternion.identity) as GameObject;
            missile_2.GetComponent<MissileController>().SetTarget(player.transform);
            missile_2.GetComponent<MissileController>().missileSpeed = bulletSpeed * 0.75f;
            missile_2.GetComponent<MissileController>().FireDownwards();
            missile_2.transform.localScale *=2f;

            yield return new WaitForSeconds(0.5f);
        }

        usingMissile = false;

        StartCoroutine(GroggyCoroutine());
        StartCoroutine(CoolDownMissile());
    }

    IEnumerator CoolDownMissile(){
        yield return new WaitForSeconds(missileTime);
        canUseMissile = true;
    }



    IEnumerator GroggyCoroutine(){
        isGroggy = true;
        yield return new WaitForSeconds(groggyTime);
        isGroggy = false;
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
        GameController.Instance.GameEnd(true);
        roomEnemyController.DeleteEnemy(enemyNum);
        Destroy(gameObject);
    }
}
