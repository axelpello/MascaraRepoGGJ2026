using UnityEngine;

public class JugadorNiebla : MonoBehaviour
{
    public float velocidad = 2f;
    Rigidbody2D rb;
    public float rotateSpeed = 10f;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void FixedUpdate()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 newPos = Vector2.MoveTowards(rb.position, mousePos, velocidad * Time.fixedDeltaTime);
        rb.MovePosition(newPos);


        Vector2 dir = mousePos - rb.position;

        if (dir.sqrMagnitude < 0.001f) return;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        angle = Mathf.LerpAngle(
           rb.rotation,
           angle,
           rotateSpeed * Time.fixedDeltaTime
       );
        rb.MoveRotation(angle);
    }
}
