// -----------------------------------------------------------------------
// <copyright file="Camera.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features.Core;
    using Exiled.API.Interfaces;
    using MapGeneration;
    using PlayerRoles.PlayableScps.Scp079.Cameras;
    using UnityEngine;

    using CameraType = Enums.CameraType;

    /// <summary>
    /// The in-game Scp079Camera.
    /// </summary>
    public class Camera : GameEntity, IWrapper<Scp079Camera>
    {
        /// <summary>
        /// A <see cref="Dictionary{TKey,TValue}"/> containing all known <see cref="Scp079Camera"/>s and their corresponding <see cref="Camera"/>.
        /// </summary>
        internal static readonly Dictionary<Scp079Camera, Camera> Camera079ToCamera = new(250);

        private static readonly Dictionary<string, CameraType> NameToCameraType = new()
        {
            // Entrance Zone
            ["CHKPT (EZ HALL)"] = CameraType.EzChkptHall,
            ["EZ CROSSING"] = CameraType.EzCrossing,
            ["EZ CURVE"] = CameraType.EzCurve,
            ["EZ HALLWAY"] = CameraType.EzHallway,
            ["EZ THREE-WAY"] = CameraType.EzThreeWay,
            ["GATE A"] = CameraType.EzGateA,
            ["GATE B"] = CameraType.EzGateB,
            ["INTERCOM BOTTOM"] = CameraType.EzIntercomBottom,
            ["INTERCOM HALL"] = CameraType.EzIntercomHall,
            ["INTERCOM PANEL"] = CameraType.EzIntercomPanel,
            ["INTERCOM STAIRS"] = CameraType.EzIntercomStairs,
            ["LARGE OFFICE"] = CameraType.EzLargeOffice,
            ["LOADING DOCK"] = CameraType.EzLoadingDock,
            ["MINOR OFFICE"] = CameraType.EzMinorOffice,
            ["TWO-STORY OFFICE"] = CameraType.EzTwoStoryOffice,

            // Heavy Containment Zone
            ["049 OUTSIDE"] = CameraType.Hcz049Outside,
            ["049 CONT CHAMBER"] = CameraType.Hcz049ContChamber,
            ["049/173 TOP"] = CameraType.Hcz049ElevTop,
            ["049 HALLWAY"] = CameraType.Hcz049Hallway,
            ["173 OUTSIDE"] = CameraType.Hcz173Outside,
            ["049/173 BOTTOM"] = CameraType.Hcz049TopFloor,
            ["079 AIRLOCK"] = CameraType.Hcz079Airlock,
            ["079 CONT CHAMBER"] = CameraType.Hcz079ContChamber,
            ["079 HALLWAY"] = CameraType.Hcz079Hallway,
            ["079 KILL SWITCH"] = CameraType.Hcz079KillSwitch,
            ["096 CONT CHAMBER"] = CameraType.Hcz096ContChamber,
            ["106 BRIDGE"] = CameraType.Hcz106Bridge,
            ["106 CATWALK"] = CameraType.Hcz106Catwalk,
            ["106 RECONTAINMENT"] = CameraType.Hcz106Recontainment,
            ["CHKPT (EZ)"] = CameraType.HczChkptEz,
            ["CHKPT (HCZ)"] = CameraType.HczChkptHcz,
            ["H.I.D. CHAMBER"] = CameraType.HczHIDChamber,
            ["H.I.D. HALLWAY"] = CameraType.HczHIDHallway,
            ["HCZ 939"] = CameraType.Hcz939,
            ["HCZ ARMORY"] = CameraType.HczArmory,
            ["HCZ ARMORY INTERIOR"] = CameraType.HczArmoryInterior,
            ["HCZ CROSSING"] = CameraType.HczCrossing,
            ["HCZ CURVE"] = CameraType.HczCurve,
            ["HCZ ELEV SYS A"] = CameraType.HczElevSysA,
            ["HCZ ELEV SYS B"] = CameraType.HczElevSysB,
            ["HCZ HALLWAY"] = CameraType.HczHallway,
            ["HCZ THREE-WAY"] = CameraType.HczThreeWay,
            ["SERVERS BOTTOM"] = CameraType.HczServersBottom,
            ["SERVERS STAIRS"] = CameraType.HczServersStairs,
            ["SERVERS TOP"] = CameraType.HczServersTop,
            ["TESLA GATE"] = CameraType.HczTeslaGate,
            ["TESTROOM BRIDGE"] = CameraType.HczTestroomBridge,
            ["TESTROOM MAIN"] = CameraType.HczTestroomMain,
            ["TESTROOM OFFICE"] = CameraType.HczTestroomOffice,
            ["WARHEAD ARMORY"] = CameraType.HczWarheadArmory,
            ["WARHEAD CONTROL"] = CameraType.HczWarheadControl,
            ["WARHEAD HALLWAY"] = CameraType.HczWarheadHallway,
            ["WARHEAD TOP"] = CameraType.HczWarheadTop,

            // Light Containment Zone
            ["173 BOTTOM"] = CameraType.Lcz173Bottom,
            ["173 HALL"] = CameraType.Lcz173Hall,
            ["914 AIRLOCK"] = CameraType.Lcz914Airlock,
            ["914 CONT CHAMBER"] = CameraType.Lcz914ContChamber,
            ["AIRLOCK"] = CameraType.LczAirlock,
            ["ARMORY"] = CameraType.LczArmory,
            ["CELLBLOCK BACK"] = CameraType.LczCellblockBack,
            ["CELLBLOCK ENTRY"] = CameraType.LczCellblockEntry,
            ["CHKPT A ENTRY"] = CameraType.LczChkptAEntry,
            ["CHKPT A INNER"] = CameraType.LczChkptAInner,
            ["CHKPT B ENTRY"] = CameraType.LczChkptBEntry,
            ["CHKPT B INNER"] = CameraType.LczChkptBInner,
            ["GLASSROOM"] = CameraType.LczGlassroom,
            ["GLASSROOM ENTRY"] = CameraType.LczGlassroomEntry,
            ["GREENHOUSE"] = CameraType.LczGreenhouse,
            ["LCZ CROSSING"] = CameraType.LczCrossing,
            ["LCZ CURVE"] = CameraType.LczCurve,
            ["LCZ ELEV SYS A"] = CameraType.LczElevSysA,
            ["LCZ ELEV SYS B"] = CameraType.LczElevSysB,
            ["LCZ HALLWAY"] = CameraType.LczHallway,
            ["LCZ THREE-WAY"] = CameraType.LczThreeWay,
            ["PC OFFICE"] = CameraType.LczPcOffice,
            ["RESTROOMS"] = CameraType.LczRestrooms,
            ["TC HALLWAY"] = CameraType.LczTcHallway,
            ["TEST CHAMBER"] = CameraType.LczTestChamber,

            // Surface
            ["EXIT PASSAGE"] = CameraType.ExitPassage,
            ["GATE A SURFACE"] = CameraType.GateASurface,
            ["GATE B SURFACE"] = CameraType.GateBSurface,
            ["MAIN STREET"] = CameraType.MainStreet,
            ["SURFACE AIRLOCK"] = CameraType.SurfaceAirlock,
            ["SURFACE BRIDGE"] = CameraType.SurfaceBridge,
            ["TUNNEL ENTRANCE"] = CameraType.TunnelEntrance,
        };

        private Room room;

        /// <summary>
        /// Initializes a new instance of the <see cref="Camera"/> class.
        /// </summary>
        /// <param name="camera079">The base camera.</param>
        internal Camera(Scp079Camera camera079)
            : base(camera079.gameObject)
        {
            Base = camera079;
            Camera079ToCamera.Add(camera079, this);
            Type = GetCameraType();
#if Debug
            if (Type is CameraType.Unknown)
                Log.Error($"[CAMERATYPE UNKNOWN] {this}");
#endif
        }

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Camera"/> which contains all the <see cref="Camera"/> instances.
        /// </summary>
        public static IReadOnlyCollection<Camera> List => Camera079ToCamera.Values;

        /// <summary>
        /// Gets a randomly selected <see cref="Camera"/>.
        /// </summary>
        /// <returns>A randomly selected <see cref="Camera"/> object.</returns>
        public static Camera Random => List.Random();

        /// <summary>
        /// Gets the base <see cref="Scp079Camera"/>.
        /// </summary>
        public Scp079Camera Base { get; }

        /// <summary>
        /// Gets the camera's name.
        /// </summary>
        public string Name => Base.Label;

        /// <summary>
        /// Gets the camera's id.
        /// </summary>
        public ushort Id => Base.SyncId;

        /// <summary>
        /// Gets the generator's <see cref="Room"/>.
        /// </summary>
        public Room Room => room ??= Room.Get(Base.Room);

        /// <summary>
        /// Gets the camera's <see cref="ZoneType"/>.
        /// </summary>
        public ZoneType Zone => Room ? Room.Zone : ZoneType.Unspecified;

        /// <summary>
        /// Gets the camera's <see cref="CameraType"/>.
        /// </summary>
        public CameraType Type { get; }

        /// <summary>
        /// Gets the camera's position.
        /// </summary>
        public override Vector3 Position => Base.Position;

        /// <summary>
        /// Gets the camera's rotation.
        /// </summary>
        public override Quaternion Rotation => Base._cameraAnchor.rotation;

        /// <summary>
        /// Gets the value of the <see cref="Camera"/> zoom.
        /// </summary>
        public float Zoom => Base.ZoomAxis.CurrentZoom;

        /// <summary>
        /// Gets or sets a value indicating whether or not this camera is being used by SCP-079.
        /// </summary>
        public bool IsBeingUsed
        {
            get => Base.IsActive;
            set => Base.IsActive = value;
        }

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Camera"/> which contains all the <see cref="Camera"/> instances given a <see cref="IEnumerable{T}"/> of <see cref="Scp079Camera"/>.
        /// </summary>
        /// <param name="cameras">The <see cref="IEnumerable{T}"/> of <see cref="Scp079Camera"/>.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Camera"/>.</returns>
        public static IEnumerable<Camera> Get(IEnumerable<Scp079Camera> cameras) => cameras.Select(Get);

        /// <summary>
        /// Gets the <see cref="Camera"/> belonging to the <see cref="Scp079Camera"/>, if any.
        /// </summary>
        /// <param name="camera079">The base <see cref="Scp079Camera"/>.</param>
        /// <returns>A <see cref="Camera"/> or <see langword="null"/> if not found.</returns>
        public static Camera Get(Scp079Camera camera079) => camera079 != null ? Camera079ToCamera.TryGetValue(camera079, out Camera camera) ? camera : new(camera079) : null;

        /// <summary>
        /// Gets a <see cref="Camera"/> given the specified <paramref name="cameraId"/>.
        /// </summary>
        /// <param name="cameraId">The camera id to be searched for.</param>
        /// <returns>The <see cref="Camera"/> with the given id or <see langword="null"/> if not found.</returns>
        public static Camera Get(uint cameraId) => List.FirstOrDefault(camera => camera.Id == cameraId);

        /// <summary>
        /// Gets a <see cref="Camera"/> given the specified <paramref name="cameraName"/>.
        /// </summary>
        /// <param name="cameraName">The name of the camera.</param>
        /// <returns>The <see cref="Camera"/> or <see langword="null"/> if not found.</returns>
        public static Camera Get(string cameraName) => List.FirstOrDefault(camera => camera.Name == cameraName);

        /// <summary>
        /// Gets a <see cref="Camera"/> given the specified <paramref name="cameraType"/>.
        /// </summary>
        /// <param name="cameraType">The <see cref="CameraType"/> to search for.</param>
        /// <returns>The <see cref="Camera"/> with the given <see cref="CameraType"/> or <see langword="null"/> if not found.</returns>
        public static Camera Get(CameraType cameraType) => List.FirstOrDefault(camera => camera.Type == cameraType);

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Camera"/> filtered based on a predicate.
        /// </summary>
        /// <param name="predicate">The condition to satisfy.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Camera"/> which contains elements that satisfy the condition.</returns>
        public static IEnumerable<Camera> Get(Func<Camera, bool> predicate) => List.Where(predicate);

        /// <summary>
        /// Get a <see cref="IEnumerable{T}"/> of <see cref="Camera"/> which contains all the <see cref="Camera"/> instance given a <see cref="IEnumerable{T}"/> of <see cref="Scp079Camera"/>.
        /// </summary>
        /// <param name="cameras">the <see cref="IEnumerable{T}"/> of <see cref="Scp079Camera"/>.</param>
        /// <param name="result">return a <see cref="IEnumerable{T}"/> of <see cref="Camera"/>, it's can be valid, or <see langword="null"/>, depending if <see cref="Scp079Camera"/> it's null or not.</param>
        /// <returns>a bool result if return sequence contain valid element.</returns>
        public static bool TryGet(IEnumerable<Scp079Camera> cameras, out IEnumerable<Camera> result) => (result = Get(cameras)).Any();

        /// <summary>
        /// Gets the <see cref="Camera"/> belonging to the <see cref="Scp079Camera"/>, if any.
        /// </summary>
        /// <param name="camera">The base <see cref="Scp079Camera"/>.</param>
        /// <param name="result">The instance of <see cref="Camera"/> which <see cref="Scp079Camera"/> base.</param>
        /// <returns><see langword="true"/> if <see cref="Camera"/> is not <see langword="null"/>, or <see langword="false"/> if <see cref="Camera"/> is <see langword="null"/>.</returns>
        public static bool TryGet(Scp079Camera camera, out Camera result) => (result = Get(camera)) != null;

        /// <summary>
        /// Gets a <see cref="Camera"/> given the specified <paramref name="cameraId"/>.
        /// </summary>
        /// <param name="cameraId">The id camera to be shearch.</param>
        /// <param name="result">the result of <see cref="Camera"/>, if <paramref name="cameraId"/> is valid.</param>
        /// <returns><see langword="true"/> if <see cref="Camera"/> is not <see langword="null"/>, or <see langword="false"/> if <see cref="Camera"/> is <see langword="null"/>.</returns>
        public static bool TryGet(uint cameraId, out Camera result) => (result = Get(cameraId)) != null;

        /// <summary>
        /// Gets a <see cref="Camera"/> given the specified <paramref name="cameraName"/>.
        /// </summary>
        /// <param name="cameraName">The name of the camera.</param>
        /// <param name="result">The <see cref="Camera"/>, if <paramref name="cameraName"/> is valid.</param>
        /// <returns><see langword="true"/> if <see cref="Camera"/> is not <see langword="null"/>, or <see langword="false"/> if <see cref="Camera"/> is <see langword="null"/>.</returns>
        public static bool TryGet(string cameraName, out Camera result) => (result = Get(cameraName)) != null;

        /// <summary>
        /// Gets a <see cref="Camera"/> given the specified <paramref name="cameraType"/>.
        /// </summary>
        /// <param name="cameraType">The <see cref="CameraType"/> to search for.</param>
        /// <param name="result">The <see cref="Camera"/> with the given <see cref="CameraType"/>.</param>
        /// <returns><see langword="true"/> if <see cref="Camera"/> is not <see langword="null"/>, or <see langword="false"/> if <see cref="Camera"/> is <see langword="null"/>.</returns>
        public static bool TryGet(CameraType cameraType, out Camera result) => (result = Get(cameraType)) != null;

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Camera"/> filtered based on a predicate.
        /// </summary>
        /// <param name="predicate">The condition to satisfy.</param>
        /// <param name="result">A <see cref="IEnumerable{T}"/> of <see cref="Camera"/> which contains elements that satisfy the condition.</param>
        /// <returns><see langword="true"/> if <see cref="Camera"/> is not <see langword="null"/>, or <see langword="false"/> if <see cref="Camera"/> is <see langword="null"/>.</returns>
        public static bool TryGet(Func<Camera, bool> predicate, out IEnumerable<Camera> result) => (result = Get(predicate)).Any();

        /// <summary>
        /// Returns the Camera in a human-readable format.
        /// </summary>
        /// <returns>A string containing Camera-related data.</returns>
        public override string ToString() => $"({Type}) [{Room}] *{Name}* |{Id}| ={IsBeingUsed}=";

        private CameraType GetCameraType()
        {
            if (NameToCameraType.ContainsKey(Name))
                return NameToCameraType[Name];
            return Base.Room.Name switch
            {
                RoomName.Hcz049 => Name switch
                {
                    "173 STAIRS" => CameraType.Hcz173Stairs,
                    "173 CONT CHAMBER" => CameraType.Hcz173ContChamber,
                    _ => CameraType.Unknown,
                },
                RoomName.Lcz173 => Name switch
                {
                    "173 STAIRS" => CameraType.Lcz173Stairs,
                    "173 CONT CHAMBER" => CameraType.Lcz173ContChamber,
                    _ => CameraType.Unknown,
                },
                _ => CameraType.Unknown,
            };
        }
    }
}