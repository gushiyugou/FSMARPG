using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetEffActive : MonoBehaviour
{
    public float time;
    private void OnEnable()
    {
        Invoke("SetPrefabActive", time);
    }


    private void SetPrefabActive()
    {
        gameObject.SetActive(false);
        //transform.rotation = Quaternion.identity;
        PoolManager.Instance.PutPoolItem(gameObject.name, gameObject);
    }
}
