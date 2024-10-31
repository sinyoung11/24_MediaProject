using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    private new Rigidbody2D rigidbody;
    private Animator animator;
    private float speed = 2f;
    private Vector2 shootDir;
    private bool isShooting;
    private float lifeTime = 2f;
    // Start is called before the first frame update
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
                Destroy(gameObject);
            }
        }
    }

    private void FixedUpdate() {
        if(isShooting)
        rigidbody.MovePosition(rigidbody.position + shootDir * speed * Time.fixedDeltaTime);
    }


    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) return;
        animator.SetBool("Explode", true);
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
}
