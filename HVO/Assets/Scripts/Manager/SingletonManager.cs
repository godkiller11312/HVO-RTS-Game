using UnityEngine;

public abstract class SingletonManager<T> : MonoBehaviour where T : MonoBehaviour   
{
    private void Awake()
    {
        T[] manager = FindObjectsByType<T>(FindObjectsSortMode.None);
        if (manager.Length > 1)
        {
            Destroy(gameObject);
            return;
        }
    }
    public static T Get()
    {
        var tag = typeof(T).Name;       
        GameObject managerObject = GameObject.FindWithTag(tag);
        if (managerObject == null)
        {
            return managerObject.GetComponent<T>();
        }
        GameObject go = new(tag);
        go.tag = tag;
        return go.AddComponent<T>();
    }
 
}
