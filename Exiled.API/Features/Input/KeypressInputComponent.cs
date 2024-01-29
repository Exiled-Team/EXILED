// -----------------------------------------------------------------------
// <copyright file="KeypressInputComponent.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Input
{
    using Exiled.API.Enums;

    /// <inheritdoc/>
    public class KeypressInputComponent : InputActionComponent
    {
        /// <summary>
        /// Gets or sets the <see cref="InputBinding"/> paired to <see cref="UUKeypressTriggerType.KT_INPUT_0"/>.
        /// </summary>
        protected InputBinding Input_KT0 { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="InputBinding"/> paired to <see cref="UUKeypressTriggerType.KT_INPUT_1"/>.
        /// </summary>
        protected InputBinding Input_KT1 { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="InputBinding"/> paired to <see cref="UUKeypressTriggerType.KT_INPUT_2"/>.
        /// </summary>
        protected InputBinding Input_KT2 { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="InputBinding"/> paired to <see cref="UUKeypressTriggerType.KT_INPUT_3"/>.
        /// </summary>
        protected InputBinding Input_KT3 { get; set; }

        /// <inheritdoc/>
        protected override void PostInitialize()
        {
            base.PostInitialize();

            BindInputActions();
        }

        /// <summary>
        /// Input actions should be bound here, in order to avoid conflicts with the current execution pipeline.
        /// <para/>
        /// All <see cref="InputBinding"/> conditions are set to <see langword="false"/> by default.
        /// </summary>
        protected virtual void BindInputActions()
        {
            BindInputAction(InputBinding.Create("INPUT_KT0", InputCondition_KT0, InputAction_KT0, UUKeypressTriggerType.KT_INPUT_0));
            BindInputAction(InputBinding.Create("INPUT_KT1", InputCondition_KT1, InputAction_KT1, UUKeypressTriggerType.KT_INPUT_1));
            BindInputAction(InputBinding.Create("INPUT_KT2", InputCondition_KT2, InputAction_KT2, UUKeypressTriggerType.KT_INPUT_2));
            BindInputAction(InputBinding.Create("INPUT_KT3", InputCondition_KT3, InputAction_KT3, UUKeypressTriggerType.KT_INPUT_3));
        }

        /// <summary>
        /// The input condition, paired to <see cref="UUKeypressTriggerType.KT_INPUT_0"/>, to be evaluated.
        /// </summary>
        /// <returns><see langword="true"/> if the condition was satified; otherwise, <see langword="false"/>.</returns>
        protected virtual bool InputCondition_KT0() => false;

        /// <summary>
        /// The input condition, paired to <see cref="UUKeypressTriggerType.KT_INPUT_1"/>, to be evaluated.
        /// </summary>
        /// <returns><see langword="true"/> if the condition was satified; otherwise, <see langword="false"/>.</returns>
        protected virtual bool InputCondition_KT1() => false;

        /// <summary>
        /// The input condition, paired to <see cref="UUKeypressTriggerType.KT_INPUT_2"/>, to be evaluated.
        /// </summary>
        /// <returns><see langword="true"/> if the condition was satified; otherwise, <see langword="false"/>.</returns>
        protected virtual bool InputCondition_KT2() => false;

        /// <summary>
        /// The input condition, paired to <see cref="UUKeypressTriggerType.KT_INPUT_3"/>, to be evaluated.
        /// </summary>
        /// <returns><see langword="true"/> if the condition was satified; otherwise, <see langword="false"/>.</returns>
        protected virtual bool InputCondition_KT3() => false;

        /// <summary>
        /// The input action, paired to <see cref="UUKeypressTriggerType.KT_INPUT_0"/>, to be executed.
        /// </summary>
        protected virtual void InputAction_KT0()
        {
        }

        /// <summary>
        /// The input action, paired to <see cref="UUKeypressTriggerType.KT_INPUT_1"/>, to be executed.
        /// </summary>
        protected virtual void InputAction_KT1()
        {
        }

        /// <summary>
        /// The input action, paired to <see cref="UUKeypressTriggerType.KT_INPUT_2"/>, to be executed.
        /// </summary>
        protected virtual void InputAction_KT2()
        {
        }

        /// <summary>
        /// The input action, paired to <see cref="UUKeypressTriggerType.KT_INPUT_3"/>, to be executed.
        /// </summary>
        protected virtual void InputAction_KT3()
        {
        }
    }
}