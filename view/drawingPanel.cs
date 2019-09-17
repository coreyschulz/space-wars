using SpaceWars;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;

namespace view
{
    public class drawingPanel: Panel
    {

        /// <summary>
        /// 
        /// </summary>
        private const int shipSize = 35;
        private const int projSize = 200;
        private const int sunSizeDivisor = 4;

        private worldView world;
        
        /// <summary>
        /// Sets up lists of all ship images. Images added during construction.
        /// overarchingImages is a list of all the sub lists. 
        /// Can be accessed as: overarchingImages[0][1]
        /// This is blue ship, thrusting, for example.
        /// </summary>
        private List<List<Image>> overarchingImages = new List<List<Image>>();
        private List<Image> allBlueShips = new List<Image>();
        private List<Image> allRedShips = new List<Image>();
        private List<Image> allVioletShips = new List<Image>();
        private List<Image> allYellowShips = new List<Image>();
        private List<Image> allBrownShips = new List<Image>();
        private List<Image> allGreyShips = new List<Image>();
        private List<Image> allWhiteShips = new List<Image>();
        private List<Image> allGreenShips = new List<Image>();
        private List<Image> allStarImages = new List<Image>();



        public drawingPanel(worldView w)
        {
            //Get the images into the overarching list. 

            allBlueShips.Add(Image.FromFile("..\\..\\..\\Resources\\Images\\ship-coast-blue.png"));
            allBlueShips.Add(Image.FromFile("..\\..\\..\\Resources\\Images\\ship-thrust-blue.png"));
            allBlueShips.Add(Image.FromFile("..\\..\\..\\Resources\\Images\\shot-blue.png"));
            //blue ships will be at locaation 0 of overarchingImages.
            overarchingImages.Add(allBlueShips);

            allRedShips.Add(Image.FromFile("..\\..\\..\\Resources\\Images\\ship-coast-red.png"));
            allRedShips.Add(Image.FromFile("..\\..\\..\\Resources\\Images\\ship-thrust-red.png"));
            allRedShips.Add(Image.FromFile("..\\..\\..\\Resources\\Images\\shot-red.png"));
            //red ships will be at locaation 1 of overarchingImages.
            overarchingImages.Add(allRedShips);

            allWhiteShips.Add(Image.FromFile("..\\..\\..\\Resources\\Images\\ship-coast-white.png"));
            allWhiteShips.Add(Image.FromFile("..\\..\\..\\Resources\\Images\\ship-thrust-white.png"));
            allWhiteShips.Add(Image.FromFile("..\\..\\..\\Resources\\Images\\shot-white.png"));
            //white ships will be at locaation 2 of overarchingImages.
            overarchingImages.Add(allWhiteShips);

            allVioletShips.Add(Image.FromFile("..\\..\\..\\Resources\\Images\\ship-coast-violet.png"));
            allVioletShips.Add(Image.FromFile("..\\..\\..\\Resources\\Images\\ship-thrust-violet.png"));
            allVioletShips.Add(Image.FromFile("..\\..\\..\\Resources\\Images\\shot-violet.png"));
            //violet ships will be at locaation 3 of overarchingImages.
            overarchingImages.Add(allVioletShips);

            allBrownShips.Add(Image.FromFile("..\\..\\..\\Resources\\Images\\ship-coast-brown.png"));
            allBrownShips.Add(Image.FromFile("..\\..\\..\\Resources\\Images\\ship-thrust-brown.png"));
            allBrownShips.Add(Image.FromFile("..\\..\\..\\Resources\\Images\\shot-brown.png"));
            //brown ships will be at locaation 4 of overarchingImages.
            overarchingImages.Add(allBrownShips);

            allGreyShips.Add(Image.FromFile("..\\..\\..\\Resources\\Images\\ship-coast-grey.png"));
            allGreyShips.Add(Image.FromFile("..\\..\\..\\Resources\\Images\\ship-thrust-grey.png"));
            allGreyShips.Add(Image.FromFile("..\\..\\..\\Resources\\Images\\shot-grey.png"));
            //grey ships will be at locaation 5 of overarchingImages.
            overarchingImages.Add(allGreyShips);

            allYellowShips.Add(Image.FromFile("..\\..\\..\\Resources\\Images\\ship-coast-yellow.png"));
            allYellowShips.Add(Image.FromFile("..\\..\\..\\Resources\\Images\\ship-thrust-yellow.png"));
            allYellowShips.Add(Image.FromFile("..\\..\\..\\Resources\\Images\\shot-yellow.png"));
            //yellow ships will be at locaation 6 of overarchingImages.
            overarchingImages.Add(allYellowShips);

            allGreenShips.Add(Image.FromFile("..\\..\\..\\Resources\\Images\\ship-coast-green.png"));
            allGreenShips.Add(Image.FromFile("..\\..\\..\\Resources\\Images\\ship-thrust-green.png"));
            allGreenShips.Add(Image.FromFile("..\\..\\..\\Resources\\Images\\shot-green.png"));
            //green ships will be at locaation 7 of overarchingImages.
            overarchingImages.Add(allGreenShips);

            allStarImages.Add(Image.FromFile("..\\..\\..\\Resources\\Images\\star.jpg"));
            //star will be at locaation 8 of overarchingImages.
            overarchingImages.Add(allStarImages);

            DoubleBuffered = true;
            world = w;
        }


        /// <summary>
        /// Helper method for DrawObjectWithTransform
        /// </summary>
        /// <param name="size">The world (and image) size</param>
        /// <param name="w">The worldspace coordinate</param>
        /// <returns></returns>
        private static int WorldSpaceToImageSpace(int size, double w)
        {
            return (int)w + size / 2;
        }

        // A delegate for DrawObjectWithTransform
        // Methods matching this delegate can draw whatever they want using e  
        public delegate void ObjectDrawer(object o, PaintEventArgs e);


        /// <summary>
        /// This method performs a translation and rotation to drawn an object in the world.
        /// </summary>
        /// <param name="e">PaintEventArgs to access the graphics (for drawing)</param>
        /// <param name="o">The object to draw</param>
        /// <param name="worldSize">The size of one edge of the world (assuming the world is square)</param>
        /// <param name="worldX">The X coordinate of the object in world space</param>
        /// <param name="worldY">The Y coordinate of the object in world space</param>
        /// <param name="angle">The orientation of the objec, measured in degrees clockwise from "up"</param>
        /// <param name="drawer">The drawer delegate. After the transformation is applied, the delegate is invoked to draw whatever it wants</param>
        private void DrawObjectWithTransform(PaintEventArgs e, object o, int worldSize, double worldX, double worldY, double angle, ObjectDrawer drawer)
        {
            // Perform the transformation
            int x = WorldSpaceToImageSpace(worldSize, worldX);
            int y = WorldSpaceToImageSpace(worldSize, worldY);


            e.Graphics.TranslateTransform(x, y);
            e.Graphics.RotateTransform((float)angle);
            // Draw the object 
            drawer(o, e);
            // Then undo the transformation
            e.Graphics.ResetTransform();
        }


        /// <summary>
        /// Acts as a drawing delegate for DrawObjectWithTransform
        /// After performing the necessary transformation (translate/rotate)
        /// DrawObjectWithTransform will invoke this method
        /// </summary>
        /// <param name="shipo">The object to draw</param>
        /// <param name="e">The PaintEventArgs to access the graphics</param>
        private void shipDrawer(object o, PaintEventArgs e)
        {
            ship shipo = o as ship;
            if (shipo.thrust == false)
            {
                //ID % 8 takes the ID and mods it by 8 to get the proper ship color.
                Image i = overarchingImages[shipo.ID % 8][0];
                e.Graphics.DrawImage(i, shipSize / -2, shipSize / -2, shipSize, shipSize);


            }
            else
            {
                Image i = overarchingImages[shipo.ID % 8][1];
                e.Graphics.DrawImage(i, shipSize / -2, shipSize / -2, shipSize, shipSize);
            }
        }

        private void starDrawer(object o, PaintEventArgs e)
        {
            star oStar = o as star;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Image i = overarchingImages[8][0]; // [8][0] is the constant location of the sun image. Won't change.

            e.Graphics.DrawImage(i, i.Width / -8, i.Height / -8, i.Width / 4, i.Height / 4); // And blits the image.

        }

        private void projectileDrawer(object o, PaintEventArgs e)
        {
            projectile oProj = o as projectile;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            //Get color of projectile
            //The projectile will comes from the certain owner at a certain locations on the owner object.
            //ID % 8 takes the ID and mods it by 8 to get a random color for the projectile! Squee~~!

            Image i = overarchingImages[oProj.ID % 8][2];
            e.Graphics.DrawImage(i, i.Width/-6, i.Height/-6, i.Height/3, i.Width/3);
        }


        // This method is invoked when the DrawingPanel needs to be re-drawn
        /// <summary>
        /// Draws all the players, stars and projectiles each time the form is invalidated.
        /// The lock is used so cross-threads can't screw it up!
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            // Draw the players
            // Draw the stars
            // Draw the projectiles.
            
            lock (world)
            {

                foreach (var shipper in world.playersInWorld.ToList())
                {
                    ship p = shipper.Value as ship;
                    if (p.hp > 0) // If the HP is above zero, the ship deserves to be drawn. 
                    {
                        DrawObjectWithTransform(e, p, world.worldPanel.Size.Width, p.GetLocation().GetX(), p.GetLocation().GetY(), p.GetDirections().ToAngle(), shipDrawer);
                
                    }
                }

                foreach (var starry in world.starsInWorld.ToList())
                {
                    star p = starry.Value as star;
                    DrawObjectWithTransform(e, p, world.worldPanel.Size.Width, p.GetLocation().GetX(), p.GetLocation().GetY(), 0, starDrawer);

                }

                foreach (var proj in world.projectilesInWorld.ToList())
                {
                    projectile p = proj.Value as projectile;

                    if (p.alive == true)
                    {
                        DrawObjectWithTransform(e, p, world.worldPanel.Size.Width, p.GetLocation().GetX(), p.GetLocation().GetY(), p.GetDirections().ToAngle(), projectileDrawer);
                    }
                    else // If it's dead, can be removed from the projectile list. 
                    {
                        world.projectilesInWorld.Remove(p.ID);
                    }
                }

            }
            // Do anything that Panel (from which we inherit) needs to do
            base.OnPaint(e);
        }
    }
}
