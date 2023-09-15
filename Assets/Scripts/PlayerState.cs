using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerState : MonoBehaviour//PunCallbacks, IPunObservable
{
    public IPlayerState currentState;

    private void Awake(){
        currentState = new StandingState();
    }

    void Update(){
        currentState.Execute(this);
    }

   /* public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){
        if(stream.IsWriting){
            stream.SendNext(currentState);
        }else if(stream.IsReading){
            currentState = (IPlayerState)stream.ReceiveNext();
        }
    }*/
}
