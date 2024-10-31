using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoveController : MonoBehaviour
{
    private float speed = 1f;
    private float collisitionOffset = 0.05f;
    private Vector2 movement, attackDir;
    private new Rigidbody2D rigidbody;
    private ContactFilter2D movementFilter;
    private Animator animator;
    public bool isMoving { get; private set; }

    List<RaycastHit2D> castColisitions = new List<RaycastHit2D>();
    // Start is called before the first frame update
    void Start() {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update() {

    }

    private void FixedUpdate() {

        if (isMoving) {

            int count = rigidbody.Cast(
                movement,
                movementFilter,
                castColisitions,
                speed * Time.fixedDeltaTime + collisitionOffset
            );

            if (count <= 0) {
                rigidbody.MovePosition(rigidbody.position + movement * speed * Time.fixedDeltaTime);
            }

            animator.SetBool("IsWalking", true);
            animator.SetFloat("XDir", movement.x);
            animator.SetFloat("YDir", movement.y);

        }
        else {
            animator.SetBool("IsWalking", false);
        }
    }

    public void OnPlayerMove(InputAction.CallbackContext context) {
        movement = context.ReadValue<Vector2>();

        isMoving = movement != Vector2.zero;

    }

    /*public void OnPlayerAttack(InputAction.CallbackContext context) {
        attackDir = context.ReadValue<Vector2>();

        if (attackDir.x > 0) {
            Debug.Log("Shoot right");
        }
        else if (attackDir.x < 0) {
            Debug.Log("Shoot left");
        }
        else if (attackDir.y > 0) {
            Debug.Log("Shoot up");
        }
        else if (attackDir.y < 0) {
            Debug.Log("Shoot down");
        }

    }*/
}
