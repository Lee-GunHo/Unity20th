using UnityEngine;

public class LooseNut : MonoBehaviour
{
    [Header("조임 단계 설정")]
    [SerializeField] private int maxTightness = 3;  // 최대 조임 횟수
    [SerializeField] private float rotationPerTighten = 30f; // 회전 각도
    [SerializeField] private AudioClip tightenSound;

    private int currentTightness = 0;
    private bool isTightened = false;

    public void Tighten()
    {
        if (isTightened)
        {
            Debug.Log("[LooseNut] 이미 다 조여진 상태");
            return;
        }

        currentTightness++;
        transform.Rotate(Vector3.right, rotationPerTighten); // 실제로 회전
        Debug.Log($"[LooseNut] 나사 조임 ({currentTightness}/{maxTightness})");

        if (tightenSound != null)
            AudioSource.PlayClipAtPoint(tightenSound, transform.position);

        if (currentTightness >= maxTightness)
        {
            isTightened = true;
            OnFullyTightened();
        }
    }

    private void OnFullyTightened()
    {
        Debug.Log("[LooseNut] 완전히 조여짐!");
        // 필요하면 애니메이션, 문 열기, 이벤트 등 추가 가능
    }
}
