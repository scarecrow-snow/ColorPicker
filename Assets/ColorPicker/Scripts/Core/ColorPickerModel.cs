using UnityEngine;
using R3;

namespace ColorPicker.Core
{
    /// <summary>
    /// カラーピッカーのModel層のインターフェース
    /// 色情報の状態管理を定義します
    /// </summary>
    public interface IColorPickerModel
    {
        /// <summary>色相値（0-1）</summary>
        float Hue { get; set; }
        
        /// <summary>彩度値（0-1）</summary>
        float Saturation { get; set; }
        
        /// <summary>明度値（0-1）</summary>
        float Value { get; set; }
        
        /// <summary>現在選択されている色</summary>
        Color CurrentColor { get; }
        
        /// <summary>色の変更を通知するObservable</summary>
        Observable<Color> ColorChanged { get; }
        
        /// <summary>HSV値を更新します</summary>
        void UpdateColor(float h, float s, float v);
    }

    /// <summary>
    /// カラーピッカーのModel層の実装
    /// HSV色空間での色情報を管理します
    /// </summary>
    public class ColorPickerModel : IColorPickerModel
    {
        private readonly ReactiveProperty<Color> colorChanged = new();
        
        public float Hue { get; set; }
        public float Saturation { get; set; }
        public float Value { get; set; }
        
        public Color CurrentColor => Color.HSVToRGB(Hue, Saturation, Value);
        
        public Observable<Color> ColorChanged => colorChanged;
        
        public void UpdateColor(float h, float s, float v)
        {
            Hue = h;
            Saturation = s;
            Value = v;
            colorChanged.Value = CurrentColor;
        }
    }
} 