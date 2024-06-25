using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Singleton instance
    private static PlayerController _instance;

    // Public property to access the instance
    public static PlayerController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PlayerController>();
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    _instance = singletonObject.AddComponent<PlayerController>();
                    singletonObject.name = typeof(PlayerController).ToString() + " (Singleton)";
                }
            }
            return _instance;
        }
    }

    public List<EnemyController> isChasedByEnemy = new List<EnemyController>();

    private void Awake()
    {
        // If there is already an instance and it's not this, destroy this
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject); // Optional: If you want the singleton to persist across scenes
        }
    }

    private void Update()
    {
        HeartBeatControl();
    }

    public void HeartBeatControl()
    {
        if (CheckIsEnemyChase())
        {
            HeartBeatController.Instance.SetBlinkInterval(1);
        }
        else
        {
            HeartBeatController.Instance.SetBlinkInterval(3);
        }
    }

    public bool CheckIsEnemyChase()
    {
        return isChasedByEnemy.Count > 0;
    }

    public void AddChase(EnemyController enemyController)
    {
        if (!isChasedByEnemy.Contains(enemyController))
        {
            isChasedByEnemy.Add(enemyController);
        }
    }
    public void RemoveChase(EnemyController enemyController)
    {
        if (isChasedByEnemy.Contains(enemyController))
        {
            isChasedByEnemy.Remove(enemyController);
        }
    }
}
