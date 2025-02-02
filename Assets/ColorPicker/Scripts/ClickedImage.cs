using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ColorPicker
{
    /// <summary>
    /// RawImageをクリックした位置の色を取得し、指定されたImageの色として設定するコンポーネント
    /// </summary>
    public class ClickedImage : MonoBehaviour, IPointerClickHandler
    {
        public event Action<Color> OnColorPicked;
        
        /// <summary>
        /// クリックイベントの処理
        /// クリックされた位置のピクセルの色を取得し、targetImageに適用します
        /// </summary>
        /// <param name="eventData">クリックイベントのデータ</param>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.pointerCurrentRaycast.gameObject.TryGetComponent(out RawImage clickedImage))
            {
                Texture2D texture = clickedImage.texture as Texture2D;
                if (texture == null) return;

                // クリック位置をRectTransformのローカル座標に変換
                Vector2 localPoint;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    clickedImage.rectTransform,
                    eventData.position,
                    eventData.pressEventCamera,
                    out localPoint
                );

                // ローカル座標を0-1の範囲に正規化
                Rect rect = clickedImage.rectTransform.rect;
                float normalizedX = (localPoint.x - rect.x) / rect.width;
                float normalizedY = (localPoint.y - rect.y) / rect.height;

                // 正規化された座標をテクスチャのピクセル座標に変換
                int pixelX = Mathf.Clamp(Mathf.FloorToInt(normalizedX * texture.width), 0, texture.width - 1);
                int pixelY = Mathf.Clamp(Mathf.FloorToInt(normalizedY * texture.height), 0, texture.height - 1);

                // ピクセルの色を取得し、ターゲットに適用
                Color pixelColor = texture.GetPixel(pixelX, pixelY);

                OnColorPicked?.Invoke(pixelColor);      

                /*
                Debug.Log($"Clicked position - Local: ({localPoint.x}, {localPoint.y}), " +
                         $"Normalized: ({normalizedX}, {normalizedY}), " +
                         $"Pixel: ({pixelX}, {pixelY}), " +
                         $"Color: {pixelColor}");
                */
            }
        }
    }
}
