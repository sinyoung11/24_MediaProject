using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolingManager : MonoBehaviour
{
    public static ObjectPoolingManager Instance;

    public int capacity = 10;
    public int maxPoolSize = 15;

    [SerializeField]
    private GameObject attackObj;

    public IObjectPool<GameObject> Pool { get; private set; }

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        Pool = new ObjectPool<GameObject>(CreatePooledObject, OnTakeObjFromPool,
            OnReturnToPool, OnDestroyObjInPool, true, capacity, maxPoolSize);
    }

    private GameObject CreatePooledObject() {
        GameObject waterObj = Instantiate(attackObj);
        waterObj.GetComponent<Water>().Pool = Pool;
        waterObj.transform.SetParent(transform);
        return waterObj;
    }

    private void OnTakeObjFromPool(GameObject obj) {
        obj.SetActive(true);
    }

    private void OnReturnToPool(GameObject obj) {
        obj.SetActive(false);
    }

    private void OnDestroyObjInPool(GameObject obj) {
        Destroy(obj);
    }
}
