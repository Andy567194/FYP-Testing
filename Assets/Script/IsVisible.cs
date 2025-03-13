using UnityEngine;

public class IsVisible : MonoBehaviour
{
    Renderer m_Renderer;
    // Use this for initialization
    public bool isVisible;
    void Start()
    {
        m_Renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Renderer.isVisible)
        {
            isVisible = true;
        }
        else isVisible = false;
    }
}