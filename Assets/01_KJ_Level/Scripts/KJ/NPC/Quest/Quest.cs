using UnityEngine;

[System.Serializable] //직렬화 -> 에디터 편집 가능
public class Quest //정보를 포함하는 클래스
{
    public string questName;
    public string questGiver;

    public bool isCompleted;
    public bool isCombackCompleted;

    [Header("Quest Info")]
    public QuestInfo info; //퀘스트에 대한 세부 정보를 담고 있는 객체.
}
