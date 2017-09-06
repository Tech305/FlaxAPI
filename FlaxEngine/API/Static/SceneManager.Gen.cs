////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2012-2017 Flax Engine. All rights reserved.
////////////////////////////////////////////////////////////////////////////////////
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Runtime.CompilerServices;

namespace FlaxEngine
{
	/// <summary>
	/// The scene manager handles the scenes collection and spawns/deleted actors.
	/// </summary>
	public static partial class SceneManager
	{
		/// <summary>
		/// Gets array of the loaded scenes.
		/// </summary>
		[UnmanagedCall]
		public static Scene[] Scenes
		{
#if UNIT_TEST_COMPILANT
			get; set;
#else
			get { return Internal_GetScenes(); }
#endif
		}

		/// <summary>
		/// Tries to find actor with given ID.
		/// </summary>
		/// <param name="id">The actor id.</param>
		/// <returns>Found actor or null if cannot find it.</returns>
#if UNIT_TEST_COMPILANT
		[Obsolete("Unit tests, don't support methods calls.")]
#endif
		[UnmanagedCall]
		public static Actor FindActor(Guid id) 
		{
#if UNIT_TEST_COMPILANT
			throw new NotImplementedException("Unit tests, don't support methods calls. Only properties can be get or set.");
#else
			return Internal_FindActorById(ref id);
#endif
		}

		/// <summary>
		/// Tries to find actor with given name.
		/// </summary>
		/// <param name="name">The actor name.</param>
		/// <returns>Found actor or null if cannot find it.</returns>
#if UNIT_TEST_COMPILANT
		[Obsolete("Unit tests, don't support methods calls.")]
#endif
		[UnmanagedCall]
		public static Actor FindActor(string name) 
		{
#if UNIT_TEST_COMPILANT
			throw new NotImplementedException("Unit tests, don't support methods calls. Only properties can be get or set.");
#else
			return Internal_FindActorByName(name);
#endif
		}

		/// <summary>
		/// Tries to find scene with given ID (checks only loaded scenes, see Scenes).
		/// </summary>
		/// <param name="id">The scene id.</param>
		/// <returns>Found scene or null if cannot find it (it's not loaded).</returns>
#if UNIT_TEST_COMPILANT
		[Obsolete("Unit tests, don't support methods calls.")]
#endif
		[UnmanagedCall]
		public static Scene FindScene(Guid id) 
		{
#if UNIT_TEST_COMPILANT
			throw new NotImplementedException("Unit tests, don't support methods calls. Only properties can be get or set.");
#else
			return Internal_FindScene(ref id);
#endif
		}

		/// <summary>
		/// Gets amount of loaded scenes.
		/// </summary>
		[UnmanagedCall]
		public static int LoadedScenesCount
		{
#if UNIT_TEST_COMPILANT
			get; set;
#else
			get { return Internal_GetLoadedScenesCount(); }
#endif
		}

		/// <summary>
		/// Checks if any scene has been loaded. Loaded scene means deserialzied and added to the scenes collection.
		/// </summary>
		[UnmanagedCall]
		public static bool IsAnySceneLoaded
		{
#if UNIT_TEST_COMPILANT
			get; set;
#else
			get { return Internal_IsAnySceneLoaded(); }
#endif
		}

		/// <summary>
		/// Checks if any scene has any actor.
		/// </summary>
		[UnmanagedCall]
		public static bool IsAnyActorInGame
		{
#if UNIT_TEST_COMPILANT
			get; set;
#else
			get { return Internal_IsAnyActorInGame(); }
#endif
		}

		/// <summary>
		/// Gets or sets value indicating whenever game logic is running (physics, script updates, etc.).
		/// </summary>
		[UnmanagedCall]
		public static bool IsGameLogicRunning
		{
#if UNIT_TEST_COMPILANT
			get; set;
#else
			get { return Internal_IsGameLogicRunning(); }
			set { Internal_SetGameLogicRunning(value); }
#endif
		}

		/// <summary>
		/// Checks if any scene action is pending.
		/// </summary>
		[UnmanagedCall]
		public static bool IsAnyAsyncActionPending
		{
#if UNIT_TEST_COMPILANT
			get; set;
#else
			get { return Internal_IsAnyActionPending(); }
#endif
		}

		/// <summary>
		/// Gets the last scene load time (in UTC).
		/// </summary>
		[UnmanagedCall]
		public static bool LastSceneLoadTime
		{
#if UNIT_TEST_COMPILANT
			get; set;
#else
			get { return Internal_GetLastSceneLoadTime(); }
#endif
		}

		/// <summary>
		/// Spawns actor on the scene.
		/// </summary>
		/// <param name="actor">The actor to spawn.</param>
		/// <param name="parent">The parent actor (will link spawned actor with this parent).</param>
#if UNIT_TEST_COMPILANT
		[Obsolete("Unit tests, don't support methods calls.")]
#endif
		[UnmanagedCall]
		public static void SpawnActor(Actor actor, Actor parent = null) 
		{
#if UNIT_TEST_COMPILANT
			throw new NotImplementedException("Unit tests, don't support methods calls. Only properties can be get or set.");
#else
			Internal_SpawnActor(Object.GetUnmanagedPtr(actor), Object.GetUnmanagedPtr(parent));
#endif
		}

		/// <summary>
		/// Saves scene to the asset.
		/// </summary>
		/// <param name="scene">The scene to serialize.</param>
		/// <returns>True if action fails, otherwise false.</returns>
#if UNIT_TEST_COMPILANT
		[Obsolete("Unit tests, don't support methods calls.")]
#endif
		[UnmanagedCall]
		public static bool SaveScene(Scene scene) 
		{
#if UNIT_TEST_COMPILANT
			throw new NotImplementedException("Unit tests, don't support methods calls. Only properties can be get or set.");
#else
			return Internal_SaveScene(Object.GetUnmanagedPtr(scene));
#endif
		}

		/// <summary>
		/// Saves scene to the bytes array.
		/// </summary>
		/// <param name="scene">The scene to serialize.</param>
		/// <returns>Bytes array with the serialized scene or null if operation failed (see log for error information).</returns>
#if UNIT_TEST_COMPILANT
		[Obsolete("Unit tests, don't support methods calls.")]
#endif
		[UnmanagedCall]
		public static byte[] SaveSceneToBytes(Scene scene) 
		{
#if UNIT_TEST_COMPILANT
			throw new NotImplementedException("Unit tests, don't support methods calls. Only properties can be get or set.");
#else
			return Internal_SaveSceneToBytes(Object.GetUnmanagedPtr(scene));
#endif
		}

		/// <summary>
		/// Saves scene to the asset. Done in the background.
		/// </summary>
		/// <param name="scene">The scene to serialize.</param>
#if UNIT_TEST_COMPILANT
		[Obsolete("Unit tests, don't support methods calls.")]
#endif
		[UnmanagedCall]
		public static void SaveSceneAsync(Scene scene) 
		{
#if UNIT_TEST_COMPILANT
			throw new NotImplementedException("Unit tests, don't support methods calls. Only properties can be get or set.");
#else
			Internal_SaveSceneAsync(Object.GetUnmanagedPtr(scene));
#endif
		}

		/// <summary>
		/// Saves scene to the asset.
		/// </summary>
		/// <returns>True if action fails, otherwise false.</returns>
#if UNIT_TEST_COMPILANT
		[Obsolete("Unit tests, don't support methods calls.")]
#endif
		[UnmanagedCall]
		public static bool SaveAllScenes() 
		{
#if UNIT_TEST_COMPILANT
			throw new NotImplementedException("Unit tests, don't support methods calls. Only properties can be get or set.");
#else
			return Internal_SaveAllScenes();
#endif
		}

		/// <summary>
		/// Saves all scenes to the assets. Done in the background.
		/// </summary>
#if UNIT_TEST_COMPILANT
		[Obsolete("Unit tests, don't support methods calls.")]
#endif
		[UnmanagedCall]
		public static void SaveAllScenesAsync() 
		{
#if UNIT_TEST_COMPILANT
			throw new NotImplementedException("Unit tests, don't support methods calls. Only properties can be get or set.");
#else
			Internal_SaveAllScenesAsync();
#endif
		}

		/// <summary>
		/// Loads scene from the asset.
		/// </summary>
		/// <param name="sceneId">The scene ID to load.</param>
		/// <returns>True if action fails, otherwise false.</returns>
#if UNIT_TEST_COMPILANT
		[Obsolete("Unit tests, don't support methods calls.")]
#endif
		[UnmanagedCall]
		public static bool LoadScene(Guid sceneId) 
		{
#if UNIT_TEST_COMPILANT
			throw new NotImplementedException("Unit tests, don't support methods calls. Only properties can be get or set.");
#else
			return Internal_LoadScene(ref sceneId);
#endif
		}

		/// <summary>
		/// Loads scene from the bytes array.
		/// </summary>
		/// <param name="sceneData">The scene data to load.</param>
		/// <returns>Loaded scene object, otherwise null if cannot load data (then see log for more information).</returns>
#if UNIT_TEST_COMPILANT
		[Obsolete("Unit tests, don't support methods calls.")]
#endif
		[UnmanagedCall]
		public static Scene LoadSceneFromBytes(byte[] sceneData) 
		{
#if UNIT_TEST_COMPILANT
			throw new NotImplementedException("Unit tests, don't support methods calls. Only properties can be get or set.");
#else
			return Internal_LoadSceneFromBytes(sceneData);
#endif
		}

		/// <summary>
		/// Loads scene from the asset. Done in the background.
		/// </summary>
		/// <param name="sceneId">The scene ID to load.</param>
#if UNIT_TEST_COMPILANT
		[Obsolete("Unit tests, don't support methods calls.")]
#endif
		[UnmanagedCall]
		public static void LoadSceneAsync(Guid sceneId) 
		{
#if UNIT_TEST_COMPILANT
			throw new NotImplementedException("Unit tests, don't support methods calls. Only properties can be get or set.");
#else
			Internal_LoadSceneAsync(ref sceneId);
#endif
		}

		/// <summary>
		/// Unloads given scene.
		/// </summary>
		/// <param name="scene">The scene to unload.</param>
		/// <returns>True if action fails, otherwise false.</returns>
#if UNIT_TEST_COMPILANT
		[Obsolete("Unit tests, don't support methods calls.")]
#endif
		[UnmanagedCall]
		public static bool UnloadScene(Scene scene) 
		{
#if UNIT_TEST_COMPILANT
			throw new NotImplementedException("Unit tests, don't support methods calls. Only properties can be get or set.");
#else
			return Internal_UnloadScene(Object.GetUnmanagedPtr(scene));
#endif
		}

		/// <summary>
		/// Unloads given scene. Done in the background.
		/// </summary>
		/// <param name="scene">The scene to unload.</param>
#if UNIT_TEST_COMPILANT
		[Obsolete("Unit tests, don't support methods calls.")]
#endif
		[UnmanagedCall]
		public static void UnloadSceneAsync(Scene scene) 
		{
#if UNIT_TEST_COMPILANT
			throw new NotImplementedException("Unit tests, don't support methods calls. Only properties can be get or set.");
#else
			Internal_UnloadSceneAsync(Object.GetUnmanagedPtr(scene));
#endif
		}

		/// <summary>
		/// Unloads all scenes.
		/// </summary>
		/// <returns>True if action fails, otherwise false.</returns>
#if UNIT_TEST_COMPILANT
		[Obsolete("Unit tests, don't support methods calls.")]
#endif
		[UnmanagedCall]
		public static bool UnloadAllScenes() 
		{
#if UNIT_TEST_COMPILANT
			throw new NotImplementedException("Unit tests, don't support methods calls. Only properties can be get or set.");
#else
			return Internal_UnloadAllScenes();
#endif
		}

		/// <summary>
		/// Unloads all scenes. Done in the background.
		/// </summary>
#if UNIT_TEST_COMPILANT
		[Obsolete("Unit tests, don't support methods calls.")]
#endif
		[UnmanagedCall]
		public static void UnloadAllScenesAsync() 
		{
#if UNIT_TEST_COMPILANT
			throw new NotImplementedException("Unit tests, don't support methods calls. Only properties can be get or set.");
#else
			Internal_UnloadAllScenesAsync();
#endif
		}

		/// <summary>
		/// Reloads scripts. Done in the background.
		/// </summary>
#if UNIT_TEST_COMPILANT
		[Obsolete("Unit tests, don't support methods calls.")]
#endif
		[UnmanagedCall]
		public static void ReloadScriptsAsync() 
		{
#if UNIT_TEST_COMPILANT
			throw new NotImplementedException("Unit tests, don't support methods calls. Only properties can be get or set.");
#else
			Internal_ReloadScriptsAsync();
#endif
		}

#region Internal Calls
#if !UNIT_TEST_COMPILANT
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Scene[] Internal_GetScenes();
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Actor Internal_FindActorById(ref Guid id);
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Actor Internal_FindActorByName(string name);
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Scene Internal_FindScene(ref Guid id);
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int Internal_GetLoadedScenesCount();
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool Internal_IsAnySceneLoaded();
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool Internal_IsAnyActorInGame();
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool Internal_IsGameLogicRunning();
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Internal_SetGameLogicRunning(bool val);
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool Internal_IsAnyActionPending();
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool Internal_GetLastSceneLoadTime();
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Internal_SpawnActor(IntPtr actor, IntPtr parent);
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool Internal_SaveScene(IntPtr scene);
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern byte[] Internal_SaveSceneToBytes(IntPtr scene);
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Internal_SaveSceneAsync(IntPtr scene);
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool Internal_SaveAllScenes();
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Internal_SaveAllScenesAsync();
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool Internal_LoadScene(ref Guid sceneId);
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Scene Internal_LoadSceneFromBytes(byte[] sceneData);
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Internal_LoadSceneAsync(ref Guid sceneId);
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool Internal_UnloadScene(IntPtr scene);
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Internal_UnloadSceneAsync(IntPtr scene);
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool Internal_UnloadAllScenes();
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Internal_UnloadAllScenesAsync();
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Internal_ReloadScriptsAsync();
#endif
#endregion
	}
}

