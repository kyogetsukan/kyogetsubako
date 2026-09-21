using UnityEngine;

namespace Kyogetsukan.Kyogetsubako
{
    /// <summary>
    /// 境月箱に載るモジュールの差し込み口（v1）。
    /// モジュール（ユニパケ）側はこれを実装したクラスを1つ以上含める。
    /// 境月箱はプロジェクト内の実装を自動で見つけて一覧に出す。
    ///
    /// ★ この差し込み口は互換性のため安定させる。既存の項目は変更・削除しない。
    ///    機能追加は「別インターフェースを足す」形で行い、v1 は壊さないこと。
    /// </summary>
    public interface IKyogetsukanModule
    {
        /// <summary>一覧・見出しに出る和名。</summary>
        string Title { get; }
        /// <summary>英語名などの副題（任意。空でも可）。</summary>
        string Subtitle { get; }
        /// <summary>説明の一文。</summary>
        string Description { get; }
        /// <summary>モジュール固有のアクセント色（一覧の点・見出しに使う）。</summary>
        Color Accent { get; }
        /// <summary>詳細ペインに描かれる中身（IMGUI）。</summary>
        void Draw();
    }
}
