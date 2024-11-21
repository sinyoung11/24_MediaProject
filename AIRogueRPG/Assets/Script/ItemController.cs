using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ItemFunc { Heal, MoveSpeedUp, AttackSpeedUp}
public class ItemInfo {
    public Sprite sprite;
    public ItemFunc itemFunc;
    public float amount;
    public int count;
    public int index;
}
public class ItemController : MonoBehaviour
{
    [SerializeField]
    private Transform ItemListUI;

    [SerializeField]
    private Transform ItemStatusListUI;

    [SerializeField]
    private List<Transform> itemList;

    [SerializeField]
    private GameObject itemUIPref, itemStatusUIPref;

    private List<ItemInfo> itemInfoList = new List<ItemInfo>();
    private PlayerAttack playerAttack;
    private PlayerMoveController playerMoveController;
    private int itemLifetime = 5;

    private static ItemController instance = null;
    public static ItemController Instance {
        get {
            if (instance == null) return null;
            return instance;
        }
    }

    private void Awake() {
        if (instance == null) {
            instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        InitializeAllItemInfo();
        InitializeItemListUI();
        Transform player = GameObject.FindWithTag("Player").transform;
        playerAttack = player.GetComponent<PlayerAttack>();
        playerMoveController = player.GetComponent<PlayerMoveController>();
    }

    private void InitializeAllItemInfo() {
        int idx = 0;
        foreach(var item in itemList) {           
            CollectionController collectionController = item.GetComponent<CollectionController>();
            ItemInfo itemInfo = SetItemInfoByController(collectionController);
            itemInfo.index = idx;
            itemInfoList.Add(itemInfo);
            idx++;
        }
    }

    private void InitializeItemListUI() {
        foreach(var itemInfo in itemInfoList) {
            GameObject itemUI = Instantiate(itemUIPref, ItemListUI);
            Button itemBtn = itemUI.transform.GetChild(0).GetComponent<Button>();
            itemBtn.GetComponent<Image>().sprite =
                itemInfo.sprite;
            itemUI.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "x " + itemInfo.count;
            itemBtn.onClick.AddListener(delegate { UseItem(itemInfo); });
        }        
    }

    private ItemInfo SetItemInfoByController(CollectionController collectionController) {
        ItemInfo itemInfo = new ItemInfo();
        if(collectionController.healthChange > 0) {
            itemInfo.itemFunc = ItemFunc.Heal;
            itemInfo.amount = collectionController.healthChange;
        }else if(collectionController.attackSpeedChange > 0) {
            itemInfo.itemFunc = ItemFunc.AttackSpeedUp;
            itemInfo.amount = collectionController.attackSpeedChange;
        }else if (collectionController.moveSpeedChange > 0) {
            itemInfo.itemFunc = ItemFunc.MoveSpeedUp;
            itemInfo.amount = collectionController.moveSpeedChange;
        }
        itemInfo.count = 0;
        itemInfo.sprite = collectionController.item.itemImage;
        return itemInfo;
    }

    public void GetItem(ItemFunc itemFunc) {
        for(int i = 0; i < itemInfoList.Count; i++) {
            if(itemInfoList[i].itemFunc == itemFunc) {
                itemInfoList[i].count++;
                ItemListUI.GetChild(i).GetChild(1).GetComponent<TextMeshProUGUI>().text = "x " + itemInfoList[i].count;
                break;
            }
        }
    }

    void UseItem(ItemInfo itemInfo) {
        if (itemInfo.count == 0) return;

        switch (itemInfo.itemFunc) {
            case ItemFunc.Heal: {
                    PlayerStatManager.Instance.HealPlayer(itemInfo.amount);
                }break;
            case ItemFunc.AttackSpeedUp: {
                    float originalSpeed = playerAttack.GetAttackSpeed();
                    playerAttack.SetAttackSpeed(originalSpeed + itemInfo.amount);
                    StartCoroutine(StartItemTimer(originalSpeed, itemInfo));
                }
                break;
            case ItemFunc.MoveSpeedUp: {
                    float originalSpeed = playerMoveController.GetMovementSpeed();
                    playerMoveController.SetMovementSpeed(originalSpeed + itemInfo.amount);
                    StartCoroutine(StartItemTimer(originalSpeed, itemInfo));
                }
                break;
        }

        itemInfo.count--;
        Transform itemUI = ItemListUI.GetChild(itemInfo.index);
        itemUI.GetChild(1).GetComponent<TextMeshProUGUI>().text = "x " + itemInfo.count;
    }

    IEnumerator StartItemTimer(float originVal, ItemInfo itemInfo) {
        GameObject itemStatusUI = Instantiate(itemStatusUIPref, ItemStatusListUI);
        itemStatusUI.transform.GetChild(0).GetComponent<Image>().sprite = itemInfo.sprite;
        Image sliderImg = itemStatusUI.transform.GetChild(1).GetChild(0).GetComponent<Image>();
        bool timerWorking = true;
        float timerValue = itemLifetime;
        while (timerWorking) {
            timerValue -= Time.deltaTime;
            sliderImg.fillAmount = timerValue / itemLifetime;
            yield return new WaitForSeconds(0.001f);

            if (timerValue <= 0f) {
                timerWorking = false;
            }
        }

        Destroy(itemStatusUI);

        if(itemInfo.itemFunc == ItemFunc.MoveSpeedUp) {
            playerMoveController.SetMovementSpeed(originVal);
        }else if(itemInfo.itemFunc == ItemFunc.AttackSpeedUp) {
            playerAttack.SetAttackSpeed(originVal);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
