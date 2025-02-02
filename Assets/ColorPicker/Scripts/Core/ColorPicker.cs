using System;
using UnityEngine;
using UnityEngine.UI;
using R3;

namespace ColorPicker
{
    /// <summary>
    /// カラーピッカーの主要なロジックを処理するクラス
    /// </summary>
    public class ColorPicker : IDisposable
    {
        // 全インスタンス間で共有するリソース
        private static Sprite hueSliderBackgroundSprite;
        private static Texture2D hueTexture;
        private static int instanceCount = 0;

        // UI要素への参照
        private Image hueSliderBackground;
        private RawImage saturationValueBackground;

        // グラデーションテクスチャのサイズ定数
        private const int GradientTextureWidth = 256;
        private const int GradientTextureHeight = 256;

        // 現在使用中のテクスチャ
        private Texture2D saturationValueTexture;

        // リアクティブストリームの購読管理
        private IDisposable disposable;


        /// <summary>
        /// カラーピッカーを初期化します
        /// </summary>
        /// <param name="hueSliderBackground">色相スライダーの背景画像</param>
        /// <param name="saturationValueBackground">彩度と明度の選択領域の背景画像</param>
        /// <param name="hueObservable">色相値の変更を通知するObservable</param>
        /// <param name="poolSize">テクスチャプールの初期サイズ</param>
        public ColorPicker(Image hueSliderBackground, RawImage saturationValueBackground, Observable<float> hueObservable)
        {
            instanceCount++;
            this.hueSliderBackground = hueSliderBackground;
            this.saturationValueBackground = saturationValueBackground;
            
            CreateHueSliderBackground();
            
            saturationValueTexture = new Texture2D(GradientTextureWidth, GradientTextureHeight);
            this.saturationValueBackground.texture = saturationValueTexture;
            ApplySaturationValueGradient(saturationValueTexture, 0);
            
            // 色相値の変更を監視し、50ms以内の連続した変更をスキップして最後の値のみを処理
            disposable = hueObservable
                    .ThrottleFirstLast(TimeSpan.FromMilliseconds(50))
                    .Subscribe(value => UpdateSaturationValueBackground(value));
        }

        /// <summary>
        /// 色相スライダーの背景グラデーションを生成します
        /// 静的リソースは一度だけ生成され、全インスタンスで共有されます
        /// </summary>
        private void CreateHueSliderBackground()
        {
            if(hueSliderBackgroundSprite == null)
            {
                hueTexture = new Texture2D(GradientTextureWidth, 1);
                ApplyHueGradient(hueTexture);
                hueSliderBackgroundSprite = Sprite.Create(hueTexture, new Rect(0, 0, GradientTextureWidth, 1), new Vector2(0.5f, 0.5f));
            }

            hueSliderBackground.sprite = hueSliderBackgroundSprite;
        }

        /// <summary>
        /// 指定された色相値に基づいて彩度と明度の選択領域を更新します
        /// </summary>
        /// <param name="hue">新しい色相値（0-1）</param>
        private void UpdateSaturationValueBackground(float hue)
        {
            ApplySaturationValueGradient(saturationValueTexture, hue);
        }

        /// <summary>
        /// 色相グラデーションをテクスチャに適用します
        /// 横方向に赤（0）から紫（1）までのグラデーションを生成します
        /// </summary>
        private void ApplyHueGradient(Texture2D texture)
        {
            for (int i = 0; i < GradientTextureWidth; i++)
            {
                Color color = Color.HSVToRGB(i / (float)GradientTextureWidth, 1f, 1f);
                texture.SetPixel(i, 0, color);
            }
            texture.Apply();
        }

        /// <summary>
        /// 指定された色相値に基づいて、彩度と明度のグラデーションを生成します
        /// 横軸：彩度（0-1）、縦軸：明度（0-1）
        /// </summary>
        private void ApplySaturationValueGradient(Texture2D texture, float hue)
        {
            for (int y = 0; y < GradientTextureHeight; y++)
            {
                for (int x = 0; x < GradientTextureWidth; x++)
                {
                    float saturation = x / (float)GradientTextureWidth;
                    float value = y / (float)GradientTextureHeight;
                    Color color = Color.HSVToRGB(hue, saturation, value);
                    texture.SetPixel(x, y, color);
                }
            }
            texture.Apply();
        }

        /// <summary>
        /// リソースを解放します
        /// 最後のインスタンスが破棄される際に静的リソースも解放されます
        /// </summary>
        public void Dispose()
        {
            instanceCount--;

            if (instanceCount == 0)
            {
                if (hueSliderBackgroundSprite != null)
                {
                    UnityEngine.Object.Destroy(hueSliderBackgroundSprite);
                    hueSliderBackgroundSprite = null;
                }
                
                if (hueTexture != null)
                {
                    UnityEngine.Object.Destroy(hueTexture);
                    hueTexture = null;
                }
            }

            UnityEngine.Object.Destroy(saturationValueTexture);
            saturationValueTexture = null;
            
            disposable?.Dispose();
            disposable = null;
            
            hueSliderBackground = null;
            saturationValueBackground = null;
        }
    }
}

