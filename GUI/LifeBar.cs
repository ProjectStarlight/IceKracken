using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace IceKracken.GUI
{
    public class LifeBar : UIState
    {
        public static bool Visible;
        public NPC TrackingNPC;
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D tex = ModContent.GetTexture("IceKracken/GUI/Frame");
            Texture2D tex2 = ModContent.GetTexture("IceKracken/GUI/Fill");

            int fillWidth = TrackingNPC != null ? (int)(TrackingNPC.life / (float)TrackingNPC.lifeMax * tex2.Width) : 0;

            Vector2 pos = new Vector2(Main.screenWidth / 2, Main.screenHeight - 30) - tex.Size() / 2;

            if (TrackingNPC != null)
            {
                Rectangle rect = new Rectangle((int)pos.X, (int)pos.Y, fillWidth, tex2.Height);
                spriteBatch.Draw(tex2, rect, tex2.Frame(), Color.White * 0.8f);

                string str = "Boss: " + TrackingNPC.life + "/" + TrackingNPC.lifeMax;
                float xOff = (Main.fontDeathText.MeasureString(str) * 0.4f).X;
                Utils.DrawBorderStringBig(spriteBatch, str, pos + new Vector2(xOff, 7), Color.White, 0.4f);
            }

            spriteBatch.Draw(tex, pos, tex.Frame(), Color.White);
        }
    }
}
