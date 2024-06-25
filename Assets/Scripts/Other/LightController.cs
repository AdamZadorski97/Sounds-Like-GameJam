using System.Collections;
using UnityEngine;
using DG.Tweening;
using VLB;

public class LightController : MonoBehaviour
{
    private Light _light;

    [Header("Flicker Timing")]
    [SerializeField] private float minTimeToNextFlicker = 0.5f; // Minimum time before next flicker
    [SerializeField] private float maxTimeToNextFlicker = 2.0f; // Maximum time before next flicker
    [SerializeField] private float minFlickerDuration = 0.05f; // Minimum flicker duration
    [SerializeField] private float maxFlickerDuration = 0.2f; // Maximum flicker duration

    [Header("Flicker Animation")]
    [SerializeField] private AnimationCurve turnOnCurve; // Curve to control the turn on pattern
    [SerializeField] private AnimationCurve turnOffCurve; // Curve to control the turn off pattern
    [SerializeField] private float flickerLightIntensity = 1.0f; // Intensity for flicker effect

    [Header("Material Animation")]
    [SerializeField] private Material lampMaterial; // Material of the lamp
    [SerializeField] private string materialProperty = "_EmissionIntensity"; // Material property to animate
    [SerializeField] private float materialFlickerIntensity = 1.0f; // Intensity for material flicker effect

    [Header("Volumetric light")]
    [SerializeField] private VolumetricLightBeamSD volumetricLightBeamSD;
    [SerializeField] private float flickerVolumetricLightIntensity = 1.0f; // Intensity for flicker effect

    private float _timeToNextFlicker;
    private bool _isFlickering;

    // Start is called before the first frame update
    void Start()
    {
        _light = GetComponent<Light>();
        ScheduleNextFlicker();
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
        DOTween.To(() => lampMaterial.GetFloat(materialProperty), x => lampMaterial.SetFloat(materialProperty, x), materialFlickerIntensity, flickerDuration).SetEase(turnOnCurve);
        DOTween.To(() => volumetricLightBeamSD.intensityGlobal, x => volumetricLightBeamSD.intensityGlobal = x, flickerVolumetricLightIntensity, flickerDuration).SetEase(turnOnCurve).OnComplete(() =>
        {
            // Turn off light intensity
            DOTween.To(() => _light.intensity, x => _light.intensity = x, 0, flickerDuration).SetEase(turnOffCurve);
            DOTween.To(() => lampMaterial.GetFloat(materialProperty), x => lampMaterial.SetFloat(materialProperty, x), 0, flickerDuration).SetEase(turnOffCurve);
            DOTween.To(() => volumetricLightBeamSD.intensityGlobal, x => volumetricLightBeamSD.intensityGlobal = x, 0, flickerDuration).SetEase(turnOffCurve).OnComplete(() =>
            {
                ScheduleNextFlicker();
            });
        });
    }
}
