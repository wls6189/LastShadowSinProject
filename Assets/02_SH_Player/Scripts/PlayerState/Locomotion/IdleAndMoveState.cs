using UnityEngine;

public class IdleAndMoveState : IState
{
    PlayerController player;
    float moveBlendValue = 0;
    float moveBlendDeltaValue = 11f;

    public IdleAndMoveState(PlayerController player)
    {
        this.player = player;
    }
    public void Enter()
    {
        player.CurrentPlayerState = PlayerState.IdleAndMove;
        player.PlayerAnimator.SetTrigger("DoIdleAndMove");
        player.IsAttacking = false;
    }

    public void Execute()
    {
        // 플레이어의 실제 이동
        Vector3 moveVector = new Vector3(0, 0, player.MoveActionValue);
        player.CharacterController.Move(moveVector * player.MoveSpeed * Time.deltaTime);

        // 이동 애니메이션 
        if (player.IsLockOn) // 대상 고정 중일 때
        {
            if (player.IsLookRight) // 적이 오른쪽에 있는 경우
            {
                if (player.MoveActionValue == 1)
                {
                    moveBlendValue += Time.deltaTime * moveBlendDeltaValue;
                    moveBlendValue = Mathf.Clamp(moveBlendValue, 0f, 1f);
                }
                else if (player.MoveActionValue == -1)
                {
                    moveBlendValue -= Time.deltaTime * moveBlendDeltaValue;
                    moveBlendValue = Mathf.Clamp(moveBlendValue, -1f, 0f);
                }
                else
                {
                    if (moveBlendValue > 0)
                    {
                        moveBlendValue -= Time.deltaTime * moveBlendDeltaValue;
                        moveBlendValue = Mathf.Clamp(moveBlendValue, 0f, 1f);
                    }
                    else if (moveBlendValue < 0)
                    {
                        moveBlendValue += Time.deltaTime * moveBlendDeltaValue;
                        moveBlendValue = Mathf.Clamp(moveBlendValue, -1f, 0f);
                    }
                }
            }
            else
            {
                if (player.MoveActionValue == 1)
                {
                    moveBlendValue -= Time.deltaTime * moveBlendDeltaValue;
                    moveBlendValue = Mathf.Clamp(moveBlendValue, -1f, 0f);
                }
                else if (player.MoveActionValue == -1)
                {
                    moveBlendValue += Time.deltaTime * moveBlendDeltaValue;
                    moveBlendValue = Mathf.Clamp(moveBlendValue, 0f, 1f);
                }
                else
                {
                    if (moveBlendValue > 0)
                    {
                        moveBlendValue -= Time.deltaTime * moveBlendDeltaValue;
                        moveBlendValue = Mathf.Clamp(moveBlendValue, 0f, 1f);
                    }
                    else if (moveBlendValue < 0)
                    {
                        moveBlendValue += Time.deltaTime * moveBlendDeltaValue;
                        moveBlendValue = Mathf.Clamp(moveBlendValue, -1f, 0f);
                    }
                }
            }
        }
        else // 대상 고정이 아닐 때
        {
            if (player.MoveActionValue == 1 || player.MoveActionValue == -1)
            {
                moveBlendValue += Time.deltaTime * moveBlendDeltaValue / 2;
            }
            else
            {
                moveBlendValue -= Time.deltaTime * moveBlendDeltaValue / 2;
            }

            moveBlendValue = Mathf.Clamp(moveBlendValue, 0f, 1f);
        }

        player.PlayerAnimator.SetFloat("MoveBlendValue", moveBlendValue); // 애니메이터의 moveBlendValue를 설정
    }

    public void Exit()
    {
        moveBlendValue = 0;
        player.PlayerAnimator.SetFloat("MoveBlendValue", moveBlendValue);
    }
}