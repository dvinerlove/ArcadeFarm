using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace farm.Models
{
    public static class Texture2DExtention
    {
        public static Texture2D GetPart(this Texture2D pSourceTexture, Rectangle pSourceRectangle)
        {
            Texture2D texture = new Texture2D(pSourceTexture.GraphicsDevice, pSourceRectangle.Width, pSourceRectangle.Height);
            Color[] data = new Color[pSourceRectangle.Width * pSourceRectangle.Height];
            pSourceTexture.GetData<Color>(0, pSourceRectangle, data, 0, data.Length);
            texture.SetData<Color>(data);
            return texture;
        }
    }
}
