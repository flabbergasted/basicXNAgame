using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Brian_s_Game
{
    public abstract class Object
    {
        public abstract info getInfo();//get description info for object
        public abstract Vector2 getPosition();//get the position of the object in the world
        public abstract int getMove();//get how far the object can move
        public abstract void setPosition(Vector2 newPos);//sets an objects position
    }
}
