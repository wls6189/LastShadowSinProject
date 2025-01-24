using UnityEngine;

public class TempPJ : MonoBehaviour
{
    GameObject target;

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(0, 0, -3 * Time.deltaTime);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("PlayerGuard") && target != other.gameObject.transform.parent.gameObject)
        {
            target = other.gameObject.transform.parent.gameObject;
            other.gameObject.transform.parent.GetComponent<PlayerStats>().Damaged(10f, 2, 2, AttackType.Normal);

        }

        if (other.CompareTag("Player") && target != other.gameObject)
        {
            target = other.gameObject;
            other.gameObject.GetComponent<PlayerStats>().Damaged(10f, 2, 2, AttackType.Normal, null, true);
        }

    }
}
