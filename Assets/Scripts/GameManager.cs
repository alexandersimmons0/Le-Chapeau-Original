using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class GameManager : MonoBehaviourPunCallbacks
{
    private int counter;

    [Header ("Stats")]
    public bool gameEnded = false;
    public float timeToWin;
    public float invisibleDuration;
    private float hatPickupTime;
    public GameObject centerHat;

    [Header ("Players")]
    public string playerPrefabLocation;
    public Vector3[] spawnPoints;
    public PlayerController[] players;
    public int playerWithHat;
    private int playersInGame;

    public static GameManager instance;

    void Awake(){
        instance = this;
    }

    void Start(){
        centerHat.SetActive(false);
        players = new PlayerController[PhotonNetwork.PlayerList.Length];
        photonView.RPC("ImInGame", RpcTarget.All);
    }

    void Update(){
        for(int i =0; i < PhotonNetwork.PlayerList.Length; i++){
            if(!players[i].hatObject.activeSelf){
                counter++;
            }
        }
        if(counter != PhotonNetwork.PlayerList.Length){
            counter = 0;
            centerHat.SetActive(false);
        }else{
            centerHat.SetActive(true);
            counter = 0;
        }
    }

    [PunRPC]
    void ImInGame(){
        playersInGame++;
        if(playersInGame == PhotonNetwork.PlayerList.Length){
            SpawnPlayer();
        }
    }

    void SpawnPlayer(){
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLocation, spawnPoints[Random.Range(0,spawnPoints.Length)], Quaternion.identity);
        PlayerController playerScript = playerObj.GetComponent<PlayerController>();
        playerScript.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }

    public PlayerController GetPlayer(int playerID){
        return players.First(x => x.id == playerID);
    }

    public PlayerController GetPlayer(GameObject playerObject){
        return players.First(x => x.gameObject == playerObject);
    }
  
    [PunRPC]
    public void GiveHat(int playerID, bool initialGive){
        if(!initialGive){
            GetPlayer(playerWithHat).SetHat(false);
        }
        playerWithHat = playerID;
        GetPlayer(playerID).SetHat(true);
        hatPickupTime = Time.time;
    }

    public bool CanGetHat(){
        if(Time.time > hatPickupTime + invisibleDuration){
            return true;
        }else{
            return false;
        }
    }

    [PunRPC]
    void WinGame(int playerID){
        gameEnded = true;
        PlayerController player = GetPlayer(playerID);
        GameUI.instance.SetWinText(player.photonPlayer.NickName);
        Invoke("GoBackToMenu", 3.0f);
    }

    void GoBackToMenu(){
        PhotonNetwork.LeaveRoom();
        NetworkManager.instance.ChangeScene("menu");
    }

}
