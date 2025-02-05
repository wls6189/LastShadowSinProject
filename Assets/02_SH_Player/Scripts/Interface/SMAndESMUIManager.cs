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
    [SerializeField] RectTransform eSMListContent;
    [SerializeField] MarkImageSO markImageSO;
    [SerializeField] GameObject sMAndESMSlotPrefab;
    [SerializeField] GameObject lockSMSlotPrefab;
    [SerializeField] RectTransform eSMInfoImagePos;
    [SerializeField] TextMeshProUGUI eSMInfoName;
    [SerializeField] TextMeshProUGUI eSMInfoGainDescription;
    [SerializeField] TextMeshProUGUI eSMInfoAbilityDescription;
    [SerializeField] TextMeshProUGUI selectESMInfoName;
    [SerializeField] TextMeshProUGUI selectESMInfoGainDescription;
    [SerializeField] TextMeshProUGUI selectESMInfoAbilityDescription;
    [SerializeField] TextMeshProUGUI spiritAshText;
    [SerializeField] RectTransform sMListContent;
    [SerializeField] RectTransform[] sMInfoImagePos = new RectTransform[4];
    [SerializeField] TextMeshProUGUI sMInfoMark;
    [SerializeField] TextMeshProUGUI sMInfoActivateMark;
    [SerializeField] TextMeshProUGUI sMInfoAttackPower;
    [SerializeField] TextMeshProUGUI sMInfoMaxHealth;
    [SerializeField] TextMeshProUGUI selectSMName;
    [SerializeField] TextMeshProUGUI selectSMAttackPower;
    [SerializeField] TextMeshProUGUI selectSMMaxHealth;
    [SerializeField] TextMeshProUGUI[] selectSMMarks = new TextMeshProUGUI[3];
    [SerializeField] TextMeshProUGUI[] selectSMMarkDescriptions = new TextMeshProUGUI[3];
    [SerializeField] Slider masterSoundSlider;

    [SerializeField] GameObject NotifySelectSwapSM;

    public RectTransform ESMSelectionMenuPos;
    public RectTransform SMSelectionMenuPos;
    public GameObject ESMBlockingPanel;
    public GameObject SMBlockingPanel;

    [HideInInspector] public bool PanelClicked;
    [HideInInspector] public bool IsSMSwap = false;
    [HideInInspector] public GameObject SMSwapGo;
    [HideInInspector] public GameObject SMSwapSelectionMenuGo;

    GameObject currentESMSlotGo;
    GameObject currentSMSlotGo;
    List<GameObject> ownedESMList = new();
    List<GameObject> ownedSMList = new();

    static SMAndESMUIManager instance;
    public static SMAndESMUIManager Instance {  get { return instance; } }
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        SetOwnedESMList();
        SetOwnedSMList();
    }
    void Update()
    {
        SetSpiritAsh();
        CurrentESMSlotInfo();
        CurrentSMSlotInfo();

        if (IsSMSwap)
        {
            NotifySelectSwapSM.SetActive(true);
        }
        else
        {
            NotifySelectSwapSM.SetActive(false);
        }
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
                GameObject go = Instantiate(sMAndESMSlotPrefab);
                go.transform.SetParent(eSMInfoImagePos);
                go.transform.position = eSMInfoImagePos.position;
                go.GetComponent<SMAndESMSlot>().EquipedState(true);

                foreach (ESMImage image in markImageSO.ESMImageList)
                {
                    if (eSM.Name == image.Name)
                    {
                        go.GetComponent<SMAndESMSlot>().SetESMSlotData(image.Image, eSM, player);
                    }
                }

                eSMInfoName.text = eSM.Name;
                eSMInfoGainDescription.text = eSM.GainDescription;
                eSMInfoAbilityDescription.text = eSM.AbilityDescription;
            }
            else
            {
                GameObject go = Instantiate(sMAndESMSlotPrefab);
                ownedESMList.Add(go);
                go.transform.SetParent(eSMListContent);
                go.GetComponent<SMAndESMSlot>().EquipedState(false);

                foreach (ESMImage image in markImageSO.ESMImageList)
                {
                    if (eSM.Name == image.Name)
                    {
                        go.GetComponent<SMAndESMSlot>().SetESMSlotData(image.Image, eSM, player);
                    }
                }
            }
        }
    }
    public void SetOwnedSMList()
    {
        if (player.PlayerMarkInventory.OwnedSpiritMark.Count <= 0) return;

        // 초기화 부분
        ownedSMList.Clear();

        for (int i = sMListContent.childCount - 1; i >= 0; i--)
        {
            Destroy(sMListContent.GetChild(i).gameObject);
        }
        foreach (var pos in sMInfoImagePos)
        {
            if (pos.childCount >= 0)
            {
                for (int i = pos.childCount - 1; i >= 0; i--)
                {
                    Destroy(pos.GetChild(i).gameObject);
                }
            }
        }

        sMInfoMark.text = "";
        sMInfoActivateMark.text = "";
        sMInfoAttackPower.text = "";
        sMInfoMaxHealth.text = "";

        // UI 채우는 단계
        float attackPowerAmount = 0;
        float maxHealthAmount = 0;
        for (int i = 0; i < 1 + DataManager.Instance.nowPlayer.DecayedStampCount; i++) // 장착 중인 영혼낙인 UI 채우기
        {
            if (player.PlayerMarkInventory.EquipedSpiritMark[i] != null)
            {
                GameObject go = Instantiate(sMAndESMSlotPrefab);
                go.transform.SetParent(sMInfoImagePos[i]);
                go.transform.position = sMInfoImagePos[i].position;
                go.GetComponent<SMAndESMSlot>().IsEquiped = true;

                foreach (SpiritMarkImage image in markImageSO.SpiritMarkImageList)
                {
                    if (player.PlayerMarkInventory.EquipedSpiritMark[i].Name == image.Name)
                    {
                        go.GetComponent<SMAndESMSlot>().SetSMSlotData(image.Image, player.PlayerMarkInventory.EquipedSpiritMark[i], player);
                        go.GetComponent<SMAndESMSlot>().ThisSMIndex = i;
                        break;
                    }
                }

                attackPowerAmount += player.PlayerMarkInventory.EquipedSpiritMark[i].AttackPower;
                maxHealthAmount += player.PlayerMarkInventory.EquipedSpiritMark[i].Health;
            }
        }
        sMInfoAttackPower.text = $"공격력 증가량 : {attackPowerAmount}";
        sMInfoMaxHealth.text = $"체력 증가량 : {maxHealthAmount}";

        foreach (var activateSM in player.PlayerMarkInventory.ActivatedMark.Keys)
        {
            sMInfoMark.text += $"{activateSM.Name}\n";

            if (player.PlayerMarkInventory.ActivatedMark[activateSM] >= 2)
            {
                sMInfoActivateMark.text += $"{activateSM.Name}\n";
            }
        }

        foreach (var ownedSM in player.PlayerMarkInventory.OwnedSpiritMark) // 보유중인 영혼낙인 UI 채우기
        {
            bool isContainInEquiped = false;

            for (int i = 0; i < 1 + DataManager.Instance.nowPlayer.DecayedStampCount; i++) // 장착 중인 영혼낙인 UI 채우기
            {
                if (ownedSM == player.PlayerMarkInventory.EquipedSpiritMark[i])
                {
                    isContainInEquiped = true;
                    break;
                }
            }
            if (isContainInEquiped) continue;

            GameObject go = Instantiate(sMAndESMSlotPrefab);
            ownedSMList.Add(go);
            go.transform.SetParent(sMListContent);
            go.GetComponent<SMAndESMSlot>().IsEquiped = false;

            foreach (SpiritMarkImage image in markImageSO.SpiritMarkImageList)
            {
                if (ownedSM.Name == image.Name)
                {
                    go.GetComponent<SMAndESMSlot>().SetSMSlotData(image.Image, ownedSM, player);
                    go.GetComponent<SMAndESMSlot>().ThisSMIndex = -1;
                    break;
                }
            }
        }

        for (int i = 3; i > DataManager.Instance.nowPlayer.DecayedStampCount; i--) // 슬롯 막기
        {
            GameObject go = Instantiate(lockSMSlotPrefab);
            go.transform.SetParent(sMInfoImagePos[i]);
            go.transform.position = sMInfoImagePos[i].position;
        }
    }
    public void ESMSlotPointerEnter(GameObject go, bool isESM)
    {
        if (isESM)
        {
            currentESMSlotGo = go;
        }
        else
        {
            currentSMSlotGo = go;
        }
    }
    public void ESMSlotPointerExit()
    {
        currentESMSlotGo = null;
        currentSMSlotGo = null;
    }
    public void ActivePanel(bool set)
    {
        if (set)
        {
            ESMBlockingPanel.SetActive(true);
            SMBlockingPanel.SetActive(true);
        }
        else
        {
            ESMBlockingPanel.SetActive(false);
            SMBlockingPanel.SetActive(false);
        }
    }
    void SetSpiritAsh()
    {
        spiritAshText.text = $"영혼재 : {player.PlayerStats.SpiritAsh.ToString()}";
    }
    void CurrentESMSlotInfo()
    {
        if (currentESMSlotGo != null)
        {
            selectESMInfoName.text = currentESMSlotGo.GetComponent<SMAndESMSlot>().ThisESM.Name;
            selectESMInfoGainDescription.text = currentESMSlotGo.GetComponent<SMAndESMSlot>().ThisESM.GainDescription;
            selectESMInfoAbilityDescription.text = currentESMSlotGo.GetComponent<SMAndESMSlot>().ThisESM.AbilityDescription;
        }
        else
        {
            selectESMInfoName.text = "";
            selectESMInfoGainDescription.text = "";
            selectESMInfoAbilityDescription.text = "";
        }
    }
    void CurrentSMSlotInfo()
    {
        if (currentSMSlotGo != null)
        {
            selectSMName.text = currentSMSlotGo.GetComponent<SMAndESMSlot>().ThisSM.Name;
            selectSMAttackPower.text = currentSMSlotGo.GetComponent<SMAndESMSlot>().ThisSM.AttackPower.ToString();
            selectSMMaxHealth.text = currentSMSlotGo.GetComponent<SMAndESMSlot>().ThisSM.Health.ToString();

            for (int i = 0; i < currentSMSlotGo.GetComponent<SMAndESMSlot>().ThisSM.Marks.Count; i++)
            {
                selectSMMarks[i].text = currentSMSlotGo.GetComponent<SMAndESMSlot>().ThisSM.Marks[i].Name;
                selectSMMarkDescriptions[i].text = currentSMSlotGo.GetComponent<SMAndESMSlot>().ThisSM.Marks[i].Description;
            }
        }
        else
        {
            selectSMName.text = "";
            selectSMAttackPower.text = "";
            selectSMMaxHealth.text = "";

            for (int i = 0; i < 3; i++)
            {
                selectSMMarks[i].text = "";
                selectSMMarkDescriptions[i].text = "";
            }
        }
    }
    public void SetMasterSound()
    {
        AudioManager.instance.SetMasterVolume(masterSoundSlider.value);
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