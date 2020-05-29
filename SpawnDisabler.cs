using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace IceKracken
{
    class SpawnDisabler : GlobalNPC
    {
        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            if (WorldGen.InWorld((int)player.Center.X / 16, (int)player.Center.Y / 16) && Main.tile[(int)player.Center.X / 16, (int)player.Center.Y / 16].wall == ModContent.WallType<BrickWall>())
            {
                spawnRate = 9000;
            }
            else base.EditSpawnRate(player, ref spawnRate, ref maxSpawns);
        }
    }
}
