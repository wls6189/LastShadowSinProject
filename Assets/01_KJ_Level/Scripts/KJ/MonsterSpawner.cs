using System.Collections;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject[] monsterPrefabs;   // 스폰할 몬스터 프리팹 배열
    public Transform spawnPos;            // 몬스터를 스폰할 위치
    public float spawnInterval = 3f;      // 몬스터 스폰 주기 (초)
    public int maxMonsters = 5;           // 한 번에 스폰할 최대 몬스터 수
    private int currentMonsters = 0;      // 현재 스폰된 몬스터 수


    void Start()
    {
        StartCoroutine(SpawnMonsters()); // 스폰 코루틴 시작
    }

    IEnumerator SpawnMonsters()
    {
        // 최대 몬스터 수에 도달할 때까지 스폰
        while (currentMonsters != maxMonsters)
        {
            SpawnMonster(); // 몬스터 스폰
            yield return new WaitForSeconds(spawnInterval); // 주기적인 대기
        }
    }

    private GameObject monster;
    void SpawnMonster()
    {
        if (monsterPrefabs.Length == 0 || spawnPos == null) return; // 프리팹이 없거나 위치가 없으면 종료

        int monsterIndex = Random.Range(0, monsterPrefabs.Length); // 랜덤으로 몬스터 선택
        monster = Instantiate(monsterPrefabs[monsterIndex], spawnPos.position, spawnPos.rotation); // 몬스터 스폰
        currentMonsters++; // 현재 몬스터 수 증가
    }
}
