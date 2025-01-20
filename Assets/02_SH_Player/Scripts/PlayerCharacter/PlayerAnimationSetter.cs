using System.Collections;
using UnityEngine;

public class PlayerAnimationSetter : MonoBehaviour
{
    PlayerController player;
    AnimationClip[] clips;
    Coroutine attackCoroutine;

    float basicHorizonSlash1Frame = 35;
    float basicHorizonSlash2Frame = 37;
    float basicVerticalSlashFrame = 40;

    void Awake()
    {
        player = GetComponent<PlayerController>();

        // 애니메이터에 있는 모든 애니메이션 클립을 가져오기
        RuntimeAnimatorController controller = player.Animator.runtimeAnimatorController;
        clips = controller.animationClips;
    }

    public void StartBasicHorizonSlash1()
    {
        StopAllCoroutines();
        StartCoroutine(BasicHorizonSlash1Coroutine());
    }

    IEnumerator BasicHorizonSlash1Coroutine()
    {
        float fps = FindAnimationClip("BasicHorizonSlash1").length / basicHorizonSlash1Frame;

        yield return new WaitForSeconds(fps * 5);
        OnAttackMoving();

        yield return new WaitForSeconds(fps);
        OnWeaponCollider();

        yield return new WaitForSeconds(fps * 4);
        OffWeaponCollider();
        OffAttackMoving();

        yield return new WaitForSeconds(fps * 15);
        OnBasicHorizonSlashCombo();
        AttackEnd();

        yield return new WaitForSeconds(fps * 8);
        OffBasicHorizonSlashCombo();
    }

    public void StartBasicHorizonSlash2()
    {
        StopAllCoroutines();
        StartCoroutine(BasicHorizonSlash2Coroutine());
    }

    IEnumerator BasicHorizonSlash2Coroutine()
    {
        float fps = FindAnimationClip("BasicHorizonSlash2").length / basicHorizonSlash2Frame;
        OffBasicHorizonSlashCombo();

        yield return new WaitForSeconds(fps * 6);
        OnAttackMoving();

        yield return new WaitForSeconds(fps);
        OnWeaponCollider();

        yield return new WaitForSeconds(fps * 4);
        OffWeaponCollider();
        OffAttackMoving();

        yield return new WaitForSeconds(fps * 19);
        AttackEnd();
    }

    Coroutine basicVerticalSlashRoutine;
    public void StartBasicVerticalSlash()
    {
        if (basicVerticalSlashRoutine == null)
        {
            StopAllCoroutines();
            basicVerticalSlashRoutine = StartCoroutine(BasicVerticalSlashCoroutine());
        }
    }
    IEnumerator BasicVerticalSlashCoroutine()
    {
        float fps = FindAnimationClip("BasicVerticalSlash").length / basicVerticalSlashFrame;
        AttackStart();

        yield return new WaitForSeconds(fps * 8);
        OnAttackMoving();
        OnWeaponCollider();

        yield return new WaitForSeconds(fps * 5);
        OffWeaponCollider();
        OffAttackMoving();

        yield return new WaitForSeconds(fps * 15);
        AttackEnd();

        yield return new WaitForSeconds(fps * 9);
        TransitionToIdle();
    }













    AnimationClip FindAnimationClip(string name)
    {
        foreach (AnimationClip clip in clips)
        {
            if (clip.name == name)
            {
                return clip;
            }
        }

        return null;
    }

    public void OnWeaponCollider()
    {
        player.WeaponCollider.enabled = true;
    }
    public void OffWeaponCollider()
    {
        player.WeaponCollider.enabled = false;
    }
    public void OnBasicHorizonSlashCombo()
    {
        player.CanBasicHorizonSlashCombo = true;
    }
    public void OffBasicHorizonSlashCombo()
    {
        player.CanBasicHorizonSlashCombo = false;
    }
    public void AttackStart()
    {
        player.IsAttacking = true;
    }
    public void AttackEnd()
    {
        player.IsAttacking = false;
    }
    public void TransitionToIdle()
    {
        player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.idleAndMoveState);
    }
    public void OnAttackMoving()
    {
        player.IsAttackMoving = true;
    }
    public void OffAttackMoving()
    {
        player.IsAttackMoving = false;
    }
}
