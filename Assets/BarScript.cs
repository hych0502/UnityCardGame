using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarScript : MonoBehaviour
{
    public TextMesh Cardname;
    public TextMesh Cost;
    public TextMesh CardTotal;
    public int Index;
    GameObject Content;
    GameObject Deckcons;
    Vector3 defpos;
    // Start is called before the first frame update
    void Start()
    {
        Deckcons = GameObject.FindGameObjectWithTag("Deck");
        transform.localPosition += new Vector3(0,0,-5);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnMouseUp()
    {
        RaycastHit hit = new RaycastHit();

        transform.position = defpos;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray.origin, ray.direction, out hit))
        {
            if (hit.transform.gameObject.name == "DeckBoard")
            {
                return;
            }

        }
        Deckcons.GetComponent<DeckCons>().CardList.Remove(Index);
        Deckcons.GetComponent<DeckCons>().ShowCardList();
        //transform.position = defpos;
    }
    private void OnMouseDrag()
    {
        Vector2 currentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = currentPos;
        transform.Translate(new Vector3(0, 0, -1));

    }
    private void OnMouseDown()
    {
        defpos = transform.position;

    }
}
