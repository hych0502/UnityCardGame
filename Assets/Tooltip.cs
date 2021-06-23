using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
    public TextMesh title;
    public TextMesh effect;
    public TextMesh costnumber;
    public TextMesh Attack;
    public TextMesh HP;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(-4, 3, 0);
    }

    // Update is called once per frame                      
    void Update()
    {
   
    }
}
