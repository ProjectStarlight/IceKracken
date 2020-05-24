using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace IceKracken.Boss
{
    class IcePlatform : MovingPlatform
    {
        public override void SafeSetDefaults()
        {
            npc.width = 200;
            npc.height = 32;
        }
        public override void SafeAI()
        {
            if (npc.ai[2] == 0) npc.ai[2] = npc.position.Y;
            if(Main.npc.Any(n => n.active && n.type == ModContent.NPCType<ArenaActor>()))
            {
                ArenaActor actor = Main.npc.FirstOrDefault(n => n.active && n.type == ModContent.NPCType<ArenaActor>()).modNPC as ArenaActor;

                if (npc.position.Y >= npc.ai[2])
                {
                    npc.ai[1] = 0;
                }

                if (npc.position.Y + 16 >= actor.WaterLevel)
                {
                    npc.ai[1] = 1;
                }

                if (npc.ai[1] == 1)
                {
                    npc.position.Y = actor.WaterLevel - 16;
                }
            }
        }
    }

    class IcePlatformSmall : MovingPlatform
    {
        public override void SafeSetDefaults()
        {
            npc.width = 100;
            npc.height = 16;
        }
        public override void SafeAI()
        {
            if(npc.ai[0] == 0) npc.ai[0] = npc.position.Y;

            if (Main.player.Any(player => player.active && player.Hitbox.Intersects(npc.Hitbox)))
            {
                if(npc.velocity.Y < 6)
                npc.velocity.Y += 0.1f;
            }
            else if (npc.position.Y > npc.ai[0]) npc.velocity.Y = -1;
            else npc.velocity.Y = 0;
        }
    }
}
