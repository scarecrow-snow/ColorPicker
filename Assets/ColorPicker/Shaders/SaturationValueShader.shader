Shader "ColorPicker/SaturationValue"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Hue ("Hue", Range(0,1)) = 0
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Opaque"
            "Queue" = "Opaque"
        }
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 positionHCS : SV_POSITION;
            };

            CBUFFER_START(UnityPerMaterial)
                float _Hue;
            CBUFFER_END
            
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }
            
            float3 HSVtoRGB(float3 HSV)
            {
                float3 RGB = 0;
                float C = HSV.z * HSV.y;
                float H = fmod(HSV.x, 1.0) * 6.0;
                float X = C * (1.0 - abs(fmod(H, 2.0) - 1.0));
                float m = HSV.z - C;
                
                
                float3 cases[6] = {
                    float3(C, X, 0),
                    float3(X, C, 0),
                    float3(0, C, X),
                    float3(0, X, C),
                    float3(X, 0, C),
                    float3(C, 0, X)
                };
                
                int index = (int)H;
                // 配列のインデックスを安全に取得
                index = clamp(index, 0, 5);
                
                return cases[index] + m;
            }
            
            float4 frag(Varyings IN) : SV_Target
            {
                // UIの座標系に合わせる（上が0、下が1）
                float3 hsv = float3(_Hue, IN.uv.x, IN.uv.y);
                float3 rgb = HSVtoRGB(hsv);
                return float4(rgb, 1);
            }
            ENDHLSL
        }
    }
} 