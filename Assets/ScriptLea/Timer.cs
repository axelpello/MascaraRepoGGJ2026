using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    //public int maxTime = 20;
    public float tiempoActual;
    public bool activo = false;
    TextMeshProUGUI timer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer = gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!activo) return;

        tiempoActual -= Time.deltaTime;
        timer.text = "La policia llegara en: " + tiempoActual.ToString("F0");

        if (tiempoActual <= 0f)
        {
            tiempoActual = 0f;
            activo = false;
            TerminoElTimer();
        }

    }
    void TerminoElTimer()
    {
        timer.text = "llego la ley >O";

    }
}
