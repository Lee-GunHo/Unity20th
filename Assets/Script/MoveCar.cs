using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class MoveCar : MonoBehaviour
{
    public float speed = 1; // 차량 기본 속도
    public int eventNum = 1; // 무슨 사건인지 식별할 변수

    // Update is called once per frame
    void Update()
    {
        if (eventNum == 1)
            this.transform.Translate(new Vector3(0, 0, speed * 50) * Time.deltaTime);
        else if (eventNum == 2)
        {

        }

    }
}
