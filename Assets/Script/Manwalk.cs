using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Manwalk : MonoBehaviour
{
    [Header("Walk")]
    public float walkSpeed = 150f;
    public Animator animator;
    public string walkBoolName = "IsWalking";

    [Header("Audio")]
    public AudioSource sfxSource;
    public AudioClip hitSFX;

    private Rigidbody rb;
    private bool isHit;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // 충돌 전에는 애니로만 이동
        if (animator) animator.SetBool(walkBoolName, true);
    }

    void Update()
    {
        if (isHit) return; // 충돌 시 이동 정지
        transform.position += transform.forward * walkSpeed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cat"))
        {
            isHit = true;
            if (animator) animator.SetBool(walkBoolName, false);
            rb.isKinematic = true;
        }
    }

    // TruckMove에서 호출하는 메서드명과 대/소문자 일치!
    public void OnHit(Vector3 truckForward, Vector3 hitPoint, float power)
    {
        if (isHit) return;
        isHit = true;

        if (animator) animator.enabled = false;
        if (sfxSource && hitSFX) sfxSource.PlayOneShot(hitSFX);

        rb.isKinematic = false;

        Vector3 impulse = (truckForward.normalized + Vector3.up * 0.6f).normalized * power;
        rb.AddForceAtPosition(impulse, hitPoint, ForceMode.Impulse);
        rb.AddTorque(transform.right * power * 0.5f, ForceMode.Impulse);
    }
}