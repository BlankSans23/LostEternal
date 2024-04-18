using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTrigger : MonoBehaviour
{
    [SerializeField] AudioSource bgm;
    [SerializeField] AudioSource triggeredMusic;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            bgm.Stop();
            triggeredMusic.Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            bgm.Play();
            triggeredMusic.Stop();
        }
    }
}
