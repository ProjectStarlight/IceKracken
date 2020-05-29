using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace IceKracken.Items
{
    class TentacleSpear : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ten-tickler");
            Tooltip.SetDefault("Poke!");
        }
        public override void SetDefaults()
        {
            item.width = 32;
            item.height = 32;
            item.channel = true;
            item.melee = true;
            item.rare = 2;
            item.damage = 6;
            item.useTime = 15;
            item.useAnimation = 15;
            item.shoot = ModContent.ProjectileType<SpearProj>();
            item.shootSpeed = 4;
            item.useStyle = 1;
            item.noUseGraphic = true;
            item.UseSound = Terraria.ID.SoundID.Item81;
        }
        public override bool UseItem(Player player)
        {
            player.frozen = true;
            return true;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            for(int k = 0; k < 3; k++)
            {
                int i = Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<SpearProj>(), item.damage, item.knockBack, player.whoAmI, 0, 1 + k);
                Main.projectile[i].scale = 1 - (k + 1) * 0.1f;
            }
            return true;
        }
        public override bool CanUseItem(Player player) => !Main.projectile.Any(n => n.active && n.owner == player.whoAmI && n.modProjectile is SpearProj);
    }

    class SpearProj : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }
        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            projectile.ai[0]++;
            if (projectile.ai[0] % 26 == 0) Main.PlaySound(Terraria.ID.SoundID.Drown, projectile.Center);
            projectile.position += player.velocity;
            Dust.NewDustPerfect(projectile.Center, 33, Vector2.One.RotatedByRandom(6.28f) * 0.5f);
            if (player.channel && projectile.ai[0] < 300)
            {
                projectile.velocity += Vector2.Normalize(projectile.Center - Main.MouseWorld).RotatedBy(projectile.ai[1] * 0.3f + 1.2f * (float)Math.Sin(projectile.ai[1] + projectile.ai[0] / 10f)) * -2f;
                if (projectile.velocity.Length() > 8) projectile.velocity = Vector2.Normalize(projectile.velocity) * 8;
                if(Vector2.Distance(projectile.Center, player.Center) > 300)
                {
                    projectile.Center = player.Center + new Vector2(0, -300).RotatedBy(projectile.rotation);
                }
            }

            else
            {
                if(projectile.ai[0] < 300) projectile.ai[0] = 300;
                projectile.velocity *= 0;
                projectile.Center += Vector2.Normalize(projectile.Center - player.Center) * -8f;
            }

            if (projectile.ai[0] > 330) projectile.Kill();
            projectile.rotation = (projectile.Center - player.Center).ToRotation() + 1.57f;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Player player = Main.player[projectile.owner];
            Texture2D tex = ModContent.GetTexture("IceKracken/Boss/TentacleBody");
            float kMax = Vector2.Distance(projectile.Center, player.Center) / 10;
            for (int k = 0; k < Vector2.Distance(projectile.Center, player.Center) / 10; k++)
            {
                spriteBatch.Draw(tex, Vector2.Lerp(projectile.Center, player.Center, k / kMax) - Main.screenPosition + Vector2.One.RotatedBy(projectile.ai[0] / 50f + k / 20f), tex.Frame(), lightColor, projectile.rotation, tex.Size() / 2, 0.7f * projectile.scale, 0, 0);
            }
            return true;
        }
    }
}
