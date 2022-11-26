using UnityEngine;
using Random = UnityEngine.Random;

public class RandomMaterialColor : MonoBehaviour
{
    public Renderer renderer;
    public string materialProperty = "_Color";

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
        renderer.SetPropertyBlock(block);
    }
}
