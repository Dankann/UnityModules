/**
 * Singleton.cs
 * Created by: Joao Borks
 * Created on: 16/10/18 (dd/mm//yy)
 */

using UnityEngine;

/// <summary>
/// Base class for Singleton objects
/// These Singleton objects are not destroyed on load and can be created from code if no active instance is found on the scene
/// </summary>
/// <typeparam name="T"><see cref="MonoBehaviour"/> type to become a Singleton</typeparam>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    /// <summary>
    /// The only active instance of the <see cref="T"/> class
    /// </summary>
    public static T Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType<T>();
                // If no instance is found, then create a new object with the Singleton attached
                if (!instance)
                    instance = new GameObject(typeof(T).Name, typeof(T)).GetComponent<T>();
            }
            return instance;
        }
    }

    /// <summary>
    /// Local reference to the only active <see cref="T"/> class
    /// </summary>
    static T instance;

    protected virtual void Awake()
    {
        if (instance && instance != this)
        {
            Destroy(gameObject);
            Debug.LogWarningFormat("Cleaning duplicated singleton object of type {0}", typeof(T).Name);
            return;
        }
        instance = GetComponent<T>();
        // To avoid being destroyed on load, the object can't have a transform parent
        if (transform.parent != null)
            transform.SetParent(null, true);
        DontDestroyOnLoad(gameObject);
    }
}