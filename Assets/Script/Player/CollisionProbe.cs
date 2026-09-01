using UnityEngine;

public class CollisionProbe : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision c) { Debug.Log("3D Hit: " + c.collider.name + " (" + LayerMask.LayerToName(c.collider.gameObject.layer) + ")"); }
    void OnCollisionStay(Collision c) { /* 계속 서있는 상대 확인용 */ }
    void OnCollisionEnter2D(Collision2D c) { Debug.Log("2D Hit: " + c.collider.name + " (" + LayerMask.LayerToName(c.collider.gameObject.layer) + ")"); }
    void OnCollisionStay2D(Collision2D c) { /* 2D도 계속 접촉 확인 */ }

}
