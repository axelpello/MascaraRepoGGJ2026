using System.Globalization;
using UnityEngine;

public class Minijuego3Trigger : MonoBehaviour
{
    //float nameToFloat;
    void Awake()
    {
        //float nameToFloat = float.Parse(gameObject.name, CultureInfo.InvariantCulture);
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        //collision.gameObject.GetComponent<Minijuego3ItemsScale>().StopCoroutine(collision.gameObject.GetComponent<Minijuego3ItemsScale>().ScaleItem(nameToFloat, collision.transform));
        //StartCoroutine(collision.gameObject.GetComponent<Minijuego3ItemsScale>().ScaleItem(nameToFloat, collision.transform));
    }
}
