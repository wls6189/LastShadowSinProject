using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static MarkImageSO;

public class SMAndESMUIManager : MonoBehaviour
{
    PlayerController player;
    [SerializeField] GameObject ESMTab;
    [SerializeField] RectTransform eSMListContent;
    [SerializeField] MarkImageSO markImageSO;
    [SerializeField] GameObject eSMSlotPrefab;
    [SerializeField] RectTransform eSMInfoImagePos;
    [SerializeField] TextMeshProUGUI eSMInfoName;
    [SerializeField] TextMeshProUGUI eSMInfoGainDescription;
    [SerializeField] TextMeshProUGUI eSMInfoAbilityDescription;
    [SerializeField] TextMeshProUGUI selectESMInfoName;
    [SerializeField] TextMeshProUGUI selectESMInfoGainDescription;
    [SerializeField] TextMeshProUGUI selectESMInfoAbilityDescription;
    [SerializeField] TextMeshProUGUI spiritAshText;

    public RectTransform SelectionMenuPos;
    public GameObject Panel;
    [HideInInspector] public bool PanelClicked;

    int currentESMSlotIndex = -1;
    List<GameObject> ownedESMList = new();

    static SMAndESMUIManager instance;
    public static SMAndESMUIManager Instance {  get { return instance; } }
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        SetOwnedESMList();
    }
    void Update()
    {
        SetSpiritAsh();
        CurrentESMSlotInfo();
    }
    public void SetPlayerController(PlayerController player)
    {
        this.player = player;
    }
    public void SetOwnedESMList()
    {
        if (player.PlayerESMInventory.OwnedESM.Count <= 0) return;

        // 초기화 단계
        ownedESMList.Clear();
        for (int i = eSMListContent.childCount - 1; i >= 0; i--)
        {
            Destroy(eSMListContent.GetChild(i).gameObject);
        }
        if (eSMInfoImagePos.childCount >= 0)
        {
            for (int i = eSMInfoImagePos.childCount - 1; i >= 0; i--)
            {
                Destroy(eSMInfoImagePos.GetChild(i).gameObject);
            }
        }
        if (player.PlayerESMInventory.EquipedESM == null)
        {
            eSMInfoName.text = "";
            eSMInfoGainDescription.text = "";
            eSMInfoAbilityDescription.text = "";
        }

        // UI 채우는 단계
        foreach (var eSM in player.PlayerESMInventory.OwnedESM)
        {
            if (player.PlayerESMInventory.EquipedESM != null && player.PlayerESMInventory.EquipedESM.Equals(eSM))
            {
                GameObject go = Instantiate(eSMSlotPrefab);
                go.transform.SetParent(eSMInfoImagePos);
                go.transform.position = eSMInfoImagePos.position;
                go.GetComponent<ESMSlot>().EquipedState(true);

                foreach (ESMImage image in markImageSO.ESMImageList)
                {
                    if (eSM.Name == image.Name)
                    {
                        go.GetComponent<ESMSlot>().SetESMSlotData(image.Image, eSM, player);
                    }
                }

                eSMInfoName.text = eSM.Name;
                eSMInfoGainDescription.text = eSM.GainDescription;
                eSMInfoAbilityDescription.text = eSM.AbilityDescription;
            }
            else
            {
                GameObject go = Instantiate(eSMSlotPrefab);
                ownedESMList.Add(go);
                go.transform.SetParent(eSMListContent);
                go.GetComponent<ESMSlot>().EquipedState(false);

                foreach (ESMImage image in markImageSO.ESMImageList)
                {
                    if (eSM.Name == image.Name)
                    {
                        go.GetComponent<ESMSlot>().SetESMSlotData(image.Image, eSM, player);
                    }
                }
            }
        }
    }
    public void ESMSlotPointerEnter(GameObject go)
    {
        currentESMSlotIndex = ownedESMList.IndexOf(go);
    }
    public void ESMSlotPointerExit()
    {
        currentESMSlotIndex = -1;
    }
    public void ActivePanel(bool set)
    {
        if (set)
        {
            Panel.SetActive(true);
        }
        else
        {
            Panel.SetActive(false);
        }
    }
    void SetSpiritAsh()
    {
        spiritAshText.text = $"영혼재 : {player.PlayerStats.SpiritAsh.ToString()}";
    }
    void CurrentESMSlotInfo()
    {
        if (currentESMSlotIndex >= 0)
        {
            selectESMInfoName.text = ownedESMList[currentESMSlotIndex].GetComponent<ESMSlot>().ThisESM.Name;
            selectESMInfoGainDescription.text = ownedESMList[currentESMSlotIndex].GetComponent<ESMSlot>().ThisESM.GainDescription;
            selectESMInfoAbilityDescription.text = ownedESMList[currentESMSlotIndex].GetComponent<ESMSlot>().ThisESM.AbilityDescription;
        }
        else
        {
            selectESMInfoName.text = "";
            selectESMInfoGainDescription.text = "";
            selectESMInfoAbilityDescription.text = "";
        }
    }
}











//LEGACY

//public void OnPointerDown(PointerEventData eventData)
//{
//    if (eventData.pointerCurrentRaycast.gameObject == Panel)
//    {
//        PanelClicked = true;
//    }
//}

//public void OnPointerUp(PointerEventData eventData)
//{
//    if (eventData.pointerCurrentRaycast.gameObject == Panel)
//    {
//        PanelClicked = false;
//    }
//}