## SmkUploaderSetting 設計

## 目的

SmkUploader というコンソールアプリを公開したが、設定ファイル(ini)の編集を一般ユーザーに強いており、ときどきエラーを引き起こしている。
素人でも安全に設定できるよう、WPFでGUIアプリケーション「SmkUploaderSetting」を開発する。

## 基本設計

### 前提

- SmkUploader はワンショットのバッチであり、その設定ファイルを事前に編集するという背景であるため、リアルタイムな保存や読込は不要。
- iniファイルの直接編集をサポートしないスタンスとするため、GUI起動時に読み込んだ異常データの混入は強くガードしなくてよく、初期値にフォールバックすれば良い。
- 設定ファイルにセクションは無い。コメントや空行はあるが無視でよい。

### 構成

- Windows 11(64bit) の .Net9 インストール済み環境で動く WPF アプリケーション。
- ソリューション
  - SmkUploader プロジェクト：開発済みのコンソールアプリ
  - SmkUploaderSetting プロジェクト：開発対象のGUIアプリ

### 要件

- 設定定義クラスを定義し、静的な設定定義オブジェクトとして定義した内容に基づいてUIとバリデーションを動的に構築
- 設定ファイルはexeと同じフォルダに存在することとする。
- 定義クラスには下記の情報を定義する
  - 設定定義バージョン（整数）
  - 項目ごとの定義のリスト
- 項目ごとに下記の情報を定義する
  - 項目名（キー）
  - 説明文
  - 型（int, double, bool, string）
  - デフォルト値
  - バリデーションルールオブジェクト（0個以上）
- バリデーションルール
  - インターフェース化して、追加しやすいようにする。
  - チェックロジックを持ち、判定結果（成否、エラーメッセージ）を返すことが出来る。
- バリデーションルールは個別に実装するものだが、下記が多用されるのでプリセットとして用意する。一部は引数をもたせる必要がある。
  - 必須チェック
  - 数値範囲チェック：最小値、最大値
  - 正規表現チェック：パターン、説明（パターン文字列を表示しても理解できないため、「半角英数字」などと示す説明）
  - 複合条件チェック：複合条件を判断するPredicate。単にカスタムルールではあるが、複合条件であることをわかりやすくする
- 保存ボタンでiniファイルに書き込み。
  - 完全に上書きすることとし、既存の未定義項目やコメントは消えて構わない。
  - 保存失敗時はダイアログ表示。
- ファイルが存在しない場合は初期値でiniファイルを生成し、UIを構築。保存ボタンでその内容を保存できる。
- リセットボタンでデフォルト値を読み込み、UIを更新。押下時に確認ダイアログを表示。

### UI構築に関する検討

- 全体レイアウト：1画面表示とし、タブやカラムレイアウトは不要。ただし保存/リセットボタンは隠れないように常に上部に表示されるようにエリアを用意する。保存に成功した時、「保存しました」というラベルをこのエリアに一定時間表示する。
- UI構築時のエラーデータ：読込データがおかしい場合は初期値にする。 ex) 数値フィールドに「abc」→初期値に戻す
- fluent のソースパックに任せるため、できるだけ独自のスタイリングを行わない。

```
ボタンエリア：水平、スティッキー
  保存成功ラベル：保存に成功した時、「保存しました」と一定時間表示する。
  保存ボタン：入力エラーが有るときは押せない。
  リセットボタン：全体を初期値に戻す。
スクロールエリア：垂直
  foreach 項目：設定定義オブジェクトで定義されている順番
    1行目
      左カラム
        ラベル："日本語項目名 (キー名)"。アンダースコアがアクセラレーションキーと誤認されるのでテキストブロックにする
        説明文
      右カラム
        入力欄：boolはチェックボックス、enumはプルダウン、その他はテキストフィールド。
    2行目
      エラーメッセージ：入力内容変更時に即時チェックして表示。
ステータスバー（フッター）：スティッキー。
  バージョン："Ver.x.x.x"
  ヘルプページへのリンク
```

### 設定定義バージョン

- 設定定義にはバージョン番号（整数）を持たせる。
- バージョンごとに定義オブジェクトを用意し、必要に応じて上位バージョンへの変換メソッドを実装する。
- 変換は複数段階を経てもよく、最終的に最新バージョンへ到達できればよい。
- 旧バージョンの設定ファイルは設定定義バージョンを持っていないので、0として扱う。
  
例：

```
最新：ver2
ver0のオブジェクト：ver1への変換可能
ver1のオブジェクト：ver2へ変換可能
ver2のオブジェクト：変換ロジックなし
→ver0は最終的にver2へ変換できるのでOK。
```

### 開発スコープ

スコープ外
- 多言語対応
- アクセシビリティ（フレームワークに任せる）

## 詳細設計

### クラス設計

#### 設定定義関連

- **SettingDefinition**
  - 設定全体の定義情報を保持するクラス。
  - プロパティ:
    - `Version: int` 設定定義バージョン
    - `Items: List<SettingItemDefinition>` 項目定義リスト

- **SettingItemDefinition**
  - 各設定項目の定義情報を保持するクラス。
  - プロパティ:
    - `Key: string` 項目名
    - `Description: string` 説明文
    - `Type: SettingValueType` 型（int, double, bool, string などのenum）
    - `DefaultValue: object` デフォルト値
    - `ValidationRules: List<IValidationRule>` バリデーションルールリスト

- **SettingValueType (enum)**
  - 設定値の型を表す列挙型（Int, Double, Bool, String）

#### バリデーション関連

- **IValidationRule**
  - バリデーションルールのインターフェース。
  - メソッド:
    - `Validate(object value, IReadOnlyDictionary<string, object> allValues): ValidationResult`
      - ※ 他項目の値も参照できるように、全体値を引数に追加

- **ValidationResult**
  - バリデーション結果を表すクラス。
  - プロパティ:
    - `IsValid: bool`
    - `ErrorMessage: string`

- **RequiredRule, RangeRule, RegexRule**
  - それぞれ必須・範囲・正規表現チェック用のバリデーションルール実装クラス。
  - RangeRuleは`Min`/`Max`、RegexRuleは`Pattern`/`PatternDescription`などのプロパティを持つ。

- **ConditionalRequiredRule**
  - 他項目の値に応じて必須かどうかを判定するバリデーションルール。
  - プロパティ:
    - `Condition: Func<IReadOnlyDictionary<string, object>, bool>` 必須となる条件式
    - `ErrorMessage: string`
  - 例: `SMKU_SERVICE`が`GYAZO`なら`SMKU_GYAZO_TOKEN`必須

#### 設定値管理

- **SettingValues**
  - 実際の設定値を保持・操作するクラス。
  - プロパティ:
    - `Values: Dictionary<string, object>`
  - メソッド:
    - `LoadFromIni(string path, SettingDefinition def): SettingValues`
    - `SaveToIni(string path, SettingDefinition def)`
    - `ResetToDefault(SettingDefinition def)`

#### UIバインディング用

- **SettingItemViewModel**
  - 各項目の値・エラー状態・UIバインディング用プロパティを持つViewModel。
  - プロパティ:
    - `Value: object`
    - `Error: string`
    - `Definition: SettingItemDefinition`
  - メソッド:
    - `Validate()`

- **SettingViewModel**
  - 設定全体のViewModel。保存・リセット・バリデーション状態管理。
  - プロパティ:
    - `Items: ObservableCollection<SettingItemViewModel>`
    - `CanSave: bool`
  - メソッド:
    - `Load()`
    - `Save()`
    - `Reset()`

#### バージョン変換

- **ISettingVersionConverter**
  - 設定値オブジェクトを旧バージョンから新バージョンへ変換するインターフェース。
  - メソッド:
    - `Convert(SettingValues oldValues): SettingValues`

- **SettingVersionConverter_v0_to_v1, ...**
  - バージョンごとの変換クラス。

#### その他

- **IniFileService**
  - iniファイルの読み書きユーティリティ。

> 実装時は上記クラスを基に、WinUI3のMVVMパターンに沿ってView/ViewModel/Modelを分離して実装する。

## 補足

現在の実際の設定ファイル例：

```ini
; リサイズ幅
;   値: 整数。0ならリサイズしない
SMKU_MAX_WIDTH=2048

; リサイズ高さ
;   値: 整数。0ならリサイズしない
SMKU_MAX_HEIGHT=2048

; ログレベル設定: ログファイルへのログ出力レベルを設定
;   値: Verbose(最も詳細), Information, Warning(推奨:最低限、高速), Error
SMKU_LOG_LEVEL=Warning

; クリップボード自動登録: アップロード後に自動的にURLをクリップボードにコピーするかどうか
;   値: True(コピーする), False(コピーしない)
SMKU_SET_CLIPBOARD=True

; ノーインタラクティブモード: エラーや警告が発生しても画面をすぐに閉じる
;   値: True(すぐに閉じる), False(ユーザー入力待ち)
SMKU_NO_INTERACTIVE=False

; 完了時に音を鳴らすかどうか
;   値: True(音を鳴らす), False(音を鳴らさない)
SMKU_RESULT_SOUND=True

; 画像ホスティングサービス名
;   値: 実装済みのサービスから選択
SMKU_SERVICE=GYAZO

; GYAZO固有設定 > アクセストークン
SMKU_GYAZO_TOKEN=s-x-xxx_xxxxxxxxxxxxxxxxxx-xxxxxxxxxxxxxxxx

; Imgbb固有設定 > APIキー
SMKU_IMGBB_API_KEY=xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

; Imgbb固有設定 > ファイル有効期限（時間）
;   値: 整数。0なら180日間
SMKU_IMGBB_EXPIRATION_HOURS=24
```

### 設定定義例（複合条件バリデーション）

```csharp
new SettingItemDefinition {
    Key = "SMKU_GYAZO_TOKEN",
    // ...existing code...
    ValidationRules = new List<IValidationRule> {
        new ConditionalRequiredRule(
            condition: values => values["SMKU_SERVICE"]?.ToString() == "GYAZO",
            errorMessage: "SMKU_SERVICEがGYAZOのとき必須です"
        )
    }
},
new SettingItemDefinition {
    Key = "SMKU_IMGBB_API_KEY",
    // ...existing code...
    ValidationRules = new List<IValidationRule> {
        new ConditionalRequiredRule(
            condition: values => values["SMKU_SERVICE"]?.ToString() == "IMGBB",
            errorMessage: "SMKU_SERVICEがIMGBBのとき必須です"
        )
    }
},
new SettingItemDefinition {
    Key = "SMKU_IMGBB_EXPIRATION_HOURS",
    // ...existing code...
    ValidationRules = new List<IValidationRule> {
        new ConditionalRequiredRule(
            condition: values => values["SMKU_SERVICE"]?.ToString() == "IMGBB",
            errorMessage: "SMKU_SERVICEがIMGBBのとき必須です"
        )
    }
}
```
