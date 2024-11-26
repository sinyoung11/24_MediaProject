using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatManager : MonoBehaviour
{
    private static PlayerStatManager instance = null;
    public static PlayerStatManager Instance {
        get {
            if (instance == null) return null;
            return instance;
        }
    }

    [SerializeField]
    private GameObject heartHpPref;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private int totalHeartNum = 3;
    private float currentHp;
    private Transform hp;
    private Animator animator;

    public bool isWorking { get; private set; }

    public void Awake() {
        if(instance == null)
        {
            instance = this;
        }
        animator = GetComponent<Animator>();
    }
    
    void Start()
    {
        
        currentHp = totalHeartNum;
        hp = GameObject.Find("Hp").transform;
        for(int i=0; i < totalHeartNum; i++) {
            GameObject heart = Instantiate(heartHpPref);
            heart.transform.SetParent(hp);
        }
        isWorking = true;

        spriteRenderer = this.GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    public void DamagePlayer(bool isWeak) {

        StartCoroutine(FalshRed());

        if (isWeak) { // -0.5
            ScreenFade.Instance.Fade(Color.red, 0.1f); // 빨간색

            currentHp -= 0.5f;
            GameController.Instance.AddLostHealth(0.5f);
        }
        else { // 1
            ScreenFade.Instance.Fade(Color.red, 0.3f); // 빨간색

            currentHp -= 1;
            GameController.Instance.AddLostHealth(1.0f);
        }

        ApplyHpUI();
        
        if (currentHp <= 0 && GameController.Instance.GetGameStarted()) {
            GameController.Instance.GameEnd(false);
            Debug.Log("Player died");
        }
    }

    private IEnumerator FalshRed(){
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.3f);
        spriteRenderer.color = originalColor;
    }

    private void ApplyHpUI() {
        int fullHeartNum = currentHp <= 0 ? 0 : (int)currentHp;
        for (int i = 0; i < totalHeartNum; i++) {
            Slider heartSlider = hp.GetChild(i).GetChild(0).GetComponent<Slider>();
            if (i < fullHeartNum) heartSlider.value = 1;
            else if (i == fullHeartNum) heartSlider.value = currentHp - fullHeartNum;
            else heartSlider.value = 0;
        }
    }

    public void HealPlayer(float amount) {
        currentHp += amount;
        if (currentHp >= totalHeartNum) currentHp = totalHeartNum;
        ScreenFade.Instance.Fade(Color.green, 0.15f); // 초록색, 강도 0.5
        ApplyHpUI();
    }

    //for testing
    public void TestHealItem() {
        ItemController.Instance.GetItem(ItemFunc.Heal);
    }

    public void TestAttackSpeedItem() {
        ItemController.Instance.GetItem(ItemFunc.AttackSpeedUp);
    }
    public void TestMovementSpeedItem() {
        ItemController.Instance.GetItem(ItemFunc.MoveSpeedUp);
    }

    public void SetIsWorking(bool isWorking) {
        this.isWorking = isWorking;

        if (!isWorking) {
            animator.enabled = false;
        }
    }
}
