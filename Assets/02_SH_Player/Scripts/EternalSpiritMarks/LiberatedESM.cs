using UnityEngine;

public class LiberatedESM : EternalSpiritMark
{
    public LiberatedESM() : base(
        "해방된 영원의 영혼낙인",
        "모든 공격이 적중할 때마다 5의 영혼낙인력을 얻습니다.",
        "영혼낙인력이 가득 찼을 때, 영혼낙인 능력 사용 시 모든 영혼낙인력을 소모하고 6초 간 영혼 해방 상태가 됩니다. 영혼 해방 상태에서는 기본 내려찍기(X)만 가능하며 공격 속도가 1.5배 상승합니다.",
        5,
        false,
        6f
        )
    { }

    public override void Ability(PlayerController player) // Update에서 호출 요
    {
        if (player.PlayerStats.CurrentSpiritMarkForce >= player.PlayerStats.MaxSpiritMarkForce && player.spiritMarkAbilityAction.WasPressedThisFrame())
        {
            player.PlayerStats.CurrentSpiritMarkForce = 0;
            player.PlayerAnimator.SetFloat("AttackSpeed", 1.5f);
            RemainDuration = Duration;
        }

        if (RemainDuration > 0)
        {
            RemainDuration -= Time.deltaTime;
        }

        if (RemainDuration < 0)
        {
            player.PlayerAnimator.SetFloat("AttackSpeed", 1f);
            RemainDuration = 0;
        }
    }
}
