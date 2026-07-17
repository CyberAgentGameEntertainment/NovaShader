# <機能名>

<!-- Phase 1 (Intent) で起票するファイル。AI-DLC のすべての起点。
     Phase 2 (Inception) で plan.md を作る際の入力になる。
     後続フェーズで要件が変わったら intent.md を更新し、その変更点からフローを再進行する（別フォルダは作らない）。

     書き方の指針: ../references/phase-1-intent.md の以下セクションを参照
       - 「Intent に書かないもの（4 種）」
       - 「Description / Completion Criteria は汎用的に書く」
       - 「品質ゲートの書き方」

     注: このファイルは docs/ai-dlc/ 配下の作業ドキュメントなので日本語でよい。
        documentation/ 配下の仕様書（英語・テンプレ準拠）とは別物。
-->

## Description

<!-- 何を作るか。1〜3 段落で具体的に。

     NOVA Shader は OSS の汎用パーティクルシェーダ基盤。汎用機能として書き、
     特定プロダクト固有の用語（プロダクト名、ゲーム内固有名詞、演出名等）は使わない。
     プロダクト固有の文脈は Context の「背景となる依頼」に閉じる。

     実装方針（どう作るか）はここに書かない（plan.md の領域）。
-->

## Context

<!-- なぜ作るか、どんな制約があるか。以下のような観点を含める:

     ✅ 書く:
     - 対象シェーダバリアント（必須。Unlit / Lit / UIParticles / Distortion のどこまで対応するか）
     - Unity 標準機能（Particle System モジュール等）との関係
     - 既存類似機構との関係（Alpha Transition / Flow Map / Custom Coord 等）
     - パフォーマンス制約（相対・定性的に。具体数値は plan.md）
     - 利用想定（誰がどう使うか）
     - 背景となる依頼（プロダクト固有の文脈・関連 Issue/スレッドリンク等はここに閉じる）
     - そのトピック固有の過去 PR 教訓（CLAUDE.md にないもの）

     ❌ 書かない（詳細は ../references/phase-1-intent.md 参照）:
     - スケジュール（リリース日 / マイルストーン / 期限）
     - 後段で決まる数値プレースホルダ
     - CLAUDE.md 既出ルール（Texture Sheet Animation 非推奨、documentation/ 英語必須等）
     - 具体検証手段の固有名（Unity Test Runner, AverageTest, /code-review 等）

     【対象シェーダ】UIParticles を含める場合は Custom Coord .xy 制約
     （documentation/UIParticles_Limitations.md）の下で成立するかも一言書く。
-->

## Completion Criteria

<!-- 完了条件。SMAV (Specific / Measurable / Atomic / Verifiable) を満たすほど AI 自律性が上がる。
     成功ケース・失敗ケース・品質ゲートの 3 要素を書く。
     プロダクト固有用語は使わず、汎用機能として記述する。
-->

### ✅ 成功ケース

<!-- 達成すべき動作。例:
     - マテリアル GUI から機能を有効化し、パラメータ調整できる
     - Custom Coord 経由でパーティクルごとに値を変えられる
     - GPU Instancing 有効時も同一の見た目になる
-->

### ❌ 失敗ケース

<!-- 起きてはいけないこと、エラーハンドリング。例:
     - 機能 OFF のマテリアルの描画結果がピクセル単位で変わらない
     - 必要な Vertex Streams が不足している時に検証エラーが表示され「Fix Now」で修正できる
     - 不正なパラメータ（Row Count 0 等）で描画破綻しない
-->

### 🔒 品質ゲート

<!-- 検証カテゴリ + 客観メトリクスで書く。
     具体の検証手段（コマンド / スキル / 計測ツールの固有名）はここに書かず、Phase 2 で plan.md に回す。

     ✅ 検証カテゴリ + 客観メトリクスの例:
     - コンパイル時エラー 0 / 警告 0（新規警告を出さない）
     - 既存および新規の自動テスト（EditMode / PlayMode）が pass
     - ビジュアルリグレッションテストで既存シーンに差分なし（意図した差分は期待画像を両プラットフォームで更新）
     - 機能仕様書（英語・テンプレ準拠）が documentation/ に追加されている
     - 負荷ゲート: OFF 時追加コスト 0、追加テクスチャサンプリング数が想定範囲内

     ❌ NG（具体手段の固有名は書かない）:
     - AverageTest pass       → 「ビジュアルリグレッションテスト pass」
     - /code-review pass      → 「自動レビュー・人手レビュー pass」
     - .meta は AI 編集禁止    → CLAUDE.md 既出のため Intent に書かない

     末尾に下記の一文を明記することを推奨:
     > 具体の検証手段（コマンド / スキル / 計測ツール）は Phase 2 (Inception) で plan.md に記述する。
-->
