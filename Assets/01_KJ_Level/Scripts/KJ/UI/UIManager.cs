using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    public bool IsSpiritShardOfTheDevotedMenuOpen;

    private string movedScene;

    public Button[] TabButtons;

    EventSystem system;

    public Selectable firstInput;

    private Button lastSelectedButton; // 마지막으로 선택된 버튼

    [SerializeField]
    GameObject GameMenu;

    [Header("SpiritShardOfTheDevoted")]
    public GameObject SoulFragMentInfo;
    public GameObject SoulFragMentMoveInfo;
    public GameObject SoulImages;
    public GameObject SoulFragMentContent;

    [Header("MapCursor")]
    [SerializeField]
    Image[] NoneCombatScenes;
    [SerializeField]
    Image[] CombatScenes;
    [SerializeField]
    public Image StartPlaySceneMapCursor;
    [SerializeField]
    public Image VillageMapCursor;
    [Header("OptionTab")]
    [SerializeField]
    Button MenuMainMenuYesBtn;
    [SerializeField]
    GameObject MainMenuMovePopup;
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

    [SerializeField]
    GameObject PlayerDieImage;
    [SerializeField]
    TextMeshProUGUI DieText;
    [SerializeField]
    public bool isPlayerDieUI;
    
    public IEnumerator PlayerDieUI(PlayerInteraction player)
    {
        isPlayerDieUI = true;
        PlayerDieImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        DieText.text = "3";
        yield return new WaitForSeconds(1f);
        DieText.text = "2";
        yield return new WaitForSeconds(1f);
        DieText.text = "1";
        yield return new WaitForSeconds(1f);
        DieText.text = " ";
        yield return new WaitForSeconds(0.1f);
        player.RespawnPlayer();
    }
    public void PlayerDieUISet()
    {
        isPlayerDieUI = false;
        PlayerDieImage.gameObject.SetActive(false);
        DieText.text = "YOU DiED";
    }
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



    public void MainMenuBtn()
    {

        MainMenuMovePopup.gameObject.SetActive(true);

        MenuMainMenuYesBtn.onClick.RemoveAllListeners();
        MenuMainMenuYesBtn.onClick.AddListener(() =>
        {
            MaineMenuMove();
        });
    }

    private void MaineMenuMove()
    {
        MainMenuMovePopup.gameObject.SetActive(false);
        GameMenu.gameObject.SetActive(false);
        RemoveSpecificDontDestroyObjects();

        SceneManager.LoadScene("MainMenu");
    }
    private void RemoveSpecificDontDestroyObjects()
    {
        string[] targetNames = { "MainCanvas", "DialogSystem", "InGameCanvas" };

        foreach (string name in targetNames)
        {
            GameObject obj = GameObject.Find(name);
            if (obj != null && obj.scene.buildIndex == -1) // DontDestroyOnLoad 상태인지 확인
            {
                Destroy(obj);
            }
        }
    }

    public void QuitButtonGame()
    {
        if(MainMenuMovePopup.activeSelf == false)
        {
            Application.Quit();
        }
       
    }

    public void SoulImageOpen(SpiritShardOfTheDevoted soulFragMent)
    {
        IsSpiritShardOfTheDevotedMenuOpen = true;

        SoulImages.gameObject.SetActive(true);

        RefreshSaveScenes(soulFragMent);
    }

    public void RefreshSaveScenes(SpiritShardOfTheDevoted soulFragMent)
    {
        DialogSystem.Instance.MouseOn();
     


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
  
        SoulFragMentMoveInfo.gameObject.SetActive(false);
        SoulImages.gameObject.SetActive(false);
        IsSpiritShardOfTheDevotedMenuOpen = false;
        SceneManager.LoadScene(movedScene);
    }

    [SerializeField]
    public GameObject CorpseCapeMap;
    [SerializeField]
    public GameObject WestHavenMap;
    [SerializeField]
    Image[] WestNoneCombatScenes;
    [SerializeField]
    Image[] WestCombatScenes;
    public void InteractRadiantTorchCCVillage() //중심부에서 
    {
        CorpseCapeMap.gameObject.SetActive(true); 

        StartPlaySceneMapCursor.gameObject.SetActive(false); 
        VillageMapCursor.gameObject.SetActive(true);

        for (int i = 0; i < NoneCombatScenes.Length; i++)
        {
            NoneCombatScenes[i].gameObject.SetActive(true);
        }
        for (int i = 0; i < CombatScenes.Length; i++)
        {
            CombatScenes[i].gameObject.SetActive(true);
        }
    }
    public void InteractRadiantTorchWestStart() //시작부분 
    {
        CorpseCapeMap.gameObject.SetActive(true);
        WestHavenMap.gameObject.SetActive(false);

        StartPlaySceneMapCursor.gameObject.SetActive(false); //시체곶 시작 부분 커서 끄기
        VillageMapCursor.gameObject.SetActive(false); //시체 곶 중심부 커서 끄기
        WestVillageMapCursor.gameObject.SetActive(false); //서쪽 마을 중심부 커서 끄기

        WestStartMapCursor.gameObject.SetActive(true);

    }
    public GameObject WestStartMapCursor;
    public GameObject WestVillageMapCursor;
    public void InteractRadiantTorchWestVillage() //중심부에서 
    {
        CorpseCapeMap.gameObject.SetActive(true);
        WestHavenMap.gameObject.SetActive(true);

        WestStartMapCursor.gameObject.SetActive(false);
        StartPlaySceneMapCursor.gameObject.SetActive(false);
        WestVillageMapCursor.gameObject.SetActive(true);

        for (int i = 0; i < WestNoneCombatScenes.Length; i++)
        {
            NoneCombatScenes[i].gameObject.SetActive(true);
        }
        for (int i = 0; i < WestCombatScenes.Length; i++)
        {
            WestCombatScenes[i].gameObject.SetActive(true);
        }
    }

    [SerializeField]
    GameObject currentSceneUI;

    [SerializeField]
    TextMeshProUGUI currentSceneName;
    public IEnumerator CurrentSceneUI(string sceneName)
    {
        currentSceneUI.gameObject.SetActive(true);
        currentSceneName.text = $"{sceneName}";

        AudioManager.instance.Playsfx(AudioManager.Sfx.NewLocation);

        yield return new WaitForSeconds(3.0f);
        CurrentSceneUISet();
    }
    private void CurrentSceneUISet()
    {
        currentSceneUI.gameObject.SetActive(false);
        currentSceneName.text = "";
    }
    public void CloseBtn()
    {
        DialogSystem.Instance.MouseOff();
        SoulImages.gameObject.SetActive(false);
        IsSpiritShardOfTheDevotedMenuOpen = false;
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
    public void ColorChange(Button clickedButton)
    {
 
        foreach(Button button in TabButtons)
        {
            Image buttonImage = button.GetComponent<Image>();

            if (button == clickedButton)
            {
                // 클릭된 버튼은 파란색
                buttonImage.color = Color.black;
            }
            else
            {
                // 나머지 버튼은 검은색
                buttonImage.color = Color.white;
            }
        }
    }
}
