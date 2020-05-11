// Copyright (c) 2012-2020 Wojciech Figat. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FlaxEditor.CustomEditors.Elements;
using FlaxEngine;
using FlaxEngine.GUI;
using Utils = FlaxEditor.Utilities.Utils;

namespace FlaxEditor.CustomEditors.Editors
{
    /// <summary>
    /// Default implementation of the inspector used to edit key-value dictionaries.
    /// </summary>
    public class DictionaryEditor : CustomEditor
    {
        private IntegerValueElement _size;
        private int _elementsCount;
        private bool _readOnly;
        private bool _canReorderItems;
        private bool _notNullItems;

        /// <summary>
        /// Determines whether this editor[can edit the specified dictionary type.
        /// </summary>
        /// <param name="type">Type of the dictionary.</param>
        /// <returns>True if can edit, otherwise false.</returns>
        public static bool CanEditType(Type type)
        {
            // Ensure it's a generic dictionary type
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                var argTypes = type.GetGenericArguments();
                var keyType = argTypes[0];
                var valueType = argTypes[1];

                // Only int and string keys are supported
                if (keyType == typeof(int) || keyType == typeof(string))
                    return true;

                // TODO: add ReadOnlyDictionaryEditor - then show keys/values of any type but without editing
            }

            return false;
        }

        /// <summary>
        /// Gets the length of the collection.
        /// </summary>
        public int Count => (Values[0] as IDictionary)?.Count ?? 0;

        /// <inheritdoc />
        public override void Initialize(LayoutElementsContainer layout)
        {
            _readOnly = false;
            _canReorderItems = true;
            _notNullItems = false;

            // No support for different collections for now
            if (HasDifferentValues || HasDifferentTypes)
                return;

            var type = Values.Type;
            var size = Count;

            // Try get MemberCollectionAttribute for collection editor meta
            var attributes = Values.GetAttributes();
            Type overrideEditorType = null;
            if (attributes != null)
            {
                var memberCollection = (MemberCollectionAttribute)attributes.FirstOrDefault(x => x is MemberCollectionAttribute);
                if (memberCollection != null)
                {
                    // TODO: handle ReadOnly and NotNullItems by filtering child editors SetValue
                    // TODO: handle CanReorderItems

                    _readOnly = memberCollection.ReadOnly;
                    _canReorderItems = memberCollection.CanReorderItems;
                    _notNullItems = memberCollection.NotNullItems;
                    overrideEditorType = Utils.GetType(memberCollection.OverrideEditorTypeName);
                }
            }

            // Size
            if (_readOnly)
            {
                layout.Label("Size", size.ToString());
            }
            else
            {
                _size = layout.IntegerValue("Size");
                _size.IntValue.MinValue = 0;
                _size.IntValue.MaxValue = ushort.MaxValue;
                _size.IntValue.Value = size;
                _size.IntValue.ValueChanged += OnSizeChanged;
            }

            // Elements
            if (size > 0)
            {
                var argTypes = type.GetGenericArguments();
                var keyType = argTypes[0];
                var valueType = argTypes[1];
                var keysEnumerable = ((IDictionary)Values[0]).Keys.OfType<object>();
                var keys = keysEnumerable as object[] ?? keysEnumerable.ToArray();
                for (int i = 0; i < size; i++)
                {
                    var item = layout.CustomContainer<UniformGridPanel>();
                    var itemGrid = item.CustomControl;
                    itemGrid.Height = TextBox.DefaultHeight; // TODO: make slots auto sizable instead of fixed height
                    itemGrid.SlotsHorizontally = 2;
                    itemGrid.SlotsVertically = 1;

                    // Key
                    // TODO: allow edit keys
                    var key = keys.ElementAt(i);
                    item.Label(key.ToString());

                    // Value
                    var overrideEditor = overrideEditorType != null ? (CustomEditor)Activator.CreateInstance(overrideEditorType) : null;
                    item.Object(new DictionaryValueContainer(valueType, key, Values), overrideEditor);
                }
            }
            _elementsCount = size;

            // Add/Remove buttons
            if (!_readOnly)
            {
                var area = layout.Space(20);
                var addButton = new Button(area.ContainerControl.Width - (16 + 16 + 2 + 2), 2, 16, 16)
                {
                    Text = "+",
                    TooltipText = "Add new item",
                    AnchorPreset = AnchorPresets.TopRight,
                    Parent = area.ContainerControl
                };
                addButton.Clicked += () =>
                {
                    if (IsSetBlocked)
                        return;

                    Resize(Count + 1);
                };
                var removeButton = new Button(addButton.Right + 2, addButton.Y, 16, 16)
                {
                    Text = "-",
                    TooltipText = "Remove last item",
                    AnchorPreset = AnchorPresets.TopRight,
                    Parent = area.ContainerControl,
                    Enabled = size > 0
                };
                removeButton.Clicked += () =>
                {
                    if (IsSetBlocked)
                        return;

                    Resize(Count - 1);
                };
            }
        }

        private void OnSizeChanged()
        {
            if (IsSetBlocked)
                return;

            Resize(_size.IntValue.Value);
        }

        /// <summary>
        /// Resizes collection to the specified new size.
        /// </summary>
        /// <param name="newSize">The new size.</param>
        protected void Resize(int newSize)
        {
            var dictionary = Values[0] as IDictionary;
            var oldSize = dictionary?.Count ?? 0;

            if (oldSize != newSize)
            {
                // Allocate new collection
                var type = Values.Type;
                var argTypes = type.GetGenericArguments();
                var keyType = argTypes[0];
                var valueType = argTypes[1];
                var newValues = (IDictionary)Activator.CreateInstance(type);

                // Copy all keys/values
                int itemsLeft = newSize;
                if (dictionary != null)
                {
                    foreach (var e in dictionary.Keys)
                    {
                        if (itemsLeft == 0)
                            break;
                        newValues[e] = dictionary[e];
                        itemsLeft--;
                    }
                }

                // Insert new items (find unique keys)
                int newItesmLeft = newSize - oldSize;
                while (newItesmLeft-- > 0)
                {
                    if (keyType == typeof(int))
                    {
                        int uniqueKey = 0;
                        bool isUnique;
                        do
                        {
                            isUnique = true;
                            foreach (var e in newValues.Keys)
                            {
                                if ((int)e == uniqueKey)
                                {
                                    uniqueKey++;
                                    isUnique = false;
                                    break;
                                }
                            }
                        } while (!isUnique);

                        newValues[uniqueKey] = Utils.GetDefaultValue(valueType);
                    }
                    else if (keyType == typeof(string))
                    {
                        string uniqueKey = "Key";
                        bool isUnique;
                        do
                        {
                            isUnique = true;
                            foreach (var e in newValues.Keys)
                            {
                                if ((string)e == uniqueKey)
                                {
                                    uniqueKey += "*";
                                    isUnique = false;
                                    break;
                                }
                            }
                        } while (!isUnique);

                        newValues[uniqueKey] = Utils.GetDefaultValue(valueType);
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                }

                SetValue(newValues);
            }
        }

        /// <inheritdoc />
        public override void Refresh()
        {
            base.Refresh();

            // No support for different collections for now
            if (HasDifferentValues || HasDifferentTypes)
                return;

            // Check if collection has been resized (by UI or from external source)
            if (Count != _elementsCount)
            {
                RebuildLayout();
            }
        }
    }
}
