// Copyright (c) 2012-2020 Wojciech Figat. All rights reserved.

namespace FlaxEngine.GUI
{
    /// <summary>
    /// Border control that draws the border around the control edges (inner and outer sides).
    /// </summary>
    public class Border : Control
    {
        /// <summary>
        /// Gets or sets the color used to draw border lines.
        /// </summary>
        [EditorOrder(0), Tooltip("The color used to draw border lines.")]
        public Color BorderColor { get; set; }

        /// <summary>
        /// The border lines width.
        /// </summary>
        [EditorOrder(10), Limit(0, float.MaxValue, 0.1f), Tooltip("The border lines width.")]
        public float BorderWidth { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Border"/> class.
        /// </summary>
        public Border()
        {
            BorderColor = Color.White;
            BorderWidth = 2.0f;
        }

        /// <inheritdoc />
        public override void Draw()
        {
            base.Draw();

            Render2D.DrawRectangle(new Rectangle(Vector2.Zero, Size), BorderColor, BorderWidth);
        }
    }
}
