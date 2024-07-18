namespace RueI.Displays;

using PlayerRoles;

/// <summary>
/// Provides a means for describing multiple <see cref="RoleTypeId"/>s.
/// </summary>
/// <remarks>
/// The purpose of the <see cref="Roles"/> enum is to enable roles to be treated like a <see cref="FlagsAttribute"/> enum. Normally,
/// <see cref="RoleTypeId"/> cannot be treated like bit flags, so this acts as a fast and convenient way to do so.
/// </remarks>
/// <include file='docs.xml' path='docs/displays/members[@name="roles"]/Roles/*'/>
[Flags]
public enum Roles
{
    /// <summary>
    /// Gets the SCP-173 role id.
    /// </summary>
    Scp173 = 1 << RoleTypeId.Scp173,

    /// <summary>
    /// Gets the Class-D role id.
    /// </summary>
    ClassD = 1 << RoleTypeId.ClassD,

    /// <summary>
    /// Gets the Specatator role id.
    /// </summary>
    Spectator = 1 << RoleTypeId.Spectator,

    /// <summary>
    /// Gets the SCP-106 role id.
    /// </summary>
    Scp106 = 1 << RoleTypeId.Scp106,

    /// <summary>
    /// Gets the NTF Specialist role id.
    /// </summary>
    NtfSpecialist = 1 << RoleTypeId.NtfSpecialist,

    /// <summary>
    /// Gets the SCP-049 role id.
    /// </summary>
    Scp049 = 1 << RoleTypeId.Scp049,

    /// <summary>
    /// Gets the Scientist role id.
    /// </summary>
    Scientist = 1 << RoleTypeId.Scientist,

    /// <summary>
    /// Gets the SCP-079 role id.
    /// </summary>
    Scp079 = 1 << RoleTypeId.Scp079,

    /// <summary>
    /// Gets the Chaos Conscript role id.
    /// </summary>
    ChaosConscript = 1 << RoleTypeId.ChaosConscript,

    /// <summary>
    /// Gets the SCP-096 role id.
    /// </summary>
    Scp096 = 1 << RoleTypeId.Scp096,

    /// <summary>
    /// Gets the SCP-049-2 role id.
    /// </summary>
    Scp0492 = 1 << RoleTypeId.Scp0492,

    /// <summary>
    /// Gets the NTF Sergeant role id.
    /// </summary>
    NtfSergeant = 1 << RoleTypeId.NtfSergeant,

    /// <summary>
    /// Gets the NTF Captain role id.
    /// </summary>
    NtfCaptain = 1 << RoleTypeId.NtfCaptain,

    /// <summary>
    /// Gets the NTF Private role id.
    /// </summary>
    NtfPrivate = 1 << RoleTypeId.NtfPrivate,

    /// <summary>
    /// Gets the Tutorial role id.
    /// </summary>
    Tutorial = 1 << RoleTypeId.Tutorial,

    /// <summary>
    /// Gets the Facility Guard role id.
    /// </summary>
    FacilityGuard = 1 << RoleTypeId.FacilityGuard,

    /// <summary>
    /// Gets the SCP-939 role id.
    /// </summary>
    Scp939 = 1 << RoleTypeId.Scp939,

    /// <summary>
    /// Gets the custom role id.
    /// </summary>
    CustomRole = 1 << RoleTypeId.CustomRole,

    /// <summary>
    /// Gets the Chaos Rifleman role id.
    /// </summary>
    ChaosRifleman = 1 << RoleTypeId.ChaosRifleman,

    /// <summary>
    /// Gets the Chaos Marauder role id.
    /// </summary>
    ChaosMarauder = 1 << RoleTypeId.ChaosMarauder,

    /// <summary>
    /// Gets the Chaos Repressor role id.
    /// </summary>
    ChaosRepressor = 1 << RoleTypeId.ChaosRepressor,

    /// <summary>
    /// Gets the Overwatch role id.
    /// </summary>
    Overwatch = 1 << RoleTypeId.Overwatch,

    /// <summary>
    /// Gets the Filmmaker role id.
    /// </summary>
    Filmmaker = 1 << RoleTypeId.Filmmaker,

    /// <summary>
    /// Gets the SCP-3114 role id.
    /// </summary>
    Scp3114 = 1 << RoleTypeId.Scp3114,

    /// <summary>
    /// Gets all of the NTF role ids, including Facility Guards.
    /// </summary>
    NtfRoles = NtfPrivate | NtfSergeant | NtfSpecialist | NtfCaptain | FacilityGuard,

    /// <summary>
    /// Gets all of the Chaos role ids.
    /// </summary>
    ChaosRoles = ChaosRifleman | ChaosConscript | ChaosMarauder | ChaosRepressor,

    /// <summary>
    /// Gets all of the military role ids.
    /// </summary>
    MilitaryRoles = NtfRoles | ChaosRoles | Tutorial,

    /// <summary>
    /// Gets all of the civilian role ids.
    /// </summary>
    CivilianRoles = ClassD | Scientist,

    /// <summary>
    /// Gets all of the human role ids.
    /// </summary>
    HumanRoles = MilitaryRoles | CivilianRoles,

    /// <summary>
    /// Gets all of the SCP role ids, excluding SCP-049-2.
    /// </summary>
    ScpsNo0492 = Scp173 | Scp106 | Scp049 | Scp079 | Scp096 | Scp939 | Scp3114,

    /// <summary>
    /// Gets all of the SCP role ids, including SCP-049-2.
    /// </summary>
    Scps = ScpsNo0492 | Scp0492,

    /// <summary>
    /// Gets all of the role ids for roles considered to be alive.
    /// </summary>
    Alive = Scps | HumanRoles,

    /// <summary>
    /// Gets all of the role ids for roles considered to be dead.
    /// </summary>
    Dead = Spectator | Overwatch | Filmmaker,

    /// <summary>
    /// Gets all role ids.
    /// </summary>
    All = Alive | Dead,
}