using UnityEngine;

public class MaleStopOnCollision : MonoBehaviour
{
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        // cat�� �ε�����
        if (collision.gameObject.CompareTag("Cat"))
        {
            // �ӵ��� ȸ�� �ӵ� ��� 0���� ����
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // �ʿ�� ������ ���߱�
            rb.isKinematic = true; // <- �� �̻� ���� ���� �� �ް� �� (���û���)
        }
    }
}
