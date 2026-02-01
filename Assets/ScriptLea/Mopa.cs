using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Mopa : MonoBehaviour
{
    int puntos = 0;
    int maxPuntos = 5;
    public TextMeshProUGUI contadorPuntos;
    public GameObject MiniJuego3;
    public SoundController SC;
    private float mopaPos;
    public SpriteRenderer mopaOnly;
    public List<Color> mopaColors;
    int colorIndx;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mopaOnly = gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
        mopaPos = gameObject.transform.localPosition.x;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(mopaPos);
        //Debug.Log(gameObject.transform.localPosition.x);
        if (mopaPos > gameObject.transform.localPosition.x + 500 || mopaPos < gameObject.transform.localPosition.x - 500)
        {
            mopaPos = gameObject.transform.localPosition.x;
            SC.PlayOnce(SoundController.SFX.Mopa);
        }
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Basura"))
        {
            puntos++;
            if (puntos >= maxPuntos)
            {
                //Debug.Log("ganaste");
                contadorPuntos.text = "";
                MiniJuego3.SetActive(false);
                foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Limpiador"))
                {
                    Destroy(obj);
                }

                foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Basura"))
                {
                    Destroy(obj);
                }
                return;
            }
            contadorPuntos.text = "Ensucia la mopa: " + puntos.ToString() + "/" + maxPuntos;
            colorIndx++;
            if (colorIndx > maxPuntos - 1)
            {
                colorIndx = 4;
                mopaOnly.color = mopaColors[colorIndx];
                //Debug.Log(colorIndx);
            }
            else
            {
                //Debug.Log(colorIndx);
                mopaOnly.color = mopaColors[colorIndx];
            }
            SC.PlayOnce(SoundController.SFX.Sucio);
            Destroy(col.gameObject);
            //Debug.Log("me oscureco");
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
            colorIndx--;
            if (colorIndx <= 0)
            {
                colorIndx = 0;
                mopaOnly.color = mopaColors[colorIndx];
                //Debug.Log(colorIndx);
                //Debug.Log(mopaColors[colorIndx]);
                //Debug.Log(mopaOnly.color);
            }
            else
            {
                mopaOnly.color = mopaColors[colorIndx];
                //Debug.Log(colorIndx);
            }
            SC.PlayOnce(SoundController.SFX.Limpio);
            Destroy(col.gameObject);
            //Debug.Log("me aclaro");
        }
    }
}
