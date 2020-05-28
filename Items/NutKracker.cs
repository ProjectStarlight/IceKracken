using IceKracken.Boss;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace IceKracken.Items
{
	public class NutKracker : ModItem
	{
		public override void SetStaticDefaults() 
		{
			// DisplayName.SetDefault("NutKracker"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			Tooltip.SetDefault("This is a basic modded sword.");
		}

		public override void SetDefaults() 
		{
			item.damage = 50;
			item.melee = true;
			item.width = 40;
			item.height = 40;
			item.useTime = 2;
			item.useAnimation = 2;
			item.useStyle = 1;
			item.knockBack = 6;
			item.value = 10000;
			item.rare = 2;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
		}

		public override void AddRecipes() 
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.DirtBlock, 10);
			recipe.AddTile(TileID.WorkBenches);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}

        public override bool UseItem(Player player)
        {
            if (Main.npc.Any(n => n.active && n.type == ModContent.NPCType<ArenaActor>()))
            {
                ArenaActor actor = Main.npc.FirstOrDefault(n => n.active && n.type == ModContent.NPCType<ArenaActor>()).modNPC as ArenaActor;

                if (player.altFunctionUse == 2)
                {
                    actor.npc.ai[0] -= 4;
                }
                else
                {
                    actor.npc.ai[0] ++;
                }
            }
            IceWorld.BossOpen = false;
            return true;
        }
        public override bool AltFunctionUse(Player player) => true;

    }
}