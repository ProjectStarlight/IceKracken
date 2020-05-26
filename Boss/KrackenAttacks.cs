using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IceKracken.Boss
{
    public partial class MainBody : ModNPC
    {
        private void RandomizeTarget()
        {
            List<int> possible = new List<int>();
            foreach(Player player in Main.player.Where(n => Vector2.Distance(n.Center, npc.Center) < 1500))
            {
                possible.Add(player.whoAmI);
            }
            if (possible.Count == 0) npc.active = false;
            npc.target = possible[Main.rand.Next(possible.Count - 1)];
        }
        private void ResetAttack()
        {
            npc.ai[3] = 0;
        }
        private void ShufflePlatforms()
        {
            int n = Platforms.Count(); //fisher yates
            while (n > 1)
            {
                n--;
                int k = Main.rand.Next(n + 1);
                NPC value = Platforms[k];
                Platforms[k] = Platforms[n];
                Platforms[n] = value;
            }
        }
        private void TentacleSpike()
        {
            RandomizeTarget();
            for(int k = 0; k < 4; k++)
            {
                Tentacle tentacle = Tentacles[k].modNPC as Tentacle;
                if (npc.ai[3] == k * 100 || (k == 0 && npc.ai[3] == 1)) //teleport where needed
                {
                    int adj = (int)Main.player[npc.target].velocity.X * 60; if (adj > 200) adj = 200;
                    Tentacles[k].Center = new Vector2(Main.player[npc.target].Center.X + adj, Tentacles[k].Center.Y);
                    tentacle.SavedPoint = Tentacles[k].Center;
                    tentacle.MovePoint = Tentacles[k].Center + new Vector2(0, -1000);

                    for(int n = 0; n < 50; n++)
                    {
                        Dust.NewDustPerfect(Tentacles[k].Center + new Vector2(0, -n * 25 + Main.rand.NextFloat(5)), DustID.Fireworks, Vector2.Zero, 0, default, 0.5f);
                    }
                }
                if (npc.ai[3] > k * 100 + 30 && npc.ai[3] < k * 100 + 90) //shooting up, first 30 frames are for tell
                {
                    int time = (int)npc.ai[3] - (k * 100 + 30);
                    Tentacles[k].Center = Vector2.SmoothStep(tentacle.SavedPoint, tentacle.MovePoint, time / 60f);
                    Tentacles[k].ai[1] += 5f; //make it squirm faster
                }
                if (npc.ai[3] > k * 100 + 90 && npc.ai[3] < k * 100 + 300) //retracting
                {
                    int time = (int)npc.ai[3] - (k * 100 + 90);
                    Tentacles[k].Center = Vector2.SmoothStep(tentacle.MovePoint, tentacle.SavedPoint, time / 210f);
                }
            }
            if (npc.ai[3] == 600) ResetAttack();
        }
        private void InkBurst()
        {
            for (float k = 0; k <= 3.14f; k += 3.14f / 5f)
            {
                if(npc.ai[3] % 3 == 0) Projectile.NewProjectile(npc.Center + new Vector2(0, 100), new Vector2(-10, 0).RotatedBy(k), ModContent.ProjectileType<InkBlob>(), 10, 0.2f, 255, 0, Main.rand.NextFloat(6.28f));
                if(npc.ai[3] == 60) ResetAttack();
            }
        }
        private void PlatformSweep()
        {
            if (npc.ai[3] == 1) //start by randomizing the platform order and assigning targets
            {
                ShufflePlatforms();
                for (int k = 0; k < 4; k++)
                {
                    Tentacle tentacle = Tentacles[k].modNPC as Tentacle;
                    Tentacles[k].Center = new Vector2(Platforms[k].Center.X, Tentacles[k].Center.Y);
                    tentacle.SavedPoint = Tentacles[k].Center;
                    tentacle.MovePoint = Platforms[k].Center + new Vector2(0, -70);
                }
            }
            if (npc.ai[3] > 60 && npc.ai[3] < 120) //rising
            {
                for (int k = 0; k < 4; k++)
                {
                    Tentacle tentacle = Tentacles[k].modNPC as Tentacle;
                    Tentacles[k].Center = Vector2.SmoothStep(tentacle.SavedPoint, tentacle.MovePoint, (npc.ai[3] - 60) / 60f);
                }
            }
            if(npc.ai[3] > 120 && npc.ai[3] < 360) //waving around
            {
                for(int k = 0; k < 4; k++)
                {
                    Tentacles[k].position.X += (float)Math.Sin(npc.ai[3] / 10f + k) * 2;
                    Tentacles[k].position.Y += (float)Math.Cos(npc.ai[3] / 10f + k) * 4;
                }
            }
            if (npc.ai[3] > 360 && npc.ai[3] < 420) //going back
            {
                for (int k = 0; k < 4; k++)
                {
                    Tentacle tentacle = Tentacles[k].modNPC as Tentacle;
                    Tentacles[k].Center = Vector2.SmoothStep(tentacle.MovePoint, tentacle.SavedPoint, (npc.ai[3] - 360) / 60f);
                }
            }
            if (npc.ai[3] == 420) ResetAttack();

        }
        private void Spew()
        {
            if (npc.ai[3] % 100 == 0)
            {
                for (int k = 0; k < 10; k++)
                {
                    Projectile.NewProjectile(npc.Center + new Vector2(0, 100), new Vector2(-100 + k * 20, 0), ModContent.ProjectileType<SpewBlob>(), 10, 0.2f);
                }
            }
            if (npc.ai[3] == 300) ResetAttack();
        }
        private void Laser()
        {
            if(npc.ai[3] == 1)
            {
                SavedPoint = npc.Center;
                npc.velocity *= 0;
                npc.rotation = 0;
            }
            if(npc.ai[3] < 60)
            {
                npc.Center = Vector2.SmoothStep(SavedPoint, Spawn + new Vector2(-800, -500), npc.ai[3] / 60f);
                npc.rotation += 3.14f / 59f;
            }

            if(npc.ai[3] == 60) SavedPoint = npc.Center; //leftmost point of laser

            if (npc.ai[3] > 60 && npc.ai[3] < 660) //lasering
            {
                npc.Center = Vector2.Lerp(SavedPoint, Spawn + new Vector2(800, -500), (npc.ai[3] - 60) / 600f);
                Projectile.NewProjectile(npc.Center + new Vector2(0, -200), new Vector2(2.6f, -50), ModContent.ProjectileType<InkBlob>(), 10, 0.2f, 255, 0, npc.ai[3] * 0.1f);
            }

            if(npc.ai[3] == 660) SavedPoint = npc.Center; //end of laser

            if (npc.ai[3] > 660 && npc.ai[3] < 720)
            {
                npc.Center = Vector2.SmoothStep(SavedPoint, Spawn + new Vector2(0, -300), (npc.ai[3] - 660) / 60f);
                npc.rotation -= 3.14f / 59f;
            }
            if (npc.ai[3] >= 720) ResetAttack();
        }
        private void Leap()
        {
            if (npc.ai[3] == 1)
            {
                SavedPoint = npc.Center;
                npc.velocity *= 0;
                npc.rotation = 0;

                for (int k = 0; k < 4; k++) //tentacles
                {
                    Tentacle tentacle = Tentacles[k].modNPC as Tentacle;
                    int off;
                    switch (k)
                    {
                        case 0: off = -430; break;
                        case 1: off = -150; break;
                        case 2: off = 150; break;
                        case 3: off = 430; break;
                        default: off = 0; break;
                    }
                    Tentacles[k].Center = new Vector2(Spawn.X + off, Tentacles[k].Center.Y);
                    tentacle.SavedPoint = Tentacles[k].Center;
                    tentacle.MovePoint = Tentacles[k].Center + new Vector2(off * 0.45f, -900);
                    for(int n = 0; n < 40; n++)
                    {
                        Dust.NewDustPerfect(Vector2.Lerp(tentacle.SavedPoint, tentacle.MovePoint, n / 30f), DustID.Fireworks, Vector2.Zero);
                    }
                }
            }
            if (npc.ai[3] < 120)
            {
                npc.Center = Vector2.SmoothStep(SavedPoint, Spawn + new Vector2(0, -500), npc.ai[3] / 120f);

                for(int k = 0; k < 4; k++) //tentacles
                {
                    Tentacle tentacle = Tentacles[k].modNPC as Tentacle;
                    Tentacles[k].Center = Vector2.SmoothStep(tentacle.SavedPoint, tentacle.MovePoint, npc.ai[3] / 120f);
                }
            }

            if (npc.ai[3] == 120) npc.velocity.Y = -15;

            if (npc.ai[3] == 150)
            {
                for (float k = 0; k <= 3.14f; k += 3.14f / 4f)
                {
                    Projectile.NewProjectile(npc.Center + new Vector2(0, 100), new Vector2(-10, 0).RotatedBy(k), ModContent.ProjectileType<InkBlob>(), 10, 0.2f, 255, 0, Main.rand.NextFloat(6.28f));
                }
            }

            if (npc.ai[3] > 120 && npc.ai[3] < 220)
            {
                npc.velocity.Y += 0.16f;
            }
            if(npc.ai[3] > 120)
            {
                for (int k = 0; k < 4; k++) //tentacles
                {
                    Tentacle tentacle = Tentacles[k].modNPC as Tentacle;
                    Tentacles[k].Center =  new Vector2(Tentacles[k].Center.X + (float)Math.Sin(npc.ai[3] / 10f + k) * 4f, Tentacles[k].Center.Y + (float)Math.Cos(npc.ai[3] / 10f + k) * 2f);
                }
            }

            if(npc.ai[3] > 540)
            {
                for (int k = 0; k < 4; k++) //tentacles
                {
                    Tentacle tentacle = Tentacles[k].modNPC as Tentacle;
                    Tentacles[k].Center = Vector2.SmoothStep(tentacle.MovePoint, tentacle.SavedPoint, (npc.ai[3] - 540) / 60f);
                }
            }
            if (npc.ai[3] == 600) ResetAttack();
        }
    }
}
