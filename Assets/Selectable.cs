using UnityEngine;
using UnityEditor;

public class Selectable : MonoBehaviour
{
    GameObject GM;
    private void Start()
    {
        GM = GameObject.FindGameObjectWithTag("GM");
    }

    private void OnMouseDown()
    {
        if (GM.GetComponent<GameManager>().Status == Mode.TargetMode)
            GM.GetComponent<GameManager>().TargetObj = gameObject;
    }
}