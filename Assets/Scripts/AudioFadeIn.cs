using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioFadeIn : MonoBehaviour
{
    [Min(0f)] public float fadeDuration = 3f;
    [Range(0f, 1f)] public float targetVolume = 0.3f;
    private AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        source.playOnAwake = false;
        source.volume = 0f;
    }

    private IEnumerator Start()
    {
        if (source.clip == null) yield break;
        source.Play();
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            source.volume = Mathf.Lerp(0f, targetVolume, elapsed / fadeDuration);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        source.volume = targetVolume;
    }
}