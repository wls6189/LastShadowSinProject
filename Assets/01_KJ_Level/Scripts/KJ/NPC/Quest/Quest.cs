using UnityEngine;

[System.Serializable] //����ȭ -> ������ ���� ����
public class Quest //������ �����ϴ� Ŭ����
{
    public string questName;
    public string questGiver;

    [Header("Quest Info")]
    public QuestInfo info; //����Ʈ�� ���� ���� ������ ��� �ִ� ��ü.
}
