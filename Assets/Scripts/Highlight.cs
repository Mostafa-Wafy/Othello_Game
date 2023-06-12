using UnityEngine;

public class Highlight : MonoBehaviour
{

    [SerializeField]
    private Color normalColour;

    [SerializeField]
    private Color mouseOverColour;

    private Material mat;


    private void Start()
    {
        mat = GetComponent<MeshRenderer>().mat;
        mat.color = normalColour;
    }

    private void OnMouseEnter() {
        mat.color = mouseOverColour; 
    }

    private void OnMouseExit() {
        mat.color = normalColour;
    }

    private void OnDestroy() {
        Destroy(mat);
    }
}
