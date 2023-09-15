using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public interface IPlayerState{
    void Enter(PlayerState player);
    void Execute(PlayerState player);
}

public class StandingState : IPlayerState{
    public void Enter(PlayerState player){
        player.currentState = this;
    }

    public void Execute(PlayerState player){
        if(Input.GetKeyDown(KeyCode.Space)){
            JumpingState jump = new JumpingState();
            jump.Enter(player);
        }
    }
}

public class JumpingState : IPlayerState{
    Rigidbody _rb;
    float jumpTime;
    GameObject hat;
    public void Enter(PlayerState player){
        hat = player.GetComponent<PlayerController>().hatObject;
        _rb = player.GetComponent<Rigidbody>();
        _rb.velocity = new Vector3(0,5,0);
        jumpTime = Time.time;
        player.currentState = this;
    }

    public void Execute(PlayerState player){
        if(Physics.Raycast(_rb.transform.position, Vector3.down, 0.5f) && (Time.time - jumpTime > 0.5f)){
            StandingState standing = new StandingState();
            standing.Enter(player);        
        }
        if(Input.GetKeyDown(KeyCode.Space) && Time.time - jumpTime > 0.1f && !hat.activeSelf){
            DoubleJumpState doubleJump = new DoubleJumpState();
            doubleJump.Enter(player);
        }
    }
}

public class DoubleJumpState : IPlayerState{
    Rigidbody _rb;
    float jumpTime;
    public void Enter(PlayerState player){
        _rb = player.GetComponent<Rigidbody>();
        _rb.velocity = new Vector3(0,7,0);
        jumpTime = Time.time;
        player.currentState = this;        
    }   

    public void Execute(PlayerState player){
        if(Physics.Raycast(_rb.transform.position, Vector3.down, 0.5f) && (Time.time - jumpTime > 0.5f)){
            StandingState standing = new StandingState();
            standing.Enter(player);            
        }if(Input.GetKey(KeyCode.LeftControl)){
            DivingState diving = new DivingState();
            diving.Enter(player);
        }
    }
}

public class DivingState : IPlayerState{
    Rigidbody _rb;
    ParticleSystem particles;
    public void Enter(PlayerState player){
        _rb = player.GetComponent<Rigidbody>();
        _rb.AddForce(0, -6000 * Time.deltaTime, 0, ForceMode.VelocityChange);
        player.currentState = this;
    }

    public void Execute(PlayerState player){
        if(Physics.Raycast(_rb.transform.position, Vector3.down, 0.5f)){
            StandingState standing = new StandingState();
            standing.Enter(player);
        }
    }
}
