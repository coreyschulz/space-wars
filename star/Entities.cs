using controller;
using Newtonsoft.Json;
using SpaceWars;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace SpaceWars
{
    //###################################################################################################################################
    //###################################################################################################################################
    //                                                      Star Class
    //###################################################################################################################################
    //###################################################################################################################################

    /// <summary>
    /// Star is compatible with JSON. 
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class star
    {
        [JsonProperty(PropertyName = "star")]
        public int ID { get; private set; }

        [JsonProperty]
        public Vector2D loc { get; private set; }

        [JsonProperty]
        public double mass { get; private set; }

        public star()
        {
        }

        /// <summary>
        /// Constructor for star object.
        /// </summary>
        /// <param name="size"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public star(double size, int x, int y, int starID)
        {
            mass = size;
            ID = starID;
            loc = new Vector2D(x, y);
        }

        /// <summary>
        /// Returns the location of a star object.
        /// </summary>
        /// <returns></returns>
        public Vector2D GetLocation()
        {
            return loc;
        }

        public void incrementMass(double increment)
        {
            mass += increment;
        }
    }

    //###################################################################################################################################
    //###################################################################################################################################
    //                                                      Ship Class
    //###################################################################################################################################
    //###################################################################################################################################

    /// <summary>
    /// Ship works with JSON. There are multiple accessor functions within used when blitting objects. 
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class ship
    {
        [JsonProperty(PropertyName = "ship")]
        public int ID { get; private set; }

        [JsonProperty]
        public Vector2D loc { get; set; }

        [JsonProperty]
        public Vector2D dir { get; private set; }

        [JsonProperty]
        public bool thrust { get; set; }

        [JsonProperty]
        public string name { get; private set; }

        [JsonProperty]
        public int hp { get; private set; }

        [JsonProperty]
        public int score { get; private set; }

        public int totalProjectiles { get; set; }
        public Vector2D velocity { get; set; }

        public bool dead = false;

        //not a JsonProperty. Used to determine ship color!?
        private int color;

        public int projectileFiringDelayTimer { get; set; }

        public SocketState socks { get; private set; }

        public int deadTimer { get; set; }

        public ship()
        {
            //blank constructor for client
        }

        /// <summary>
        /// Constructor for ship object
        /// </summary>
        /// <param name="id">player ID</param>
        /// <param name="sock">ship's socketstate.</param>
        /// <param name="_hp">starting HP</param>
        /// <param name="_location">starting LOC</param>
        /// <param name="_name">player name</param>
        /// <param name="_scores">player score</param>
        /// <param name="direction">starting direction</param>
        /// <param name="_projectileFiringDelay">Proj fire delay</param>
        public ship(int id, SocketState sock, int _hp, Vector2D _location, string _name,int _scores, Vector2D direction, int _projectileFiringDelay)
        {
            totalProjectiles = 0;
            ID = id;
            deadTimer = 0;
            velocity = new Vector2D(0, 0); // Initial velocity of zero.
            socks = sock;
            loc = _location;
            dir = direction;
            name = _name;
            hp = _hp;
            projectileFiringDelayTimer = _projectileFiringDelay;
        }

        /// <summary>
        /// Returns the state of thrust.
        /// </summary>
        /// <returns></returns>
        public bool getThrust()
        {
            return thrust;
        }

        public double calcAccuracy()
        {
            return (score / totalProjectiles) * 100;
        }

        /// <summary>
        /// Decreases projectile delay timer.
        /// </summary>
        public void decreaseProjectileDelay()
        {
            projectileFiringDelayTimer--;
        }

        /// <summary>
        /// Resets proj delay timer to default.
        /// </summary>
        /// <param name="value"></param>
        public void resetProjectileDelay(int value)
        {
            projectileFiringDelayTimer = value;
        }

        /// <summary>
        /// Sets location of the ship. 
        /// </summary>
        /// <param name="newLocation"></param>
        public void setLocation(Vector2D newLocation)
        {
            loc = newLocation;
        }

        /// <summary>
        /// Decreases HP of the ship.
        /// </summary>
        /// <param name="respawn"></param>
        public void decreaseHP(int respawn)
        {
            hp--;
            if(hp <= 0)
            {
                kill(respawn);
            }
        }

        /// <summary>
        /// Sets the color of the ship. 
        /// </summary>
        /// <param name="col"></param>
        public void setColor(int col)
        {
            color = col;
        }

        /// <summary>
        /// Gets the location of the ship.
        /// </summary>
        /// <returns></returns>
        public Vector2D GetLocation()
        {
            return loc;
        }

        /// <summary>
        /// Gets the current directions of the ship.
        /// </summary>
        /// <returns></returns>
        public Vector2D GetDirections()
        {
            return dir;
        }

        /// <summary>
        /// If hit, decreases HP and if dead, kills the ship.
        /// </summary>
        public void hit()
        {
            hp--;
            if(checkIfDead())
            {
                dead = true;
            }
        }

        /// <summary>
        /// If HP less or == 0, the ship is dead.
        /// </summary>
        /// <returns></returns>
        public bool checkIfDead()
        {
            if(hp <= 0)
            {
                //The ship is dead
                return true;
            }
            return false;
        }

        /// <summary>
        /// Increases score.
        /// </summary>
        public void increaseScore()
        {
            score++;
        }

        public void shoot()
        {
            totalProjectiles++;
        }

        /// <summary>
        /// Allows ship to rotate right.
        /// </summary>
        /// <param name="degreez">How many degrees the ship rotates by.</param>
        public void rotateRight(int degreez)
        {
            //do some vector stuff with the direction

            dir.Rotate(degreez);

        }

        /// <summary>
        /// Allows the ship to rotate left. 
        /// </summary>
        /// <param name="degreez">Degrees to rotate by</param>
        public void rotateLeft(int degreez)
        {
            dir.Rotate(-degreez);
        }

        /// <summary>
        /// Allows the ship to thrust.
        /// </summary>
        public void Thrust()
        {
            Vector2D forwardMotion = new Vector2D(1, 1);
            loc += forwardMotion;
            //do some vector stuff with the direction
        }

        /// <summary>
        /// Kills the ship and sets the deadtimer to the respawn rate.
        /// </summary>
        /// <param name="respawnrate"></param>
        public void kill(int respawnrate)
        {
            dead = true;
            hp = 0;
            deadTimer = respawnrate;
        }

        /// <summary>
        /// Revives the ship when it's time to come back in the game!
        /// </summary>
        /// <param name="worldSize"></param>
        public void frankenstein(int worldSize)
        {
            hp = 5;
            dead = false;
            //set the ship at a new random location.
            Random newLoc = new Random();
            //reset the velocity
            velocity = new Vector2D(0, 0);
            //resent the posion
            loc = new Vector2D(newLoc.Next(-worldSize / 2, worldSize / 2), newLoc.Next(-worldSize / 2, worldSize / 2));
        }

        /// <summary>
        /// Allows the ship to thrust.
        /// </summary>
        /// <param name="v"></param>
        public void setThrust(bool v)
        {
            thrust = v;
        }
    }

    //###################################################################################################################################
    //###################################################################################################################################
    //                                                      Projectile Class
    //###################################################################################################################################
    //###################################################################################################################################

    /// <summary>
    /// Projectile class compatible with JSON. 
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class projectile
    {
        [JsonProperty(PropertyName = "proj")]
        public int ID { get; private set; }

        [JsonProperty]
        public Vector2D loc { get; set; }

        [JsonProperty]
        private Vector2D dir { get; set; }

        [JsonProperty]
        public bool alive { get; private set; }

        [JsonProperty]
        public int owner { get; private set; }

        public Vector2D velocity { get; set; }

        public projectile()
        {
        }
        /// <summary>
        /// Constructor for server use.
        /// </summary>
        /// <param name="_ID"></param>
        /// <param name="owners"></param>
        /// <param name="_loc"></param>
        /// <param name="_dir"></param>
        public projectile(int _ID, int owners, Vector2D _loc, Vector2D _dir)
        {
            velocity = new Vector2D(0, 0);
            owner = owners;
            ID = _ID;
            loc = _loc;
            dir = _dir;
            alive = true;
        }

        /// <summary>
        /// Sets the projectile to dead.
        /// </summary>
        public void kill()
        {
            alive = false;
        }

        /// <summary>
        /// Returns location.
        /// </summary>
        /// <returns></returns>
        public Vector2D GetLocation()
        {
            return loc;
        }

        /// <summary>
        /// Returns directions.
        /// </summary>
        /// <returns></returns>
        public Vector2D GetDirections()
        {
            return dir;
        }


        /// <summary>
        /// Sets a projectile to a new location.
        /// </summary>
        /// <param name="newLocation">new location to be set to.</param>
        public void setLocation(Vector2D newLocation)
        {
            loc = newLocation;
        }
    }
}

//###################################################################################################################################
//###################################################################################################################################
//                                                      World Class
//###################################################################################################################################
//###################################################################################################################################
namespace world
{
    public class World
    {
        /// <summary>
        /// The overarching ships in world.
        /// </summary>
        public ConcurrentDictionary<SocketState, ship> shipsInWorld { get; private set; }
        /// <summary>
        /// Overarching array of projectiles in the world.
        /// </summary>
        public ConcurrentDictionary<int, projectile> projectilesInWorld { get; private set; }
        /// <summary>
        /// Overarching array of stars in the world.
        /// </summary>
        public Dictionary<int, star> starsInWorld { get; private set; }
        int countOfShips { get; set; }
        int countOfProjectil { get; set; }
        int countOfStart { get; set; }

        /// <summary>
        /// Clears / sets variables to starting values (zero)
        /// </summary>
        public World()
        {
            shipsInWorld = new ConcurrentDictionary<SocketState, ship>();
            projectilesInWorld = new ConcurrentDictionary<int, projectile>();
            starsInWorld = new Dictionary<int, star>();
            countOfShips = 0;
            countOfProjectil = 0;
            countOfStart = 0;
       
        }

        /// <summary>
        /// Creates a projectile from passed in ship.
        /// </summary>
        /// <param name="ship_"></param>
        public void createProjectile(ship ship_)
        {
            Vector2D dur = new Vector2D(ship_.dir.GetX(), ship_.dir.GetY());
            projectile project = new projectile(countOfProjectil, ship_.ID, ship_.loc, dur);
             projectilesInWorld.TryAdd(countOfProjectil, project);
            countOfProjectil++;
        }

        /// <summary>
        /// Adds a ship to the world.
        /// </summary>
        /// <param name="ss"></param>
        /// <param name="ships"></param>
        public void AddShip(SocketState ss, ship ships)
        {
            shipsInWorld.TryAdd(ss, ships);
            //shipsInWorld.Add(number, ships);
            countOfShips++;
        }

        /// <summary>
        /// Gets the underlying dictionary of ships in world.
        /// </summary>
        /// <returns></returns>
        public ConcurrentDictionary<SocketState, ship> getShips()
        {
            return this.shipsInWorld;
        }

        /// <summary>
        /// Adds a star to the game world.
        /// </summary>
        /// <param name="size"></param>
        /// <param name="xLocation"></param>
        /// <param name="yLocation"></param>
        public void addStar(double size, int xLocation, int yLocation)
        {
            star st = new star(size, xLocation, yLocation, starsInWorld.Count);
            starsInWorld.Add(countOfStart, st);
            countOfStart++;
        }

        /// <summary>
        /// Detects collisions between stars and ships. 
        /// If a ship hits the star, kills it.
        /// </summary>
        /// <param name="shipSize"></param>
        /// <param name="starSize"></param>
        /// <param name="respawnRate"></param>
        public void detectCollisions(int shipSize, double starSize, int respawnRate)
        {
            //Here we will want to go through every single item and see if it has collided
            //Check if the distance between the two is less than the ship size or the star size

            //Detects collision for the star
            foreach(ship sh in shipsInWorld.Values)
            {
                foreach(star st in starsInWorld.Values)
                {
                    Vector2D holder = (sh.GetLocation() - st.GetLocation());
                    double len = holder.Length();
                    if(len < starSize)
                    {
                        //if the ship is by the star, it will be dead
                        if (sh.dead == false)
                        {
                            //If the ship is not already dead, we will kill it.
                             sh.kill(respawnRate);
                        }

                    }
                }
            }
        }

    }
}
