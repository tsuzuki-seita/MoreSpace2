Shader "UI/Noise_UI"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        
        // UIマスク用プロパティ（必須）
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _WriteMask ("Stencil Write Mask", Float) = 255
        _ReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15

        // ノイズ用パラメータ
        _HorizonValue ("Horizon Value", Range(0, 1)) = 0
        _Seed ("Seed", Int) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_ReadMask]
            WriteMask [_WriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            fixed4 _Color;
            float4 _ClipRect;
            float _HorizonValue;
            int _Seed;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
                OUT.texcoord = v.texcoord;
                OUT.color = v.color * _Color;
                return OUT;
            }

            // 乱数生成関数
            float rnd(float2 value, int Seed)
            {
                return frac(sin(dot(value.xy, float2(12.9898, 78.233)) + Seed) * 43758.5453);
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                // ノイズ計算
                float rndValue = rnd(IN.texcoord, _Seed);
                int tmp = step(rndValue, 0.5) * 2 - 1;
                float rndU = _HorizonValue * tmp * rndValue;

                // UVをずらす
                float2 uv = float2(frac(IN.texcoord.x + rndU), IN.texcoord.y);

                // テクスチャサンプリング
                half4 color = tex2D(_MainTex, uv) * IN.color;
                color.rgb += float3(rndU, rndU, rndU) * 1.5;

                // UIのマスク対応（RectMask2Dなど）
                #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif

                // アルファが低すぎる場合は描画しない（お好みで）
                clip (color.a - 0.001);

                return color;
            }
            ENDCG
        }
    }
}