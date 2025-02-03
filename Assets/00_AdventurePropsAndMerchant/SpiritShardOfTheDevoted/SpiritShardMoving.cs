using UnityEngine;

public class SpiritShardMoving : MonoBehaviour
{
    bool IsMoveUp;
    void Update()
    {
        transform.eulerAngles += new Vector3(0, 60f * Time.deltaTime, 0);

        if (transform.position.y > 1.2f)
        {
            IsMoveUp = false;
        }
        if (transform.position.y < 0.8f)
        {
            IsMoveUp = true;
        }

        if (IsMoveUp)
        {
            transform.position += new Vector3(0, 0.15f * Time.deltaTime, 0);
        }
        else
        {
            transform.position -= new Vector3(0, 0.15f * Time.deltaTime, 0);
        }
    }
}
