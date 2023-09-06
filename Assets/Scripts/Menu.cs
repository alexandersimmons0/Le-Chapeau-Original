using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class Menu : MonoBehaviourPunCallbacks
{
    [Header("Screens")]
    public GameObject mainScreen;
    public GameObject lobbyScreen;

    [Header("Main Screen")]
    public Button createRoomButton;
    public Button joinRoomButton;

    [Header("Lobby Screen")]
    public TextMeshProUGUI playerListText;
    public Button startGameButton;

    void Start(){
        createRoomButton.interactable = false;
        joinRoomButton.interactable = false;
        SetScreen(mainScreen);
    }

    void SetScreen(GameObject screen){
        mainScreen.SetActive(false);
        lobbyScreen.SetActive(false);
        screen.SetActive(true);
    }

    public void OnCreatedRoomButton(TMP_InputField roomNameInput){
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 1;
        PhotonNetwork.CreateRoom(roomNameInput.text, roomOptions);
        //NetworkManager.instance.CreateRoom(roomNameInput.text, roomOptions);
    }

    public void OnJoinRoomButton(TMP_InputField roomNameInput){
                NetworkManager.instance.JoinRoom(roomNameInput.text);
    }
    
    public override void OnPlayerLeftRoom(Player otherPlayer){
        UpdateLobbyUI();
    }

    public override void OnConnectedToMaster(){
        Debug.Log("connected to master");
        createRoomButton.interactable = true;
        joinRoomButton.interactable = true;
    }

    public void OnLeaveLobbyButton(){
        PhotonNetwork.LeaveRoom();
        SetScreen(mainScreen);
    }

    public void OnStartGameButton(){
        NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.All, "game");
    }

    public void OnPlayerNameUpdate(TMP_InputField playerNameInput){
        PhotonNetwork.NickName = playerNameInput.text;
    }

    [PunRPC]
    public void UpdateLobbyUI(){
        playerListText.text = "";
        foreach(Player player in PhotonNetwork.PlayerList){
            playerListText.text += player.NickName + "\n";
        }
        if(PhotonNetwork.IsMasterClient){
            startGameButton.interactable = true;
        }else{
            startGameButton.interactable = false;
        }
    }
    [PunRPC]
    public override void OnJoinedRoom(){
        SetScreen(lobbyScreen);
        photonView.RPC("UpdateLobbyUI", RpcTarget.All);
    }
}
