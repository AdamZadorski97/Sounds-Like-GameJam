using System.Collections;
using UnityEngine;
using DG.Tweening;
using VLB;

public class LightController : MonoBehaviour
{
    [SerializeField] private Light _light;

    [Header("Flicker Timing")]
    [SerializeField] private float minTimeToNextFlicker = 0.5f; // Minimum time before next flicker
    [SerializeField] private float maxTimeToNextFlicker = 2.0f; // Maximum time before next flicker
    [SerializeField] private float minFlickerDuration = 0.05f; // Minimum flicker duration
    [SerializeField] private float maxFlickerDuration = 0.2f; // Maximum flicker duration

    [Header("Light On/Off Durations")]
    [SerializeField] private float minOnDuration = 0.5f; // Minimum time light stays on
    [SerializeField] private float maxOnDuration = 2.0f; // Maximum time light stays on
    [SerializeField] private float minOffDuration = 0.5f; // Minimum time light stays off
    [SerializeField] private float maxOffDuration = 2.0f; // Maximum time light stays off

    [Header("Flicker Animation")]
    [SerializeField] private AnimationCurve turnOnCurve; // Curve to control the turn on pattern
    [SerializeField] private AnimationCurve turnOffCurve; // Curve to control the turn off pattern
    [SerializeField] private float flickerLightIntensity = 1.0f; // Intensity for flicker effect

    [Header("Material Animation")]
    [SerializeField] private MeshRenderer lampMaterial; // Material of the lamp
    [SerializeField] private string materialProperty = "_EmissionIntensity"; // Material property to animate
    [SerializeField] private float materialFlickerIntensity = 1.0f; // Intensity for material flicker effect

    [Header("Volumetric light")]
    [SerializeField] private VolumetricLightBeamSD volumetricLightBeamSD;
    [SerializeField] private float flickerVolumetricLightIntensity = 1.0f; // Intensity for flicker effect
    [SerializeField] private Color flickerColor = Color.white;

    [Header("Occlusion Settings")]
    [SerializeField] private Transform player; // Reference to the player
    [SerializeField] private float maxDistance = 10.0f; // Maximum distance before the light disappears

    private float _timeToNextFlicker;
    private bool _isFlickering;

    // Start is called before the first frame update
    void Start()
    {
        ScheduleNextFlicker();
        player = PlayerController.Instance.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isFlickering)
        {
            _timeToNextFlicker -= Time.deltaTime;
            if (_timeToNextFlicker <= 0f)
            {
                StartFlickering();
            }
        }

        CheckPlayerDistance();
    }

    private void CheckPlayerDistance()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        if (distance > maxDistance)
        {
            SetLightVisibility(false);
        }
        else
        {
            SetLightVisibility(true);
        }
    }

    private void SetLightVisibility(bool isVisible)
    {
        _light.enabled = isVisible;
        lampMaterial.enabled = isVisible;
        volumetricLightBeamSD.enabled = isVisible;
    }

    private void ScheduleNextFlicker()
    {
        _timeToNextFlicker = Random.Range(minTimeToNextFlicker, maxTimeToNextFlicker);
        _isFlickering = false;
    }

    private void StartFlickering()
    {
        _isFlickering = true;
        float flickerDuration = Random.Range(minFlickerDuration, maxFlickerDuration);

        // Flicker light intensity
        DOTween.To(() => _light.intensity, x => _light.intensity = x, flickerLightIntensity, flickerDuration).SetEase(turnOnCurve);
        DOTween.To(() => lampMaterial.material.GetColor("_EmissionColor"), x => lampMaterial.material.SetColor("_EmissionColor", x), flickerColor, flickerDuration).SetEase(turnOnCurve);
        DOTween.To(() => volumetricLightBeamSD.intensityGlobal, x => volumetricLightBeamSD.intensityGlobal = x, flickerVolumetricLightIntensity, flickerDuration).SetEase(turnOnCurve).OnComplete(() =>
        {
            float onDuration = Random.Range(minOnDuration, maxOnDuration);

            // Wait for the on duration, then turn off the light
            DOVirtual.DelayedCall(onDuration, () =>
            {
                // Turn off light intensity
                DOTween.To(() => _light.intensity, x => _light.intensity = x, 0, flickerDuration).SetEase(turnOffCurve);
                DOTween.To(() => lampMaterial.material.GetColor("_EmissionColor"), x => lampMaterial.material.SetColor("_EmissionColor", x), Color.black, flickerDuration).SetEase(turnOffCurve);
                DOTween.To(() => volumetricLightBeamSD.intensityGlobal, x => volumetricLightBeamSD.intensityGlobal = x, 0, flickerDuration).SetEase(turnOffCurve).OnComplete(() =>
                {
                    float offDuration = Random.Range(minOffDuration, maxOffDuration);

                    // Wait for the off duration, then schedule the next flicker
                    DOVirtual.DelayedCall(offDuration, () =>
                    {
                        ScheduleNextFlicker();
                    });
                });
            });
        });
    }
}
