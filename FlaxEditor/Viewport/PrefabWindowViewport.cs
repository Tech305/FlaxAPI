// Copyright (c) 2012-2018 Wojciech Figat. All rights reserved.

using System;
using System.Collections.Generic;
using FlaxEditor.Content;
using FlaxEditor.Gizmo;
using FlaxEditor.GUI.Drag;
using FlaxEditor.SceneGraph;
using FlaxEditor.SceneGraph.Actors;
using FlaxEditor.Viewport.Cameras;
using FlaxEditor.Viewport.Previews;
using FlaxEditor.Viewport.Widgets;
using FlaxEditor.Windows.Assets;
using FlaxEngine;
using FlaxEngine.GUI;
using FlaxEngine.Rendering;

namespace FlaxEditor.Viewport
{
    /// <summary>
    /// Editor viewport used by the <see cref="PrefabWindow"/>
    /// </summary>
    /// <seealso cref="PrefabWindow" />
    /// <seealso cref="PrefabPreview" />
    /// <seealso cref="IGizmoOwner" />
    public class PrefabWindowViewport : PrefabPreview, IEditorPrimitivesOwner
    {
        private readonly PrefabWindow _window;

        private readonly ViewportWidgetButton _gizmoModeTranslate;
        private readonly ViewportWidgetButton _gizmoModeRotate;
        private readonly ViewportWidgetButton _gizmoModeScale;

        private ViewportWidgetButton _translateSnappng;
        private ViewportWidgetButton _rotateSnapping;
        private ViewportWidgetButton _scaleSnapping;

        private readonly DragAssets _dragAssets = new DragAssets();
        private readonly DragActorType _dragActorType = new DragActorType();

        /// <summary>
        /// The transform gizmo.
        /// </summary>
        public readonly TransformGizmo TransformGizmo;

        /// <summary>
        /// The selection outline postFx.
        /// </summary>
        public SelectionOutline SelectionOutline;

        /// <summary>
        /// The editor primitives postFx.
        /// </summary>
        public EditorPrimitives EditorPrimitives;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrefabWindowViewport"/> class.
        /// </summary>
        /// <param name="window">Editor window.</param>
        public PrefabWindowViewport(PrefabWindow window)
        : base(true)
        {
            _window = window;
            _window.SelectionChanged += OnSelectionChanged;
            Undo = window.Undo;
            ViewportCamera = new FPSCamera();

            // Prepare rendering task
            Task.ActorsSource = ActorsSources.CustomActors;
            Task.Flags = ViewFlags.DefaultEditor & ~ViewFlags.EditorSprites;
            Task.End += RenderTaskOnEnd;

            // Create post effects
            SelectionOutline = FlaxEngine.Object.New<SelectionOutline>();
            SelectionOutline.SelectionGetter = () => _window.Selection;
            Task.CustomPostFx.Add(SelectionOutline);
            EditorPrimitives = FlaxEngine.Object.New<EditorPrimitives>();
            EditorPrimitives.DrawDebugDraw = false;
            EditorPrimitives.Viewport = this;
            Task.CustomPostFx.Add(EditorPrimitives);

            // Add transformation gizmo
            TransformGizmo = new TransformGizmo(this);
            TransformGizmo.OnApplyTransformation += ApplyTransform;
            TransformGizmo.ModeChanged += OnGizmoModeChanged;
            TransformGizmo.Duplicate += _window.Duplicate;
            Gizmos.Active = TransformGizmo;

            // Transform space widget
            var transformSpaceWidget = new ViewportWidgetsContainer(ViewportWidgetLocation.UpperRight);
            var transformSpaceToggle = new ViewportWidgetButton(string.Empty, window.Editor.Icons.World16, null, true)
            {
                Checked = TransformGizmo.ActiveTransformSpace == TransformGizmo.TransformSpace.World,
                TooltipText = "Gizmo transform space (world or local)",
                Parent = transformSpaceWidget
            };
            transformSpaceToggle.OnToggle += OnTransformSpaceToggle;
            transformSpaceWidget.Parent = this;

            // Scale snapping widget
            var scaleSnappingWidget = new ViewportWidgetsContainer(ViewportWidgetLocation.UpperRight);
            var enableScaleSnapping = new ViewportWidgetButton(string.Empty, window.Editor.Icons.ScaleStep16, null, true)
            {
                Checked = TransformGizmo.ScaleSnapEnabled,
                TooltipText = "Enable scale snapping",
                Parent = scaleSnappingWidget
            };
            enableScaleSnapping.OnToggle += OnScaleSnappingToggle;
            var scaleSnappingCM = new ContextMenu();
            _scaleSnapping = new ViewportWidgetButton(TransformGizmo.ScaleSnapValue.ToString(), Sprite.Invalid, scaleSnappingCM);
            _scaleSnapping.TooltipText = "Scale snapping values";
            for (int i = 0; i < EditorViewportScaleSnapValues.Length; i++)
            {
                var v = EditorViewportScaleSnapValues[i];
                var button = scaleSnappingCM.AddButton(v.ToString());
                button.Tag = v;
            }
            scaleSnappingCM.ButtonClicked += OnWidgetScaleSnapClick;
            scaleSnappingCM.VisibleChanged += OnWidgetScaleSnapShowHide;
            _scaleSnapping.Parent = scaleSnappingWidget;
            scaleSnappingWidget.Parent = this;

            // Rotation snapping widget
            var rotateSnappingWidget = new ViewportWidgetsContainer(ViewportWidgetLocation.UpperRight);
            var enableRotateSnapping = new ViewportWidgetButton(string.Empty, window.Editor.Icons.RotateStep16, null, true)
            {
                Checked = TransformGizmo.RotationSnapEnabled,
                TooltipText = "Enable rotation snapping",
                Parent = rotateSnappingWidget
            };
            enableRotateSnapping.OnToggle += OnRotateSnappingToggle;
            var rotateSnappingCM = new ContextMenu();
            _rotateSnapping = new ViewportWidgetButton(TransformGizmo.RotationSnapValue.ToString(), Sprite.Invalid, rotateSnappingCM);
            _rotateSnapping.TooltipText = "Rotation snapping values";
            for (int i = 0; i < EditorViewportRotateSnapValues.Length; i++)
            {
                var v = EditorViewportRotateSnapValues[i];
                var button = rotateSnappingCM.AddButton(v.ToString());
                button.Tag = v;
            }
            rotateSnappingCM.ButtonClicked += OnWidgetRotateSnapClick;
            rotateSnappingCM.VisibleChanged += OnWidgetRotateSnapShowHide;
            _rotateSnapping.Parent = rotateSnappingWidget;
            rotateSnappingWidget.Parent = this;

            // Translation snapping widget
            var translateSnappingWidget = new ViewportWidgetsContainer(ViewportWidgetLocation.UpperRight);
            var enableTranslateSnapping = new ViewportWidgetButton(string.Empty, window.Editor.Icons.Grid16, null, true)
            {
                Checked = TransformGizmo.TranslationSnapEnable,
                TooltipText = "Enable position snapping",
                Parent = translateSnappingWidget
            };
            enableTranslateSnapping.OnToggle += OnTranslateSnappingToggle;
            var translateSnappingCM = new ContextMenu();
            _translateSnappng = new ViewportWidgetButton(TransformGizmo.TranslationSnapValue.ToString(), Sprite.Invalid, translateSnappingCM);
            _translateSnappng.TooltipText = "Position snapping values";
            for (int i = 0; i < EditorViewportTranslateSnapValues.Length; i++)
            {
                var v = EditorViewportTranslateSnapValues[i];
                var button = translateSnappingCM.AddButton(v.ToString());
                button.Tag = v;
            }
            translateSnappingCM.ButtonClicked += OnWidgetTranslateSnapClick;
            translateSnappingCM.VisibleChanged += OnWidgetTranslateSnapShowHide;
            _translateSnappng.Parent = translateSnappingWidget;
            translateSnappingWidget.Parent = this;

            // Gizmo mode widget
            var gizmoMode = new ViewportWidgetsContainer(ViewportWidgetLocation.UpperRight);
            _gizmoModeTranslate = new ViewportWidgetButton(string.Empty, window.Editor.Icons.Translate16, null, true)
            {
                Tag = TransformGizmo.Mode.Translate,
                TooltipText = "Translate gizmo mode",
                Checked = true,
                Parent = gizmoMode
            };
            _gizmoModeTranslate.OnToggle += OnGizmoModeToggle;
            _gizmoModeRotate = new ViewportWidgetButton(string.Empty, window.Editor.Icons.Rotate16, null, true)
            {
                Tag = TransformGizmo.Mode.Rotate,
                TooltipText = "Rotate gizmo mode",
                Parent = gizmoMode
            };
            _gizmoModeRotate.OnToggle += OnGizmoModeToggle;
            _gizmoModeScale = new ViewportWidgetButton(string.Empty, window.Editor.Icons.Scale16, null, true)
            {
                Tag = TransformGizmo.Mode.Scale,
                TooltipText = "Scale gizmo mode",
                Parent = gizmoMode
            };
            _gizmoModeScale.OnToggle += OnGizmoModeToggle;
            gizmoMode.Parent = this;
        }

        private void RenderTaskOnEnd(SceneRenderTask task, GPUContext context)
        {
            // Render editor primitives, gizmo and debug shapes in debug view modes
            if (task.Mode != ViewMode.Default)
            {
                // Note: can use Output buffer as both input and output because EditorPrimitives is using a intermediate buffers
                EditorPrimitives.Render(context, task, task.Output, task.Output);
            }
        }

        /// <summary>
        /// Moves the viewport to visualize selected actors.
        /// </summary>
        public void ShowSelectedActors()
        {
            ((FPSCamera)ViewportCamera).ShowActors(TransformGizmo.SelectedParents);
        }

        /// <inheritdoc />
        public GizmosCollection Gizmos { get; } = new GizmosCollection();

        /// <inheritdoc />
        public float ViewFarPlane => _farPlane;

        /// <inheritdoc />
        public bool IsLeftMouseButtonDown => _input.IsMouseLeftDown;

        /// <inheritdoc />
        public bool IsRightMouseButtonDown => _input.IsMouseRightDown;

        /// <inheritdoc />
        public bool IsAltKeyDown => _input.IsAltDown;

        /// <inheritdoc />
        public Vector2 MouseDelta => _mouseDeltaLeft * 1000;

        /// <inheritdoc />
        public bool UseSnapping => Root.GetKey(Keys.Control);

        /// <inheritdoc />
        public bool UseDuplicate => Root.GetKey(Keys.Shift);

        /// <inheritdoc />
        public Undo Undo { get; }

        /// <inheritdoc />
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            for (int i = 0; i < Gizmos.Count; i++)
            {
                Gizmos[i].Update(deltaTime);
            }
        }

        private void OnGizmoModeToggle(ViewportWidgetButton button)
        {
            TransformGizmo.ActiveMode = (TransformGizmo.Mode)(int)button.Tag;
        }

        private void OnTranslateSnappingToggle(ViewportWidgetButton button)
        {
            TransformGizmo.TranslationSnapEnable = !TransformGizmo.TranslationSnapEnable;
        }

        private void OnRotateSnappingToggle(ViewportWidgetButton button)
        {
            TransformGizmo.RotationSnapEnabled = !TransformGizmo.RotationSnapEnabled;
        }

        private void OnScaleSnappingToggle(ViewportWidgetButton button)
        {
            TransformGizmo.ScaleSnapEnabled = !TransformGizmo.ScaleSnapEnabled;
        }

        private void OnTransformSpaceToggle(ViewportWidgetButton button)
        {
            TransformGizmo.ToggleTransformSpace();
        }

        private void OnGizmoModeChanged()
        {
            // Update all viewport widgets status
            var mode = TransformGizmo.ActiveMode;
            _gizmoModeTranslate.Checked = mode == TransformGizmo.Mode.Translate;
            _gizmoModeRotate.Checked = mode == TransformGizmo.Mode.Rotate;
            _gizmoModeScale.Checked = mode == TransformGizmo.Mode.Scale;
        }

        private static readonly float[] EditorViewportScaleSnapValues =
        {
            0.05f,
            0.1f,
            0.25f,
            0.5f,
            1.0f,
            2.0f,
            4.0f,
            6.0f,
            8.0f,
        };

        private void OnWidgetScaleSnapClick(ContextMenuButton button)
        {
            var v = (float)button.Tag;
            TransformGizmo.ScaleSnapValue = v;
            _scaleSnapping.Text = v.ToString();
        }

        private void OnWidgetScaleSnapShowHide(Control control)
        {
            if (control.Visible == false)
                return;

            var ccm = (ContextMenu)control;
            foreach (var e in ccm.Items)
            {
                if (e is ContextMenuButton b)
                {
                    var v = (float)b.Tag;
                    b.Icon = Mathf.Abs(TransformGizmo.ScaleSnapValue - v) < 0.001f
                             ? Style.Current.CheckBoxTick
                             : Sprite.Invalid;
                }
            }
        }

        private static readonly float[] EditorViewportRotateSnapValues =
        {
            1.0f,
            5.0f,
            10.0f,
            15.0f,
            30.0f,
            45.0f,
            60.0f,
            90.0f,
        };

        private void OnWidgetRotateSnapClick(ContextMenuButton button)
        {
            var v = (float)button.Tag;
            TransformGizmo.RotationSnapValue = v;
            _rotateSnapping.Text = v.ToString();
        }

        private void OnWidgetRotateSnapShowHide(Control control)
        {
            if (control.Visible == false)
                return;

            var ccm = (ContextMenu)control;
            foreach (var e in ccm.Items)
            {
                if (e is ContextMenuButton b)
                {
                    var v = (float)b.Tag;
                    b.Icon = Mathf.Abs(TransformGizmo.RotationSnapValue - v) < 0.001f
                             ? Style.Current.CheckBoxTick
                             : Sprite.Invalid;
                }
            }
        }

        private static readonly float[] EditorViewportTranslateSnapValues =
        {
            0.1f,
            0.5f,
            1.0f,
            5.0f,
            10.0f,
            100.0f,
            1000.0f,
        };

        private void OnWidgetTranslateSnapClick(ContextMenuButton button)
        {
            var v = (float)button.Tag;
            TransformGizmo.TranslationSnapValue = v;
            _translateSnappng.Text = v.ToString();
        }

        private void OnWidgetTranslateSnapShowHide(Control control)
        {
            if (control.Visible == false)
                return;

            var ccm = (ContextMenu)control;
            foreach (var e in ccm.Items)
            {
                if (e is ContextMenuButton b)
                {
                    var v = (float)b.Tag;
                    b.Icon = Mathf.Abs(TransformGizmo.TranslationSnapValue - v) < 0.001f
                             ? Style.Current.CheckBoxTick
                             : Sprite.Invalid;
                }
            }
        }

        private void OnSelectionChanged()
        {
            Gizmos.ForEach(x => x.OnSelectionChanged(_window.Selection));
        }

        /// <summary>
        /// Applies the transform to the collection of scene graph nodes.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="translationDelta">The translation delta.</param>
        /// <param name="rotationDelta">The rotation delta.</param>
        /// <param name="scaleDelta">The scale delta.</param>
        public void ApplyTransform(List<SceneGraphNode> selection, ref Vector3 translationDelta, ref Quaternion rotationDelta, ref Vector3 scaleDelta)
        {
            bool applyRotation = !rotationDelta.IsIdentity;
            bool useObjCenter = TransformGizmo.ActivePivot == TransformGizmo.PivotType.ObjectCenter;
            Vector3 gizmoPosition = TransformGizmo.Position;

            // Transform selected objects
            for (int i = 0; i < selection.Count; i++)
            {
                var obj = selection[i];
                var trans = obj.Transform;

                // Apply rotation
                if (applyRotation)
                {
                    Vector3 pivotOffset = trans.Translation - gizmoPosition;
                    if (useObjCenter || pivotOffset.IsZero)
                    {
                        //trans.Orientation *= rotationDelta;
                        trans.Orientation *= Quaternion.Invert(trans.Orientation) * rotationDelta * trans.Orientation;
                    }
                    else
                    {
                        Matrix.RotationQuaternion(ref trans.Orientation, out var transWorld);
                        Matrix.RotationQuaternion(ref rotationDelta, out var deltaWorld);
                        Matrix world = transWorld * Matrix.Translation(pivotOffset) * deltaWorld * Matrix.Translation(-pivotOffset);
                        trans.SetRotation(ref world);
                        trans.Translation += world.TranslationVector;
                    }
                }

                // Apply scale
                const float scaleLimit = 99_999_999.0f;
                trans.Scale = Vector3.Clamp(trans.Scale + scaleDelta, new Vector3(-scaleLimit), new Vector3(scaleLimit));

                // Apply translation
                trans.Translation += translationDelta;

                obj.Transform = trans;
            }
        }

        /// <inheritdoc />
        protected override void OnLeftMouseButtonUp()
        {
            // Skip if was controlling mouse or mouse is not over the area
            if (_prevInput.IsControllingMouse || !Bounds.Contains(ref _viewMousePos))
                return;

            if (TransformGizmo.IsActive)
            {
                // Ensure player is not moving objects
                if (TransformGizmo.ActiveAxis != TransformGizmo.Axis.None)
                    return;
            }
            else
            {
                // For now just pick objects in transform gizmo mode
                return;
            }

            // Get mouse ray and try to hit any object
            var ray = MouseRay;
            float closest = float.MaxValue;
            var hit = _window.Graph.Root.RayCast(ref ray, ref closest, SceneGraphNode.RayCastData.FlagTypes.SkipColliders);

            // Update selection
            if (hit != null)
            {
                // For child actor nodes (mesh, link or sth) we need to select it's owning actor node first or any other child node (but not a child actor)
                if (hit is ActorChildNode actorChildNode)
                {
                    var parentNode = actorChildNode.ParentNode;
                    bool canChildBeSelected = _window.Selection.Contains(parentNode);
                    if (!canChildBeSelected)
                    {
                        for (int i = 0; i < parentNode.ChildNodes.Count; i++)
                        {
                            if (_window.Selection.Contains(parentNode.ChildNodes[i]))
                            {
                                canChildBeSelected = true;
                                break;
                            }
                        }
                    }

                    if (!canChildBeSelected)
                    {
                        // Select parent
                        hit = parentNode;
                    }
                }

                bool addRemove = Root.GetKey(Keys.Control);
                bool isSelected = _window.Selection.Contains(hit);

                if (addRemove)
                {
                    if (isSelected)
                        _window.Deselect(hit);
                    else
                        _window.Select(hit, true);
                }
                else
                {
                    _window.Select(hit);
                }
            }
            else
            {
                _window.Deselect();
            }

            // Keep focus
            Focus();

            base.OnLeftMouseButtonUp();
        }

        /// <inheritdoc />
        public override bool OnKeyDown(Keys key)
        {
            if (key == Keys.Alpha1)
            {
                TransformGizmo.ActiveMode = TransformGizmo.Mode.Translate;
                return true;
            }
            if (key == Keys.Alpha2)
            {
                TransformGizmo.ActiveMode = TransformGizmo.Mode.Rotate;
                return true;
            }
            if (key == Keys.Alpha3)
            {
                TransformGizmo.ActiveMode = TransformGizmo.Mode.Scale;
                return true;
            }
            if (key == Keys.F)
            {
                ShowSelectedActors();
                return true;
            }

            return base.OnKeyDown(key);
        }

        /// <inheritdoc />
        public override DragDropEffect OnDragEnter(ref Vector2 location, DragData data)
        {
            var result = base.OnDragEnter(ref location, data);
            if (result != DragDropEffect.None)
                return result;

            if (_dragAssets.OnDragEnter(data, ValidateDragItem))
                result = _dragAssets.Effect;
            if (_dragActorType.OnDragEnter(data, ValidateDragActorType))
                result = _dragActorType.Effect;

            return result;
        }

        private bool ValidateDragItem(ContentItem contentItem)
        {
            switch (contentItem.ItemDomain)
            {
            case ContentDomain.Material:
            case ContentDomain.Model:
            case ContentDomain.Audio:
            case ContentDomain.Prefab: return true;
            default: return false;
            }
        }

        private bool ValidateDragActorType(Type actorType)
        {
            return true;
        }

        /// <inheritdoc />
        public override DragDropEffect OnDragMove(ref Vector2 location, DragData data)
        {
            var result = base.OnDragMove(ref location, data);
            if (result != DragDropEffect.None)
                return result;

            if (_dragAssets.HasValidDrag)
                return _dragAssets.Effect;
            if (_dragActorType.HasValidDrag)
                return _dragActorType.Effect;

            return DragDropEffect.None;
        }

        /// <inheritdoc />
        public override void OnDragLeave()
        {
            _dragAssets.OnDragLeave();
            _dragActorType.OnDragLeave();

            base.OnDragLeave();
        }

        private Vector3 PostProcessSpawnedActorLocation(Actor actor, ref Vector3 hitLocation)
        {
            BoundingBox box;
            Editor.GetActorEditorBox(actor, out box);

            // Place the object
            var location = hitLocation - (box.Size.Length * 0.5f) * ViewDirection;

            // Apply grid snapping if enabled
            if (UseSnapping || TransformGizmo.TranslationSnapEnable)
            {
                float snapValue = TransformGizmo.TranslationSnapValue;
                location = new Vector3(
                    (int)(location.X / snapValue) * snapValue,
                    (int)(location.Y / snapValue) * snapValue,
                    (int)(location.Z / snapValue) * snapValue);
            }

            return location;
        }

        private void Spawn(AssetItem item, SceneGraphNode hit, ref Vector3 hitLocation)
        {
            switch (item.ItemDomain)
            {
            case ContentDomain.Material:
            {
                if (hit is ModelActorNode.EntryNode meshNode)
                {
                    var material = FlaxEngine.Content.LoadAsync<MaterialBase>(item.ID);
                    using (new UndoBlock(Undo, meshNode.ModelActor, "Change material"))
                        meshNode.Entry.Material = material;
                }
                else if (hit is BoxBrushNode.SideLinkNode brushSurfaceNode)
                {
                    var material = FlaxEngine.Content.LoadAsync<MaterialBase>(item.ID);
                    using (new UndoBlock(Undo, brushSurfaceNode.Brush, "Change material"))
                        brushSurfaceNode.Surface.Material = material;
                }

                break;
            }
            case ContentDomain.Model:
            {
                if (item.TypeName == typeof(SkinnedModel).FullName)
                {
                    var model = FlaxEngine.Content.LoadAsync<SkinnedModel>(item.ID);
                    var actor = AnimatedModel.New();
                    actor.Name = item.ShortName;
                    actor.SkinnedModel = model;
                    actor.Position = PostProcessSpawnedActorLocation(actor, ref hitLocation);
                    Spawn(actor);
                }
                else
                {
                    var model = FlaxEngine.Content.LoadAsync<Model>(item.ID);
                    var actor = ModelActor.New();
                    actor.Name = item.ShortName;
                    actor.Model = model;
                    actor.Position = PostProcessSpawnedActorLocation(actor, ref hitLocation);
                    Spawn(actor);
                }

                break;
            }
            case ContentDomain.Audio:
            {
                var clip = FlaxEngine.Content.LoadAsync<AudioClip>(item.ID);
                var actor = AudioSource.New();
                actor.Name = item.ShortName;
                actor.Clip = clip;
                actor.Position = PostProcessSpawnedActorLocation(actor, ref hitLocation);
                Spawn(actor);

                break;
            }
            case ContentDomain.Prefab:
            {
                var prefab = FlaxEngine.Content.LoadAsync<Prefab>(item.ID);
                var actor = PrefabManager.SpawnPrefab(prefab, null);
                actor.Name = item.ShortName;
                actor.Position = PostProcessSpawnedActorLocation(actor, ref hitLocation);
                Spawn(actor);

                break;
            }
            default: throw new ArgumentOutOfRangeException();
            }
        }

        private void Spawn(Actor actor)
        {
            _window.Spawn(actor);
        }

        private void Spawn(Type item, SceneGraphNode hit, ref Vector3 hitLocation)
        {
            var actor = FlaxEngine.Object.New(item) as Actor;
            if (actor == null)
            {
                Editor.LogWarning("Failed to spawn actor of type " + item.FullName);
                return;
            }
            actor.Name = item.Name;
            actor.Position = PostProcessSpawnedActorLocation(actor, ref hitLocation);
            Spawn(actor);
        }

        /// <inheritdoc />
        public override DragDropEffect OnDragDrop(ref Vector2 location, DragData data)
        {
            var result = base.OnDragDrop(ref location, data);
            if (result != DragDropEffect.None)
                return result;

            // Check if drag sth
            Vector3 hitLocation = ViewPosition;
            SceneGraphNode hit = null;
            if (_dragAssets.HasValidDrag || _dragActorType.HasValidDrag)
            {
                // Get mouse ray and try to hit any object
                var ray = ConvertMouseToRay(ref location);
                float closest = float.MaxValue;
                hit = _window.Graph.Root.RayCast(ref ray, ref closest, SceneGraphNode.RayCastData.FlagTypes.SkipColliders);
                if (hit != null)
                {
                    // Use hit location
                    hitLocation = ray.Position + ray.Direction * closest;
                }
                else
                {
                    // Use area in front of the viewport
                    hitLocation = ViewPosition + ViewDirection * 100;
                }
            }

            // Drag assets
            if (_dragAssets.HasValidDrag)
            {
                result = _dragAssets.Effect;

                // Process items
                for (int i = 0; i < _dragAssets.Objects.Count; i++)
                {
                    var item = _dragAssets.Objects[i];
                    Spawn(item, hit, ref hitLocation);
                }
            }
            // Drag actor type
            else if (_dragActorType.HasValidDrag)
            {
                result = _dragActorType.Effect;

                // Process items
                for (int i = 0; i < _dragActorType.Objects.Count; i++)
                {
                    var item = _dragActorType.Objects[i];
                    Spawn(item, hit, ref hitLocation);
                }
            }

            return result;
        }

        /// <inheritdoc />
        public override void OnDestroy()
        {
            FlaxEngine.Object.Destroy(ref SelectionOutline);
            FlaxEngine.Object.Destroy(ref EditorPrimitives);

            base.OnDestroy();
        }

        /// <inheritdoc />
        public ViewportDebugDrawData DebugDrawData => null;
    }
}
