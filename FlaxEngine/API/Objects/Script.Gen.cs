// Copyright (c) 2012-2020 Wojciech Figat. All rights reserved.
// This code was generated by a tool. Changes to this file may cause
// incorrect behavior and will be lost if the code is regenerated.

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FlaxEngine
{
    /// <summary>
    /// Base class from which every every script derives.
    /// </summary>
    public abstract partial class Script : Object
    {
        /// <summary>
        /// Enable/disable script updates.
        /// </summary>
        [UnmanagedCall]
        [HideInEditor]
        public bool Enabled
        {
#if UNIT_TEST_COMPILANT
            get; set;
#else
            get { return Internal_GetEnabled(unmanagedPtr); }
            set { Internal_SetEnabled(unmanagedPtr, value); }
#endif
        }

        /// <summary>
        /// Gets or sets the actor owning that script.
        /// </summary>
        /// <remarks>
        /// Changing script parent breaks any existing prefab links.
        /// </remarks>
        [UnmanagedCall]
        [HideInEditor, NoAnimate]
        public Actor Actor
        {
#if UNIT_TEST_COMPILANT
            get; set;
#else
            get { return Internal_GetActor(unmanagedPtr); }
            set { Internal_SetActor(unmanagedPtr, FlaxEngine.Object.GetUnmanagedPtr(value)); }
#endif
        }

        /// <summary>
        /// Gets or sets zero-based index in parent actor scripts list.
        /// </summary>
        [UnmanagedCall]
        [HideInEditor, NoAnimate]
        public int OrderInParent
        {
#if UNIT_TEST_COMPILANT
            get; set;
#else
            get { return Internal_GetOrderInParent(unmanagedPtr); }
            set { Internal_SetOrderInParent(unmanagedPtr, value); }
#endif
        }

        /// <summary>
        /// Gets a value indicating whether this script has a valid linkage to the prefab asset.
        /// </summary>
        [UnmanagedCall]
        public bool HasPrefabLink
        {
#if UNIT_TEST_COMPILANT
            get; set;
#else
            get { return Internal_HasPrefabLink(unmanagedPtr); }
#endif
        }

        /// <summary>
        /// Gets the prefab asset ID. Empty if no prefab link exists.
        /// </summary>
        [UnmanagedCall]
        public Guid PrefabID
        {
#if UNIT_TEST_COMPILANT
            get; set;
#else
            get { Guid resultAsRef; Internal_GetPrefabID(unmanagedPtr, out resultAsRef); return resultAsRef; }
#endif
        }

        /// <summary>
        /// Gets the ID of the object within a script that is used for synchronization with this script. Empty if no prefab link exists.
        /// </summary>
        [UnmanagedCall]
        public Guid PrefabObjectID
        {
#if UNIT_TEST_COMPILANT
            get; set;
#else
            get { Guid resultAsRef; Internal_GetPrefabObjectID(unmanagedPtr, out resultAsRef); return resultAsRef; }
#endif
        }

        /// <summary>
        /// Breaks the prefab linkage for this script.
        /// </summary>
#if UNIT_TEST_COMPILANT
        [Obsolete("Unit tests, don't support methods calls.")]
#endif
        [UnmanagedCall]
        [NoAnimate]
        public void BreakPrefabLink()
        {
#if UNIT_TEST_COMPILANT
            throw new NotImplementedException("Unit tests, don't support methods calls. Only properties can be get or set.");
#else
            Internal_BreakPrefabLink(unmanagedPtr);
#endif
        }

        /// <summary>
        /// Tries to find the script of the given type in all the loaded scenes.
        /// </summary>
        /// <param name="type">The type of the script to find.</param>
        /// <returns>Script instance if found, null otherwise.</returns>
#if UNIT_TEST_COMPILANT
        [Obsolete("Unit tests, don't support methods calls.")]
#endif
        [UnmanagedCall]
        public static Script Find(Type type)
        {
#if UNIT_TEST_COMPILANT
            throw new NotImplementedException("Unit tests, don't support methods calls. Only properties can be get or set.");
#else
            return Internal_FindByType(type);
#endif
        }

        #region Internal Calls

#if !UNIT_TEST_COMPILANT
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool Internal_GetEnabled(IntPtr obj);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Internal_SetEnabled(IntPtr obj, bool val);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern Actor Internal_GetActor(IntPtr obj);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Internal_SetActor(IntPtr obj, IntPtr val);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern int Internal_GetOrderInParent(IntPtr obj);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Internal_SetOrderInParent(IntPtr obj, int val);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool Internal_HasPrefabLink(IntPtr obj);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Internal_GetPrefabID(IntPtr obj, out Guid resultAsRef);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Internal_GetPrefabObjectID(IntPtr obj, out Guid resultAsRef);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Internal_BreakPrefabLink(IntPtr obj);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern Script Internal_FindByType(Type type);
#endif

        #endregion
    }
}
