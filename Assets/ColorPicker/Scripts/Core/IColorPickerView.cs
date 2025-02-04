using UnityEngine;
using R3;

namespace ColorPicker.Core
{
    /// <summary>
    /// カラーピッカーのView層のインターフェース
    /// UIとの相互作用を定義します
    /// </summary>
    public interface IColorPickerView
    {
        /// <summary>色相値の変更を通知するObservable</summary>
        Observable<float> OnHueChanged { get; }
        
        /// <summary>彩度(x)と明度(y)の変更を通知するObservable</summary>
        Observable<Vector2> OnSaturationValueChanged { get; }
        
        /// <summary>選択された色をUIに反映します</summary>
        void UpdateColorDisplay(Color color);
        
        /// <summary>色相値の変更をシェーダーに反映します</summary>
        void UpdateHueShader(float hue);
    }
} 