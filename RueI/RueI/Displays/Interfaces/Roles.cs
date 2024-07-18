namespace RueI.Extensions;

using PlayerRoles;

/// <summary>
/// Provides a means for describing multiple <see cref="RoleTypeId"/>s.
/// </summary>
public readonly struct Roles
{
    private readonly RoleTypeId[] roles;

    /// <summary>
    /// Initializes a new instance of the <see cref="Roles"/> struct.
    /// </summary>
    /// <param name="enumerableRoles">The roles to use.</param>
    public Roles(IEnumerable<RoleTypeId> enumerableRoles)
    {
        roles = enumerableRoles.Distinct().ToArray();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Roles"/> struct.
    /// </summary>
    /// <param name="role">The role to use.</param>
    public Roles(RoleTypeId role)
    {
        roles = new[] { role };
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Roles"/> struct.
    /// </summary>
    /// <param name="roles">The roles to use.</param>
    public Roles(params RoleTypeId[] roles)
    {
        this.roles = roles.Distinct().ToArray();
    }

    /// <summary>
    /// Gets the Spectator role.
    /// </summary>
    public static Roles Spectator { get; } = new(RoleTypeId.Spectator);

    /// <summary>
    /// Gets the Overwatch role.
    /// </summary>
    public static Roles Overwatch { get; } = new(RoleTypeId.Overwatch);

    /// <summary>
    /// Gets the Filmmaker role.
    /// </summary>
    public static Roles Filmmaker { get; } = new(RoleTypeId.Filmmaker);

    /// <summary>
    /// Gets the custom role.
    /// </summary>
    public static Roles CustomRole { get; } = new(RoleTypeId.CustomRole);

    /// <summary>
    /// Gets the SCP-173 role.
    /// </summary>
    public static Roles Scp173 { get; } = new(RoleTypeId.Scp173);

    /// <summary>
    /// Gets the SCP-106 role.
    /// </summary>
    public static Roles Scp106 { get; } = new(RoleTypeId.Scp106);

    /// <summary>
    /// Gets the SCP-049 role.
    /// </summary>
    public static Roles Scp049 { get; } = new(RoleTypeId.Scp049);

    /// <summary>
    /// Gets the SCP-0492 role.
    /// </summary>
    public static Roles Scp0492 { get; } = new(RoleTypeId.Scp0492);

    /// <summary>
    /// Gets the SCP-079 role.
    /// </summary>
    public static Roles Scp079 { get; } = new(RoleTypeId.Scp079);

    /// <summary>
    /// Gets the SCP-939 role.
    /// </summary>
    public static Roles Scp939 { get; } = new(RoleTypeId.Scp939);

    /// <summary>
    /// Gets the SCP-096 role.
    /// </summary>
    public static Roles Scp096 { get; } = new(RoleTypeId.Scp096);

    /// <summary>
    /// Gets the SCP-3114 role.
    /// </summary>
    public static Roles Scp3114 { get; } = new(RoleTypeId.Scp3114);

    /// <summary>
    /// Gets all of the SCPs, excluding SCP-0492.
    /// </summary>
    public static Roles ScpsNo0492 { get; } = Scp173 | Scp106 | Scp049 | Scp079 | Scp939 | Scp096 | Scp3114;

    /// <summary>
    /// Gets all of the SCPs.
    /// </summary>
    public static Roles ScpRoles { get; } = ScpsNo0492 | Scp0492;

    /// <summary>
    /// Gets the Tutorial role.
    /// </summary>
    public static Roles Tutorial { get; } = new(RoleTypeId.Tutorial);

    /// <summary>
    /// Gets the Class-D role.
    /// </summary>
    public static Roles ClassD { get; } = new(RoleTypeId.ClassD);

    /// <summary>
    /// Gets the Chaos Rifleman role.
    /// </summary>
    public static Roles ChaosRifleman { get; } = new(RoleTypeId.ChaosRifleman);

    /// <summary>
    /// Gets the Chaos Conscript role.
    /// </summary>
    public static Roles ChaosConscript { get; } = new(RoleTypeId.ChaosConscript);

    /// <summary>
    /// Gets the Chaos Marauder role.
    /// </summary>
    public static Roles ChaosMarauder { get; } = new(RoleTypeId.ChaosMarauder);

    /// <summary>
    /// Gets the Chaos Repressor role.
    /// </summary>
    public static Roles ChaosRepressor { get; } = new(RoleTypeId.ChaosRepressor);

    /// <summary>
    /// Gets all of the Chaos Insurgency roles.
    /// </summary>
    public static Roles ChaosRoles { get; } = ChaosRifleman | ChaosConscript | ChaosMarauder | ChaosRepressor;

    /// <summary>
    /// Gets all of the Chaos Insurgency roles and Class-D.
    /// </summary>
    public static Roles ChaosSide { get; } = ChaosRoles | ClassD;

    /// <summary>
    /// Gets the Scientist role.
    /// </summary>
    public static Roles Scientist { get; } = new(RoleTypeId.Scientist);

    /// <summary>
    /// Gets the facility guard role.
    /// </summary>
    public static Roles FacilityGuard { get; } = new(RoleTypeId.FacilityGuard);

    /// <summary>
    /// Gets the NTF private role.
    /// </summary>
    public static Roles NtfPrivate { get; } = new(RoleTypeId.NtfPrivate);

    /// <summary>
    /// Gets the NTF private role.
    /// </summary>
    public static Roles NtfSergeant { get; } = new(RoleTypeId.NtfSergeant);

    /// <summary>
    /// Gets the NTF specialist role.
    /// </summary>
    public static Roles NtfSpecialist { get; } = new(RoleTypeId.NtfSpecialist);

    /// <summary>
    /// Gets the NTF captain role.
    /// </summary>
    public static Roles NtfCaptain { get; } = new(RoleTypeId.NtfCaptain);

    /// <summary>
    /// Gets all of the NTF roles.
    /// </summary>
    public static Roles NtfRoles { get; } = NtfPrivate | NtfSergeant | NtfSpecialist | NtfCaptain;

    /// <summary>
    /// Gets all of the NTF roles and Scientists.
    /// </summary>
    public static Roles NtfSide { get; } = NtfRoles | Scientist;

    /// <summary>
    /// Combines two <see cref="Roles"/>.
    /// </summary>
    /// <param name="left">The first <see cref="Roles"/>.</param>
    /// <param name="right">The second <see cref="Roles"/>.</param>
    /// <returns>A combined <see cref="Roles"/>.</returns>
    public static Roles operator |(Roles left, Roles right) => new(left.roles.Union(right.roles));

    /// <summary>
    /// Determines whether or not this contains a certain <see cref="RoleTypeId"/>.
    /// </summary>
    /// <param name="roleId">The <see cref="RoleTypeId"/> to check.</param>
    /// <returns>A value indicating whether or not this has a certain <see cref="RoleTypeId"/>.</returns>
    public bool Has(RoleTypeId roleId) => roles.Contains(roleId);
}