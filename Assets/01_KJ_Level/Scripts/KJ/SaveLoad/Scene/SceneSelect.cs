using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSelect : MonoBehaviour 
{

    public GameObject PlayerNickNameImage; //비어있는 슬롯을 눌렀을 때 뜨는 창 
    public TextMeshProUGUI[] slotText; //슬롯의 텍스트
    public TextMeshProUGUI[] slotSceneText; //슬롯의 텍스트
    public TextMeshProUGUI newPlayerName;

    bool[] savefile = new bool[3]; //배열이기 때문에 초기화 해줘야 작동함.

    [SerializeField]
    GameObject slotsImage; //슬롯들의 UI
    [SerializeField]
    GameObject buttonsImage; //메인 메뉴 처음에 보이는 버튼들에 대한 이미지

    [SerializeField]
    GameObject slotDeletedImage; //슬롯 정말 삭제할건지에 대한 이미지
    [SerializeField]
    GameObject continueCheackImage;


    [SerializeField]
    Button SlotDeleteButtons;
    [SerializeField]
    Button ContinueCheackButton;

    [SerializeField]
    TextMeshProUGUI ContinueCheackText;

    [SerializeField]
    GameObject tempSlider;

    private void Start()
    {
        //슬롯별로 저장된 데이터가 존재하는지 판단.
        for(int i =0; i <3; i++) //슬롯이 현재 3까지 존재 하므로, 0,1,2 범위 사용
        {
            if (File.Exists(DataManager.Instance.path + $"{i}")) //즉, Save0~Save2 중에 파일이 존재한다면.ex) Save0 파일 존재
            {
                savefile[i] = true; //해당 Save0~Save2 중 존재한 파일을 true로. ex) Save0 파일을 true로 

                DataManager.Instance.nowSlot = i; //슬롯넘버 할당.  ex) Save0 파일의 넘버가 0이므로 0을 슬롯0번으로 할당.

                DataManager.Instance.LoadData();

                slotText[i].text = DataManager.Instance.nowPlayer.name; //해당 슬롯0번의 텍스트가 PlayerData의 name으로 저장
                slotSceneText[i].text = "현재 맵 : " + DataManager.Instance.nowPlayer.currentMap + "\n" +
                   "현재 구역 : " +  DataManager.Instance.nowPlayer.currentScene; //해당 슬롯0번의 텍스트가 PlayerData의 name으로 저장
               
            }
            else
            {
                slotText[i].text = "비어있음";
                DataManager.Instance.DataClear();
        
            }
        }
       
    }



    public void Slot(int number) //버튼 클릭 이벤트 호출
    {
        DataManager.Instance.SlotSave(number);

        if (slotDeletedImage.gameObject.activeSelf == false)
        {

            DataManager.Instance.nowSlot = number; //호출했을 때 매개변수로 받은 숫자가 슬롯의 번호가 되는 개념.


            if (savefile[number]) //1. 저장된 데이터가 있다면. 즉, 파일이 존재했다면 -> Start문 참고.
            {
                DataManager.Instance.LoadData(); //불러오기
               
                GoGame(); //
            }
            else //2.저장된 데이터가 없을 때
            {
                PlayerNickNameImageOpen();
            }
        }
          
   
    }
    public void GoGame() //인 게임으로 들어가는 버튼을 통해 함수 호출.
    {
        //저장된 데이터가 없을 때 -> 빈 슬롯이였다면
        if (!savefile[DataManager.Instance.nowSlot]) //Slot 메서드에서 DataManager.Instance.nowSlot을 업데이트 해줬기 때문에 사용 가능.
        {
            DataManager.Instance.nowPlayer.name = newPlayerName.text;

            DataManager.Instance.SaveData(); // 데이터 저장
            SceneManager.LoadScene(1);
        }

        else
        {
            Debug.Log("빈 슬롯 아님");
            string sceneToLoad = DataManager.Instance.nowPlayer.currentScene;

            SceneManager.LoadScene(sceneToLoad); // 씬 로드



        }


    }
    public void NewGameButton()
    {
        SlotsFullCheack();

        if (isSlotsFull == false) //슬롯이 꽉 차 있지 않을 때만 
        {
            SlotImageOpen();
        }
        else
        {
            continueCheackImage.gameObject.SetActive(true);

            ContinueCheackText.text = "모든 슬롯이 꽉 찼습니다. 슬롯을 지워 새로운 슬롯을 만들어야 합니다.";

            ContinueCheackButton.onClick.RemoveAllListeners();
            ContinueCheackButton.onClick.AddListener(() =>
            {
                continueCheackImage.gameObject.SetActive(false);
                SlotImageOpen();
                return;
            });
        }


    }


    public void ContinueButton()
    {

        SlotsEmptyCheak();

        continueCheackImage.gameObject.SetActive(true);

        if (isSlotsEmpty)
        {
           
            ContinueCheackText.text = "모든 슬롯들이 모두 비어있습니다. 새로운 슬롯을 만드시겠습니까?";

            ContinueCheackButton.onClick.RemoveAllListeners();
            ContinueCheackButton.onClick.AddListener(() =>
            {
                continueCheackImage.gameObject.SetActive(false);
                SlotImageOpen();
                return;
            });
        }
        else
        {
            if (PlayerPrefs.HasKey("PreviousSlot"))
            {
                
                int currentSlot = PlayerPrefs.GetInt("PreviousSlot");

                Debug.Log("데이터 이전 Slot " + currentSlot);

                ContinueCheackText.text = "최근 진행한 슬롯은 " + currentSlot + "번 입니다. 해당 슬롯으로 계속 진행하시겠습니까?";

                ContinueCheackButton.onClick.RemoveAllListeners();
                ContinueCheackButton.onClick.AddListener(() =>
                {
                    continueCheackImage.gameObject.SetActive(false);

                    if (continueCheackImage.activeSelf == false)
                    {
                        int previousSlot = PlayerPrefs.GetInt("PreviousSlot");  // 이전 슬롯 번호 가져오기
                        Debug.Log("데이터 이전 Slot " + previousSlot);
                        // 이전 슬롯 번호가 유효한 경우
                        if (previousSlot >= 0 && previousSlot < savefile.Length && savefile[previousSlot])
                        {
                            // 해당 슬롯에서 데이터 로드
                            DataManager.Instance.nowSlot = previousSlot;
                            DataManager.Instance.LoadData();
                            Debug.Log("Slot " + previousSlot + " data loaded.");

                            // 게임 시작
                            GoGame();
                        }
                        else
                        {
                            Debug.Log("이전 슬롯에 저장된 데이터가 없습니다.");
                          
                        }
                    }
                });
            }
            else
            {
                Debug.Log("키가 없습니다.");

                ContinueCheackText.text = "최근 진행한 슬롯이 없습니다. 새로운 슬롯을 만드시겠습니까?";
                ContinueCheackButton.onClick.RemoveAllListeners();
                ContinueCheackButton.onClick.AddListener(() =>
                {
                    continueCheackImage.gameObject.SetActive(false);
                    SlotImageOpen();
                    return;
                });
            }
          
        }

       

      
    }

    public void LoadButton()
    {
        SlotImageOpen();
    }

  

    public void SlotImageOpen()
    {
        if(continueCheackImage.activeSelf == false)
        {
            buttonsImage.gameObject.SetActive(false);
            slotsImage.gameObject.SetActive(true);
        }
     
    }
    public void SlotImageClose() //슬롯 UI - 닫기 버튼
    {
        if(slotDeletedImage.gameObject.activeSelf == false) //슬록 삭제 문구 이미지가 비활성화 일 때만 아래 ui 활성화
        {
            buttonsImage.gameObject.SetActive(true);
            slotsImage.gameObject.SetActive(false);
        }
    
    }
    public void PlayerNickNameImageOpen()
    {
        if (slotDeletedImage.gameObject.activeSelf == false)
        {
            PlayerNickNameImage.gameObject.SetActive(true);
        }
       
    }


    public void SlotDeleteButton(int number)
    {
        string filePath = DataManager.Instance.path + $"{number}";

        if (slotDeletedImage.gameObject.activeSelf == false && File.Exists(filePath))
        {
            slotDeletedImage.gameObject.SetActive(true);
            SlotDeleteButtons.onClick.RemoveAllListeners();
            SlotDeleteButtons.onClick.AddListener(() =>
            {
                slotDeletedImage.gameObject.SetActive(false);
                SlotDeleted(number);
            });
        }



    }
    private void SlotDeleted(int number)
    {
        DataManager.Instance.nowSlot = number;

        if (savefile[number] == true)
        {
            savefile[number] = false;

            string filePath = DataManager.Instance.path + $"{number}";
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Debug.Log($"Slot {number} data file deleted.");
            }
            else
            {
                Debug.LogWarning($"Slot {number} data file not found.");
            }

        }
        slotText[number].text = "비어있음";
        slotSceneText[number].text = "슬롯 " + number;

        // 슬롯을 비었으므로 데이터 클리어
        DataManager.Instance.DataClear();
        DataManager.Instance.DataCelarSlot();
    }

    [SerializeField]
    private bool isSlotsFull;
    private void SlotsFullCheack()
    {
        isSlotsFull = false;
        for (int i = 0; i < savefile.Length; i++)
        {
            if (savefile[i] == false) //세이브파일에 하나라도 비어있을 경우
            {
                return;
            }
          
        }
        isSlotsFull = true;
    }

    [SerializeField]
    public bool isSlotsEmpty;
    private void SlotsEmptyCheak()
    {
        isSlotsEmpty = true;

        for (int i = 0; i < savefile.Length; i++)
        {
            if (savefile[i] == true) // 하나라도 채워져 있는 경우
            {
                isSlotsEmpty = false;  // 세이브파일에 하나라도 채워져 있으면 비어있지 않음
                return;  // 더 이상 체크할 필요 없으므로 종료
            }

        }

    }

  
    public void QuitButton()
    {
        if(continueCheackImage.activeSelf == false)
        {
            Application.Quit();
        }
    }

    public void OptionButton()
    {
        tempSlider.SetActive(true);
    }
    public void OffOption()
    {
        tempSlider.SetActive(false);
    }

}
