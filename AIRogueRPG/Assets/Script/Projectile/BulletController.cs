using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float lifeTime;

    private float bulletSpeed;

    private Vector2 lastPos;
    private Vector2 curPos;
    private Vector2 playerPos;
    // Start is called before the first frame update
    void Start() 
    {
        StartCoroutine(DeathDelay());
    }

    void Update()
    {
        curPos = transform.position;
        transform.position = Vector2.MoveTowards(transform.position, playerPos, bulletSpeed * Time.deltaTime);
        if(curPos == lastPos)
        {
            Destroy(gameObject);
        }
        lastPos = curPos;
    }

    public void GetPlayer(Transform player)
    {
        playerPos = player.position;
    }

    public void GetBulletSpeed(float speed){
        bulletSpeed = speed;
    }

    IEnumerator DeathDelay()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.CompareTag("Player")){
            // TO DO Damage To Player
            PlayerStatManager.Instance.DamagePlayer(true);
            Destroy(gameObject);
        }
        if(col.CompareTag("Obstacle")){
            Destroy(gameObject);
        }

        
    }
}
