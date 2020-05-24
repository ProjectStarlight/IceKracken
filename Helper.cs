using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Graphics;

namespace IceKracken
{
    static class Helper
    {
        public static int SamplePerlin2D(int x, int y, int min, int max)
        {
            Texture2D perlin = TextureManager.Load("Images/Misc/Perlin");

            Color[] rawData = new Color[perlin.Width]; //array of colors
            Rectangle row = new Rectangle(0, y, perlin.Width, 1); //one row of the image
            perlin.GetData<Color>(0, row, rawData, 0, perlin.Width); //put the color data from the image into the array
            return (int)(min + rawData[x % 512].R / 255f * max);
        }
    }
}
