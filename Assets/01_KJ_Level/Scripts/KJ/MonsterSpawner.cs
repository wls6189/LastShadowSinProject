using System.Collections;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject[] monsterPrefabs;   // 스폰할 몬스터 프리팹 배열
    public Transform[] spawnPoints;        // 몬스터를 스폰할 위치 배열
    public float spawnInterval = 3f;       // 몬스터 스폰 주기 (초)
    public int maxMonsters = 5;            // 한 번에 스폰할 최대 몬스터 수
    private int currentMonsters = 0;       // 현재 스폰된 몬스터 수

    void Start()
    {
        StartCoroutine(SpawnMonsters());
    }

    IEnumerator SpawnMonsters()
    {
        while (true)
        {
            // 현재 스폰된 몬스터 수가 최대 수보다 적으면 몬스터 스폰
            if (currentMonsters < maxMonsters)
            {
                // 랜덤 스폰 포인트 선택
                int spawnPointIndex = Random.Range(0, spawnPoints.Length);
                Transform spawnPoint = spawnPoints[spawnPointIndex];

                // 랜덤 몬스터 선택
                int monsterIndex = Random.Range(0, monsterPrefabs.Length);
                GameObject monsterPrefab = monsterPrefabs[monsterIndex];

                // 몬스터 스폰
                Instantiate(monsterPrefab, spawnPoint.position, spawnPoint.rotation);
                currentMonsters++;

                // 스폰 후 잠시 대기
                yield return new WaitForSeconds(spawnInterval);
            }

            // 최대 몬스터 수에 도달하면 잠시 대기 후 다시 확인
            else
            {
                yield return null;
            }
        }
    }

    
    public void OnMonsterDeath() //-> 몬스터 죽이면 호출해야 함. 
    {
        currentMonsters--;
    }

}
