using UnityEngine;

public class DestructiveESM : EternalSpiritMark
{
    float increaseAmount = 0;
    public DestructiveESM() : base(
        "파괴적인 영원의 영혼낙인",
        "모든 공격이 적중할 때마다 10의 영혼낙인력을 얻습니다.",
        "영혼낙인력이 가득 찼을 때, 영혼낙인 능력 사용 시 모든 영혼낙인력을 소모하고 양 방향으로 공격력의 300%의 피해를 주는 소용돌이를 발사합니다.",
        10,
        false,
        0f
        )
    { }

    public override void Ability(PlayerController player) // Update에서 호출 요
    {
        if (player.PlayerStats.CurrentSpiritMarkForce >= player.PlayerStats.MaxSpiritMarkForce && player.spiritMarkAbilityAction.WasPressedThisFrame())
        {
            player.PlayerStats.CurrentSpiritMarkForce = 0;
            player.FireDestructiveESMProjectile();
        }
    }
}