using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;

namespace IceKracken.BlockMechanic
{
    class TestProjectile : InteractiveProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 80;
            projectile.height = 80;
            projectile.timeLeft = 600;
            projectile.aiStyle = -1;
        }
        public override void AI()
        {
            if(projectile.timeLeft == 600)
            {
                ValidPoints.Add(new Point16((int)projectile.Center.X / 16, (int)projectile.Center.Y / 16 - 6));
            }
        }
        public override void GoodEffects()
        {
            Main.NewText("Good outcome!");
        }
        public override void BadEffects()
        {
            Main.NewText("Bad outcome!");
        }
    }
}
