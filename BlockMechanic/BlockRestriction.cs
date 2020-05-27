/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.DataStructures;

namespace IceKracken.BlockMechanic
{
    class BlockRestriction : GlobalItem
    {
        public override bool CanUseItem(Item item, Player player)
        {
            Tile tile = Main.tile[Player.tileTargetX, Player.tileTargetY];
            if(item.createTile != -1 && tile.wall == ModContent.WallType<BrickWall>() && 
                !Main.projectile.Any(n => n.active && n.timeLeft > 10 && n.modProjectile is InteractiveProjectile && (n.modProjectile as InteractiveProjectile).ValidPoints.Contains( new Point16(Player.tileTargetX, Player.tileTargetY))))
            {
                return false;
            }
            return base.CanUseItem(item, player);
        }
    }
}*/
