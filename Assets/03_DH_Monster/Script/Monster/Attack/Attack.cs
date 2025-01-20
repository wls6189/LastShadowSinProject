using UnityEngine;

[CreateAssetMenu(fileName = "New Attack", menuName = "Enemy/Attack")]
public class Attack : ScriptableObject
{
    public string attackName;              // 공격 이름
    public AttackType attackType;          // 공격 타입
    public float attackRange;              // 공격 사거리

    [Header("(indicator Settings")]
    public int indicatorhLevel;             // 전조 단계 (느낌표 개수)
    public Color indicatorColor;           // 전조 색상
}