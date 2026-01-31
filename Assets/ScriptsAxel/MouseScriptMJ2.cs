using Unity.VisualScripting;
using UnityEngine;

public class MouseScriptMJ2 : MonoBehaviour{

    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); //Guarda la posicion del mouse en mousePos cuando hace click
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero); //En hit se guarda lo que clickeó

                if (hit.collider != null) 
                {
                    if (hit.collider.CompareTag("Crayon")) 
                    {
                        Debug.Log("Tocaste un crayon");
                    }
        }
        }
    }
}

