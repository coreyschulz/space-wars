using SpaceWars;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace view
{
    class scoreBoardPanel : Panel
    {
        private worldView world;

        /// <summary>
        /// Sets the dimensions for everything. 
        /// </summary>
        private const int scoresWidth = 145;
        private const int maxBarWidth = 120;
        private const int hpSeparator = 24;

        // A list of all brushes for player colors. 
        private List<Brush> allBrushes;

        /// <summary>
        /// Constructor. Adds brushes to overarching list during construction.
        /// </summary>
        /// <param name="w"></param>
        public scoreBoardPanel(worldView w) 
        {
            //These brushes are in the same list order as the ship images and can be accessed
            //  by the same mod! 
            allBrushes = new List<Brush>();
            allBrushes.Add(new SolidBrush(Color.DarkBlue));
            allBrushes.Add(new SolidBrush(Color.Red));
            allBrushes.Add(new SolidBrush(Color.White));
            allBrushes.Add(new SolidBrush(Color.Violet));
            allBrushes.Add(new SolidBrush(Color.Brown));
            allBrushes.Add(new SolidBrush(Color.Gray));
            allBrushes.Add(new SolidBrush(Color.Yellow));
            allBrushes.Add(new SolidBrush(Color.Green));

            DoubleBuffered = true;
            world = w;
        }

        /// <summary>
        /// Draws the player name, score and a HP bar below it. Dependent on player health, 
        /// the bar scales. And, dependent on player color, the bar is that color too!
        /// </summary>
        /// <param name="e"></param>
        /// <param name="pname">Player name.</param>
        /// <param name="hp">How many HP the ship has. Over 9000?</param>
        /// <param name="score">Current score. Good luck trying to beat my score.</param>
        /// <param name="startNamePos">Where the name will be blitted, vertically</param>
        /// <param name="startHealthPos">Where the HP bar will be blitted, vertically</param>
        /// <param name="ID">ID of the ship. Used to determine HP bar color.</param>
        private void sibylDrawer(PaintEventArgs e, string pname, int hp, int score, int startNamePos, int startHealthPos, int ID)
        {
            string drawstring = pname + ": " + score; // String to be drawn. Format: [Name]: [SCORE]

            Font drawerFont = new Font("Arial", 13);
            SolidBrush drawBrush = new SolidBrush(Color.Black);
            Pen rectPen = new Pen(Color.BlueViolet);

            PointF drawPoint = new PointF(10, startNamePos);
            Rectangle health;

            if (hp == 5)      health = new Rectangle(10, startHealthPos, maxBarWidth, 20); // If health is 5, draw the full rect.
            else if (hp == 4) health = new Rectangle(10, startHealthPos, maxBarWidth - (1 * hpSeparator), 20); // Otherwise, scale it down based on health. 
            else if (hp == 3) health = new Rectangle(10, startHealthPos, maxBarWidth - (2 * hpSeparator), 20);
            else if (hp == 2) health = new Rectangle(10, startHealthPos, maxBarWidth - (3 * hpSeparator), 20);
            else if (hp == 1) health = new Rectangle(10, startHealthPos, maxBarWidth - (4 * hpSeparator), 20);
            else health = new Rectangle(10, startHealthPos, 0, 20); // If it's got no health, it's got no bar.

            e.Graphics.DrawString(drawstring, drawerFont, drawBrush, drawPoint); // Draws the string at the desired position. 
            e.Graphics.DrawRectangle(rectPen, health);
            e.Graphics.FillRectangle(allBrushes[ID % 8], health); // And the rectangle! ID % 8 gets the proper color, I think. I'm colorblind though. 
        }

        /// <summary>
        /// Used in onPaint to order the ships according to score before Sibyl can paint them. 
        /// </summary>
        /// <returns></returns>
        private List<ship> scoreSorter()
        {
            List<ship> returner = new List<ship>();

            foreach (var item in world.playersInWorld.Values)
            {
                returner.Add(item);
            }

            returner = returner.OrderBy(o => o.score).ToList();
            returner.Reverse();

            return returner;
        }

        /// <summary>
        /// Is used to redraw the scoreboard panel each time it's invalidated.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            int startNamePos = 10; // We want to start blitting here. 
            int startHealthPos = 30;
            if (world.playersInWorld.Count > 0)
            {
                lock (world) // In case of pesky threading issues...
                {
                    foreach (var player in scoreSorter()) // For each player in the list, sorted by player score...
                    {
                        sibylDrawer(e, player.name, player.hp, player.score, startNamePos, startHealthPos, player.ID); // Draw it with Sibyl!
                        startNamePos += 50; // Increment the blit positions!
                        startHealthPos += 50; 
                    }
                }
            }

            base.OnPaint(e);
        }




       
    }
}
