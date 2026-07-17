# NovaShader リポジトリ配置マップ

Phase 2 (Inception) で UoW の配置先を決める時、Phase 3 (Construction) で実装に入る時に参照する。

このファイルは **「どこに何を置くか」** に特化した参照資料。
規約事項（`.meta` の扱い、documentation/ の英語必須、PR 言語など）は CLAUDE.md / documentation_guidelines.md に一元化されているので、そちらを参照すること（末尾の「規約は別ファイル」セクション参照）。

NovaShader は **単一リポジトリ**。SIRIUS のようなサブモジュール構成ではないため、PR は原則 1 本にまとまる（分けるとしてもレビューコスト目的の分割のみ）。

## 編集対象と配置先のマトリクス

| 作業 | 編集対象 | 場所 |
|---|---|---|
| Intent 起票 | intent.md | `docs/ai-dlc/<date>-<topic-slug>/` |
| Inception | plan.md | `docs/ai-dlc/<date>-<topic-slug>/` |
| シェーダ実装 | .shader / .hlsl | `Assets/Nova/Runtime/Core/Shaders/` |
| ランタイムスクリプト | C# | `Assets/Nova/Runtime/Core/Scripts/` |
| エディタ実装（GUI / プロパティ / キーワード / 検証） | C# | `Assets/Nova/Editor/Core/Scripts/` |
| シェーダ最適化系 | C# | `Assets/Nova/Editor/Core/Scripts/Optimizer/` |
| テストコード | C# | `Assets/Tests/Runtime/`, `Assets/Tests/Editor/` |
| テストシーン・マテリアル | Scene / Material | `Assets/Tests/Scenes/` |
| テスト期待画像 | png | `Assets/Tests/SuccessfulImages/Linear/<platform>/None/` |
| 機能仕様書（英語・テンプレ準拠） | md | `documentation/{FeatureName}_Specification.md` |
| 仕様書の画像 | png / gif | `documentation/Images/` |
| パッケージ利用者向けドキュメント | md | `Documentation~/` |
| 公開ドキュメント | md | `README.md`, `README_JA.md` |
| デモ | Scene / Material / スクリプト | `Assets/Demo/` |
| サンプル | Assets | `Assets/Samples/` |
| Claude 資産 | SKILL.md / agent 定義 | `.claude/skills/`, `.claude/agents/` |
| AI-DLC フロー自体の改善 | 本スキルや reference | `.claude/skills/nova-ai-dlc/` |

## コミットしない / 触らないもの

| 対象 | 理由 |
|---|---|
| `Assets/ActualImages/` | テスト失敗時の実画像出力（gitignore 済み） |
| `Assets/OptimizedShaders/` | OptimizedShaderGenerator の生成出力（gitignore 済み） |
| `Assets/Tests/expected.png` / `actual.png` | テスト一時ファイル（gitignore 済み） |
| `Packages/manifest.json` / `packages-lock.json` の環境起因差分 | 意図した依存変更のみコミット |
| `ProjectSettings/ProjectVersion.txt` の環境起因差分 | Unity バージョン更新を意図した変更のみコミット |
| `.meta` の AI 生成 | GUID 衝突リスク。Unity Editor 生成に限る |

## 新機能追加で典型的に触るファイル群（配線チェーン）

機能トグル + Custom Coord 対応の新機能を追加する場合の典型セット:

| 層 | ファイル |
|---|---|
| シェーダプロパティ / HLSL | `Assets/Nova/Runtime/Core/Shaders/ParticlesUber.hlsl`, `Particles.hlsl`, 各 `.shader` |
| プロパティ名定数 | `Assets/Nova/Editor/Core/Scripts/MaterialPropertyNames.cs` |
| マテリアルプロパティ | `Assets/Nova/Editor/Core/Scripts/ParticlesUberCommonMaterialProperties.cs` |
| GUI | `Assets/Nova/Editor/Core/Scripts/ParticlesUberCommonGUI.cs` |
| キーワード管理 | `Assets/Nova/Editor/Core/Scripts/ParticlesUberUnlitMaterialPostProcessor.cs` |
| Vertex Streams 検証 | `Assets/Nova/Editor/Core/Scripts/RendererErrorHandler.cs` |
| 最適化互換 | `Assets/Nova/Editor/Core/Scripts/Optimizer/`（キーワード保護リスト） |

詳細な拡張手順は `documentation/CustomCoord_SystemArchitecture.md` の Extension Guidelines を参照。

## 規約は別ファイル

このファイルでは規約を再掲しない。実装着手前に以下を必ず確認すること:

| 規約 | 参照先 |
|---|---|
| `documentation/` は英語 + 禁止事項あり | [CLAUDE.md](../../../../CLAUDE.md), [documentation_guidelines.md](../../../../documentation/documentation_guidelines.md) |
| 仕様書テンプレート | [FeatureSpecification_Template.md](../../../../documentation/FeatureSpecification_Template.md) |
| PR タイトル英語 / 本文・コミット日本語 | [CLAUDE.md](../../../../CLAUDE.md) |
| Unity 最低バージョン 2022.3 LTS / enum 後方互換 | [CLAUDE.md](../../../../CLAUDE.md) |

迷ったら CLAUDE.md を読む。このファイルは「**どこに何を置くか**」だけを答える。
