using Unity.VisualScripting;
using UnityEngine;

public class MainGM : MonoBehaviour
{
    [SerializeField]
    GameObject[] MiniJuegos = new GameObject[3];
    public GameObject selectorLevel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Hola");
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); //Guarda la posicion del mouse en mousePos cuando hace click
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero); //En hit se guarda lo que clickeó
            Debug.Log("Click detectado!");

            if (hit.collider != null) //Si toque algo
            {
                Debug.Log("Click sobre: " + hit.collider.name + " | Layer: " + hit.collider.gameObject.layer + " | Tag: " + hit.collider.tag);

                if (hit.collider.gameObject.layer == 3) //El layer 3 es "minijuegos".
                {
                    Debug.Log("Layer correcto (3) detectado!");
                    Debug.Log("Tag detectado: '" + hit.collider.tag + "'");

                    switch (hit.collider.tag)
                    {
                        case "0":
                            Debug.Log("Case 0 alcanzado!");
                            if (MiniJuegos[0] != null)
                            {
                                MiniJuegos[0].SetActive(true);
                                Debug.Log("Juego 0 activado");
                            }
                            else
                            {
                                Debug.LogError("MiniJuegos[0] es NULL! Asignalo en el Inspector");
                            }
                            break;
                        case "1":
                            Debug.Log("Case 1 alcanzado!");
                            if (MiniJuegos[1] != null)
                            {
                                MiniJuegos[1].SetActive(true);
                                Debug.Log("Juego 1 activado");
                            }
                            else
                            {
                                Debug.LogError("MiniJuegos[1] es NULL! Asignalo en el Inspector");
                            }
                            break;
                        case "2":
                            Debug.Log("Case 2 alcanzado!");
                            if (MiniJuegos[2] != null)
                            {
                                MiniJuegos[2].SetActive(true);
                                Debug.Log("Juego 2 activado");
                            }
                            else
                            {
                                Debug.LogError("MiniJuegos[2] es NULL! Asignalo en el Inspector");
                            }
                            break;
                        default:
                            Debug.LogWarning("Tag no reconocido: '" + hit.collider.tag + "' - No coincide con ningun case");
                            break;
                    }
                    selectorLevel.transform.gameObject.SetActive(false);
                }
                //Debug.Log("Click sobre: " + hit.collider.name);
            }
        }
    }
}
