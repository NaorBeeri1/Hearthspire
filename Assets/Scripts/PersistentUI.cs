using UnityEngine;

public class PersistentUI : MonoBehaviour
{
    private static PersistentUI Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}