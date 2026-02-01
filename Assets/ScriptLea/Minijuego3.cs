using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Minijuego3 : MonoBehaviour
{
    private MainGM mouseScript;
    /* private MainGM mouseScript;
    private bool isMopaSelected = false;
    private GameObject mopa;
   void Start()
    {
        mouseScript = GameObject.Find("Mouse").GetComponent<MainGM>();
        mouseScript.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            //Debug.Log(hit.collider.gameObject.name);
            if (hit.collider != null)
            {
                Debug.Log(hit.collider.gameObject.transform.name);
                if (hit.collider.gameObject.name == "Mopa")
                {
                    Debug.Log("SADDASADSDASDS");
                    mopa = hit.collider.gameObject;
                    isMopaSelected = true;
                }
                //Debug.Log("Click sobre: " + hit.collider.name);
            }
        }

        if (isMopaSelected)
        {
            mopa.transform.localPosition = mouseScript.transform.localPosition;
        }
    }*/
    private GameObject mopa;
    public GameObject BG;
    public List<Sprite> BGAnim;
    private bool isMopaSelected = false;
    public GameObject basura;
    public List<Sprite> basuraSprite;
    public GameObject limpieza;
    public List<Sprite> limpiezaSprite;
    public List<GameObject> positions;
    public List<GameObject> scalePoints;
    public bool isSpawnin = true;
    Vector2 mousePos;
    IEnumerator BasuraDrop()
    {
        while (isSpawnin)
        {
            int rnd = Random.Range(0, 100);
            GameObject instancia;
            if (rnd > 85)
            {
                rnd = Random.Range(0, positions.Count());

                int toNumber = int.Parse(positions[rnd].name);
                int negToNumber = -int.Parse(positions[rnd].name);
                Quaternion rot = Quaternion.Euler(0f, 0f, toNumber);
                //Debug.Log(toNumber);
                instancia = Instantiate(basura, positions[rnd].transform.position, rot);
                instancia.transform.GetChild(0).transform.localEulerAngles = new Vector3(0f, 0f, negToNumber);
                //basura.transform.SetParent(transform);
                rnd = Random.Range(0, basuraSprite.Count());
                instancia.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = basuraSprite[rnd];
            }
            else
            {
                rnd = Random.Range(0, positions.Count());

                int toNumber = int.Parse(positions[rnd].name);
                int negToNumber = -int.Parse(positions[rnd].name);
                Quaternion rot = Quaternion.Euler(0f, 0f, toNumber);
                //Debug.Log(toNumber);
                instancia = Instantiate(limpieza, positions[rnd].transform.position, rot);
                instancia.transform.GetChild(0).transform.localEulerAngles = new Vector3(0f, 0f, negToNumber);

                //limpieza.transform.SetParent(transform);
                rnd = Random.Range(0, limpiezaSprite.Count());
                instancia.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = limpiezaSprite[rnd];
            }
            yield return new WaitForSeconds(.1f);
        }
    }

    IEnumerator BgAnim()
    {
        while (true)
        {
            for (int i = 0; i < BGAnim.Count(); i++)
            {
                BG.GetComponent<SpriteRenderer>().sprite = BGAnim[i];
                yield return new WaitForSeconds(.1f);
            }
            yield return null;
        }
    }


    void Start()
    {
        mouseScript = GameObject.Find("Mouse").GetComponent<MainGM>();
        mouseScript.enabled = false;
        StartCoroutine(BgAnim());
        StartCoroutine(BasuraDrop());
        //drop();
    }

    void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonDown(0) && !isMopaSelected)
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider.name == "Mopa")
            {
                mopa = hit.collider.gameObject;
                isMopaSelected = true;
            }
        }
        if (isMopaSelected)
        {
            Vector3 pos = mopa.transform.position;
            pos.x = mousePos.x;
            mopa.transform.position = pos;

        }
    }


}
