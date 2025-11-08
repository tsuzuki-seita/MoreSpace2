using UnityEngine;

[ExecuteAlways] // シーンビュー上でも動かしたいなら付ける。再生中だけで良ければ外してOK
public class SkyboxStarScroller : MonoBehaviour
{
    [SerializeField] private float speedX = 0.01f; // 横方向の速さ（小さくするとゆっくり）
    [SerializeField] private float speedY = 0.00f; // 縦方向に流したければここも設定

    private float _offsetX;
    private float _offsetY;

    private void Update()
    {
        if (RenderSettings.skybox == null) return;

        _offsetX += speedX * Time.deltaTime;
        _offsetY += speedY * Time.deltaTime;

        // Unlit/Texture の _MainTex のオフセットを更新
        RenderSettings.skybox.SetTextureOffset("_MainTex", new Vector2(_offsetX, _offsetY));
    }
}
