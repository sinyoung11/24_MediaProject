using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileController : MonoBehaviour
{
    public Vector2 initialDirection;
    public float lifeTime;
    public float missileSpeed;
    public float turnSpeed;

    private float freeTime = 1.0f;

    private Transform player;
    [SerializeField] private Rigidbody2D rb;

    void Start()
    {
        SetInitialDirection();
        StartCoroutine(DeathDelay());
    }

    void FixedUpdate()
    {
        if (player != null)
        {
            // 현재 방향과 목표 방향 계산
            Vector2 dir = transform.right;
            Vector2 targetDir = player.position - transform.position;

            // 목표를 바라보는 방향 계산 (외적 사용)
            Vector3 crossVec = Vector3.Cross(dir, targetDir);
            float inner = Vector3.Dot(Vector3.forward, crossVec);
            float addAngle = inner > 0 ? turnSpeed * Time.fixedDeltaTime : -turnSpeed * Time.fixedDeltaTime;
            float saveAngle = addAngle + transform.rotation.eulerAngles.z;

            // 오브젝트 회전 적용
            transform.rotation = Quaternion.Euler(0, 0, saveAngle);

            // 이동 방향 설정
            float moveDirAngle = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
            Vector2 moveDir = new Vector2(Mathf.Cos(moveDirAngle), Mathf.Sin(moveDirAngle));
            rb.velocity = moveDir * missileSpeed;
        }
        freeTime -= Time.deltaTime;
    }

    public void SetTarget(Transform target)
    {
        player = target;
    }

    public void SetInitialDirection()
    {
        if (initialDirection != Vector2.zero)
        {
            rb.velocity = initialDirection.normalized * missileSpeed;
        }
    }

    public void FireUpwards()
    {
        transform.rotation = Quaternion.Euler(0, 0, 90);
        SetInitialDirection();
    }

    public void FireDownwards()
    {
        transform.rotation = Quaternion.Euler(0, 0, -90);
        SetInitialDirection();
    }

    IEnumerator DeathDelay() {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            // TO DO Damage To Player
            PlayerStatManager.Instance.DamagePlayer(true);
            Destroy(gameObject);
        }
        if (col.CompareTag("Obstacle"))
        {
            if(freeTime > 0 ) return;
            Destroy(gameObject);
        }
    }
}
