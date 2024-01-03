using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI;
using UnityEngine.Rendering;

// this scripts make the scene transition screen have a transparant inside circle 
public class SceneTransitionPanel : Image
{
    public override Material materialForRendering
    {
        get
        {
            Material material = new Material(base.materialForRendering);
            material.SetInt("_StencilComp", (int)CompareFunction.NotEqual);
            return material;
        }
    }
}
