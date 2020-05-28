using Mono.Cecil.Cil;
using MonoMod.Cil;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using IceKracken.Boss;
using IceKracken.GUI;
using System.Collections.Generic;
using Terraria.UI;
using IceKracken.BlockMechanic;
using Terraria.DataStructures;

namespace IceKracken
{
	public class IceKracken : Mod
	{
        public IntroText introText;
        UserInterface customResources;

        public LifeBar lifeBar;
        UserInterface customResourcesBar;

        public static IceKracken Instance { get; set; }
        public IceKracken()
        {
            Instance = this;
        }
        public override void Load() //hooking time!
        {
            introText = new IntroText();
            customResources = new UserInterface();
            customResources.SetState(introText);

            lifeBar = new LifeBar();
            customResourcesBar = new UserInterface();
            customResourcesBar.SetState(lifeBar);

            On.Terraria.Player.Update_NPCCollision += PlatformCollision;
            On.Terraria.Main.DrawInterface += DrawBlingBlingBoy;
            On.Terraria.Player.PlaceThing += PlacementRestriction;

            IL.Terraria.Projectile.VanillaAI += GrapplePlatforms;
            IL.Terraria.Main.DoDraw += DrawWater;
        }

        private void PlacementRestriction(On.Terraria.Player.orig_PlaceThing orig, Terraria.Player self)
        {
            orig(self);
            return;
            Tile tile = Main.tile[Player.tileTargetX, Player.tileTargetY];
            if (tile.wall == ModContent.WallType<BrickWall>() &&
                !Main.projectile.Any(n => n.active && n.timeLeft > 10 && n.modProjectile is InteractiveProjectile && (n.modProjectile as InteractiveProjectile).ValidPoints.Contains(new Point16(Player.tileTargetX, Player.tileTargetY))))
            {
                return;
            }
            else orig(self);
        }

        private void DrawBlingBlingBoy(On.Terraria.Main.orig_DrawInterface orig, Main self, GameTime gameTime)
        {
            orig(self, gameTime);
            Main.spriteBatch.Begin();
            //if(Main.rand.Next(10) == 0)
            Main.spriteBatch.Draw(ModContent.GetTexture("IceKracken/Bling"), new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White * 0f);
            Main.spriteBatch.End();
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            layers.Add(new LegacyGameInterfaceLayer("NutKracker: Intro Text",
            delegate
            {
                if (IntroText.Visible)
                {
                    customResources.Update(Main._drawInterfaceGameTime);
                    introText.Draw(Main.spriteBatch);
                }

                return true;
            }, InterfaceScaleType.UI));

            layers.Add(new LegacyGameInterfaceLayer("NutKracker: LifeBar",
            delegate
            {
                if (Main.npc.Any(n => n.active && n.modNPC is MainBody))
                {
                    customResourcesBar.Update(Main._drawInterfaceGameTime);
                    lifeBar.Draw(Main.spriteBatch);
                }
                return true;
            }, InterfaceScaleType.UI));
        }



        #region IL
        private void GrapplePlatforms(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.TryGotoNext(i => i.MatchLdfld<Projectile>("aiStyle"), i => i.MatchLdcI4(7));
            c.TryGotoNext(i => i.MatchLdfld<Projectile>("ai"), i => i.MatchLdcI4(0), i => i.MatchLdelemR4(), i => i.MatchLdcR4(2));
            c.TryGotoNext(i => i.MatchLdloc(143)); //flag2 in source code
            c.Index++;
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<GrapplePlatformDelegate>(EmitGrapplePlatformDelegate);
            c.TryGotoNext(i => i.MatchStfld<Player>("grapCount"));
            c.Index++;
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<UngrapplePlatformDelegate>(EmitUngrapplePlatformDelegate);
        }
        private delegate bool GrapplePlatformDelegate(bool fail, Projectile proj);
        private bool EmitGrapplePlatformDelegate(bool fail, Projectile proj)
        {
            if (proj.timeLeft < 36000 - 3)
                for (int k = 0; k < Main.maxNPCs; k++)
                {
                    NPC n = Main.npc[k];
                    if (n.active && n.modNPC is MovingPlatform && n.Hitbox.Intersects(proj.Hitbox))
                    {
                        proj.position += n.velocity;
                        return false;
                    }
                }
            return fail;
        }
        private delegate void UngrapplePlatformDelegate(Projectile proj);
        private void EmitUngrapplePlatformDelegate(Projectile proj)
        {
            Player player = Main.player[proj.owner];
            int numHooks = 3;
            //time to replicate retarded vanilla hardcoding, wheee
            if (proj.type == 165) numHooks = 8;
            if (proj.type == 256) numHooks = 2;
            if (proj.type == 372) numHooks = 2;
            if (proj.type == 652) numHooks = 1;
            if (proj.type >= 646 && proj.type <= 649) numHooks = 4;
            //end vanilla zoink

            ProjectileLoader.NumGrappleHooks(proj, player, ref numHooks);
            if (player.grapCount > numHooks) Main.projectile[player.grappling.OrderBy(n => (Main.projectile[n].active ? 0 : 999999) + Main.projectile[n].timeLeft).ToArray()[0]].Kill();
        }

        private void DrawWater(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.TryGotoNext(n => n.MatchLdfld<Main>("DrawCacheNPCsBehindNonSolidTiles"));
            c.Index--;

            c.EmitDelegate<DrawWaterDelegate>(DrawWater);
        }

        private delegate void DrawWaterDelegate();

        private void DrawWater()
        {
            foreach (NPC npc in Main.npc.Where(n => n.active && n.modNPC is ArenaActor))
            {
                (Main.npc.FirstOrDefault(n => n.active && n.modNPC is ArenaActor).modNPC as ArenaActor).SpecialDraw2(Main.spriteBatch);

                foreach(NPC npc2 in Main.npc.Where(n => n.active && n.modNPC is IUnderwater && !(n.modNPC is MainBody)))
                {
                    (npc2.modNPC as IUnderwater).DrawUnderWater(Main.spriteBatch);
                }

                foreach (Projectile proj in Main.projectile.Where(n => n.active && n.modProjectile is IUnderwater))
                {
                    (proj.modProjectile as IUnderwater).DrawUnderWater(Main.spriteBatch);
                }

                foreach (NPC npc3 in Main.npc.Where(n => n.active && n.modNPC is MainBody))
                {
                    (npc3.modNPC as IUnderwater).DrawUnderWater(Main.spriteBatch);
                }


                (Main.npc.FirstOrDefault(n => n.active && n.modNPC is ArenaActor).modNPC as ArenaActor).SpecialDraw(Main.spriteBatch);
            }
        }


        #endregion
        #region On.
        private void PlatformCollision(On.Terraria.Player.orig_Update_NPCCollision orig, Player self)
        {
            if (self.controlDown) self.GetModPlayer<IcePlayer>().PlatformTimer = 5;
            if (self.controlDown || self.GetModPlayer<IcePlayer>().PlatformTimer > 0 || self.GoingDownWithGrapple) { orig(self); return; }
            foreach (NPC npc in Main.npc.Where(n => n.active && n.modNPC != null && n.modNPC is MovingPlatform))
            {
                if (new Rectangle((int)self.position.X, (int)self.position.Y + (self.height), self.width, 1).Intersects
                (new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, 8 + (self.velocity.Y > 0 ? (int)self.velocity.Y : 0))) && self.position.Y <= npc.position.Y)
                {
                    if (!self.justJumped && self.velocity.Y >= 0)
                    {
                        self.gfxOffY = npc.gfxOffY;
                        self.velocity.Y = 0;
                        self.fallStart = (int)(self.position.Y / 16f);
                        self.position.Y = npc.position.Y - self.height + 4;
                        orig(self);
                    }
                }
            }

            orig(self);
        }
        #endregion
    }
}