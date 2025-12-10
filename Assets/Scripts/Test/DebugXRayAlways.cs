using UnityEngine;

public class DebugXRayAlways : MonoBehaviour
{
    [SerializeField] private Renderer wireRenderer;

    private static readonly int BaseColorID = Shader.PropertyToID("_BaseColor");
    private static readonly int XRayColorID = Shader.PropertyToID("_XRayColor");

    private void Reset()
    {
        if (wireRenderer == null)
            wireRenderer = GetComponent<Renderer>();
    }

    private void Start()
    {
        var mat = wireRenderer.material;

        // デバッグ用：とりあえず常にド派手に表示させる
        mat.SetColor(BaseColorID, new Color(1f, 0f, 0f, 0.1f));  // 手前ほぼ透明
        mat.SetColor(XRayColorID, new Color(0f, 1f, 0f, 1f));    // 奥側はネオングリーン

        //wireRenderer.enabled = true;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.X))
        {
            wireRenderer.enabled = !wireRenderer.enabled;
        }
    }

}
