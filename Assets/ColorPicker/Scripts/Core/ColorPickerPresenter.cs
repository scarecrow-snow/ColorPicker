using System;
using R3;


namespace ColorPicker.Core
{
    /// <summary>
    /// カラーピッカーのPresenter層
    /// ModelとViewの間の相互作用を管理します
    /// </summary>
    public class ColorPickerPresenter : IDisposable
    {
        private readonly IColorPickerModel model;
        private readonly IColorPickerView view;
        private readonly IDisposable disposable;

        public ColorPickerPresenter(IColorPickerModel model, IColorPickerView view)
        {
            this.model = model;
            this.view = view;
            
            // 複数のSubscriptionをまとめて管理
            var subscriptions = new[]
            {
                // ViewからのイベントをModelに伝播
                view.OnHueChanged
                    .Subscribe(h => model.UpdateColor(h, model.Saturation, model.Value)),
                    
                view.OnSaturationValueChanged
                    .Subscribe(sv => model.UpdateColor(model.Hue, sv.x, sv.y)),
                    
                // ModelからViewへの更新
                model.ColorChanged
                    .Subscribe(color => view.UpdateColorDisplay(color))
            };
            
            // 複数のSubscriptionを1つのIDisposableにまとめる
            disposable = new CompositeDisposable(subscriptions);
        }

        public void Dispose()
        {
            disposable?.Dispose();
        }
    }
} 