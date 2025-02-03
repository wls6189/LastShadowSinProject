using Unity.VisualScripting;
using UnityEngine;

public class look : MonoBehaviour
{

    void Update()
    {
        transform.LookAt(transform.position + Camera.main.transform.forward);
    }
}
