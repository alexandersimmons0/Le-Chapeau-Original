using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    private bool respawn;

    [HideInInspector]
    public int id;
    public float curHatTime;

    [Header ("Info")]
    public float moveSpeed;
    public float jumpForce;
    public GameObject hatObject;
    public Vector3[] spawnPoints;

    [Header ("Components")]
    public Rigidbody rig;
    public Player photonPlayer;

    void Update(){
        Move();
        if(Input.GetKeyDown(KeyCode.Space)){
            TryJump();
        }
        if(PhotonNetwork.IsMasterClient){
            if(curHatTime >= GameManager.instance.timeToWin && !GameManager.instance.gameEnded){
                GameManager.instance.gameEnded = true;
                GameManager.instance.photonView.RPC("WinGame", RpcTarget.All, id);
            }
        }
        if(hatObject.activeInHierarchy){
            curHatTime += Time.deltaTime;
        }
        if(this.transform.position.y <= -1&& !respawn){
            respawn = true;
            SetHat(false);
            Invoke("Respawn", 2f);
        }
    }

    void Respawn(){
        this.transform.position = spawnPoints[Random.Range(0,3)];
        respawn = false;
    }

    void Move(){
        float x = Input.GetAxis("Horizontal") * moveSpeed;
        float z = Input.GetAxis("Vertical") * moveSpeed;
        rig.velocity = new Vector3(x, rig.velocity.y, z);
    }

    void TryJump(){
        Ray ray = new Ray (transform.position, Vector3.down);
        if(Physics.Raycast(ray, 0.7f)){
            rig.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    [PunRPC]
    public void Initialize(Player player){
        photonPlayer = player;
        id = player.ActorNumber;
        GameManager.instance.players[id - 1] = this;
        if(id == 1){
            GameManager.instance.GiveHat(id, true);
        }
        if(!photonView.IsMine){
            rig.isKinematic = true;
        }
    }

    public void SetHat(bool hasHat){
        hatObject.SetActive(hasHat);
    }

    void OnCollisionEnter(Collision collision){
        if(!photonView.IsMine){
            return;
        }
        if(collision.gameObject.CompareTag("Player")){
            if(GameManager.instance.GetPlayer(collision.gameObject).id == GameManager.instance.playerWithHat){
                if(GameManager.instance.CanGetHat()){
                    GameManager.instance.photonView.RPC("GiveHat", RpcTarget.All, id, false);
                }
            }
        }
        if(collision.gameObject.CompareTag("Hat")){
            if(GameManager.instance.CanGetHat()){
                GameManager.instance.photonView.RPC("GiveHat", RpcTarget.All, id, false);
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){
        if(stream.IsWriting){
            stream.SendNext(curHatTime);
        }else if(stream.IsReading){
            curHatTime = (float)stream.ReceiveNext();
        }
    }
}
