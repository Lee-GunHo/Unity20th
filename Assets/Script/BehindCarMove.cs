using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class BehindCarMove : MonoBehaviour
{
    public float speed = 1000f;
    public bool autoMove = true;

    [Header("Audio")]
    public AudioSource engineSource;
    public AudioClip engineLoop;

    private Rigidbody rb;
    private bool isMoving;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        isMoving = autoMove;

        if(engineSource&&engineLoop)
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

        if(engineSource)
        {
            float v = rb.linearVelocity.magnitude;
            engineSource.pitch = Mathf.Lerp(0.8f, 1.25f, v / Mathf.Max(0.1f, speed));
            engineSource.volume = 0.6f + 0.4f * (v / Mathf.Max(0.1f, speed));
        }
    }
    // Update is called once per frame
    public void StopNow()
    {
        isMoving = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        if (engineSource && engineSource.isPlaying) engineSource.Stop();
    }

}
