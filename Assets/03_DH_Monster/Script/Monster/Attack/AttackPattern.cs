using UnityEngine;

[CreateAssetMenu(fileName = "NewAttackPattern", menuName = "Enemy/AttackPattern")]
public class AttackPattern : ScriptableObject
{
    public string patternName; // 패턴 이름
    public Attack[] attacks; // 패턴에 포함된 공격
}