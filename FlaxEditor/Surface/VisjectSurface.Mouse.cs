﻿////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2012-2017 Flax Engine. All rights reserved.
////////////////////////////////////////////////////////////////////////////////////

using FlaxEngine;

namespace FlaxEditor.Surface
{
    public partial class VisjectSurface
    {
        /// <summary>
        /// Gets the node under the mouse location.
        /// </summary>
        /// <returns>The node or null if no intersection.</returns>
        private SurfaceNode GetNodeUnderMouse()
        {
            // TODO: optimize it later -> cals mouse pos in children space and perform box vs point test faster
            if (GetChildAt(_mousePos) is SurfaceNode node)
                return node;
            return null;
        }

        private void UpdateSelectionRectangle()
        {
            // TODO: finish selecting ndoes with rect
            /*var selectionRect = Rectangle.FromPoints(_leftMouseDownPos, _mousePos) - _viewOffset;
            
            // Find nodes to select
            for (int i = 0; i < _nodes.Count; i++)
            {
                _nodes[i].IsSelected = _nodes[i]->GetBounds().Intersects(selectionRect);
            }*/
        }

        /// <inheritdoc />
        public override void OnMouseEnter(Vector2 location)
        {
            _lastBoxUnderMouse = null;

            // Cache mouse location
            _mousePos = location;

            base.OnMouseEnter(location);
        }

        /// <inheritdoc />
        public override void OnMouseMove(Vector2 location)
        {
            _lastBoxUnderMouse = null;

            // Cache mouse location
            _mousePos = location;

            // Moving around surface with mouse
            if (_rightMouseDown)
            {
                // Calculate delta
                Vector2 delta = location - _rightMouseDownPos;
                if (delta.LengthSquared > 0.01f)
                {
                    // Move view
                    _mouseMoveAmount += delta.Length;
                    _viewOffset += delta;
                    _rightMouseDownPos = location;
                    Cursor = CursorType.SizeAll;
                }

                // Handled
                return;
            }

            // Check if user is selecting or moving node(s)
            if (_leftMouseDown)
            {
                if (_startBox != null) // Connecting
                {
                }
                else if (_isMovingSelection) // Moving
                {
                    // Calculate delta
                    Vector2 delta = location - _leftMouseDownPos;
                    if (delta.LengthSquared > 0.01f)
                    {
                        // Move selected nodes
                        delta /= Scale;
                        for (int i = 0; i < _nodes.Count; i++)
                        {
                            var node = _nodes[i];
                            if (node.IsSelected)
                            {
                                node.Location += delta;
                            }
                        }
                        _leftMouseDownPos = location;
                        Cursor = CursorType.SizeAll;
                        MarkAsEdited(false);
                    }

                    // Handled
                    return;
                }
                else // Selecting
                {
                    UpdateSelectionRectangle();
                    
                    // Handled
                    return;
                }
            }

            base.OnMouseMove(location);
        }

        /// <inheritdoc />
        public override void OnMouseLeave()
        {
            // Clear flags
            if (_leftMouseDown)
            {
                _leftMouseDown = false;
            }
            if (_startBox != null)
            {
                ConnectingEnd(null);
            }
            if (_rightMouseDown)
            {
                _rightMouseDown = false;
                Cursor = CursorType.Default;
            }

            base.OnMouseLeave();
        }

        /// <inheritdoc />
        public override bool OnMouseWheel(Vector2 location, int delta)
        {
            if (IsMouseOver)
            {
                // Change scale
                SetScale(_targeScale + delta * 0.0008f);
            }

            return base.OnMouseWheel(location, delta);
        }

        /// <inheritdoc />
        public override bool OnMouseDown(Vector2 location, MouseButtons buttons)
        {
            // Check if user is connecting boxes
            if (_startBox != null)
                return true;

            // Cache data
            _isMovingSelection = false;
            _mousePos = location;
            if (buttons == MouseButtons.Left)
            {
                _leftMouseDown = true;
                _leftMouseDownPos = location;
            }
            if (buttons == MouseButtons.Right)
            {
                _rightMouseDown = true;
                _rightMouseDownPos = location;
            }

            // Check if any node is under the mouse
            SurfaceNode nodeAtMouse = GetNodeUnderMouse();
            Vector2 cLocation = location - _viewOffset;
            if (nodeAtMouse != null)
            {
                // Check if mouse is over header and user is pressing mouse left button
                if (_leftMouseDown && nodeAtMouse.HitsHeader(ref cLocation))
                {
                    _isMovingSelection = true;

                    // Check if user is pressing control
                    if (ParentWindow.GetKey(KeyCode.CONTROL))
                    {
                        // Add to selection
                        AddToSelection(nodeAtMouse);
                    }
                    // Check if node isn't selected
                    else if (!nodeAtMouse.IsSelected)
                    {
                        // Select node
                        Select(nodeAtMouse);
                    }

                    // Start moving
                    _leftMouseDown = true;
                    _leftMouseDownPos = location;

                    Focus();
                    return true;
                }
            }
            else
            {
                // Cache flags and state
                if (_leftMouseDown)
                {
                    ClearSelection();
                    Focus();
                    return true;
                }
                if (_rightMouseDown)
                {
                    Focus();
                    return true;
                }
            }

            // Base
            if (base.OnMouseDown(location, buttons))
            {
                // Clear flags to disable handling mouse events by itself (children should do)
                _leftMouseDown = _rightMouseDown = false;
                return true;
            }

            Focus();
            return true;
        }

        /// <inheritdoc />
        public override bool OnMouseUp(Vector2 location, MouseButtons buttons)
        {
            // Cache mouse location
            _mousePos = location;

            // Check if any node is under the mouse
            SurfaceNode nodeAtMouse = GetNodeUnderMouse();
            if (nodeAtMouse == null)
            {
                // Check if no move has been made at all
                if (_mouseMoveAmount < 5.0f)
                {
                    ClearSelection();
                }
            }

            // Cache flags and state
            if (_leftMouseDown && buttons == MouseButtons.Left)
            {
                _leftMouseDown = false;
                Cursor = CursorType.Default;

                if (!_isMovingSelection && _startBox == null)
                {
                    UpdateSelectionRectangle();
                }
            }
            if (_rightMouseDown && buttons == MouseButtons.Right)
            {
                _rightMouseDown = false;
                Cursor = CursorType.Default;

                // Check if no move has been made at all
                if (_mouseMoveAmount < 3.0f)
                {
                    // Check if any node is under the mouse
                    _cmStartPos = location - _viewOffset;
                    if (nodeAtMouse != null)
                    {
                        // Show secondary context menu
                        ShowSecondaryCM(nodeAtMouse, _cmStartPos);
                    }
                    else
                    {
                        // Show primary context menu
                        ShowPrimaryMenu(_cmStartPos);
                    }
                }
                _mouseMoveAmount = 0;
            }

            // Base
            if (base.OnMouseUp(location, buttons))
                return true;

            if (buttons == MouseButtons.Left)
            {
                ConnectingEnd(null);
            }

            Focus();
            return true;
        }

        /// <inheritdoc />
        public override void OnKeyUp(KeyCode key)
        {
            if (ContainsFocus)
            {
                if (key == KeyCode.DELETE)
                {
                    DeleteSelection();
                }
            }

            base.OnKeyUp(key);
        }
    }
}
