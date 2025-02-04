using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    public bool playerInRange; // 대화 시작 범위 안에 플레이어가 있는 경우에 대한 여부
    [SerializeField]
    GameObject player;
    [SerializeField]
    float rotationSpeed; //회전속도

  
    [SerializeField]
    public GameObject NpcTalkImage; //Talk[F] 라는  텍스트를 보여주기 위함.

    Quaternion initRot;

    TextMeshProUGUI npcDialogText;

    TextMeshProUGUI npcGiverText;

    Button OptionFirstBtn; //옵션버튼1
    TextMeshProUGUI OptionFirstBtnText; //옵션버튼1텍스트

    Button OptionSecondBtn; //옵션버튼2
    TextMeshProUGUI OptionSecondBtnText; //옵션버튼2 텍스트

    GameObject PlayerDialouge;  //대화하는 부분 보여주기 위한 이미지

    GameObject NpcDialouge; //선택 버튼1,2 부분 보여주기 위한 이미지.

  
    Button BackBtn; //선택 버튼 2
    TextMeshProUGUI BackBtnText; //선택 버튼 2


    Button ThirdBtn; //보상 버튼 

    [SerializeField]
    bool isAccepted;
    [SerializeField]
    bool isDecline;


    [Header("QuestInfo")]
    public List<Quest> Quests; // NPC가 가지고 있는 퀘스트 목록 관리
    public Quest currentActiveQuest;
    private int ActiveQuestIndex = 0;
    public bool isFirstTimeInteraction = true;


    //private int CurrentFirstDialog = 0;
    //private int CurrentSecondDialog = 0;



    public string QuestGiver;

    private PlayerInteraction playerInteraction;

    void Start()
    {
        

        if (DataManager.Instance.nowPlayer.questGivers.Contains(QuestGiver))
        {
            if (DataManager.Instance.nowPlayer.allCompletedQuests.Count > 0)
            {
                currentActiveQuest = Quests[ActiveQuestIndex];
            }
        }
           
        playerInteraction = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInteraction>();

        if (DialogSystem.Instance != null)
        {
            npcDialogText = DialogSystem.Instance.StoryDialogText;

            OptionFirstBtn = DialogSystem.Instance.optBtn1;
            OptionFirstBtnText = DialogSystem.Instance.optBtn1.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();

            OptionSecondBtn = DialogSystem.Instance.optBtn2;
            OptionSecondBtnText = DialogSystem.Instance.optBtn2.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();


          
            BackBtn = DialogSystem.Instance.BackBtn;
            BackBtnText = DialogSystem.Instance.BackBtn.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();


            ThirdBtn = DialogSystem.Instance.ReceiveBtn;

            NpcDialouge = DialogSystem.Instance.NpcDialouge;
            PlayerDialouge = DialogSystem.Instance.PlayerTalkDialouge;

            npcGiverText = DialogSystem.Instance.NpcGiverText;

            initRot =  transform.rotation;
        }

        
    }



    public void PlayerWithTalk()
    {    
        NpcTalkImage.GetComponentInChildren<TextMeshProUGUI>().text = "Talk [F]";

        StartTalk();
    }
    public void PlayerLookNpc(Vector3 playerPos)
    {
        Vector3 target = playerPos;

        Vector3 dir = transform.position - target;
        dir.y = 0;

        Quaternion targetRotation = Quaternion.LookRotation(dir) * Quaternion.Euler(0, 180, 0);

        StartCoroutine(LookAtPlayerRoutine(targetRotation));
    }

  
    private void StartTalk()
    {
        #region 잠시대기
        //if (DataManager.Instance.nowPlayer.questGivers.Contains(QuestGiver))
        //{
        //    Debug.Log("0");

        //    if (currentActiveQuest.isCompleted == true) //퀘스트를 완료했을 경우. 즉, 이미 이전에 동일한 퀘스트를 완료했는데, 또 보상 받을려고? ㅋ
        //    {
        //        DialogSystem.Instance.OpenDialogUI();

        //        PlayerDialouge.gameObject.SetActive(true);

        //        ThirdBtn.gameObject.SetActive(false);

        //        npcDialogText.text = currentActiveQuest.info.CombackFinishAnswer;

        //        StartCoroutine(delayTalkStop());

        //        return;
        //    }

        //    if (AreQuestRequirmentsCompleted())
        //    {
        //        Debug.Log("2");
        //        DialogSystem.Instance.OpenDialogUI();


        //        SubmitRequiredItems();

        //        npcDialogText.text = currentActiveQuest.info.FinishAnswer;

        //        NpcDialouge.gameObject.SetActive(false);
        //        PlayerDialouge.gameObject.SetActive(true);

        //        ThirdBtn.gameObject.SetActive(true);

        //        ThirdBtn.GetComponentInChildren<TextMeshProUGUI>().text = "[보상 받기]";
        //        ThirdBtn.onClick.RemoveAllListeners();
        //        ThirdBtn.onClick.AddListener(() =>
        //        {

        //            TalkStop();

        //            ReceiveReward();
        //            DialogSystem.Instance.CloseDialogUI();
        //        });


        //    }
        //    else
        //    {
        //        DialogSystem.Instance.OpenDialogUI();

        //        NpcDialouge.gameObject.SetActive(true); //옵션 이미지 가리기.

        //        PlayerDialouge.gameObject.SetActive(false); //대화 이미지 보이기


        //        currentActiveQuest = Quests[ActiveQuestIndex];
        //        npcDialogText.text = currentActiveQuest.info.AcceptCombackAnswer;

        //        StartCoroutine(delayTalkStop());



        //    }



        //    return;
        //}

        #endregion 

        if (isFirstTimeInteraction) //첫 조우
        {
            isFirstTimeInteraction = false;

            NpcDialouge.gameObject.SetActive(true);
            PlayerDialouge.gameObject.SetActive(false);
          

            currentActiveQuest = Quests[ActiveQuestIndex];
            npcDialogText.text = currentActiveQuest.info.InitExplain;
    

            StartCoroutine(DelayToPlayerDialouge(1.5f));

            FirstInteraction(); // 대화 UI 초기화 및 시작        
        } 

        if(DataManager.Instance.nowPlayer.isPromise)
        {
            if (DataManager.Instance.nowPlayer.isOpenChaosRift == true) //혼돈의 틈새 열려진 상태에서 첫 조우
            {             
                if (AreQuestRequirmentsCompleted()) //혼돈의 틈새 열렸는지 닫혔는지에 대한 메서드 호출
                {
                    //열었음.  
                    DataManager.Instance.nowPlayer.isOpenChaosRift = false;
                }
                else //여전히 닫혀 있음.
                {                  
                    Debug.Log("약속 조건인 혼돈의 틈새 여는거 불 충족");

                    DialogSystem.Instance.OpenDialogUI();

                    NpcDialouge.gameObject.SetActive(true);
                    PlayerDialouge.gameObject.SetActive(false);

                    npcDialogText.text = currentActiveQuest.info.OpenChaosRiftExplain;


                    StartCoroutine(DelayToPlayerDialouge(1.5f));

                    OpenChaosRiftInteraction();
                }
            }
             if (DataManager.Instance.nowPlayer.isOpenChaosRift == false)//혼돈의 틈새 닫혀진 상태에서 첫 조우
            {

                Debug.Log("약속 조건인 혼돈의 틈새 여는거 충족");
                DialogSystem.Instance.OpenDialogUI();

                NpcDialouge.gameObject.SetActive(true);
                PlayerDialouge.gameObject.SetActive(false);

                npcDialogText.text = currentActiveQuest.info.CloseChaosRiftExplain;

                StartCoroutine(DelayToPlayerDialouge(1.5f));

                CloseChaosRiftInteraction();
            }

            if (DataManager.Instance.nowPlayer.isReCloseChaosRift == true && 
                DataManager.Instance.nowPlayer.isOpenChaosRift == false) //혼돈의 틈새가 닫혀지고 재조우
            {
                DialogSystem.Instance.OpenDialogUI();

                NpcDialouge.gameObject.SetActive(true);
                PlayerDialouge.gameObject.SetActive(false);

                npcDialogText.text = currentActiveQuest.info.ReCloseChaosRiftExplain;

                StartCoroutine(DelayToPlayerDialouge(1.5f));

                ReCloseChaosRiftInteraction();
            }
        }
    }

    private void FirstInteraction()
    {

        ThirdBtn.gameObject.SetActive(false); //보상 버튼 비활성화

        npcGiverText.text = currentActiveQuest.questGiver; //퀘스트 제공자 동기화 부분

        OptionFirstBtnText.text = currentActiveQuest.info.InitialFirstQuestion;
        OptionFirstBtn.onClick.RemoveAllListeners();
        OptionFirstBtn.onClick.AddListener(() =>
        {
            PlayerDialouge.gameObject.SetActive(false);
            NpcDialouge.gameObject.SetActive(true);
            npcDialogText.text = currentActiveQuest.info.InitialFirstAnswer;
            StartCoroutine(DelayToPlayerDialouge(3.0f));
        });

        OptionSecondBtnText.text = currentActiveQuest.info.InitialSecondQuestion;
        OptionSecondBtn.onClick.RemoveAllListeners();
        OptionSecondBtn.onClick.AddListener(() =>
        {
            PlayerDialouge.gameObject.SetActive(false);
            NpcDialouge.gameObject.SetActive(true);
            npcDialogText.text = currentActiveQuest.info.InitialSecondAnswer; //마지막 대화 표시

            StartCoroutine(DelayToPlayerDialouge(3.0f));
        });

        BackBtnText.text = currentActiveQuest.info.InitialBackQuestion;
        BackBtn.onClick.RemoveAllListeners();
        BackBtn.onClick.AddListener(() =>
        {
            if(!DataManager.Instance.nowPlayer.questGivers.Contains(currentActiveQuest.questGiver))
            {
                currentActiveQuest = Quests[ActiveQuestIndex];

                PlayerDialouge.gameObject.SetActive(false);
                NpcDialouge.gameObject.SetActive(true);

                DataManager.Instance.nowPlayer.isPromise = true;

                npcDialogText.text = currentActiveQuest.info.InitialFinishAnswer;

                QuestManager.Instance.AddActivePromiseSave(currentActiveQuest.questGiver, currentActiveQuest);

                StartCoroutine(delayTalkStop());
            }
           

        });

    }

    private void OpenChaosRiftInteraction()
    {
        ThirdBtn.gameObject.SetActive(false); //보상 버튼 비활성화

        npcGiverText.text = currentActiveQuest.questGiver; //퀘스트 제공자 동기화 부분

        OptionFirstBtnText.text = currentActiveQuest.info.OpenChaosRiftFirstQuestion;
        OptionFirstBtn.onClick.RemoveAllListeners();
        OptionFirstBtn.onClick.AddListener(() =>
        {
            PlayerDialouge.gameObject.SetActive(false);
            NpcDialouge.gameObject.SetActive(true);
            npcDialogText.text = currentActiveQuest.info.OpenChaosRiftFirstAnswer;
            StartCoroutine(DelayToPlayerDialouge(3.0f));
        });

        OptionSecondBtnText.text = currentActiveQuest.info.OpenChaosRiftSecondQuestion;
        OptionSecondBtn.onClick.RemoveAllListeners();
        OptionSecondBtn.onClick.AddListener(() =>
        {
            PlayerDialouge.gameObject.SetActive(false);
            NpcDialouge.gameObject.SetActive(true);
            npcDialogText.text = currentActiveQuest.info.OpenChaosRiftSecondAnswer; //마지막 대화 표시

            StartCoroutine(DelayToPlayerDialouge(3.0f));
        });

        BackBtnText.text = currentActiveQuest.info.InitialBackQuestion;
        BackBtn.onClick.RemoveAllListeners();
        BackBtn.onClick.AddListener(() =>
        {

            PlayerDialouge.gameObject.SetActive(false);
            NpcDialouge.gameObject.SetActive(true);

            npcDialogText.text = currentActiveQuest.info.OpenChaosRiftFinishAnswer;

    
            StartCoroutine(delayTalkStop());

        });
    }

    private void CloseChaosRiftInteraction()
    {
      

        npcGiverText.text = currentActiveQuest.questGiver; //퀘스트 제공자 동기화 부분

        OptionFirstBtnText.text = currentActiveQuest.info.CloseChaosRiftFirstQuestion;
        OptionFirstBtn.onClick.RemoveAllListeners();
        OptionFirstBtn.onClick.AddListener(() =>
        {
            PlayerDialouge.gameObject.SetActive(false);
            NpcDialouge.gameObject.SetActive(true);
            npcDialogText.text = currentActiveQuest.info.CloseChaosRiftFirstAnswer;
            StartCoroutine(DelayToPlayerDialouge(3.0f));
        });

        OptionSecondBtnText.text = currentActiveQuest.info.CloseChaosRiftSecondQuestion;
        OptionSecondBtn.onClick.RemoveAllListeners();
        OptionSecondBtn.onClick.AddListener(() =>
        {
            PlayerDialouge.gameObject.SetActive(false);
            NpcDialouge.gameObject.SetActive(true);
            npcDialogText.text = currentActiveQuest.info.CloseChaosRiftSecondAnswer; //마지막 대화 표시

            StartCoroutine(DelayToPlayerDialouge(3.0f));
        });

        BackBtnText.text = currentActiveQuest.info.InitialBackQuestion;
        BackBtn.onClick.RemoveAllListeners();
        BackBtn.onClick.AddListener(() =>
        {

            PlayerDialouge.gameObject.SetActive(false);
            NpcDialouge.gameObject.SetActive(true);

            DataManager.Instance.nowPlayer.isOpenChaosRift = false;
            DataManager.Instance.nowPlayer.isReCloseChaosRift = true;

            npcDialogText.text = currentActiveQuest.info.CloseChaosRiftFinishAnswer;


            StartCoroutine(delayTalkStop());

            //npc 옆에 문드러진 도장 생성
            ReceiveReward();
        });
    }

    private void ReCloseChaosRiftInteraction()
    {

        npcGiverText.text = currentActiveQuest.questGiver; //퀘스트 제공자 동기화 부분

        OptionFirstBtnText.text = currentActiveQuest.info.ReCloseChaosRiftFirstQuestion;
        OptionFirstBtn.onClick.RemoveAllListeners();
        OptionFirstBtn.onClick.AddListener(() =>
        {
            PlayerDialouge.gameObject.SetActive(false);
            NpcDialouge.gameObject.SetActive(true);
            npcDialogText.text = currentActiveQuest.info.ReCloseChaosRiftFirstAnswer;
            StartCoroutine(DelayToPlayerDialouge(3.0f));
        });

        OptionSecondBtnText.text = currentActiveQuest.info.ReCloseChaosRiftSecondQuestion;
        OptionSecondBtn.onClick.RemoveAllListeners();
        OptionSecondBtn.onClick.AddListener(() =>
        {
            PlayerDialouge.gameObject.SetActive(false);
            NpcDialouge.gameObject.SetActive(true);
            npcDialogText.text = currentActiveQuest.info.ReCloseChaosRiftSecondAnswer; //마지막 대화 표시

            StartCoroutine(DelayToPlayerDialouge(3.0f));
        });

        BackBtnText.text = currentActiveQuest.info.InitialBackQuestion;
        BackBtn.onClick.RemoveAllListeners();
        BackBtn.onClick.AddListener(() =>
        {

            PlayerDialouge.gameObject.SetActive(false);
            NpcDialouge.gameObject.SetActive(true);

            DataManager.Instance.nowPlayer.isOpenChaosRift = false;
            DataManager.Instance.nowPlayer.isReCloseChaosRift = true;

            npcDialogText.text = currentActiveQuest.info.ReCloseChaosRiftFinishAnswer;


            StartCoroutine(delayTalkStop());

            //npc 옆에 문드러진 도장 생성

        });
    }

    IEnumerator DelayToPlayerDialouge(float delay)
    {
        yield return new WaitForSeconds(delay);
        PlayerDialouge.gameObject.SetActive(true);
        NpcDialouge.gameObject.SetActive(false);
    }
    IEnumerator delayTalkStop()
    {
        yield return new WaitForSecondsRealtime(3.0f);
       
        TalkStop();
    }
   

    private bool AreQuestRequirmentsCompleted() //Npc가 요청한 요구사항들을 플레이어가 갖고 있는지 체크
    {


        currentActiveQuest = Quests[ActiveQuestIndex];

        string firstRequiredItem = currentActiveQuest.info.firstRequirment;

        int firstRequiredAmount = currentActiveQuest.info.firstRequirmentAmount;

        var firstItemCounter = 0; //현재 내가 들고있는 첫번째 요구한 아이템의 개수 0으로 잡고

        Debug.Log(playerInteraction.collectedItems);

        foreach (var item in playerInteraction.collectedItems)
        {
            Debug.Log($"인벤토리 아이템: {item.Key}, 요구 아이템: {firstRequiredItem}");

            if (item.Key == firstRequiredItem)
            {
                firstItemCounter += item.Value; //현재 내가 들고있는 첫번째 요구한 아이템의 개수를 1씩 증가

                Debug.Log(firstItemCounter);
            }
        }

        string secondRequiredItem = currentActiveQuest.info.secondRequirment;
        int secondRequirmentAmount = currentActiveQuest.info.secondRequirmentAmount;

        var secondItemCounter = 0;

        //@@@@@@@ NPC가 요구했던 것들이 더 많다면 여기 아래에 더 추가 할 수 있음@@@@@@@@@ 


        //현재 내가 들고있는 첫번째 또는 두번째 요구한 아이템의 개수가 >= 목표치보다 많을 경우
        if (firstItemCounter >= firstRequiredAmount && secondItemCounter >= secondRequirmentAmount)
        {
            Debug.Log("요구 충족");
            return true;
        }
        else
        {
            return false;
        }
    }

    private void SubmitRequiredItems() //NPC가 요구했던 아이템들을 플레이어가 제출하는 메서드
    {
        string firstRequiredItem = currentActiveQuest.info.firstRequirment;
        int firstRequiredAmount = currentActiveQuest.info.firstRequirmentAmount;

        if (firstRequiredItem != "")
        {
            player.GetComponent<PlayerInteraction>().RemoveItem(firstRequiredItem, firstRequiredAmount);
        }

        string secondRequiredItem = currentActiveQuest.info.secondRequirment;
        int secondRequirmentAmount = currentActiveQuest.info.secondRequirmentAmount;

        if (secondRequiredItem != "")
        {
            player.GetComponent<PlayerInteraction>().RemoveItem(secondRequiredItem, secondRequirmentAmount);
        }

        //@@@@@@@ NPC가 요구했던 것들이 더 많다면 여기 아래에 더 추가 할 수 있음@@@@@@@@@ 
    }

    public float offsetX = 1.0f; // NPC 오른쪽으로 이동할 거리

    private void ReceiveReward()
    {

     

        QuestManager.Instance.AddCompletedPromiseSave(currentActiveQuest);
       
        if (currentActiveQuest.info.rewardItem1 != null)
        {
          
            Debug.Log("문드러진 도장 생성");
            Vector3 npcPosition = transform.position;

            // 오른쪽 위치 계산
            Vector3 rightDirection = transform.right;

            // 오른쪽 방향으로 offset만큼 이동
            Vector3 spawnPosition = npcPosition + (rightDirection * offsetX);
            spawnPosition.y = 0.03f;

            Quaternion spawnRotation = Quaternion.Euler(0, -90, 0);

            GameObject DecayedStamp =  Instantiate(currentActiveQuest.info.rewardItem1, spawnPosition, spawnRotation);
        }

        //if (currentActiveQuest.info.rewardItem2 != "")
        //{
        //    player.GetComponent<PlayerInteraction>().CollectItem(currentActiveQuest.info.rewardItem2);
        //}
   

        //@@@@@@@ NPC가 보상해줄 것들이 더 많다면 여기 아래에 더 추가 할 수 있음@@@@@@@@@ 


    }
    public void TalkStop()
    {

        // transform.rotation = initRot;
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
