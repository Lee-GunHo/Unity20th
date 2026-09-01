using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MoveCar))]
public class TruckCollisionHandler : MonoBehaviour
{
    public GameObject otherCar;

    [Header("Wobble Effect")]
    public float wobbleFrequency = 2f;
    public float wobbleAmplitude = 5f;

    [Header("Behavior Control")]
    public float wobbleDuration = 1.0f;
    public float turnSpeed = 45f;

    [Header("Collision")]
    public string buildingTag = "Building";

    // === 충돌 SFX 추가 ===
    [Header("Crash SFX")]
    public AudioClip crashSfx;                      // 부딪힐 때 재생할 효과음
    [Range(0f, 2f)] public float crashVolume = 1.3f;
    public bool play2D = true;                      // 2D(항상 크게)인지 여부
    public AudioSource sfxSourceOverride;           // 2D 재생용 소스(선택). 비면 자동 생성

    private enum TruckState { Wobbling, TurningRight, Crashed }
    private TruckState currentState = TruckState.Wobbling;

    private Rigidbody truckRigidbody;
    private MoveCar moveCarScript;
    private float startAngleY;
    private float stateTimer = 0f;

    void Start()
    {
        truckRigidbody = GetComponent<Rigidbody>();
        moveCarScript = GetComponent<MoveCar>();
        startAngleY = truckRigidbody.rotation.eulerAngles.y;

        // 2D 재생인데 소스가 없으면 자동으로 생성
        if (play2D && sfxSourceOverride == null)
        {
            sfxSourceOverride = gameObject.AddComponent<AudioSource>();
            sfxSourceOverride.playOnAwake = false;
            sfxSourceOverride.loop = false;
            sfxSourceOverride.spatialBlend = 0f;  // 2D
            sfxSourceOverride.dopplerLevel = 0f;
            sfxSourceOverride.volume = 1f;
        }
    }

    void FixedUpdate()
    {
        stateTimer += Time.fixedDeltaTime;

        switch (currentState)
        {
            case TruckState.Wobbling:
                {
                    float wobble = Mathf.Sin(Time.time * wobbleFrequency);
                    float targetWobbleAngle = wobble * wobbleAmplitude;
                    var wobbleRotation = Quaternion.Euler(
                        truckRigidbody.rotation.eulerAngles.x,
                        startAngleY + targetWobbleAngle,
                        truckRigidbody.rotation.eulerAngles.z
                    );
                    truckRigidbody.MoveRotation(wobbleRotation);

                    if (stateTimer >= wobbleDuration)
                        currentState = TruckState.TurningRight;
                }
                break;

            case TruckState.TurningRight:
                {
                    Quaternion turnOffset = Quaternion.Euler(0, turnSpeed * Time.fixedDeltaTime, 0);
                    truckRigidbody.MoveRotation(truckRigidbody.rotation * turnOffset);
                }
                break;

            case TruckState.Crashed:
                // 완전 정지 상태
                break;
        }
    }

    // 콜라이더가 트리거가 아닐 때
    private void OnCollisionEnter(Collision collision)
    {
        if (currentState == TruckState.Crashed) return;
        if (!IsBuildingHit(collision.collider.transform)) return;

        // 충돌 지점 추출
        Vector3 hitPoint = (collision.contactCount > 0)
            ? collision.GetContact(0).point
            : transform.position;

        CrashStop(hitPoint);
    }

    // 콜라이더가 트리거일 때
    private void OnTriggerEnter(Collider other)
    {
        if (currentState == TruckState.Crashed) return;
        if (!IsBuildingHit(other.transform)) return;

        Vector3 hitPoint = other.ClosestPoint(transform.position);
        CrashStop(hitPoint);
    }

    private bool IsBuildingHit(Transform hit)
    {
        return hit.CompareTag(buildingTag) || hit.root.CompareTag(buildingTag);
    }

    private void CrashStop(Vector3 hitPoint)
    {
        currentState = TruckState.Crashed;

        // 0) 충돌 사운드 1회 재생
        PlayCrashSfx(hitPoint);

        // 1) 이 트럭 완전 정지
        if (moveCarScript) moveCarScript.enabled = false;
        if (truckRigidbody)
        {
            truckRigidbody.linearVelocity = Vector3.zero;          // linearVelocity → velocity
            truckRigidbody.angularVelocity = Vector3.zero;

            // 필요시 둘 다 사용해 완전 잠금
            truckRigidbody.isKinematic = true;
            truckRigidbody.constraints = RigidbodyConstraints.FreezeAll;
        }

        // 2) 다른 차도 정지
        StopSpecificCar(otherCar);

        // 3) 엔진 루프 끄기 (있을 때만)
        var engine = GetComponent<EngineAudioOnMove>();
        if (engine) engine.StopNow();

        // 4) 타임라인/애니메이터 등이 움직임 주면 차단
        var anim = GetComponent<Animator>();
        if (anim) anim.enabled = false;
    }

    void StopSpecificCar(GameObject car)
    {
        if (!car) return;

        var otherMove = car.GetComponent<MoveCar>();
        if (otherMove) otherMove.enabled = false;

        var otherRb = car.GetComponent<Rigidbody>();
        if (otherRb)
        {
            otherRb.linearVelocity = Vector3.zero;                 // linearVelocity → velocity
            otherRb.angularVelocity = Vector3.zero;
            otherRb.isKinematic = true;
            otherRb.constraints = RigidbodyConstraints.FreezeAll;
        }

        var otherEngine = car.GetComponent<EngineAudioOnMove>();
        if (otherEngine) otherEngine.StopNow();

        var otherAnim = car.GetComponent<Animator>();
        if (otherAnim) otherAnim.enabled = false;
    }

    // === SFX 재생 로직 ===
    private void PlayCrashSfx(Vector3 worldPoint)
    {
        if (crashSfx == null) return;

        if (play2D)
        {
            if (sfxSourceOverride == null)
            {
                // 혹시 없으면 임시 2D 소스 생성
                var temp = new GameObject("Temp2D_CrashSFX");
                var src = temp.AddComponent<AudioSource>();
                src.playOnAwake = false;
                src.loop = false;
                src.spatialBlend = 0f;
                src.dopplerLevel = 0f;
                src.PlayOneShot(crashSfx, crashVolume);
                Destroy(temp, crashSfx.length + 0.1f);
            }
            else
            {
                sfxSourceOverride.PlayOneShot(crashSfx, crashVolume);
            }
        }
        else
        {
            // 3D 위치감 있게
            AudioSource.PlayClipAtPoint(crashSfx, worldPoint, Mathf.Clamp01(crashVolume));
        }
    }
}
