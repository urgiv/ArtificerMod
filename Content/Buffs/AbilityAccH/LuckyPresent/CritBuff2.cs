using Terraria;
using Terraria.ModLoader;

namespace ArtificerMod.Content.Buffs.AbilityAccH.LuckyPresent
{
	public class CritBuff2 : ModBuff
	{
        public override void Update(Player player, ref int buffIndex)
        {
			player.GetCritChance(DamageClass.Generic) += 20f;
		}
    }
}