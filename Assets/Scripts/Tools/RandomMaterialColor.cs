using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomMaterialColor : MonoBehaviour
{
    public Renderer renderer;
    public string materialProperty = "_Color";
    public Material[] onlyOnSpecificMaterials;

    public Color[] palette = {
        Color.white,
        Color.red,
        Color.green,
        Color.yellow,
        Color.blue,
    };

    private void Awake()
    {
        var block = new MaterialPropertyBlock();
        var colorId = Random.Range(0, palette.Length);
        block.SetColor(materialProperty, palette[colorId]);
        
        if (onlyOnSpecificMaterials != null)
        {
            var materialSet = new HashSet<Material>(onlyOnSpecificMaterials);
            
            for (var i = 0; i < renderer.sharedMaterials.Length; i++)
            {
                if (materialSet.Contains(renderer.sharedMaterials[i]))
                    renderer.SetPropertyBlock(block, i);
            }
        }
        else
        {
            renderer.SetPropertyBlock(block);
        }
    }
}
