# 境月箱 / kyogetsubako

境月館のツールを、これ一つに。境月館の各ツール（モジュール）を載せて呼び出す無料の土台（VPM パッケージ）。

- パッケージ: `jp.kyogetsukan.kyogetsubako`
- VCC 追加用 URL（Pages 有効化後）: `https://kyogetsukan.github.io/kyogetsubako/index.json`
- モジュールの作り方: [`docs/module-guide.md`](docs/module-guide.md)

## リポジトリ構成
```
package/                     … VPM パッケージ本体（配布されるのはここ）
  package.json
  Editor/
    Kyogetsukan.Kyogetsubako.Editor.asmdef
    IKyogetsukanModule.cs    … モジュールの差し込み口（v1・安定）
    KyogetsubakoWindow.cs    … 境月箱ウィンドウ（Tools > 境月館 > 境月箱）
  README.md
.github/workflows/release.yml … タグ push で zip 化・Release・index.json 更新
tools/make_index.py           … index.json を作る/更新する
docs/module-guide.md          … モジュールの作り方
```

## 初回セットアップ手順
1. GitHub でリポジトリ `kyogetsukan/kyogetsubako` を作成し、この中身を push する
2. リポジトリの Settings → Pages で、Source を `gh-pages` ブランチ（/root）に設定
   - まだ `gh-pages` が無い場合、最初のリリースで自動作成される
3. タグ `v0.1.0` を push する（例: `git tag v0.1.0 && git push origin v0.1.0`）
   - Actions が走り、Release に zip が付き、`gh-pages` の `index.json` が更新される
4. VCC / ALCOM の「Add Repository」に `https://kyogetsukan.github.io/kyogetsubako/index.json` を追加
5. プロジェクトに「kyogetsukan kyogetsubako」を追加 → `Tools > 境月館 > 境月箱` で開く

## 更新の出し方
- `package/package.json` の `version` を上げる（例: 0.1.0 → 0.1.1）
- 新しいタグ（例: `v0.1.1`）を push する
- Actions が index.json に版を足し、VCC に更新が出る

土台の更新は VCC から自動で届く。各モジュール（ユニパケ）を入れると有効化される。

MIT License.
