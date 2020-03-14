// Copyright (c) 2012-2020 Wojciech Figat. All rights reserved.
// This code was generated by a tool. Changes to this file may cause
// incorrect behavior and will be lost if the code is regenerated.

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FlaxEngine
{
    /// <summary>
    /// Physics joint that maintains an upper or lower (or both) bound on the distance between two bodies.
    /// </summary>
    [Serializable]
    public sealed partial class DistanceJoint : Joint
    {
        /// <summary>
        /// Creates new <see cref="DistanceJoint"/> object.
        /// </summary>
        private DistanceJoint() : base()
        {
        }

        /// <summary>
        /// Creates new instance of <see cref="DistanceJoint"/> object.
        /// </summary>
        /// <returns>Created object.</returns>
#if UNIT_TEST_COMPILANT
        [Obsolete("Unit tests, don't support methods calls.")]
#endif
        [UnmanagedCall]
        public static DistanceJoint New()
        {
#if UNIT_TEST_COMPILANT
            throw new NotImplementedException("Unit tests, don't support methods calls. Only properties can be get or set.");
#else
            return Internal_Create(typeof(DistanceJoint)) as DistanceJoint;
#endif
        }

        /// <summary>
        /// Gets or sets the joint mode flags. Controls joint behaviour.
        /// </summary>
        [UnmanagedCall]
        [EditorOrder(100), DefaultValue(DistanceJointFlag.MinDistance | DistanceJointFlag.MaxDistance), EditorDisplay("Joint"), Tooltip("The joint mode flags. Controls joint behaviour.")]
        public DistanceJointFlag Flags
        {
#if UNIT_TEST_COMPILANT
            get; set;
#else
            get { return Internal_GetFlags(unmanagedPtr); }
            set { Internal_SetFlags(unmanagedPtr, value); }
#endif
        }

        /// <summary>
        /// Gets or sets the allowed minimum distance for the joint.
        /// </summary>
        /// <remarks>
        /// Used only when DistanceJointFlag.MinDistance flag is set. The minimum distance must be no more than the maximum distance. Default: 0, Range: [0, float.MaxValue].
        /// </remarks>
        [UnmanagedCall]
        [EditorOrder(110), DefaultValue(0.0f), Limit(0.0f), EditorDisplay("Joint"), Tooltip("The allowed minimum distance for the joint. Used only when DistanceJointFlag.MinDistance flag is set. The minimum distance must be no more than the maximum distance.")]
        public float MinDistance
        {
#if UNIT_TEST_COMPILANT
            get; set;
#else
            get { return Internal_GetMinDistance(unmanagedPtr); }
            set { Internal_SetMinDistance(unmanagedPtr, value); }
#endif
        }

        /// <summary>
        /// Gets or sets the allowed maximum distance for the joint.
        /// </summary>
        /// <remarks>
        /// Used only when DistanceJointFlag.MaxDistance flag is set. The maximum distance must be no less than the minimum distance. Default: 0, Range: [0, float.MaxValue].
        /// </remarks>
        [UnmanagedCall]
        [EditorOrder(120), DefaultValue(10.0f), Limit(0.0f), EditorDisplay("Joint"), Tooltip("The allowed maximum distance for the joint. Used only when DistanceJointFlag.MinDistance flag is set. The maximum distance must be no less than the minimum distance.")]
        public float MaxDistance
        {
#if UNIT_TEST_COMPILANT
            get; set;
#else
            get { return Internal_GetMaxDistance(unmanagedPtr); }
            set { Internal_SetMaxDistance(unmanagedPtr, value); }
#endif
        }

        /// <summary>
        /// Gets or sets the error tolerance of the joint.
        /// </summary>
        /// <remarks>
        /// The distance beyond the joint's [min, max] range before the joint becomes active. Default: 25, Range: [0.1, float.MaxValue].
        /// </remarks>
        [UnmanagedCall]
        [EditorOrder(130), DefaultValue(25.0f), Limit(0.0f), EditorDisplay("Joint"), Tooltip("The error tolerance of the joint.")]
        public float Tolerance
        {
#if UNIT_TEST_COMPILANT
            get; set;
#else
            get { return Internal_GetTolerance(unmanagedPtr); }
            set { Internal_SetTolerance(unmanagedPtr, value); }
#endif
        }

        /// <summary>
        /// Gets or sets the spring parameters.
        /// </summary>
        /// <remarks>
        /// Used only when DistanceJointFlag.Spring flag is set.
        /// </remarks>
        [UnmanagedCall]
        [EditorOrder(140), EditorDisplay("Joint"), Tooltip("The spring parameters. Used only when DistanceJointFlag.Spring flag is set.")]
        public SpringParameters SpringParameters
        {
#if UNIT_TEST_COMPILANT
            get; set;
#else
            get { SpringParameters resultAsRef; Internal_GetSpringParameters(unmanagedPtr, out resultAsRef); return resultAsRef; }
            set { Internal_SetSpringParameters(unmanagedPtr, ref value); }
#endif
        }

        /// <summary>
        /// Gets the current distance of the joint.
        /// </summary>
        [UnmanagedCall]
        public float CurrentDistance
        {
#if UNIT_TEST_COMPILANT
            get; set;
#else
            get { return Internal_GetCurrentDistance(unmanagedPtr); }
#endif
        }

        #region Internal Calls

#if !UNIT_TEST_COMPILANT
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern DistanceJointFlag Internal_GetFlags(IntPtr obj);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Internal_SetFlags(IntPtr obj, DistanceJointFlag val);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern float Internal_GetMinDistance(IntPtr obj);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Internal_SetMinDistance(IntPtr obj, float val);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern float Internal_GetMaxDistance(IntPtr obj);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Internal_SetMaxDistance(IntPtr obj, float val);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern float Internal_GetTolerance(IntPtr obj);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Internal_SetTolerance(IntPtr obj, float val);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Internal_GetSpringParameters(IntPtr obj, out SpringParameters resultAsRef);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Internal_SetSpringParameters(IntPtr obj, ref SpringParameters val);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern float Internal_GetCurrentDistance(IntPtr obj);
#endif

        #endregion
    }
}
