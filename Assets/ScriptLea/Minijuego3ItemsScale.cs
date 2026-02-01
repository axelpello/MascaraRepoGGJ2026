using System.Collections;
using System.Globalization;
using UnityEngine;

public class Minijuego3ItemsScale : MonoBehaviour
{
    private Vector3 scaleVec3;
    [SerializeField]
    float vel = 3;
    public float duracion;
    Rigidbody2D rb;
    public IEnumerator ScaleItem()
    {
        scaleVec3 = gameObject.transform.localScale;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duracion;
            transform.localScale = Vector3.Lerp(scaleVec3, new Vector3(.6f, .6f, .6f), t);
            yield return null;
        }

        transform.localScale = scaleVec3;
    }

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        StartCoroutine(ScaleItem());
    }
    void FixedUpdate()
    {
        rb.linearVelocity = -transform.up * vel * 3f;
    }
    void Update()
    {
        //transform.position += transform.TransformDirection(Vector3.down) * vel * Time.deltaTime;
    }
}
