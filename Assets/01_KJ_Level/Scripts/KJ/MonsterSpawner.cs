using System.Collections;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject[] monsterPrefabs;   // ������ ���� ������ �迭
    public Transform[] spawnPoints;        // ���͸� ������ ��ġ �迭
    public float spawnInterval = 3f;       // ���� ���� �ֱ� (��)
    public int maxMonsters = 5;            // �� ���� ������ �ִ� ���� ��
    private int currentMonsters = 0;       // ���� ������ ���� ��

    void Start()
    {
        StartCoroutine(SpawnMonsters());
    }

    IEnumerator SpawnMonsters()
    {
        while (true)
        {
            // ���� ������ ���� ���� �ִ� ������ ������ ���� ����
            if (currentMonsters < maxMonsters)
            {
                // ���� ���� ����Ʈ ����
                int spawnPointIndex = Random.Range(0, spawnPoints.Length);
                Transform spawnPoint = spawnPoints[spawnPointIndex];

                // ���� ���� ����
                int monsterIndex = Random.Range(0, monsterPrefabs.Length);
                GameObject monsterPrefab = monsterPrefabs[monsterIndex];

                // ���� ����
                Instantiate(monsterPrefab, spawnPoint.position, spawnPoint.rotation);
                currentMonsters++;

                // ���� �� ��� ���
                yield return new WaitForSeconds(spawnInterval);
            }

            // �ִ� ���� ���� �����ϸ� ��� ��� �� �ٽ� Ȯ��
            else
            {
                yield return null;
            }
        }
    }

    
    public void OnMonsterDeath() //-> ���� ���̸� ȣ���ؾ� ��. 
    {
        currentMonsters--;
    }

}
