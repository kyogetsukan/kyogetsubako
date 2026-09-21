using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kyogetsukan.Kyogetsubako
{
    /// <summary>
    /// 境月箱 本体（UI Toolkit）。プロジェクト内の IKyogetsukanModule 実装を拾い、
    /// 左：モジュール一覧／右：選択中モジュールの中身、で表示する。
    /// モジュール（ユニパケ）が無ければ空状態になる。
    /// </summary>
    public class KyogetsubakoWindow : EditorWindow
    {
        private readonly List<IKyogetsukanModule> _modules = new List<IKyogetsukanModule>();
        private int _selected = 0;

        private VisualElement _sidebarList;
        private VisualElement _detail;

        static Color C(string hex) { ColorUtility.TryParseHtmlString(hex, out var c); return c; }
        static readonly Color BG     = C("#0f1116");
        static readonly Color BAR    = C("#171a21");
        static readonly Color SIDE   = C("#12151b");
        static readonly Color LINE   = C("#262b36");
        static readonly Color SELBG  = C("#1c2230");
        static readonly Color TXT    = C("#e7ebf0");
        static readonly Color SUB    = C("#8892a0");
        static readonly Color CAP    = C("#7c8797");
        static readonly Color ACC    = C("#7aa2ff");
        static readonly Color PANEL  = C("#161a22");

        [MenuItem("Tools/境月館/境月箱")]
        public static void Open()
        {
            var w = GetWindow<KyogetsubakoWindow>();
            w.titleContent = new GUIContent("境月箱");
            w.minSize = new Vector2(720, 460);
        }

        private void Reload()
        {
            _modules.Clear();
            foreach (var t in TypeCache.GetTypesDerivedFrom<IKyogetsukanModule>())
            {
                if (t.IsAbstract || t.IsInterface) continue;
                if (t.GetConstructor(Type.EmptyTypes) == null) continue;
                try { _modules.Add((IKyogetsukanModule)Activator.CreateInstance(t)); }
                catch (Exception e) { Debug.LogWarning("[境月箱] " + t.Name + " 読み込み失敗: " + e.Message); }
            }
            _modules.Sort((a, b) => string.CompareOrdinal(a.Title, b.Title));
            if (_selected >= _modules.Count) _selected = 0;
        }

        public void CreateGUI()
        {
            Reload();
            var root = rootVisualElement;
            root.style.flexDirection = FlexDirection.Column;
            root.style.backgroundColor = BG;

            var bar = Row();
            bar.style.height = 44; bar.style.alignItems = Align.Center;
            bar.style.paddingLeft = 14; bar.style.paddingRight = 10; bar.style.backgroundColor = BAR;
            bar.style.borderBottomWidth = 1; bar.style.borderBottomColor = LINE;
            var logo = new Label("境月"); logo.style.unityFontStyleAndWeight = FontStyle.Bold; logo.style.fontSize = 16; logo.style.color = TXT;
            var logo2 = new Label("箱"); logo2.style.unityFontStyleAndWeight = FontStyle.Bold; logo2.style.fontSize = 16; logo2.style.color = ACC;
            bar.Add(logo); bar.Add(logo2);
            var sp = new VisualElement(); sp.style.flexGrow = 1; bar.Add(sp);
            var meta = new Label("モジュール " + _modules.Count); meta.style.fontSize = 11; meta.style.color = SUB; meta.style.marginRight = 10; bar.Add(meta);
            var reload = new Button(() => Rebuild()) { text = "再読み込み" }; reload.style.fontSize = 11; bar.Add(reload);
            root.Add(bar);

            var body = Row(); body.style.flexGrow = 1;
            var side = new VisualElement();
            side.style.width = 264; side.style.backgroundColor = SIDE;
            side.style.paddingTop = 10; side.style.paddingBottom = 10; side.style.paddingLeft = 8; side.style.paddingRight = 8;
            side.style.borderRightWidth = 1; side.style.borderRightColor = LINE;
            var sh = new Label("導入済みモジュール"); sh.style.fontSize = 11; sh.style.color = C("#6f7887"); sh.style.marginBottom = 6; sh.style.marginLeft = 6;
            side.Add(sh);
            _sidebarList = new ScrollView(ScrollViewMode.Vertical); _sidebarList.style.flexGrow = 1; side.Add(_sidebarList);
            var hint = new Label("対応モジュールを取り込むと、ここに自動で並びます。");
            hint.style.fontSize = 11; hint.style.color = C("#5c6572"); hint.style.whiteSpace = WhiteSpace.Normal;
            hint.style.marginTop = 8; hint.style.paddingTop = 8; hint.style.borderTopWidth = 1; hint.style.borderTopColor = LINE;
            side.Add(hint);
            body.Add(side);

            _detail = new ScrollView(ScrollViewMode.Vertical); _detail.style.flexGrow = 1;
            _detail.style.paddingTop = 18; _detail.style.paddingBottom = 18; _detail.style.paddingLeft = 20; _detail.style.paddingRight = 20;
            body.Add(_detail);
            root.Add(body);

            var foot = Row(); foot.style.height = 30; foot.style.alignItems = Align.Center;
            foot.style.paddingLeft = 14; foot.style.paddingRight = 14; foot.style.backgroundColor = SIDE;
            foot.style.borderTopWidth = 1; foot.style.borderTopColor = LINE;
            var f1 = new Label("境月箱"); f1.style.fontSize = 11; f1.style.color = C("#6f7887"); foot.Add(f1);
            var fsp = new VisualElement(); fsp.style.flexGrow = 1; foot.Add(fsp);
            var f2 = new Label("土台の更新はVCCから自動で届きます／モジュールを入れると使えます");
            f2.style.fontSize = 11; f2.style.color = C("#6f7887"); foot.Add(f2);
            root.Add(foot);

            BuildSidebar();
            ShowDetail();
        }

        private void Rebuild() { Reload(); rootVisualElement.Clear(); CreateGUI(); }

        private void BuildSidebar()
        {
            _sidebarList.Clear();
            if (_modules.Count == 0)
            {
                var empty = new Label("モジュールがありません");
                empty.style.fontSize = 12; empty.style.color = SUB; empty.style.marginTop = 6; empty.style.marginLeft = 6;
                _sidebarList.Add(empty);
                return;
            }
            for (int i = 0; i < _modules.Count; i++)
            {
                int idx = i; var m = _modules[i]; bool sel = idx == _selected;
                var row = new VisualElement();
                row.style.flexDirection = FlexDirection.Row; row.style.alignItems = Align.Center;
                row.style.paddingTop = 9; row.style.paddingBottom = 9; row.style.paddingLeft = 9; row.style.paddingRight = 9; row.style.marginBottom = 3;
                Radius(row, 8);
                if (sel) { row.style.backgroundColor = SELBG; row.style.borderLeftWidth = 3; row.style.borderLeftColor = m.Accent; }
                var dot = new VisualElement(); dot.style.width = 9; dot.style.height = 9; Radius(dot, 5); dot.style.backgroundColor = m.Accent; dot.style.marginRight = 10;
                row.Add(dot);
                var col = new VisualElement();
                var nm = new Label(m.Title); nm.style.fontSize = 13; nm.style.unityFontStyleAndWeight = FontStyle.Bold; nm.style.color = sel ? Color.white : TXT; col.Add(nm);
                if (!string.IsNullOrEmpty(m.Subtitle)) { var cap = new Label(m.Subtitle); cap.style.fontSize = 10; cap.style.color = CAP; col.Add(cap); }
                row.Add(col);
                row.RegisterCallback<MouseDownEvent>(_ => { _selected = idx; BuildSidebar(); ShowDetail(); });
                _sidebarList.Add(row);
            }
        }

        private void ShowDetail()
        {
            _detail.Clear();
            if (_modules.Count == 0)
            {
                var box = new Label("導入済みのモジュールがありません。\n対応モジュール（ユニパケ）を取り込むと、ここに表示されます。");
                box.style.whiteSpace = WhiteSpace.Normal; box.style.fontSize = 13; box.style.color = SUB;
                _detail.Add(box);
                return;
            }
            var m = _modules[_selected];
            var title = new Label(m.Title); title.style.fontSize = 22; title.style.unityFontStyleAndWeight = FontStyle.Bold; title.style.color = m.Accent; _detail.Add(title);
            if (!string.IsNullOrEmpty(m.Description))
            {
                var desc = new Label(m.Description); desc.style.fontSize = 13; desc.style.color = C("#aab4c1"); desc.style.whiteSpace = WhiteSpace.Normal; desc.style.marginTop = 6; _detail.Add(desc);
            }
            var div = new VisualElement(); div.style.height = 1; div.style.backgroundColor = LINE; div.style.marginTop = 14; div.style.marginBottom = 14; _detail.Add(div);

            var panel = new VisualElement();
            panel.style.backgroundColor = PANEL; Radius(panel, 10);
            panel.style.borderLeftWidth = 1; panel.style.borderRightWidth = 1; panel.style.borderTopWidth = 1; panel.style.borderBottomWidth = 1;
            panel.style.borderLeftColor = LINE; panel.style.borderRightColor = LINE; panel.style.borderTopColor = LINE; panel.style.borderBottomColor = LINE;
            panel.style.paddingTop = 12; panel.style.paddingBottom = 12; panel.style.paddingLeft = 14; panel.style.paddingRight = 14;
            var imgui = new IMGUIContainer(() => { try { m.Draw(); } catch (Exception e) { EditorGUILayout.HelpBox(e.Message, MessageType.Error); } });
            panel.Add(imgui); _detail.Add(panel);

            var by = new Label("この機能はモジュール「" + m.Title + "」が提供しています。");
            by.style.fontSize = 11; by.style.color = C("#6f7887"); by.style.marginTop = 12; _detail.Add(by);
        }

        private static VisualElement Row() { var e = new VisualElement(); e.style.flexDirection = FlexDirection.Row; return e; }
        private static void Radius(VisualElement e, int r)
        {
            e.style.borderTopLeftRadius = r; e.style.borderTopRightRadius = r;
            e.style.borderBottomLeftRadius = r; e.style.borderBottomRightRadius = r;
        }
    }
}
