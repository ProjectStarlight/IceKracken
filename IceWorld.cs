using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.World.Generation;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.ModLoader.IO;
using StructureHelper;
using Terraria.DataStructures;

namespace IceKracken
{
    class IceWorld : ModWorld
    {
        public static Rectangle IceZone = new Rectangle();
        public static bool BossOpen;
        public static bool BossDowned;
        #region Worldgen
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
            int DesertIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Micro Biomes"));

            tasks.Insert(DesertIndex + 1, new PassLegacy("Ice Kracken Arena Gen", ArenaGen));
        }

        private void ArenaGen(GenerationProgress progress)
        {
            progress.Message = "Unleashing the Kracken";
            int left = FindFirstSnow();
            int right = FindFirstSnow(true);
            int width = right - left;

            int center = left + width / 2;

            int bottom = FindIceDepth(center);

            IceZone = new Rectangle(center - 40, bottom - 100, 80, 200);
            StructureHelper.StructureHelper.GenerateStructure("SquidTemple", new Point16(center - 40, bottom - 150), mod);
        }
        private int FindIceDepth(int start)
        {
            for (int x = start; x < start + 20; x++)
            {
                for (int y = Main.maxTilesY; y > 0; y--)
                {
                    if (WorldGen.InWorld(x, y))
                        if (Main.tile[x, y].type == TileID.IceBlock) return y;
                }
            }
            return 0;
        }
        private int FindFirstSnow(bool startFromRight = false)
        {
            if (startFromRight)
            {
                for (int x = Main.maxTilesX; x > 0; x--)
                {
                    for (int y = 0; y < Main.worldSurface; y++)
                    {
                        if(WorldGen.InWorld(x, y))
                        if (Main.tile[x, y].type == TileID.SnowBlock) return x;
                    }
                }
            }
            else
            {
                for(int x = 0; x < Main.maxTilesX; x++)
                {
                    for(int y = 0; y < Main.worldSurface; y++)
                    {
                        if (WorldGen.InWorld(x, y))
                        if (Main.tile[x, y].type == TileID.SnowBlock) return x;
                    }
                }
            }
            return 0;
        }
        #endregion
        public override void PreUpdate()
        {
            if(!Main.npc.Any(n => n.active && n.type == ModContent.NPCType<Boss.ArenaActor>()))
            {
                NPC.NewNPC(IceZone.Center.X * 16 + 232, IceZone.Center.Y * 16 - 64, ModContent.NPCType<Boss.ArenaActor>());
            }
        }

        public override TagCompound Save()
        {
            return new TagCompound()
            {
                ["Left"] = IceZone.X,
                ["Top"] = IceZone.Y,
                ["Width"] = IceZone.Width,
                ["Height"] = IceZone.Height,
                ["Open"] = BossOpen,
                ["Downed"] = BossDowned
            };
        }
        public override void Load(TagCompound tag)
        {
            IceZone = new Rectangle(tag.GetInt("Left"), tag.GetInt("Top"), tag.GetInt("Width"), tag.GetInt("Height"));
            BossOpen = tag.GetBool("Open");
            BossDowned = tag.GetBool("Downed");
        }
    }
}
