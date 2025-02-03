using UnityEngine;

public class Projectile : MonoBehaviour
{
    bool IsMoveRight = true;

    private void Start()
    {
        Destroy(gameObject, 5f);
    }
    void Update()
    {
        if (IsMoveRight)
        {
            transform.position += new Vector3(0, 0, 10f * Time.deltaTime);
        }
        else
        {
            transform.position -= new Vector3(0, 0, 10f * Time.deltaTime);
        }
    }

    public void SetIsMoveRight(bool isMoveRight)
    {
        if (isMoveRight)
        {
            transform.eulerAngles = new Vector3(-180, 0, 0);
            IsMoveRight = true;
        }
        else
        {
            transform.eulerAngles = Vector3.zero;
            IsMoveRight = false;
        }
    }
}
