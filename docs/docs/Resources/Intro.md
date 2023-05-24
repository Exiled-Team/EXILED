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
- [HotKeyButton](#hotkeybutton)
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

```md title="Latest Updated: 08/23/2021"
| Id  | RoleTypeId     | Team | Side            | LeadingTeam     |
|-----|----------------|------|-----------------|-----------------|
-1 | None | Dead | None | Draw|
0 | Scp173 | SCPs | Scp | Anomalies|
1 | ClassD | ClassD | ChaosInsurgency | ChaosInsurgency|
2 | Spectator | Dead | None | Draw|
3 | Scp106 | SCPs | Scp | Anomalies|
4 | NtfSpecialist | FoundationForces | Mtf | FacilityForces|
5 | Scp049 | SCPs | Scp | Anomalies|
6 | Scientist | Scientists | Mtf | FacilityForces|
7 | Scp079 | SCPs | Scp | Anomalies|
8 | ChaosConscript | ChaosInsurgency | ChaosInsurgency | ChaosInsurgency|
9 | Scp096 | SCPs | Scp | Anomalies|
10 | Scp0492 | SCPs | Scp | Anomalies|
11 | NtfSergeant | FoundationForces | Mtf | FacilityForces|
12 | NtfCaptain | FoundationForces | Mtf | FacilityForces|
13 | NtfPrivate | FoundationForces | Mtf | FacilityForces|
14 | Tutorial | OtherAlive | Tutorial | Draw|
15 | FacilityGuard | FoundationForces | Mtf | FacilityForces|
16 | Scp939 | SCPs | Scp | Anomalies|
17 | CustomRole | Dead | None | Draw|
18 | ChaosRifleman | ChaosInsurgency | ChaosInsurgency | ChaosInsurgency|
19 | ChaosRepressor | ChaosInsurgency | ChaosInsurgency | ChaosInsurgency|
20 | ChaosMarauder | ChaosInsurgency | ChaosInsurgency | ChaosInsurgency|
21 | Overwatch | Dead | None | Draw|
22 | Filmmaker | Dead | None | Draw|
```

</details>

### ItemType

<details><summary> <b>Items</b></summary>

```md  title="Latest Updated: 24/05/2023"
<Item>                        (<id>)
None -1
KeycardJanitor 0
KeycardScientist 1
KeycardResearchCoordinator 2
KeycardZoneManager 3
KeycardGuard 4
KeycardNTFOfficer 5
KeycardContainmentEngineer 6
KeycardNTFLieutenant 7
KeycardNTFCommander 8
KeycardFacilityManager 9
KeycardChaosInsurgency 10
KeycardO5 11
Radio 12
GunCOM15 13
Medkit 14
Flashlight 15
MicroHID 16
SCP500 17
SCP207 18
Ammo12gauge 19
GunE11SR 20
GunCrossvec 21
Ammo556x45 22
GunFSP9 23
GunLogicer 24
GrenadeHE 25
GrenadeFlash 26
Ammo44cal 27
Ammo762x39 28
Ammo9x19 29
GunCOM18 30
SCP018 31
SCP268 32
Adrenaline 33
Painkillers 34
Coin 35
ArmorLight 36
ArmorCombat 37
ArmorHeavy 38
GunRevolver 39
GunAK 40
GunShotgun 41
SCP330 42
SCP2176 43
SCP244a 44
SCP244b 45
SCP1853 46
ParticleDisruptor 47
GunCom45 48
SCP1576 49
Jailbird 50
AntiSCP207 51
```

</details>


### AmmoType

<details><summary> <b>Ammo</b></summary>

```md title="Latest Updated: 24/05/2023"
None 0
Nato556 1
Nato762 2
Nato9 3
Ammo12Gauge 4
Ammo44Cal 5
```

</details>

### DoorType

<details><summary> <b>Doors</b></summary>

```md title="Latest Updated: 24/05/2023"
UnknownDoor 0
Scp914Door 1
GR18Inner 2
Scp049Gate 3
Scp049Armory 4
Scp079First 5
Scp079Second 6
Scp096 7
Scp106Bottom 8
Scp106Primary 9
Scp106Secondary 10
Scp173Gate 11
Scp173Connector 12
Scp173Armory 13
Scp173Bottom 14
GR18Gate 15
Scp914Gate 16
Scp939Cryo 17
CheckpointLczA 18
CheckpointLczB 19
EntranceDoor 20
EscapePrimary 21
EscapeSecondary 22
ServersBottom 23
GateA 24
GateB 25
HczArmory 26
HeavyContainmentDoor 27
HID 28
HIDLeft 29
HIDRight 30
Intercom 31
LczArmory 32
LczCafe 33
LczWc 34
LightContainmentDoor 35
NukeArmory 36
NukeSurface 37
PrisonDoor 38
SurfaceGate 39
Scp330 40
Scp330Chamber 41
CheckpointGate 42
SurfaceDoor 43
CheckpointEzHczA 44
CheckpointEzHczB 45
UnknownGate 46
UnknownElevator 47
ElevatorGateA 48
ElevatorGateB 49
ElevatorNuke 50
ElevatorScp049 51
ElevatorLczA 52
ElevatorLczB 53
CheckpointArmoryA 54
CheckpointArmoryB 55
Airlock 56
```

</details>



### RoomType

<details><summary> <b>Rooms</b></summary>

```md title="Latest Updated: 24/05/2023"
Unknown 0
LczArmory 1
LczCurve 2
LczStraight 3
Lcz012 4
Lcz914 5
LczCrossing 6
LczTCross 7
LczCafe 8
LczPlants 9
LczToilets 10
LczAirlock 11
Lcz173 12
LczClassDSpawn 13
LczCheckpointB 14
LczGlassBox 15
LczCheckpointA 16
Hcz079 17
HczEzCheckpointA 18
HczEzCheckpointB 19
HczArmory 20
Hcz939 21
HczHid 22
Hcz049 23
HczCrossing 24
Hcz106 25
HczNuke 26
HczTesla 27
HczServers 28
HczTCross 29
HczCurve 30
Hcz096 31
EzVent 32
EzIntercom 33
EzGateA 34
EzDownstairsPcs 35
EzCurve 36
EzPcs 37
EzCrossing 38
EzCollapsedTunnel 39
EzConference 40
EzStraight 41
EzCafeteria 42
EzUpstairsPcs 43
EzGateB 44
EzShelter 45
Pocket 46
Surface 47
HczStraight 48
EzTCross 49
Lcz330 50
EzCheckpointHallway 51
HczTestRoom 52
HczElevatorA 53
HczElevatorB 54
```

</details>

### ElevatorType

<details><summary> <b>Elevators</b></summary>

```md title="Latest Updated: 24/05/2023"
Unknown 0
GateA 1
GateB 2
Nuke 3
Scp049 4
LczA 5
LczB 6
```

</details>

### DamageType

<details><summary> <b>DamageType</b></summary>

```md title="Latest Updated: 24/05/2023"
Unknown 0
Falldown 1
Warhead 2
Decontamination 3
Asphyxiation 4
Poison 5
Bleeding 6
Firearm 7
MicroHid 8
Tesla 9
Scp 10
Explosion 11
Scp018 12
Scp207 13
Recontainment 14
Crushed 15
FemurBreaker 16
PocketDimension 17
FriendlyFireDetector 18
SeveredHands 19
Custom 20
Scp049 21
Scp096 22
Scp173 23
Scp939 24
Scp0492 25
Scp106 26
Crossvec 27
Logicer 28
Revolver 29
Shotgun 30
AK 31
Com15 32
Com18 33
Fsp9 34
E11Sr 35
Hypothermia 36
ParticleDisruptor 37
CardiacArrest 38
Com45 39
Jailbird 40
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

```md title="Latest Updated: 24/05/2023"
AmnesiaItems 0
AmnesiaVision 1
Asphyxiated 2
Bleeding 3
Blinded 4
Burned 5
Concussed 6
Corroding 7
Deafened 8
Decontaminating 9
Disabled 10
Ensnared 11
Exhausted 12
Flashed 13
Hemorrhage 14
Invigorated 15
BodyshotReduction 16
Poisoned 17
Scp207 18
Invisible 19
SinkHole 20
DamageReduction 21
MovementBoost 22
RainbowTaste 23
SeveredHands 24
Stained 25
Vitality 26
Hypothermia 27
Scp1853 28
CardiacArrest 29
InsufficientLighting 30
SoundtrackMute 31
SpawnProtected 32
Traumatized 33
AntiScp207 34
Scanned 35
```

</details>

### KeycardPermissions

<details><summary> <b>Keycard Perms</b></summary>

```md title="Latest Updated: 24/05/2023"
None 0
Checkpoints 1
ExitGates 2
Intercom 4
AlphaWarhead 8
ContainmentLevelOne 16
ContainmentLevelTwo 32
ContainmentLevelThree 64
ArmoryLevelOne 128
ArmoryLevelTwo 256
ArmoryLevelThree 512
ScpOverride 1024
```

</details>

### DoorLockType

<details><summary> <b>Lock Type</b></summary>

```md title="Latest Updated: 24/05/2023"
None 0
Regular079 1
Lockdown079 2
Warhead 4
AdminCommand 8
DecontLockdown 16
DecontEvacuate 32
SpecialDoorFeature 64
NoPower 128
Isolation 256
Lockdown2176 512
```

</details>

### StructureType

<details><summary> <b>Structures</b></summary>

```md title="Latest Updated: 24/05/2023"
StandardLocker 0
LargeGunLocker 1
ScpPedestal 2
Scp079Generator 3
SmallWallCabinet 4
Workstation 5
```

</details>

### BloodType

<details><summary> <b>Blood</b></summary>

```md title="Latest Updated: 24/05/2023"
Default 0
Scp106 1
Spreaded 2
Faded 3
```

</details>

### GeneratorState

<details><summary> <b>GeneratorState</b></summary>

```md title="Latest Updated: 24/05/2023"
None 1
Unlocked 2
Open 4
Activating 8
Engaged 16
```

</details>

### HotKeyButton

<details><summary> <b>Hot Keys</b></summary>

```md title="Latest Updated: 24/05/2023"
Keycard 0
PrimaryFirearm 1
SecondaryFirearm 2
Medical 3
Grenade 4
```

</details>

### IntercomStates

<details><summary> <b>Intercom States</b></summary>

```md title="Latest Updated: 24/05/2023"
Ready
Transmitting
TransmittingBypass
Restarting
AdminSpeaking
Muted
Custom
```

</details>

### BroadcastFlags

<details><summary> <b>BroadcastFlags</b></summary>

```md title="Latest Updated: 24/05/2023"
Normal 0
Truncated 1
AdminChat 2
```

</details>



### AttachmentNames

<details><summary> <b>Attachment Names</b></summary>

```md title="Latest Updated: 24/05/2023"
None 0
IronSights 1
DotSight 2
HoloSight 3
NightVisionSight 4
AmmoSight 5
ScopeSight 6
StandardStock 7
ExtendedStock 8
RetractedStock 9
LightweightStock 10
HeavyStock 11
RecoilReducingStock 12
Foregrip 13
Laser 14
Flashlight 15
AmmoCounter 16
StandardBarrel 17
ExtendedBarrel 18
SoundSuppressor 19
FlashHider 20
MuzzleBrake 21
MuzzleBooster 22
StandardMagFMJ 23
StandardMagAP 24
StandardMagJHP 25
ExtendedMagFMJ 26
ExtendedMagAP 27
ExtendedMagJHP 28
DrumMagFMJ 29
DrumMagAP 30
DrumMagJHP 31
LowcapMagFMJ 32
LowcapMagAP 33
LowcapMagJHP 34
CylinderMag4 35
CylinderMag6 36
CylinderMag8 37
CarbineBody 38
RifleBody 39
ShortBarrel 40
ShotgunChoke 41
ShotgunExtendedBarrel 42
NoRifleStock 43
ShotgunSingleShot 44
ShotgunDoubleShot 45
```

</details>

### SpawnReasons

<details><summary> <b>Spawn Reasons</b></summary>

```md title="Latest Updated: 24/05/2023"
None 0
RoundStart 1
LateJoin 2
Respawn 3
Died 4
Escaped 5
Revived 6
ForceClass 7
Destroyed 8
```

</details>

### Prefabs

<details><summary> <b>Available Prefabs</b></summary>

```md title="Latest Updated: 24/05/2023"
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

