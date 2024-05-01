using Exiled.API.Enums;

namespace Exiled.API.Features.DamageHandlers2
{
    using BaseHandler = PlayerStatsSystem.AttackerDamageHandler;

    public class AttackerDamageHandler : DamageHandler
    {
        public AttackerDamageHandler(BaseHandler baseHandler, DamageType damageType)
            : base(baseHandler, damageType)
        {
        }

        public AttackerDamageHandler(Player target, Player attacker, float damage, DamageType damageType)
            : base(target, damage, damageType)
        {
            Attacker = attacker;
        }

        public Player Attacker { get; }
    }
}