# docs/ai-dlc/ 運用ルール

このフォルダは AI 駆動開発フロー (AI-DLC) の中間成果物置き場。
フローの起点は `.claude/skills/nova-ai-dlc/SKILL.md`（`/nova-ai-dlc` で起動）。

## フォルダ構成

```
docs/ai-dlc/
└── <YYYY-MM-DD>-<topic-slug>/   # トピックごとに 1 フォルダ
    ├── intent.md                # Phase 1 (Intent) の成果物
    └── plan.md                  # Phase 2 (Inception) の成果物
```

## ルール

- フォルダ名は `<date>-<topic-slug>`。slug は実装手段ではなく **解決したい課題** で命名する
- ファイル名は artifact 名で固定（`intent.md` / `plan.md`）。日付やトピックをファイル名に入れない
- Phase 間の情報引き継ぎはすべてこのフォルダのファイル経由で行う（各 Phase の自己完結性を担保する）
- 要件が変わったら既存フォルダの intent.md / plan.md を更新する。別フォルダを作らない
- このフォルダの成果物は **日本語でよい**。`documentation/` フォルダ（英語必須・documentation_guidelines.md 適用）とは別物。完成した機能の正式な仕様書は `documentation/{FeatureName}_Specification.md` として英語で別途作成する
