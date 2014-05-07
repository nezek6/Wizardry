using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WizardryShared
{
    public class AnimationTexture
    {
        Texture2D texture;
        Color[,] colorArray;

        public Texture2D Texture
        {
            get { return texture;}  
        }
        public Color[,] ColorArray
        {
            get { return colorArray; }
        }

        public AnimationTexture(Texture2D texture, Color[,] colorArray)
        {
            this.texture = texture;
            this.colorArray = colorArray;
        }
    }
}
