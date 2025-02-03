using UnityEngine;

public class TEMPDROPITEM : MonoBehaviour
{
    MonsterDrop m;

    private void Awake()
    {
        TryGetComponent(out m);
    }

    private void Start()
    {
        m.Drop();
    }
}
