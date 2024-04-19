using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using NETWORK_ENGINE;

public class GameMaster : NetworkComponent
{
    [SerializeField] int totalEnemies = 3;

    [SerializeField] int enemiesDefeated = 0;
    [SerializeField] int maxLives = 1; //incremented by total players
    [SerializeField] SceneTransition loadScreen;

    GameObject playerUI;

    public bool GameStarted = false;

    public override void HandleMessage(string flag, string value)
    {
        if (flag == "STARTGAME")
        {
            GameStarted = true;
            if (IsClient)
            {
                GameObject.FindGameObjectWithTag("ReadyUp").transform.parent.gameObject.GetComponent<Canvas>().enabled = false;
                playerUI.GetComponent<Canvas>().enabled = true;
                loadScreen.FadeIn();
            }
        }

        if (flag == "FADE" && IsClient)
        {
            loadScreen.FadeOut();
        }

        if (flag == "WIN" && IsClient)
        {
            Win();
        }
        if (flag == "LOSE" && IsClient)
        {
            GameOver();
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
                    SendUpdate("FADE", "");
                    for (int p = players.Length - 1; p >= 0; p--)
                    {
                        GameObject player = MyCore.NetCreateObject(0, players[p].Owner, GameObject.Find("P" + (p + 1).ToString() + "Start").transform.position);
                        player.GetComponent<Player>().pName = players[p].pName;
                        player.GetComponent<Player>().playerNumber = p;
                        maxLives++;
                    }
                    GameStarted = true;
                    yield return new WaitForSeconds(1.1f);
                    SendUpdate("STARTGAME", "");
                }
            }
            yield return new WaitForSeconds(.1f);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        playerUI = GameObject.Find("playerUI");
        playerUI.GetComponent<Canvas>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Win() {
        //play cutscene - do we wanna make this a video that just plays?
        //then send to credits
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        StartCoroutine(LoadScene("Win"));
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

    void GameOver()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        StartCoroutine(LoadScene("Lose"));
    }

    IEnumerator LoadScene(string scene)
    {
        yield return new WaitForSeconds(1.1f);
        SceneManager.LoadScene(scene);
    }

    public void PlayerDefeated() {
        if (IsServer)
        {
            maxLives--;

            if (maxLives <= 0)
                SendUpdate("LOSE", "");
        }
    }

}
