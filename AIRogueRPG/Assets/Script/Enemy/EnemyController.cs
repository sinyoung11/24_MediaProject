using System.Collections;
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
    protected GameObject player;
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    public int enemyNum = -1;
    public RoomEnemyController roomEnemyController;
    public EnemyState currState = EnemyState.Idle;
    public EnemyType enemyType;
    public float healthPoint = 100.0f;
    public float range;
    public float speed;
    public float attackRange;
    public float coolDown;

    protected bool chooseDir = false;
    protected bool dead = false;
    protected bool canAttack = false;
    protected bool coolDownAttack = false;
    public bool notInRoom = false;
    protected Vector3 targetPosition;
    public GameObject bulletPrefab;

    protected virtual void Start() {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    protected virtual void Update() {}

    protected virtual bool IsPlayerInRange(float range) { return false; }

    protected virtual IEnumerator ChooseTargetPosition() { yield return null; }


    protected virtual void Wander() {}


    protected virtual void Follow() {}

    protected virtual void OnTriggerEnter2D(Collider2D other) {}

    protected virtual void Attack() {}

    protected virtual IEnumerator CoolDown() { yield return null; }

    public virtual void Damaged(float amount) {}

    public virtual void Death() {}
}