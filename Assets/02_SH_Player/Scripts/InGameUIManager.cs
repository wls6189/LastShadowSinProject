using UnityEngine;
using UnityEngine.UI;

public class InGameUIManager : MonoBehaviour
{
    [SerializeField] PlayerStats playerStats;

    [SerializeField] Image healthBar;
    [SerializeField] Image spiritMarkForceBar;
    [SerializeField] GameObject spiritWavePrefab;
    [SerializeField] Transform spiritWavePos;
    [SerializeField] MarkImageSO markImageSO;

    GameObject[] spiritWaveArray = new GameObject[10];

    Color spiritWaveVividColor = new Color(1f, 0.9f, 0.4f, 1f);
    Color spiritWaveFaintColor = new Color(1f, 0.9f, 0.4f, 0.4f);
    Color spiritMarkForceVividColor = new Color(0.3f, 0.45f, 0.9f, 1f);
    Color spiritMarkForceFaintColor = new Color(0.3f, 0.45f, 0.9f, 0.3f);


    void Awake()
    {
        InitializeSpiritWave();
    }

    void Update()
    {
        ManageBar();
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
    void ManageBar()
    {
        healthBar.fillAmount = playerStats.CurrentHealth / playerStats.MaxHealth;

        spiritMarkForceBar.fillAmount = playerStats.CurrentSpiritMarkForce / playerStats.MaxSpiritMarkForce;

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
}
