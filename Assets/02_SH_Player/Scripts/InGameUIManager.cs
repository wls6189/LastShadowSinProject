using UnityEngine;
using UnityEngine.UI;

public class InGameUIManager : MonoBehaviour
{
    [SerializeField] Image healthBar;
    [SerializeField] PlayerStats playerStats;
    void Start()
    {
        
    }

    void Update()
    {
        ManageHealthBar();
    }

    void ManageHealthBar()
    {
        healthBar.fillAmount = playerStats.CurrentHealth / playerStats.MaxHealth;
    }
}
