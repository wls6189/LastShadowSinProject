using UnityEngine;

public class AirborneState : IState
{
    PlayerController player;


    public AirborneState(PlayerController player)
    {
        this.player = player;
    }
    public void Enter()
    {
        player.CurrentPlayerState = PlayerState.Airborne;

        player.Animator.SetTrigger("DoJump");
    }

    public void Execute()
    {
        Vector3 moveVector = new Vector3(0, 0, player.MoveActionValue);
        player.CharacterController.Move(moveVector * player.MoveSpeed * Time.deltaTime);
    }

    public void Exit()
    {

    }
}