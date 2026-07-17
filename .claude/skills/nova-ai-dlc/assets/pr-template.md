<!-- nova-ai-dlc PR 本文テンプレート（lean）。Phase 3 Step 7 で PR 本文に使う。

     方針:
     - PR タイトルは英語、本文は日本語（CLAUDE.md 規約）。
     - lean に保つ。設計根拠・棄却案・全変更ファイル列挙は載せない（それらは plan.md 側に閉じる）。
     - <...> を埋め、不要な行は削除する。
     - このガイドコメント自体は最終本文から削除する。
     - 書き方の指針: ../references/phase-3-construction.md の「Step 7」を参照。
-->

## 概要

<!-- 何を / なぜ。1〜3 行の箇条書き。
     必ず触れる（該当する場合）:
     - 既定値（デフォルト）の変更と、その切替手段（マテリアルプロパティ等）
     - 追加アセット（テストシーン / 期待画像等）の意図
     - documentation/ への仕様書追加・更新
-->
- <変更の要点>

## 設計意図

<!-- レビュー補助。なぜこのアプローチを採ったかを 1〜2 行で簡潔に。
     棄却案・詳細な比較は plan.md に委ね、ここでは要点だけ書いて lean を保つ。 -->
- <採用したアプローチと理由。例: 既存の Custom Coord 系配線パターンに揃え、新規インフラなし／OFF 時ゼロコスト。詳細は plan.md 参照>

## テスト

<!-- 品質ゲート（MUST）: Unity Test Runner 全件（PlayMode AverageTest + EditMode）成功。
     期待画像は Windows(Direct3D12) / macOS(Apple Silicon+Metal) の両系統が整合していること。
     失敗は最大 3 試行（初回+リトライ2回）で 1 回でも成功なら可。3 試行連続失敗は確定失敗として人間の判断に引き渡す。
     実際に確認してから [x] にする。 -->
- [ ] Windows (Direct3D12) で全 AverageTest + EditMode テスト成功
- [ ] macOS (Apple Silicon / Metal) で全 AverageTest + EditMode テスト成功
- <その他に行った確認があれば追記。なければ削除>

## 関連

<!-- Intent / Plan へのリンクは常に残す（AI-DLC トレーサビリティ）。 -->
- Intent / Plan: docs/ai-dlc/<date>-<topic-slug>/
- 関連 Issue: <あれば。なければ削除>
