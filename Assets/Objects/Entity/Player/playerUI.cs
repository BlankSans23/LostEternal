using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class playerUI : MonoBehaviour
{
     public Image hpBar;
     public Image reloadMeter;
     Player p;
    public Image atkBuff;
    public Image defBuff;

    float hpToDisplay;
    float maxHp;
    Player localP;

    bool reset = false;

    //bullet CD is 6s
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void FindPlayer() {
        GetComponent<Canvas>().enabled = true;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            Player temp = player.GetComponent<Player>();
            if (temp.IsLocalPlayer)
            { //im sorry for how heinous this is
                localP = temp.GetComponent<Player>();
                maxHp = localP.stats[StatType.HP];
            }
        }
    }

    // Update is called once per frame
    void Update()
    {//this makes me sad but oh well independent scripts do be independent doe
        if (localP == null)
            return;
        hpToDisplay = localP.stats[StatType.HP];
        hpBar.fillAmount = hpToDisplay / maxHp;
        if (!reset && !localP.bulletLoaded) {
            reloadMeter.fillAmount = 0;
            StartCoroutine(fillReloadMeter());
            reset = true;
        }
    }

    IEnumerator fillReloadMeter() {
        float cycles = 100f;
        while (reloadMeter.fillAmount < 1f) {
            reloadMeter.fillAmount += 1f/cycles;
            yield return new WaitForSeconds(localP.bulletCooldown/cycles);
        }
        if (reloadMeter.fillAmount >= 1) {
            reset = false;
        }
    }

}
