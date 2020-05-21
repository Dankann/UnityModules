/**
 * ObjectPooler.cs
 * Created by: Dankann Weissmuller
 * Created on: 15/08/18 (dd/mm/yy)
**/

using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "PoolItem", menuName = "Pool System/Pool Item", order = 0)]
public class ObjectPoolItem : ScriptableObject
{
    public GameObject objectToPool;
    public int amountToPool;
    public bool shouldExpand;
    public PoolMessageType messageType;
}