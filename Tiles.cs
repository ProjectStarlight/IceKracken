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
            AddMapEntry(new Color(50, 90, 120));
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
            AddMapEntry(new Color(100, 150, 255));
        }
        public override void NearbyEffects(int i, int j, bool closer)
        {
            Lighting.AddLight(new Vector2(i, j) * 16, new Vector3(1, 1, 1) * 0.01f);
        }
    }
    class Brick2 : ModTile
    {
        public override bool Autoload(ref string name, ref string texture)
        {
            texture = "IceKracken/Brick";
            return base.Autoload(ref name, ref texture);
        }
        public override void SetDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            minPick = 100;
            soundType = SoundID.Tink;
            dustType = DustID.Stone;
        }
        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (IceWorld.BossOpen && !Main.npc.Any(n => n.active && n.modNPC is Boss.MainBody)) Main.tile[i, j].inActive(true);
            else Main.tile[i, j].inActive(false);
        }
    }
    public class BrickPlacer2 : ModItem
    {
        public override string Texture => "IceKracken/Items/NutKracker";
        public override void SetDefaults()
        {
            item.useStyle = 1;
            item.useTime = 2;
            item.useAnimation = 2;
            item.createTile = ModContent.TileType<Brick2>();
            item.autoReuse = true;
        }
    }
}
