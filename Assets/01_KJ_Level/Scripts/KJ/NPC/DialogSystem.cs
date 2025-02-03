using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DialogSystem : MonoBehaviour
{

    private static DialogSystem instance;
    public static DialogSystem Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("DialogSystem"); //EventBus라는 빈 객체를 만들고
                instance = go.AddComponent<DialogSystem>(); //EventBus 빈 객체에 EventBus 스크립트(컴포넌트)을 추가
            }
            return instance;
        }



    }

    public bool isdialogueCanvas; //다이얼로그 UI가 열려있는지 여부

    [Header("# NPC UI")]
    public TextMeshProUGUI StoryDialogText;

    public TextMeshProUGUI NpcGiverText;

    public Button optBtn1;
    public Button optBtn2;
    public Button BackBtn;

    public Canvas DialougeCanvas; //다이얼로그 UI


    public Button FirstBtn;
    public Button SecondBtn;
    public Button ReceiveBtn;


    public GameObject NpcDialouge;
    public GameObject PlayerTalkDialouge;
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

    public void OpenDialogUI()
    {
        DialougeCanvas.gameObject.SetActive(true);
        isdialogueCanvas = true;

        MouseOn();
    }

    public void CloseDialogUI()
    {
        DialougeCanvas.gameObject.SetActive(false);
        isdialogueCanvas = false;

        MouseOff();
    }

    public void MouseOn()
    {
        Cursor.lockState = CursorLockMode.None; //마우스 보여주기
        Cursor.visible = true;
    }
    public void MouseOff()
    {
        Cursor.lockState = CursorLockMode.Locked; //마우스 보여주지 않기
        Cursor.visible = false;
    }
}
