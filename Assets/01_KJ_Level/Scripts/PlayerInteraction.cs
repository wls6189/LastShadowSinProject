using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerInteraction : MonoBehaviour
{
    public readonly Dictionary<string, int> collectedItems = new Dictionary<string, int>(); // 아이템 이름과 개수를 관리

    private NPC currentNPC;

    private SpiritShardOfTheDevoted soulfragMent;
    private SpiritSpring soulWell;
    private DecayedStamp crackedSeal;
    private RadiantTorch radiantTorch;
    private ChaosRift chaosRift;
    private DroppedItem droppedItem;
    //private MonsterSpawner monsterSpawner; 임시용


    InputActionAsset inputActionAsset;
    InputAction interactAction;

    private void Awake()
    {
        inputActionAsset = GetComponent<PlayerInput>().actions;
        
    }
    private void Start()
    {
        interactAction = inputActionAsset.FindAction("Interact");

        //if(DataManager.Instance.nowPlayer.saveActiveObjects.Contains(soulfragMent))
        //{

        //}
        transform.position = DataManager.Instance.nowPlayer.Initposition;

        GetComponent<PlayerStats>().CurrentHealth = DataManager.Instance.nowPlayer.CurrentHealth;
        GetComponent<PlayerStats>().CurrentSpiritWave = DataManager.Instance.nowPlayer.CurrentSpiritWave;
        GetComponent<PlayerStats>().CurrentSpiritMarkForce = DataManager.Instance.nowPlayer.CurrentSpiritMarkForce;

        //monsterSpawner = GameObject.Find("MonsterSpawner").GetComponent<MonsterSpawner>();
    }

    private void Update()
    {

        if (interactAction.WasPressedThisFrame() && isInteractionReady)
        {
            isInteractionStart = true;

        }

        if (currentNPC != null && currentNPC.playerInRange)
        {
            currentNPC.NpcTalkImage.gameObject.SetActive(true);

            if (interactAction.WasPressedThisFrame() && isNpcInteraction)
            {
                isNpcInteraction = false;
                currentNPC.PlayerWithTalk();
                currentNPC.PlayerLookNpc(transform.position);
            }
               
           
        }
     



    }

    public void CollectItem(string itemName)
    {

        if (collectedItems.ContainsKey(itemName))
        {
            DataManager.Instance.collectedItems[itemName] += 1; // 기존 아이템 개수 증가
        }
        else
        {
            DataManager.Instance.collectedItems.Add(itemName, 1); // 새 아이템 추가

        }

    } //임시 메서드1


    public void RemoveItem(string itemName,int count) ////임시 메서드3
    {
        if (collectedItems.ContainsKey(itemName)) //인벤토리에 itemName이라는 아이템이 있을 경우. 아이템 이름으로 판별.
        {
            collectedItems[itemName] -= count;
            Debug.Log($"아이템 '{itemName}'의 개수가 {count} 만큼 감소. 남은 개수: {collectedItems[itemName]}");

            if (collectedItems[itemName] <= 0)
            {
                collectedItems.Remove(itemName); // 개수가 0 이하가 되면 딕셔너리에서 삭제
                Debug.Log($"아이템 '{itemName}'이 인벤토리에서 없어짐.");
            }
        }
    }

  
    public void PlayerDie()
    {
        if (UIManager.Instance.isPlayerDieUI == false)
        {
            StartCoroutine(UIManager.Instance.PlayerDieUI(this));
        }      
    }

    public void RespawnPlayer()
    {
        UIManager.Instance.PlayerDieUISet();
        SceneManager.LoadScene(DataManager.Instance.nowPlayer.currentScene);
        transform.position = DataManager.Instance.nowPlayer.position;

    }

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("FragMent") )
        {
            isInteractionReady = true;
        }
        if (other.CompareTag("SoulWell"))
        {
            isInteractionReady = true;
            if (!DataManager.Instance.nowPlayer.currentSceneSpiritSpring.Contains
            (other.GetComponent< SpiritSpring>().currentSceneSpiritSpring))
            {
                other.GetComponentInChildren<TextMeshProUGUI>().text = "Interaction [F]";
            }
                
        }
        if(other.CompareTag("CrackedSeal"))
        {
            isInteractionReady = true;
            if (!DataManager.Instance.nowPlayer.currentScenesDecayedStamp.Contains
                 (other.GetComponent<DecayedStamp>().currentScenesDecayedStamp))
            {
                other.GetComponentInChildren<TextMeshProUGUI>().text = "Interaction [F]";
            }
        }

        if ( other.CompareTag("NextPortal") || other.CompareTag("PreviousPortal")
            || other.CompareTag("ChaosRift") || other.CompareTag("DroppedItem") || other.CompareTag("RadiantTorch")) //상호작용 확인.
        {
            isInteractionReady = true;
            other.GetComponentInChildren<TextMeshProUGUI>().text = "Interaction [F]"; // UI 텍스트 업데이트                
        }

        if (other.CompareTag("FragMent") && isInteractionStart) //헌신자 영혼파편
        {
            soulfragMent = other.GetComponent<SpiritShardOfTheDevoted>();

            if (!soulfragMent.isSave) // 처음 상호작용 시 저장
            {
                GetComponent<PlayerStats>().CurrentHealth = DataManager.Instance.nowPlayer.InitMaxHealth;

                SpriritShardOfTheDevotedSave();

                soulfragMent.isSave = true; // 이미 저장된 상태로 플래그 설정
                
                other.GetComponentInChildren<TextMeshProUGUI>().text = "Interaction [F]"; // UI 텍스트 업데이트
                Debug.Log("영혼 파편이 저장.");
            }
            else // 이미 저장된 상태에서 다시 상호작용
            {
                UIManager.Instance.SoulImageOpen(soulfragMent);
            }
            isInteractionStart = false;
        } //헌신자 영혼파편

        if (other.CompareTag("NextPortal") && isInteractionStart)
        {
            isInteractionStart = false;
            other.GetComponent<Portal>().LoadScene(true, GetComponent<PlayerStats>());
        } //다음 포탈
        if (other.CompareTag("PreviousPortal") && isInteractionStart)
        {
            isInteractionStart = false;
            other.GetComponent<Portal>().LoadScene(false, GetComponent<PlayerStats>());
        } //이전 포탈
        if(other.CompareTag("SoulWell") && isInteractionStart)
        {
            isInteractionStart = false;

           soulWell = other.GetComponent<SpiritSpring>();
            other.GetComponentInChildren<TextMeshProUGUI>().text = " ";
            soulWell.InteractionPlayer();

            GetComponent<PlayerController>().PlayerChaliceOfAtonement.LoadCOAData();

        } //영혼샘
        if(other.CompareTag("CrackedSeal") && isInteractionStart)
        {
            isInteractionStart = false;

            crackedSeal = other.GetComponent<DecayedStamp>();
            other.GetComponentInChildren<TextMeshProUGUI>().text = " ";
            crackedSeal.InteractionPlayer();

            GetComponent<PlayerController>().PlayerMarkInventory.LoadSMData();
        } //문드러진 도장

        if(other.CompareTag("ChaosRift") && isInteractionStart)
        {
            isInteractionStart = false;

            Debug.Log("ChaosRift 상호작용");
            CollectItem(GetCleanName(other.gameObject.name));

            chaosRift = other.GetComponent<ChaosRift>();
            chaosRift.InteractionPlayer();
        } //혼돈의틈새

        if (other.CompareTag("NPC"))
        {
            isNpcInteraction = true;
            currentNPC = other.GetComponent<NPC>();
        }
         
        if (other.CompareTag("DroppedItem") && isInteractionStart)
        {
            isInteractionStart = false;
            droppedItem = other.GetComponent<DroppedItem>();
            droppedItem.PickUpItem(this.GetComponent<PlayerController>());
        } //드랍 아이템
        if (other.CompareTag("RadiantTorch") && isInteractionStart) //빛나는 횟불
        {
            isInteractionStart = false;
            radiantTorch = other.GetComponent<RadiantTorch>();
            radiantTorch.InteractionPlayer();
        }
    }
    [SerializeField]
    private bool isInteractionReady;

    private bool isNpcInteraction;

    [SerializeField]
    private bool isInteractionStart;



    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("FragMent"))
        {
            soulfragMent = null;
        }
        if (other.CompareTag("DroppedItem"))
        {
            droppedItem = null;
        }
        if (other.CompareTag("RadiantTorch"))
        {
            radiantTorch = null;
        }
        if (other.CompareTag("NextPortal")|| other.CompareTag("PreviousPortal") 
            || other.CompareTag("SoulWell") || other.CompareTag("CrackedSeal") 
            || other.CompareTag("ChaosRift") || other.CompareTag("DroppedItem")
            || other.CompareTag("RadiantTorch")) //상호작용 확인.
        {
            isInteractionReady = false;
            other.GetComponentInChildren<TextMeshProUGUI>().text = ""; // UI 텍스트 업데이트

        }

        if (other.CompareTag("NPC"))
        {
            isNpcInteraction = false;
            currentNPC.NpcTalkImage.gameObject.SetActive(false);
        }

       
    }


    //LoadStatDataWhenQuit() 글씨 띄우고 딜레이 5초 준다음에 부활 위치. 
    public void LoadStatDataWhenQuit()
    {
        //헌신자의 영혼파편과 상호작용하여 세이브하고 나서 다시 로드 했을 때 세이브 했을 때의  CurrentHealth, MaxSpiritWave 저장
        //GetComponent<PlayerStats>().CurrentHealth = DataManager.Instance.nowPlayer.MaxHealth;

        //GetComponent<PlayerStats>().CurrentSpiritWave = DataManager.Instance.nowPlayer.MaxSpiritWave;

        //GetComponent<PlayerStats>().CurrentSpiritMarkForce = DataManager.Instance.nowPlayer.MaxSpiritMarkForce;
    }
    private void SpriritShardOfTheDevotedSave()
    {
        if (DataManager.Instance.nowPlayer.currentScene == "StartPlayScene" ||
         DataManager.Instance.nowPlayer.currentScene == "LastVillage" ||
         DataManager.Instance.nowPlayer.currentScene == "Village")
        {
            DataManager.Instance.nowPlayer.currentMap = "시체 곶";
        }

        if (DataManager.Instance.nowPlayer.currentScene == "DeapSouthernVillage" ||
            DataManager.Instance.nowPlayer.currentScene == "SouthernVillage")
        {
            DataManager.Instance.nowPlayer.currentMap = "서쪽 안식처";
        }

     

        //영혼파편 세이브 관련
        DataManager.Instance.SaveSoulFragment(soulfragMent, soulfragMent.fragmentID, soulfragMent.sceneflow.currentSceneName);

        // 플레이어 위치 및 현재 씬 저장
        DataManager.Instance.nowPlayer.position = transform.position; //위치. 
        DataManager.Instance.nowPlayer.currentScene = SceneManager.GetActiveScene().name; //현재씬 
        //플레이어 체력 , 영혼파동 세기 , MaxSpiritMarkForce  저장
        DataManager.Instance.nowPlayer.MaxHealth = GetComponent<PlayerStats>().CurrentHealth;
        DataManager.Instance.nowPlayer.MaxSpiritWave = GetComponent<PlayerStats>().CurrentSpiritWave;
        DataManager.Instance.nowPlayer.MaxSpiritMarkForce = GetComponent<PlayerStats>().CurrentSpiritMarkForce;

        DataManager.Instance.nowPlayer.EquipedESM = GetComponent<PlayerController>().PlayerESMInventory.EquipedESM; // 장착중인 영원의 영혼낙인

        DataManager.Instance.nowPlayer.OwnedESM = GetComponent<PlayerController>().PlayerESMInventory.OwnedESM; // 보유 중인 영원의 영혼낙인
        DataManager.Instance.nowPlayer.CurrentHealth = GetComponent<PlayerController>().PlayerStats.CurrentHealth; // 현재 체력
        DataManager.Instance.nowPlayer.CurrentSpiritWave = GetComponent<PlayerController>().PlayerStats.CurrentSpiritWave; // 현재 영혼의 파동
        DataManager.Instance.nowPlayer.CurrentSpiritMarkForce = GetComponent<PlayerController>().PlayerStats.CurrentSpiritMarkForce; // 현재 영혼낙인력
        DataManager.Instance.nowPlayer.SpiritAshAmount = GetComponent<PlayerController>().PlayerStats.SpiritAsh; // 영혼재
                                                                                                                 //마지막으로 저장된 것들을 json으로 저장
        DataManager.Instance.nowPlayer.EquipedSpiritMark = GetComponent<PlayerController>().PlayerMarkInventory.EquipedSpiritMark; // 장착중인 영원의 영혼낙인
        DataManager.Instance.nowPlayer.OwnedSpiritMark = GetComponent<PlayerController>().PlayerMarkInventory.OwnedSpiritMark; // 보유 중인 영원의 영혼낙인

        GetComponent<PlayerController>().PlayerStats.CurrentHealth = GetComponent<PlayerController>().PlayerStats.MaxHealth;
        DataManager.Instance.SaveData();
    }


    private string GetCleanName(string originalName)
    {
        // \s* -> 공백을 포함한 0개 이상의 공백 문자 제거
        //\(.*\) -> 괄호 안에 있는 모든 문자 제거.

        return Regex.Replace(originalName, @"\s*\(.*\)", "");
    }
}
