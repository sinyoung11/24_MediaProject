using System.Collections;
using System.Collections.Generic;
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

    private int totalHeartNum = 3;
    private float currentHp;
    private Transform hp;

    public void Awake() {
        if(instance == null) {
            instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        currentHp = totalHeartNum;
        hp = GameObject.Find("Hp").transform;
        for(int i=0; i < totalHeartNum; i++) {
            GameObject heart = Instantiate(heartHpPref);
            heart.transform.SetParent(hp);
        }
    }

    public void DamagePlayer(bool isWeak) {

        if (isWeak) { // -0.5
            currentHp -= 0.5f;
        }
        else { // 1
            currentHp -= 1;
        }

        int fullHeartNum = currentHp <= 0 ? 0 : (int)currentHp;

        for(int i=0; i<totalHeartNum; i++) {
            Slider heartSlider = hp.GetChild(i).GetChild(0).GetComponent<Slider>();
            if (i < fullHeartNum) heartSlider.value = 1;
            else if (i == fullHeartNum) heartSlider.value = currentHp - fullHeartNum;
            else heartSlider.value = 0;
        }
        if (currentHp <= 0) {
            Debug.Log("Player died");
        }
    }

    //for testing
    public void TestWeakAttack() {
        DamagePlayer(true);
    }

    public void TestStrongAttack() {
        DamagePlayer(false);
    }
}
