using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{

    [SerializeField]
    private GameObject attackObj;

    private Vector2 attackDir;
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPlayerAttack(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Started) {
            attackDir = context.ReadValue<Vector2>();
            animator.SetFloat("XDir", attackDir.x);
            animator.SetFloat("YDir", attackDir.y);

            Vector3 initPos = transform.position;
            initPos.y -= 0.5f;
            GameObject water = Instantiate(attackObj, initPos, Quaternion.identity);
            water.GetComponent<Water>().ShootWater(attackDir);

        }
        

    }
    
}
