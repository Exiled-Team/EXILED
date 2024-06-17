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
- [Spawn Reasons](#spawnreasons)
- [Prefabs](#prefabs)

### External resources

- [Available Colors (en.scpslgame.com)](https://en.scpslgame.com/index.php/Docs:Permissions#Colors)

## Resources

### RoleType, Team, Side and LeadingTeam

<details><summary> <b>Roles</b></summary>

```md title="Latest Updated: 13.5.0.0"
| Id  | RoleTypeId     | Team             | Side             | LeadingTeam     |
|-----|----------------|------------------|------------------|-----------------|
| -1  | None           | Dead             | None             | Draw            |
| 0   | Scp173         | SCPs             | Scp              | Anomalies       |
| 1   | ClassD         | ClassD           | ChaosInsurgency  | ChaosInsurgency |
| 2   | Spectator      | Dead             | None             | Draw            |
| 3   | Scp106         | SCPs             | Scp              | Anomalies       |
| 4   | NtfSpecialist  | FoundationForces | Mtf              | FacilityForces  |
| 5   | Scp049         | SCPs             | Scp              | Anomalies       |
| 6   | Scientist      | Scientists       | Mtf              | FacilityForces  |
| 7   | Scp079         | SCPs             | Scp              | Anomalies       |
| 8   | ChaosConscript | ChaosInsurgency  | ChaosInsurgency  | ChaosInsurgency |
| 9   | Scp096         | SCPs             | Scp              | Anomalies       |
| 10  | Scp0492        | SCPs             | Scp              | Anomalies       |
| 11  | NtfSergeant    | FoundationForces | Mtf              | FacilityForces  |
| 12  | NtfCaptain     | FoundationForces | Mtf              | FacilityForces  |
| 13  | NtfPrivate     | FoundationForces | Mtf              | FacilityForces  |
| 14  | Tutorial       | OtherAlive       | Tutorial         | Draw            |
| 15  | FacilityGuard  | FoundationForces | Mtf              | FacilityForces  |
| 16  | Scp939         | SCPs             | Scp              | Anomalies       |
| 17  | CustomRole     | Dead             | None             | Draw            |
| 18  | ChaosRifleman  | ChaosInsurgency  | ChaosInsurgency  | ChaosInsurgency |
| 19  | ChaosMarauder  | ChaosInsurgency  | ChaosInsurgency  | ChaosInsurgency |
| 20  | ChaosRepressor | ChaosInsurgency  | ChaosInsurgency  | ChaosInsurgency |
| 21  | Overwatch      | Dead             | None             | Draw            |
| 22  | Filmmaker      | Dead             | None             | Draw            |
| 23  | Scp3114        | SCPs             | Scp              | Anomalies       |
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
[-1] None
[0] AmnesiaItems
[1] AmnesiaVision
[2] Asphyxiated
[3] Bleeding
[4] Blinded
[5] Burned
[6] Concussed
[7] Corroding
[8] Deafened
[9] Decontaminating
[10] Disabled
[11] Ensnared
[12] Exhausted
[13] Flashed
[14] Hemorrhage
[15] Invigorated
[16] BodyshotReduction
[17] Poisoned
[18] Scp207
[19] Invisible
[20] SinkHole
[21] DamageReduction
[22] MovementBoost
[23] RainbowTaste
[24] SeveredHands
[25] Stained
[26] Vitality
[27] Hypothermia
[28] Scp1853
[29] CardiacArrest
[30] InsufficientLighting
[31] SoundtrackMute
[32] SpawnProtected
[33] Traumatized
[34] AntiScp207
[35] Scanned
[36] PocketCorroding
[37] SilentWalk
[38] Marshmallow
[39] Strangled
[40] Ghostly
[41] FogControl
[42] Slowness
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

### SpawnReasons

<details><summary> <b>Spawn Reasons</b></summary>

```md title="Latest Updated: 8.9.6.0"
[0] None
[1] RoundStart
[2] LateJoin
[3] Respawn
[4] Died
[5] Escaped
[6] Revived
[7] ForceClass
[8] Destroyed
```

</details>

### Prefabs

<details><summary> <b>Available Prefabs</b></summary>

```md title="Latest Updated: 02/13/2022"
Guid                                 | Name

43658aa2-f339-6044-eb2b-937db0c2c4bd | Player
5bfd1bbe-10a4-e184-4a2e-381314b3380c | PlaybackLobby
9a77040d-663e-8a14-a8a2-297249bce483 | Pickup
307eb9b0-d080-9dc4-78e6-673847876412 | Work Station
0b58d568-fcd7-5384-abce-593a7931d65d | SCP-173_Ragdoll
f602bb4b-88de-d554-5976-5c2e18af4479 | Ragdoll_1
ea314e24-bddd-5264-5b08-dadd1bcfa75e | SCP-106_Ragdoll
2b0290fb-6764-8f44-48ab-9294fe063c8f | Ragdoll_4
05488a04-eda9-a724-18c9-bf2edbe23031 | Ragdoll_6
e12d94d4-66ef-c734-2af0-aef522db57cb | Ragdoll_7
9d7cf7ef-eec0-ece4-196c-4fd2c3cfd03a | Ragdoll_8
e53f7b09-ad63-f924-6a96-0be4381af7f0 | SCP-096_Ragdoll
be41bb5a-3b5f-bc84-4ad4-d4e24dfa168f | Ragdoll_10
c87cf6f7-fc36-f144-6ae5-727c8c8f4b9b | Ragdoll_14
b8d25875-6346-0314-68a9-7d1b7ec71167 | SCP-939-53_Ragdoll
d2e872e1-1133-0984-186d-d3cdc686883f | SCP-939-89_Ragdoll
c69da0e5-a829-6a04-c8d9-f404a1073cfe | Grenade Flash
8063e113-c1f1-1514-7bc5-840ea8ee5f01 | Grenade Frag
38f8296e-fcf4-44f4-491b-b5dc69b8125b | Grenade SCP-018
33f5e0b4-fb1c-0134-493f-5d7aec09dc38 | EZ BreakableDoor
5fbbe939-51c2-ef74-a9ed-bc0abfefa132 | HCZ BreakableDoor
b82d6236-b9f5-33d4-e8ee-8ee33fba6edd | LCZ BreakableDoor
3353122b-0ba2-5d14-fa64-886c45425967 | sportTargetPrefab
422b08ed-0bc0-6cb4-7a7f-81dd37c430c0 | dboyTargetPrefab
4f03f7fa-f417-ae84-382b-962c31614d1a | binaryTargetPrefab
a0e7ee93-b802-e5a4-38bd-95e27cc133ea | TantrumObj
43c40e13-5a2a-b3a4-9ba8-29c7002cedaf | Tutorial_Ragdoll
bf9a7ae6-aaea-0174-d807-e0d4adb1c524 | PrimitiveObjectToy
6996edbf-2adf-a5b4-e8ce-e089cf9710ae | LightSourceToy
19b3629a-3298-8324-0ad0-e841def23244 | RegularKeycardPickup
ef69975c-5a03-b9c4-fa26-0b6145b05824 | ChaosKeycardPickup
8359dd57-d964-98c4-5871-586da0d50878 | RadioPickup
52f9fa65-832f-b0f4-ab15-0ac33a45b853 | Com15Pickup
06361fcf-1355-ea54-7a0b-d7a29244eae9 | MedkitPickup
9902569b-0bc8-cf74-b814-a69789ed8c5a | FlashlightPickup
35f6c267-d9b6-f5a4-4a87-5523b7424052 | MicroHidPickup
30d95cc3-8b1f-bd14-4b66-f7350cf3bae9 | SCP500Pickup
46572711-4d8b-f8a4-2a81-b1ca2ff15b5d | SCP207Pickup
e7588f50-a788-bd44-89bf-f9dae4ab2071 | Ammo12gaPickup
9958e2c0-668f-9f14-c9ed-1cd97281f3d3 | E11SRPickup
7a39d145-d2d1-5724-7ad5-660cbe2f5757 | CrossvecPickup
0282bdfe-9880-d284-1807-2d4e11fc540d | Ammo556mmPickup
d32145e1-e7d9-d674-fbaa-078247910c49 | Fsp9Pickup
4ce1ab59-83ff-aa14-db7a-65e79c48cf8e | LogicerPickup
3f98e495-a544-11b4-dbc3-a03797786f52 | HegPickup
6e4bfac7-e1c9-9af4-9a76-c025cc8bbb37 | FlashbangPickup
8627c2a9-e397-2164-08dd-97f9fddab207 | Ammo44calPickup
ecba736b-7b69-0f14-ea94-7c9067dc7ea8 | Ammo762mmPickup
89a36c3a-be6b-5914-7b75-1287c79f19dc | Ammo9mmPickup
2a12ef7e-b39d-ed34-6979-571e541231b1 | Com18Pickup
a1d0c7dd-6523-8a34-3b4a-5124f47b93dd | Scp018Projectile
6fbfc036-04fb-1f94-7af0-1335064c0198 | SCP268Pickup
9695f1b9-46d6-7054-c9af-a35a4fefafe1 | AdrenalinePrefab
9925eed6-900f-7444-880f-393468fa1a63 | PainkillersPickup
522f199f-ce6f-5814-9a67-f0191d0110a9 | CoinPickup
51703b4d-a309-11c4-8af7-bdb8d95214c0 | Light Armor Pickup
02e10b6d-9d4d-ed14-2b8b-f5219522da77 | Combat Armor Pickup
19d03dd5-b491-acc4-ea16-be8ad5a33783 | Heavy Armor Pickup
635a3623-281c-e5c4-297d-7f07cd6a0eef | RevolverPickup
1821b416-953c-98f4-c9b8-09d2c192b8b1 | AkPickup
d6abff39-0c5c-1804-58de-ac4478538837 | ShotgunPickup
65141804-5071-27e4-c8c0-23c547ce629c | Scp330Pickup
830e7527-1f40-d0d4-3a3e-ff49f5a6176c | Scp2176Projectile
2401ec76-dce3-cf34-b858-7a9c7dc83b0b | SCP244APickup Variant
39825db8-2df8-eed4-caa5-a4c334c669a0 | SCP244BPickup Variant
68f13209-e652-6024-2b89-0f75fb88a998 | Scp268PedestalStructure Variant
17054030-9461-d104-5b92-9456c9eb0ab7 | Scp207PedestalStructure Variant
f4149b66-c503-87a4-0b93-aabfe7c352da | Scp500PedestalStructure Variant
a149d3eb-11bd-de24-f9dd-57187f5771ef | Scp018PedestalStructure Variant
5ad5dc6d-7bc5-3154-8b1a-3598b96e0d5b | LargeGunLockerStructure
850f84ad-e273-1824-8885-11ae5e01e2f4 | RifleRackStructure
d54bead1-286f-3004-facd-74482a872ad8 | MiscLocker
daf3ccde-4392-c0e4-882d-b7002185c6b8 | GeneratorStructure
ad8a455f-062d-dea4-5b47-ac9217d4c58b | Spawnable Work Station Structure
5b227bd2-1ed2-8fc4-2aa1-4856d7cb7472 | RegularMedkitStructure
db602577-8d4f-97b4-890b-8c893bfcd553 | AdrenalineMedkitStructure
fff1c10c-a719-bea4-d95c-3e262ed03ab2 | Scp2176PedestalStructure Variant
53cd67d2-995b-3374-4892-4190ffd48ee9 | HegProjectile
2a6e5abb-7999-b8d4-a926-310e3e9e2a13 | FlashbangProjectile
```

</details>

