using UnityEngine;

public class SingletonPersistant<T> : MonoBehaviour where T : Component
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<T>();
                if (instance == null)
                {
                    Debug.Log("You are missing a singletonPersistant in your scene.");
                }
            }
            return instance;
        }
    }

    protected virtual void Awake()
    {
        DontDestroyOnLoad(this);
        if (instance == null)
        {
            instance = this as T;
        }
        else
        {
            Destroy(gameObject);
            Debug.Log("You have a duplicate singleton in your scene. Self destructing.");
        }
    }
}
