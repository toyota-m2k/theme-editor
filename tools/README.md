# tools/ — 開発用スクリプト

## resolve_material_tones.ps1

Material Design 3（Material Color Utilities = MCU）の **標準トーン表** を算出するスクリプトです。
`ThemeEditor/ThemeEditor/StandardTones.cs` に埋め込んでいる定数（各ロールの Light/Dark ×
Normal/Medium/High のトーン値）の生成元です。

### これは何を計算しているか

Material のコントラストは、ロールごとの **トーン（HCT の Tone = CIELAB の L\*）** だけを動かして
実現されます（色相・彩度はコントラスト間で一定）。トーンは「背景ロールに対する目標コントラスト比」から
逆算されるため、**元の色相に依存しない定数** になります。本スクリプトは MCU の以下を忠実に移植し、
全 38 ロールのトーンを Light/Dark・Normal/Medium/High で解決します。

- `Contrast.ratioOfTones / lighter / darker`（輝度ベースのコントラスト計算）
- `ContrastCurve`（コントラストレベル → 目標比の補間。Normal=0.0 / Medium=0.5 / High=1.0）
- `DynamicColor.getTone` + `foregroundTone`（背景に対して目標比を満たすトーンの求解）
- `ToneDeltaPair`（primary↔primaryContainer や *Fixed↔*FixedDim の相互拘束、50–59 の回避）
- `highestSurface`（Light=surfaceDim / Dark=surfaceBright）

### 使い方

PowerShell 7（`pwsh`）が必要です。リポジトリルートから:

```powershell
pwsh -NoProfile -File tools/resolve_material_tones.ps1
```

出力は次の形式（各値は丸めた L\* トーン、0–100）:

```
role                     LIGHT N/M/H    DARK N/M/H
primary                  40/23/18       80/88/95
primaryContainer         90/46/31       30/60/79
...
```

### 出力の正しさの確認

**Normal 列（各行の先頭の値）が、既知の Material ベーストーンと一致すること** が検証ポイントです。
例: primary=40/80、primaryContainer=90/30、onSurface=10/90、surface=98/6。
一致していれば移植が正しく、Medium/High も信頼できます。

### `StandardTones.cs` への反映方法

出力の `LIGHT N/M/H` と `DARK N/M/H` を、`StandardTones.cs` の `Map` の各行へ転記します。

```csharp
// role                     = (new(LightN, LightM, LightH), new(DarkN, DarkM, DarkH)),
["primary"]                 = (new(40, 23, 18),  new(80, 88, 95)),
["primaryContainer"]        = (new(90, 46, 31),  new(30, 60, 79)),
```

変更が必要なのは `StandardTones.cs` のみで、生成ロジック（`AndroidThemeSet.GenerateContrastFromNormal`
や `ColorSpace.ShiftLightness`）は触りません。

## 再生成が必要になるケース

現在の表は **MCU 2021 spec** の値です。次のような場合に再生成／差し替えを検討します。

1. **Theme Builder のバージョン（spec）を厳密に合わせたいとき**
   現行の Material Theme Builder は新しい **2025 spec** を使う可能性があり、特に高コントラストでの
   コンテナ色の挙動などが 2021 と異なります。厳密一致が必要になったら以下のいずれかで更新します。
   - スクリプト内のロール定義（ベーストーン・`ContrastCurve`・`ToneDeltaPair`・背景）を対象 spec に
     合わせて書き換え、再実行する。
   - **より簡単で確実な方法**: 未編集（pristine）の Theme Builder エクスポート
     （`colors.xml` と `night/colors.xml`、`*_mediumContrast` / `*_highContrast` を含む）から、
     各ロールの色の L\* を読み取って直接 `StandardTones.cs` に転記する。これがそのバージョンの
     ground truth になります（スクリプト不要）。

2. **生成される Medium/High の見た目を調整したいとき**
   コントラストの効き方を変えたい場合は、スクリプト内の `ContrastCurve`（目標比）や
   ベーストーンを調整して再生成します。

3. **対象ロールを増減したとき**
   `AndroidColorTheme` に扱うロールを追加した場合は、スクリプトの `$R`（ロール定義）と
   出力順 `$order`、および `StandardTones.cs` の `Map` に同じロールを追加します。

### 補足

- トーン値は色相に依存しない定数のため、どんなテーマ（シード色）でも同じ表が使えます。
- 実行時に色空間変換を行うのは `ColorSpace`（CIELAB）で、生成は「編集後 Normal の L\* を
  `(標準Medium − 標準Normal)` 分シフト」する差分方式です。表を差し替えるだけで挙動が更新されます。
