using System.Collections;
using UnityEngine;

public class GuardState : IState
{
    PlayerController player;
    float guardBlendValue = 0;
    float guardBlendDeltaValue = 3f;

    public GuardState(PlayerController player)
    {
        this.player = player;
    }
    public void Enter()
    {
        player.CurrentPlayerState = PlayerState.Guard;
        player.Animator.SetTrigger("DoGuard");
        player.IsParring = true;
        player.IsGuarding = true;
    }

    public void Execute()
    {
        // 플레이어의 실제 이동
        Vector3 moveVector = new Vector3(0, 0, player.MoveActionValue);
        player.CharacterController.Move(moveVector * player.MoveSpeed * Time.deltaTime / 2);

        // 이동 애니메이션 
        if (player.IsLockOn) // 대상 고정 중일 때
        {
            if (player.IsLookRight) // 적이 오른쪽에 있는 경우
            {
                if (player.MoveActionValue == 1)
                {
                    guardBlendValue += Time.deltaTime * guardBlendDeltaValue * 2;
                    guardBlendValue = Mathf.Clamp(guardBlendValue, 0f, 1f);
                }
                else if (player.MoveActionValue == -1)
                {
                    guardBlendValue -= Time.deltaTime * guardBlendDeltaValue * 2;
                    guardBlendValue = Mathf.Clamp(guardBlendValue, -1f, 0f);
                }
                else
                {
                    if (guardBlendValue > 0)
                    {
                        guardBlendValue -= Time.deltaTime * guardBlendDeltaValue * 2;
                        guardBlendValue = Mathf.Clamp(guardBlendValue, 0f, 1f);
                    }
                    else if (guardBlendValue < 0)
                    {
                        guardBlendValue += Time.deltaTime * guardBlendDeltaValue * 2;
                        guardBlendValue = Mathf.Clamp(guardBlendValue, -1f, 0f);
                    }
                }
            }
            else
            {
                if (player.MoveActionValue == 1)
                {
                    guardBlendValue -= Time.deltaTime * guardBlendDeltaValue * 2;
                    guardBlendValue = Mathf.Clamp(guardBlendValue, -1f, 0f);
                }
                else if (player.MoveActionValue == -1)
                {
                    guardBlendValue += Time.deltaTime * guardBlendDeltaValue * 2;
                    guardBlendValue = Mathf.Clamp(guardBlendValue, 0f, 1f);
                }
                else
                {
                    if (guardBlendValue > 0)
                    {
                        guardBlendValue -= Time.deltaTime * guardBlendDeltaValue * 2;
                        guardBlendValue = Mathf.Clamp(guardBlendValue, 0f, 1f);
                    }
                    else if (guardBlendValue < 0)
                    {
                        guardBlendValue += Time.deltaTime * guardBlendDeltaValue * 2;
                        guardBlendValue = Mathf.Clamp(guardBlendValue, -1f, 0f);
                    }
                }
            }
        }
        else // 대상 고정이 아닐 때
        {
            if (player.MoveActionValue == 1 || player.MoveActionValue == -1)
            {
                guardBlendValue += Time.deltaTime * guardBlendDeltaValue;
            }
            else
            {
                guardBlendValue -= Time.deltaTime * guardBlendDeltaValue;
            }

            guardBlendValue = Mathf.Clamp(guardBlendValue, 0f, 1f);
        }

        player.Animator.SetFloat("GuardBlendValue", guardBlendValue); // 애니메이터의 moveBlendValue를 설정
    }

    public void Exit()
    {
        player.IsParring = false;
        player.IsGuarding = false;
        guardBlendValue = 0;
        player.Animator.SetFloat("GuardBlendValue", guardBlendValue);
    }
}