using controller;
using SpaceWars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using world;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace Server
{
    public class myServe
    {
        string FILENAME = "..\\..\\..\\Resources\\settings.xml";
        //This is the server information and opject.
        int hitPoints { get; set; } // "I forgot to mention, this is the story of how I became the strongest hero."
        int projectileSpeed { get; set; } // Fire first, you win. 
        double engineStrength { get; set; } // Read: NASCAR
        int turningRate { get; set; } // "We're going to hit that wall!!" "Oh, thank god." 
        int shipSize { get; set; } // A ship isn't really as big as a star, but this is a fantasy based on reality.
        double starSize { get; set; } // A star isn't really as small as a ship, but this is a fantasy based on reality.
        double starMass { get; set; } // supermassive!
        int universeSize { get; set; } // Our universe is like two steps big in Skyrim.
        int timePerFrame { get; set; } // "I only play games 60 FPS or above." -- most people in our major
        int projectileFiringDelay { get; set; } // Hang on, I have to reload!
        int respawnDelay { get; set; } // "Your sister is dead! I killed her with my own hands! [for eight frames aprox.]"
        bool touhouMode { get; set; } // I never liked the Touhou games. Either way, this makes the game into a bullethell. Combine with low bullet respawn time. 
        bool bulletGravity { get; set; } // Adds dimensions upon dimensions of realism to the game. (read: adds marginal depth to game)
        bool supermassive { get; set; } // Lets you shoot the sun to add extra mass!? Is that science or what? 

        public const string connectionString = "server=atr.eng.utah.edu;" +
        "database=cs3500_u0912970;" +
        "uid=cs3500_u0912970;" +
        "password=hello1234";


        double duration { get; set; }//Duration of the game

        //We need to keep track of all of the clients on the server
        int countOfClients = 0;
        World theWorld;
        static System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        static System.Diagnostics.Stopwatch gameTimer = new System.Diagnostics.Stopwatch();

        /// <summary>
        /// Makes the World object, and it manages most other things. 
        /// </summary>
        public myServe()
        {
            gameTimer.Start();
            //I will also want to create the one world that is associated with the server
            theWorld = new World();
            XMLRead();
        }

        /// <summary>
        /// Class to build a ship and set it to a random position.
        /// </summary>
        /// <param name="ss"></param>
        public void buildship(SocketState ss)
        {
            //random starting location
            //we want to strip the '/n' off of the end
            String _name = ss.bs.ToString().Replace("\n", "");
            //making the name lower case so that it won't ever cause a fire or anything
            _name = _name.ToLower();
            Random rnd = new Random();
            int startingXPosition = rnd.Next(-universeSize/2, universeSize/2);
            int startingYPosition = rnd.Next(-universeSize / 2, universeSize / 2);
            Vector2D loc = new Vector2D(startingXPosition, startingYPosition);
            lock (theWorld) // Adds ship to the world and increments the count so no ships have the same ID. 
            {
                theWorld.AddShip(ss, new ship(countOfClients, ss, hitPoints, loc, _name, 0, new Vector2D(), projectileFiringDelay));
                countOfClients++;
            }
        }

        /// <summary>
        /// Starts the connection loop that allows clients to connect. 
        /// </summary>
        public void startConnection()
        {
            
            //Create a new Connection State
            Console.WriteLine("Awaiting Connection");
            networking.ServerAwaitingClientLoop(HandleNewClient); // Handles new client.
            

            while(true) // Updates the screen indefinitely. 
            {
                update();
            }

        }
        
        public void update()
        {
            sw.Start(); // FPS timer, essentially. Ensures we don't overwhelm the thread. 
            while (sw.ElapsedMilliseconds < timePerFrame)
            {
                //Do nothing. But do nothing with purpose.
            }
            sw.Reset();

            lock (theWorld)
            {
                String sending = getTheWorld();
                foreach (ship s in theWorld.getShips().Values)
                {
                    //We will get the socketstate stored in each ship and send the data
                    if (networking.Send(s.socks, sending)) { }
                    else
                    {
                        Console.WriteLine("Client removed~~~"); // Deal with removing clients elegantly. 
                        foreach (var sockey in theWorld.shipsInWorld)
                        {
                            if (sockey.Value == s)
                            {
                                ship temper;
                                theWorld.shipsInWorld.TryRemove(sockey.Key, out temper);
                            }
                        }

                    }
                }
            }

            foreach (var projedy in theWorld.projectilesInWorld)
            {
                projectile remover;
                if (projedy.Value.alive == false) // If the projectile is dead, it needn't be in our dictionary any more. 
                {
                    theWorld.projectilesInWorld.TryRemove(projedy.Key, out remover);
                }
            }

            foreach (var ships in theWorld.getShips().Values)
            {
                foreach (var stars in theWorld.starsInWorld.Values) // Updates each ship location. 
                    updateShipLocation(ships, stars);


                if (ships.dead == true)
                {
                    //decrease the dead counter
                    ships.deadTimer--;
                    if (ships.deadTimer <= 0) // Changes the dead timer if dead, and if it's time to respawn, handle that. 
                    {
                        ships.frankenstein(universeSize);
                    }
                }
                ships.decreaseProjectileDelay(); // Handles decreasing projectile delay.
            }

            foreach (var projectile in theWorld.projectilesInWorld.Values)
            {
                foreach (var starry in theWorld.starsInWorld.Values)
                {
                    updateProjectileLocation(projectile, starry);
                }
            }

        

            checkShipProjCollision();
        }



        public void save()
        {
                // Connect to the DB
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    try
                    {
                        // Open a connection
                        conn.Open();

                        // Create a command
                        MySqlCommand command = conn.CreateCommand();
                        command.CommandText = "select CardNum, Name from Patrons";

                        // Execute the command and cycle through the DataReader object
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine(addInfoToServer());
                            }
                        }

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
          
        }


        /// <summary>
        /// Checks if a ship is coliding with projectiles. 
        /// </summary>
        public void checkShipProjCollision()
        {
            foreach (var ships in theWorld.shipsInWorld.Values)
            {
                foreach (var proj in theWorld.projectilesInWorld.Values)
                {
                    if (proj.owner != ships.ID) // So you can't kill yourself, check the ID.
                    {
                        Vector2D holder = (proj.GetLocation() - ships.GetLocation());
                        double len = holder.Length();
                        if (len < shipSize)
                        {
                            ships.decreaseHP(respawnDelay);
                            if (ships.hp <= 0)
                            {
                                increaseShipScore(proj.owner);
                            }
                            proj.kill(); // Kills the projectile if it collides with ship.
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Called when the ship's score needs to be incremented by one.
        /// </summary>
        /// <param name="id"></param>
        public void increaseShipScore(int id)
        {
            foreach(ship sh in theWorld.shipsInWorld.Values)
            {
                if(id == sh.ID)
                {
                    sh.increaseScore();
                    return;
                }
            }
            
        }

        /// <summary>
        /// Uses a stringbuilder to serialize the world into a string to send.
        /// </summary>
        /// <returns></returns>
        public string getTheWorld()
        {
            StringBuilder overarchingWorld = new StringBuilder();
            lock (theWorld)
            {
                foreach (var shipper in theWorld.shipsInWorld.Values)
                {
                    theWorld.detectCollisions(shipSize, starSize, respawnDelay);
                    overarchingWorld.Append(JsonConvert.SerializeObject(shipper) + "\n");
                }
                foreach (var starrer in theWorld.starsInWorld.Values)
                {
                    overarchingWorld.Append(JsonConvert.SerializeObject(starrer) + "\n");
                }
                foreach (var projectiler in theWorld.projectilesInWorld.Values)
                {
                    detectCollisions(starSize); // Checks if projectile collides with sun... Different function dependent on supermassive mode.
                    overarchingWorld.Append(JsonConvert.SerializeObject(projectiler) + "\n"); // Adds it all...
                }
            }

            //Serialize the world into a JSON string and add a null terminating character at the end.
            return overarchingWorld.ToString(); // Returns the world in string form!
        }


    /// <summary>
    /// Processes incoming data to blit on the screen.
    /// Adds items to overarching item dictionaries if they aren't there
    /// If they are there, updates the entries. 
    /// </summary>
    /// <param name="ss"></param>
    public void processMessageServer(SocketState ss)
        {
            string totalData = ss.bs.ToString();
            string[] parts = Regex.Split(totalData, @"(?<=[\n])");

            //the second part in parts will be the command
            foreach (string p in parts)
            {
                if (p.Length == 0)
                    continue;
                if (p[p.Length - 1] != '\n')
                    break;

                ss.bs.Remove(0, p.Length); // Removes the now unnecessary data fromt the StringBuilder.

                //for each object we want to determine what it is.
                //The one is message bites is the one we want
                lock (theWorld)
                {
                    if (theWorld.shipsInWorld[ss].dead == false)
                    {
                        if (p.StartsWith("("))
                        {
                            if (p.Contains("T"))
                            {
                                //thrust
                                theWorld.shipsInWorld[ss].setThrust(true);
                            }
                            if (p.Contains("R"))
                            {
                                //right
                                theWorld.shipsInWorld[ss].rotateRight(turningRate);
                            }
                            if (p.Contains("L"))
                            {
                                //roate the ship left
                                theWorld.shipsInWorld[ss].rotateLeft(turningRate);
                            }
                            if (p.Contains("F"))
                            {
                                //firing
                                //First we want to check that the projectile firing delay is being obeyed
                                if (theWorld.shipsInWorld[ss].projectileFiringDelayTimer <= 0) // If the timer is equal to zero or below zero, 
                                {
                                    theWorld.createProjectile(theWorld.shipsInWorld[ss]);     //We can shoot a projectile
                                    theWorld.shipsInWorld[ss].resetProjectileDelay(projectileFiringDelay);//We will reset the projectile firing delay for it to begin
                                                                                              //counting down again.
                                }
                            }
                        }
                        else
                        {
                            //"You can barely stay on your feet...! Just give up already!"
                        }
                    }
                }
            }

            
            networking.GetData(ss); // And asks for more data when we're done processing! 
            
        }


       
        /// <summary>
        /// Get the client's name if it joins the game! Then send the world size and stuff.
        /// </summary>
        /// <param name="socks"></param>
        public void HandleNewClient(SocketState socks)
        {
            socks.Call = ReceiveName;
            networking.GetData(socks);
        }

        /// <summary>
        /// Gets the name from the socketstate. 
        /// Sends ship ID and worldsize back, forwarding the handshake. After this, the server and clients are synced.
        /// </summary>
        /// <param name="socks"></param>
        public void ReceiveName(SocketState socks)
        {
            lock (this)
            {
                //send the startup data. The server must not send any world data to a client before sending the startup data.
                //send world size and ID
                //add client to dictionary
                buildship(socks);
                Console.WriteLine("New Client added");
                networking.Send(socks, countOfClients.ToString() + '\n' + this.universeSize.ToString() + '\n');
                socks.Call = processMessageServer;
                networking.GetData(socks);
            }
        }

        /// <summary>
        /// Uses vector math to update the ship's location. 
        /// </summary>
        /// <param name="shippyUpdate">Ship to be updated.</param>
        /// <param name="starry">star for gravity</param>
        private void updateShipLocation(ship shippyUpdate, star starry)
        {
            Vector2D gravity = starry.loc - shippyUpdate.loc; // Find the vector from the ship to the star.
            gravity.Normalize(); // Turn the vector into a unit-length direction by normalizing. 
            gravity *= starry.mass; // Adjust strentth of vector by multiplying star's mass. 

            Vector2D thrust;

            //We need to only do this thrus if the ship is thrusting, otherwise the star will only have the impact on the ship
            if (shippyUpdate.getThrust() == true)
            {
                thrust = shippyUpdate.GetDirections();// So long as rotate is only ever applied, should always be normalized. 

                thrust = thrust * (engineStrength); // Adjust the length of the vector by multiplying by the engine strength. 
                shippyUpdate.setThrust(false);
            }
            else
            {
                thrust = new Vector2D(0, 0); // no thrust for you.
            }
        Vector2D acceleration = gravity + thrust; // combine all forces.

        shippyUpdate.velocity += acceleration; // Add acceleration to velocity.
            
        shippyUpdate.loc = shippyUpdate.loc + shippyUpdate.velocity; // Apply new velocity to position.

        checkIfOffScreen(shippyUpdate); // Wraparound if offscreen.
           

        }

        /// <summary>
        /// Fires the projectile in a line from the ship, straight. 
        /// </summary>
        /// <param name="proj"></param>
        private void updateProjectileLocation(projectile proj, star starry)
        {

            if (bulletGravity == false)
            {
                Vector2D direction = proj.GetDirections(); // We have the direction
                direction *= projectileSpeed;

                proj.loc = proj.loc + direction;// Apply new velocity to position.

                checkIfOffScreen(proj, touhouMode);
            }
            else
            {
                Vector2D gravity = starry.loc - proj.loc; // Find the vector from the ship to the star.
                gravity.Normalize(); // Turn the vector into a unit-length direction by normalizing. 
                gravity *= starry.mass; // Adjust strentth of vector by multiplying star's mass. 

                Vector2D thrust = new Vector2D(0, 0); // no thrust for you.
                
                Vector2D acceleration = gravity + thrust; // combine all forces.

                proj.velocity += acceleration; // Add acceleration to velocity.

                proj.loc = proj.loc + proj.velocity; // Apply new velocity to position.

                checkIfOffScreen(proj, touhouMode); // Wraparound if offscreen.
            }

            
        }

        public string addInfoToServer()
        {
            string addingPlayersToTable = "";
            foreach (var sh in theWorld.shipsInWorld.Values)
            {
                addingPlayersToTable += "insert into Scores_Of_Players values (\"" + sh.ID + "\", \"" + sh.score + "\", " + sh.calcAccuracy() + "\");"; 
            }
            return addingPlayersToTable;
        }

        public string gameInfo()
        {
            string game = "insert into games (col 2)" + "values(" + getDuration() + ");";
            return game;
        }

        public double getDuration()
        {
            return 0.0;
            //TODO:  Still need to implement the stopwatch to start getting the time of the game
        }


        /// <summary>
        /// Will check to see if the screen has flown off screen.  If it has, it will wrap the ship around to the other side of the screen
        /// </summary>
        /// <param name="sh"></param>
        public void checkIfOffScreen(ship sh)
        {
            if(sh.GetLocation().GetX()> universeSize/2 || sh.GetLocation().GetX() < -universeSize/2)
            {
                //we will want it to keep the y position and just rotate to the other side of the screen.
                double yLocation = sh.GetLocation().GetY();
                double oldxLocation = sh.GetLocation().GetX();
                Vector2D newLocation = new Vector2D(-(oldxLocation), yLocation);
                //Set the ship location to the new location
                sh.setLocation(newLocation);
            }
            if (sh.GetLocation().GetY() > universeSize / 2 || sh.GetLocation().GetY() < -universeSize / 2)
            {
                //we will want it to keep the y position and just rotate to the other side of the screen.
                double yLocation = sh.GetLocation().GetY();
                double xLocation = sh.GetLocation().GetX();
                Vector2D newLocation = new Vector2D(xLocation, -yLocation);
                sh.setLocation(newLocation);
            }
        }

        public void checkIfOffScreen(projectile pro, bool extra)
        {

            if (extra == false) // If Touhou is off, make the projectiles kill when they get offscreen.
            {
                if (pro.GetLocation().GetX() > universeSize / 2 + 50 || pro.GetLocation().GetX() < -universeSize / 2 - 50)
                {
                    pro.kill();
                }
                if (pro.GetLocation().GetY() > universeSize / 2 + 50 || pro.GetLocation().GetY() < -universeSize / 2 - 50)
                {
                    pro.kill();
                }
            }
            else
            {
                if (pro.GetLocation().GetX() > universeSize / 2 || pro.GetLocation().GetX() < -universeSize / 2)
                {
                    //we will want it to keep the y position and just rotate to the other side of the screen.
                    double yLocation = pro.GetLocation().GetY();
                    double oldxLocation = pro.GetLocation().GetX();
                    Vector2D newLocation = new Vector2D(-(oldxLocation), yLocation);
                    //Set the projectile location to the new location
                    pro.setLocation(newLocation);
                }
                if (pro.GetLocation().GetY() > universeSize / 2 || pro.GetLocation().GetY() < -universeSize / 2)
                {
                    //we will want it to keep the y position and just rotate to the other side of the screen.
                    double yLocation = pro.GetLocation().GetY();
                    double xLocation = pro.GetLocation().GetX();
                    Vector2D newLocation = new Vector2D(xLocation, -yLocation);
                    pro.setLocation(newLocation); // Set projectile to modified location.
                }
            }

        }

        /// <summary>
        /// Detects collisions between projectiles and stars. 
        /// If a projectile hits a star, kills it.
        /// </summary>
        /// <param name="starSize"></param>
        public void detectCollisions(double starSize)
        {
            //Here we will want to go through every single item and see if it has collided
            //Check if the distance between the two is less than the projectile size or the star size

            
            //Detects collision for the star
            foreach (var proj in theWorld.projectilesInWorld.Values)
            {
                foreach (star st in theWorld.starsInWorld.Values)
                {
                    Vector2D holder = (proj.GetLocation() - st.GetLocation());
                    double len = holder.Length();
                    if (len < starSize)
                    {
                        proj.kill();
                        if (supermassive)
                        {
                            st.incrementMass(.0005);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Reads into the game's variables the settings. 
        /// </summary>
        public void XMLRead()
        {
            XmlReader reader = XmlReader.Create(FILENAME);
            while(reader.Read())
            {
                if(reader.IsStartElement())
                {
                    //get the element name and switch
                    switch(reader.Name)
                    {
                        case "hitPoints":
                            
                            hitPoints = reader.ReadElementContentAsInt();
                            break;
                        case "projectileSpeed":
                          
                            projectileSpeed = reader.ReadElementContentAsInt();
                            break;
                        case "engineStrength":
                           
                            engineStrength = reader.ReadElementContentAsDouble();
                            break;
                        case "turningRate":
                            
                            turningRate = reader.ReadElementContentAsInt();
                            break;
                        case "shipSize":
                            
                            shipSize = reader.ReadElementContentAsInt();
                            break;
                        case "starSize":

                            starSize = reader.ReadElementContentAsInt();
                            break;

                        case "starMass":
                            starMass = reader.ReadElementContentAsDouble();
                            break;
                        case "universeSize":
                             
                            universeSize = reader.ReadElementContentAsInt();
                            break;
                        case "timePerFrame":
                             
                            timePerFrame = reader.ReadElementContentAsInt();
                            break;

                        case "projectileFiringDelay":
                             
                            projectileFiringDelay = reader.ReadElementContentAsInt();
                            break;
                        case "respawnDelay":
                             
                            respawnDelay = reader.ReadElementContentAsInt();
                            break;

                        case "touhouMode":
                            touhouMode = reader.ReadElementContentAsBoolean();
                            break;

                        case "bulletGravity":
                            bulletGravity = reader.ReadElementContentAsBoolean();
                            break;

                        case "supermassiveMode":
                            supermassive = reader.ReadElementContentAsBoolean();
                            break;

                        case "Star":
                            int xLoc =9999;
                            int yLoc= 9999;
                            double mass =0;
                            reader.Read();
                            reader.Read();
                            if(reader.Name == "x")
                            {
                                xLoc = reader.ReadElementContentAsInt();
                            }
                            reader.Read();
                            if (reader.Name == "y")
                            {
                                yLoc = reader.ReadElementContentAsInt();
                            }
                            reader.Read();
                            if (reader.Name == "mass")
                            {
                                mass = reader.ReadElementContentAsDouble();
                            }
                            //Now that we have all the information, we will create a star
                            theWorld.addStar(mass, xLoc, yLoc);
                            break;
                    }
                }
            }
        }
    }
}
