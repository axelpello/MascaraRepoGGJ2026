using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class MouseScriptMJ2 : MonoBehaviour
{
    private MainGM mouseScript;
    private RaycastHit2D hit;
    private Vector2 mousePos;
    private bool crayonSeleccionado = false;
    void Start()
    {
        mouseScript = GameObject.Find("Mouse").GetComponent<MainGM>();
        mouseScript.enabled = false;
        Debug.Log("ESTOY CORRIENDO -SCRIPT MINIJUEGO 2-");
    }
    void Update()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos = new Vector2(worldPos.x, worldPos.y);

        if (Input.GetMouseButtonDown(0))
        {
            if (!crayonSeleccionado)
            {
                // Si NO tengo nada, intento agarrar
                hit = Physics2D.Raycast(mousePos, Vector2.zero);

                if (hit.collider != null && hit.collider.CompareTag("Crayon"))
                {
                    crayonSeleccionado = true;
                    Debug.Log("Crayón agarrado");
                }
            }
            else
            {
                // Si YA tenía algo, lo suelto
                crayonSeleccionado = false;
                Debug.Log("Crayón soltado");
            }
        }

        if (crayonSeleccionado && hit.collider != null)
        {
            hit.collider.gameObject.transform.position = new Vector3(mousePos.x, mousePos.y, 0);
        }
    }
}


