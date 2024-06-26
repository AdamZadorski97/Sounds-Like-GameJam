using BNG;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // Singleton instance
    private static PlayerController _instance;

    // Public property to access the instance

    public bool isHide;

    public CanvasGroup gameOverScreen;
    public AudioSource audioSource;
    public AudioClip screamClip;
    public AudioClip getDamageClip;
    [SerializeField] private float lookAtDuration = 1.0f;
    [SerializeField] private Transform cameraRotation;
    [SerializeField] private List<Image> healthImages; // List of images representing health points

    private int currentHealthIndex;
    public CheckSpell checkSpell;
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
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        // Check if the object the wand collided with is a projectile
        ProjectileController projectile = collision.gameObject.GetComponent<ProjectileController>();
        if (projectile != null)
        {
            TakeDamage(1);
        }
    }
    private void Start()
    {
        currentHealthIndex = healthImages.Count - 1; // Initialize the health index
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

    public void TakeDamage(float damageAmount)
    {
        audioSource.PlayOneShot(getDamageClip);
        if (currentHealthIndex >= 0)
        {
            healthImages[currentHealthIndex].gameObject.SetActive(false);
            currentHealthIndex--;

            if (currentHealthIndex < 0)
            {
                GameOver(transform.position); // Call GameOver when health is depleted
            }
        }
    }

    public void HidePlayer(bool state)
    {
        isHide = state;
    }

    public void GameOver(Vector3 vector3)
    {
        audioSource = GetComponent<AudioSource>();
        GetComponent<CharacterController>().enabled = false;
        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(2f); // Wait for 2 seconds
        sequence.Append(gameOverScreen.DOFade(1, 0.8f)); // Then fade
        sequence.Play();
        StartCoroutine(LerpLookAt(vector3));
        audioSource.PlayOneShot(screamClip, 1);
    }

    private IEnumerator LerpLookAt(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - cameraRotation.position;
        direction.y = 0; // Ignore the Y axis to only rotate on the horizontal plane

        Quaternion startRotation = cameraRotation.rotation;
        Quaternion endRotation = Quaternion.LookRotation(direction);
        float elapsedTime = 0f;

        while (elapsedTime < lookAtDuration)
        {
            cameraRotation.rotation = Quaternion.Lerp(startRotation, endRotation, elapsedTime / lookAtDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cameraRotation.rotation = endRotation; // Ensure it ends exactly at the target rotation
    }
}
