using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;




#region 旧版对象池
/// <summary>
///
/// </summary>
//public class PoolManager : SingletonMono<PoolManager>
//{
//    public Dictionary<string,Stack<GameObject>> PoolDic = new Dictionary<string,Stack<GameObject>>();
//    private string poolName;
//    private string folderName;
//    private string prefabName;
//    GameObject poolObject = null;
//    /// <summary>
//    /// 
//    /// </summary>
//    /// <param name="prefabName">对象池字典名字</param>
//    /// <returns></returns>
//    public GameObject GetPoolObjet(string prefabName, string folderName = null)
//    {

//        poolName = $"{prefabName}Pool";
//        GameObject obj = null;
//        if(PoolDic.ContainsKey(poolName) && PoolDic[poolName].Count > 0)
//        {
//            obj = PoolDic[poolName].Pop();
//            obj.SetActive(true);
//        }
//        //直接动态加载
//        else
//        {
//            obj = Instantiate(Resources.Load<GameObject>($"{folderName}/{prefabName}"));
//            obj.name = prefabName;
//        }
//        if (!transform.Find(poolName))
//            poolObject = new GameObject(poolName);

//        poolObject.transform.parent = transform;
//        obj.transform.parent = poolObject.transform;
//        return obj;
//    }

//    /// <summary>
//    /// 放入对象池
//    /// </summary>
//    /// <param name="name">放入抽屉的名字</param>
//    /// <param name="obj">放入的对象</param>
//    public void PutPoolObject(string prefabName, GameObject obj, string folderName = null)
//    {

//        poolName = $"{prefabName}Pool";
//        obj.SetActive(false);
//        if (!PoolDic.ContainsKey(poolName))
//            PoolDic.Add(poolName, new Stack<GameObject>());
//        PoolDic[poolName].Push(obj);
//        if (!transform.Find(poolName))
//        {
//            poolObject = new GameObject(poolName);
//            poolObject.transform.parent = transform;
//        }
//        obj.transform.parent = poolObject.transform;
//    }

//    public void ClearPool()
//    {
//        PoolDic.Clear();
//    }

//}

//public class PoolManager : SingletonMono<PoolManager>
//{
//    public Dictionary<string, Stack<GameObject>> PoolDic = new Dictionary<string, Stack<GameObject>>();

//    /// <summary>
//    /// 从对象池获取对象
//    /// </summary>
//    /// <param name="prefabName">预制体名称</param>
//    /// <param name="folderName">文件夹路径</param>
//    /// <returns>对象实例</returns>
//    public GameObject GetPoolObject(string prefabName, string folderName = null)
//    {
//        string poolName = $"{prefabName}Pool";
//        GameObject obj = null;

//        // 构建正确的资源路径
//        string resourcePath = string.IsNullOrEmpty(folderName) ? prefabName : $"{folderName}/{prefabName}";

//        Debug.Log($" 路径: {resourcePath}");

//        // 检查对象池中是否有可用对象
//        if (PoolDic.ContainsKey(poolName) && PoolDic[poolName].Count > 0)
//        {
//            // 安全地从对象池获取对象
//            while (PoolDic[poolName].Count > 0 && obj == null)
//            {
//                obj = PoolDic[poolName].Pop();

//                // 检查对象是否已被销毁
//                if (obj == null)
//                {
//                    Debug.LogWarning(" 对象池中存在已销毁的对象引用，跳过");
//                    continue;
//                }
//            }

//            if (obj != null)
//            {
//                obj.SetActive(true);
//                Debug.Log($" 从对象池获取: {prefabName}");
//            }
//        }

//        // 如果对象池为空或所有对象都已销毁，创建新对象
//        if (obj == null)
//        {
//            // 加载预制体
//            GameObject prefab = Resources.Load<GameObject>(resourcePath);
//            if (prefab == null)
//            {
//                Debug.LogError($" 路径不存在: {resourcePath}");
//                Debug.LogError($"1. 具体路径: Assets/Resources/{resourcePath}.prefab");
//                Debug.LogError($"2. 预制体名字: {prefabName}");
//                return null;
//            }

//            obj = Instantiate(prefab);
//            obj.name = prefabName;
//            Debug.Log($" 实例化预制体: {prefabName}");
//        }

//        // 设置父对象
//        SetObjectParent(obj, poolName);

//        return obj;
//    }

//    /// <summary>
//    /// 将对象放回对象池
//    /// </summary>
//    /// <param name="prefabName">预制体名称</param>
//    /// <param name="obj">要放回的对象</param>
//    public void PutPoolObject(string prefabName, GameObject obj)
//    {
//        if (obj == null)
//        {
//            Debug.LogWarning(" 要回收对象的引用为null,不能回收");
//            return;
//        }

//        string poolName = $"{prefabName}Pool";

//        // 初始化对象池（如果不存在）
//        if (!PoolDic.ContainsKey(poolName))
//        {
//            PoolDic.Add(poolName, new Stack<GameObject>());
//        }

//        // 停用对象
//        obj.SetActive(false);

//        // 放入对象池
//        PoolDic[poolName].Push(obj);

//        // 设置父对象
//        SetObjectParent(obj, poolName);

//        Debug.Log($" 对象已回收对应对象池: {prefabName}");
//    }

//    /// <summary>
//    /// 设置对象的父对象
//    /// </summary>
//    private void SetObjectParent(GameObject obj, string poolName)
//    {
//        if (obj == null) return;

//        // 查找或创建对象池的父对象
//        Transform poolParent = transform.Find(poolName);
//        if (poolParent == null)
//        {
//            GameObject newPoolParent = new GameObject(poolName);
//            newPoolParent.transform.SetParent(transform);
//            poolParent = newPoolParent.transform;
//        }

//        // 设置父对象
//        obj.transform.SetParent(poolParent);
//    }

//    /// <summary>
//    /// 预加载对象到对象池
//    /// </summary>
//    public void PreloadObjects(string prefabName, string folderName = "Prefabs", int count = 5)
//    {
//        string poolName = $"{prefabName}Pool";
//        string resourcePath = string.IsNullOrEmpty(folderName) ? prefabName : $"{folderName}/{prefabName}";

//        GameObject prefab = Resources.Load<GameObject>(resourcePath);
//        if (prefab == null)
//        {
//            Debug.LogError($" 预加载无效: 无法预加载预制体 {resourcePath}");
//            return;
//        }

//        // 初始化对象池
//        if (!PoolDic.ContainsKey(poolName))
//        {
//            PoolDic.Add(poolName, new Stack<GameObject>());
//        }

//        // 预创建对象
//        for (int i = 0; i < count; i++)
//        {
//            GameObject obj = Instantiate(prefab);
//            obj.name = prefabName;
//            PutPoolObject(prefabName, obj);
//        }

//        Debug.Log($" 预加载完成: {prefabName} x{count}");
//    }

//    /// <summary>
//    /// 清理对象池
//    /// </summary>
//    public void ClearPool(string prefabName = null)
//    {
//        if (prefabName != null)
//        {
//            // 清理特定对象池
//            string poolName = $"{prefabName}Pool";
//            if (PoolDic.ContainsKey(poolName))
//            {
//                // 销毁所有对象
//                foreach (GameObject obj in PoolDic[poolName])
//                {
//                    if (obj != null) Destroy(obj);
//                }
//                PoolDic.Remove(poolName);

//                // 销毁父对象
//                Transform poolParent = transform.Find(poolName);
//                if (poolParent != null) Destroy(poolParent.gameObject);

//                Debug.Log($"清空对象池: {prefabName}");
//            }
//        }
//        else
//        {
//            // 清理所有对象池
//            foreach (var pool in PoolDic)
//            {
//                foreach (GameObject obj in pool.Value)
//                {
//                    if (obj != null) Destroy(obj);
//                }

//                // 销毁父对象
//                Transform poolParent = transform.Find(pool.Key);
//                if (poolParent != null) Destroy(poolParent.gameObject);
//            }

//            PoolDic.Clear();
//            Debug.Log("清空缓存池");
//        }
//    }
//}
#endregion


#region 新版对象池系统
public class PoolManager : SingletonMono<PoolManager>
{
    private Dictionary<string, Queue<GameObject>> PoolDictionary = new Dictionary<string, Queue<GameObject>>();
    private Dictionary<string,GameObject> PoolGameObjectDiction = new Dictionary<string,GameObject>();
    
    public GameObject GetPoolItem(string itemName,GameObject itemObj,Transform parent = null,Transform itemParent = null)
    {
        if (PoolDictionary.ContainsKey(itemName))
        {
            GameObject getItem = PoolDictionary[itemName].Dequeue();
            if (parent != null)
            {
                getItem.transform.SetParent(parent);
            }
            return getItem;
        }
        else
        {
            CreatePool(itemName, itemObj);
            GameObject getItem = PoolDictionary[itemName].Dequeue();
            if (parent != null)
            {
                getItem.transform.SetParent(parent);
            }
            return getItem;
        }
    }

    public void PutPoolItem(string poolName,GameObject itemObj)
    {
        if (PoolDictionary.ContainsKey(poolName))
        {
            GameObject pool = GetPoolGameObject(poolName);
            PoolDictionary[poolName].Enqueue(itemObj);
            itemObj.transform.SetParent(pool.transform);
        }
        else
        {
            CreatePool(poolName, itemObj);
        }
    }


    private void CreatePool(string poolName,GameObject poolItem = null,int initCount = 5)
    {
        GameObject poolGameObject = new GameObject($"{poolName}Pool");
        PoolGameObjectDiction.Add($"{poolName}Pool", poolGameObject);
        poolGameObject.transform.parent = transform;
        Queue<GameObject> pool = new Queue<GameObject>();
        for(int i = 0;i<initCount; i++)
        {
            GameObject item = Instantiate(poolItem, poolGameObject.transform);
            item.SetActive(false);
            item.name = poolItem.name;
            pool.Enqueue(item);
        }
        PoolDictionary.Add(poolName, pool);
    }


    private GameObject GetPoolGameObject(string poolName)
    {
        return PoolGameObjectDiction[$"{poolName}Pool"];
    }

    public void ClearDesignatedPool(string poolName)
    {
        if (PoolDictionary.ContainsKey(poolName))
        {
            PoolDictionary[poolName].Clear();
            PoolDictionary.Remove(poolName);
        }
    }

    public void ClearAllPool() { PoolDictionary.Clear(); }
}


#endregion