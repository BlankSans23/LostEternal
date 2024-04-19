using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] Animator loadingScreen;

    public void FadeOut() {
        loadingScreen.SetBool("Loading", true);
    }

    public void FadeIn()
    {
        loadingScreen.SetBool("Loading", false);
    }
}
