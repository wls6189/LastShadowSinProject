using System;
using UnityEngine;
using UnityEngine.EventSystems;
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
                GameObject go = new GameObject("UIManager"); //EventBus라는 빈 객체를 만들고
                instance = go.AddComponent<UIManager>(); //EventBus 빈 객체에 EventBus 스크립트(컴포넌트)을 추가
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
        else
        {
            Destroy(instance.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }
    public bool IsGameMenuOpen;

    public Button[] TabButtons;
    private bool isChoose = false;

    EventSystem system;
    public Selectable firstInput;
    private Button lastSelectedButton; // 마지막으로 선택된 버튼
    void Start()
    {
        system = EventSystem.current;

        if (TabButtons != null && TabButtons.Length > 0)
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
            Debug.Log("E");
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
                }
            }
        }
    }

    [SerializeField]
    GameObject GameMenu;
    public void GameMenuOpen()
    {
        GameMenu.gameObject.SetActive(true);
        IsGameMenuOpen = true;
        DialogSystem.Instance.MouseMoveStop();
    }

    public void GameMenuClose()
    {
        GameMenu.gameObject.SetActive(false);
        IsGameMenuOpen = false;
        DialogSystem.Instance.MouseMoveStart(); // 메뉴를 열 때 마우스 이동 정지
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
                buttonImage.color = Color.black;
            }
        }
    }
}
