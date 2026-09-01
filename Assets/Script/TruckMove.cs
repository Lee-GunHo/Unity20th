using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TruckMove : MonoBehaviour
{
    [Header("Move")]
    public float speed = 1000f;
    public bool automove = true;

    [Header("Audio")]
    public AudioSource engineSource;
    public AudioClip engineLoop;
    public AudioClip crashSFX;

    [Header("Refs")]
    public BehindCarMove CarBehind;

    private Rigidbody rb;
    private bool isMoving;
    private bool hasCrashed;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        isMoving = automove;

        if (engineSource && engineLoop)
        {
            engineSource.clip = engineLoop;
            engineSource.loop = true;
            if (isMoving && !engineSource.isPlaying) engineSource.Play();
        }

    }

    void FixedUpdate()
    {
        if (!isMoving) return;

        Vector3 next = rb.position + transform.forward * speed * Time.fixedDeltaTime;
        rb.MovePosition(next);

        if (engineSource)
        {

            float v = rb.linearVelocity.magnitude;
            engineSource.pitch = Mathf.Lerp(0.8f, 1.3f, v / Mathf.Max(0.1f, speed));
            engineSource.volume = 0.6f + 0.4f * (v / Mathf.Max(0.1f, speed));
        }
    }

    private void OnCollisionEnter(Collision col)
    {
        if (hasCrashed) return;

        if (col.gameObject.CompareTag("male"))
        {
            hasCrashed = true;
            StopNow();

            if (crashSFX != null)
            {
                var listener = Camera.main ? Camera.main.gameObject : gameObject;
                var src = listener.AddComponent<AudioSource>();
                src.spatialBlend = 0f;  
                src.playOnAwake = false;
                src.volume = 1f;      
                src.PlayOneShot(crashSFX, 1f);
                Destroy(src, crashSFX.length);
            }

            var ped = col.gameObject.GetComponentInParent<PedMove>();
            if (ped != null)
            {
                Vector3 hitPoint = col.GetContact(0).point;
                Vector3 dir = transform.forward;
                float power = Mathf.Clamp(rb.linearVelocity.magnitude * 1.5f, 6f, 20f);
                ped.OnHit(dir, hitPoint, power);
            }

            if (CarBehind != null) CarBehind.StopNow();
        }
    }

    public void StopNow()
    {
        isMoving = false;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        if (engineSource && engineSource.isPlaying) engineSource.Stop();
    }
}
