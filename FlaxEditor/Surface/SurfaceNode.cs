////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2012-2017 Flax Engine. All rights reserved.
////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using FlaxEditor.Surface.Elements;
using FlaxEngine;
using FlaxEngine.Assertions;
using FlaxEngine.GUI;

namespace FlaxEditor.Surface
{
    /// <summary>
    /// Visject Surface node control.
    /// </summary>
    /// <seealso cref="FlaxEngine.GUI.ContainerControl" />
    public class SurfaceNode : ContainerControl
    {
        private Rectangle _headerRect;
        private Rectangle _closeButtonRect;
        private Rectangle _footerRect;
        private Vector2 _mousePosition;
        private bool _isSelected;

        /// <summary>
        /// The surface.
        /// </summary>
        public readonly VisjectSurface Surface;

        /// <summary>
        /// The node archetype.
        /// </summary>
        public readonly NodeArchetype Archetype;

        /// <summary>
        /// The group archetype.
        /// </summary>
        public readonly GroupArchetype GroupArchetype;

        /// <summary>
        /// The elements collection.
        /// </summary>
        public readonly List<ISurfaceNodeElement> Elements = new List<ISurfaceNodeElement>();

        /// <summary>
        /// Gets a value indicating whether this node is selected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this node is selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsSelected
        {
            get => _isSelected;
            internal set { _isSelected = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SurfaceNode"/> class.
        /// </summary>
        /// <param name="surface">The surface.</param>
        /// <param name="nodeArch">The node archetype.</param>
        /// <param name="groupArch">The group archetype.</param>
        public SurfaceNode(VisjectSurface surface, NodeArchetype nodeArch, GroupArchetype groupArch)
            : base(true, 0, 0, nodeArch.Size.X + Constants.NodeMarginX * 2, nodeArch.Size.Y + Constants.NodeMarginY * 2 + Constants.NodeHeaderSize + Constants.NodeFooterSize)
        {
            Surface = surface;
            Archetype = nodeArch;
            GroupArchetype = groupArch;
        }

        internal void AddElement(ISurfaceNodeElement element)
        {
            Elements.Add(element);
            if (element is Control control)
                AddChild(control);
        }

        internal void RemoveElemenet(ISurfaceNodeElement element)
        {
            if (element is Control control)
                RemoveChild(control);
            Elements.Remove(element);
        }

        internal bool HitsHeader(ref Vector2 location)
        {
            return _headerRect.MakeOffseted(Location).Contains(ref location);
        }
        
        /// <summary>
        /// Remeove all connections from and to that node.
        /// </summary>
        public void RemoveConnections()
        {
            for (int i = 0; i < Elements.Count; i++)
            {
                if(Elements[i] is Box box)
                    box.RemoveConnections();
            }

            UpdateBoxesTypes();
        }

        /// <summary>
        /// Updates dependant/independent boxes types.
        /// </summary>
        public void UpdateBoxesTypes()
        {
            // Check there is no need to use box types depedency feature
            if (Archetype.DependentBoxes == null || Archetype.IndependentBoxes == null)
            {
                // Back
                return;
            }

            // Get type to assign to all dependent boxes
            ConnectionType type = Archetype.DefaultType;
            for (int i = 0; i < Archetype.IndependentBoxes.Length; i++)
            {
                if (Archetype.IndependentBoxes[i] == -1)
                    break;
                var b = GetBox(Archetype.IndependentBoxes[i]);
                if (b != null && b.HasAnyConnection)
                {
                    // Check if that type if part of default type
                    if ((Archetype.DefaultType & b.Connections[0].DefaultType) != 0)
                    {
                        type = b.Connections[0].CurrentType;
                        break;
                    }
                }
            }

            // Assign connection type
            for (int i = 0; i < Archetype.DependentBoxes.Length; i++)
            {
                if (Archetype.DependentBoxes[i] == -1)
                    break;
                var b = GetBox(Archetype.DependentBoxes[i]);
                if (b != null)
                {
                    // Set new type
                    b.CurrentType = type;
                }
            }

            // Validate minor independent boxes to fit main one
            for (int i = 0; i < Archetype.IndependentBoxes.Length; i++)
            {
                if (Archetype.IndependentBoxes[i] == -1)
                    break;
                var b = GetBox(Archetype.IndependentBoxes[i]);
                if (b != null)
                {
                    // Set new type
                    b.CurrentType = type;
                }
            }
        }

        /// <summary>
        /// Tries to get box with given ID.
        /// </summary>
        /// <param name="id">The box id.</param>
        /// <returns>Box or null if cannot find.</returns>
        public Box GetBox(int id)
        {
            // TODO: maybe create local cache for boxes? but not a dictionary, use lookup table because ids are usally small (less than 20)
            for (int i = 0; i < Elements.Count; i++)
            {
                if (Elements[i] is Box box && box.ID == id)
                    return box;
            }
            return null;
        }

        /// <summary>
        /// Called when node gets loaded and elements are created.
        /// </summary>
        public virtual void OnLoaded()
        {
        }

        /// <summary>
        /// Called when surface gets loaded and boxes are connected.
        /// </summary>
        public virtual void OnSurfaceLoaded()
        {
            UpdateRectangles();
            UpdateBoxesTypes();
        }

        /// <summary>
        /// Updates the given box connection.
        /// </summary>
        /// <param name="box">The box.</param>
        public virtual void ConnectionTick(Box box)
        {
            UpdateBoxesTypes();
        }

        /// <summary>
        /// Updates the cached rectangles on node size change.
        /// </summary>
        protected virtual void UpdateRectangles()
        {
            const float footerSize = Constants.NodeFooterSize;
            const float headerSize = Constants.NodeHeaderSize;
            const float closeButtonMargin = Constants.NodeCloseButtonMargin;
            const float closeButtonSize = Constants.NodeCloseButtonSize;
            _headerRect = new Rectangle(0, 0, Width, headerSize);
            _closeButtonRect = new Rectangle(Width - closeButtonSize - closeButtonMargin, closeButtonMargin, closeButtonSize, closeButtonSize);
            _footerRect = new Rectangle(0, Height - footerSize, Width, footerSize);
        }

        /// <inheritdoc />
        public override void Draw()
        {
            var style = Style.Current;
            BackgroundColor = _isSelected ? Color.OrangeRed : style.BackgroundNormal;

            base.Draw();

            /*
            // Node layout lines rendering
            float marginX = SURFACE_NODE_MARGIN_X * _scale;
            float marginY = SURFACE_NODE_MARGIN_Y * _scale;
            float top = (SURFACE_NODE_HEADER_SIZE + SURFACE_NODE_MARGIN_Y) * _scale;
            float footer = SURFACE_NODE_FOOTER_SIZE * _scale;
            render.DrawRectangle(Rect(marginX, top, _width - 2 * marginX, _height - top - marginY - footer), Color::Red);
            */

            // Header
            Render2D.FillRectangle(_headerRect, style.BackgroundHighlighted);
            Render2D.DrawText(style.FontLarge, Archetype.Title, _headerRect, style.Foreground, TextAlignment.Center, TextAlignment.Center, TextWrapping.NoWrap, 1.0f);

            // Close button
            if ((Archetype.Flags & NodeFlags.NoCloseButton) == 0)
            {
                float alpha = _closeButtonRect.Contains(_mousePosition) ? 1.0f : 0.7f;
                Render2D.DrawSprite(style.Cross, _closeButtonRect, new Color(alpha));
            }

            // Footer
            Render2D.FillRectangle(_footerRect, GroupArchetype.Color);
        }

        /// <inheritdoc />
        public override void OnMouseEnter(Vector2 location)
        {
            _mousePosition = location;

            base.OnMouseEnter(location);
        }

        /// <inheritdoc />
        public override void OnMouseMove(Vector2 location)
        {
            _mousePosition = location;

            base.OnMouseMove(location);
        }

        /// <inheritdoc />
        public override void OnMouseLeave()
        {
            _mousePosition = Vector2.Minimum;

            base.OnMouseLeave();
        }

        /// <inheritdoc />
        public override bool OnMouseUp(Vector2 location, MouseButtons buttons)
        {
            if (base.OnMouseUp(location, buttons))
                return true;

            // Close
            if ((Archetype.Flags & NodeFlags.NoCloseButton) == 0)
            {
                if (_closeButtonRect.Contains(location))
                {
                    Surface.Delete(this);
                    return true;
                }
            }
            // Secondary Context Menu
            if (buttons == MouseButtons.Right)
            {
                Surface.ShowSecondaryCM(this, PointToParent(location));
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        protected override void SetScaleInternal(ref Vector2 scale)
        {
            base.SetScaleInternal(ref scale);

            UpdateRectangles();
        }
    }
}
