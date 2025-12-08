using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static object _lock = new object();
    private static bool _wasDestroyed;
    public static T Instance
    {
        get
        {
            if (_instance) return _instance;
                if (!_instance && !_wasDestroyed)
                {
                    _instance = FindFirstObjectByType<T>();
                    if (!_instance)
                    {
                        var go = new GameObject(typeof(T).Name);
                        _instance = go.AddComponent<T>();
                    }
                }
                return _instance;
            
        }
    }

    protected virtual bool PersistAcrossScenes => true;

    

    protected virtual void Awake()
    {
        if (_instance && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this as T;

        if (PersistAcrossScenes)
            DontDestroyOnLoad(gameObject);

        OnSingletonAwake();
    }

    protected virtual void OnDestroy()
    {
        if (_instance == this) _instance = null;
        
        _wasDestroyed = true;
    }
    protected virtual void OnSingletonAwake() { }
}
