using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace WizardryShared
{
    public class TextureProvider
    {
		ContentManager content;

        Dictionary<string, AnimationTexture> animationTextures = new Dictionary<string, AnimationTexture>();

        public TextureProvider( ContentManager content )
        {
            this.content = content;
        }

        public AnimationTexture GetTexture(string name)
        {
            if (animationTextures.ContainsKey(name)){
                return animationTextures[name];
            }
            Texture2D texture = content.Load<Texture2D>(name);
            Color[,] colorArray = TextureTo2DArray(texture);
            AnimationTexture at = new AnimationTexture(texture, colorArray);
            animationTextures.Add(name, at);
            return at;
        }

        private Color[,] TextureTo2DArray(Texture2D texture)
        {
            Color[] colors1D = new Color[texture.Width * texture.Height];
            texture.GetData(colors1D);

            Color[,] colors2D = new Color[texture.Width, texture.Height];
            for (int x = 0; x < texture.Width; x++)
                for (int y = 0; y < texture.Height; y++)
                    colors2D[x, y] = colors1D[x + y * texture.Width];

            return colors2D;
        }
    }
}
