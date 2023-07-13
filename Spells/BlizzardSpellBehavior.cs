using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlizzardSpellBehavior : MonoBehaviour
{
    public float radius = 1f;
    public float lifetime = 5f;
    public float timeBetweenAudioClips = .5f;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    private void Start()
    {
        // Start the PlaySoundCoroutine
        StartCoroutine(PlayAudioClipCoroutine());
    }

    private void Update()
    {
        Destroy(gameObject, lifetime);
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    private IEnumerator PlayAudioClipCoroutine()
    {
        float timeToWait = lifetime - 1f;
        while (timeToWait > 0)
        {
            audioSource.volume = Random.Range(.35f, .75f);
            audioSource.pitch = Random.Range(.95f, 1.05f);
            audioSource.Play();

            float waitTime = Random.Range(.1f, .5f);
            timeToWait -= waitTime;
            yield return new WaitForSeconds(waitTime);
        }
    }
}
