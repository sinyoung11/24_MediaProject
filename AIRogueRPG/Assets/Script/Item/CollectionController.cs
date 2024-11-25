﻿using UnityEngine;

[System.Serializable]
public class Item
{
    public string name;
    public string description;
    public Sprite itemImage;
}

public class CollectionController : MonoBehaviour
{

    public Item item;
    public float healthChange;
    public float moveSpeedChange;
    public float attackSpeedChange;
    public float bulletSizeChange;

    public ItemFunc itemFunc;
    public float amount;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = item.itemImage;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            // TO DO Item Effect
            // PlayerController.collectedAmount++;
            // GameController.HealPlayer(healthChange);
            // GameController.MoveSpeedChange(moveSpeedChange);
            // GameController.FireRateChange(attackSpeedChange);
            // GameController.BulletSizeChange(bulletSizeChange);
            // GameController.instance.UpdateCollectedItems(this);
            ItemController.Instance.GetItem(itemFunc);
            Destroy(gameObject);
        }
    }
}
