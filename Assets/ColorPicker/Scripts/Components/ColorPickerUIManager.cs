using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using R3;
using R3.Triggers;
using UnityEngine.EventSystems;

using ColorPicker.Core;
using System;


namespace ColorPicker.Components
{
    /// <summary>
    /// カラーピッカーのView層の実装
    /// UIコンポーネントとの相互作用を管理します
    /// </summary>
    public class ColorPickerUIManager : MonoBehaviour, IColorPickerView
    {
        /// <summary>色相を選択するスライダー</summary>
        [SerializeField] private Slider hueSlider;
        /// <summary>色相スライダーの背景画像</summary>
        [SerializeField] private Image hueSliderBackground;
        /// <summary>彩度と明度を選択する領域の背景画像</summary>
        [SerializeField] private Image saturationValueBackground;
        /// <summary>選択された色を表示する画像</summary>
        [SerializeField] private Image colorPickerImage;
        /// <summary>彩度と明度の選択位置を示すポインター</summary>
        [SerializeField] private RectTransform colorPickerPointer;
        /// <summary>彩度と明度の表示に使用するシェーダー</summary>
        [SerializeField] private Shader saturationValueShader;
        /// <summary>色が選択されたときに発火するイベント</summary>
        public UnityEvent<Color> colorPickedEvent;

        /// <summary>MVPパターンのPresenter</summary>
        private ColorPickerPresenter presenter;
        /// <summary>シェーダーの制御を担当するコントローラー</summary>
        private ColorPickerShaderController shaderController;
        /// <summary>色相の変更を通知するReactiveProperty</summary>
        private ReactiveProperty<float> hueChangedProperty = new(0f);
        /// <summary>彩度と明度の変更を通知するReactiveProperty</summary>
        private ReactiveProperty<Vector2> saturationValueChangedProperty = new(new Vector2(0.5f, 0.5f));

        /// <summary>グラデーションテクスチャの幅</summary>
        private const int GradientTextureWidth = 256;
        /// <summary>グラデーションテクスチャの高さ</summary>
        private const int GradientTextureHeight = 1;
        /// <summary>連続的な値の変更を間引く時間（ミリ秒）</summary>
        private const int ThrottleDelayMilliseconds = 50;

        // IColorPickerViewの実装
        public Observable<float> OnHueChanged => hueChangedProperty;
        public Observable<Vector2> OnSaturationValueChanged => saturationValueChangedProperty;

        // 全インスタンス間で共有するリソース
        private static Sprite hueSliderBackgroundSprite;
        private static Texture2D hueTexture;
        private static int instanceCount = 0;

        private void Start()
        {
            instanceCount++;
            CreateHueSliderBackground();
            
            // 各コンポーネントの初期化
            shaderController = new ColorPickerShaderController(saturationValueShader);
            saturationValueBackground.material = shaderController.GetMaterial();

            // MVPパターンの構築
            var model = new ColorPickerModel();
            presenter = new ColorPickerPresenter(model, this);

            // スライダーのイベントを購読
            hueSlider.onValueChanged
                .AsObservable()
                .ThrottleFirstLast(TimeSpan.FromMilliseconds(ThrottleDelayMilliseconds))
                .Subscribe(OnHueSliderChanged)
                .AddTo(this);

            // 彩度・明度の選択エリアのクリックイベントを購読
            saturationValueBackground
                .OnPointerClickAsObservable()
                .Subscribe(eventData => HandleSaturationValueClick(eventData))
                .AddTo(this);

            // 初期値の設定
            if (hueSlider != null)
            {
                OnHueSliderChanged(hueSlider.value);
            }
        }


        /// <summary>
        /// 色相スライダーの値が変更されたときの処理
        /// </summary>
        /// <param name="value">新しい色相値（0-1）</param>
        private void OnHueSliderChanged(float value)
        {
            hueChangedProperty.Value = value;
            UpdateHueShader(value);
        }

        /// <summary>
        /// 選択された色をUIに反映し、イベントを発火します
        /// </summary>
        /// <param name="color">選択された色</param>
        public void UpdateColorDisplay(Color color)
        {
            if (colorPickerImage != null)
            {
                colorPickerImage.color = color;
                colorPickedEvent?.Invoke(color);
            }
        }

        /// <summary>
        /// シェーダーの色相値を更新します
        /// </summary>
        /// <param name="hue">新しい色相値（0-1）</param>
        public void UpdateHueShader(float hue)
        {
            shaderController?.UpdateHue(hue);
        }

        /// <summary>
        /// 彩度・明度の選択エリアがクリックされたときの処理
        /// </summary>
        private void HandleSaturationValueClick(PointerEventData eventData)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                saturationValueBackground.rectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 localPoint))
            {
                // ローカル座標を0-1の範囲に正規化
                Rect rect = saturationValueBackground.rectTransform.rect;
                float saturation = Mathf.Clamp01((localPoint.x - rect.x) / rect.width);
                float value = Mathf.Clamp01((localPoint.y - rect.y) / rect.height);

                var saturationValue = new Vector2(saturation, value);
                saturationValueChangedProperty.Value = saturationValue;
                UpdatePointerPosition(localPoint);
            }
        }

        /// <summary>
        /// ポインターの位置を更新します
        /// </summary>
        private void UpdatePointerPosition(Vector2 localPoint)
        {
            if (colorPickerPointer == null) return;
            colorPickerPointer.localPosition = localPoint;
        }

        /// <summary>
        /// 色相スライダーの背景グラデーションを生成します
        /// 静的リソースは一度だけ生成され、全インスタンスで共有されます
        /// </summary>
        private void CreateHueSliderBackground()
        {
            if(hueSliderBackgroundSprite == null)
            {
                hueTexture = new Texture2D(GradientTextureWidth, GradientTextureHeight);
                ApplyHueGradient(hueTexture);
                hueSliderBackgroundSprite = Sprite.Create(
                    hueTexture, 
                    new Rect(0, 0, GradientTextureWidth, GradientTextureHeight), 
                    new Vector2(0.5f, 0.5f)
                );
            }
            hueSliderBackground.sprite = hueSliderBackgroundSprite;
        }

        /// <summary>
        /// 色相グラデーションをテクスチャに適用します
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

        private void OnDestroy()
        {
            instanceCount--;
            
            // 最後のインスタンスが破棄されるときのみ静的リソースを解放
            if (instanceCount == 0)
            {
                if (hueSliderBackgroundSprite != null)
                {
                    Destroy(hueSliderBackgroundSprite);
                    hueSliderBackgroundSprite = null;
                }
                
                if (hueTexture != null)
                {
                    Destroy(hueTexture);
                    hueTexture = null;
                }
            }

            presenter?.Dispose();
            shaderController?.Dispose();
            hueChangedProperty?.Dispose();
            saturationValueChangedProperty?.Dispose();
        }
    }
}