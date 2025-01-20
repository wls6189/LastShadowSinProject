using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneInGame : MonoBehaviour
{
    public TextMeshProUGUI name;
    public TextMeshProUGUI level;
    public TextMeshProUGUI coin;

    //public GameObject[] WeaponItem; 테스트용



    private void Start() 
    {
        // 게임 씬에 넘어오면 플레이어의 정보(이름, 레벨, 코인)가 알맞게 나타나야 함.
        name.text += DataManager.Instance.nowPlayer.name; // name 은 현재 이름: 이므로 name += 형태로 진행.
        level.text += DataManager.Instance.nowPlayer.level.ToString(); // text string이고 level은 int이므로 ToString()
        coin.text += DataManager.Instance.nowPlayer.coin.ToString();

        // 플레이어의 위치 설정 (씬이 완전히 로드된 후 위치 변경)
    
       

        // 아이템 설정 (주석처리된 부분, 필요시 활성화)
        // ItemSetting(DataManager.Instance.nowPlayer.item);
    }

    public void SetPlayerPos()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Debug.Log("플레이어 찾음");
            Debug.Log("플레이어 위치는 " + DataManager.Instance.nowPlayer.position);
            player.transform.position = DataManager.Instance.nowPlayer.position;
        }
    }


    public void LevelUp()
    {
        DataManager.Instance.nowPlayer.level++; //값 바꾸기
        level.text = "레벨 : " + DataManager.Instance.nowPlayer.level.ToString(); //UI변화를 위함
    }

    public void CoinUp()
    {
        DataManager.Instance.nowPlayer.coin += 100;
        coin.text = "코인 : " + DataManager.Instance.nowPlayer.coin.ToString(); //UI변화를 위함
    }

    public void Save()
    {
        DataManager.Instance.SaveData();
    }



    //public void ItemSetting(int number)
    //{
    //    for(int i = 0; i< WeaponItem.Length; i++)
    //    {
    //        if(number == i) //선택한 무기(0)가 첫번째(0) 라면 
    //        { 
    //            WeaponItem[i].SetActive(true); //첫번째 무기만 활성화
    //            DataManager.Instance.nowPlayer.item = number; //활성화된 무기의 번호가 PlayerData의 item 변수에 저장됨. 즉, 대응 됨.
    //        }
    //        else //아니라면
    //        {
    //            WeaponItem[i].SetActive(false); //비활성화
    //        }
    //    }
    //}
}
