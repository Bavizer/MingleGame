using MEC;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MingleGame.Tools;

public static class LocationTools
{
    public static void Rotate(Transform transform, Vector3 delta, float duration = 0f)
    {
        if (duration <= 0f)
            transform.Rotate(delta);
        else
            Timing.RunCoroutine(Rotate_Coroutine(transform, delta, duration));
    }

    public static void SetLightIntensity(LightSourceToy light, float intensity, float switchingDuration = 0f)
    {
        if (switchingDuration <= 0f)
            light.NetworkLightIntensity = intensity;
        else
            Timing.RunCoroutine(SetLightIntensity_Coroutine(light, intensity, switchingDuration));
    }

    public static void SwitchLightColors(LightSourceToy light, IEnumerable<Color> colors, float delta, float duration = 0)
    {
        if (duration <= 0f)
            light.NetworkLightColor = colors.Last();
        else
            Timing.RunCoroutine(SwitchLightColor_Coroutine(light, colors, delta, duration));
    }

    private static IEnumerator<float> Rotate_Coroutine(Transform transform, Vector3 delta, float duration)
    {
        var elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            yield return Timing.WaitForOneFrame;
            transform.Rotate(delta * Time.deltaTime);
            elapsedTime += Time.deltaTime;
        }
    }

    private static IEnumerator<float> SetLightIntensity_Coroutine(LightSourceToy light, float intensity, float switchingDuration)
    {
        var elapsedTime = 0f;
        var initialIntensity = light.NetworkLightIntensity;

        while (elapsedTime < switchingDuration)
        {
            yield return Timing.WaitForOneFrame;
            light.NetworkLightIntensity = Mathf.Lerp(initialIntensity, intensity, elapsedTime / switchingDuration);
            elapsedTime += Time.deltaTime;
        }
        light.NetworkLightIntensity = intensity;
    }

    private static IEnumerator<float> SwitchLightColor_Coroutine(LightSourceToy light, IEnumerable<Color> colors, float delta, float duration)
    {
        var startTime = Time.time;
        var elapsedTime = 0f;

        var colors1 = colors.ToArray();
        while (elapsedTime < duration)
        {
            foreach (var color in colors1)
            {
                yield return Timing.WaitForSeconds(delta);
                light.NetworkLightColor = color;
                elapsedTime = Time.time - startTime;
            }
        }
    }
}
