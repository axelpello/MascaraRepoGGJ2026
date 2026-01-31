using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public int maxTime = 20;
    public float tiempoActual;
    public bool activo = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!activo) return;

        tiempoActual -= Time.deltaTime;
        gameObject.GetComponent<TextMeshProUGUI>().text = "La policia llegara en: " + tiempoActual.ToString("F0");

        if (tiempoActual <= 0f)
        {
            tiempoActual = 0f;
            activo = false;
            TerminoElTimer();
        }

    }
    void TerminoElTimer()
    {
        Debug.Log("⏰ Timer terminado");
    }
}
