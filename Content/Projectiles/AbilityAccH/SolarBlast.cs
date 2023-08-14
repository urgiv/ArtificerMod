using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArtificerMod.Content.Projectiles.AbilityAccH
{

	public class SolarBlast : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 50;
		}

		public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.CultistBossLightningOrbArc);
			Projectile.aiStyle = -1;
			Projectile.hostile = false;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Generic;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 1000;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 12;
			Projectile.tileCollide = false;
			Projectile.extraUpdates = 6;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
			for (int i = 0; i < Projectile.oldPos.Length; i++)
			{
				if (Projectile.oldPos[i] == Vector2.Zero)
				{
					return false;
				}
				// Outer part/outline of the beam
				Vector2 drawPos = Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2f;
				Main.EntitySpriteDraw(texture, drawPos, null, new Color(175, 175, 100, 150), 0f, Projectile.Size / 2f, 1f, SpriteEffects.None, 0);
			}
			return true;
		}

		public override void PostDraw(Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
			for (int i = 0; i < Projectile.oldPos.Length && !(Projectile.oldPos[i] == Vector2.Zero); i++)
			{
				// Inner part of the beam
				Vector2 drawPos = Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2f;
				Main.EntitySpriteDraw(texture, drawPos, null, new Color(170, 170, 75, 50), 0f, Projectile.Size / 2f, 0.6f, SpriteEffects.None, 0);
			}
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			projHitbox.Inflate(36, 36);
			// Makes a damaging trail
			for (int i = 0; i < Projectile.oldPos.Length; i++)
			{
				if (Projectile.oldPos[i] != Vector2.Zero)
				{
					Rectangle trailHitbox = new Rectangle((int)Projectile.oldPos[i].X, (int)Projectile.oldPos[i].Y, Projectile.width, Projectile.height);
					if (targetHitbox.Intersects(trailHitbox))
					{
						return true;
					}
				}
			}
			return targetHitbox.Intersects(projHitbox);
		}

		// This makes it so the projectile trail doesn't abruptly vanish all at once
		public override bool OnTileCollide(Vector2 oldVelocity)
        {
			Projectile.velocity = Vector2.Zero;
			Projectile.damage = 0;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 50;
			return false;
        }

		// Same effect as above, but for when the projectile is to expire
		bool timeSet = false;
		public void PreKill()
		{
			Projectile.velocity = Vector2.Zero;
			Projectile.damage = 0;
			Projectile.tileCollide = false;
			if(timeSet == false)
            {
				Projectile.timeLeft = 50;
				timeSet = true;
			}
		}

		// Adapted from the Lunatic Cultist's lightning
		public override void AI()
		{
			if(Projectile.timeLeft <= 50)
            {
				PreKill();
				return;
            }
			Projectile.frameCounter++;
			Lighting.AddLight(Projectile.Center, 0.7f, 0.7f, 0.35f);
			if (Projectile.velocity == Vector2.Zero)
			{
				if (Projectile.frameCounter >= Projectile.extraUpdates * 2)
				{
					Projectile.frameCounter = 0;
					bool killProj = true;
					for (int i = 1; i < Projectile.oldPos.Length; i++)
					{
						if (Projectile.oldPos[i] != Projectile.oldPos[0])
						{
							killProj = false;
						}
					}
					if (killProj)
					{
						Projectile.Kill();
						return;
					}
				}
				if (Main.rand.NextBool(Projectile.extraUpdates))
				{
					for (int j = 0; j < 2; j++)
					{
						float dustRot = Projectile.rotation + ((Main.rand.NextBool(2)) ? (-1f) : 1f) * ((float)Math.PI / 2f);
						float dustSpd = (float)Main.rand.NextDouble() * 0.8f + 1f;
						Vector2 dustVel = new Vector2((float)Math.Cos((double)dustRot) * dustSpd, (float)Math.Sin((double)dustRot) * dustSpd);
						int dust1 = Dust.NewDust(Projectile.Center, 0, 0, DustID.HeatRay, dustVel.X, dustVel.Y);
						Main.dust[dust1].noGravity = true;
						Main.dust[dust1].scale = 1.2f;
					}
					if (Main.rand.NextBool(5))
					{
						Vector2 spinningpoint = Projectile.velocity;
						Vector2 dustOffset = spinningpoint.RotatedBy(MathHelper.PiOver2) * ((float)Main.rand.NextDouble() - 0.5f) * (float)Projectile.width;
						int dust2 = Dust.NewDust(Projectile.Center + dustOffset - Vector2.One * 4f, 8, 8, DustID.Smoke, 0f, 0f, 100, default, 1.5f);
						Main.dust[dust2].velocity *= 0.5f;
						Main.dust[dust2].velocity.Y = 0f - Math.Abs(Main.dust[dust2].velocity.Y);
					}
				}
			}
		}
	}
}