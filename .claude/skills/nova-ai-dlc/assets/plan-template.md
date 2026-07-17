# <機能名> 実装計画

<!-- Phase 2 (Inception) で生成するファイル。
     intent.md を入力に、UoW 分解と採用設計を記述する。
     Phase 3 (Construction) で UoW ごとに該当セクションを読む。
-->

## 採用設計

<!-- 採用した設計案の名称と概要。例:
     **Plan B: 独立機能として実装**
     - シェーダプロパティ `_Dissolve...` + キーワード `_DISSOLVE_ENABLED` (shader_feature_local_fragment)
     - Custom Coord で進行度をパーティクル単位制御
     - UIParticles は .xy 制約下で対応

     以下を必ず含める（Phase 2 Step 3 のインタフェース設計）:
     - プロパティ設計: 名前 / 型 / デフォルト値 / Custom Coord 対応
     - キーワード設計: 名前 / shader_feature_local_vertex か _fragment かとその理由
     - enum 追加の有無（既存 enum は末尾追加のみ）
     - GUI 配置: ParticlesUberCommonGUI のどのセクションか
     - 対象バリアント / パスへの波及範囲
-->

## 棄却した代替案

<!-- 検討したが採用しなかった案と理由。後から「なぜこの設計か」を辿るために残す。
     棄却理由は客観的に書く（「複雑だから」ではなく「UoW 数が 2 倍になる」
     「既存のキーワード管理集約原則と異質」など）。

     ### Plan A: <名前>
     棄却理由: ...

     ### Plan C: <名前>
     棄却理由: ...
-->

## Units of Work

<!-- 実装単位の一覧。各 UoW に以下を書く:
     - 対象: 編集ファイル / ディレクトリ（references/nova-repo-map.md 参照）
     - 依存: 他のどの UoW が完了している必要があるか
     - 並列可能か
     - 担当: AI / 人間 / 両方
       （期待画像の他プラットフォーム分キャプチャや Editor 上の配置作業は人間担当にする）

     UoW 粒度の目安: 1 UoW = コミット 1〜2 個分

     典型構成: シェーダ実装 → エディタ配線 → テスト+期待画像 → 仕様書(英語) → デモ
-->

### UoW#1 [<役割>] <依存・並列の注記>

- 対象: <パス>
- 追加 / 変更ファイル: <一覧>
- 依存: <他 UoW or なし>
- 担当: AI / 人間 / 両方

### UoW#2 [...]

...

## 並列可能ペア

<!-- 領域独立で同時実装できる UoW の組。依存関係の理解のため明示しておく。

     例:
     - UoW#2 ‖ UoW#4
-->

## 検証手段

<!-- intent.md の品質ゲート（検証カテゴリ + メトリクス）を、具体のコマンド / ツールに落とす。

     例:
     - コンパイル: Unity batchmode コンパイルチェック（エラー 0 / 新規警告 0）
     - 自動テスト: Unity Test Runner EditMode + PlayMode 全件（AverageTest 含む）
     - 期待画像: Windows(D3D12) は AI が更新、macOS(Metal) は人間依頼（UoW#N）
     - 負荷: Frame Debugger で OFF 時のパス数・サンプリング数を確認
-->

## 触ってはいけないファイル

<!-- 個別 UoW のスコープと無関係に、本機能の実装中に触ってはいけないもの。例:
     - *.meta (既存 GUID 維持、新規は Unity Editor 任せ)
     - Assets/ActualImages/, Assets/OptimizedShaders/ (gitignore 済み出力)
     - Packages/manifest.json / ProjectVersion.txt の環境起因差分
-->

## PR 構成

<!-- 単一リポジトリなので原則 1 本。分割するならレビューコスト目的の分割理由を書く。
     PR タイトルは英語 / 本文・コミットは日本語（CLAUDE.md）。
-->
