using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static MarkImageSO;

public class InGameUIManager : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] PlayerStats playerStats;
    [SerializeField] MarkImageSO markImageSO;

    [SerializeField] Image healthBar;
    [SerializeField] Image healthBarBase;
    [SerializeField] Image healthBarFrame;
    [SerializeField] Image spiritMarkForceBar;
    [SerializeField] GameObject spiritWavePrefab;
    [SerializeField] Transform spiritWavePos;
    [SerializeField] Image durationBar;
    [SerializeField] TextMeshProUGUI chaliceOfAtonementCount;
    [SerializeField] Image eSMImage;

    GameObject[] spiritWaveArray = new GameObject[10];

    Color spiritWaveVividColor = new Color(1f, 0.9f, 0.4f, 1f);
    Color spiritWaveFaintColor = new Color(1f, 0.9f, 0.4f, 0.4f);
    Color spiritMarkForceVividColor = new Color(0.3f, 0.45f, 0.9f, 1f);
    Color spiritMarkForceFaintColor = new Color(0.3f, 0.45f, 0.9f, 0.3f);

    float originHealthBarWidth = 400;
    float originHealthFrameWidth = 410;
    float originMaxHealthRatio = 100;

    void Awake()
    {
        InitializeSpiritWave();
        playerStats = player.PlayerStats;
    }

    void Update()
    {
        ManageBarWidth();
        ManageBar();
        ManagechaliceOfAtonementCount();
        ManageESMImage();
    }

    void InitializeSpiritWave()
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject go = Instantiate(spiritWavePrefab, spiritWavePos);
            go.transform.localPosition += new Vector3(42.5f, 0, 0) * i;

            spiritWaveArray[i] = go;
        }
    }
    void ManageBarWidth()
    {
        healthBar.GetComponent<RectTransform>().sizeDelta = new Vector2(originHealthBarWidth * (playerStats.MaxHealth / originMaxHealthRatio), healthBar.GetComponent<RectTransform>().sizeDelta.y);
        healthBarBase.GetComponent<RectTransform>().sizeDelta = new Vector2(originHealthBarWidth * (playerStats.MaxHealth / originMaxHealthRatio), healthBarBase.GetComponent<RectTransform>().sizeDelta.y);
        healthBarFrame.GetComponent<RectTransform>().sizeDelta = new Vector2(originHealthFrameWidth * (playerStats.MaxHealth / originMaxHealthRatio), healthBarFrame.GetComponent<RectTransform>().sizeDelta.y);
    }
    void ManageBar()
    {
        healthBar.fillAmount = playerStats.CurrentHealth / playerStats.MaxHealth;
        spiritMarkForceBar.fillAmount = playerStats.CurrentSpiritMarkForce / playerStats.MaxSpiritMarkForce;
        durationBar.fillAmount = player.PlayerESMInventory.EquipedESM.RemainDuration / player.PlayerESMInventory.EquipedESM.Duration;

        if (spiritMarkForceBar.fillAmount < 1)
        {
            spiritMarkForceBar.color = spiritMarkForceFaintColor;
        }
        else
        {
            spiritMarkForceBar.color = spiritMarkForceVividColor;
        }
        for (int i = 0; i < 10; i++)
        {
            if ((playerStats.CurrentSpiritWave - i) > 0 && (playerStats.CurrentSpiritWave - i) < 1)
            {
                spiritWaveArray[i].GetComponentsInChildren<Image>()[1].fillAmount = (playerStats.CurrentSpiritWave - i) / 1;
                spiritWaveArray[i].GetComponentsInChildren<Image>()[1].color = spiritWaveFaintColor;
            }
            else if ((playerStats.CurrentSpiritWave - i) >= 1)
            {
                spiritWaveArray[i].GetComponentsInChildren<Image>()[1].fillAmount = 1;
                spiritWaveArray[i].GetComponentsInChildren<Image>()[1].color = spiritWaveVividColor;
            }
            else
            {
                spiritWaveArray[i].GetComponentsInChildren<Image>()[1].fillAmount = 0;
                spiritWaveArray[i].GetComponentsInChildren<Image>()[1].color = spiritWaveFaintColor;
            }
        }
    }
    void ManagechaliceOfAtonementCount()
    {
        chaliceOfAtonementCount.text = $"{player.PlayerChaliceOfAtonement.CurrentChaliceOfAtonementCount}/{player.PlayerChaliceOfAtonement.MaxChaliceOfAtonementCount}";
    }
    void ManageESMImage()
    {
        foreach (ESMImage image in markImageSO.ESMImageList)
        {
            if (image.Name == player.PlayerESMInventory.EquipedESM.Name)
            {
                eSMImage.sprite = image.Image;
            }
        }
    }
}
