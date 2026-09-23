using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kyogetsukan.Kyogetsubako
{
    /// <summary>
    /// Scene ビューに出す、境月箱を開くための小さなボタン。
    /// Unity のオーバーレイなので、ドラッグしてどこにでも動かせる。初期位置は Scene ビュー右上、
    /// オーバーレイメニュー（「⋮」）の左隣になるようにしてある。
    /// どれかのモジュールが KyogetsubakoNotifications 経由で知らせてくると、右上に赤ポチが出る。
    /// </summary>
    [Overlay(typeof(SceneView), Id, "境月箱",
        defaultDisplay = true,
        defaultDockZone = DockZone.TopToolbar,
        defaultDockIndex = int.MaxValue,
        defaultLayout = Layout.HorizontalToolbar)]
    public class KyogetsubakoOverlay : ToolbarOverlay
    {
        public const string Id = "kyogetsukan-kyogetsubako-overlay";

        KyogetsubakoOverlay() : base(KyogetsubakoOpenButton.Id) { }
    }

    [EditorToolbarElement(Id, typeof(SceneView))]
    class KyogetsubakoOpenButton : EditorToolbarButton
    {
        public const string Id = "Kyogetsukan/KyogetsubakoOpenButton";

        readonly VisualElement _dot;
        bool _lastState;
        double _nextCheck;

        public KyogetsubakoOpenButton()
        {
            text = "境月箱";
            tooltip = "境月箱を開く";
            clicked += KyogetsubakoWindow.Open;

            // 見やすいように、地の色を少し濃く・文字を太字に
            style.unityFontStyleAndWeight = FontStyle.Bold;
            style.backgroundColor = new Color(0.10f, 0.10f, 0.11f, 1f);
            style.borderTopLeftRadius = 4;
            style.borderTopRightRadius = 4;
            style.borderBottomLeftRadius = 4;
            style.borderBottomRightRadius = 4;

            // 「境月箱」の3文字で折り返さないように、行を1本に固定して幅の方に逃がす
            style.whiteSpace = WhiteSpace.NoWrap;
            style.fontSize = 11;
            style.paddingLeft = 4;
            style.paddingRight = 4;
            style.height = new StyleLength(StyleKeyword.Auto);
            style.flexShrink = 0;

            // 右上に出す赤ポチ。普段は隠しておく。
            _dot = new VisualElement();
            _dot.style.position = Position.Absolute;
            _dot.style.top = 2;
            _dot.style.right = 2;
            _dot.style.width = 6;
            _dot.style.height = 6;
            _dot.style.borderTopLeftRadius = 3;
            _dot.style.borderTopRightRadius = 3;
            _dot.style.borderBottomLeftRadius = 3;
            _dot.style.borderBottomRightRadius = 3;
            _dot.style.backgroundColor = new Color(0.95f, 0.25f, 0.25f);
            _dot.style.display = DisplayStyle.None;
            _dot.pickingMode = PickingMode.Ignore;
            Add(_dot);

            // ボタンが実際に画面に出ている間だけ見に行く（消えている間は無駄に動かさない）
            RegisterCallback<AttachToPanelEvent>(_ => EditorApplication.update += Tick);
            RegisterCallback<DetachFromPanelEvent>(_ => EditorApplication.update -= Tick);
        }

        void Tick()
        {
            // 毎フレーム見に行くと重いので、1秒おきにだけ確認する
            if (EditorApplication.timeSinceStartup < _nextCheck) return;
            _nextCheck = EditorApplication.timeSinceStartup + 1.0;

            var has = KyogetsubakoNotifications.AnyPending();
            if (has == _lastState) return;
            _lastState = has;
            _dot.style.display = has ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}
