using UnityEngine;
using System;

namespace ColorPicker.Core
{
    /// <summary>
    /// カラーピッカーのシェーダー制御クラス
    /// 彩度・明度選択用のシェーダーマテリアルを管理します
    /// </summary>
    public class ColorPickerShaderController : IDisposable
    {
        /// <summary>彩度と明度の表示に使用するマテリアル</summary>
        private readonly Material saturationValueMaterial;
        /// <summary>シェーダーの色相プロパティのID</summary>
        private static readonly int HuePropertyID = Shader.PropertyToID("_Hue");
        
        /// <summary>
        /// シェーダーコントローラーを初期化します
        /// </summary>
        /// <param name="shader">使用するシェーダー</param>
        /// <exception cref="ArgumentNullException">シェーダーがnullの場合にスローされます</exception>
        public ColorPickerShaderController(Shader shader)
        {
            if (shader == null)
                throw new ArgumentNullException(nameof(shader));
            
            saturationValueMaterial = new Material(shader);
        }
        
        /// <summary>シェーダーの色相値を更新します</summary>
        public void UpdateHue(float hue)
        {
            saturationValueMaterial.SetFloat(HuePropertyID, hue);
        }
        
        /// <summary>シェーダーマテリアルを取得します</summary>
        public Material GetMaterial() => saturationValueMaterial;

        public void Dispose()
        {
            if (saturationValueMaterial != null)
            {
                UnityEngine.Object.Destroy(saturationValueMaterial);
            }
        }
    }
}