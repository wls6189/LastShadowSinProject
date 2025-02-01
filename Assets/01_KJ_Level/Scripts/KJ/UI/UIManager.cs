using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public static UIManager Instance
    {
        get
        {
            if (instance == null)
            {
                // 기존에 이미 존재하는 UIManager를 검색
                instance = FindObjectOfType<UIManager>();

                // 존재하지 않으면 새로 생성
                if (instance == null)
                {
                    GameObject go = new GameObject("UIManager");
                    instance = go.AddComponent<UIManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject); // 중복 생성 방지
        }
    }

    public bool IsGameMenuOpen;

    public Button[] TabButtons;

    private bool isChoose = false;

    EventSystem system;

    public Selectable firstInput;

    private Button lastSelectedButton; // 마지막으로 선택된 버튼

    public GameObject SoulFragMentInfo;


    public GameObject SoulFragMentMoveInfo;


    public GameObject SoulImages;

  

    public GameObject SoulFragMentContent;

    void Start()
    {

        system = EventSystem.current;

        if (TabButtons != null && TabButtons.Length > 0 )
        {
            TabButtons[0].Select(); // 첫 번째 버튼 선택
            lastSelectedButton = TabButtons[0];
            ColorChange(lastSelectedButton); // 초기 색상 설정
        }

    }

 

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            NavigateButtons(true); 
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            NavigateButtons(false); 
        }
    }
    private void NavigateButtons(bool isShiftPressed)
    {
        // 현재 선택된 Selectable 가져오기
        Selectable current = system.currentSelectedGameObject?.GetComponent<Selectable>();

        if (current == null && lastSelectedButton != null) //마우스로 빈 공간 선택시 current가 null이므로, null일 경우
        {
            lastSelectedButton.Select(); //마지막 버튼 선택
            current = lastSelectedButton; //마지막으로 버튼을 current에 저장.
        }

        if (current != null)
        {
            Selectable next = isShiftPressed ? current.FindSelectableOnLeft() : current.FindSelectableOnRight();  //true/false에 따른 왼쪽 이동과 오른쪽이동.

            if (next != null)
            {
                Debug.Log("2");
                next.Select();

                Button nextButton = next.GetComponent<Button>(); //선
                if (nextButton != null && nextButton != lastSelectedButton)
                {
                    Debug.Log("3");
                    nextButton.onClick.Invoke();
                    lastSelectedButton = nextButton; // 마지막 선택된 버튼 업데이트
                    ColorChange(lastSelectedButton);
                }
            }
        }
    }

    [SerializeField]
    GameObject GameMenu;
    public void GameMenuOpen()
    {
        GameMenu.gameObject.SetActive(true);
        if (DataManager.Instance.nowPlayer.allActiveQuests.Count > 0)
        {
            QuestManager.Instance.RefreshQuestList();
        }
        else if (DataManager.Instance.nowPlayer.allCompletedQuests.Count > 0)
        {
            QuestManager.Instance.RefreshQuestList();
        }

     

     
        IsGameMenuOpen = true;
        DialogSystem.Instance.MouseOn();
    }

    public void GameMenuClose()
    {
        DialogSystem.Instance.MouseOff();
        GameMenu.gameObject.SetActive(false);
        IsGameMenuOpen = false;
  
    }


    public void SoulImageOpen(SoulFragMent soulFragMent)
    {
        SoulImages.gameObject.SetActive(true);

        RefreshSaveScenes(soulFragMent);
    }

    [SerializeField]
    private bool isSoulFragMentUI;

    private string movedScene;

    public void RefreshSaveScenes(SoulFragMent soulFragMent)
    {
        DialogSystem.Instance.MouseOn();
        isSoulFragMentUI = true;


        // 기존 UI 정리
        foreach (Transform child in SoulFragMentContent.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < DataManager.Instance.nowPlayer.saveActiveObjects.Count; i++)
        {
            string currentScene = DataManager.Instance.nowPlayer.saveObjectSceneName[i]; // 동일한 인덱스의 ID 가져오기

            // SoulFragMentInfo를 인스턴스화
            GameObject soulFragMentObj = Instantiate(SoulFragMentInfo, Vector3.zero, Quaternion.identity);
            soulFragMentObj.transform.SetParent(SoulFragMentContent.transform, false);

            // SoulLow 컴포넌트 가져오기
            SoulLow sLow = soulFragMentObj.GetComponent<SoulLow>();

            if (sLow != null)
            {
                sLow.currentScene.text = currentScene; // 해당 인덱스의 saveObjectId를 설정

                sLow.SelectSceneButton.onClick.RemoveAllListeners();
                sLow.SelectSceneButton.onClick.AddListener(() =>
                {
                    //isSoulFragMentUI = false;
                    //SoulCanvas.gameObject.SetActive(false);
                    //SceneManager.LoadScene(currentScene);

                    SoulFragMentMoveInfo.gameObject.SetActive(true);
                    movedScene = currentScene;
                });

                
            }
            else
            {
                Debug.LogError("SoulLow component is missing on the instantiated SoulFragMent object!");
            }
        }
    
    }

    public void SelectSceneMove()
    {
  
        isSoulFragMentUI = false;
        SoulFragMentMoveInfo.gameObject.SetActive(false);
        SoulImages.gameObject.SetActive(false);
        SceneManager.LoadScene(movedScene);
    }

    public void CloseBtn()
    {
        DialogSystem.Instance.MouseOff();
        SoulImages.gameObject.SetActive(false);
    }
    public void ColorChange(Button clickedButton)
    {
 
        foreach(Button button in TabButtons)
        {
            Image buttonImage = button.GetComponent<Image>();

            if (button == clickedButton)
            {
                // 클릭된 버튼은 파란색
                buttonImage.color = Color.blue;
            }
            else
            {
                // 나머지 버튼은 검은색
                buttonImage.color = Color.white;
            }
        }
    }
}
