using UnityEngine;

public class TempMob : MonoBehaviour
{
    [SerializeField] GameObject tp;
    float a;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        a += Time.deltaTime;

        if (a > 2)
        {
            a = 0;
            Instantiate(tp);
        }
    }
}
