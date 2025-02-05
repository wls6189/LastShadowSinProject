using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    public Image bossHealthBar; 
    public Image bossWillpowerBar;
    public TextMeshProUGUI bossNameText;
    public string bossName; 
    public EnemyStats bossEnemyStats; 

    private void Start()
    {
        
        if (bossNameText != null)
        {
            bossNameText.text = bossName; 
        }
    }

    private void Update()
    {
        if (bossEnemyStats != null)
        {
            
            bossHealthBar.fillAmount = bossEnemyStats.currentHealth / bossEnemyStats.maxHealth;
            bossWillpowerBar.fillAmount = bossEnemyStats.currentWillpower / bossEnemyStats.maxWillpower;
        }
    }
}
