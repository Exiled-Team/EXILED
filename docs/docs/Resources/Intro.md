---
sidebar_position: 1
---

### Index

- [RoleType, Team, Side, LeadingTeam](#roletype-team-side-and-leadingteam)
- [ItemType](#itemtype)
- [AmmoType](#ammotype)
- [DoorType](#doortype)
- [RoomType](#roomtype)
- [ElevatorType](#elevatortype)
- [DamageType](#damagetype)
- [Damage Handlers](#damagehandlers)
- [EffectType](#effecttype)
- [Keycard Permissions](#keycardpermissions)
- [DoorLockType](#doorlocktype)
- [StructureType](#structuretype)
- [BloodType](#bloodtype)
- [GeneratorState](#generatorstate)
- [IntercomStates](#intercomstates)
- [BroadcastType](#broadcasttype)
- [Attachment Names](#attachmentnames)
- [RoleChangeReason](#rolechangereason)

### External resources

- [Available Colors (en.scpslgame.com)](https://en.scpslgame.com/index.php/Docs:Permissions#Colors)

## Resources

### RoleType, Team, Side and LeadingTeam

<details><summary> <b>Roles</b></summary>

```md title="Latest Updated: 13.5.0.0"
| Id  | RoleTypeId     |
|-----|----------------|
| -1  | None           |
| 0   | Scp173         |
| 1   | ClassD         |
| 2   | Spectator      |
| 3   | Scp106         |
| 4   | NtfSpecialist  |
| 5   | Scp049         |
| 6   | Scientist      |
| 7   | Scp079         |
| 8   | ChaosConscript |
| 9   | Scp096         |
| 10  | Scp0492        |
| 11  | NtfSergeant    |
| 12  | NtfCaptain     |
| 13  | NtfPrivate     |
| 14  | Tutorial       |
| 15  | FacilityGuard  |
| 16  | Scp939         |
| 17  | CustomRole     |
| 18  | ChaosRifleman  |
| 19  | ChaosMarauder  |
| 20  | ChaosRepressor |
| 21  | Overwatch      |
| 22  | Filmmaker      |
| 23  | Scp3114        |
```

</details>

### ItemType

<details><summary> <b>Items</b></summary>

```md  title="Latest Updated: 13.5.0.0"
[-1] None
[0] KeycardJanitor
[1] KeycardScientist
[2] KeycardResearchCoordinator
[3] KeycardZoneManager
[4] KeycardGuard
[5] KeycardMTFPrivate
[6] KeycardContainmentEngineer
[7] KeycardMTFOperative
[8] KeycardMTFCaptain
[9] KeycardFacilityManager
[10] KeycardChaosInsurgency
[11] KeycardO5
[12] Radio
[13] GunCOM15
[14] Medkit
[15] Flashlight
[16] MicroHID
[17] SCP500
[18] SCP207
[19] Ammo12gauge
[20] GunE11SR
[21] GunCrossvec
[22] Ammo556x45
[23] GunFSP9
[24] GunLogicer
[25] GrenadeHE
[26] GrenadeFlash
[27] Ammo44cal
[28] Ammo762x39
[29] Ammo9x19
[30] GunCOM18
[31] SCP018
[32] SCP268
[33] Adrenaline
[34] Painkillers
[35] Coin
[36] ArmorLight
[37] ArmorCombat
[38] ArmorHeavy
[39] GunRevolver
[40] GunAK
[41] GunShotgun
[42] SCP330
[43] SCP2176
[44] SCP244a
[45] SCP244b
[46] SCP1853
[47] ParticleDisruptor
[48] GunCom45
[49] SCP1576
[50] Jailbird
[51] AntiSCP207
[52] GunFRMG0
[53] GunA7
[54] Lantern
```

</details>


### AmmoType

<details><summary> <b>Ammo</b></summary>

```md title="Latest Updated: 8.9.6.0"
[0] None
[1] Nato556
[2] Nato762
[3] Nato9
[4] Ammo12Gauge
[5] Ammo44Cal
```

</details>

### DoorType

<details><summary> <b>Doors</b></summary>

```md title="Latest Updated: 8.9.6.0"
[0] UnknownDoor
[1] Scp914Door
[2] GR18Inner
[3] Scp049Gate
[4] Scp049Armory
[5] Scp079First
[6] Scp079Second
[7] Scp096
[8] Scp079Armory
[9] Scp106Primary
[10] Scp106Secondary
[11] Scp173Gate
[12] Scp173Connector
[13] Scp173Armory
[14] Scp173Bottom
[15] GR18Gate
[16] Scp914Gate
[17] Scp939Cryo
[18] CheckpointLczA
[19] CheckpointLczB
[20] EntranceDoor
[21] EscapePrimary
[22] EscapeSecondary
[23] ServersBottom
[24] GateA
[25] GateB
[26] HczArmory
[27] HeavyContainmentDoor
[28] HID
[29] HIDLeft
[30] HIDRight
[31] Intercom
[32] LczArmory
[33] LczCafe
[34] LczWc
[35] LightContainmentDoor
[36] NukeArmory
[37] NukeSurface
[38] PrisonDoor
[39] SurfaceGate
[40] Scp330
[41] Scp330Chamber
[42] CheckpointGate
[43] SurfaceDoor
[44] CheckpointEzHczA
[45] CheckpointEzHczB
[46] UnknownGate
[47] UnknownElevator
[48] ElevatorGateA
[49] ElevatorGateB
[50] ElevatorNuke
[51] ElevatorScp049
[52] ElevatorLczA
[53] ElevatorLczB
[54] CheckpointArmoryA
[55] CheckpointArmoryB
[56] Airlock
[57] Scp173NewGate
```

</details>



### RoomType

<details><summary> <b>Rooms</b></summary>

```md title="Latest Updated: 8.9.6.0"
[0] Unknown
[1] LczArmory
[2] LczCurve
[3] LczStraight
[4] Lcz914
[5] LczCrossing
[6] LczTCross
[7] LczCafe
[8] LczPlants
[9] LczToilets
[10] LczAirlock
[11] Lcz173
[12] LczClassDSpawn
[13] LczCheckpointB
[14] LczGlassBox
[15] LczCheckpointA
[16] Hcz079
[17] HczEzCheckpointA
[18] HczEzCheckpointB
[19] HczArmory
[20] Hcz939
[21] HczHid
[22] Hcz049
[23] HczCrossing
[24] Hcz106
[25] HczNuke
[26] HczTesla
[27] HczServers
[28] HczTCross
[29] HczCurve
[30] Hcz096
[31] EzVent
[32] EzIntercom
[33] EzGateA
[34] EzDownstairsPcs
[35] EzCurve
[36] EzPcs
[37] EzCrossing
[38] EzCollapsedTunnel
[39] EzConference
[40] EzStraight
[41] EzCafeteria
[42] EzUpstairsPcs
[43] EzGateB
[44] EzShelter
[45] Pocket
[46] Surface
[47] HczStraight
[48] EzTCross
[49] Lcz330
[50] EzCheckpointHallway
[51] HczTestRoom
[52] HczElevatorA
[53] HczElevatorB
```

</details>

### ElevatorType

<details><summary> <b>Elevators</b></summary>

```md title="Latest Updated: 8.9.6.0"
[0] Unknown
[1] GateA
[2] GateB
[3] Nuke
[4] Scp049
[5] LczA
[6] LczB
```

</details>

### DamageType

<details><summary> <b>DamageType</b></summary>

```md title="Latest Updated: 8.9.6.0"
[0] Unknown
[1] Falldown
[2] Warhead
[3] Decontamination
[4] Asphyxiation
[5] Poison
[6] Bleeding
[7] Firearm
[8] MicroHid
[9] Tesla
[10] Scp
[11] Explosion
[12] Scp018
[13] Scp207
[14] Recontainment
[15] Crushed
[16] FemurBreaker
[17] PocketDimension
[18] FriendlyFireDetector
[19] SeveredHands
[20] Custom
[21] Scp049
[22] Scp096
[23] Scp173
[24] Scp939
[25] Scp0492
[26] Scp106
[27] Crossvec
[28] Logicer
[29] Revolver
[30] Shotgun
[31] AK
[32] Com15
[33] Com18
[34] Fsp9
[35] E11Sr
[36] Hypothermia
[37] ParticleDisruptor
[38] CardiacArrest
[39] Com45
[40] Jailbird
[41] Frmg0
[42] A7
[43] Scp3114
[44] Strangled
[45] Marshmallow
```

</details>

### DamageHandlers

<details><summary> <b>Damage Handlers</b></summary>

```md title="Latest Updated: 05/08/2022"
All available DamageHandlers

+ Symbol ':' literally means "inherits from"
* In C#, inheritance is a process in which one object acquires all the properties and behaviors of its parent object automatically.

PlayerStatsSystem::DamageHandlerBase
PlayerStatsSystem::StandardDamageHandler : DamageHandlerBase
PlayerStatsSystem::AttackerDamageHandler : StandardDamageHandler
PlayerStatsSystem::CustomReasonDamageHandler : StandardDamageHandler
PlayerStatsSystem::UniversalDamageHandler : StandardDamageHandler
PlayerStatsSystem::WarheadDamageHandler : StandardDamageHandler
PlayerStatsSystem::RecontainmentDamageHandler : AttackerDamageHandler
PlayerStatsSystem::FirearmDamageHandler : AttackerDamageHandler
PlayerStatsSystem::ScpDamageHandler : AttackerDamageHandler
PlayerStatsSystem::Scp096DamageHandler : AttackerDamageHandler
PlayerStatsSystem::MicroHidDamageHandler : AttackerDamageHandler
PlayerStatsSystem::ExplosionDamageHandler : AttackerDamageHandler
PlayerStatsSystem::Scp018DamageHandler : AttackerDamageHandler
```

</details>

### EffectType

<details><summary> <b>Effects</b></summary>

```md title="Latest Updated: 8.9.6.0"
[0] None
[1] AmnesiaItems
[2] AmnesiaVision
[3] Asphyxiated
[4] Bleeding
[5] Blinded
[6] Burned
[7] Concussed
[8] Corroding
[9] Deafened
[10] Decontaminating
[11] Disabled
[12] Ensnared
[13] Exhausted
[14] Flashed
[15] Hemorrhage
[16] Invigorated
[17] BodyshotReduction
[18] Poisoned
[19] Scp207
[20] Invisible
[21] SinkHole
[22] DamageReduction
[23] MovementBoost
[24] RainbowTaste
[25] SeveredHands
[26] Stained
[27] Vitality
[28] Hypothermia
[29] Scp1853
[30] CardiacArrest
[31] InsufficientLighting
[32] SoundtrackMute
[33] SpawnProtected
[34] Traumatized
[35] AntiScp207
[36] Scanned
[37] PocketCorroding
[38] SilentWalk
[39] Marshmallow
[40] Strangled
[41] Ghostly
[42] FogControl
[43] Slowness
```

</details>

### KeycardPermissions

<details><summary> <b>Keycard Perms</b></summary>

```md title="Latest Updated: 8.9.6.0"
[0] None
[1] Checkpoints
[2] ExitGates
[4] Intercom
[8] AlphaWarhead
[16] ContainmentLevelOne
[32] ContainmentLevelTwo
[64] ContainmentLevelThree
[128] ArmoryLevelOne
[256] ArmoryLevelTwo
[512] ArmoryLevelThree
[1024] ScpOverride
```

</details>

### DoorLockType

<details><summary> <b>Lock Type</b></summary>

```md title="Latest Updated: 8.9.6.0"
[0] None
[1] Regular079
[2] Lockdown079
[4] Warhead
[8] AdminCommand
[16] DecontLockdown
[32] DecontEvacuate
[64] SpecialDoorFeature
[128] NoPower
[256] Isolation
[512] Lockdown2176
```

</details>

### StructureType

<details><summary> <b>Structures</b></summary>

```md title="Latest Updated: 13.5.0.0"
[0] StandardLocker
[1] LargeGunLocker
[2] ScpPedestal
[3] Scp079Generator
[4] SmallWallCabinet
[5] Workstation
```

</details>

### BloodType

<details><summary> <b>Blood</b></summary>

```md title="Latest Updated: 8.9.6.0"
[0] Default
[1] Scp106
[2] Spreaded
[3] Faded
```

</details>

### GeneratorState

<details><summary> <b>GeneratorState</b></summary>

```md title="Latest Updated: 8.9.6.0"
[1] None
[2] Unlocked
[4] Open
[8] Activating
[16] Engaged
```

</details>

### IntercomStates

<details><summary> <b>Intercom States</b></summary>

```md title="Latest Updated: 13.5.0.0"
[0] Ready
[1] Starting
[2] InUse
[3] Cooldown
[4] NotFound
```

</details>

### BroadcastFlags

<details><summary> <b>BroadcastFlags</b></summary>

```md title="Latest Updated: 13.5.0.0"
[0] Normal
[1] Truncated
[2] AdminChat
```

</details>



### AttachmentNames

<details><summary> <b>Attachment Names</b></summary>

```md title="Latest Updated: 8.9.6.0"
[0] None
[1] IronSights
[2] DotSight
[3] HoloSight
[4] NightVisionSight
[5] AmmoSight
[6] ScopeSight
[7] StandardStock
[8] ExtendedStock
[9] RetractedStock
[10] LightweightStock
[11] HeavyStock
[12] RecoilReducingStock
[13] Foregrip
[14] Laser
[15] Flashlight
[16] AmmoCounter
[17] StandardBarrel
[18] ExtendedBarrel
[19] SoundSuppressor
[20] FlashHider
[21] MuzzleBrake
[22] MuzzleBooster
[23] StandardMagFMJ
[24] StandardMagAP
[25] StandardMagJHP
[26] ExtendedMagFMJ
[27] ExtendedMagAP
[28] ExtendedMagJHP
[29] DrumMagFMJ
[30] DrumMagAP
[31] DrumMagJHP
[32] LowcapMagFMJ
[33] LowcapMagAP
[34] LowcapMagJHP
[35] CylinderMag4
[36] CylinderMag6
[37] CylinderMag8
[38] CarbineBody
[39] RifleBody
[40] ShortBarrel
[41] ShotgunChoke
[42] ShotgunExtendedBarrel
[43] NoRifleStock
[44] ShotgunSingleShot
[45] ShotgunDoubleShot
```

</details>

### RoleChangeReason

<details><summary> <b>RoleChangeReason</b></summary>

```md title="Latest Updated: 13.5.0.0"
[0] None
[1] RoundStart
[2] LateJoin
[3] Respawn
[4] Died
[5] Escaped
[6] Revived
[7] RemoteAdmin
[8] Destroyed
```

</details>


