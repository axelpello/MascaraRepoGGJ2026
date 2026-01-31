using TMPro;
using UnityEngine;

public class Mopa : MonoBehaviour
{
    int puntos = 0;
    int maxPuntos = 5;
    public TextMeshProUGUI contadorPuntos;
    public GameObject MiniJuego3;
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
            puntos++;
            if (puntos >= maxPuntos)
            {
                Debug.Log("ganaste");
                MiniJuego3.SetActive(false);
            }
            contadorPuntos.text = "Ensucia la mopa: " + puntos.ToString() + "/" + maxPuntos;
            Destroy(col.gameObject);
            Debug.Log("me oscureco");
        }
        if (col.CompareTag("Limpiador"))
        {
            if (puntos > 0)
            {
                puntos--;
            }
            else
            {
                puntos = 0;
            }
            contadorPuntos.text = "Ensucia la mopa: " + puntos.ToString() + "/" + maxPuntos;
            Destroy(col.gameObject);
            Debug.Log("me aclaro");
        }
    }
}
