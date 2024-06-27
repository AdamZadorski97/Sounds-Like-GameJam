using BNG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerBreath : MonoBehaviour
{
    [SerializeField] private AudioClip Breath;
    [SerializeField] private AudioClip TakeBreath;
    [SerializeField] private AudioSource AudioSource;
    [SerializeField] private float BreathRange;
    [SerializeField] private float TakeRange;
    [SerializeField] private float BreathSpeed;

    [SerializeField] private EnemyAgroController EnemyAgro;
    [SerializeField] private UnityEvent BreathAgro;

    private bool BreathBool = true;

    private void Start()
    {
        AudioSource.clip = Breath;
        EnemyAgro.detectionRange = BreathRange;
        StartCoroutine(BreathCorutine());
    }

    private void Update()
    {
        if (InputBridge.Instance.AButtonDown)
        {
            BreathBool = false;
            EnemyAgro.detectionRange = TakeRange;
            AudioSource.clip = TakeBreath;
            AudioSource.Play();
        }

        if (InputBridge.Instance.AButtonUp)
        {
            BreathBool = true;
            EnemyAgro.detectionRange = BreathRange;
            AudioSource.clip = Breath;
            StartCoroutine(BreathCorutine());
        }
    }

    private IEnumerator BreathCorutine()
    {
        while (BreathBool)
        {
            yield return new WaitForSeconds(BreathSpeed);
            AudioSource.Play();
            BreathAgro.Invoke();.
        }
    }




}
