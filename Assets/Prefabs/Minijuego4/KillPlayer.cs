using UnityEngine;

public class KillPlayer : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        collision.GetComponent<JugadorNiebla>().ResetPos();
    }
}
