﻿using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Raylib_cs;

namespace AIE_32_ASTEROIDS
{
    class Asteroid : GameObject
    {

        public int ID;
        public static int nextID = 0;

        public float radius = 40;

        public bool asteroidCollisionPlayer = false;

        Color asteroidHitColour = new Color(255, 100, 100, 75);
        //Color asteroidNormalColour = new Color(255, 255, 255, 100);
        Color asteroidCurrentColour = new Color(255, 255, 255, 100);

        //Program program;
        //public Vector2 pos = new Vector2();

        public Asteroid(Program program, Vector2 pos, Vector2 dir, float radius)  : base(program, pos, dir)
            // Constructor - so calling this function, call the class? object instantiation?
        {
            ID = nextID;
            nextID += 1;
        }

        public override void Update()
        {
            pos += dir;

            //wrap around the screen
            if (pos.X < 0) pos.X = program.windowWidth;
            if (pos.X > program.windowWidth) pos.X = 0;
            if (pos.Y < 0) pos.Y = program.windowHeight;
            if (pos.Y > program.windowHeight) pos.Y = 0;
        }

        public override void Draw()
        {
            //Color c = new Color(255, 255, 255, 100);

            //Raylib.DrawCircle((int)pos.X, (int)pos.Y, radius, Color.WHITE);

            Color color = asteroidCollisionPlayer ? asteroidHitColour : asteroidCurrentColour;

            Raylib.DrawCircle((int)pos.X, (int)pos.Y, radius, color);

        }
    }
}
