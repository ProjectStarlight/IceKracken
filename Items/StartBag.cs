using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace IceKracken.Items
{
    class StartBag : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Call of Calamari");
            Tooltip.SetDefault("Equips you with gear and teleports you to Auroracle's temple");
        }
        public override void SetDefaults()
        {
            item.maxStack = 1;
            item.rare = ItemRarityID.Red;
        }
        public override bool CanRightClick() => true;
        public override void RightClick(Player player)
        {
            Vector2 pos = IceWorld.IceZone.TopLeft() * 16 + new Microsoft.Xna.Framework.Vector2(840, -680);
            player.statLifeMax = 200;
            player.statManaMax = 200;
            player.SpawnX = (int)pos.X / 16;
            player.SpawnY = (int)pos.Y / 16;
            player.position = pos;
            player.fallStart = (int)pos.Y;

            Item.NewItem(player.Center, ItemID.LightsBane);
            Item.NewItem(player.Center, ItemID.CorruptYoyo);
            Item.NewItem(player.Center, ItemID.SapphireStaff);
            Item.NewItem(player.Center, ItemID.GoldBow);
            Item.NewItem(player.Center, ItemID.GrapplingHook);
            Item.NewItem(player.Center, ItemID.WoodenArrow, 999);
            Item.NewItem(player.Center, ItemID.DirtBlock, 999);

            Item.NewItem(player.Center, ItemID.IronHelmet);
            Item.NewItem(player.Center, ItemID.IronChainmail);
            Item.NewItem(player.Center, ItemID.IronGreaves);
            Item.NewItem(player.Center, ItemID.SapphireRobe);
            Item.NewItem(player.Center, ItemID.BlizzardinaBottle);
            Item.NewItem(player.Center, ItemID.BandofRegeneration);
            Item.NewItem(player.Center, ItemID.Shackle);
            Item.NewItem(player.Center, ItemID.FlurryBoots);
            Item.NewItem(player.Center, ItemID.LuckyHorseshoe);
        }
    }
}
