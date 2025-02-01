using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class PlayerInteraction : MonoBehaviour
{
    public readonly Dictionary<string, int> collectedItems = new Dictionary<string, int>(); // 아이템 이름과 개수를 관리

    private NPC currentNPC;

    private SoulFragMent soulfragMent;

    InputActionAsset inputActionAsset;
    InputAction interactAction;

    private void Awake()
    {
        inputActionAsset = GetComponent<PlayerInput>().actions;
        
    }
    private void Start()
    {
        interactAction = inputActionAsset.FindAction("Interact");
        transform.position = DataManager.Instance.nowPlayer.position;

 
    }

    private void Update()
    {
        if(interactAction.WasPressedThisFrame() && isInteractionReady)
        {
            isInteractionStart = true;

        }

        if(Input.GetKeyDown(KeyCode.I))
        {
            InventoryCheck();
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
            collectedItems[itemName] += 1; // 기존 아이템 개수 증가
        }
        else
        {
            collectedItems.Add(itemName, 1); // 새 아이템 추가

        }

    } //임시 메서드1

    void InventoryCheck()
    {

        foreach (var item in collectedItems)
        {
            Debug.Log($"아이템: {item.Key}, 개수: {item.Value}");
        }


        if (collectedItems.Count == 0)
        {
            Debug.Log("인벤토리가 비어 있습니다.");
        }
    } //임시 메서드2

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

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("FragMent") || other.CompareTag("NextPortal") || other.CompareTag("PreviousPortal")) //상호작용 확인.
        {
            isInteractionReady = true;

           
        }
        if (other.CompareTag("FragMent") && isInteractionStart) //헌신자 영혼파편
        {
            soulfragMent = other.GetComponent<SoulFragMent>();

            if (!soulfragMent.isSave) // 처음 상호작용 시 저장
            {
                SavePlayerData();
                soulfragMent.isSave = true; // 이미 저장된 상태로 플래그 설정
                DataManager.Instance.SaveSoulFragment(soulfragMent, soulfragMent.fragmentID,soulfragMent.sceneflow.currentSceneName);
                
                //DataManager.Instance.SaveObjectFunc(soulfragMent);
                
                other.GetComponentInChildren<TextMeshProUGUI>().text = "Saved! [F]"; // UI 텍스트 업데이트
                Debug.Log("영혼 파편이 저장.");
            }
            else // 이미 저장된 상태에서 다시 상호작용
            {
                UIManager.Instance.SoulImageOpen(soulfragMent);
                other.GetComponentInChildren<TextMeshProUGUI>().text = "Interaction [F]"; // UI 텍스트 업데이트
                Debug.Log("영혼 파편이 이미 저장.");
            }
            isInteractionStart = false;
        }

        if (other.CompareTag("NextPortal") && isInteractionStart)
        {
            isInteractionStart = false;
            other.GetComponent<Portal>().LoadScene(true);
        }
        if (other.CompareTag("PreviousPortal") && isInteractionStart)
        {
            isInteractionStart = false;
            other.GetComponent<Portal>().LoadScene(false);
        }
        if (other.CompareTag("NPC"))
        {
            isNpcInteraction = true;
            currentNPC = other.GetComponent<NPC>();
        }
    }

    private bool isInteractionReady;

    private bool isNpcInteraction;


    private bool isInteractionStart;
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("FragMent") || other.CompareTag("NextPortal") || other.CompareTag("PreviousPortal")) 
        {
            isInteractionReady = false;
        }

        if (other.CompareTag("FragMent"))
        {
            soulfragMent = null;
        }

        if (other.CompareTag("NPC"))
        {
            isNpcInteraction = false;
            currentNPC.NpcTalkImage.gameObject.SetActive(false);
            currentNPC = null;
        }
    }

    private void SavePlayerData()
    {
        Debug.Log("SAVE");
        // 플레이어 위치 및 현재 씬 저장
        DataManager.Instance.nowPlayer.position = transform.position;

        DataManager.Instance.nowPlayer.currentScene = SceneManager.GetActiveScene().name;

        //활성화 또는 완료된 약속들 저장


        //현재 슬롯 저장. -> Continue 버튼을 위함.


        // 데이터 저장
        DataManager.Instance.SaveData();


    }
}
