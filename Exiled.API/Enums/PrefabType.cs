// -----------------------------------------------------------------------
// <copyright file="PrefabType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    using Exiled.API.Features.Attributes;

    /// <summary>
    /// Type of prefab.
    /// </summary>
    public enum PrefabType
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1602 // Enumeration items should be documented
        [Prefab(1883254029, "Player")]
        Player,

        [Prefab(2295511789, "EZ BreakableDoor")]
        EZBreakableDoor,

        [Prefab(2295511789, "HCZ BreakableDoor")]
        HCZBreakableDoor,

        [Prefab(3038351124, "LCZ BreakableDoor")]
        LCZBreakableDoor,

        [Prefab(1704345398, "sportTargetPrefab")]
        SportTarget,

        [Prefab(858699872, "dboyTargetPrefab")]
        DBoyTarget,

        [Prefab(3613149668, "binaryTargetPrefab")]
        BinaryTarget,

        [Prefab(1306864341, "TantrumObj")]
        TantrumObj,

        [Prefab(1321952889, "PrimitiveObjectToy")]
        PrimitiveObjectToy,

        [Prefab(3956448839, "LightSourceToy")]
        LightSourceToy,

        [Prefab(2672653014, "RegularKeycardPickup")]
        RegularKeycardPickup,

        [Prefab(335436768, "ChaosKeycardPickup")]
        ChaosKeycardPickup,

        [Prefab(248357067, "RadioPickup")]
        RadioPickup,

        [Prefab(1925130715, "Com15Pickup")]
        Com15Pickup,

        [Prefab(2808038258, "MedkitPickup")]
        MedkitPickup,

        [Prefab(2606539874, "FlashlightPickup")]
        FlashlightPickup,

        [Prefab(2974277164, "MicroHidPickup")]
        MicroHidPickup,

        [Prefab(1367360155, "SCP500Pickup")]
        SCP500Pickup,

        [Prefab(689511071, "SCP207Pickup")]
        SCP207Pickup,

        [Prefab(4056235189, "Ammo12gaPickup")]
        Ammo12gaPickup,

        [Prefab(212068596, "E11SRPickup")]
        E11SRPickup,

        [Prefab(1982658896, "CrossvecPickup")]
        CrossvecPickup,

        [Prefab(2474630775, "Ammo556mmPickup")]
        Ammo556mmPickup,

        [Prefab(3462306180, "Fsp9Pickup")]
        Fsp9Pickup,

        [Prefab(2405374689, "LogicerPickup")]
        LogicerPickup,

        [Prefab(1273232029, "HegPickup")]
        HegPickup,

        [Prefab(3871663704, "FlashbangPickup")]
        FlashbangPickup,

        [Prefab(1499866827, "Ammo44calPickup")]
        Ammo44calPickup,

        [Prefab(3685499023, "Ammo762mmPickup")]
        Ammo762mmPickup,

        [Prefab(2344368365, "Ammo9mmPickup")]
        Ammo9mmPickup,

        [Prefab(1749039070, "Com18Pickup")]
        Com18Pickup,

        [Prefab(3525743409, "Scp018Projectile")]
        Scp018Projectile,

        [Prefab(3711531185, "SCP268Pickup")]
        SCP268Pickup,

        [Prefab(1573779433, "AdrenalinePrefab")]
        AdrenalinePrefab,

        [Prefab(3124923193, "PainkillersPickup")]
        PainkillersPickup,

        [Prefab(3134959991, "CoinPickup")]
        CoinPickup,

        [Prefab(941440279, "Light Armor Pickup")]
        LightArmorPickup,

        [Prefab(3118088094, "Combat Armor Pickup")]
        CombatArmorPickup,

        [Prefab(3164421243, "Heavy Armor Pickup")]
        HeavyArmorPickup,

        [Prefab(1861159387, "RevolverPickup")]
        RevolverPickup,

        [Prefab(3814984482, "AkPickup")]
        AkPickup,

        [Prefab(3180035653, "ShotgunPickup")]
        ShotgunPickup,

        [Prefab(464602874, "Scp330Pickup")]
        Scp330Pickup,

        [Prefab(1983050408, "Scp2176Projectile")]
        Scp2176Projectile,

        [Prefab(2088018000, "SCP244APickup Variant")]
        SCP244APickup,

        [Prefab(3030062014, "SCP244BPickup Variant")]
        SCP244BPickup,

        [Prefab(2702950243, "SCP1853Pickup")]
        SCP1853Pickup,

        [Prefab(3881162440, "DisruptorPickup")]
        DisruptorPickup,

        [Prefab(504857316, "Com45Pickup")]
        Com45Pickup,

        [Prefab(303271247, "SCP1576Pickup")]
        SCP1576Pickup,

        [Prefab(2915316078, "JailbirdPickup")]
        JailbirdPickup,

        [Prefab(1209253563, "AntiSCP207Pickup")]
        AntiSCP207Pickup,

        [Prefab(2216560136, "FRMG0Pickup")]
        FRMG0Pickup,

        [Prefab(74988289, "A7Pickup")]
        A7Pickup,

        [Prefab(3532394942, "LanternPickup")]
        LanternPickup,

        [Prefab(825024811, "Amnestic Cloud Hazard")]
        AmnesticCloudHazard,

        [Prefab(2286635216, "Scp018PedestalStructure Variant")]
        Scp018PedestalStructure,

        [Prefab(664776131, "Scp207PedestalStructure Variant")]
        Scp207PedestalStructure,

        [Prefab(3724306703, "Scp244PedestalStructure Variant")]
        Scp244PedestalStructure,

        [Prefab(3849573771, "Scp268PedestalStructure Variant")]
        Scp268PedestalStructure,

        [Prefab(373821065, "Scp500PedestalStructure Variant")]
        Scp500PedestalStructure,

        [Prefab(3962534659, "Scp1853PedestalStructure Variant")]
        Scp1853PedestalStructure,

        [Prefab(3578915554, "Scp2176PedestalStructure Variant")]
        Scp2176PedestalStructure,

        [Prefab(3372339835, "Scp1576PedestalStructure Variant")]
        Scp1576PedestalStructure,

        [Prefab(2830750618, "LargeGunLockerStructure")]
        LargeGunLockerStructure,

        [Prefab(3352879624, "RifleRackStructure")]
        RifleRackStructure,

        [Prefab(1964083310, "MiscLocker")]
        MiscLocker,

        [Prefab(2724603877, "GeneratorStructure")]
        GeneratorStructure,

        [Prefab(1783091262, "Spawnable Work Station Structure")]
        WorkstationStructure,

        [Prefab(4040822781, "RegularMedkitStructure")]
        RegularMedkitStructure,

        [Prefab(2525847434, "AdrenalineMedkitStructure")]
        AdrenalineMedkitStructure,

        [Prefab(427210814, "HegProjectile")]
        HegProjectile,

        [Prefab(2409733045, "FlashbangProjectile")]
        FlashbangProjectile,

        [Prefab(1062458989, "SCP-173_Ragdoll")]
        Scp173Ragdoll,

        [Prefab(1951328980, "Ragdoll_1")]
        Ragdoll1,

        [Prefab(992490681, "SCP-106_Ragdoll")]
        Scp106Ragdoll,

        [Prefab(3219675689, "Ragdoll_4")]
        Ragdoll4,

        [Prefab(417388851, "Ragdoll_7")]
        Ragdoll7,

        [Prefab(3185790062, "Ragdoll_6")]
        Ragdoll6,

        [Prefab(2567420661, "Ragdoll_8")]
        Ragdoll8,

        [Prefab(149379640, "SCP-096_Ragdoll")]
        Scp096Ragdoll,

        [Prefab(1862774274, "Ragdoll_10")]
        Ragdoll10,

        [Prefab(2710373253, "Ragdoll_Tut")]
        RagdollTutorial,

        [Prefab(1389252654, "Ragdoll_12")]
        Ragdoll12,

        [Prefab(3175759689, "SCP-939_Ragdoll")]
        Scp939Ragdoll,

        [Prefab(3721192489, "Scp3114_Ragdoll")]
        Scp3114Ragdoll,
    }
}