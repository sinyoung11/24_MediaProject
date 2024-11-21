using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour
{
    private Vector2 attackDir;
    private Animator animator;

    [SerializeField]
    private Image coolDownImg;

    private float totalCoolTime = 2f;
    private float coolTime;
    private bool startCoolDown, fillCoolTime;
    private float attackSpeed = 2f;
    void Start()
    {
        animator = GetComponent<Animator>();
        coolTime = totalCoolTime;
        startCoolDown = false;
        fillCoolTime = false;
    }

    private void Update() {
        if (startCoolDown) {
            coolTime -= Time.deltaTime;
            coolDownImg.fillAmount = coolTime / totalCoolTime;

            if (coolTime <= 0f) {
                startCoolDown = false;
                StartCoroutine(StartFillCoolTime());
            }
        }

        if (fillCoolTime) {
            coolTime += Time.deltaTime * totalCoolTime;
            coolDownImg.fillAmount = coolTime / totalCoolTime;
            if (coolTime >= totalCoolTime) {
                coolTime = totalCoolTime;
                fillCoolTime = false;
            }
        }
    }

    IEnumerator StartFillCoolTime() {
        yield return new WaitForSeconds(totalCoolTime);
        fillCoolTime = true;
    }

    public void OnPlayerAttack(InputAction.CallbackContext context) {
        if (!PlayerStatManager.Instance.isWorking) return;

        if (context.phase == InputActionPhase.Started) {
            attackDir = context.ReadValue<Vector2>();
            animator.SetFloat("XDir", attackDir.x);
            animator.SetFloat("YDir", attackDir.y);

            if (coolTime >= totalCoolTime) {
                startCoolDown = true;
            }

            if (coolTime>0 && startCoolDown && !fillCoolTime) {
                Vector3 initPos = transform.position;
                initPos.y -= 0.5f;
                //GameObject water = Instantiate(attackObj, initPos, Quaternion.identity);
                GameObject water = ObjectPoolingManager.Instance.Pool.Get();
                water.transform.position = initPos;
                water.transform.rotation = Quaternion.identity;
                water.GetComponent<Water>().SetWaterSpeed(attackSpeed);
                water.GetComponent<Water>().ShootWater(attackDir);
            }
        }
       
    }

    public void SetAttackSpeed(float speed) {
        attackSpeed = speed;
    }

    public float GetAttackSpeed() {
        return attackSpeed;
    }
    
}
