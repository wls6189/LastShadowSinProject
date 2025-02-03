using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class SelectionMenu : MonoBehaviour
{
    PlayerController player;
    EternalSpiritMark eSM;

    GameObject equipBtn;
    GameObject unequipBtn;
    GameObject cancelBtn;

    Color eSMFrameSelectColor = new Color(1f, 1f, 1f, 1f);
    Color eSMFrameOriginColor = new Color(0.6f, 0.6f, 0.6f, 1f);

    void Awake()
    {
        equipBtn = transform.GetChild(0).gameObject; // 자식 오브젝트의 순서 중요!!!!!!!!!!!!!!!!!!!
        unequipBtn = transform.GetChild(1).gameObject;
        cancelBtn = transform.GetChild(2).gameObject;
    }
    private void Update()
    {
        //if (SMAndESMUIManager.Instance.PanelClicked)
        //{
        //    DestroyUI();
        //}
    }
    public void SetBtnIsEquiped(bool isEquiped, PlayerController player, EternalSpiritMark eSM)
    {
        if (isEquiped)
        {
            equipBtn.SetActive(false);
        }
        else
        {
            unequipBtn.SetActive(false);
        }

        this.player = player;
        this.eSM = eSM;
    }

    public void PointerEnter(GameObject go)
    {
        go.GetComponent<Image>().color = eSMFrameSelectColor;
    }
    public void PointerExit(GameObject go)
    {
        go.GetComponent<Image>().color = eSMFrameOriginColor;
    }
    public void PointerClick(GameObject go)
    {
        if (go.name == "Equip")
        {
            player.PlayerESMInventory.SwapESM(eSM);
            DestroyUI();
        }
        else if (go.name == "Unequip")
        {
            player.PlayerESMInventory.EquipedESM = null;
            SMAndESMUIManager.Instance.SetOwnedESMList();
            DestroyUI();
        }
        else if (go.name == "Cancel")
        {
            DestroyUI();
        }
        else
        {
            DestroyUI();
        }
    }
    public void DestroyUI()
    {
        Destroy(gameObject);
        SMAndESMUIManager.Instance.ActivePanel(false);
    }
}
