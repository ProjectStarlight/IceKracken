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
    public class Tentacle : ModNPC, IUnderwater
    {
        public override string Texture => "IceKracken/Invisible";
        public MainBody Parent { get; set; }
        public Vector2 MovePoint;
        public Vector2 SavedPoint;
        public int OffBody;
        public override void SetDefaults()
        {
            npc.width = 60;
            npc.height = 80;
            npc.lifeMax = 250;
            npc.damage = 20;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.knockBackResist = 0f;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor) => false;
        public enum TentacleStates
        {
            SpawnAnimation = 0,
            FirstPhase = 1
        }
        public override bool CheckActive() => false;
        public void DrawUnderWater(SpriteBatch spriteBatch)
        {
            if (Parent != null)
            {
                Texture2D top = ModContent.GetTexture("IceKracken/Boss/TentacleTop");
                Texture2D body = ModContent.GetTexture("IceKracken/Boss/TentacleBody");
                Texture2D ring = ModContent.GetTexture("IceKracken/Boss/TentacleRing");

                float dist = npc.Center.X - Parent.npc.Center.X;


                int underMax = 0;
                underMax = (int)(npc.ai[1] / 60 * 40);
                if (underMax > 40) underMax = 40;

                if (Parent.npc.ai[0] != (int)MainBody.AIStates.SpawnAnimation)
                {
                    NPC actor = Main.npc.FirstOrDefault(n => n.active && n.modNPC is ArenaActor);
                    underMax = (int)(actor.Center.Y - Parent.npc.Center.Y / 10f);
                }
                for (int k = 0; k < underMax; k++)
                {
                    Vector2 pos = Parent.npc.Center + new Vector2(OffBody - 9 + (float)Math.Sin(npc.ai[1] / 20f + k) * 2, 100 + k * 10);
                    spriteBatch.Draw(body, pos - Main.screenPosition, Lighting.GetColor((int)pos.X / 16, (int)pos.Y / 16));
                }


                if (npc.ai[1] > 60)
                {
                    spriteBatch.Draw(top, npc.Center - Main.screenPosition, top.Frame(), Lighting.GetColor((int)npc.Center.X / 16, (int)npc.Center.Y / 16), 0, top.Size() / 2, 1, 0, 0);

                    for (int k = 0; k < Vector2.Distance(npc.Center + new Vector2(0, npc.height / 2), SavedPoint) / 10f; k++)
                    {
                        Vector2 pos = new Vector2((float)Math.Sin(npc.ai[1] / 20f + k) * 4, 0) + Vector2.Lerp(npc.Center + new Vector2(0, npc.height / 2), SavedPoint, k / Vector2.Distance(npc.Center + new Vector2(0, npc.height / 2), SavedPoint) * 10f);
                        spriteBatch.Draw(body, pos - Main.screenPosition, body.Frame(), Lighting.GetColor((int)pos.X / 16, (int)pos.Y / 16), 0, body.Size() / 2, 1, 0, 0);
                    }

                    Color color;
                    switch (npc.ai[0])
                    {
                        case 1: color = Color.LightPink; break;
                        case 0: color = Color.LimeGreen; break;
                        case 2: color = Color.Red; break;
                        default: color = Color.Black; break;
                    }

                    int squish = (int)(Math.Sin(npc.ai[1] * 0.1f) * 5);
                    Rectangle rect = new Rectangle((int)(npc.Center.X - Main.screenPosition.X), (int)(npc.Center.Y - Main.screenPosition.Y) + 40, 34 - squish, 16 + (int)(squish * 0.4f));
                    spriteBatch.Draw(ring, rect, ring.Frame(), color * 0.6f, 0, ring.Size() / 2, 0, 0);
                }
            }
        }

        public override bool CheckDead()
        {
            npc.life = 1;
            npc.ai[0] = 2;
            return false;
        }
        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (npc.life < damage) damage = npc.life;
            Parent.npc.life -= damage;
        }
        public override void ModifyHitByItem(Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            if (npc.life < damage) damage = npc.life;
            Parent.npc.life -= damage;
        }
        public override void AI()
        {
            /* AI fields:
             * 0: state
             * 1: timer
             * 2: attack
             * 3: attack timer
             */
            npc.dontTakeDamage = npc.ai[0] != 0;

            if ((npc.ai[0] == 0 || npc.ai[0] == 1) && npc.ai[1] == 0) SavedPoint = npc.Center;
            npc.ai[1]++;


            if (npc.ai[1] >= 60 && npc.ai[1] < 120) //spawning animation logic
            {
                npc.Center = Vector2.SmoothStep(SavedPoint, MovePoint, (npc.ai[1] - 60) / 60f);
            }

            if (Parent == null || !Parent.npc.active)
            {
                npc.active = false;
            }

        }
    }
}
