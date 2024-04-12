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

    int hpToDisplay;
    int maxHp;
    Player localP;

    bool reset = false;

    //bullet CD is 6s
    

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players) {
            if (player.GetComponent<Player>().IsLocalPlayer) { //im sorry for how heinous this is
                localP = player.GetComponent<Player>();
                maxHp = localP.stats[StatType.HP];
            }
        }
    }

    // Update is called once per frame
    void Update()
    {//this makes me sad but oh well independent scripts do be independent doe
        hpToDisplay = localP.stats[StatType.HP];
        hpBar.fillAmount = hpToDisplay / maxHp;
        if (!localP.bulletLoaded&& !reset) {
            reloadMeter.fillAmount = 0;
            StartCoroutine(fillReloadMeter());
            reset = true;
        }
        //do we get a timer here???
        //and then do we update the reload meter here? or in a coroutine
    }

    IEnumerator fillReloadMeter() {
        while (reloadMeter.fillAmount < 1f) {
            reloadMeter.fillAmount += .06f;
            yield return new WaitForSeconds(.1f);
        }
        if (reloadMeter.fillAmount == 1) {
            reset = false;
        }
    }

}
