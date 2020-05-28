using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using IceKracken.BlockMechanic;
using Microsoft.Xna.Framework.Graphics;

namespace IceKracken
{
    public class SwitchPlacer : ModItem
    {
        public override string Texture => "IceKracken/Items/NutKracker";
        public override void SetDefaults()
        {
            item.useStyle = 1;
            item.useTime = 2;
            item.useAnimation = 2;
            item.createTile = ModContent.TileType<DoorSwitch>();
            item.autoReuse = true;
        }
    }
    class DoorSwitch : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileFrameImportant[Type] = true;
        }
        public override void NearbyEffects(int i, int j, bool closer)
        {
            if(!IceWorld.BossOpen && !Main.projectile.Any(n => n.active && n.modProjectile is DoorBomb))
            {
                Projectile.NewProjectile(new Vector2(i + 1, j) * 16, new Vector2(1, 0), ModContent.ProjectileType<DoorBomb>(), 0, 0);
            }
        }
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (!IceWorld.BossOpen)
            {
                Vector2 pos = new Vector2(i, j) * 16 - Main.screenPosition + new Vector2(210, 150);
                Utils.DrawBorderString(spriteBatch, "Place blocks on", pos, Color.White, 0.7f);
                Utils.DrawBorderString(spriteBatch, "BLUE", pos + new Vector2(90, 0), Color.DeepSkyBlue, 0.7f);
                Utils.DrawBorderString(spriteBatch, "squares", pos + new Vector2(130, 0), Color.White, 0.7f);
            }
        }
    }
}
