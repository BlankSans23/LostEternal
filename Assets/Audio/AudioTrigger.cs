using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioTrigger : MonoBehaviour
{
    AudioSource triggeredMusic;

    private void Start()
    {
        triggeredMusic = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Player p;
        if (other.TryGetComponent<Player>(out p) && p.IsLocalPlayer)
        {
            triggeredMusic.Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Player p;
        if (other.TryGetComponent<Player>(out p) && p.IsLocalPlayer)
        {
            triggeredMusic.Stop();
        }
    }
}
