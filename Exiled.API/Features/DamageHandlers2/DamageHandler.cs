using Exiled.API.Enums;
using PlayerStatsSystem;
using BaseHandler = PlayerStatsSystem.StandardDamageHandler;

namespace Exiled.API.Features.DamageHandlers2
{
    public class DamageHandler : DamageHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DamageHandler"/> class.
        /// </summary>
        protected DamageHandler()
        {
        }

        public DamageHandler(Player target, Player attacker, float damage, DamageType damageType)
        {
            Target = target;
            Type = damageType;
        }

        public BaseHandler Base { get; }

        public Player Target { get; }

        public Player Attacker { get; }
    }
}