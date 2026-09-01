using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FootStepController : MonoBehaviour
{
    [Header("Clips")]
    public AudioClip[] footstepClips;
    [Range(0f, 1f)] public float volume = 0.7f;
    [Range(0f, 0.5f)] public float pitchRandomRange = 0.07f;

    [Header("Step Distance (meters)")]
    public float walkStepDistance = 1.3f;
    public float runStepDistance = 1.0f;
    public float minMoveSpeed = 0.05f;

    [Header("Ground Check")]
    public LayerMask groundMask = ~0;
    public float groundCheckDistance = 0.2f;

    [Header("Run Key (optional)")]
    public KeyCode runKey = KeyCode.LeftShift;

    AudioSource _src;
    CharacterController _cc;
    Rigidbody _rb;
    Vector3 _lastXZ;
    float _accum;

    void Awake()
    {
        _src = GetComponent<AudioSource>();
        _cc = GetComponent<CharacterController>();
        _rb = GetComponent<Rigidbody>();

        _src.playOnAwake = false;
        _src.loop = false;
        _src.spatialBlend = 1f;
        _src.rolloffMode = AudioRolloffMode.Logarithmic;
        _src.minDistance = 1f;
        _src.maxDistance = 35f;

        _lastXZ = new Vector3(transform.position.x, 0f, transform.position.z);
    }

    void Update()
    {
        if (!IsGrounded()) { ResetStep(); return; }

        float speed = GetHorizontalSpeed();
        if (speed < minMoveSpeed) { ResetStep(); return; }

        Vector3 currentXZ = new Vector3(transform.position.x, 0f, transform.position.z);
        float frameMove = Vector3.Distance(currentXZ, _lastXZ);
        _lastXZ = currentXZ;

        _accum += frameMove;

        bool running = Input.GetKey(runKey);
        float targetDist = running ? runStepDistance : walkStepDistance;

        if (_accum >= targetDist)
        {
            PlayFootstep(running);
            _accum = 0f;
        }
    }

    void ResetStep()
    {
        _accum = 0f;
        _lastXZ = new Vector3(transform.position.x, 0f, transform.position.z);
    }

    float GetHorizontalSpeed()
    {
        Vector3 currentXZ = new Vector3(transform.position.x, 0f, transform.position.z);
        float speed = Vector3.Distance(currentXZ, _lastXZ) / Mathf.Max(Time.deltaTime, 0.0001f);
        return speed;
    }

    bool IsGrounded()
    {
        if (_cc) return _cc.isGrounded;
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        return Physics.Raycast(origin, Vector3.down, groundCheckDistance + 0.1f, groundMask, QueryTriggerInteraction.Ignore);
    }

    void PlayFootstep(bool running)
    {
        if (footstepClips == null || footstepClips.Length == 0) return;

        int idx = Random.Range(0, footstepClips.Length);
        AudioClip clip = footstepClips[idx];

        float basePitch = 1f + (running ? 0.05f : 0f);
        float jitter = Random.Range(-pitchRandomRange, pitchRandomRange);
        _src.pitch = Mathf.Clamp(basePitch + jitter, 0.5f, 1.5f);

        _src.PlayOneShot(clip, volume);
    }
}