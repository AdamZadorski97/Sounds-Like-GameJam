using BNG;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Singleton instance
    private static PlayerController _instance;

    // Public property to access the instance

    public bool isHide;

    public CanvasGroup gameOverScreen;
    public AudioSource audioSource;
    public AudioClip screamClip;


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
    private bool isChase;
    public void HeartBeatControl()
    {
        if (CheckIsEnemyChase())
        {
            if (!isChase)
            {
                isChase = true;
                HeartBeatController.Instance.SetBlinkInterval(true);
                MusicController.Instance.ChangeMusicByName("Chase");
            }
        }
        else
        {
            if (isChase)
            {
                isChase = false;
                HeartBeatController.Instance.SetBlinkInterval(false);
                MusicController.Instance.ChangeMusicByName("Calm");
            }
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


    public void HidePlayer(bool state)
    {
        isHide = state;
    }

    public void GameOver()
    {
        GetComponent<BNGPlayerController>().enabled = false;
        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(2f); // Wait for 1 second
        sequence.Append(gameOverScreen.DOFade(1, 0.8f)); // Then fade
        sequence.Play();

        audioSource.PlayOneShot(screamClip);
    }
}
