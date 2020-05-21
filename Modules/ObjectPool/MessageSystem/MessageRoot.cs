/**
 * ObjectPooler.cs
 * Created by: Dankann Weissmuller
 * Created on: 15/08/18 (dd/mm/yy)
**/

using UnityEngine;

public class PoolMessageSender
{
    public PoolMessageSender(Transform root)
    {
        this.root = root;
    }

    Transform root;

    public void Send(bool enable)
    {
        MonoBehaviour[] behaviours = root.GetComponents<MonoBehaviour>();

        foreach (MonoBehaviour behaviour in behaviours)
            if (behaviour is IPooledObject)
            {
                if (enable)
                    (behaviour as IPooledObject).OnEnableByPool();
                else
                    (behaviour as IPooledObject).OnDisableByPool();
            }
    }

    public void Broadcast(bool enable, bool notifyInactiveObjects)
    {
        foreach (MonoBehaviour behaviour in root.GetComponentsInChildren<MonoBehaviour>(notifyInactiveObjects))
            if (behaviour is IPooledObject)
            {
                if (enable)
                    (behaviour as IPooledObject).OnEnableByPool();
                else
                    (behaviour as IPooledObject).OnDisableByPool();
            }
    }
}
