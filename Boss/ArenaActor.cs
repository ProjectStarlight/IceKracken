using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics;
using Terraria.ModLoader;

namespace IceKracken.Boss
{
    class ArenaActor : ModNPC
    {
        int whitelistID = ModContent.WallType<BrickWall>();
        public override string Texture => "IceKracken/Invisible";
        public float WaterLevel { get => npc.Center.Y + 35 * 16 - npc.ai[0]; }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("");
        }
        public override void SetDefaults()
        {
            npc.dontTakeDamage = true;
            npc.dontCountMe = true;
            npc.immortal = true;
            npc.noGravity = true;
            npc.lifeMax = 2;
        }
        public override bool? CanBeHitByItem(Player player, Item item) => false;
        public override bool? CanBeHitByProjectile(Projectile projectile) => false;
        public override bool CheckActive() => false;

        public override void AI()
        {
            /*AI fields:
             * 0: water level
             * 
             * 
             * 
             */

            npc.ai[1] += 0.04f;
            npc.ai[2] += 0.01f;

            if (npc.ai[0] < 150) npc.ai[0] = 150; //water clamping and return logic
            if (!Main.npc.Any(n => n.active && n.modNPC is MainBody) && npc.ai[0] > 150) npc.ai[0]--;

            if (npc.ai[1] > 6.28f) npc.ai[1] = 0;

            if (!Main.npc.Any(n => n.active && n.modNPC is IcePlatform))
            {
                SpawnPlatform(-600, 200);
                SpawnPlatform(600, 200);

                SpawnPlatform(-400, -50);
                SpawnPlatform(400, -50);

                SpawnPlatform(-150, -320);
                SpawnPlatform(150, -320);

                SpawnPlatform(-250, -150, true);
                SpawnPlatform(250, -150, true);
            }

            Vector2 pos = npc.Center + new Vector2(-1600, 35 * 16) + new Vector2(0, -npc.ai[0]);

            for(int k = 0; k < 45; k++)
            {
                Vector2 target = pos + new Vector2(k / 45f * 3200, 0);

                if (Main.tile[(int)target.X / 16, (int)target.Y / 16].wall == whitelistID)
                {
                    float sin = (float)Math.Sin(npc.ai[1] + k);
                    float sin2 = (float)Math.Sin(npc.ai[2] + k * 0.2f);
                    float cos = (float)Math.Cos(npc.ai[2] + k);
                    Lighting.AddLight(target, new Vector3(10 * (1 + sin2), 14 * (1 + cos), 18) * (0.02f + sin * 0.003f));
                }
            }

            for(int k = 0; k < 10; k++)
            {
                Lighting.AddLight(npc.Center + new Vector2(0, -200 + k * 60), new Vector3(1, 1, 1) * 0.4f);
                Lighting.AddLight(npc.Center + new Vector2(-400, -200 + k * 60), new Vector3(1, 1, 1) * 0.2f);
                Lighting.AddLight(npc.Center + new Vector2(400, -200 + k * 60), new Vector3(1, 1, 1) * 0.2f);
            }

            foreach (Player player in Main.player.Where(n => n.Hitbox.Intersects(new Rectangle((int)pos.X, (int)pos.Y, 200 * 16, (int)npc.ai[0]))))
            {
                player.wet = true;
                player.AddBuff(ModContent.BuffType<Buffs.PrismaticDrown>(), 4, false);
            }

            if (npc.ai[0] == 150 && !Main.npc.Any(n => n.active && n.modNPC is MainBody)) //ready to spawn another squid
            {
                if(Main.player.Any(n => n.Hitbox.Intersects(new Rectangle((int)pos.X, (int)pos.Y, 200 * 16, (int)npc.ai[0]))))
                {
                    NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y + 650, ModContent.NPCType<MainBody>());
                }
            }
        }

        public void SpecialDraw(SpriteBatch spriteBatch)
        {
            Vector2 pos = npc.Center + new Vector2(- 1600, 35 * 16) + new Vector2(0, -npc.ai[0]) - Main.screenPosition;

            pos += Main.screenPosition;
            for(int x = (int)pos.X / 16; x < (int)pos.X / 16 + 200; x++)
            {
                for (int y = (int)pos.Y / 16; y <= (int)pos.Y / 16 + (int)npc.ai[0] / 16; y++)
                {
                    if (Main.tile[x, y + 1].active() && Lighting.Brightness(x, y + 1) <= 0.4f)
                    {
                        Color color = Color.Black * (1 - Lighting.Brightness(x, y + 1) * 4);
                        spriteBatch.Draw(Main.blackTileTexture, new Vector2(x, y + 1) * 16 - Main.screenPosition, color);
                    }
                    if (Main.tile[x, y + 1].wall == whitelistID)
                    {
                        Color color = Lighting.GetColor(x, y + 1).MultiplyRGB(new Color(100, 200, 255)) * (0.5f - Lighting.Brightness(x, y + 1) * 0.2f);
                        spriteBatch.Draw(Main.blackTileTexture, new Vector2(x, y + 1) * 16 - Main.screenPosition, color);
                    }
                    if (Main.tile[x, y].wall == whitelistID && y == (int)pos.Y / 16)
                    {
                        Color color = Lighting.GetColor(x, y + 1).MultiplyRGB(new Color(100, 200, 255)) * (0.5f - Lighting.Brightness(x, y + 1) * 0.2f);

                        float offset = npc.ai[0] % 16;
                        if (offset == 0) offset = 16;
                        offset += (float)Math.Sin(npc.ai[1] * (4) + x) * (2);

                        spriteBatch.Draw(Main.blackTileTexture, new Rectangle((int)(x * 16 - Main.screenPosition.X), (int)((y + 1) * 16 - Main.screenPosition.Y - (offset) + 1), 16, (int)(offset)), color);

                        spriteBatch.Draw(Main.magicPixel, new Rectangle((int)(x * 16 - Main.screenPosition.X), (int)((y + 1) * 16 - Main.screenPosition.Y - (offset) + 1), 16, 2), Main.magicPixel.Frame(), Color.White * 0.6f);
                    }
                }
            }
        }
        public void SpecialDraw2(SpriteBatch spriteBatch) //haha meme method name hahaaa
        {
            Color color = new Color(230, 255, 255);

            Rectangle source = new Rectangle(0, 100, 512, 200);
            spriteBatch.Draw(TextureManager.Load("Images/Misc/Perlin"), new Rectangle((int)npc.Center.X - 520 - (int)Main.screenPosition.X, (int)npc.Center.Y - 400 - (int)Main.screenPosition.Y, 1040, 850), source, color * 0.8f);


            Texture2D tex = ModContent.GetTexture("IceKracken/Boss/Window");
            for (int x = 0; x < tex.Width / 16; x++)
            {
                for (int y = 0; y < tex.Height / 16; y++)
                {
                    Vector2 pos = npc.Center - (tex.Size() / 2) + new Vector2(x, y) * 16;
                    spriteBatch.Draw(tex, pos - Main.screenPosition, new Rectangle(x * 16, y * 16, 16, 16), Lighting.GetColor((int)pos.X / 16, (int)pos.Y / 16), 0, Vector2.Zero, 1, 0, 0);
                }
            }
            Texture2D tex2 = ModContent.GetTexture("IceKracken/Boss/WindowIn");
            spriteBatch.Draw(tex2, npc.Center - Main.screenPosition, null, Color.White * 0.4f, 0, tex2.Size() / 2, 1, 0, 0);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.Additive);

            Texture2D tex3 = ModContent.GetTexture("IceKracken/Boss/Godray");

            for (int k = 0; k < 4; k++)
            {
                spriteBatch.Draw(tex3, npc.Center + new Vector2(450, -250) - Main.screenPosition, null, Color.White * 0.5f, 0.9f + (float)Math.Sin(npc.ai[2] * 2 + k) * 0.13f, Vector2.Zero, 1.5f, 0, 0);
                spriteBatch.Draw(tex3, npc.Center + new Vector2(-450, -250) - Main.screenPosition, null, Color.White * 0.5f, 0.45f + (float)Math.Sin(npc.ai[2] * 2 + k) * 0.13f, Vector2.Zero, 1.5f, 0, 0);

                spriteBatch.Draw(tex3, npc.Center + new Vector2(0, -450) - Main.screenPosition, null, Color.White * 0.5f, 0.68f + (float)Math.Sin(npc.ai[2] * 2 + (k / 4f *  6.28f)) * 0.13f, Vector2.Zero, 1.9f, 0, 0);
            }

            spriteBatch.End();
            spriteBatch.Begin();
        }

        private void SpawnPlatform(int x, int y, bool small = false)
        {
            if(small) NPC.NewNPC((int)(npc.Center.X + x), (int)(npc.Center.Y + y), ModContent.NPCType<IcePlatformSmall>());
            else NPC.NewNPC((int)(npc.Center.X + x), (int)(npc.Center.Y + y), ModContent.NPCType<IcePlatform>() );
        }

    }
}
