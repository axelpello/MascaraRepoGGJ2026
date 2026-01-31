using UnityEngine;

public class JugadorNiebla : MonoBehaviour
{
    public float velocidad = 2f;
    Rigidbody2D rb;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void FixedUpdate()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 newPos = Vector2.MoveTowards(rb.position, mousePos, velocidad * Time.fixedDeltaTime);
        rb.MovePosition(newPos);
    }
}
