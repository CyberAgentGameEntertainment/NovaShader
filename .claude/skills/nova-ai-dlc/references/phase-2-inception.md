# Phase 2: Inception（設計と UoW 分解）

このフェーズでは、Intent を「どう作るか」に落とす。
複数の設計案を発散させて1つを選び、実装単位 (UoW: Unit of Work) に分解する。

## 成果物

`docs/ai-dlc/<date>-<topic-slug>/plan.md` (テンプレ: [../assets/plan-template.md](../assets/plan-template.md))

## 入力

- `docs/ai-dlc/<date>-<topic-slug>/intent.md` (Phase 1 の成果物)
- `documentation/` の関連仕様書（CustomCoord_SystemArchitecture.md / TEXCOORD_Usage_Strategy.md / UIParticles_Limitations.md / 既存 Feature Specification）
- [./nova-repo-map.md](./nova-repo-map.md) (リポジトリ配置マップ)

## 手順

### Step 1: 既存実装の深掘り調査（チェックリスト）

設計案を考える前に必ず以下を完了する。**1 つでも飛ばすと Phase 2 の途中で「実装の制約条件を見落として案を出し直し」になる確率が高い**。AI には「関連ファイル名を列挙して終わり」になりがちなので、チェックリストとして明示する。

```
□ documentation/ の関連仕様書を Read（Custom Coord / TEXCOORD / UIParticles 制約 / 類似機能の Specification）
□ 直接変更する関数 / HLSL 関数 / クラスの【本体ロジック】を Read（シグネチャだけでなく中身まで）
□ その関数を呼ぶ側 (1-hop caller) を Read
□ その関数が依存する側 (1-hop callee) を Read
□ 類似既存機能の【プロパティ配線チェーン】を一式 Read:
    シェーダプロパティ定義 (.shader / ParticlesUber.hlsl)
    → MaterialPropertyNames.cs → ParticlesUberCommonMaterialProperties.cs
    → GUI (ParticlesUberCommonGUI.cs) → キーワード管理 (ParticlesUberUnlitMaterialPostProcessor.cs)
    → 検証 (RendererErrorHandler.cs)
□ シェーダ変更を伴う場合: 対象パスの TEXCOORD 空きスロットを確認
    （documentation/TEXCOORD_Usage_Strategy.md の割当表 + 実コードで裏取り）
□ シェーダ変更を伴う場合: 類似キーワードの `shader_feature_local*` / `multi_compile*` の使い分けを grep で確認
□ 対象シェーダバリアント（Unlit / Lit / UIParticles / Distortion）と全パス
    （Forward / ShadowCaster / DepthNormals / DepthOnly）への波及範囲を列挙
□ OptimizedShaderGenerator への影響を確認（新規キーワードが最適化で削除されないか）
□ 既存パターンを 1 段落で言語化する（次項参照）
```

#### 既存パターンを 1 段落で言語化する

チェックリスト最後の項目が特に重要。「この機能領域の既存コードは X の原則で動いている」を 1〜2 文で書き出す。

例:
- 「Custom Coord は **decimal エンコード原則**。`値 % 10` でストリーム、`値 / 10` でコンポーネントを引く。全 30+ プロパティが同じ `GET_CUSTOM_COORD` マクロを通る」
- 「機能トグルは **プロパティ float + shader_feature_local キーワードのペア原則**。キーワード設定は MaterialPostProcessor に集約され、GUI は直接キーワードを触らない」
- 「UIParticles は **標準シェーダのサブセット原則**。専用 enum (UICustomCoord) で選択肢を制限し、シェーダ側は共通 HLSL を使う」

この言語化があれば、後続の設計案で「既存原則に沿うか / 逸脱するか」が即座に判断できる。

調査範囲が広い場合は Explore エージェントを並列起動して効率化できる（読み取りのみなので安全）。

### Step 2: 設計案を 2〜3 案発散（既存パターンとの整合性を必ず評価）

すぐに 1 案に絞らず、**複数案を発散** させてから比較する。
案ごとに「採用したらどう実装するか」「どこで詰まるか」を簡潔に書く。

例:
- **Plan A**: Alpha Transition の拡張モードとして実装 — 既存配線を流用できるが、モード分岐が複雑化
- **Plan B**: 独立機能として実装 — 配線一式の新規追加が必要だが、既存機能に副作用ゼロ
- **Plan C**: シェーダキーワードなしで動的分岐 — バリアント数は増えないが OFF 時コスト 0 を満たせない

#### 各案を 2 軸で評価する

1. **既存原則との整合**: Step 1 で言語化した「この領域の既存原則」と整合するか / 逸脱するか
2. **新規導入インフラ**: 既存にない仕組み（新規 keyword / 新規 varying・TEXCOORD スロット / 新規テクスチャサンプラ / 新規 enum / 新規 GUI パターン）をどれだけ追加するか

#### 採用判断の優先順

1. **既存原則に整合する案を最優先する**。既存と同じ仕組みで実現できるなら、それを選ぶ
2. 既存原則に逸脱する案を採用する場合、**明示的な理由** が必要
   例: 性能要件で既存パターンが破綻する、新規ユースケースで既存パターンが構造的に対応できない、等
3. 「新しい仕組みを足したほうがエレガント / 拡張性がある」は採用理由として **弱い**。既存原則を尊重したほうが、レビュー時間 / 学習コスト / 将来の改修コストが小さい

「既存原則と異質である」ことは **客観的な棄却理由** として書いてよい（既存設計と異質な案はそれ自体がメンテナンスコスト要因）。

### Step 3: 採用案を選び、棄却案と理由を記録、公開インタフェースを明示

採用は 1 案だが、**棄却案も plan.md に残す**。後から「なぜこの設計にしたのか」が辿れる。

棄却理由は具体的に書く:
- ❌ 「複雑だから」← 主観的
- ✅ 「モード分岐が TintColorMode と同種の後方互換 enum 問題を再生産し、UoW 数が 2 倍になる」← 客観的
- ✅ 「既存の機能トグルはすべて MaterialPostProcessor 集約でキーワード管理しており、GUI 直接設定を持ち込む本案はその原則と異質」← 既存原則整合を理由にしてよい

#### 公開インタフェースの選定を明示する

採用設計セクションに以下を明記する（intent.md の対象シェーダ + Step 1 で確認した既存パターンと整合させる）:

- **プロパティ設計**: プロパティ名（`_FeatureName...` 命名規則）、型、デフォルト値、Custom Coord 対応の有無
- **キーワード設計**: キーワード名、`shader_feature_local_vertex` / `_fragment` の選定理由
- **enum 追加の有無**: 既存 enum の末尾追加か新規 enum か（**既存 enum の順序変更は後方互換を壊すため禁止**。TintColorMode の教訓）
- **GUI 配置**: ParticlesUberCommonGUI のどのセクションに挿入するか（既存ヘッダの順序を尊重）
- **UIParticles 対応**: 対応する場合、.xy 制約下で成立するか

### Step 4: UoW に分解

採用設計を実装単位 (UoW) に分解する。各 UoW は:

- **対象**: 編集ファイル / ディレクトリ
- **依存**: 他のどの UoW が完了している必要があるか
- **並列可能か**: 領域独立な UoW は同時実装できる（依存関係を明示しておくと将来役立つ）
- **担当**: AI 実装 / 人間作業 / 両方
  - **人間作業推奨**: テストシーン・マテリアルの Unity Editor 上での配置、期待画像のキャプチャ（特に AI が実行できない側の OS）、デモ演出の調整などは Unity Editor 上で対話的にやる方が早い

UoW 粒度の目安: **1 UoW = コミット 1〜2 個分**。これより大きいと Phase 3 での進捗管理が難しくなる。

典型的な UoW 構成（新機能追加の場合）:
1. HLSL / シェーダ実装（Unlit → Lit → UIParticles の順で波及）
2. エディタ配線（PropertyNames / MaterialProperties / GUI / PostProcessor / ErrorHandler）
3. テスト（テストシーン更新 or 追加、期待画像更新）
4. ドキュメント（`documentation/{FeatureName}_Specification.md` — 英語・テンプレート準拠、CLAUDE.md の仕様書リスト更新）
5. デモ / サンプル（必要な場合）

### Step 5: 配置先の確認

各 UoW の編集対象がどのフォルダに着地するかを [./nova-repo-map.md](./nova-repo-map.md) で確認し、plan.md に明記する。
`documentation/` に置くファイルは英語 + documentation_guidelines.md 準拠が必要になる点を UoW に反映する。

### Step 6: テンプレートに沿って plan.md を書く

```bash
cp .claude/skills/nova-ai-dlc/assets/plan-template.md \
   docs/ai-dlc/<date>-<topic-slug>/plan.md
```

埋める項目:
- 採用設計（**プロパティ / キーワード / GUI のインタフェース設計** を含む）
- 棄却した代替案と理由（既存原則との整合性を観点に含めてよい）
- UoW 一覧（対象 / 依存 / 担当 / 配置先）
- 並列可能ペア
- 触ってはいけないファイル
- 検証手段（Intent の品質ゲートを具体のコマンド / ツールに落とす）

### Step 7: 内容の最終確認

ユーザーに plan.md の概要を提示する。
**全文の貼り付けは不要**。要約 + 「全文は `docs/ai-dlc/.../plan.md` 参照」で十分。

ユーザーが UoW 分解・採用案・棄却理由について意見を持つ可能性が高いので、
ファイルを直接編集してもらう運用も歓迎する。

### Step 8: 完了メッセージ

```
✅ Phase 2 (Inception) 完了
   出力: docs/ai-dlc/<date>-<topic-slug>/plan.md
   採用設計: <Plan B の名前>
   UoW: <N> 件

次は Phase 3 (Construction) です。
コンテキストをリセットしてから実行することを推奨します（任意）:

   /clear
   /nova-ai-dlc <トピック>を実装
```

## このフェーズで AskUserQuestion を使う場面

- ✅ 採用案が複数の妥当な選択肢に分かれる時
- ✅ ユーザーが特定の UoW を「自分でやる / AI に任せる」を選ぶ時
- ❌ 「この UoW 分解で良いですか？」のような全体確認（plan.md を見せて誘導すれば十分）

## 注意事項

- **既存実装を読まずに設計提案しない** — Step 1 のチェックリストをすべて埋めるまで Step 2 に進まない。これを破ると、Phase 2 内で何度も「既存パターンと異質だった」「配線チェーンの一部を見落としていた」で案を出し直すことになる
- **既存原則に逸脱する案を採用する時は理由を明示する** — 「新しい仕組みを足したほうが拡張性がある」だけでは不十分。intent の要件 / 性能制約 / 構造的限界など客観的な裏付けを書く
- **既存 enum の順序・値を変えない** — 後方互換を壊す（TintColorMode が BaseMapMode と順序が異なるのはこの理由）。追加は末尾のみ
- **公開インタフェースの選定は intent の対象シェーダを入力にする** — intent.md に対象シェーダバリアントが書かれていなければ Phase 1 に戻って確認する
- **棄却案を消さない** — 「なぜこの設計か」の歴史的記録。後で「別案で行けば良かった」と気付いた時に参照する
- **人間作業 UoW を明示する** — 期待画像キャプチャや Editor 上の配置作業を AI に任せようとすると Phase 3 で破綻する
- **Intent の SMAV 不足を後から修正しない** — もし intent.md の Completion Criteria が曖昧で Phase 2 が進まない場合は、Phase 1 に戻る判断をする
