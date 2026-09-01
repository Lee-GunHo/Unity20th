using UnityEngine;

public class LogReleaser : MonoBehaviour
{
    // 인스펙터 창에서 떨어뜨릴 통나무들을 모두 할당
    public Rigidbody[] logs;
    // 인스펙터 창에서 부딪힐 작은 차를 할당
    public Rigidbody smallCar;

    // Timeline에서 이 함수를 호출
    public void ReleaseLogs()
    {
        Debug.Log("사고 발생! 통나무를 해제합니다.");

        // 작은 차의 Kinematic을 해제하여 물리 충격을 받도록 준비
        if (smallCar != null)
        {
            smallCar.isKinematic = false;
        }

        // 모든 통나무의 Kinematic을 해제하고 부모 관계를 끊음
        foreach (Rigidbody logRb in logs)
        {
            if (logRb != null)
            {
                logRb.transform.SetParent(null); // 트럭과의 부모-자식 관계 해제
                logRb.isKinematic = false;      // 물리 엔진 활성화 (중력 적용 시작)
                
                // 통나무가 살짝 튀어 오르듯이 날아가도록 힘을 가함
                // 방향 벡터는 트럭의 종류와 통나무가 떨어지는 방식에 따라 조정
                Vector3 launchDirection = (Vector3.up * 0.5f + Vector3.forward * 0.5f + Random.insideUnitSphere * 0.2f).normalized;
                float launchForce = Random.Range(5f, 10f); // 통나무마다 약간씩 다른 힘을 줌
                
                // 트럭의 로컬 기준으로 힘을 가하려면 이 부분을 조정해야 할 수도 있음
                // 현재는 월드 기준으로 힘을 가함 (트럭이 z축 방향으로 달린다고 가정)
                // 만약 트럭의 현재 방향에 따라 힘을 주고 싶다면 아래처럼 변경:
                // launchDirection = logRb.transform.up * 0.5f + logRb.transform.forward * 0.5f + Random.insideUnitSphere * 0.2f;

                logRb.AddForce(launchDirection * launchForce, ForceMode.Impulse); 
                // Impulse: 순간적인 힘을 가함 (폭발, 충돌 등)
            }
        }
    }
}