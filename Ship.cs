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
    public class Ship : Object
    {
        public int type;//ship's type number corresponding to a ship type 1=scout 2=troop 3=destroyer, etc.
        public Texture2D ship;//the sprite associated with a ship
        public Vector2 destination;
        public int health;//how much punishment the ship can take
        public int damage;//how much damage the ship can deal
        public int team;//which side the ship is on 1 or 2
        info newInfo;
        int move;//how far the ship can move(manhattan distance)
        Vector2 pos;//ships position

        public Ship (int Type,int Team, Vector2 Position, Texture2D Texture)

        {
            
            type = Type;
            pos = Position;
            ship = Texture;
            team = Team;
            if (type == 1)
            {
                move = 3;
                damage = 1;
                health = 1;
            }
            else if (type == 2)
            {
                move = 1;
                damage = 5;
                health = 10;
            }
            else if (type == 3)
            {

                move = 2;
                damage = 2;
                health = 10;
            }

        }        
        public override info getInfo()//get description info for object
        {
            if (type == 1)
            {
                newInfo = new info("Scout", "Useful for scouting out planets", "Move:3 Damage:1 Health:" + health);                
            }
            else if (type == 2)
            {
                newInfo = new info("Destroyer", "Useful for space combat", "Move:1 Damage:5 Health:" + health);                
            }
            else if (type == 3)
            {
                newInfo = new info("Troop Ship", "Useful for attacking planets", "Move:2 Damage:2 Health:" + health);               
            }
            return newInfo;
        }
        public override Vector2 getPosition()//get the position of the object in the world
        {
            return pos;
        }
        public override void setPosition(Vector2 newPos)//get the position of the object in the world
        {
            pos.X = newPos.X;
            pos.Y = newPos.Y;
        }
        public override int getMove()//get how far the object can move
        {
            return move;
        }
    }
}
