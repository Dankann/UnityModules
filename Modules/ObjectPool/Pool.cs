/**
 * ObjectPooler.cs
 * Created by: Dankann Weissmuller
 * Created on: 14/08/18 (dd/mm/yy)
**/

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PoolList", menuName = "Pool System/Pool List", order = 0)]
public class Pool : ScriptableObject
{    
    public List<ObjectPoolItem> items;
}
