﻿////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2012-2017 Flax Engine. All rights reserved.
////////////////////////////////////////////////////////////////////////////////////

using System;

namespace FlaxEngine.GUI.Docking
{
    /// <summary>
    /// Helper class used to handle docking windows dragging and docking.
    /// </summary>
    public class DockHintWindow
    {
        private FloatWindowDockPanel _toMove;

        private Vector2 _dragOffset;
        private Vector2 _defaultWindowSize;
        private Rectangle _rectDock;
        private Rectangle _rectWindow;
        private Vector2 _mouse;
        private DockState _toSet;
        private DockPanel _toDock;

        private Rectangle _rLeft, _rRight, _rBottom, _rUpper, _rCenter;

        private DockHintWindow(FloatWindowDockPanel toMove)
        {
            _toMove = toMove;
            _toSet = DockState.Float;
            var window = toMove.Window.NativeWindow;

            // Remove focus from drag target
            _toMove.Focus();
            _toMove.Defocus();

            // Focus window
            window.Focus();

            // Calculate dragging offset and move window to the destination position
            Vector2 mouse = Application.MousePosition;
            Vector2 baseWinPos = window.Position;
            _dragOffset = mouse - baseWinPos;

            // Get initial size
            _defaultWindowSize = window.Size;

            // Init proxy window
            Proxy.Init(ref _defaultWindowSize);
            
            // Bind events
            Proxy.Window.OnMouseUp += onMouseUp;
            Proxy.Window.OnMouseMove += onMouseMove;
            Proxy.Window.OnLostFocus += onLostFocus;

            // Start tracking mouse
            Proxy.Window.StartTrackingMouse(false);

            // Update window GUI
            Proxy.Window.GUI.PerformLayout();

            // Update rectangles
            updateRects();

            // Hide base window
            window.Hide();

            // Enable hit window presentation
            Proxy.Window.RenderingEnabled = true;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            // End tracking mouse
            Proxy.Window.EndTrackingMouse();

            // Disable rendering
            Proxy.Window.RenderingEnabled = false;

            // Unbind events
            Proxy.Window.OnMouseUp -= onMouseUp;
            Proxy.Window.OnMouseMove -= onMouseMove;
            Proxy.Window.OnLostFocus -= onLostFocus;
            
            // Hide the proxy
            Proxy.Hide();

            // Check if window won't be docked
            if (_toSet == DockState.Float)
            {
                var window = _toMove.Window.NativeWindow;
                Vector2 mouse = Application.MousePosition;

                // Move base window
                window.Position = mouse - _dragOffset;

                // Show base window
                window.Show();
            }
            else
            {
                bool hasNoChildPanels = _toMove.ChildPanelsCount == 0;

                // Check if window has only single tab
                if (hasNoChildPanels && _toMove.TabsCount == 1)
                {
                    // Dock window
                    _toMove.GetTab(0).Show(_toSet, _toDock);
                }
                // Check if dock as tab and has no child panels
                else if (hasNoChildPanels && _toSet == DockState.DockFill)
                {
                    // Dock all tabs
                    while (_toMove.TabsCount > 0)
                    {
                        _toMove.GetTab(0).Show(DockState.DockFill, _toDock);
                    }
                }
                else
                {
                    var selectedTab = _toMove.SelectedTab;

                    // Dock the first tab into the target location
                    var firstTab = _toMove.GetTab(0);
                    firstTab.Show(_toSet, _toDock);

                    // Dock rest of the tabs
                    while (_toMove.TabsCount > 0)
                    {
                        _toMove.GetTab(0).Show(DockState.DockFill, firstTab);
                    }

                    // Keep selected tab being selected
                    selectedTab?.SelectTab();
                }

                // Focus target window
                _toDock.ParentWindow.Focus();
            }
        }

        /// <summary>
        /// Creates the new dragging hit window.
        /// </summary>
        /// <param name="toMove">Floating dock panel to move.</param>
        /// <returns>The dock hint window object.</returns>
        public static DockHintWindow Create(FloatWindowDockPanel toMove)
        {
            if (toMove == null)
                throw new ArgumentNullException();

            return new DockHintWindow(toMove);
        }

        /// <summary>
        /// Creates the new dragging hit window.
        /// </summary>
        /// <param name="toMove">Dock window to move.</param>
        /// <returns>The dock hint window object.</returns>
        public static DockHintWindow Create(DockWindow toMove)
        {
            if (toMove == null)
                throw new ArgumentNullException();

            // Show floating
            toMove.ShowFloating();

            // Move window to the mouse position (with some offset for caption bar)
            var window = toMove.ParentWindow;
            Vector2 mouse = Application.MousePosition;
            window.NativeWindow.Position = mouse - new Vector2(8, 8);

            // Get floating panel
            var floatingPanelToMove = window.GetChild(0) as FloatWindowDockPanel;

            return new DockHintWindow(floatingPanelToMove);
        }

        /// <summary>
        /// Calculates window rectangle in the dock window.
        /// </summary>
        /// <param name="state">Window dock state.</param>
        /// <param name="rect">Dock panel rectangle.</param>
        /// <returns>Calculated window rectangle.</returns>
        public static Rectangle CalculateDockRect(DockState state, ref Rectangle rect)
        {
            Rectangle result = rect;
            switch (state)
            {
                case DockState.DockFill:
                    result.Location.Y += DockPanel.DefaultHeaderHeight;
                    result.Size.Y -= DockPanel.DefaultHeaderHeight;
                    break;
                case DockState.DockTop:
                    result.Size.Y *= DockPanel.DefaultSplitterValue;
                    break;
                case DockState.DockLeft:
                    result.Size.X *= DockPanel.DefaultSplitterValue;
                    break;
                case DockState.DockBottom:
                    result.Location.Y += result.Size.Y * (1 - DockPanel.DefaultSplitterValue);
                    result.Size.Y *= DockPanel.DefaultSplitterValue;
                    break;
                case DockState.DockRight:
                    result.Location.X += result.Size.X * (1 - DockPanel.DefaultSplitterValue);
                    result.Size.X *= DockPanel.DefaultSplitterValue;
                    break;
            }
            return result;
        }

        private void updateRects()
        {
            // Cache mouse position
            _mouse = Application.MousePosition;

            // Check intersection with any dock panel
            _toDock = _toMove.MasterPanel.HitTest(ref _mouse, _toMove);

            // Check dock state to use
            bool showProxyHints = _toDock != null;
            bool showBorderHints = showProxyHints;
            bool showCenterHint = showProxyHints;
            if (showProxyHints)
            {
                // For master panel disable docking at sides
                //if (_toDock->IsMaster())
                //	showBorderHints = false;

                // If moved window has not only tabs but also child panels disable docking as tab
                if (_toMove.ChildPanelsCount > 0)
                    showCenterHint = false;

                /////
                // disable docking windows with one or more dock panels inside
                if (_toMove.ChildPanelsCount > 0)
                    showBorderHints = false;
                /////

                // Get dock area
                _rectDock = _toDock.DockAreaBounds;

                // Cache dock rectangles
                Vector2 size = _rectDock.Size;
                Vector2 offset = _rectDock.Location;
                float BorderMargin = 4.0f;
                float ProxyHintWindowsSize2 = Proxy.HintWindowsSize * 0.5f;
                float CenterX = size.X * 0.5f;
                float CenterY = size.Y * 0.5f;
                _rUpper = new Rectangle(CenterX - ProxyHintWindowsSize2, BorderMargin, Proxy.HintWindowsSize, Proxy.HintWindowsSize) + offset;
                _rBottom = new Rectangle(CenterX - ProxyHintWindowsSize2, size.Y - Proxy.HintWindowsSize - BorderMargin, Proxy.HintWindowsSize, Proxy.HintWindowsSize) + offset;
                _rLeft = new Rectangle(BorderMargin, CenterY - ProxyHintWindowsSize2, Proxy.HintWindowsSize, Proxy.HintWindowsSize) + offset;
                _rRight = new Rectangle(size.X - Proxy.HintWindowsSize - BorderMargin, CenterY - ProxyHintWindowsSize2, Proxy.HintWindowsSize, Proxy.HintWindowsSize) + offset;
                _rCenter = new Rectangle(CenterX - ProxyHintWindowsSize2, CenterY - ProxyHintWindowsSize2, Proxy.HintWindowsSize, Proxy.HintWindowsSize) + offset;

                // Hit test
                DockState toSet = DockState.Float;
                if (showBorderHints)
                {
                    if (_rUpper.Contains(_mouse))
                        toSet = DockState.DockTop;
                    else if (_rBottom.Contains(_mouse))
                        toSet = DockState.DockBottom;
                    else if (_rLeft.Contains(_mouse))
                        toSet = DockState.DockLeft;
                    else if (_rRight.Contains(_mouse))
                        toSet = DockState.DockRight;
                }
                if (showCenterHint && _rCenter.Contains(_mouse))
                    toSet = DockState.DockFill;
                _toSet = toSet;

                // Show proxy hint windows
                Proxy.Down.Position = _rBottom.Location;
                Proxy.Left.Position = _rLeft.Location;
                Proxy.Right.Position = _rRight.Location;
                Proxy.Up.Position = _rUpper.Location;
                Proxy.Center.Position = _rCenter.Location;
            }
            else
            {
                _toSet = DockState.Float;
            }

            // Update proxy hint windows visibility
            Proxy.Down.IsVisible = showProxyHints & showBorderHints;
            Proxy.Left.IsVisible = showProxyHints & showBorderHints;
            Proxy.Right.IsVisible = showProxyHints & showBorderHints;
            Proxy.Up.IsVisible = showProxyHints & showBorderHints;
            Proxy.Center.IsVisible = showProxyHints & showCenterHint;

            // Calculate proxy/dock/window rectangles
            if (_toDock == null)
            {
                // Floating window over nothing
                _rectWindow = new Rectangle(_mouse - _dragOffset, _defaultWindowSize);
            }
            else
            {
                if (_toSet == DockState.Float)
                {
                    // Floating window over dock panel
                    _rectWindow = new Rectangle(_mouse - _dragOffset, _defaultWindowSize);
                }
                else
                {
                    // Use only part of the dock panel to show hint
                    _rectWindow = CalculateDockRect(_toSet, ref _rectDock);
                }
            }

            // Update proxy window
            Proxy.Window.ClientBounds = _rectWindow;
        }

        private void onMouseUp(Vector2 location, MouseButtons buttons)
        {
            if (buttons == MouseButtons.Left)
            {
                Dispose();
            }
        }

        private void onMouseMove(Vector2 mousePos)
        {

            updateRects();
        }

        private WindowHitCodes onHitTest(Vector2 mouse)
        {
            return WindowHitCodes.Transparent;
        }

        private void onLostFocus()
        {
            Dispose();
        }

        /// <summary>
        /// Contains helper proxy windows shared across docking panels. They are used to visualize docking window locations.
        /// </summary>
        public static class Proxy
        {
            public static FlaxEngine.Window Window;
            public static FlaxEngine.Window Left;
            public static FlaxEngine.Window Right;
            public static FlaxEngine.Window Up;
            public static FlaxEngine.Window Down;
            public static FlaxEngine.Window Center;

            public const float HintWindowsSize = 32.0f;

            /// <summary>
            /// Inits docking proxy windows.
            /// </summary>
            /// <param name="initSize">Initial size of the proxy window.</param>
            public static void Init(ref Vector2 initSize)
            {
                if (Window == null)
                {
                    // Create proxy window
                    CreateWindowSettings settings = CreateWindowSettings.Default;
                    settings.Title = "DockHint.Window";
                    settings.Size = initSize;
                    settings.AllowInput = true;
                    settings.AllowMaximize = false;
                    settings.AllowMinimize = false;
                    settings.HasBorder = false;
                    settings.HasSizingFrame = false;
                    settings.IsRegularWindow = false;
                    settings.SupportsTransparency = true;
                    settings.ShowInTaskbar = false;
                    settings.ShowAfterFirstPaint = true;
                    settings.IsTopmost = true;

                    Window = FlaxEngine.Window.Create(settings);

                    // Set opacity and background color
                    Window.Opacity = 0.6f;
                    Window.GUI.BackgroundColor = Style.Current.DragWindow;
                }
                else
                {
                    // Resize proxy
                    Window.ClientSize = initSize;
                }

                CreateProxy(ref Left, "DockHint.Left");
                CreateProxy(ref Right, "DockHint.Right");
                CreateProxy(ref Up, "DockHint.Up");
                CreateProxy(ref Down, "DockHint.Down");
                CreateProxy(ref Center, "DockHint.Center");
            }

            private static void CreateProxy(ref FlaxEngine.Window win, string name)
            {
                if (win != null)
                    return;

                CreateWindowSettings settings = CreateWindowSettings.Default;
                settings.Title = name;
                settings.Size = new Vector2(HintWindowsSize);
                settings.AllowInput = false;
                settings.AllowMaximize = false;
                settings.AllowMinimize = false;
                settings.HasBorder = false;
                settings.HasSizingFrame = false;
                settings.IsRegularWindow = false;
                settings.SupportsTransparency = true;
                settings.ShowInTaskbar = false;
                settings.ActivateWhenFirstShown = false;
                settings.IsTopmost = true;

                win = FlaxEngine.Window.Create(settings);

                win.Opacity = 0.6f;
                win.GUI.BackgroundColor = Style.Current.DragWindow;
            }

            /// <summary>
            /// Hides proxy windows.
            /// </summary>
            public static void Hide()
            {
                HideProxy(ref Window);
                HideProxy(ref Left);
                HideProxy(ref Right);
                HideProxy(ref Up);
                HideProxy(ref Down);
                HideProxy(ref Center);
            }

            private static void HideProxy(ref FlaxEngine.Window win)
            {
                if (win)
                {
                   win.Hide();
                }
            }

            /// <summary>
            /// Releases proxy data and windows.
            /// </summary>
            public static void Dispsoe()
            {
                DisposeProxy(ref Window);
                DisposeProxy(ref Left);
                DisposeProxy(ref Right);
                DisposeProxy(ref Up);
                DisposeProxy(ref Down);
                DisposeProxy(ref Center);
            }

            private static void DisposeProxy(ref FlaxEngine.Window win)
            {
                if (win)
                {
                    win.Close(ClosingReason.User);
                    win = null;
                }
            }
        }
    }
}
