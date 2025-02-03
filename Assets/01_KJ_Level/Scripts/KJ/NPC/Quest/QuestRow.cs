using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestRow : MonoBehaviour
{

    public TextMeshProUGUI quesetName; //퀘스트 이름
    public TextMeshProUGUI questGiver; //퀘스트 제공자(npc)

    public TextMeshProUGUI questHint; //퀘스트 제공자(npc)




    public bool isActive;
    public bool isTracking;

    //public TextMeshProUGUI coinAmount;  //코인의 양

    public Image firstReward; //첫번째 보상 이미지
    public TextMeshProUGUI firstRewardAmount; //첫번째 보상의 양



}
