using UnityEngine;

public class UIAutoDestroy : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 10f);
    }
}
