Shader "ColorPicker/SaturationValue"
{
    // マテリアルインスペクターで設定可能なプロパティ
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {} 
        _Hue ("Hue", Range(0,1)) = 0          // 色相値（0-1の範囲）
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
        }
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            // URPの基本的なヘルパー関数を含むライブラリ
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // 頂点シェーダーへの入力構造体
            struct Attributes
            {
                float4 positionOS : POSITION;   // オブジェクト空間での頂点位置
                float2 uv : TEXCOORD0;          // テクスチャ座標
            };

            // フラグメントシェーダーへの入力構造体
            struct Varyings
            {
                float2 uv : TEXCOORD0;          // 補間されたUV座標
                float4 positionHCS : SV_POSITION; // クリップ空間での位置
            };

            // マテリアルプロパティ用のバッファ
            CBUFFER_START(UnityPerMaterial)
                float _Hue;    // 色相値
            CBUFFER_END
            
            // 頂点シェーダー
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                // オブジェクト空間からクリップ空間への変換
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }
            
            // HSV色空間からRGB色空間への変換関数
            float3 HSVtoRGB(float3 HSV)
            {
                float C = HSV.z * HSV.y;    // 彩度と明度から色の強さを計算
                float H = fmod(HSV.x, 1.0) * 6.0;  // 色相を0-6の範囲に変換
                float X = C * (1.0 - abs(fmod(H, 2.0) - 1.0));  // 中間の色成分
                float m = HSV.z - C;    // 明度調整用の加算値
                
                // 色相の6つの場合に対応するRGB値
                float3 cases[6] = {
                    float3(C, X, 0),    // 赤から黄へ
                    float3(X, C, 0),    // 黄から緑へ
                    float3(0, C, X),    // 緑から水色へ
                    float3(0, X, C),    // 水色から青へ
                    float3(X, 0, C),    // 青から紫へ
                    float3(C, 0, X)     // 紫から赤へ
                };
                
                int index = (int)H;
                // 配列の範囲外アクセスを防ぐ
                index = clamp(index, 0, 5);
                
                return cases[index] + m;
            }
            
            // フラグメントシェーダー
            float4 frag(Varyings IN) : SV_Target
            {
                // UV座標を使って彩度（x）と明度（y）を設定
                // _Hueは外部から設定された色相値
                float3 hsv = float3(_Hue, IN.uv.x, IN.uv.y);
                float3 rgb = HSVtoRGB(hsv);
                return float4(rgb, 1);  // アルファ値は1（完全不透明）
            }
            ENDHLSL
        }
    }
} 