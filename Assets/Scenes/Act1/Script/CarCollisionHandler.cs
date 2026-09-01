using UnityEngine;
using UnityEngine.Playables;

public class CarCollisionHandler : MonoBehaviour
{
    // ===== 기존 필드 =====
    public PlayableDirector accidentTimeline;
    public LogReleaser logReleaser;

    private Rigidbody selfRigidbody;
    private bool hasCollided = false;

    // ===== 추가: 충돌 SFX 옵션 =====
    [Header("Collision SFX")]
    public AudioClip cargoHitSFX;                   // 통나무와 처음 부딪힐 때 재생할 SFX
    [Range(0f, 2f)] public float cargoHitVolume = 1.2f;
    public bool play2D = true;                      // true면 2D(거리감쇠 없음, 크게), false면 3D(위치감)
    public AudioSource sfxSourceOverride;           // 2D 재생용 소스(선택). 비어있으면 자동 생성

    void Start()
    {
        selfRigidbody = GetComponent<Rigidbody>();
        if (selfRigidbody == null)
        {
            Debug.LogError("CarCollisionHandler는 Rigidbody가 필요합니다!");
        }

        // 2D 재생인데 소스가 없으면, 자동으로 2D 전용 오디오소스 하나 생성
        if (play2D && sfxSourceOverride == null)
        {
            var src = gameObject.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.loop = false;
            src.spatialBlend = 0f;   // 2D
            src.dopplerLevel = 0f;
            sfxSourceOverride = src;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasCollided || !collision.gameObject.CompareTag("Log"))
            return;

        hasCollided = true;

        // ---- 충돌 지점 미리 확보 (3D 재생 시 위치에 사용) ----
        Vector3 hitPoint = transform.position;
        if (collision.contactCount > 0)
            hitPoint = collision.GetContact(0).point;

        // 1) 충돌 사운드 먼저 재생 (지연 없이)
        PlayCargoHitSfx(hitPoint);

        Debug.Log($"충돌 감지! ({collision.gameObject.name}) 사고 차량과 통나무를 그 자리에 정지시킵니다.");

        // 2) 타임라인 중지
        if (accidentTimeline != null)
            accidentTimeline.Stop();

        // 3) 차량 이동/물리 정지
        if (selfRigidbody != null)
        {
            GameObject truck = GameObject.Find("truck_wood (4) 1");
            if (truck != null)
            {
                var mv = truck.GetComponent<MoveCar>();
                if (mv) mv.enabled = false;
            }

            var myMove = GetComponent<MoveCar>();
            if (myMove) myMove.enabled = false;

            GameObject Block = GameObject.Find("BlockFromAccident");
            if (Block) Block.SetActive(false);

            // Rigidbody는 velocity/angluarVelocity가 맞습니다.
            selfRigidbody.linearVelocity = Vector3.zero;
            selfRigidbody.angularVelocity = Vector3.zero;
        }

        // 4) 모든 통나무 정지
        if (logReleaser != null && logReleaser.logs != null)
        {
            foreach (Rigidbody logRb in logReleaser.logs)
            {
                if (logRb == null) continue;
                logRb.isKinematic = true;
                logRb.linearVelocity = Vector3.zero;
                logRb.angularVelocity = Vector3.zero;
            }
        }
        else
        {
            Debug.LogWarning("CarCollisionHandler에 logReleaser가 할당되지 않아 통나무를 멈출 수 없습니다.");
        }
    }

    private void PlayCargoHitSfx(Vector3 worldPoint)
    {
        if (cargoHitSFX == null)
            return;

        if (play2D)
        {
            // 2D: 항상 크게, 위치/거리 무시
            if (sfxSourceOverride != null)
            {
                sfxSourceOverride.spatialBlend = 0f;    // 보장
                sfxSourceOverride.dopplerLevel = 0f;
                sfxSourceOverride.volume = 1f;          // 볼륨은 클립의 기본, 최종은 PlayOneShot 배율로
                sfxSourceOverride.PlayOneShot(cargoHitSFX, cargoHitVolume);
            }
            else
            {
                // 혹시나 소스가 없으면 임시 오디오소스 생성해서 2D 재생
                var temp = new GameObject("Temp2D_CargoHitSFX");
                var src = temp.AddComponent<AudioSource>();
                src.playOnAwake = false;
                src.loop = false;
                src.spatialBlend = 0f;
                src.dopplerLevel = 0f;
                src.PlayOneShot(cargoHitSFX, cargoHitVolume);
                GameObject.Destroy(temp, cargoHitSFX.length + 0.1f);
            }
        }
        else
        {
            // 3D: 위치감 있게 들리도록 해당 충돌 지점에서 재생
            AudioSource.PlayClipAtPoint(cargoHitSFX, worldPoint, Mathf.Clamp01(cargoHitVolume));
        }
    }
}
