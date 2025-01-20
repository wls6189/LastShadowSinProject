using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [HideInInspector] public bool IsTouching;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            IsTouching = true;
        }

    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            IsTouching = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            IsTouching = false;
        }
    }
}
