using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace IceKracken
{
    public class BrickWallPlacer : ModItem
    {
        public override string Texture => "IceKracken/Items/NutKracker";
        public override void SetDefaults()
        {
            item.useStyle = 1;
            item.useTime = 2;
            item.useAnimation = 2;
            item.createWall = ModContent.WallType<BrickWall>();
            item.autoReuse = true;
        }
    }
    class BrickWall : ModWall
    {
        public override void SetDefaults()
        {
            Main.wallHouse[Type] = false;
            AddMapEntry(new Color(65, 70, 75));
        }
    }

    public class BrickPlacer : ModItem
    {
        public override string Texture => "IceKracken/Items/NutKracker";
        public override void SetDefaults()
        {
            item.useStyle = 1;
            item.useTime = 2;
            item.useAnimation = 2;
            item.createTile = ModContent.TileType<Brick>();
            item.autoReuse = true;
        }
    }
    class Brick : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            minPick = 100;
            soundType = SoundID.Tink;
            dustType = DustID.Stone;
        }
    }
}
