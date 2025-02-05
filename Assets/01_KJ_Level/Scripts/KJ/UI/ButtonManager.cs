using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonManager : MonoBehaviour
{
    public Button[] buttons;  
    public Sprite defaultSprite; 
    public Sprite hoverSprite;    
    public Sprite clickSprite;    

    void Start()
    {
        // 모든 버튼에 대해 이벤트 리스너를 설정
        foreach (Button button in buttons)
        {
            // 버튼의 Image 컴포넌트 가져오기
            Image buttonImage = button.GetComponent<Image>();

            // 버튼에 PointerEnter, PointerExit 이벤트 추가
            EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();

            // PointerEnter 이벤트 (마우스를 올렸을 때)
            EventTrigger.Entry entryEnter = new EventTrigger.Entry();
            entryEnter.eventID = EventTriggerType.PointerEnter;
            entryEnter.callback.AddListener((data) => { OnPointerEnter(buttonImage); });
            trigger.triggers.Add(entryEnter);

            // PointerExit 이벤트 (마우스를 버튼에서 뺐을 때)
            EventTrigger.Entry entryExit = new EventTrigger.Entry();
            entryExit.eventID = EventTriggerType.PointerExit;
            entryExit.callback.AddListener((data) => { OnPointerExit(buttonImage); });
            trigger.triggers.Add(entryExit);

            // PointerClick 이벤트 (버튼 클릭 시)
            EventTrigger.Entry entryClick = new EventTrigger.Entry();
            entryClick.eventID = EventTriggerType.PointerClick;
            entryClick.callback.AddListener((data) => { OnPointerClick(buttonImage); });
            trigger.triggers.Add(entryClick);
        }
    }

    
    private void OnPointerEnter(Image buttonImage)
    {
        buttonImage.sprite = hoverSprite;
    }

  
    private void OnPointerExit(Image buttonImage)
    {
        buttonImage.sprite = defaultSprite;
    }

    // 버튼 클릭 시
    private void OnPointerClick(Image buttonImage)
    {
        buttonImage.sprite = clickSprite;

        ResetButtonImage();
    }

    private void ResetButtonImage()
    {
        // 모든 버튼에 대해 기본 이미지로 리셋
        foreach (Button button in buttons)
        {
            Image buttonImage = button.GetComponent<Image>();
            buttonImage.sprite = defaultSprite;
        }
    }
}
