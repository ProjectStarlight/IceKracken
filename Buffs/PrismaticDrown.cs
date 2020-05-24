using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace IceKracken.Buffs
{
    class PrismaticDrown : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Prismatic Drown");
            Description.SetDefault("You are drowning in prismatic waters!");
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<IcePlayer>().DoT = 20;
            player.slow = true;
        }
    }
}
