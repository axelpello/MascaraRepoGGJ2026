using UnityEngine;

public class Mopa : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Basura"))
        {
            Destroy(col.gameObject);
            Debug.Log("me oscureco");
        }
        if (col.CompareTag("Limpiador"))
        {
            Destroy(col.gameObject);
            Debug.Log("me aclaro");
        }
    }
}
