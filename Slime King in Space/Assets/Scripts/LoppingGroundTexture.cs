using UnityEngine;

public class LoppingGroundTexture : MonoBehaviour
{
    public float groundSpeed;
    public Renderer groundRenderer;

    // Update is called once per frame
    void Update()
    {
        groundRenderer.material.mainTextureOffset += new Vector2(0f, groundSpeed * Time.deltaTime);
    }
}
