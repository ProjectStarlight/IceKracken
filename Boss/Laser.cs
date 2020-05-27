using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using IceKracken.BlockMechanic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;

namespace IceKracken.Boss
{
    class Laser : InteractiveProjectile, IUnderwater
    {
        public override void SetDefaults()
        {
            projectile.width = 60;
            projectile.height = 1;
            projectile.damage = 50;
            projectile.hostile = true;
            projectile.timeLeft = 600;
            projectile.aiStyle = -1;
        }
        public override bool OnTileCollide(Vector2 oldVelocity) => false;
        public override void AI()
        {
            if(projectile.timeLeft == 599)
            {
                int y = (int)projectile.Center.Y / 16 - 34;
                for(int k = 0; k < 58; k++)
                {
                    int x = (int)projectile.Center.X / 16 + 22 + k;
                    ValidPoints.Add(new Point16(x, y));
                }
            }
            projectile.ai[1]++;
            projectile.Center = Main.npc.FirstOrDefault(n => n.modNPC is MainBody).Center;

            //collision
            for (int k = 0; k < 100; k++)
            {
                Vector2 pos = projectile.position + new Vector2(0, -10 * k);
                Rectangle rect = new Rectangle((int)projectile.position.X, (int)projectile.position.Y - k * 10, 60, 10);

                if (Main.tile[(int)pos.X / 16 + 2, (int)pos.Y / 16].active() || Main.tile[(int)pos.X / 16 - 2, (int)pos.Y / 16].active())
                {
                    break;
                }
                foreach (Player player in Main.player.Where(n => n.active && n.Hitbox.Intersects(rect))) player.Hurt(PlayerDeathReason.ByCustomReason(player.name + " got lasered to death by a squid..."), 50, 0);
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor) => false;
        public void DrawUnderWater(SpriteBatch spriteBatch)
        {
            Texture2D tex = ModContent.GetTexture(Texture);
            for(int k = 0; k < 100; k++)
            {
                float sin = 1 + (float)Math.Sin(projectile.ai[1] / 10f + k);
                float cos = 1 + (float)Math.Cos(projectile.ai[1] / 10f + k);
                Color color = new Color(0.5f + cos * 0.2f, 0.8f, 0.5f + sin * 0.2f) * 1.2f;

                Vector2 pos = projectile.position + new Vector2(0, -10 * k);
                if (Main.tile[(int)pos.X / 16 + 2, (int)pos.Y / 16 + 2].active() || Main.tile[(int)pos.X / 16 - 2, (int)pos.Y / 16 + 2].active())
                {
                    for(int n = 0; n < 20; n++)
                    {
                        Dust d = Dust.NewDustPerfect(pos + new Vector2(Main.rand.Next(0, 60), 32), 264, Vector2.One.RotatedByRandom(6.28f) * Main.rand.NextFloat(3), 0, color, 2.2f);
                        d.noGravity = true;
                        d.rotation = Main.rand.NextFloat(6.28f);
                    }
                    break;
                }

                spriteBatch.Draw(tex, pos - Main.screenPosition, color);
                if(k % 15 == 0) Lighting.AddLight(pos, color.ToVector3());
            }
        }
    }
}
