using UnityEngine;

public class Mark
{
    public string Name { get; }
    public string Description { get; }

    protected Mark(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public virtual void Effect(PlayerController player, int count) { }// Awake에서 호출 요
    public override bool Equals(object obj) // 이름이 같으면 같다고 해버리기
    {
        if (obj is Mark other)
        {
            return Name == other.Name;
        }

        return false;
    }
    public override int GetHashCode() // 이름으로 해쉬코드 반환하기
    {
        return Name.GetHashCode();
    }
}
