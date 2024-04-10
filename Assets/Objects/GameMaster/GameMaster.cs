using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using NETWORK_ENGINE;

public class GameMaster : NetworkComponent
{
    [SerializeField] int totalEnemies = 3;

    [SerializeField] int enemiesDefeated = 0;
    int maxLives = 1; //incremented by total players

    public bool GameStarted = false;

    public override void HandleMessage(string flag, string value)
    {
        if (flag == "STARTGAME")
        {
            GameStarted = true;
            if (IsClient)
            {
                GameObject.FindGameObjectWithTag("ReadyUp").transform.parent.gameObject.GetComponent<Canvas>().enabled = false;
            }
        }

        if (flag == "WIN" && IsClient)
        {
            Win();
        }
    }

    public override void NetworkedStart()
    {

    }

    public override IEnumerator SlowUpdate()
    {
        while (IsServer)
        {
            NPM[] players = GameObject.FindObjectsOfType<NPM>();
            bool playersReady = true;
            if (players.Length > 1)
            {
                foreach (NPM p in players)
                {
                    if (!p.isReady)
                        playersReady = false;
                }
                if (playersReady && !GameStarted)
                {
                    for (int p = players.Length - 1; p >= 0; p--)
                    {
                        GameObject player = MyCore.NetCreateObject(0, players[p].Owner, GameObject.Find("P" + (p + 1).ToString() + "Start").transform.position);
                        player.GetComponent<Player>().pName = players[p].pName;
                        player.GetComponent<Player>().playerNumber = p;
                        maxLives++;
                    }
                    GameStarted = true;
                    SendUpdate("STARTGAME", "");
                }
            }
            yield return new WaitForSeconds(.1f);
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void Win() {
        //play cutscene - do we wanna make this a video that just plays?
        //then send to credits
        SceneManager.LoadScene("Win");
    }

    public void DefeatEnemy() {
        if (IsServer) {
            enemiesDefeated++;
            if (enemiesDefeated >= totalEnemies)
            {
                SendUpdate("WIN", "");
            }
        }
    }

}
