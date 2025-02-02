# Unity Color Picker

Unity用のシンプルで使いやすいカラーピッカーコンポーネントです。HSV（色相・彩度・明度）モデルを使用して直感的な色の選択が可能です。

![カラーピッカーのスクリーンショット]()  <!-- スクリーンショットを追加することをお勧めします -->

## 特徴

- HSVカラーモデルによる直感的な色選択
- 色相スライダーと彩度/明度選択エリアの2段階選択
- UnityEventを使用した柔軟なカラー変更通知
- 最適化されたテクスチャ生成とメモリ管理
- Unity UIシステムとの完全な互換性

## 必要環境

- Unity 6 6000.0.29f1 にて作成
- [R3（UniRx互換ライブラリ）](https://github.com/Cysharp/R3)

## インストール方法

1. このリポジトリをクローンまたはダウンロード
2. `Assets/ColorPicker` フォルダをUnityプロジェクトの `Assets` フォルダにコピー
3. R3をプロジェクトにインストール（パッケージマネージャーまたは手動で）

## 使用方法

### 基本的なセットアップ

1. シーンにカラーピッカーのプレハブをドラッグ＆ドロップ
2. ColorPickerUIManagerコンポーネントで必要なUI要素を設定:
   - Hue Slider: 色相選択用のスライダー
   - Hue Slider Background: 色相グラデーション表示用のImage
   - Saturation Value Background: 彩度/明度選択用のRawImage
   - Color Picker Image: 選択された色を表示するImage

3. UnityEventを使用する
   - ColorPickerUIManagerコンポーネントのインスペクタ上から設定する
   - Saturation Value Backgroundがマウスなどでクリックされた場合に発火  
 
