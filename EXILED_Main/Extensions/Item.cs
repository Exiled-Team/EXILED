namespace EXILED.Extensions 
{
    public static class Item 
    {

        /// <summary>
        /// Set the durability value of an <see cref="Inventory.SyncItemInfo">item</see>.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        public static void SetWeaponAmmo(this Inventory.SyncListItemInfo list, Inventory.SyncItemInfo item, int amount) => list.ModifyDuration(list.IndexOf(item), amount);

        public static void SetWeaponAmmo(this ReferenceHub hub, Inventory.SyncItemInfo item, int amount) => hub.inventory.items.ModifyDuration(hub.inventory.items.IndexOf(item), amount);

        /// <summary>
        /// Get the durability value of an <see cref="Inventory.SyncItemInfo">item</see>.
        /// </summary>
        /// <param name="item"></param>
        public static float GetWeaponAmmo(this Inventory.SyncItemInfo item) => item.durability;
        
        /// <summary>
        /// Check if an <see cref="ItemType">item</see> is ammo.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsAmmo(this ItemType item) =>
            item == ItemType.Ammo556 || item == ItemType.Ammo9mm || item == ItemType.Ammo762;

        /// <summary>
        /// Check if an <see cref="ItemType">item</see> is a weapon.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>

        public static bool IsWeapon( this ItemType type , bool checkMicro) =>
            type == ItemType.GunCOM15 || type == ItemType.GunE11SR || type == ItemType.GunLogicer
            || type == ItemType.GunMP7 || type == ItemType.GunProject90 || type == ItemType.GunUSP
            || (checkMicro && type == ItemType.MicroHID);

        /// <summary>
        /// Check if an <see cref="ItemType">item</see> is a SCP item.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsSCP(this ItemType type) =>
            type == ItemType.SCP018 || type == ItemType.SCP500 || type == ItemType.SCP268 || type == ItemType.SCP207;

        /// <summary>
        /// Check if an <see cref="ItemType">item</see> is a throwable item.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsThrowable(this ItemType type) =>
            type == ItemType.SCP018 || type == ItemType.GrenadeFrag || type == ItemType.GrenadeFlash;

        /// <summary>
        /// Check if an <see cref="ItemType">item</see> is a medical item.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsMedical(this ItemType type) =>
            type == ItemType.Painkillers || type == ItemType.Medkit || type == ItemType.SCP500 || type == ItemType.Adrenaline;

        /// <summary>
        /// Check if an <see cref="ItemType">item</see> is a utility item.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsUtility(this ItemType type) =>
            type == ItemType.Disarmer || type == ItemType.Flashlight || type == ItemType.Radio || type == ItemType.WeaponManagerTablet;

        /// <summary>
        /// Check if an <see cref="ItemType">item</see> is a keycard.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsKeycard(this ItemType type) =>
            ItemType.KeycardChaosInsurgency == type || ItemType.KeycardContainmentEngineer == type || ItemType.KeycardFacilityManager == type
            || ItemType.KeycardGuard == type || ItemType.KeycardJanitor == type || ItemType.KeycardNTFCommander == type
            || ItemType.KeycardNTFLieutenant == type || ItemType.KeycardO5 == type || ItemType.KeycardScientist == type
            || ItemType.KeycardScientistMajor == type || ItemType.KeycardSeniorGuard == type || ItemType.KeycardZoneManager == type;
    }
}

