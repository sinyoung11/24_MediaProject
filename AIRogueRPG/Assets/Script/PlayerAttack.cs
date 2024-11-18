using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    private Vector2 attackDir;
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();

    }

    public void OnPlayerAttack(InputAction.CallbackContext context) {
        if (!PlayerStatManager.Instance.isWorking) return;

        if (context.phase == InputActionPhase.Started) {
            attackDir = context.ReadValue<Vector2>();
            animator.SetFloat("XDir", attackDir.x);
            animator.SetFloat("YDir", attackDir.y);

            Vector3 initPos = transform.position;
            initPos.y -= 0.5f;
            //GameObject water = Instantiate(attackObj, initPos, Quaternion.identity);
            GameObject water = ObjectPoolingManager.Instance.Pool.Get();
            water.transform.position = initPos;
            water.transform.rotation = Quaternion.identity;
            water.GetComponent<Water>().ShootWater(attackDir);

        }
       
    }
    
}
