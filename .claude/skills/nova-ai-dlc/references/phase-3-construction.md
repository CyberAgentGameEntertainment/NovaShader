# Phase 3: Construction（実装）

このフェーズでは plan.md に従って実装を進め、コンパイル・テストまでを直列で進める。コミット以降（コミット / push / PR 作成）は git 操作にあたるため、**ユーザーが明示的に許可した場合にのみ実行する**（[SKILL.md](../SKILL.md) 共通ルール「git 操作はユーザー許可制」を参照）。許可がなければ Step 4 完了時点（実装＋テスト通過）でいったん報告して止まる。

## 入力

- `docs/ai-dlc/<date>-<topic-slug>/plan.md` (Phase 2 の成果物)
- [./nova-repo-map.md](./nova-repo-map.md) (リポジトリ配置マップ)
- `documentation/` の関連仕様書（plan.md が参照しているもの）

## 成果物

- NovaShader リポジトリへの PR × 1
- 新機能の場合: `documentation/{FeatureName}_Specification.md`（英語・テンプレート準拠）
- (将来) `docs/ai-dlc/<date>-<topic-slug>/review-notes.md` などを同フォルダに追加可能

## 手順

### Step 1: plan.md を読み込み、UoW 順序を確定

plan.md の依存関係に従って UoW の実行順を確定する。
人間作業推奨の UoW は AI が実装をスキップし、後で人間が完了させる前提で進める。

### Step 2: UoW を 1 件ずつ実装

各 UoW について:

1. plan.md の該当 UoW セクションを読む
2. 関連する `documentation/` 仕様書を読む（まだ読んでなければ）
3. 編集対象ファイルを Read してから Edit / Write
4. **判断点があればメッセージで報告**（AskUserQuestion ではなく地の文で）

シェーダ変更の UoW では以下を必ず守る:
- 対象バリアント（Unlit / Lit / UIParticles / Distortion）と全パス（Forward / ShadowCaster / DepthNormals / DepthOnly）への波及を plan.md 通りにカバーする
- 新規キーワードは plan.md で決めた `shader_feature_local_vertex` / `_fragment` を使う
- 既存 enum の順序・値を変えない（追加は末尾のみ）

ドキュメント UoW（`documentation/` 配下）では、**編集前に documentation_guidelines.md の禁止事項（トラブルシューティング / チェックリスト / 更新履歴・日付 / バージョン情報）を確認**し、英語 + FeatureSpecification_Template.md 準拠で書く（CLAUDE.md の Documentation Editing Protocol）。

### Step 3: コンパイルチェック

UoW 完了ごと、または複数 UoW がひと段落したら、Unity Editor でコンパイルエラー 0 を確認する。

- Unity Editor が起動している環境なら、フォーカスによるドメインリロード後の Console でエラー確認（ユーザーに依頼してもよい）
- CLI で確認する場合の例（Unity のパスは環境に合わせる）:
  ```
  Unity -batchmode -quit -projectPath . -logFile Logs/compile-check.log
  ```
  ログに `error CS` / `Shader error` が無いことを確認する
- シェーダ変更時はマテリアルのコンパイルエラー（マゼンタ化）も確認対象

エラーが出たら直して再実行。エラーが消えるまで次に進まない。

### Step 4: テスト実行（PR マージ前の品質ゲート — MUST）

**品質ゲート（MUST）**: PR をマージする前に、**Unity Test Runner の全テスト（`Assets/Tests/Runtime/AverageTest.cs` の PlayMode ビジュアルリグレッション全ケース + EditMode 全テスト）が成功していること**を必須とする。CI に Unity テストの自動ゲートは存在しないため、**AI が手動で TestRunner を実行して担保する**。シェーダ・描画の変更は変更対象以外のテストシーン（Unlit / Lit / UIParticle / Distortion / VertexDeformation / OptimizedShader）にも波及し得るので、**変更箇所に関係なく必ず全件を回す**。一部だけ回して成功しても品質ゲートを満たしたとは見なさない。

実行方法:
- Unity Editor の Test Runner ウィンドウ（EditMode / PlayMode）から全件実行
- CLI の例（結果は NUnit XML で判定する）:
  ```
  Unity -batchmode -projectPath . -runTests -testPlatform EditMode -testResults Logs/editmode-results.xml
  Unity -batchmode -projectPath . -runTests -testPlatform PlayMode  -testResults Logs/playmode-results.xml
  ```
  PlayMode 全件はシーンロード込みで時間がかかるため `run_in_background` で実行し、完了後に XML を読んで合否判定する。CLI がタイムアウトしても Unity 側でテストが完走していることがあるので、**XML の有無と中身で判定**する。

**プラットフォーム条件**: `AverageTest` の期待画像は **Windows: Direct3D12 / macOS: Apple Silicon + Metal** の 2 系統で `Assets/Tests/SuccessfulImages/` に登録されている。実行環境の Graphics API が期待画像の系統と一致していることを確認する。macOS では初回に `setup.sh` の実行（FLIP ツールの quarantine 解除）が必要（`Documentation~/TestDocument.md` 参照）。

**flaky リトライ規則（MUST）**: FLIP 閾値近傍のテストは初回に失敗することがある。試行回数は**初回 + リトライ 2 回 = 最大 3 試行**とし、リトライ対象は**直前の試行で失敗したテストのみ**（成功テストは再実行しない）。**3 試行のうち 1 回でも成功すれば PASS**（flaky 扱い）。**3 試行連続で失敗したテストは「確定失敗」とし、FLIP 値 / 閾値 / 差分画像（`Assets/ActualImages/` に出力される `.diff.png` / `.expected.png`）を添えて AskUserQuestion で人間の判断に引き渡す**。flaky か実装起因かの最終判定は人間が行い、実装起因と判断された場合のみ修正して再度全件を成功させる。

失敗ケースのみの再実行はテストフィルタで絞れる:
```
-testFilter "Tests.Runtime.AverageTest.Test(\"Test_Distortion\")"
```

### Step 4.5: 見た目が意図的に変わる場合（期待画像の更新）

新機能のテストシーン追加や意図した見た目の変更がある場合:

1. テストを実行して `Assets/ActualImages/` に実画像を出力させる
2. 実画像を目視で検証し、**意図した変化であることを確認**（意図しない差分を期待画像に取り込まない）
3. メニュー `Tools/NOVA Shader/Test/Copy AverageTest Result` で `Assets/Tests/SuccessfulImages/` にコピー
4. 再度全件テストを実行して PASS を確認
5. 更新した期待画像は **実装と同じ PR でコミット**する

**両プラットフォーム対応（MUST）**: 期待画像は Windows (Direct3D12) と macOS (Apple Silicon/Metal) の 2 系統ある。実行中の OS で更新できるのは片側だけなので、**もう片側はユーザーまたは他環境での実行を依頼する**（plan.md の人間作業 UoW に含めておく）。片側の画像だけ更新した状態でマージしない。

### Step 5: コミット

> **このステップ以降（コミット / push / PR）は git 操作。実行前にユーザーの明示的な許可を得ること**（[SKILL.md](../SKILL.md) 共通ルール「git 操作はユーザー許可制」）。許可がなければ Step 4 完了時点で報告して止まり、以降の手順は許可が出てから進める。以下は **許可された後に従う手順**。

**重要なルール:**
- コミットメッセージは **日本語**（CLAUDE.md 規約）
- `git add` は **ファイル名を明示**（`-A` / `.` は意図しない差分混入のリスク。特に `Packages/manifest.json` / `packages-lock.json` / `ProjectSettings/ProjectVersion.txt` の環境起因差分）
- `.meta` ファイルは Unity Editor が生成したものをそのままコミット（AI が生成した GUID はコミット前に検出・除去）
- UoW 単位を目安にコミットを分ける。ドキュメント（`documentation/`）は実装とは別コミットにする

```bash
git add Assets/Nova/Runtime/Core/Shaders/... Assets/Nova/Editor/Core/Scripts/...
git commit -m "Dissolve Transition のシェーダ実装を追加"

git add documentation/DissolveTransition_Specification.md CLAUDE.md
git commit -m "Dissolve Transition の仕様書を追加"
```

### Step 6: ブランチ作成と push

`origin/main` から feature ブランチを作成して push する（既存ブランチ命名は `feature/...` が主流）:

```bash
git checkout -b feature/dissolve-transition origin/main
# (コミットを積む or cherry-pick)
git push -u origin feature/dissolve-transition
```

### Step 7: PR 作成

本文は [assets/pr-template.md](../assets/pr-template.md) を雛形にする。

- **PR タイトルは英語、本文は日本語**（CLAUDE.md 規約）
- 概要に既定値変更・追加アセット・期待画像更新の意図を明記する
- **テスト欄は品質ゲート（全 AverageTest + EditMode 成功、両プラットフォームの期待画像整合、リトライ規則）を実際に確認した上で `[x]`** にする
- 作成前に PR タイトルと本文を提示し、**AskUserQuestion で確認してから作成**する

```bash
gh pr create --base main \
  --title "Add Dissolve Transition feature" --body-file <一時ファイル>
```

### Step 8: 完了メッセージ

```
✅ Phase 3 (Construction) 完了
   作成 PR: #XXX
   関連 plan: docs/ai-dlc/<date>-<topic-slug>/plan.md

次は Phase 4 (Review) です。既存スキルと人手レビューを使います:

   /code-review
   （+ GitHub 上での人手レビュー依頼）
```

## このフェーズで AskUserQuestion を使う場面

- ✅ テスト失敗の対応方針（確定失敗の flaky / 実装起因判定）
- ✅ PR 作成時のコミット粒度・タイトル
- ❌ 「次の UoW に進んでいいですか？」のような形式的確認

## 注意事項

- **plan.md と異なる実装をした場合は plan.md を更新する** — 「採用設計」と「実装」が食い違う状態を残さない
- **判断点をメッセージで報告する** — ユーザーが気付ける形にする（AskUserQuestion で止めないが、報告は重要）
- **`.meta` ファイルを AI が生成・編集していないか確認** — Unity Editor が生成したものに限定
- **`documentation/` の編集は Documentation Editing Protocol に従う** — 禁止事項（トラブルシューティング / チェックリスト / 日付・履歴 / バージョン情報）を混入させない。英語で書く
- **`Assets/ActualImages/` / `Assets/OptimizedShaders/` をコミットしない** — gitignore 済みのテスト・生成出力
- **PR を分けるべき変更は分ける** — 1 PR にしすぎると review コストが増える。plan.md の見積もりを尊重する
