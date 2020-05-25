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
    public partial class MainBody : ModNPC , IUnderwater
    {
        public List<NPC> Tentacles = new List<NPC>(); //the tentacle NPCs which this boss controls
        public List<NPC> Platforms = new List<NPC>(); //the big platforms the boss' arena has
        Vector2 Spawn;

        #region TML hooks
        public override string Texture => "IceKracken/Invisible";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("[PH] Frost Kracken");
        }
        public override void SetDefaults()
        {
            npc.lifeMax = 3500;
            npc.width = 80;
            npc.height = 80;
            npc.boss = true;
            npc.damage = 0;
            npc.noGravity = true;
            npc.aiStyle = -1;
            npc.npcSlots = 100;
            music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/SquidBoss");
            npc.noTileCollide = true;
            npc.knockBackResist = 0;
            npc.immortal = true;
        }
        public override bool CheckActive() => false;
        public void DrawUnderWater(SpriteBatch spriteBatch)
        {
            for (int k = 3; k > 0; k--)
            {
                Texture2D tex2 = ModContent.GetTexture("IceKracken/Boss/BodyRing");
                Vector2 pos = npc.Center + new Vector2(0, 70 + k * 35).RotatedBy(npc.rotation) - Main.screenPosition;
                int squish = k * 10 + (int)(Math.Sin(npc.ai[1] / 10f - k / 4f * 6.28f) * 20);
                Rectangle rect = new Rectangle((int)pos.X, (int)pos.Y, tex2.Width + (3 - k) * 20 - squish, tex2.Height + (int)(squish * 0.4f) + (3 - k) * 5);

                float sin = 1 + (float)Math.Sin(npc.ai[1] / 10f - k);
                float cos = 1 + (float)Math.Cos(npc.ai[1] / 10f + k);
                Color color = new Color(0.5f + cos * 0.2f, 0.8f, 0.5f + sin * 0.2f) * 0.7f;

                spriteBatch.Draw(tex2, rect, tex2.Frame(), color, 0, tex2.Size() / 2, 0, 0);
            }

            Texture2D tex = ModContent.GetTexture("IceKracken/Boss/BodyUnder");
            spriteBatch.Draw(tex, npc.Center - Main.screenPosition, tex.Frame(), Color.White, npc.rotation, tex.Size() / 2, 1, 0, 0);
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor) => false;
        #endregion

        #region AI
        public enum AIStates
        {
            SpawnEffects = 0,
            SpawnAnimation = 1,
            FirstPhase = 2,
            FirstPhaseTwo = 3,
            SecondPhase = 4
        }
        public override void AI()
        {
            npc.ai[1]++;
            /*AI fields:
             * 0: phase
             * 1: timer
             * 2: attack phase
             * 3: attack timer
             */
            if (npc.ai[0] == (int)AIStates.SpawnEffects)
            {
                npc.ai[0] = (int)AIStates.SpawnAnimation;

                foreach (NPC npc in Main.npc.Where(n => n.active && n.modNPC is IcePlatform)) Platforms.Add(npc);

                Spawn = npc.Center;

                (mod as IceKracken).introText.Display("I'm Bad at Names", "Aurora Calamari", 600);
                Main.LocalPlayer.GetModPlayer<IcePlayer>().ScreenMoveTarget = npc.Center;
                Main.LocalPlayer.GetModPlayer<IcePlayer>().ScreenMovePan = npc.Center + new Vector2(0, -600);
                Main.LocalPlayer.GetModPlayer<IcePlayer>().ScreenMoveTime = 600;

                IceKracken.Instance.lifeBar.TrackingNPC = npc;
            }
            if (npc.ai[0] == (int)AIStates.SpawnAnimation)
            {
                if (npc.ai[1] < 200)
                {
                    npc.Center = Vector2.SmoothStep(Spawn, Spawn + new Vector2(0, -600), npc.ai[1] / 200f);
                }

                for (int k = 0; k < 4; k++)
                {
                    if (npc.ai[1] == 200 + k * 50)
                    {
                        int x;
                        int y;
                        int xb;
                        switch (k) //I handle these manually to get them to line up with the window correctly
                        {
                            case 0: x = -370; y = 0; xb = -50; break;
                            case 1: x = -420; y = -100; xb = -20; break;
                            case 3: x = 370; y = 0; xb = 50; break;
                            case 2: x = 420; y = -100; xb = 20; break;
                            default: x = 0; y = 0; xb = 0; break;
                        }
                        int i = NPC.NewNPC((int)npc.Center.X + x, (int)npc.Center.Y + 550, ModContent.NPCType<Tentacle>(), 0, k == 1 || k == 2 ? 1 : 0); //middle 2 tentacles should be vulnerable
                        (Main.npc[i].modNPC as Tentacle).Parent = this;
                        (Main.npc[i].modNPC as Tentacle).MovePoint = new Vector2((int)npc.Center.X + x, (int)npc.Center.Y - y);
                        (Main.npc[i].modNPC as Tentacle).OffBody = xb;
                        Tentacles.Add(Main.npc[i]);
                    }
                }

                if (npc.ai[1] > 600)
                {
                    foreach (NPC tentacle in Tentacles)
                    {
                        Tentacle mt = tentacle.modNPC as Tentacle;
                        tentacle.Center = Vector2.SmoothStep(mt.MovePoint, mt.SavedPoint, (npc.ai[1] - 600) / 100f);
                    }
                }
                if (npc.ai[1] > 700) npc.ai[0] = (int)AIStates.FirstPhase;
            }

            if (npc.ai[0] == (int)AIStates.FirstPhase) //first phase, part 1. Tentacle attacks and ink.
            {
                npc.ai[3]++;
                if (npc.ai[3] == 1)
                {
                    if (Tentacles.Count(n => n.ai[0] == 2) == 2) //phasing logic
                    {
                        npc.ai[0] = (int)AIStates.FirstPhaseTwo;
                        npc.ai[1] = 0;
                    }
                    else //else advance the attack pattern
                    {
                        npc.ai[2]++;
                        if (npc.ai[2] > 3)
                        {
                            npc.ai[2] = 1;
                        }
                    }
                }
                switch (npc.ai[2])
                {
                    case 0: //cycle attack
                        break;
                    case 1: //tentacle attack
                        TentacleSpike();
                        break;
                    case 2: //ink burst
                        InkBurst();
                        break;
                    case 3: //platform attack
                        PlatformSweep();
                        break;
                }
            }

            if (npc.ai[0] == (int)AIStates.FirstPhaseTwo) //first phase, part 2. Tentacle attacks and ink. Raise water first.
            {
                if (npc.ai[1] < 325)
                {
                    Main.npc.FirstOrDefault(n => n.active && n.modNPC is ArenaActor).ai[0]++;
                    npc.Center = Vector2.SmoothStep(Spawn + new Vector2(0, -600), Spawn + new Vector2(0, -750), npc.ai[1] / 325f);
                }
                if(npc.ai[1] == 325)
                {
                    foreach (NPC tentacle in Tentacles.Where(n => n.ai[0] == 1)) tentacle.ai[0] = 0; //make the remaining tentacles vulnerable
                }
                if(npc.ai[1] > 325) //continue attacking otherwise
                {
                    npc.ai[3]++;
                    if (npc.ai[3] == 1)
                    {
                        if (Tentacles.Count(n => n.ai[0] == 2) == 4) //phasing logic
                        {
                            npc.ai[0] = (int)AIStates.SecondPhase;
                            npc.ai[1] = 0;
                        }
                        else //else advance the attack pattern
                        {
                            npc.ai[2]++;
                            if (npc.ai[2] > 3)
                            {
                                npc.ai[2] = 1;
                            }
                        }
                    }
                    switch (npc.ai[2])
                    {
                        case 0: //cycle attack
                            break;
                        case 1: //tentacle attack
                            TentacleSpike();
                            break;
                        case 2: //ink burst
                            InkBurst();
                            break;
                        case 3: //platform attack
                            PlatformSweep();
                            break;
                    }
                }
            }
            if (npc.ai[0] == (int)AIStates.SecondPhase) //second phase
            {
                if (npc.ai[1] < 300)
                {
                    Main.npc.FirstOrDefault(n => n.active && n.modNPC is ArenaActor).ai[0]++;
                }
            }
            #endregion
        }
    }
}
