<div align="center">
	<img src="./Booth/icon.png" alt="logo" height="128" />
</div>

# SmkUploader
Clipboard image uploader CLI for imagePad in VRChat (and for everything else)

アプリケーション概要： https://docs.google.com/document/d/1Lng8lue28fDLf1T5D6NUervUHl-wxoci__3t3xWaQc4

ビルド済みバイナリ： https://booth.pm/ja/items/7289195

```
画像をアップロードしURLを取得するツールです。画像の指定方法は複数あります:
  - クリップボードに登録されたファイル・画像
  - クリップボードに登録された パス/URL のテキスト
  - コマンドライン引数での パス/URL の指定

詳細はドキュメントサイトを参照してください。
https://docs.google.com/document/d/1Lng8lue28fDLf1T5D6NUervUHl-wxoci__3t3xWaQc4

Shiftキーを押しながら起動すると設定画面が開きます。

<コマンド書式>
書式: SmkUploader.exe [OPTIONS] [IMAGE_PATH|IMAGE_URL]

OPTIONS:
  --help,-h    このヘルプメッセージを表示
  --version,-v バージョン情報を表示
  --KEY=VALUE  設定ファイル及び環境変数の内容をオーバーライドします。
KEY:           設定項目のキー。 \"SMKU_\" で始まります。
VALUE:         設定項目の値。
IMAGE_PATH:    アップロードする画像のパス。
IMAGE_URL:     アップロードする画像のURL。

<環境変数について>
設定キーと同じ名前の環境変数で設定ファイルの内容をオーバーライドできます。
```