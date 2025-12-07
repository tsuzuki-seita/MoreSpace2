Shader "Custom/Wireframe_Front"
{
    Properties
    {
        _BaseColor     ("Visible Color", Color) = (1, 0, 0, 0.3)
        _XRayColor     ("X-Ray Color",  Color) = (0, 1, 0, 1.0)
        _OcclusionBias ("Occlusion Bias (world units)", Float) = 0.05
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalRenderPipeline"
            "RenderType"     = "Transparent"
            "Queue"          = "Transparent+10"
        }

        LOD 100

        Pass
        {
            Name "FORWARD"
            Tags { "LightMode" = "UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            Cull  Back
            ZWrite Off        // 自分では深度を書かない
            ZTest Always      // occlusion 判定は DepthTexture で自前判定

            HLSLPROGRAM
            #pragma vertex   vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float4 screenPos   : TEXCOORD0;
                float  viewDepth   : TEXCOORD1; // カメラからの距離
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _XRayColor;
                float  _OcclusionBias;
            CBUFFER_END

            Varyings vert (Attributes IN)
            {
                Varyings OUT;

                // ワールド → ビュー空間で「カメラからの距離」を取る
                float3 positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                float3 positionVS = TransformWorldToView(positionWS);
                // Unity ではカメラは -Z 向きなので、距離は -z
                OUT.viewDepth = -positionVS.z;

                // クリップ座標・スクリーン座標
                float4 posHCS = TransformWorldToHClip(positionWS);
                OUT.positionHCS = posHCS;
                OUT.screenPos   = ComputeScreenPos(posHCS);

                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                // このピクセルのスクリーンUV
                float2 uv = IN.screenPos.xy / IN.screenPos.w;

                // シーン中「一番手前のオブジェクト」の深度（DepthTexture）を取得
                float sceneRawDepth  = SampleSceneDepth(uv);
                float sceneEyeDepth  = LinearEyeDepth(sceneRawDepth, _ZBufferParams); // カメラからの距離

                // 自分（Wireframe_Front）の深さ（カメラからの距離）
                float objectDepth = IN.viewDepth;

                // 「シーンの手前オブジェクトより _OcclusionBias 以上奥」にあれば occluded
                //   objectDepth > sceneEyeDepth + bias  → 奥（緑）
                //   そうでなければ手前 or ほぼ同じ位置（赤）
                float isOccluded = step(sceneEyeDepth + _OcclusionBias, objectDepth);

                half4 col = lerp(_BaseColor, _XRayColor, isOccluded);
                return col;
            }
            ENDHLSL
        }
    }

    FallBack Off
}
