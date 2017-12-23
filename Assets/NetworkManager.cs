using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviour {

    public GameObject standbyCamera;
    SpawnSpot[] spawnSpots;
    GameObject[] players;
    SpawnSpot mySpawnSpot;
    List<SpawnSpot> spawnSpotsList;
    List<int> greenTeam;
    List<int> redTeam;
    List<int> renegades;
    bool gameWon;
    string teamWon;
    public int greenTeamKills;
    public int redTeamKills;
    public int[] renegadesKills = new int[10];
    Vector2 scrollPosition;
    string map = "Facing Worlds";
    public bool offlineMode = false;
    bool connecting = false;
    bool _disconnect = false;
    public float respawnTimer = 0;
    float timer;
    bool hasPickedTeam = false;
    int teamId = 0;
    List<string> chatMessages;
    int maxChatMessages = 5;
    GUIStyle fontSize;
    bool scoreBoardOn;
    int mvp = -1;
    string mvpName;
    int winningTeam = -1;

    public bool disconnect {
        get { return _disconnect; }
        set { _disconnect = value; }
    }

    // Use this for initialization
    void Start() {
        spawnSpotsList = new List<SpawnSpot>();
        PhotonNetwork.player.name = PlayerPrefs.GetString("Username", "N00B");
        chatMessages = new List<string>();
        greenTeam = new List<int>();
        redTeam = new List<int>();
        renegades = new List<int>();
        fontSize = new GUIStyle();
        fontSize.fontSize = 40;
        fontSize.normal.textColor = Color.white;
    }


    void OnDestroy() {
        PlayerPrefs.SetString("Username", PhotonNetwork.player.name);
    }

    public void AddChatMessage(string m) {
        gameObject.GetComponent<PhotonView>().RPC("AddChatMessage_RPC", PhotonTargets.All, m);
        Debug.Log("sdf");
    }

    public void RemoveChatMessage() {
        gameObject.GetComponent<PhotonView>().RPC("RemoveChatMessage_RPC", PhotonTargets.All);
    }

    [RPC]
    void AddChatMessage_RPC(string m) {
        while (chatMessages.Count >= maxChatMessages) {
            chatMessages.RemoveAt(0);
        }
        timer = 7.5f;
        chatMessages.Add(m);
    }

    [RPC]
    void RemoveChatMessage_RPC() {
        chatMessages.Clear();
    }

    void Connect() {
        PhotonNetwork.ConnectUsingSettings("MultiFPS v0.22");
    }

    void OnGUI() {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());

        if (PhotonNetwork.connected == false && connecting == false) {
            GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Username: ");
            PhotonNetwork.player.name = GUILayout.TextField(PhotonNetwork.player.name, GUILayout.Width(200));
            GUILayout.EndHorizontal();
            

            if (GUILayout.Button("Single Player")) {
                if (PhotonNetwork.player.name == "") {
                    PhotonNetwork.player.name = "Loser";
                }
                connecting = true;
                PhotonNetwork.offlineMode = true;
                OnJoinedLobby();
            }

            if (GUILayout.Button("Multi Player")) {
                if (PhotonNetwork.player.name == "") {
                    PhotonNetwork.player.name = "Loser";
                }
                connecting = true;
                Connect();
            }

            scrollPosition = GUI.BeginScrollView(new Rect(Screen.width/2-50, Screen.height/2 + Screen.height/8, 100, 200), scrollPosition, new Rect(0, 0, 100, 200));
            if (GUI.Button(new Rect(0, 0, 100, 20), "Facing Worlds")) {
                if (map != "Facing Worlds") {
                    map = "Facing Worlds";
                    DontDestroyOnLoad(gameObject);
                    DontDestroyOnLoad(standbyCamera);
                    Application.LoadLevel("Facing Worlds");
                }
            }
            if (GUI.Button(new Rect(0, 20, 100, 20), "Cubes")) {
                if (map != "Cubes") {
                    map = "Cubes";
                    DontDestroyOnLoad(gameObject);
                    DontDestroyOnLoad(standbyCamera);
                    Application.LoadLevel("Cubes");
                }
            }
            GUI.EndScrollView();
            if (Application.platform == RuntimePlatform.WindowsPlayer && Application.platform == RuntimePlatform.OSXPlayer) {
                if (GUI.Button(new Rect(Screen.width - 100, 0, 100, 30), "Quit")) {
                    Application.Quit();
                }
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        if (PhotonNetwork.connected == true && connecting == false) {
            if (hasPickedTeam) {
                GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
                GUILayout.BeginVertical();
                GUILayout.FlexibleSpace();

                foreach (string msg in chatMessages) {
                    GUILayout.Label(msg);
                }

                GUILayout.EndVertical();
                GUILayout.EndArea();
            }
            else {
                GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.BeginVertical();
                GUILayout.FlexibleSpace();


                if (GUILayout.Button("Red Team")) {
                    SpawnMyPlayer(1);
                }

                else if (GUILayout.Button("Green Team")) {
                    SpawnMyPlayer(2);
                }

                else if (GUILayout.Button("Random")) {
                    SpawnMyPlayer(Random.Range(1, 3));
                }

                else if (GUILayout.Button("Renegade!")) {
                    SpawnMyPlayer(0);
                }



                GUILayout.FlexibleSpace();
                GUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.EndArea();
            }
            int i = 0;
            foreach (PhotonPlayer player in PhotonNetwork.playerList) {
                if ((bool)player.customProperties["hasPickedTeam"]) {
                    if ((int)player.customProperties["TeamId"] == 2 && !renegades.Contains(player.ID) && !greenTeam.Contains(player.ID) && !redTeam.Contains(player.ID)) {
                        greenTeam.Add(player.ID);
                    }
                    else if ((int)player.customProperties["TeamId"] == 1 && !renegades.Contains(player.ID) && !greenTeam.Contains(player.ID) && !redTeam.Contains(player.ID)) {
                        redTeam.Add(player.ID);
                    }
                    else if ((int)player.customProperties["TeamId"] == 0 && !renegades.Contains(player.ID) && !greenTeam.Contains(player.ID) && !redTeam.Contains(player.ID)) {
                        renegades.Add(player.ID);
                    }
                }

            }
            if (scoreBoardOn) {
                GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
                GUI.Box(new Rect(Screen.width - 150, 10, 150, 300), "");
                GUI.Label(new Rect(Screen.width - 150, 10, 33, 20), "name");
                GUI.Label(new Rect(Screen.width - 60, 10, 33, 20), "kills");
                GUI.Label(new Rect(Screen.width - 33, 10, 33, 20), "deaths");
                scrollPosition = GUI.BeginScrollView(new Rect(Screen.width - 150, 20, 150, 100), scrollPosition, new Rect(0, 0, 150, 100));



                foreach (int playerID in greenTeam) {
                    PhotonPlayer greenTeamPlayer = PhotonPlayer.Find(playerID);
                    if (greenTeamPlayer != null) {
                        greenTeamKills = 0;
                        ExitGames.Client.Photon.Hashtable playerStats = greenTeamPlayer.customProperties;
                        int kills = (int)playerStats["Kills"];
                        int deaths = (int)playerStats["Deaths"];
                        greenTeamKills += (int)playerStats["Kills"];
                        GUI.Label(new Rect(0, 20 * i + 20, 100, 20), greenTeamPlayer.name);
                        GUI.Label(new Rect(100, 20 * i + 20, 25, 20), kills.ToString());
                        GUI.Label(new Rect(125, 20 * i + 20, 25, 20), deaths.ToString());
                        i++;
                    }
                }
                GUI.Label(new Rect(0, 5, 75, 20), "Green Team");
                GUI.Label(new Rect(75, 5, 25, 20), greenTeamKills.ToString());

                GUI.EndScrollView();
                scrollPosition = GUI.BeginScrollView(new Rect(Screen.width - 150, 120, 150, 100), scrollPosition, new Rect(0, 0, 150, 100));


                foreach (int playerID in redTeam) {
                    PhotonPlayer redTeamPlayer = PhotonPlayer.Find(playerID);
                    if (redTeamPlayer != null) {
                        redTeamKills = 0;
                        ExitGames.Client.Photon.Hashtable playerStats = redTeamPlayer.customProperties;
                        int kills = (int)playerStats["Kills"];
                        int deaths = (int)playerStats["Deaths"];
                        redTeamKills += (int)playerStats["Kills"];
                        GUI.Label(new Rect(0, 20 * i + 20, 100, 20), redTeamPlayer.name);
                        GUI.Label(new Rect(100, 20 * i + 20, 25, 20), kills.ToString());
                        GUI.Label(new Rect(125, 20 * i + 20, 25, 20), deaths.ToString());
                        i++;
                    }
                }
                GUI.Label(new Rect(0, 5, 75, 20), "Red Team");
                GUI.Label(new Rect(75, 5, 25, 20), redTeamKills.ToString());
                GUI.EndScrollView();
                scrollPosition = GUI.BeginScrollView(new Rect(Screen.width - 150, 220, 150, 100), scrollPosition, new Rect(0, 0, 150, 100));

                GUI.Label(new Rect(0, 5, 75, 20), "Renegades");
                foreach (int playerID in renegades) {
                    PhotonPlayer renegadesPlayer = PhotonPlayer.Find(playerID);
                    if (renegadesPlayer != null) {
                        ExitGames.Client.Photon.Hashtable playerStats = renegadesPlayer.customProperties;
                        int kills = (int)playerStats["Kills"];
                        int deaths = (int)playerStats["Deaths"];
                        GUI.Label(new Rect(0, 20 * i + 20, 100, 20), renegadesPlayer.name);
                        GUI.Label(new Rect(100, 20 * i + 20, 25, 20), kills.ToString());
                        GUI.Label(new Rect(125, 20 * i + 20, 25, 20), deaths.ToString());
                        i++;
                    }
                }

                GUI.EndScrollView();
                GUILayout.EndArea();
            }
        }

        if (gameWon && mvp != -1) {
            GUI.Box(new Rect(Screen.width / 2 - 100, Screen.width / 2 - 50, 250, 100),"");
            GUI.Label(new Rect(Screen.width/2-100,Screen.width/2-30,200,60),teamWon,fontSize);
            GUI.Label(new Rect(Screen.width/2-100,Screen.width/2,200,60),"MVP: " + mvpName,fontSize);
        }
    }

    void OnJoinedLobby() {
        Debug.Log("OnJoinedLobby");
        ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "map", map } };
        PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties,10);
    }

    void OnPhotonRandomJoinFailed() {
        Debug.Log("OnPhotonRandomJoinFailed");
        string[] roomPropsInLobby = { "map"};
        ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "map", map } };
        PhotonNetwork.CreateRoom(null, true, true, 10, customRoomProperties, roomPropsInLobby);
    }

    void OnJoinedRoom() {
        Debug.Log("OnJoinedRoom");
        connecting = false;
        spawnSpots = GameObject.FindObjectsOfType<SpawnSpot>();
        ExitGames.Client.Photon.Hashtable playerStats = new ExitGames.Client.Photon.Hashtable();
        playerStats.Add("Kills", 0);
        playerStats.Add("Deaths", 0);
        playerStats.Add("TeamId", 0);
        playerStats.Add("hasPickedTeam", false);
        playerStats.Add("Killstreak", 0);
        playerStats.Add("KillstreaksAvailable1", 0);
        PhotonNetwork.player.SetCustomProperties(playerStats);
    }

    void OnPhotonPlayerConnected(PhotonPlayer player) {
        ExitGames.Client.Photon.Hashtable playerStats = new ExitGames.Client.Photon.Hashtable();
        playerStats.Add("Kills", 0);
        playerStats.Add("Deaths", 0);
        playerStats.Add("TeamId", 0);
        playerStats.Add("hasPickedTeam", false);
        playerStats.Add("Killstreak", 0);
        playerStats.Add("KillstreaksAvailable1",0);
        player.SetCustomProperties(playerStats);
    }

    void OnPhotonPlayerDisconnected(PhotonPlayer player) {
        player.SetCustomProperties(null);
        if ((int)player.customProperties["TeamId"] == 2) {
            greenTeam.Remove(player.ID);
        }
        else if ((int)player.customProperties["TeamId"] == 1) {
            redTeam.Remove(player.ID);
        }
        else if ((int)player.customProperties["TeamId"] == 0) {
            renegades.Remove(player.ID);
        }
    }

    void DisconnectFromRoom() {
        
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
        PhotonNetwork.offlineMode = false;
        PhotonNetwork.player.SetCustomProperties(null);
        if ((int)PhotonNetwork.player.customProperties["TeamId"] == 2) {
            greenTeam.Remove(PhotonNetwork.player.ID);
        }
        else if ((int)PhotonNetwork.player.customProperties["TeamId"] == 1) {
            redTeam.Remove(PhotonNetwork.player.ID);
        }
        else if ((int)PhotonNetwork.player.customProperties["TeamId"] == 0) {
            renegades.Remove(PhotonNetwork.player.ID);
        }
        disconnect = false;
        hasPickedTeam = false;
    }

    void SpawnMyPlayer(int teamID) {
        this.teamId = teamID;
        hasPickedTeam = true;


        if (spawnSpots == null) {
            Debug.LogError("WTF?!?!?");
            return;
        }
        players = GameObject.FindGameObjectsWithTag("Player");
        
        if (players.Length > 0) {
            foreach (SpawnSpot spawn in spawnSpots) {
                spawnSpotsList.Add(spawn);
                foreach (GameObject player in players) {
                    if (Vector3.Distance(player.transform.position, spawn.transform.position) < 30) {
                        spawnSpotsList.Remove(spawn);
                    }
                }
            }
            mySpawnSpot = spawnSpotsList[Random.Range(0, spawnSpotsList.Count)];
        }
        else {
            mySpawnSpot = spawnSpots[Random.Range(0, spawnSpots.Length)];
            Debug.Log("random");
        }

        GameObject myPlayerGO = (GameObject)PhotonNetwork.Instantiate("PlayerController", mySpawnSpot.transform.position, mySpawnSpot.transform.rotation, 0);

        PhotonPlayer photonPlayer = PhotonPlayer.Find(myPlayerGO.GetComponent<PhotonView>().owner.ID);
        ExitGames.Client.Photon.Hashtable playerStats = photonPlayer.customProperties;
        playerStats["TeamId"] = this.teamId;
        playerStats["hasPickedTeam"] = true;
        photonPlayer.SetCustomProperties(playerStats);

        standbyCamera.SetActive(false);
        spawnSpotsList.Clear();
        ((MonoBehaviour)myPlayerGO.GetComponent("MouseLook")).enabled = true;
        ((MonoBehaviour)myPlayerGO.GetComponent("PlayerShooting")).enabled = true;
        myPlayerGO.GetComponent<FPSInputController>().enabled = true;
        myPlayerGO.GetComponent<CharacterMotor>().enabled = true;
        myPlayerGO.GetComponent<PhotonView>().RPC("SetTeamID", PhotonTargets.AllBuffered, teamID);
        Debug.Log("Picked");
        myPlayerGO.transform.FindChild("Main Camera").gameObject.SetActive(true);
        myPlayerGO.transform.FindChild("Sniper").gameObject.SetActive(false);
    }

    void Update() {
        if (PhotonNetwork.connected == true && connecting == false) {
            if (timer > 0) {
                timer -= Time.deltaTime;
                if (timer <= 0) {
                        RemoveChatMessage();
                    timer = 0f;
                }
            }
        }

        if (respawnTimer > 0) {
            respawnTimer -= Time.deltaTime;

            if (respawnTimer <= 0) {
                SpawnMyPlayer(teamId);
            }
        }
        if (disconnect) {
            DisconnectFromRoom();
            standbyCamera.SetActive(true);
        }

        if (greenTeamKills >= 50 && !gameWon) {
            winningTeam = 2;
            gameWon = true;
            teamWon = "Green Team Wins";
        }

        if (redTeamKills >= 50 && !gameWon) {
            winningTeam = 1;
            gameWon = true;
            teamWon = "Red Team Wins";
        }
        foreach (int playerId in renegades) {
            PhotonPlayer player = PhotonPlayer.Find(playerId);
            if (player != null) {
                ExitGames.Client.Photon.Hashtable playerStats = player.customProperties;
                if ((int)playerStats["Kills"] >= 30 && !gameWon) {
                    mvp = player.ID;
                    mvpName = player.name;
                    winningTeam = 0;
                    gameWon = true;
                    teamWon = player.name + " Wins";
                }
            }
        }

        if (winningTeam == 2 && mvp == -1) {
            foreach (int playerID in greenTeam) {
                PhotonPlayer greenTeamPlayer = PhotonPlayer.Find(playerID);                
                ExitGames.Client.Photon.Hashtable playerStats = greenTeamPlayer.customProperties;
                if (mvp == null) {
                    mvp = greenTeamPlayer.ID;
                }
                PhotonPlayer mvpPlayer = PhotonPlayer.Find(mvp);
                ExitGames.Client.Photon.Hashtable mvpStats = mvpPlayer.customProperties;
                if (greenTeamPlayer != null && (int)mvpStats["Kills"] < (int)playerStats["Kills"]) {
                    mvp = greenTeamPlayer.ID;
                    mvpName = mvpPlayer.name;
                }
            }
        }
        else if (winningTeam == 1 && mvp == -1) {
            foreach (int playerID in redTeam) {
                PhotonPlayer redTeamPlayer = PhotonPlayer.Find(playerID);
                ExitGames.Client.Photon.Hashtable playerStats = redTeamPlayer.customProperties;
                if (mvp == null) {
                    mvp = redTeamPlayer.ID;
                }
                PhotonPlayer mvpPlayer = PhotonPlayer.Find(mvp);
                ExitGames.Client.Photon.Hashtable mvpStats = mvpPlayer.customProperties;
                if (redTeamPlayer != null && (int)mvpStats["Kills"] < (int)playerStats["Kills"]) {
                    mvp = redTeamPlayer.ID;
                    mvpName = mvpPlayer.name;
                }
           }
        }


        if (Input.GetKeyDown(KeyCode.Tab)) {
            scoreBoardOn = !scoreBoardOn;
        }
        if (gameWon && mvp != -1) {
            GameOver();
        }

    }

    public void ScoreBoard(int dead, int killer = -1) {
        gameObject.GetComponent<PhotonView>().RPC("ScoreBoard_RPC", PhotonTargets.All, dead, killer);
    }

    [RPC]
    void ScoreBoard_RPC(int dead, int killer = -1) {
        PhotonPlayer deadPlayer = PhotonPlayer.Find(dead);
        ExitGames.Client.Photon.Hashtable deadStats = deadPlayer.customProperties;
        deadStats["Deaths"] = (int)deadStats["Deaths"] + 1;
        deadStats["Killstreak"] = 0;
        deadPlayer.SetCustomProperties(deadStats);
        if (killer != -1) {
            PhotonPlayer killerPlayer = PhotonPlayer.Find(killer);
            ExitGames.Client.Photon.Hashtable killerStats = killerPlayer.customProperties;
            killerStats["Kills"] = (int)killerStats["Kills"] + 1;
            killerStats["Killstreak"] = (int)killerStats["Killstreak"] + 1;
            killerPlayer.SetCustomProperties(killerStats);
        }

    }

    public void GameOver(){
        gameObject.GetComponent<PhotonView>().RPC("GameOver_RPC", PhotonTargets.All);
    }

    [RPC]
    void GameOver_RPC() {
        Invoke("GameOverTime", 10);        
    }
    void GameOverTime() {
        greenTeam.Clear();
        redTeam.Clear();
        renegades.Clear();
        gameWon = false;
        teamWon = null;
        greenTeamKills = 0;
        redTeamKills = 0;
        renegadesKills = new int[10];
        mvp = -1;
        mvpName = null;
        winningTeam = -1;
        disconnect = true;
    }

}
