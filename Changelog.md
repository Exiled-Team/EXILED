# Exiled 2.14.0 -> 3.0.0 changelog
### This will be a mostly all-inclusive list of all changes between EXILED 2.14.0 and the current 3.0.0 alpha build. This is meant to work as a guide for Plugin Developers to see and understand the API changes that have occured.

First of all, anything that was marked as [Obsolete] in 2.14.0 has been removed from 3.0.0.
This includes all things like outdated extensions (we still had SCP-330 STUFF!) and Stuff like Map.SpawnRagdoll. Not all obsolete removals will be noted, consider this fair warning that if you were using an obsolete something, it's gone now.
This may not be a completely all-inclusive list of every change, as there are MANY of them. But the goal is to give general direction for people to look for what they need.

## Exiled.API
### Renamed/Moved API Features
- EffectType.Scp268 -> EffectType.Invisible
- DoorExtensions -> Features.Door


### Changes to Existing API Features
- AmmoType has been expanded to include all new base-game ammo types.
- ZoneType is now a flag.
- RoleType.GetRandomSpawnPoint() -> RoleTYpe.GetRandomSpawnProperties()
- Map.IsLCZDecontaminated -> Map.IsLczDecontaminated
- Map.TurnOffAllLights(float, bool) -> Map.TurnOffAllLights(float, ZoneType)
  - ZoneType is a flag, so multiple values can be set at once, if for example you want LCZ and ENT to go dark, but not HCZ. If you pass the sole flag as Unspecified, it will blackout every light in every room.
- Map.SpawnGrenade -> Player.ThrowItem() OR Throwable.Throw()
- AmmoBox Player.Ammo -> Dictionary<ItemType, ushort> Player.Ammo
- int Player.CufferId -> Player Player.Cuffer
- Player.AddItem(SyncItemInfo) -> Player.AddItem(Item)
  - You can also use Item.Give(Player)



### New API Features
- Enum: DoorBeepType
- Enum: DoorLockType
- Enum: HidState
- Enum: ItemType
- Enum: KeycardPermissions
- Enum: RadioRange
- Enum: SpawnReason
- Enum: ThrowRequest
- Struct: ArmorAmmoLimit
- Struct: RadioRangeSettings
- Feature: Item
  - Inherited by: Ammo, Armor, ExplosiveGrenade (also inherits Throwable), Firearm, FlashGrenade (also inherits Throwable), Keycard, MicroHID, Radio, Throwable, Usable
    * You can implicitly cast between Item and any of these subtypes. Most events and Player API objects will be given as generic Items, but you can `if (ev.Item is Firearm firearm)` to access firearm-specific features, if it is a firearm.
  - To spawn an item, instead of the old `ItemType.Type.Spawn(Vector3)` extension, you now use `new Item(ItemType).Spawn(Vector3)`. Note that if you want to change attributes of the item, you can do so in the initialization like so: `new Item(ItemType) { Weight = 5f }.Spawn(Vector3)`.
    - If you want to spawn a specific subtype item, like a gun, and change things like how much ammo it has, it's attachments, etc. You can use ``new Firearm(ItemType) { Ammo = byte.MaxValue, Weight = 10f }.Spawn(Vector3)`` for example. This works with all of the Item subtypes.
- Feature: Pickup
- Scp914.Scp914Controller
- Feature: Door
  - Includes most commonly used members, such as Door.Open, Door.Type, Door.LockType, Door.IsBreakable, etc.
- Feature: Map.Lockers
- Feature: Map.PocketDimensionTeleporters
- Feature: Map.Pickups
- Feature: Player.Radio
- Feature: Player.Get(uint)
  - This is to be used to get a player based off their NetworkIdentity.netId
- Feature: Player.ReloadWeapon()
- Feature: Player.ChangeEffectIntensity(EffectType)
- Feature: Room.LightIntensity
- Feature: Room.Cameras
- Feature: Server.PlayerCount
- Feature: Server.MaxPlayerCount
- Feature: Server.RunCommand()



## Exiled.Events
### Renamed Events/EventArgs
- ThrowingGrenade event has been renamed to ThrowingItem
- UsingMedicalItem event has been renamed to UsingItem

### Removed Events
- DequippedMedicalItem event has been removed.
- DroppedItem event has been removed.
- ChangedRole event has been removed.
