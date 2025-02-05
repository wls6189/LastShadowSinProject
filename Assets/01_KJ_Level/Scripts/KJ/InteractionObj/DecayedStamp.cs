using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DecayedStamp : Interaction
{
    //ó�� ��ȣ�ۿ� �� ������ ���� �� Destroy �ߴٰ� �ٽ� �ε��ϸ� ��Ȱ��ȭ. 

   

    [SerializeField]
    public string currentScenesDecayedStamp;

    [SerializeField]
    private GameObject EffectPrefab;

 
    void Start()
    {
        currentScenesDecayedStamp = SceneManager.GetActiveScene().name;

        if (DataManager.Instance.nowPlayer.currentScenesDecayedStamp.Contains(currentScenesDecayedStamp))
        {
            if (DataManager.Instance.nowPlayer.DecayedStampCount > 0)
            {
                Destroy(this.gameObject);
                //this.gameObject.SetActive(true);
            }
        }
        

    }

    public override void InteractionPlayer( )
    {
        if (DataManager.Instance.nowPlayer.currentScenesDecayedStamp.Contains(currentScenesDecayedStamp))
        {
            Debug.Log("�̹� ������ ����Ǽ� ī��Ʈ �� �ø�");
            return;
        }
        DataManager.Instance.nowPlayer.DecayedStampCount += 1;
        DataManager.Instance.nowPlayer.currentScenesDecayedStamp.Add(currentScenesDecayedStamp);
        DataManager.Instance.SaveData();
        EffectPrefab.gameObject.SetActive(false);
    }
}
