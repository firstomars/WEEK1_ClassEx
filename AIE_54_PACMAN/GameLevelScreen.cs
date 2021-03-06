﻿using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Raylib_cs;

namespace AIE_54_PACMAN
{
    enum TileType
    {
        OUT_OF_BOUNDS = -1,
        EMPTY,  //0
        WALL,   //1
        DOT,     //2
        PLAYERSTART, //3
        GHOST_SPAWN //4
    }

    class GameLevelScreen : IGameState //child game state class inheriting from IGameState
    {
        //these are member variables now that they have been taken out of "draw". 
        //Can be accessed by other functions in this class
        float tileWidth = 32;
        float tileHeight = 32;
        float mapOffsetX = 50;
        float mapOffsetY = 50;

        TileType[,] map;

        int score = 0;
        int lives = 1;
        
        int numPacDots = 0;

        Player player;

        List<Ghost> ghosts = new List<Ghost>();

        public GameLevelScreen(Program program) : base(program)
        {
            LoadLevel();
        }

        void LoadLevel()
        {
            //How does the below connect with TileType? Does 0 connect with Empty etc?

            int[,] tilemap = new int[,]
            { // 0 1 2 3 4 5 6 7 8 9
                {1,1,1,1,1,1,1,1,1,1},  // 0
                {1,0,0,0,0,0,0,4,0,1},  // 1
                {1,0,1,0,0,0,0,0,0,1},  // 2
                {1,0,1,0,1,1,1,0,0,1},  // 3
                {1,0,1,0,0,3,1,0,1,1},  // 4
                {1,0,1,1,1,0,1,0,0,1},  // 5
                {1,0,0,0,0,0,1,1,0,1},  // 6
                {1,0,0,0,0,0,1,1,0,1},  // 7
                {1,0,0,0,0,0,0,0,0,1},  // 8
                {1,1,1,1,1,1,1,1,1,1},  // 9
            };

            map = new TileType[tilemap.GetLength(0),tilemap.GetLength(1)];
            //0 refers to ...
            //1 refers to ...


            //this for loop copies the above 2D array data into "map" as defined at the top of the class
            for (int row = 0; row <tilemap.GetLength(0); row++)
            {
                for (int col = 0; col < tilemap.GetLength(1); col++)
                {
                    SetTileValue(row, col, (TileType)tilemap[row, col]);
                    //map[row, col] = (TileType)tilemap[row, col];
                }
            }

            //this for loop copies the above 2D array data into "map" as defined at the top of the class
            for (int row = 0; row < tilemap.GetLength(0); row++)
            {
                for (int col = 0; col < tilemap.GetLength(1); col++)
                {
                    var tileVal = GetTileValue(row, col);

                    if (tileVal == TileType.EMPTY)              SetTileValue(row, col, TileType.DOT);
                    
                    if (tileVal == TileType.PLAYERSTART)        CreatePlayer(row, col);

                    if (tileVal == TileType.GHOST_SPAWN)        CreateGhost(row, col);
                }
            }
        }

        public void CreateGhost(int row, int col)
        {
            var rect = GetTileRect(row, col);

            Vector2 pos = new Vector2(rect.x + (rect.width / 2), rect.y + (rect.height / 2));

            Ghost ghost = new Ghost(this, pos);

            ghosts.Add(ghost);

            SetTileValue(row, col, TileType.EMPTY); //after spawned
        }

        public void CreatePlayer(int row, int col)
        {
            var rect = GetTileRect(row, col);
            Vector2 pos = new Vector2(rect.x + (rect.width / 2), rect.y + (rect.height / 2));
            player = new Player(this, pos);
            SetTileValue(row, col, TileType.EMPTY);
        }

        public TileType GetTileValue(int row, int col)
        {
            if (row < 0 || col < 0 || row >= map.GetLength(0) || col >= map.GetLength(1))
                return TileType.OUT_OF_BOUNDS;

            return map[row, col];
        }

        public TileType GetTileValue(Vector2 pos)
        {
            int row = GetYPosToRow(pos.Y);
            int col = GetXPosToCol(pos.X);
            return GetTileValue(row, col);
        }

        public void SetTileValue(int row, int col, TileType newState)
        {
            var oldState = map[row, col];

            //what do the below && if statements mean!?
            if (newState == TileType.DOT && oldState != TileType.DOT)
            {
                numPacDots += 1;
            }

            else if (oldState == TileType.DOT && newState != TileType.DOT)
            {
                numPacDots -= 1;
            }
            
            map[row, col] = newState;
        }

        public void SetTileValue(Vector2 position, TileType value)
        {
            int row = GetYPosToRow(position.Y);
            int col = GetXPosToCol(position.X);
            SetTileValue(row, col, value);
        }

        public Rectangle GetTileRect(int row, int col)
        {
            float xPos = mapOffsetX + (col * tileWidth);
            float yPos = mapOffsetY + (row * tileHeight);
            return new Rectangle(xPos, yPos, tileWidth, tileHeight);
        }

        public Rectangle GetTileRect(Vector2 pos)
        {
            int row = GetYPosToRow(pos.Y);
            int col = GetXPosToCol(pos.X);
            return GetTileRect(row, col);
        }

        public int GetTileID(int row, int col)
        {
            return row * map.GetLength(1) + col;
        }

        public int GetTileID(Vector2 pos)
        {
            int row = GetYPosToRow(pos.Y);
            int col = GetXPosToCol(pos.X);
            return GetTileID(row, col);
        }

        public Color GetTileColor(int row, int col)
        {
            var tileValue = GetTileValue(row, col);
            if (tileValue == TileType.EMPTY) return Color.BLACK;
            if (tileValue == TileType.WALL) return Color.BLUE;
            if (tileValue == TileType.DOT) return Color.BLACK;

            return Color.PINK;
        }

        public int GetYPosToRow(float yPos)
        {
            //Get row pos by taking ypos of player (e.g. 100) and dividing it by the tile height
            // map offset takes into account the top corner of where we gen. the map
            return (int)((yPos - mapOffsetY) / tileHeight);
        }

        public int GetXPosToCol(float xPos)
        {
            return (int)((xPos - mapOffsetX) / tileWidth);
        }

        public override void Update()
        {
            player.Update();
            HandleGhostPlayerCollisions();
            foreach (var ghost in ghosts)
            {
                ghost.Update();
            }
        }

        public override void Draw()
        {
            DrawMap();
            DrawUI();
            player.Draw();

            foreach (var ghost in ghosts)
            {
                ghost.Draw();
            }

            if (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL))
            {
                DebugDraw();
            }
        }

        private void DrawUI()
        {
            Raylib.DrawText($"SCORE: {score}", 10, 10, 25, Color.WHITE);
            Raylib.DrawText($"LIVES: {lives}", program.windowWidth - 120, 10, 25, Color.WHITE);
            Raylib.DrawText(program.playerName.ToUpper(), program.windowWidth / 2 - 80, 10, 25, Color.YELLOW);
        }

        private void DrawMap()
        {
            for (int row = 0; row < map.GetLength(0); row++)
            {

                for (int col = 0; col < map.GetLength(1); col++)
                {
                    var tileValue = GetTileValue(row, col);
                    var tileColor = GetTileColor(row, col);
                    var rect = GetTileRect(row, col);

                    Raylib.DrawRectangleRec(rect, tileColor);
                    
                    if (tileValue == TileType.DOT)
                    {
                        int pacDotSize = 2;
                        Raylib.DrawCircle
                            ((int)(rect.x + (rect.width/2)), 
                            (int)(rect.y + (rect.height / 2)), 
                            pacDotSize, Color.WHITE);
                    }
                }
            }
        }

        void DebugDraw()
        {
            for (int row = 0; row <map.GetLength(0); row++)
            {
                for (int col = 0; col < map.GetLength(1); col++)
                {
                    var rect = GetTileRect(row, col);
                    var color = new Color(255, 255, 255, 125);

                    Raylib.DrawRectangleRec(rect, color);
                    Raylib.DrawRectangleLinesEx(rect, 1, Color.WHITE);

                    int tileID = GetTileID(row, col);
                    int tileVal = (int)GetTileValue(row, col);
                    
                    Raylib.DrawText
                        (tileID.ToString(), 
                        (int)(rect.x+2), 
                        (int)rect.y, 10, 
                        Color.BLACK);
                    Raylib.DrawText
                        (tileVal.ToString(), 
                        (int)(rect.x+2), 
                        (int)(rect.y + rect.height - 12), 
                        10, Color.BLACK);
                }
            }
        }

        public void EatPacDot(Vector2 pos)
        {
            SetTileValue(pos, TileType.EMPTY);
            score += 10;

            if (numPacDots <= 0)
            {
                LoadLevel();
            }
        }

        void HandleGhostPlayerCollisions()
        {

            foreach(var ghost in ghosts)
            {
                var playerTileId = GetTileID(player.GetPosition());
                var ghostTileId = GetTileID(ghost.GetPosition());
                
                if (playerTileId == ghostTileId)
                {
                    player.OnCollision(ghost);
                    ghost.OnCollision(player);
                    lives -= 1;

                    if (lives < 0)
                    {
                        program.ChangeGameState(new HighScoreScreen(program));
                    }

                }
            }
        }
    }
}
