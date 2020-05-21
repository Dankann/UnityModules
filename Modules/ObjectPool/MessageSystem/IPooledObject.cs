﻿/**
 * ObjectPooler.cs
 * Created by: Dankann Weissmuller
 * Created on: 15/08/18 (dd/mm/yy)
 */
public interface IPooledObject
{
    void OnEnableByPool();
    void OnDisableByPool();
}
