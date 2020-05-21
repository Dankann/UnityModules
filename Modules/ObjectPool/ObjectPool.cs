/**
 * ObjectPool.cs
 * Created by: Dankann Weissmuller
 * Created on: 14/08/18 (dd/mm/yy)
**/

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class ObjectPool : Singleton<ObjectPool>
{
    [Serializable]
    public class ListWrapper
    {
        [SerializeField]
        public List<GameObject> myList = new List<GameObject>();
    }

    [Serializable]
    public class QueueWrapper
    {
        [SerializeField]
        public Queue<GameObject> myList = new Queue<GameObject>();
    }

    static PoolMessageSender messageRoot;

    //static List<ListWrapper> pooledObjects;
    static List<QueueWrapper> availableObjects;
    static List<ListWrapper> unavailableObjects;

    static Dictionary<GameObject, int> poolPrefabs = new Dictionary<GameObject, int>();
    static bool doneOnce;

    public Pool poolAsset;

    /// <summary>
    /// Create/Update object pool on awake and every time a new level is loaded
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        CreatePool();
    }

    /// <summary>
    /// Get an available object from the pool, otherwise instantiate a new one
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    static GameObject GetInstance(int index)
    {
        GameObject objInstance = null;
        while (objInstance == null)
        {
            if (availableObjects[index].myList.Count > 0)
            {
                objInstance = availableObjects[index].myList.Dequeue();
                if(objInstance != null)
                {
                    Instance.Notify(index, objInstance.transform, true);
                    objInstance.SetActive(true);
                    unavailableObjects[index].myList.Add(objInstance);
                    return objInstance;
                }
                else
                {
                    continue;
                }
            }
            else if(unavailableObjects[index].myList.Count < Instance.poolAsset.items[index].amountToPool || Instance.poolAsset.items[index].shouldExpand)
            {
                Instance.CreateNewInstance(index);
                return GetInstance(index);
            }
            else
            {
                return null;
            }
        }
        return null;
    }
    
    /// <summary>
    /// Get an available object from the pool, otherwise instantiate a new one
    /// </summary>
    /// <param name="prefab"></param>
    /// <returns></returns>
    public static GameObject GetInstance(GameObject prefab)
    {
        var index = poolPrefabs[prefab];
        return GetInstance(index);
    }
    
    /// <summary>
    /// Get an available object from the pool, otherwise instantiate a new one. Set it's parent
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public static GameObject GetInstance(GameObject prefab, Transform parent)
    {
        var temp = GetInstance(prefab);
        temp.transform.SetParent(parent);
        return temp;
    }
    
    /// <summary>
    /// Get an available object from the pool, otherwise instantiate a new one. Set it's parent, position and rotation
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public static GameObject GetInstance(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
    {
        var temp = GetInstance(prefab, parent);
        temp.transform.position = position;
        temp.transform.rotation = rotation;
        return temp;
    }

    /// <summary>
    /// Disable an instance GameObject from the pool, and make it available for recycle
    /// </summary>
    /// <param name="instance"></param>
    public static void RecycleInstance(GameObject instance)
    {
        for (int j = 0; j < unavailableObjects.Count; j++)
        {
            if (unavailableObjects[j].myList.Contains(instance))
            {
                unavailableObjects[j].myList.Remove(instance);
                availableObjects[j].myList.Enqueue(instance);
                instance.SetActive(false);
                Instance.Notify(j, instance.transform, false);
                instance.transform.SetParent(Instance.transform);                
                return;
            }
        }
        //Debug.LogWarningFormat("Instance {0} doesnt exist in the object pool", instance);
    }

    /// <summary>
    /// Create the object pool
    /// </summary>
    void CreatePool()
    {
        ///Update Dictionary if needed
        foreach (ObjectPoolItem obj in poolAsset.items)
        {
            if (!poolPrefabs.ContainsKey(obj.objectToPool))
            {
                poolPrefabs.Add(obj.objectToPool, poolAsset.items.IndexOf(obj));
            }
        }

        if (!doneOnce)
        {            
            availableObjects = new List<QueueWrapper>(poolAsset.items.Count);
            unavailableObjects = new List<ListWrapper>(poolAsset.items.Count);
        }

        for (int i = 0; i < unavailableObjects.Count; i++)
        {
            //Check for null instances
            for (int j = 0; j < unavailableObjects[i].myList.Count; j++)
            {
                if (unavailableObjects[i].myList[j] != null)
                {
                    unavailableObjects[i].myList[j].transform.SetParent(transform);
                    availableObjects[i].myList.Enqueue(unavailableObjects[i].myList[j]);
                }
                unavailableObjects[i].myList.RemoveAt(j);
                j--;
            }
        }

        for (int i = 0; i < availableObjects.Capacity; i++)
        {
            if (!doneOnce)
            {
                availableObjects.Add(new QueueWrapper());
                unavailableObjects.Add(new ListWrapper());
            }            
            //Create new instances if needed
            while (availableObjects[i].myList.Count + unavailableObjects[i].myList.Count < poolAsset.items[i].amountToPool)
            {
                CreateNewInstance(i);
            }
        }
        
        doneOnce = true;
    }
    
    /// <summary>
    /// Instantiate and initialize object in the pool
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    GameObject CreateNewInstance(int index)
    {
        GameObject obj = Instantiate(poolAsset.items[index].objectToPool, transform);
        obj.SetActive(false);
        availableObjects[index].myList.Enqueue(obj);
        return obj;
    }
    
    /// <summary>
    /// Send or Broadcast message for MessageRoot Object when enablind or disabling an object from the pool
    /// </summary>
    /// <param name="prefabIndex"></param>
    /// <param name="instanceIndex"></param>
    /// <param name="enable"></param>
    void Notify(int prefabIndex, Transform instance, bool enable)
    {
        messageRoot = new PoolMessageSender(instance);
        switch (poolAsset.items[prefabIndex].messageType)
        {
            case PoolMessageType.SendMessage:
                messageRoot.Send(enable);
                break;

            case PoolMessageType.BroadcastMessage:
                messageRoot.Broadcast(enable, true);
                break;
        }
    }
}
