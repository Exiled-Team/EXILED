# Ongoing API Rework
This text is continuously updated to announce or explain changes—whether they're pending, already implemented, or coming soon. It remains a work in progress as we refine and optimize the Exiled API.

## Inheritable `enum`.
In a significant overhaul, we're introducing inheritable enums to replace identifiers. These enums, namely `EnumClass`, `UnmanagedEnumClass`, and `UniqueUnmanagedEnumClass` with the prefixes `EC`, `UE`, and `UU`, act as classes extending the capabilities of regular enums. They offer flexibility by seamlessly integrating with existing enums and accepting unmanaged data types. To ensure uniqueness and prevent conflicts, we've introduced the `UniqueUnmanagedEnumClass`.

## `Pawn` Class
To streamline the Exiled API without custom APIs while supporting these changes, we're introducing a new `Player` derived class named `Pawn`. This class goes beyond the standard `Player` functionalities, offering additional methods and properties tailored for enhanced ease of use and compatibility with custom APIs.

## `GameEntity` Class
The `GameEntity` class, introduced in Exiled API, provides a mechanism to designate certain entities from the underlying game as Exiled game entities. This class seamlessly integrates with the ECS (Exiled Component System), offering native support for all its features. It simplifies the addition and management of ECS components, eliminating the need for static extension methods or static approaches.

## `EBehaviour<T>` Class
The revamped `EBehaviour` class now exists as a generic version, replacing the previous non-generic variant and offering enhanced functionality. This updated class introduces three specialized wrapped versions: `EPlayerBehaviour`, `EItemBehaviour`, and `EPickupBehaviour`. Each wrapped version is meticulously crafted to handle and manage its respective entity type, providing tailored functionality for a seamless integration with custom behaviors. The introduction of the generic version adds versatility and clarity to the overall design, allowing for a more streamlined and efficient utilization of entity-specific behaviors.

## `ItemTracker` Class
The `ItemTracker` is a `StaticActor` and central component for managing and tracking behaviours associated with items and pickups in the game. Let's delve into its logical structure and functionality.

### Event Dispatchers

The class employs dynamic event dispatchers to handle various events triggered during item and pickup interactions. These dispatchers serve as flexible conduits for executing custom logic tied to specific game events.

Item Events:

- `ItemAddedDispatcher:` Manages delegates fired when an item is added.
- `ItemRemovedDispatcher:` Manages delegates fired when an item is removed.
- `ItemRestoredDispatcher:` Manages delegates fired when an item is restored.
- `ItemTrackingModifiedDispatcher:` Manages delegates fired when an item's tracking is modified.

Pickup Events:

- `PickupAddedDispatcher:` Manages delegates fired when a pickup is added.
- `PickupRemovedDispatcher:` Manages delegates fired when a pickup is removed.
- `PickupRestoredDispatcher:` Manages delegates fired when a pickup is restored.
- `PickupTrackingModifiedDispatcher:` Manages delegates fired when a pickup's tracking is modified.

The `ItemTracker` actor acts as a robust system for dynamically handling the complexities of item and pickup interactions in the game, providing a flexible and scalable solution for tracking and managing associated behaviours.
The actor is being actively used for `CustomItem` and `CustomAbility` APIs.

## `NullableObject` Class

The `NullableObject` class is an abstract base class that serves as a foundation for defining nullable objects.

### Implicit Conversion Operator

#### `implicit operator bool(NullableObject @object)`
The class includes an implicit conversion operator that allows instances of the `NullableObject` class to be implicitly converted to a boolean value. This conversion facilitates conditional checks for nullability. If the `NullableObject` instance is not null, the conversion yields `true`; otherwise, it results in `false`.

### How It Works
The `NullableObject` class provides a simple mechanism for representing nullable objects by implementing an implicit conversion to a boolean value. When instances of this class are used in a boolean context (e.g., in conditional statements), the implicit conversion operator checks whether the instance is not null, allowing for concise and expressive nullability checks in code.

This class can be used as a base for other classes that need to convey nullable semantics while leveraging boolean expressions for clarity.

## `ITypeCast` Interface

The `ITypeCast` interface acts as a mechanism for casting objects of a specified type (`T`) to other types in a flexible and safe manner. It provides methods for both unsafe and safe casting, enabling seamless conversion between types.

### Method Details

#### `Cast<TObject>()`
This method unsafely casts the current instance of type `T` to the specified `TObject` type. It assumes that the casting operation is always valid and may lead to runtime exceptions if misused.

#### `Cast<TObject>(out TObject param)`
The safe casting method attempts to cast the current instance of type `T` to the specified `TObject` type. If the casting is successful, it returns `true` and provides the casted object through the `param` parameter. If the casting fails, it returns `false`.

#### `As<TObject>()`
An alias for the `Cast<TObject>()` method, providing a more readable and expressive alternative.

#### `Is<TObject>(out TObject param)`
An alias for the `Cast<TObject>(out TObject param)` method, offering a more intuitive way to express the casting operation.

### How It Works
The `ITypeCast` interface essentially facilitates the casting of objects of type `T` to other types, offering both an unsafe method for situations where the casting is assumed to be valid and a safe method for scenarios where validation is required. It aims to provide a flexible and understandable way to work with type conversions in a generic context.

## `TypeCastMono<T>` Class

The `TypeCastMono<T>` class is an abstract `MonoBehaviour` that implements the `ITypeCast<T>` interface. It serves as a generic mechanism for casting MonoBehaviours to other specified types in a Unity environment.

### How It Works
The `TypeCastMono<T>` class extends `MonoBehaviour` and implements the `ITypeCast<T>` interface. It provides a basic structure for safely and unsafely casting MonoBehaviours to other types in a Unity project. Derived classes can use this as a foundation to enable type casting for specific MonoBehaviours while ensuring flexibility and safety in the casting process.

## Current Structure Evolution
Our existing structure has evolved to accommodate these changes. In the `Legacy` version, we have:
```cs
// Serializable class - Serves as a serializable config class, packing in more values and insights about the custom behavior.
abstract class CustomBehaviour : TypeCastObject<CustomBehaviour>
```
```cs
// Execution class (Gameplay) - Functions as a class with the actual logic, leveraging the ECS (Exiled Component System).
abstract class BehaviourComponent : EBehaviour
```
```cs
// Settings class - Functions as a non-serializable config class, but can be serialized in the CustomBehaviour class. It holds useful values influencing the custom behavior.
class Settings
```
In the `Additive Properties` version, we've introduced:
```cs
// Serializable class - Serves as a serializable config class, packing in more values and insights about the custom behavior.
abstract class CustomBehaviour : TypeCastObject<CustomBehaviour>, IAdditiveBehaviour
```
```cs
// Execution class (Gameplay) - Functions as a class with the actual logic, leveraging the ECS (Exiled Component System).
abstract class BehaviourComponent : EBehaviour, IAdditiveSettings<Settings>
```
```cs
// Settings class - Functions as a non-serializable config class, but can be serialized in the CustomBehaviour class. It holds useful values influencing the custom behavior.
class Settings : IAddittiveProperty
```
Additionally, in the `Additive Properties Collection` version, we've further expanded our structure to:
```cs

// Execution class (Gameplay) - Functions as a class with the actual logic, leveraging the ECS (Exiled Component System).
abstract class BehaviourComponent : EBehaviour, IAdditiveSettingsCollection<Settings>
```

These changes aim to enhance the modularity, flexibility, and overall coherence of the Exiled API, allowing for seamless integration and expansion of custom features.

## Documentation and Flexibility

Documentation is a crucial part of this rework process and should be created alongside the implementation of the entire API overhaul. Until stated otherwise, every aspect is subject to potential modifications and even radical changes if deemed necessary. This flexibility ensures that we can adapt swiftly to evolving requirements and achieve the most optimal design for the Exiled API.

# Custom Roles API Overview

The Custom Roles API introduces a versatile framework for managing custom roles in your game. Let's delve into the key components:

## `CustomRole` Class

The `CustomRole` class serves as the serializable base class for custom roles. For a more specialized version, you can utilize `CustomRole<T>`, where `T` must be a subclass of `RoleBehaviour`. This provides a flexible foundation for defining distinct role behaviors.

## `RoleSettings` Class

To fine-tune the characteristics of your custom roles, the `RoleSettings` class comes into play. It offers an array of settings that can be leveraged during the creation of custom roles. These settings empower you to tailor various aspects of your roles to meet specific gameplay requirements.

## `RoleBehaviour` Class

The `RoleBehaviour` class takes center stage in implementing the actual gameplay modifications associated with a custom role. It serves as the execution class, allowing you to define the unique behaviors and interactions that the role brings to the game.

## `CustomTeam` Class

For a more comprehensive customization experience, the API includes the `CustomTeam` class. This feature enables the creation of custom teams by assembling units with specific custom roles. By leveraging the `CustomTeam` class, you can design intricate team dynamics within your game.

This API provides a robust foundation for crafting diverse and engaging roles, offering flexibility in role behaviors, extensive customization through settings, and the ability to structure custom teams. Explore the possibilities and elevate your game with the Custom Roles API.

# Custom Escapes API Overview

The Custom Escapes API introduces a robust framework for managing custom escape scenarios in your game. Let's explore the key elements of this versatile API:

## `CustomEscape` Class

The cornerstone of the API is the `CustomEscape` class, serving as the serializable base class for defining custom escape scenarios. For more specialized instances, developers can leverage `CustomEscape<T>`, where `T` must be a subclass of `EscapeBehaviour`. This flexible structure empowers you to craft unique escape behaviors tailored to specific scenarios.

## `EscapeSettings` Class

Fine-tune your custom escape scenarios with the `EscapeSettings` class. This component provides an array of settings that enhance the creative possibilities during the creation of custom escapes. These settings allow you to shape various aspects of escape scenarios, adapting them to specific gameplay requirements.

## `EscapeBehaviour` Class

Implementing the actual gameplay modifications associated with custom escapes is where the `EscapeBehaviour` class shines. This execution class allows you to define distinctive behaviors and interactions for custom escapes, ensuring an engaging and immersive experience. Notably, it seamlessly handles escape mechanics, even for base game roles that may not typically be allowed to escape from the inner escape zone.

This API streamlines the process of creating diverse and compelling escape scenarios. Whether you're crafting unique escape behaviors, customizing scenarios with settings, or ensuring a seamless experience for all roles, the Custom Escapes API provides the tools you need.

# Custom Abilities API Overview

Custom abilities seamlessly integrate with all game entities (`GameEntity`), providing a universal framework for leveraging unique abilities across various aspects of the game. This generic implementation ensures the applicability of custom abilities to entities such as `Player`, `Item`, and `Pickup`, and holds the potential for extension to other game entities. Specialized wrappers have been meticulously crafted for `Player`, `Item`, and `Pickup`, each featuring customized implementations tailored to their specific methods. The design's inherent flexibility allows for the future inclusion of wrappers designed for additional game entities, further expanding the adaptability and versatility of custom abilities.

The system currently comprises five foundational ability behavior classes, each defining distinct characteristics of the ability: `AbilityBehaviourBase<TEntity>`, `ActiveAbilityBehaviour<TEntity>`, `LevelAbilityBehaviour<TEntity>`, `PassiveAbilityBehaviour<TEntity>`, and `UnlockableAbilityBehaviour<TEntity>`. The wrapped classes share the same names as those mentioned above, organized within different namespaces. Notably, these wrapped classes do not necessitate a generic `TEntity` parameter, as they already define it through inheritance from the respective base classes. This organizational approach streamlines usage and enhances the clarity of the custom abilities system.

## `CustomAbility<T>` Class
The `CustomAbility<T>` class serves as a versatile tool for defining and managing abilities in a generic manner. It provides the flexibility to implement abilities either in their raw form or wrapped within custom wrappers or existing ones. This approach ensures a seamless integration of diverse abilities across different contexts, making it a powerful and adaptable solution for managing a variety of game entity abilities.
In contrast to other classes, the `CustomAbility<T>` class lacks a non-generic version, requiring mandatory inclusion of the `T` parameter, which must be of type `GameEntity`.

## `AbilitySettings` Class
The `AbilitySettings` class serves as a foundational structure for defining settings integral to the functionality of custom abilities. Designed to be inherited and implementing the `IAdditiveProperty` interface, it offers a versatile framework for configuring various aspects of abilities. Developers can tailor these settings to align with the desired behavior and characteristics of specific abilities.

Key Features:

- Inheritance Compatibility: The class is designed for inheritance, allowing developers to build upon its structure to create custom settings tailored to specific abilities.
- IAdditiveProperty Interface: By implementing the IAdditiveProperty interface, the class seamlessly integrates with Exiled's additive property system, facilitating additive modifications to settings.
- Comprehensive Configuration: Various settings are encapsulated within the class, including cooldown, forced cooldown on addition, windup time, duration, default level, maximum level, and status hints. This comprehensive set of parameters empowers developers to finely tune the behavior of custom abilities.

## `IAbilityBehaviour` Interface
This interface serves as a marker interface for custom ability behaviors, ensuring a unified base type for easy retrieval during iterations or similar operations. Its primary purpose is to guarantee that all implementations are recognized as custom ability behaviors.

## `AbilityBehaviourBase<TEntity>` Class
The `AbilityBehaviourBase<TEntity>` class stands as a cornerstone in the technical architecture of custom abilities within the system. Its primary function lies in encapsulating and orchestrating the intricate gameplay logic specific to a designated entity type, denoted by the generic parameter `TEntity`.

By virtue of its generic design, this class seamlessly integrates with various entity types, such as `Player`, `Item`, and `Pickup`, among others. It provides a structured framework for implementing and defining the nuanced gameplay mechanics intrinsic to each custom ability.

Developers can extend this class to create specialized ability behaviors, tailoring the logic to the unique characteristics of the associated entity. This technical foundation ensures a modular and extensible approach to custom ability development, promoting code reusability and maintainability across diverse entities.

## `PassiveAbilityBehaviour<TEntity>` Class
The `PassiveAbilityBehaviour<TEntity>` class extends the functionality of the base class, `AbilityBehaviourBase<TEntity>`, without introducing any additional elements. It serves as a specialized implementation of custom abilities characterized by a permanent and passive nature.

In technical terms, this class is designed to encapsulate a set of behaviors that persist indefinitely without requiring continuous actions from the associated entity. Developers are expected to define the specifics of the passive behavior by leveraging Exiled's event system and related mechanisms.

By inheriting from the base class, `PassiveAbilityBehaviour<TEntity>` gains access to the structured framework for managing the core logic of custom abilities linked to a specific entity type. This class is particularly suitable for scenarios where ongoing player or entity interaction is unnecessary.

## `ActiveAbilityBehaviour<TEntity>`
The `ActiveAbilityBehaviour<TEntity>` class extends the functionality of the base class, `AbilityBehaviourBase<TEntity>`. It serves as a comprehensive structure for implementing active abilities, empowering the associated entity to decide when and how to trigger the ability.

Key Features:

- Cooldown System: The class incorporates a built-in cooldown mechanism, regulating the frequency of ability usage.
- Extensive Override Methods: Developers can override numerous methods to tailor the logic of the active ability to specific requirements.
- Event Notifications: Events are integrated to signal changes in the ability's status and workflow.
- Duration Control: The class offers flexibility by supporting both one-time, one-shot abilities with no duration and duration-based abilities that remain active for a specified period.

Developers can leverage `ActiveAbilityBehaviour<TEntity>` as a foundation for creating dynamic and interactive abilities, allowing entities to actively manage the activation and execution of the ability in diverse scenarios.

## `LevelAbilityBehaviour<TEntity>` Class
The `LevelAbilityBehaviour<TEntity>` class, an extension of the `ActiveAbilityBehaviour<TEntity>` class, introduces a nuanced layer by incorporating support for levels. This feature empowers developers to design abilities with multiple levels, influencing their behavior based on the current level.

Key Aspects:

- Level Support: This class facilitates the creation of active abilities with distinct levels, offering developers the ability to customize the behavior for varying levels.
- Method Overrides: Extensive methods are available for developers to override, allowing fine-grained control over the logic associated with different levels.
- Event Notifications: Events are seamlessly integrated, providing real-time updates when the ability's level changes, is added, removed, or when the maximum level is attained.
- Level Progression Logic: The class incorporates a method to define the conditions triggering level increments or decrements.

By leveraging `LevelAbilityBehaviour<TEntity>`, developers can imbue active abilities with a scalable and adaptable structure, responsive to dynamic changes in levels and offering a rich, multi-tiered user experience.

## `UnlockableAbilityBehaviour<TEntity>`
The `UnlockableAbilityBehaviour<TEntity>` class, an extension of the `LevelAbilityBehaviour<TEntity>`, introduces the concept of unlockable abilities. Inherent to its design, this class mandates specific conditions to be met before the ability can be utilized. Developers can seamlessly integrate this feature, enhancing gameplay dynamics by requiring players to fulfill prerequisites for unlocking and employing the ability.

Key Features:

- Unlockable Functionality: This class enables the creation of abilities that demand fulfillment of predetermined conditions before becoming accessible, contributing to a layered and strategic gameplay experience.
- Condition Definition: Developers can articulate the conditions necessary for unlocking the ability through a dedicated method, affording granular control over the unlocking process.
- Event-driven Updates: Real-time event notifications keep developers informed about the current state and workflow of the unlockable ability, facilitating responsive and dynamic interactions.

By leveraging `UnlockableAbilityBehaviour<TEntity>`, developers can introduce a captivating layer of progression and strategy to their game, where players must strategically meet certain criteria to unlock and unleash powerful abilities.