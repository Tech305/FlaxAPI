// Copyright (c) 2012-2020 Wojciech Figat. All rights reserved.
// This code was generated by a tool. Changes to this file may cause
// incorrect behavior and will be lost if the code is regenerated.

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using FlaxEngine;

namespace FlaxEditor
{
    /// <summary>
    /// Foliage tools for editor. Allows to spawn and modify foliage instances.
    /// </summary>
    public static partial class FoliageTools
    {
        /// <summary>
        /// Paints the foliage instances using the given foliage types selection and the brush location.
        /// </summary>
        /// <param name="foliage">The foliage actor.</param>
        /// <param name="foliageTypesIndices">The non-empty array of foliage types indices. Each array element is a zero-based index of the foliage instance type descriptor to use for painting.</param>
        /// <param name="brushPosition">The brush location (world space).</param>
        /// <param name="brushRadius">The brush radius (world space).</param>
        /// <param name="additive">True if paint with new instance, otherwise will remove existing ones.</param>
#if UNIT_TEST_COMPILANT
        [Obsolete("Unit tests, don't support methods calls.")]
#endif
        [UnmanagedCall]
        public static void Paint(Foliage foliage, int[] foliageTypesIndices, Vector3 brushPosition, float brushRadius, bool additive)
        {
#if UNIT_TEST_COMPILANT
            throw new NotImplementedException("Unit tests, don't support methods calls. Only properties can be get or set.");
#else
            Internal_Paint(FlaxEngine.Object.GetUnmanagedPtr(foliage), foliageTypesIndices, ref brushPosition, brushRadius, additive);
#endif
        }

        #region Internal Calls

#if !UNIT_TEST_COMPILANT
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Internal_Paint(IntPtr foliage, int[] foliageTypesIndices, ref Vector3 brushPosition, float brushRadius, bool additive);
#endif

        #endregion
    }
}
