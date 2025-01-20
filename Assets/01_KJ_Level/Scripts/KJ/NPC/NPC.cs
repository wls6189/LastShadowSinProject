using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    [SerializeField]
    private bool playerInRange; // 대화 시작 범위 안에 플레이어가 있는 경우에 대한 여부
    [SerializeField]
    GameObject player;
    [SerializeField]
    float rotationSpeed; //회전속도
    [SerializeField]
    private bool isTalkingwithPlayer;
    [SerializeField]
    GameObject NpcTalkImage; //Talk[F] 라는  텍스트를 보여주기 위함.

    Quaternion initRot;

    TextMeshProUGUI npcDialogText;

    TextMeshProUGUI npcGiverText;

    Button OptionFirstBtn; //옵션버튼1
    TextMeshProUGUI OptionFirstBtnText; //옵션버튼1텍스트

    Button OptionSecondBtn; //옵션버튼2
    TextMeshProUGUI OptionSecondBtnText; //옵션버튼2 텍스트

    GameObject TalkDialouge;  //대화하는 부분 보여주기 위한 이미지

    GameObject QuestionDialouge; //선택 버튼1,2 부분 보여주기 위한 이미지.

    Button FirstBtn; //선택 버튼 1

    Button SecondBtn; //선택 버튼 2

    Button BackBtn; //선택 버튼 2


    Button ThirdBtn; //보상 버튼 

    


    [Header("QuestInfo")]
    public List<Quest> Quests; // NPC가 가지고 있는 퀘스트 목록 관리
    public Quest currentActiveQuest;
    private int ActiveQuestIndex = 0;
    public bool isFirstTimeInteraction = true;
    private int CurrentFirstDialog = 0;
    private int CurrentSecondDialog = 0;

    void Start()
    {
    

        npcDialogText = DialogSystem.Instance.StoryDialogText;

        OptionFirstBtn = DialogSystem.Instance.optBtn1;
        OptionFirstBtnText = DialogSystem.Instance.optBtn1.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();

        OptionSecondBtn = DialogSystem.Instance.optBtn2;
        OptionSecondBtnText = DialogSystem.Instance.optBtn2.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();


        FirstBtn = DialogSystem.Instance.FirstBtn;
        SecondBtn = DialogSystem.Instance.SecondBtn;
        BackBtn = DialogSystem.Instance.BackBtn;
        ThirdBtn = DialogSystem.Instance.ReceiveBtn;

        TalkDialouge = DialogSystem.Instance.TalkDialouge;
        QuestionDialouge = DialogSystem.Instance.QuestDialouge;

        npcGiverText = DialogSystem.Instance.NpcGiverText;

        initRot = transform.rotation;
   
    }

    void Update()
    {
        if (playerInRange)
        {
            NpcTalkImage.gameObject.SetActive(true);

            NpcTalkImage.GetComponentInChildren<TextMeshProUGUI>().text = "Talk [F]";

            if (Input.GetKeyDown(KeyCode.F) && !isTalkingwithPlayer)
            {
                StartTalk();
            }
        }
        else
        {
            NpcTalkImage.gameObject.SetActive(false);
        }
    }

    private void StartTalk()
    {
        isTalkingwithPlayer = true;

        Vector3 dir = transform.position - player.transform.position;
        dir.y = 0;

        Quaternion targetRotation = Quaternion.LookRotation(dir);

        StartCoroutine(LookAtPlayerRoutine(targetRotation));

        if (isFirstTimeInteraction)
        {
            isFirstTimeInteraction = false;

            currentActiveQuest = Quests[ActiveQuestIndex];
            UpdateDialogUI(); // 대화 UI 초기화 및 시작

            CurrentFirstDialog = 0;
            CurrentSecondDialog = 0;
        }

        else
        {
            if (isDecline == true) //이전에 대화 첫 시작했을 때 퀘스트를 거절했을 것이다. 따라서 isDeclined가 당연히 true 였을 것임. 
            {
                UpdateDialogUI();
            }
            
            if(isAccepted && currentActiveQuest.isCompleted == false)
            {
                if(AreQuestRequirmentsCompleted())
                {
                    DialogSystem.Instance.OpenDialogUI();

                    SubmitRequiredItems();
                    npcDialogText.text = currentActiveQuest.info.FinishAnswer;

                    QuestionDialouge.gameObject.SetActive(false);
                    TalkDialouge.gameObject.SetActive(true);

                    FirstBtn.gameObject.SetActive(false);
                    SecondBtn.gameObject.SetActive(false);
                    ThirdBtn.gameObject.SetActive(true);

                    ThirdBtn.GetComponentInChildren<TextMeshProUGUI>().text = "[보상 받기]";
                    ThirdBtn.onClick.RemoveAllListeners();
                    ThirdBtn.onClick.AddListener(() =>
                    {
                        isTalkingwithPlayer = false;
                        TalkStop();

                        ReceiveReward();
                        DialogSystem.Instance.CloseDialogUI();
                    });

                }
                else
                {
                    DialogSystem.Instance.OpenDialogUI();

                    QuestionDialouge.gameObject.SetActive(false); //옵션 이미지 가리기.

                    TalkDialouge.gameObject.SetActive(true); //대화 이미지 보이기

                    FirstBtn.gameObject.SetActive(false); //버튼 가리기1
                    SecondBtn.gameObject.SetActive(false);//버튼 가리기2

                    npcDialogText.text = currentActiveQuest.info.AcceptCombackAnswer;

                    StartCoroutine(delayTalkText());

                }
            }

            if (currentActiveQuest.isCompleted == true) //퀘스트를 완료했을 경우. 즉, 이미 이전에 동일한 퀘스트를 완료했는데, 또 보상 받을려고? ㅋ
            {
                DialogSystem.Instance.OpenDialogUI();

                TalkDialouge.gameObject.SetActive(true);

                FirstBtn.gameObject.SetActive(false);
                SecondBtn.gameObject.SetActive(false);
                ThirdBtn.gameObject.SetActive(false);

                npcDialogText.text = currentActiveQuest.info.CombackFinishAnswer;

                StartCoroutine(delayTalkText());

            }
        }

       

    }

 
   


    [SerializeField]
    bool isFirstOption; 
    [SerializeField]
    bool isSecondOption;
    private void UpdateDialogUI()
    {
        CurrentFirstDialog = 0;
        CurrentSecondDialog = 0;
        DialogSystem.Instance.OpenDialogUI();

        QuestionDialouge.gameObject.SetActive(true);
        TalkDialouge.gameObject.SetActive(false);

        FirstBtn.gameObject.SetActive(true);
        SecondBtn.gameObject.SetActive(true);
        BackBtn.gameObject.SetActive(true);

        ThirdBtn.gameObject.SetActive(false);

        npcGiverText.text = currentActiveQuest.questGiver; //퀘스트 제공자 동기화 부분


        isFirstOption = false;
        isSecondOption = false;

        OptionFirstBtnText.text = currentActiveQuest.info.InitialFirstQuestion;
        OptionFirstBtn.onClick.RemoveAllListeners();
        OptionFirstBtn.onClick.AddListener(() =>
        {
            isFirstOption = true;

            QuestionDialouge.gameObject.SetActive(false);
            TalkDialouge.gameObject.SetActive(true);

            npcDialogText.text = currentActiveQuest.info.InitialFirstAnswer;
            AnswerButton();
        });

        OptionSecondBtnText.text = currentActiveQuest.info.InitialSecondQuestion;
        OptionSecondBtn.onClick.RemoveAllListeners();
        OptionSecondBtn.onClick.AddListener(() =>
        {
            isSecondOption = true;

            QuestionDialouge.gameObject.SetActive(false);
            TalkDialouge.gameObject.SetActive(true);

            npcDialogText.text = currentActiveQuest.info.InitialSecondAnswer; //마지막 대화 표시

            AnswerButton();
        });
        BackBtn.GetComponentInChildren<TextMeshProUGUI>().text = "돌아가기";
        BackBtn.onClick.RemoveAllListeners();
        BackBtn.onClick.AddListener(() =>
        {
            isTalkingwithPlayer = false;
            TalkStop();

        });

    }

 
    private void AnswerButton()
    { 
        //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ 1번째 버튼 @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        if (isFirstOption)
        {
            if (CurrentFirstDialog < currentActiveQuest.info.First_Answer1.Count)
            {
                string question = currentActiveQuest.info.First_Question1[CurrentFirstDialog];

                FirstBtn.GetComponentInChildren<TextMeshProUGUI>().text = question;
                FirstBtn.onClick.RemoveAllListeners();
                FirstBtn.onClick.AddListener(() =>
                {
                    // FirstDialog 범위 초과 방지
                    string lastNpcTalk = currentActiveQuest.info.First_Answer1[CurrentFirstDialog];
                    npcDialogText.text = lastNpcTalk;

                    CurrentFirstDialog++;

                    // FirstDialog 범위 초과 방지
                    if (CurrentFirstDialog >= currentActiveQuest.info.First_Answer1.Count)
                    {

                        CurrentFirstDialog = currentActiveQuest.info.First_Answer1.Count - 1;

                        QuestCheack(lastNpcTalk);
                        return;
                    }
                    AnswerButton();
                });
            }
            // Second 버튼 설정
            if (CurrentSecondDialog < currentActiveQuest.info.First_Answer2.Count)
            {
                string question = currentActiveQuest.info.First_Question2[CurrentSecondDialog];

                SecondBtn.GetComponentInChildren<TextMeshProUGUI>().text = question;
                SecondBtn.onClick.RemoveAllListeners();
                SecondBtn.onClick.AddListener(() =>
                {
                    // FirstDialog 범위 초과 방지
                    string lastNpcTalk = currentActiveQuest.info.First_Answer2[CurrentSecondDialog];
                    npcDialogText.text = lastNpcTalk;

                    CurrentSecondDialog++;

                    // FirstDialog 범위 초과 방지
                    if (CurrentSecondDialog >= currentActiveQuest.info.First_Answer2.Count)
                    {

                        CurrentSecondDialog = currentActiveQuest.info.First_Answer2.Count - 1;

                        QuestCheack(lastNpcTalk);
                        return;
                    }
                    AnswerButton();
                });
            }
        }

        //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ 2번째 버튼 @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        if(isSecondOption)
        {
            if (CurrentFirstDialog < currentActiveQuest.info.Second_Answer1.Count)
            {
                string question = currentActiveQuest.info.Second_Question1[CurrentFirstDialog];

                FirstBtn.GetComponentInChildren<TextMeshProUGUI>().text = question;
                FirstBtn.onClick.RemoveAllListeners();
                FirstBtn.onClick.AddListener(() =>
                { 
                    // FirstDialog 범위 초과 방지
                    string lastNpcTalk = currentActiveQuest.info.Second_Answer1[CurrentFirstDialog];
                    npcDialogText.text = lastNpcTalk;

                    CurrentFirstDialog++;

                    // FirstDialog 범위 초과 방지
                    if (CurrentFirstDialog >= currentActiveQuest.info.Second_Answer1.Count)
                    {

                        CurrentFirstDialog = currentActiveQuest.info.Second_Answer1.Count - 1;

                        QuestCheack(lastNpcTalk);
                        return;
                    }
                    AnswerButton();
                });
            }
            // Second 버튼 설정
            if (CurrentSecondDialog < currentActiveQuest.info.Second_Answer2.Count)
            {
                string question = currentActiveQuest.info.Second_Question2[CurrentSecondDialog];

                SecondBtn.GetComponentInChildren<TextMeshProUGUI>().text = question;
                SecondBtn.onClick.RemoveAllListeners();
                SecondBtn.onClick.AddListener(() =>
                {
                    string lastNpcTalk = currentActiveQuest.info.Second_Answer2[CurrentSecondDialog];
                    npcDialogText.text = lastNpcTalk;

                    CurrentSecondDialog++;

                    // FirstDialog 범위 초과 방지
                    if (CurrentSecondDialog >= currentActiveQuest.info.Second_Answer2.Count)
                    {
                       
                        CurrentSecondDialog = currentActiveQuest.info.Second_Answer2.Count - 1;

                        QuestCheack(lastNpcTalk);
                        return;
                    }
                    AnswerButton();
                });
            }
        }
    
    }

    private void QuestCheack(string laskTalk)
    {
        if (IsQuestRelated(laskTalk))
        {
            AcceptAndDecline();        
        }
        else
        {
            FirstBtn.gameObject.SetActive(false);
            SecondBtn.gameObject.SetActive(false);

            Invoke("UpdateDialogUI", 1.0f);
        }
    }

    [SerializeField]
    bool isAccepted;
    [SerializeField]
    bool isDecline;
    private void AcceptAndDecline()
    {
        FirstBtn.GetComponentInChildren<TextMeshProUGUI>().text = currentActiveQuest.info.finalFirstAnswer;
        FirstBtn.onClick.RemoveAllListeners();
        FirstBtn.onClick.AddListener(() =>
        {
            isAccepted = true;
            isDecline = false;
            FirstBtn.gameObject.SetActive(false);
            SecondBtn.gameObject.SetActive(false);

            npcDialogText.text = currentActiveQuest.info.AcceptThankyouAnswer;


            QuestManager.Instance.AddActiveQuest(currentActiveQuest);

            StartCoroutine(delayTalkText());
        });
        SecondBtn.GetComponentInChildren<TextMeshProUGUI>().text = currentActiveQuest.info.finalSecondAnswer;
        SecondBtn.onClick.RemoveAllListeners();
        SecondBtn.onClick.AddListener(() =>
        {
            isDecline = true;
            isAccepted = false;
            FirstBtn.gameObject.SetActive(false);
            SecondBtn.gameObject.SetActive(false);
            npcDialogText.text = currentActiveQuest.info.DeclineAnswer;

            StartCoroutine(delayTalkText());
        });
    }

    IEnumerator delayTalkText()
    {
        yield return new WaitForSecondsRealtime(1.0f);
        TalkStop();
    }
    //private void AcceptedQuest()
    //{

    //    if (currentActiveQuest.isHasNoRequirements) //퀘스트에 보상있는 퀘스트라면 
    //    {
    //        npcDialogText.text = currentActiveQuest.info.combackCompleted;
    //        AcceptText.text = "[Take Reward]";
    //        NpcAcceptBtn.onClick.RemoveAllListeners();
    //        NpcAcceptBtn.onClick.AddListener(() =>
    //        {
    //            ReceiveReward();
    //            NpcRotation();
    //        });
    //        NpcDeclineBtn.gameObject.SetActive(false); //거절 버튼 비활성화.
    //    }
    //    else //퀘스트에 보상이 없는 퀘스트 라면 
    //    {
    //        npcDialogText.text = currentActiveQuest.info.acceptAnswer;
    //        CloseDialogUI();
    //    }
    //}

    private bool AreQuestRequirmentsCompleted() //Npc가 요청한 요구사항들을 플레이어가 갖고 있는지 체크
    {

        string firstRequiredItem = currentActiveQuest.info.firstRequirmentItem;
        int firstRequiredAmount = currentActiveQuest.info.firstRequirmentAmount;

        var firstItemCounter = 0; //현재 내가 들고있는 첫번째 요구한 아이템의 개수 0으로 잡고


        foreach (var item in player.GetComponent<PlayerControllerTest>().collectedItems)
        {
            if (item.Key == firstRequiredItem)
            {
                firstItemCounter += item.Value; //현재 내가 들고있는 첫번째 요구한 아이템의 개수를 1씩 증가
            }
        }

        string secondRequiredItem = currentActiveQuest.info.secondRequirmentItem;
        int secondRequirmentAmount = currentActiveQuest.info.secondRequirmentAmount;

        var secondItemCounter = 0;

        //@@@@@@@ NPC가 요구했던 것들이 더 많다면 여기 아래에 더 추가 할 수 있음@@@@@@@@@ 


        //현재 내가 들고있는 첫번째 또는 두번째 요구한 아이템의 개수가 >= 목표치보다 많을 경우
        if (firstItemCounter >= firstRequiredAmount && secondItemCounter >= secondRequirmentAmount)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void SubmitRequiredItems() //NPC가 요구했던 아이템들을 플레이어가 제출하는 메서드
    {
        string firstRequiredItem = currentActiveQuest.info.firstRequirmentItem;
        int firstRequiredAmount = currentActiveQuest.info.firstRequirmentAmount;

        if (firstRequiredItem != "")
        {
            player.GetComponent<PlayerControllerTest>().RemoveItem(firstRequiredItem, firstRequiredAmount);
        }

        string secondRequiredItem = currentActiveQuest.info.secondRequirmentItem;
        int secondRequirmentAmount = currentActiveQuest.info.secondRequirmentAmount;

        if (secondRequiredItem != "")
        {
            player.GetComponent<PlayerControllerTest>().RemoveItem(secondRequiredItem, secondRequirmentAmount);
        }

        //@@@@@@@ NPC가 요구했던 것들이 더 많다면 여기 아래에 더 추가 할 수 있음@@@@@@@@@ 
    }
    private void ReceiveReward()
    {
        QuestManager.Instance.MarkQuestCompleted(currentActiveQuest);

        currentActiveQuest.isCompleted = true; //퀘스트 완료


        if (currentActiveQuest.info.rewardItem1 != "")
        {
            player.GetComponent<PlayerControllerTest>().CollectItem(currentActiveQuest.info.rewardItem1);
        }

        if (currentActiveQuest.info.rewardItem2 != "")
        {
            player.GetComponent<PlayerControllerTest>().CollectItem(currentActiveQuest.info.rewardItem2);
        }

        //@@@@@@@ NPC가 보상해줄 것들이 더 많다면 여기 아래에 더 추가 할 수 있음@@@@@@@@@ 




        //ActiveQuestIndex++; //NPC_A에게 전달 받았던 퀘스트를 완료하고, NPC_A의 두번째 퀘스트를 시작하려고 인덱스 증가.

        //NPC_A의 두번째 퀘스트 시작.
        //if (ActiveQuestIndex <= quests.Count)
        //{
        //    currentActiveQuest = quests[ActiveQuestIndex]; //이전과는 다른 퀘스트 이고, 위에서 ActiveQuestIndex을 1증가 시켰기 때문에 
        //    //퀘스트도 다를 것임. 따라서 다른 퀘스트를 가져와서 현재 활성화된 퀘스트에 저장

        //    CurrentDialog = 0; //CurrentDialog을 0으로 해서 대화상자를 재 설정
        //    DialogSystem.Instance.CloseDialogUI(); //대화 UI 비활성화
        //    isTalkingwithPlayer = false;
        //    NpcRotation();
        //}
        //else
        //{
        //    DialogSystem.Instance.CloseDialogUI();
        //    isTalkingwithPlayer = false;

        //    Debug.Log(this.gameObject.name + "의 퀘스트는 더 이상 없다.");
        //}
    }
    private bool IsQuestRelated(string question)
    {
        // 특정 조건으로 퀘스트 대화를 식별
        return question.Contains("퀘스트") || question.Contains("미션") || question.Contains("임무");
    }

    public void TalkStop()
    {
        isTalkingwithPlayer = false;
        StartCoroutine(LookAtPlayerRoutine(initRot));
        DialogSystem.Instance.CloseDialogUI();
    }


    private IEnumerator LookAtPlayerRoutine(Quaternion targetRotation)
    {


        while (Quaternion.Angle(transform.rotation, targetRotation) > 1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            yield return null;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
