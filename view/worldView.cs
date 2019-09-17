using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using controller;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using SpaceWars;
using Newtonsoft.Json;
using System.Threading;
using System.Timers;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace view
{
    public partial class worldView : Form
    {
        public Socket theServer;
        public SocketState sockMonkey;
        public drawingPanel worldPanel;
        scoreBoardPanel scores;
        public int playerColorAssign = 0;
        

        /// <summary>
        /// Dictionaries for every type of object possible in the world, so the rest of the program can access.
        /// The ID of the object is the key, and the object itself is the value.
        /// </summary>
        public Dictionary<int, object> itemsInWorld  { get; private set; }
        public Dictionary<int, ship> playersInWorld { get; private set; }
        public Dictionary<int, star> starsInWorld { get; private set; }
        public Dictionary<int, projectile> projectilesInWorld { get; private set; }
        
        private int playernumber { get; set; }
        int worldSize { get; set; }
        private bool right { get; set; }
        private bool left { get; set; }
        private bool thrust { get; set; }
        private bool fire { get; set; }

        /// <summary>
        /// Initializes the worldView. 
        /// DoubleBuffering needs to be enabled!
        /// Sets the location of the panel on the overarching screen. 
        /// </summary>
        public worldView()
        {
            InitializeComponent();
            //Can we add a location like have it on the left of the window?
            worldPanel = new drawingPanel(this);
            worldPanel.Location = new Point(0, 27);
            worldPanel.Size = new Size(30, 30);

            //Set the BG color to black. Because space is, objectively, black. 
            //Or, rather, a lack of other colors leads to the appearance of black 
            //   as the color of space. 
            worldPanel.BackColor = System.Drawing.Color.Black;

            //uber important. We want to be able to control the panel with the main form. 
            this.Controls.Add(worldPanel);



            //This is the scoreboard panel. Sets it up similarly to the way the worldView panel is setup above.
            scores = new scoreBoardPanel(this);
            scores.Location = new Point(300, 27);
            scores.Size = new Size(150, 750);
            //scores.BackColor = System.Drawing.Color.LightSteelBlue;
            this.Controls.Add(scores);
            playersInWorld = new Dictionary<int, ship>();

            // Start a new timer that will redraw the game every 15 milliseconds 
            // This should correspond to about 67 frames per second.
            System.Timers.Timer frameTimer = new System.Timers.Timer();
            frameTimer.Interval = 15;
            frameTimer.Elapsed += Redraw;
            frameTimer.Start();

            // Sets the dictionaries to be empty upon startup.
            itemsInWorld = new Dictionary<int, object>();
            playersInWorld = new Dictionary<int, ship>();
            projectilesInWorld = new Dictionary<int, projectile>();
            starsInWorld = new Dictionary<int, star>();

        }

        /// <summary>
        /// Redraw the game. Invoked every time the "Frame timer" ticks. Invalidates the form 
        /// so the OnPaint method is invoked. Necessary to draw things.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Redraw(object sender, ElapsedEventArgs e)
        {
            //Invalidate the form and all of its children. 
            //Will cause the form to update as soon as it can. 
            //do we need to create a new invalidate each time?
            MethodInvoker invalidate = new MethodInvoker(() => { this.Invalidate(true); });
            try { this.Invoke(invalidate); }
            catch { } // Catch to be rid of ObjectDisposedExceptions upon invalidation of the form. 
        }


        /// <summary>
        /// Gets the list of commands and sends it using the Network function. 
        /// </summary>
        public void sendData()
        {
            string command = getListOfCommands();

            networking.Send(sockMonkey, command);
        }


        /// <summary>
        /// When the connect button is clicked, disable everything and start the game!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void connectButton_Click(object sender, EventArgs e)
        {
            // If hostname box is empty, gives a message. 
            if (AddressTextbox.Text == "")
            {
                MessageBox.Show("Ain't no DHCP without literally any info.");
                return;
            }

            if (NameTextbox.Text == "")
            {
                MessageBox.Show("'Error. I didn't quite catch that.' \nWhat's your name, again?");
                return;
            }

            //Disable controls; try and connect to host. 
            connectButton.Enabled = false;
            AddressTextbox.Enabled = false;
            NameTextbox.Enabled = false;

            //Connect to server. First thing we want to do once connection is made is FirstContact. 
            theServer = networking.ConnectToServer(firstContact, AddressTextbox.Text);


        }

        /// <summary>
        /// Invoked upon first contact from the server. Sends the name. 
        /// </summary>
        /// <param name="istate"></param>
        private void firstContact(SocketState istate)
        {
            //We will get the  socket that we just created so we can send data to the samw socket
            sockMonkey = istate;
            //Sends player name to the server.
            networking.Send(istate, NameTextbox.Text.TrimEnd('\0'));
            istate.Call = ReceiveStartup;

            networking.GetData(istate);
        }

        /// <summary>
        /// This handles the final step of the hanshake. Receives the setup instructions from the server. 
        /// Player ID as well as world size.
        /// </summary>
        /// <param name="state"></param>
        private void ReceiveStartup(SocketState state)
        {
            //Gets the Player ID and the world size out of state.sb

            string totalData = state.bs.ToString();
            string[] parts = Regex.Split(totalData, @"(?<=[\n])");

            int counter = 0;
            //go throught the entire message
            foreach (String p in parts)
            {
                if (p.Length == 0)
                    continue;

                if (p[p.Length - 1] != '\n')
                    break;

                if(counter == 0)
                {
                    ///this is the player id.
                    this.playernumber = Int32.Parse(p);
                    counter ++;
                }
                else
                {
                    try
                    {
                        int size = Int32.Parse(p);

                    MethodInvoker updateWorldSize = new MethodInvoker(
                        () => { this.worldSize = size; });
                    //changing the scoreboard so that it is on the right of the game.

                    this.Invoke(updateWorldSize);
                    }
                    catch
                    {
                        MessageBox.Show("Error connecting, it happens, just restart the program and try again!!");
                    }

                    MethodInvoker updateScoreBoardLocation = new MethodInvoker(
                    () => { this.scores.Location = new Point(this.worldSize + 3, 27); });
                    this.Invoke(updateScoreBoardLocation);
                }

                byte[] messageBytes = Encoding.UTF8.GetBytes(p);

                state.bs.Remove(0, p.Length); // Remove the (now) unnecessary data from the string buider.
            }

            //Updates the action to take when network event happens. 
            state.Call = processMessage;

            MethodInvoker updatePanelWorldSize = new MethodInvoker(
                () =>
                {
                    worldPanel.Size = new Size(this.worldSize, this.worldSize); // Updates the world size for the shooter. Window size reformats dependent. 
                });
            this.Invoke(updatePanelWorldSize);

            //Start waiting for data.
            networking.GetData(state);
        }
        
        /// <summary>
        /// Processes incoming data to blit on the screen.
        /// Adds items to overarching item dictionaries if they aren't there
        /// If they are there, updates the entries. 
        /// </summary>
        /// <param name="ss"></param>
        public  void processMessage(SocketState ss)
        {
            playerColorAssign = 0;
            string totalData = ss.bs.ToString();
            string[] parts = Regex.Split(totalData, @"(?<=[\n])");

            //go throught the entire message
            foreach (String p in parts)
            {
               
                if (p.Length == 0)
                    continue;

                if (p[p.Length - 1] != '\n')
                    break;
                ss.bs.Remove(0, p.Length); // Removes the now unnecessary data fromt the StringBuilder.

                //for each object we want to determine what it is.
                //The one is message bites is the one we want
                lock (this)
                {
                    JObject obj = JObject.Parse(p);


                    JToken token = obj["star"];
                    if (token != null)
                    {
                        //we know it's a star since only stars have mass
                        star rebuilt = JsonConvert.DeserializeObject<star>(obj.ToString());
                        if (!starsInWorld.ContainsKey(rebuilt.ID))
                        {
                            //we want to add it to the dictionary.
                            starsInWorld.Add(rebuilt.ID, rebuilt);
                        }
                        else
                        {
                            //update items with new information.
                            starsInWorld[rebuilt.ID] = rebuilt;
                        }

                        continue; // Skips past the rest of the things in the loop.
                    }
                    token = obj["proj"];
                    if (token != null)
                    {
                        //we know it's a projectile
                        projectile rebuilt = JsonConvert.DeserializeObject<projectile>(obj.ToString());
                        if (!projectilesInWorld.ContainsKey(rebuilt.ID))
                        {
                            //we want to add it to the dictionary.
                            projectilesInWorld.Add(rebuilt.ID, rebuilt);
                        }
                        else
                        {
                            //update items with new information.
                            projectilesInWorld[rebuilt.ID] = rebuilt;
                        }

                        continue;
                    }


                    token = obj["ship"];
                    if (token != null)
                    {
                        
                        //we know it's a projectile
                        ship rebuilt = JsonConvert.DeserializeObject<ship>(obj.ToString());
                        if (!playersInWorld.ContainsKey(rebuilt.ID))
                        {
                            //we want to add it to the dictionary.
                            //we will also give the ship a player color at this time.
                            rebuilt.setColor(rebuilt.hp % 8);
                            playersInWorld.Add(rebuilt.ID, rebuilt);
                            //increase the player color assign.
                            
                        }
                        else
                        {
                            
                            //update items with new information.
                            playersInWorld[rebuilt.ID] = rebuilt;
                        }
                        continue;
                    }

                }
            }
                networking.GetData(ss); // And asks for more data when we're done processing! 
            
        }

        private void worldView_Load(object sender, EventArgs e)
        {

        }

        private void scoreBoardPanel_Paint(object sender, PaintEventArgs e)
        {
        }

        /// <summary>
        /// When a key's down, edits a private bool variable to true. That variable is set back to false if the key's up. 
        /// That can be used to construct a string to send to the server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void worldView_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Right)
            {
                //We want to send a right command.
                //A command is a '\n' terminated string containing one or more single-letter commands enclosed in parentheses
                //I am just going to set a boolean, and then if the boolean is true, it will add it to a string,
                //and each second, the string will be processed with the list of commands.
                right = true;
            }
            if (e.KeyCode == Keys.Left)
            {
                left = true;
            }
            if(e.KeyCode == Keys.Up)
            {
                thrust = true;
            }
            if(e.KeyCode == Keys.Space)
            {
                fire = true;
            }
            //whenever a key is pressed, we want to send that and any combination of pressed keys to the server.
            //This is where our error is.
            sendData();
        }


        /// <summary>
        /// When a key is released, updates the private bool so it'll no longer be 
        /// sent to the server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void worldView_KeyUp(object sender, KeyEventArgs e)
        {
            //This isn't working TODO: TA
            if (e.KeyCode == Keys.Right)
            {
                right = false;
            }
            if (e.KeyCode == Keys.Left)
            {
                left = false;
            }
            if (e.KeyCode == Keys.Up)
            {
                thrust = false;
            }
            if (e.KeyCode == Keys.Space)
            {
                fire = false;
            }

        }

        /// <summary>
        /// Returns a string of all the buttons currently being held down. 
        /// Used to send that data to the server.
        /// </summary>
        /// <returns></returns>
        private string getListOfCommands()
        {
            string holder = "";
            if(right == true)
            {
                holder += "R";
            }
            if(left == true)
            {
                holder += "L";
            }
            if(thrust == true)
            {
                holder += "T";
            }
            if(fire == true)
            {
                holder += "F";
            }
            return "(" + holder + ")\n";
        }

        private void WorldPanel_Paint(object sender, PaintEventArgs e)
        {
            this.OnPaint(e);
        }

        private void AddressTextbox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
