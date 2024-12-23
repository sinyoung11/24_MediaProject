using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Water : MonoBehaviour
{
    private new Rigidbody2D rigidbody;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private float speed = 2f;
    private float damage = 35.0f;
    private Vector2 shootDir;
    private bool isShooting;
    private float lifeTime = 2f;

    [SerializeField]
    private Sprite defaultSprite;
    
    public IObjectPool<GameObject> Pool { get; set; }
    // Start is called before the first frame update

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("water_attack_anim") == true) {
            float animTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            if (animTime >= 1.0f) { //animation ended
                //Destroy(gameObject);
                Pool.Release(gameObject);
                spriteRenderer.sprite = defaultSprite;
            }
        }
    }

    private void FixedUpdate() {
        if(isShooting)
        rigidbody.MovePosition(rigidbody.position + shootDir * speed * Time.fixedDeltaTime);
    }


    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player") || collision.CompareTag("Water")) return;
        animator.SetBool("Explode", true);
        if (collision.CompareTag("Enemy")) {
            EnemyController enemy = collision.gameObject.GetComponent<EnemyController>();
            enemy.Damaged(damage);

        }
    }

    public void ShootWater(Vector2 dir) {
        shootDir = dir;
        isShooting = true;
        StartCoroutine(StartExplode());
    }

    IEnumerator StartExplode() {
        yield return new WaitForSeconds(lifeTime);
        animator.SetBool("Explode", true);
    }

    public void SetWaterSpeed(float speed) {
        this.speed = speed;
    }

    public float GetWaterSpeed() {
        return speed;
    }
}
