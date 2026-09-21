# モジュール（ユニパケ）の作り方

境月箱に載るツールは「モジュール」として作る。以下の型を守れば、境月箱が自動で拾って一覧に並べる。

## 1. 前提
- 開発プロジェクトに境月箱（土台）を入れておく（VCC で追加）
- モジュールは境月箱のアセンブリ `Kyogetsukan.Kyogetsubako.Editor` を参照する

## 2. モジュールの最小形

`Assets/<ツール名>/Editor/` に asmdef とスクリプトを置く。

asmdef（例: `Kyogetsukan.MyTool.Editor.asmdef`）:

```json
{
  "name": "Kyogetsukan.MyTool.Editor",
  "rootNamespace": "Kyogetsukan.MyTool",
  "references": [ "Kyogetsukan.Kyogetsubako.Editor" ],
  "includePlatforms": [ "Editor" ],
  "autoReferenced": true
}
```

実装（例: `MyTool.cs`）:

```csharp
using UnityEditor;
using UnityEngine;
using Kyogetsukan.Kyogetsubako;

namespace Kyogetsukan.MyTool
{
    public class MyTool : IKyogetsukanModule
    {
        public string Title => "ツール名（和名）";
        public string Subtitle => "English Name";
        public string Description => "何をするツールかを一文で。";
        public Color Accent { get { ColorUtility.TryParseHtmlString("#cf9f52", out var c); return c; } }

        public void Draw()
        {
            // ここに実機能。EditorGUILayout で自由に組む。
            if (GUILayout.Button("実行")) Debug.Log("実行した");
        }
    }
}
```

- 実機能はこのモジュール側に書く。土台側には置かない（土台だけで動いてしまうため）。
- 1つのユニパケに複数モジュール（複数の `IKyogetsukanModule` 実装）を入れてもよい。

## 3. ユニパケとして書き出す
1. `Assets/<ツール名>/` フォルダを選択
2. 右クリック → Export Package...
3. 土台本体（`Packages` 配下）が **含まれない** ことを確認して書き出す
   （土台は利用者が VCC で別途入れる前提）
4. 出来た `.unitypackage` を配布する

## 4. 配布ページに書くこと
- 「先に無料の『境月箱』（VCC）を入れてから、このユニパケをインポートしてください」
- 「境月箱が入っていないとコンパイルエラーになります」

## 5. 更新
- モジュールを更新したら、利用者はユニパケを入れ直す（＝再インポート）。
- もちアプデ方式を組み込めば、この入れ直しをボタン一発にできる（今後の拡張）。

## 6. 差し込み口を壊さないこと
`IKyogetsukanModule`（v1）の既存項目は変更・削除しない。機能追加は別インターフェースを足す形で行う。
ここを壊すと、これまでに出した全モジュールが動かなくなる。
