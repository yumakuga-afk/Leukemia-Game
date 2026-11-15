using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
    protected Material mat;

    protected virtual void Start()
    {
        mat = GetComponent<Renderer>().material;
        HideHighlight();
    }

    public void ShowHighlight()
    {
        if(mat !=null)
            mat.color = new Color(mat.color.r,mat.color.g,mat.color.b, 1f);
    }

    public void HideHighlight()
    {
        if(mat != null)
        {
            // Debug.Log("work");
             mat.color = new Color(mat.color.r,mat.color.g,mat.color.b, 0f);
        }
    }

    public abstract void Interact();
}