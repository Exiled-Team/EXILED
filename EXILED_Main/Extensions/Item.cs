namespace EXILED.Extensions {
    public static class Item {

        public static void SetWeaponAmmo( this Inventory.SyncItemInfo item, int amount ) => item.durability = amount;

        public static float GetWeaponAmmo( this Inventory.SyncItemInfo item ) => item.durability;

        public static bool IsAmmo( this ItemType item ) =>
            (item == ItemType.Ammo556 || item == ItemType.Ammo9mm || item == ItemType.Ammo762);

        public static bool IsWeapon( this ItemType type ) =>
            (type == ItemType.GunCOM15 || type == ItemType.GunE11SR || type == ItemType.GunLogicer
            || type == ItemType.GunMP7 || type == ItemType.GunProject90 || type == ItemType.GunUSP
            || type == ItemType.MicroHID);

        public static bool IsSCP( this ItemType type ) =>
            (type == ItemType.SCP018 || type == ItemType.SCP500 || type == ItemType.SCP268 || type == ItemType.SCP207);

        public static bool IsThrowable( this ItemType type ) =>
            (type == ItemType.SCP018 || type == ItemType.GrenadeFrag || type == ItemType.GrenadeFlash);

        public static bool IsMedical( this ItemType type ) =>
            (type == ItemType.Painkillers || type == ItemType.Medkit || type == ItemType.SCP500 || type == ItemType.Adrenaline);

        public static bool IsUtility( this ItemType type ) =>
            (type == ItemType.Disarmer || type == ItemType.Flashlight || type == ItemType.Radio || type == ItemType.WeaponManagerTablet);

        public static bool IsKeycard( this ItemType type ) =>
            (ItemType.KeycardChaosInsurgency == type || ItemType.KeycardContainmentEngineer == type || ItemType.KeycardFacilityManager == type
            || ItemType.KeycardGuard == type || ItemType.KeycardJanitor == type || ItemType.KeycardNTFCommander == type
            || ItemType.KeycardNTFLieutenant == type || ItemType.KeycardO5 == type || ItemType.KeycardScientist == type
            || ItemType.KeycardScientistMajor == type || ItemType.KeycardSeniorGuard == type || ItemType.KeycardZoneManager == type);

    }
}
