using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoveController : MonoBehaviour
{
    private static PlayerMoveController instance;
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");
    private static readonly int XDir = Animator.StringToHash("XDir");
    private static readonly int YDir = Animator.StringToHash("YDir");

    public static PlayerMoveController Instance{
        get{
            if(instance == null) return null;
            return instance;
        }
    }
    private float speed = 3f;
    private float collisitionOffset = 0.05f;
    private Vector2 movement, attackDir;
    private new Rigidbody2D rigidbody;
    private ContactFilter2D movementFilter;
    private Animator animator;
    public bool isMoving { get; private set; }

    List<RaycastHit2D> castColisitions = new List<RaycastHit2D>();

    void Awake(){
        if(instance == null){
            instance = this;
        }
        else{
            Debug.LogError("PlayerMoveController already has instance");
            Destroy(this.gameObject);
        }
        
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate() {
        if (!PlayerStatManager.Instance.isWorking) return;
        if (isMoving) {

            int count = rigidbody.Cast(
                movement,
                movementFilter,
                castColisitions,
                speed * Time.fixedDeltaTime + collisitionOffset
            );

            if (count <= 0) {
                rigidbody.MovePosition(rigidbody.position + movement * (speed * Time.fixedDeltaTime));
            }

            animator.SetBool(IsWalking, true);
            animator.SetFloat(XDir, movement.x);
            animator.SetFloat(YDir, movement.y);
        }
        else {
            animator.SetBool(IsWalking, false);
        }
    }

    public void OnPlayerMove(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();

        isMoving = movement != Vector2.zero;
    }

    public void SetMovementSpeed(float speed) {
        this.speed = speed;
    }

    public float GetMovementSpeed() {
        return speed;
    }

    public void MovePlayerPosition(Vector3 worldPosition){
        this.transform.position = worldPosition;
    }
}
