using UnityEngine;

public class Player : MonoBehaviour
{
    public bool IsGrabbed { get; private set; }

    public void SetGrabbedState(bool grabbed)
    {
        IsGrabbed = grabbed;

        // 그랩 상태에서 이동을 막는 코드 추가
        if (grabbed)
        {
            // 추가적으로 애니메이션이나 다른 처리가 필요할 수 있습니다.
        }
    }

    private void Update()
    {
        if (!IsGrabbed)
        {
            // 이동 처리 코드
        }
    }
}
