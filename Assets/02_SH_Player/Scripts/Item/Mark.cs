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

    public virtual void Effect(PlayerController player, int count) { }// Awake���� ȣ�� ��
    public override bool Equals(object obj) // �̸��� ������ ���ٰ� �ع�����
    {
        if (obj is Mark other)
        {
            return Name == other.Name;
        }

        return false;
    }
    public override int GetHashCode() // �̸����� �ؽ��ڵ� ��ȯ�ϱ�
    {
        return Name.GetHashCode();
    }
}
