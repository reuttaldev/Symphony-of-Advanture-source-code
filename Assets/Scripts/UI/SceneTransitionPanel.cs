using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI;
using UnityEngine.Rendering;

// this scripts make the scene transition screen have a transparant inside circle 
public class SceneTransitionPanel : MonoBehaviour
{
    private Image image;
    public float fillValue =0;
    private readonly int parmId = Shader.PropertyToID(name: "_FillValue");
    private void Awake()
    {
        image = GetComponent<Image>();
    }
    private void Update()
    {
        image.materialForRendering.SetFloat(parmId, fillValue);
    }
}
