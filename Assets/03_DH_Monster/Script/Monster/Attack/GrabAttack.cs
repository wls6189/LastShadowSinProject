using UnityEngine;

public class GrabAttack : MonoBehaviour
{
    public Transform grabPoint; // �÷��̾ ������ ��ġ
    public float grabDuration = 2.0f; // ��� ���� �ð�
    private Animator animator; // Animator ������Ʈ ����
    public float damageMultiplier;

    private void Start()
    {      
        animator = GetComponentInParent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // �÷��̾����� Ȯ��
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                // �ִϸ��̼� Ʈ���� Ȱ��ȭ
                animator.SetTrigger("GrabSuccess");
                // �÷��̾� ��� ����
                //player.OnGrabbed(grabPoint, grabDuration);
                //EnemyStats enemyStats = GetComponent<EnemyStats>();
                //if (enemyStats != null)
                //{
                //    // ���� ������ ���
                //    float finalDamage = enemyStats.attackPower * damageMultiplier;

                //    // �÷��̾��� PlayerStats ��������
                //    PlayerStats playerStats = other.GetComponent<PlayerStats>();
                //    if (playerStats != null)
                //    {
                //        // �÷��̾�� ���� ������ ����
                //        playerStats.Damaged(finalDamage);
                    }
                }
            }
        }
    