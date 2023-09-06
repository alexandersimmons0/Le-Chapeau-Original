using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager instance;

    void Start(){
        PhotonNetwork.ConnectUsingSettings();
    }

    void Awake(){
        if(instance != null && instance != this){
            gameObject.SetActive(false);
        }else{
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void CreateRoom(string roomName){
        PhotonNetwork.CreateRoom(roomName);
    }

    public void JoinRoom(string roomName){
        PhotonNetwork.JoinRoom(roomName);
    }

 /*   public void ChangeScene(string sceneName){
        PhotonNetwork.LoadLevel(sceneName);
    }*/

 /*   public override void OnConnectedToMaster(){
        CreateRoom("testRoom");
    }
*/
    public override void OnCreatedRoom(){
        Debug.Log("Created room: " + PhotonNetwork.CurrentRoom.Name);
    }

    [PunRPC]
    public void ChangeScene(string sceneName){
        PhotonNetwork.LoadLevel(sceneName);
    }
}
