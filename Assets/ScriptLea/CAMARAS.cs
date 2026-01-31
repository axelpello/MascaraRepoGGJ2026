using System.Collections;
using UnityEditor.ShaderGraph;
using UnityEngine;

public class CAMARAS : MonoBehaviour
{
    public float maxAngle = 75f;
    public float speed;

    Quaternion leftRot;
    Quaternion rightRot;
    bool goRight = true;

    void Start()
    {
        speed = Random.Range(1, 7);
        leftRot = Quaternion.Euler(0, 0, -maxAngle);
        rightRot = Quaternion.Euler(0, 0, maxAngle);
    }

    void Update()
    {
        Quaternion target = goRight ? rightRot : leftRot;

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            target,
            speed * Time.deltaTime
        );

        if (Quaternion.Angle(transform.rotation, target) < 0.5f)
            goRight = !goRight;
    }

}
