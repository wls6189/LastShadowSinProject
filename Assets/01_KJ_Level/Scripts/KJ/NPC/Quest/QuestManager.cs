using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI.Table;

public class QuestManager : MonoBehaviour
{
    private static QuestManager instance;
    public static QuestManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("QuestManager"); //EventBus라는 빈 객체를 만들고
                instance = go.AddComponent<QuestManager>(); //EventBus 빈 객체에 EventBus 스크립트(컴포넌트)을 추가
            }
            return instance;
        }



    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

        }
        else // 1. go.AddComponent<EventBus>(); -> 2. Awake 실행이므로 evetbus가 아직 null이다. 그래서 eventbus = this를 해준다. 
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }

    [SerializeField]
    GameObject FinishQuestExplain;
    [SerializeField]
    GameObject NotFinishQuestExplain;

    public List<Quest> allActiveQuests; //활성화된 퀘스트를 보관할 리스트. 즉, 새로운 퀘스트가 시작 될 때 이 리스트에 추가됨
    public List<Quest> allCompletedQuests; //활성화된 퀘스트를 완료하고 나서 보관할 리스트. 즉, 퀘스트를 완료했을 때 이 리스트에 추가됨.


    [Header("QuestMenu")]
    public bool isQuestMenuOpen; //내 퀘스트 UI 활성화 여부

    public GameObject activeQuestPrefab;  //현재 활성화된 퀘스트를 표시하기 위한 UI 프리팹
    public GameObject completeQuestPrefab; //완료된 퀘스트를 표시하기 위한 UI 프리팹

    public GameObject activeQuestHintPrefab; //완료된 퀘스트를 표시하기 위한 UI 프리팹

    public GameObject questMenuContent; //퀘스트 메뉴의 스크롤뷰 콘텐츠 객체를 참조, 활성화된 퀘스트와 완료된 퀘스트를 UI에 동적으로 추가할 때,
                                        //이 객체의 자식으로 프리팹이 생성됩니다.

    [Header("QuestTracker")]
    public GameObject questTrackerContent; //현재 추적 중인 퀘스트 정보를 표시하는 UI의 콘텐츠 객체

    public void AddActiveQuest(string questGvier , Quest quest)
    {
        // allActiveQuests.Add(quest);

        DataManager.Instance.SaveActiveQuest(quest.questGiver,quest);

        RefreshQuestList();
    }



    public void MarkQuestCompleted(Quest quest)
    {

        //allCompletedQuests.Add(quest);

        DataManager.Instance.SaveCompletedQuest(quest.questGiver, quest);

        RefreshQuestList();
    }

    public void RefreshQuestList()//활성화된 퀘스트 목록 또는 완료된 퀘스트 목록에 대한 퀘스트 새로고침 기능이 필요.
    {
        FinishQuestExplain.gameObject.SetActive(false);
        NotFinishQuestExplain.gameObject.SetActive(false);


        foreach (Transform child in questMenuContent.transform)
        {
            Destroy(child.gameObject);
        }

        foreach(Quest actvieQuest in DataManager.Instance.nowPlayer.allActiveQuests)
        {
            GameObject questPrefab = Instantiate(activeQuestPrefab, Vector3.zero, Quaternion.identity); //활성화된 퀘스트 프리펩 생성
            questPrefab.transform.SetParent(questMenuContent.transform, false); //활성화된 퀘스트 프리펩의 부모를 Content로 설정

            QuestRow qRow = questPrefab.GetComponent<QuestRow>(); //활성화된 퀘스트 프리펩의 QuestRow 컴포넌트 가져옴

            qRow.quesetName.text = actvieQuest.questName;
            qRow.questGiver.text = actvieQuest.questGiver;

            qRow.isActive = true;
            qRow.isTracking = true;


            Button button = questPrefab.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => OnQuestClicked(actvieQuest));
            }
        }

        foreach (Quest completeQuest in DataManager.Instance.nowPlayer.allCompletedQuests)
        {
            GameObject questPrefab = Instantiate(completeQuestPrefab, Vector3.zero, Quaternion.identity); //활성화된 퀘스트 프리펩 생성
            questPrefab.transform.SetParent(questMenuContent.transform, false); //활성화된 퀘스트 프리펩의 부모를 Content로 설정

            QuestRow qRow = questPrefab.GetComponent<QuestRow>(); //활성화된 퀘스트 프리펩의 QuestRow 컴포넌트 가져옴

            qRow.quesetName.text = completeQuest.questName;
            qRow.questGiver.text = completeQuest.questGiver;

            qRow.isActive = false;
            qRow.isTracking = false;

            Button button = questPrefab.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => OnQuestFiniShClicked(completeQuest));
            }
        }
    }

    private void OnQuestClicked(Quest activeQuest)
    {
        NotFinishQuestExplain.gameObject.SetActive(true);
        FinishQuestExplain.gameObject.SetActive(false);

        QuestRow qRow = NotFinishQuestExplain.GetComponent<QuestRow>();
        qRow.quesetName.text = activeQuest.questName;
        qRow.questGiver.text = activeQuest.questGiver;
        qRow.questHint.text = activeQuest.info.hintExplain;
    }
    private void OnQuestFiniShClicked(Quest finishQuest)
    {
        FinishQuestExplain.gameObject.SetActive(true);
        NotFinishQuestExplain.gameObject.SetActive(false);

        QuestRow qRow = FinishQuestExplain.GetComponent<QuestRow>();

        qRow.quesetName.text = finishQuest.questName;
        qRow.questGiver.text = finishQuest.questGiver;

        qRow.questHint.text = finishQuest.info.hintExplain;

        qRow.firstReward.sprite = finishQuest.info.rewardImage;

        qRow.firstRewardAmount.text = finishQuest.info.firstRequirmentAmount.ToString();

    

    }
}
