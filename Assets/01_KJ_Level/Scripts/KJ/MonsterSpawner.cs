using System.Collections;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject[] monsterPrefabs;   // ������ ���� ������ �迭
    public Transform spawnPos;            // ���͸� ������ ��ġ
    public float spawnInterval = 3f;      // ���� ���� �ֱ� (��)
    public int maxMonsters = 5;           // �� ���� ������ �ִ� ���� ��
    private int currentMonsters = 0;      // ���� ������ ���� ��


    void Start()
    {
        StartCoroutine(SpawnMonsters()); // ���� �ڷ�ƾ ����
    }

    IEnumerator SpawnMonsters()
    {
        // �ִ� ���� ���� ������ ������ ����
        while (currentMonsters != maxMonsters)
        {
            SpawnMonster(); // ���� ����
            yield return new WaitForSeconds(spawnInterval); // �ֱ����� ���
        }
    }

    private GameObject monster;
    void SpawnMonster()
    {
        if (monsterPrefabs.Length == 0 || spawnPos == null) return; // �������� ���ų� ��ġ�� ������ ����

        int monsterIndex = Random.Range(0, monsterPrefabs.Length); // �������� ���� ����
        monster = Instantiate(monsterPrefabs[monsterIndex], spawnPos.position, spawnPos.rotation); // ���� ����
        currentMonsters++; // ���� ���� �� ����
    }
}
