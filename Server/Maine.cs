using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Maine
    {
        public delegate void read(myServe s);
        
        static void Main(string[] args)
        {
            myServe serving = new myServe();
            //We will start the client Loop
            serving.startConnection();
            

            
        }
        static public void readLine(myServe server)
        {
            while (true) {
                string s = Console.ReadLine();
                if (s.Equals("exit"))
                {
                    Console.WriteLine("exit");
                    server.save();
                }
            }
        }
    }
}
