---
name: nova-ai-dlc
description: "NOVA Shader の AI 駆動開発フロー (AI-DLC) の起点スキル。ユーザーが /nova-ai-dlc と明示的に呼び出した時のみ起動し、Intent → Inception → Construction の各 Phase を厳密に進行する。中間成果物は docs/ai-dlc/<date>-<topic-slug>/ に蓄積される。"
disable-model-invocation: true
---

# NOVA Shader AI 駆動開発フロー (AI-DLC) 起点スキル

このスキルは NOVA Shader リポジトリで **AI 駆動開発フロー** を開始/継続するためのもの。
ユーザーが `/nova-ai-dlc` と明示的に呼び出したときのみ起動する（`disable-model-invocation: true` 指定）。
普段の開発スタイルに影響を与えず、AI-DLC を選択したユーザー・タスクにだけ適用される。

## 思想

このフローは AWS が提唱する **AI-DLC (AI-Driven Development Life Cycle)** をベースにしている。
従来の「人間が書く → AI が補助」を反転させ、**AI が能動的に計画・提案 → 人間が承認 → AI が実装** という主従逆転を行う。

| 観点 | 従来 | AI-DLC |
|---|---|---|
| AI の役割 | 補助 | 能動的参加者 |
| 人間の役割 | 書く・レビュー | 判断・承認 |
| プロセス | 固定 | 意図に応じて適応 |

このスキルは厳密にこのフローを進行させる。ただし**自走で判断できる部分は決め打ちで進む**。ユーザーは進行を中断させたり会話で介入できる前提なので、AskUserQuestion は「人間にしか判断できない場面」に限定して使う。

## フロー全体図

```
Phase 1            Phase 2             Phase 3             Phase 4
Intent 起票   →    Inception     →    Construction   →    Review
（何を/なぜ）       （どう作るか）        （実装+PR）          （レビュー）
   │                  │                   │                   │
intent.md          plan.md             コード+PR          /code-review + 人手
   └───────────────────┴──── docs/ai-dlc/<date>-<topic-slug>/ ────┘
```

各 Phase の成果物は Markdown ファイルとして `docs/ai-dlc/<date>-<topic-slug>/` に蓄積される。
Phase 間の情報引き継ぎは **すべてファイル経由**。これにより各 Phase が自己完結し、Phase 完了後にコンテキストをリセットしても次 Phase を再現可能になる。

Phase 4 (Review) は既存の `/code-review` スキルと人手レビュー（GitHub PR）を利用する。専用スキルは持たない。

**注意**: `docs/ai-dlc/` は AI-DLC の作業ドキュメント置き場であり、`documentation/` フォルダ（英語必須・documentation_guidelines.md 適用）とは別物。AI-DLC 成果物は日本語で書いてよい。機能が完成したら Phase 3 で `documentation/{FeatureName}_Specification.md`（英語・テンプレート準拠）を別途作成する。

## 引数の解釈

引数は **自由テキスト** として受け取り、以下を意識して推測する:

1. **トピック** — 何の機能・改善か。例: "Trail 対応", "Dissolve Transition", "FlipBook 補間改善"
2. **開始フェーズ** — どの Phase から始めるか。明示キーワードがあれば採用、なければ既存状況から推定

### フェーズキーワード（参考。網羅でなくていい）

| キーワード例 | フェーズ |
|---|---|
| `intent`, `起票`, `新規`, `提案` | Phase 1 (Intent) |
| `inception`, `計画`, `設計`, `plan` | Phase 2 (Inception) |
| `construction`, `実装`, `construct`, `作る` | Phase 3 (Construction) |

明示キーワードがなければ、既存フォルダの中身から判定する:
- フォルダなし → Phase 1 (新規 Intent)
- `intent.md` のみ → Phase 2 が次
- `intent.md` + `plan.md` → Phase 3 が次

### 引数解釈の動作例

| 入力 | 解釈 | 動作 |
|---|---|---|
| `/nova-ai-dlc` | 引数なし | `docs/ai-dlc/` を ls して進行中のフォルダを列挙、選択肢を提示 |
| `/nova-ai-dlc dissolve実装` | トピック=dissolve実装 | フォルダ検索 → なければ新規 Phase 1 |
| `/nova-ai-dlc dissolve実装をinceptionから` | トピック+フェーズ=Phase 2 | 既存フォルダで Phase 2 から開始 |
| `/nova-ai-dlc Trail対応の計画立てて` | トピック+「計画」=Phase 2 | 既存フォルダ検索 → Phase 2 |
| `/nova-ai-dlc 続きから` | 直近の進行中フォルダ | 進行中フォルダの中身から次フェーズを判定 |
| `/nova-ai-dlc <GitHub Issue/PR URL>` 等 | URL を含む | 該当ツール（`gh` 等）で内容を取得 → トピック推定 → 新規 Phase 1 |

### 外部情報源 URL が含まれる場合

引数や対話の中で **GitHub Issue / PR / Discussion** などの URL が渡された場合は、`gh api` 等で内容を取得し、Intent の Description / Context の素材として利用する。

引き出す情報の典型例:
- 機能のトピック（フォルダ slug 推定の材料）
- 依頼の背景・経緯（Context の「背景となる依頼」に閉じる）
- 既存代替手段の有無・Unity 標準機能との関係
- 利用想定（誰がどう使うか）

スケジュール情報がそこに書かれていても Intent には転記しない（Phase 1 reference の「Intent に書かないもの」参照）。スケジュールは GitHub Issue / PR description 側で管理する。

## 処理フロー（このスキルが実行する手順）

### Step 1: 引数の解釈

引数からトピックとフェーズを推測する。複数候補で曖昧さがある場合のみ AskUserQuestion を使う。
**単一候補か妥当な推測ができる場合は決め打ちで進む**。ユーザーが意図と違えば会話で訂正する。

### Step 2: 既存フォルダの検索

```bash
ls docs/ai-dlc/
```

トピックキーワードでフォルダ名をファジーマッチ。フォルダ名形式は `<date>-<topic-slug>`。

- 候補 0 件 → 新規 Phase 1 を開始（フォルダはこのスキルが作成する）
- 候補 1 件 → 採用、次ステップへ
- 候補 2 件以上 → AskUserQuestion で 1 つ選択

### Step 3: フォルダの作成 / 検証

**新規の場合:**
- 日付プレフィックス: 今日の日付を `YYYY-MM-DD` で取得（`date -u +%Y-%m-%d`）
- トピック slug: 日本語入力を kebab-case 英数字に正規化（例: `dissolve実装` → `dissolve-transition`、`Trail対応` → `trail-support`）
  - **課題ベースで命名する**: 実装手段ではなく解決したい課題で命名する。
    - ⭕ `flipbook-frame-interpolation`（課題 = FlipBook のフレーム間補間）
    - ❌ `motion-vector-texture`（実装手段 = モーションベクタテクスチャ）

    理由: Intent は実装方針を固定しないドキュメントなので、フォルダ名が実装手段に寄ると Phase 2 で別案に切り替わった時に陳腐化する。
- 作成: `mkdir -p docs/ai-dlc/<date>-<topic-slug>/`

正規化に決定的な答えがない場合はもっとも自然な候補を採用して進む。ユーザーが気に入らなければ会話で訂正する。

**既存の場合:**
- `ls docs/ai-dlc/<date>-<topic-slug>/` で既存ファイル確認
- フェーズ整合性チェック（例: Phase 2 を開始しようとしているのに `intent.md` がなければエラー報告）

### Step 4: フェーズの実行

該当フェーズの reference を読み込み、その手順に従って実行する:

| フェーズ | reference |
|---|---|
| Phase 1 (Intent) | [references/phase-1-intent.md](references/phase-1-intent.md) |
| Phase 2 (Inception) | [references/phase-2-inception.md](references/phase-2-inception.md) |
| Phase 3 (Construction) | [references/phase-3-construction.md](references/phase-3-construction.md) |

reference の読み込みは「フェーズ実行直前」に行うこと。スキル起動時に全部読むとコンテキストが無駄に膨れる。

### Step 5: フェーズ完了時のメッセージ

フェーズが完了したら、生成したファイルのパスと **次フェーズの起動コマンド** を提示する。
コンテキストリセット (`/clear`) を **軽く推奨** するが、強制はしない。

```
✅ Phase 1 (Intent) 完了
   出力: docs/ai-dlc/2026-07-17-dissolve-transition/intent.md

次は Phase 2 (Inception) です。
コンテキストをリセットしてから実行すると、Markdown 引き継ぎの自己完結性が担保され
トークン消費を抑えられます（任意）。

   /clear
   /nova-ai-dlc dissolve実装をinceptionから
```

次フェーズの引数は **コピペで動く形** にすること（トピック名 + フェーズ指定を含める）。

## 共通ルール

### git 操作はユーザー許可制（MUST）

`git add` / `commit` / `push`、ブランチ作成、PR 作成（`gh pr create`）など、**リポジトリの状態やリモートを変える操作は、ユーザーが明示的に許可した場合にのみ行う**。許可がない間は、実装・コンパイル・テスト・ドキュメント編集・作業ツリー上の変更までに留める。

- 各 Phase は「git 操作の直前」で一旦立ち止まり、何をどうコミット / PR 化するか提示してユーザーの承認を得る
- この方針は下記「Human in the Loop の方針」の *迷ったら進む* および「自走で判断できる部分は決め打ちで進む」より **優先する**（git 操作は自走の対象外）
- コミット規約・ブランチ運用・PR 手順の詳細は [CLAUDE.md](../../../CLAUDE.md) の Pull Request Guidelines に従う（**PR タイトルは英語、PR 説明・コミットメッセージは日本語**）

### 触ってはいけないファイル

- `.meta` ファイル → Unity Editor が生成する。AI が生成する GUID は既存と衝突する可能性がある
- `Assets/ActualImages/`、`Assets/OptimizedShaders/` → テスト・生成物の出力先（gitignore 済み。コミット対象にしない）
- `Packages/manifest.json` / `Packages/packages-lock.json` / `ProjectSettings/ProjectVersion.txt` → 検証中に発生した意図しない差分はコミットしない（意図した変更のみコミット）

### 配置先の判断

実装対象がリポジトリのどのフォルダに着地するかは [references/nova-repo-map.md](references/nova-repo-map.md) を参照。
このスキルは **Phase 3 (Construction) で実装に入る直前** にこの reference を読み込む。

### 品質ゲート（PR マージ前 — MUST）

PR をマージする前に、**Unity Test Runner の全テスト（`Assets/Tests/Runtime/AverageTest.cs` の PlayMode ビジュアルリグレッション全ケース + EditMode 全テスト）が成功していること**を必須とする。`AverageTest` は NVIDIA FLIP による画像比較で、期待画像はプラットフォーム別（**Windows: Direct3D12 / macOS: Apple Silicon + Metal**）に `Assets/Tests/SuccessfulImages/` に登録されている。**見た目に影響し得るシェーダ変更は、最終的に両プラットフォームで期待画像の更新とテスト成功が担保されるまでマージしない**（実行できない側の OS はユーザーまたは他環境に依頼する）。CI に Unity テストの自動ゲートは無いため、**AI が手動で TestRunner を全件実行**して担保する。閾値近傍で初回失敗するテストは「失敗テストのみ」を対象に**初回 + リトライ 2 回 = 最大 3 試行**まで実行し、**1 回でも成功すれば PASS** として扱う。**3 試行連続で失敗したテストは「確定失敗」とし、FLIP 値 / 閾値 / 差分画像パスを添えて AskUserQuestion で人間の判断に引き渡す**（flaky か実装起因かの最終判定は人間が行う）。具体手順は [Phase 3 Step 4](references/phase-3-construction.md) を参照。

### Markdown 引き継ぎ規約

- すべての中間成果物は `docs/ai-dlc/<date>-<topic-slug>/` に置く
- ファイル名は **artifact 名で固定**: `intent.md` / `plan.md`
- 日付やトピックはフォルダ名に持たせる（ファイル名には持たせない）
- 次フェーズで読むファイルがすべて揃った状態でフェーズ完了とする
- `docs/ai-dlc/` の成果物は日本語で書いてよい（`documentation/` フォルダのガイドライン適用外）。ただし **`documentation/` フォルダに置くファイルは英語 + documentation_guidelines.md 準拠が MUST**

### Human in the Loop の方針

AI-DLC の核は「重要な判断には人間確認が必須」。ただし NOVA では **過度の確認はフローを停滞させる** ため次のように運用する:

- ✅ AskUserQuestion を使う: 真に複数の妥当な選択肢がある場面、データ削除など不可逆な操作前
- ❌ 使わない: 自動推測で妥当な選択ができる場面、補完情報の確認（ユーザーは会話で介入できる）

迷ったら **進む** ことを優先する。間違っていればユーザーが訂正する。

## 関連ドキュメント

- [docs/ai-dlc/README.md](../../../docs/ai-dlc/README.md) — `docs/ai-dlc/` 配下の運用ルール
- [references/nova-repo-map.md](references/nova-repo-map.md) — リポジトリ配置マップ
- [assets/intent-template.md](assets/intent-template.md) — Intent 雛形
- [assets/plan-template.md](assets/plan-template.md) — Plan 雛形
- [assets/pr-template.md](assets/pr-template.md) — PR 本文雛形（Phase 3・lean）
