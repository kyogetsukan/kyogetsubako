using System;
using System.Collections.Generic;

namespace Kyogetsukan.Kyogetsubako
{
    /// <summary>
    /// 「境月箱を開くボタン」に赤ポチを出すかどうかを、各モジュール側から知らせるための小さな仕組み。
    /// モジュールは、自分の状態を見て true/false を返す関数をここに登録しておくだけでいい。
    /// 箱そのものは中身（どのモジュールが何を知らせたいか）には興味を持たない。
    /// </summary>
    public static class KyogetsubakoNotifications
    {
        static readonly List<Func<bool>> _sources = new List<Func<bool>>();

        /// <summary>
        /// 「今、知らせたいことがあるか」を返す関数を登録する。
        /// これは頻繁に呼ばれるので、重い処理（通信など）はせず、すでに分かっている状態を返すだけにすること。
        /// </summary>
        public static void Register(Func<bool> hasNotification)
        {
            if (hasNotification != null) _sources.Add(hasNotification);
        }

        /// <summary>登録されているうちどれか一つでも true を返せば true。</summary>
        public static bool AnyPending()
        {
            foreach (var f in _sources)
            {
                try { if (f()) return true; }
                catch { /* モジュール側の不具合で箱ごと巻き込まれないようにする */ }
            }
            return false;
        }
    }
}
