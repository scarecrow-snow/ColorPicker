using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using R3;


namespace ColorPicker
{
    /// <summary>
    /// カラーピッカーのUI要素とイベントを管理するクラス
    /// </summary>
    public class ColorPickerUIManager : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("色相（Hue）を選択するスライダー（0-1の範囲）")]
        private Slider hueSlider;

        [SerializeField]
        [Tooltip("色相スライダーの背景となるグラデーション画像")]
        private Image hueSliderBackground;

        [SerializeField]
        [Tooltip("彩度（横軸）と明度（縦軸）を選択するための領域画像")]
        private RawImage saturationValueBackground;
        
        [SerializeField]
        [Tooltip("選択された色を表示するImage")]
        private Image colorPickerImage;

        [SerializeField]
        [Tooltip("選択された色を通知するイベント")]
        private UnityEvent<Color> colorPickedEvent;

        /// <summary>
        /// カラーピッカーのロジックを処理するインスタンス
        /// </summary>
        private ColorPicker colorPicker;

        

        /// <summary>
        /// 初期化処理
        /// </summary>
        private void Start()
        {

            colorPicker = new ColorPicker(
                hueSliderBackground, 
                saturationValueBackground,
                hueSlider.onValueChanged.AsObservable()
            );
        }

        void OnEnable()
        {
            saturationValueBackground.GetComponent<ClickedImage>().OnColorPicked += OnColorPicked;
        }

        void OnDisable()
        {
            saturationValueBackground.GetComponent<ClickedImage>().OnColorPicked -= OnColorPicked;
        }

        /// <summary>
        /// クリックされた色を取得して、colorPickerImageに適用する
        /// </summary>
        /// <param name="color">クリックされた色</param>
        private void OnColorPicked(Color color)
        {
            colorPickerImage.color = color;
            colorPickedEvent?.Invoke(color);
        }

        /// <summary>
        /// クリーンアップ処理
        /// </summary>
        private void OnDestroy()
        {
            if (colorPicker != null)
            {
                colorPicker.Dispose();
                colorPicker = null;
            }
        }
    }
}