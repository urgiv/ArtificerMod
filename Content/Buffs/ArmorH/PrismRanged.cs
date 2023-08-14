using ArtificerMod.Common;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArtificerMod.Content.Buffs.ArmorH
{
	public class PrismRanged : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoTimeDisplay[Type] = false;
			Main.debuff[Type] = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetDamage(DamageClass.Ranged) += 0.04f;
			if (!player.TryGetModPlayer(out ArtificerPlayer artificer))
			{
				return;
			}
			if (artificer.radiantArmorFlag)
			{
				player.GetCritChance(DamageClass.Ranged) += 4f;
			}
		}
	}
}