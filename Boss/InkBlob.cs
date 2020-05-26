using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace IceKracken.Boss
{
    class InkBlob : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 80;
            projectile.height = 80;
            projectile.aiStyle = -1;
            projectile.timeLeft = 300;
            projectile.hostile = true;
            projectile.damage = 15;
        }
        public override void AI()
        {
            projectile.velocity.Y += 0.11f;
            projectile.scale -= 1 / 400f;

            projectile.ai[1] += 0.1f;
            projectile.rotation += Main.rand.NextFloat(0.2f);

            float sin = 1 + (float)Math.Sin(projectile.ai[1]);
            float cos = 1 + (float)Math.Cos(projectile.ai[1]);
            Color color = new Color(0.5f + cos * 0.2f, 0.8f, 0.5f + sin * 0.2f);
            Lighting.AddLight(projectile.Center, color.ToVector3());
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = ModContent.GetTexture(Texture);

            float sin = 1 + (float)Math.Sin(projectile.ai[1]);
            float cos = 1 + (float)Math.Cos(projectile.ai[1]);
            Color color = new Color(0.5f + cos * 0.2f, 0.8f, 0.5f + sin * 0.2f);

            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, tex.Frame(), color, projectile.rotation, tex.Size() / 2, projectile.scale, 0, 0);
            return false;
        }
    }
    class SpewBlob : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.aiStyle = -1;
            projectile.timeLeft = 300;
            projectile.hostile = true;
            projectile.damage = 15;
        }
        public override void AI()
        {
            projectile.velocity.Y -= 0.14f;
            projectile.velocity.X *= 0.9f;

            projectile.ai[1] += 0.1f;
            projectile.rotation = projectile.velocity.ToRotation() + 1.57f;

            float sin = 1 + (float)Math.Sin(projectile.ai[1]);
            float cos = 1 + (float)Math.Cos(projectile.ai[1]);
            Color color = new Color(0.5f + cos * 0.2f, 0.8f, 0.5f + sin * 0.2f);
            Lighting.AddLight(projectile.Center, color.ToVector3() * 0.6f);

            Dust d = Dust.NewDustPerfect(projectile.Center, 264, -projectile.velocity * 0.5f, 0, color, 1.4f);
            d.noGravity = true;
            d.rotation = Main.rand.NextFloat(6.28f);
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = ModContent.GetTexture(Texture);

            float sin = 1 + (float)Math.Sin(projectile.ai[1]);
            float cos = 1 + (float)Math.Cos(projectile.ai[1]);
            Color color = new Color(0.5f + cos * 0.2f, 0.8f, 0.5f + sin * 0.2f);

            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, tex.Frame(), color, projectile.rotation, tex.Size() / 2, projectile.scale, 0, 0);
            return false;
        }
    }
}
