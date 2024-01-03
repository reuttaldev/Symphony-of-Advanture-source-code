using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI;
using UnityEngine.Rendering;

// this scripts make the scene transition screen have a transparant inside circle 
public class SceneTransitionPanel : MonoBehaviour
{
    private Image image;
    public float circleSize =0;
    private readonly int circleSizeId = Shader.PropertyToID(name: "_circleSize");
    private void Awake()
    {
        image = GetComponent<Image>();
    }
    private void Update()
    {
        image.materialForRendering.SetFloat(circleSizeId, circleSize);
    }
}
