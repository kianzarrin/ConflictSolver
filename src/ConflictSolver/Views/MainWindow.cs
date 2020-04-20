// <copyright file="MainWindow.cs" company="dymanoid">
// Copyright (c) dymanoid. All rights reserved.
// </copyright>

using ConflictSolver.UI;
using UnityEngine;

namespace ConflictSolver.Views
{
    /// <summary>
    /// The mod's main window.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812", Justification = "Instantiated indirectly by Unity")]
    internal sealed class MainWindow : WindowBase<MainViewModel>
    {
        private Vector2 _scrollPosition;
        private GUIStyle _monotypeLabelStyle;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
            : base(Strings.ModName, Appearance.DefaultWindowBoundaries, resizable: true)
        {
        }

        /// <inheritdoc/>
        public override void Update()
        {
            base.Update();

            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.F1))
            {
                IsVisible = true;
            }
        }

        /// <summary>
        /// Draws the GUI elements of this window.
        /// </summary>
        /// <param name="skin">The skin this window is being drawn in.</param>
        protected override void DrawWindow(WindowSkin skin)
        {
            if (DataContext == null)
            {
                return;
            }

            if (DataContext.IsProcessingSnapshot)
            {
                DrawTools.CenterInArea(DrawProcessingLabel);
                return;
            }

            if (!DataContext.IsSnapshotAvailable)
            {
                DrawTools.CenterInArea(DrawSnapshotButton);
                return;
            }

            if (_monotypeLabelStyle is null)
            {
                _monotypeLabelStyle = new GUIStyle(skin.UnitySkin.label)
                {
                    font = Font.CreateDynamicFontFromOSFont(Appearance.MonotypeFontNames, Appearance.FontSize),
                };
            }

            GUILayout.BeginVertical();
            DrawHeader();
            DrawSnapshotResults();
            GUILayout.EndVertical();
        }

        private static void DrawProcessingLabel()
        {
            GUI.contentColor = Colors.Text;
            GUILayout.Label(Strings.Processing);
        }

        private void DrawSnapshotButton()
        {
            if (DrawTools.DrawButton(Strings.TakeSnapshot))
            {
                DataContext.TakeSnapshot();
            }
        }

        private void DrawHeader()
        {
            GUILayout.BeginHorizontal(GUILayout.ExpandHeight(false));

            bool actionRequested = DrawTools.DrawButton(Strings.ExpandAll);
            if (actionRequested)
            {
                DataContext.ExpandAll();
            }

            actionRequested = DrawTools.DrawButton(Strings.CollapseAll);
            if (actionRequested)
            {
                DataContext.CollapseAll();
            }

            actionRequested = DrawTools.DrawButton(Strings.CopyToClipboard);
            if (actionRequested)
            {
                DataContext.CopyToClipboard();
            }

            GUILayout.FlexibleSpace();

            actionRequested = DrawTools.DrawButton(Strings.DeleteSnapshot);
            if (actionRequested)
            {
                DataContext.Clear();
            }

            GUILayout.EndHorizontal();
            GUILayout.Space(10);
        }

        private void DrawSnapshotResults()
        {
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

            foreach (var mod in DataContext.Snapshot)
            {
                MonitoredModView.DrawModView(mod, _monotypeLabelStyle);
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndScrollView();
        }
    }
}
