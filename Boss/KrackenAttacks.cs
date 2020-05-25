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
            npc.target = possible[Main.rand.Next(possible.Count - 1)];
        }
        private void ResetAttack()
        {
            npc.ai[3] = 0;
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
            if (npc.ai[3] == 660) ResetAttack();
        }
        private void InkBurst()
        {

        }
        public void PlatformSweep()
        {

        }
    }
}
