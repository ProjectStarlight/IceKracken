using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace IceKracken
{
    class IcePlayer : ModPlayer
    {
        public int DoT { get; set; }

        public int Shake { get; set; } = 0;
        public int panDown;
        public int ScreenMoveTime = 0;
        public Vector2 ScreenMoveTarget = new Vector2(0, 0);
        public Vector2 ScreenMovePan = new Vector2(0, 0);
        private int ScreenMoveTimer = 0;

        public int PlatformTimer { get; set; }
        public override void PreUpdate()
        {
            if (PlatformTimer > 0) PlatformTimer--;
        }
        public override void UpdateBadLifeRegen()
        {
            if (DoT > 0)
            {
                player.lifeRegen = 0;
                player.lifeRegenTime = 0;
                player.lifeRegen -= DoT * 2;

                Main.musicFade[Main.curMusic] = 0.05f;
            }
        }
        public override void ResetEffects()
        {
            DoT = 0;
        }
        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if (player.HasBuff(ModContent.BuffType<Buffs.PrismaticDrown>())) damageSource = PlayerDeathReason.ByCustomReason(player.name + " fell into the drink...");
            return true;
        }
        public override void ModifyScreenPosition()
        {
            if (ScreenMoveTime > 0 && ScreenMoveTarget != Vector2.Zero)
            {
                Vector2 off = new Vector2(Main.screenWidth, Main.screenHeight) / -2;
                if (ScreenMoveTimer <= 30) //go out
                {
                    Main.screenPosition = Vector2.SmoothStep(Main.LocalPlayer.Center + off, ScreenMoveTarget + off, ScreenMoveTimer / 30f);
                }
                else if (ScreenMoveTimer >= ScreenMoveTime - 30) //go in
                {
                    Main.screenPosition = Vector2.SmoothStep((ScreenMovePan == Vector2.Zero ? ScreenMoveTarget : ScreenMovePan) + off, Main.LocalPlayer.Center + off, (ScreenMoveTimer - (ScreenMoveTime - 30)) / 30f);
                }
                else
                {
                    if (ScreenMovePan == Vector2.Zero) Main.screenPosition = ScreenMoveTarget + off; //stay on target
                    else if (ScreenMoveTimer <= ScreenMoveTime - 300) Main.screenPosition = Vector2.SmoothStep(ScreenMoveTarget + off, ScreenMovePan + off, ScreenMoveTimer / (float)(ScreenMoveTime - 300));
                    else Main.screenPosition = ScreenMovePan + off;
                }

                if (ScreenMoveTimer == ScreenMoveTime) { ScreenMoveTime = 0; ScreenMoveTimer = 0; ScreenMoveTarget = Vector2.Zero; ScreenMovePan = Vector2.Zero; }
                ScreenMoveTimer++;
            }

            if(Main.tile[(int)player.Center.X / 16, (int)player.Center.Y / 16].wall == ModContent.WallType<BrickWall>() && 
                Main.npc.Any(n => n.active && n.modNPC is Boss.MainBody && n.ai[0] == (int)Boss.MainBody.AIStates.SecondPhase && n.ai[1] > 300) && panDown < 150) //the worlds most ungodly check ever
            {
                panDown++;
            }
            else if (panDown > 0)
            {
                panDown--;
            }
            Main.screenPosition += new Vector2(0, panDown);

            if (Shake > 0)
            {
                Main.screenPosition.Y += Main.rand.Next(-Shake, Shake);
                Main.screenPosition.X += Main.rand.Next(-Shake, Shake);
                if (Shake > 0) { Shake--; }
            }
        }
    }
}
