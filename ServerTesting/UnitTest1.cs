using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using world;
using controller;
using System.Net.Sockets;
using System.Net;
using SpaceWars;
using Server;

namespace ServerTesting
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void CreateWorld()
        {
            World world = new World();
            Socket socket;
            IPAddress ip;

            networking.MakeSocket("localhost", out socket, out ip);
            SocketState ss = new SocketState(socket);
            ship sh = new ship(1, ss, 5, new Vector2D(8, 8), "JoCee", 0, new Vector2D(8, 9), 7);
            world.AddShip(ss, sh);
            Assert.AreEqual(1, world.shipsInWorld.Count);
        }


        [TestMethod]
        public void Addstar()
        {
            World world = new World();
            Socket socket;
            IPAddress ip;

            networking.MakeSocket("localhost", out socket, out ip);
            SocketState ss = new SocketState(socket);
            ship sh = new ship(1, ss, 5, new Vector2D(8, 8), "JoCee", 0, new Vector2D(8, 9), 7);
            world.AddShip(ss, sh);
            world.addStar(0.3, 0, 0);
            Assert.AreEqual(1, world.starsInWorld.Count);
        }

        [TestMethod]
        public void hit()
        {
            int hp = 5;
            World world = new World();
            
            Socket socket;
            IPAddress ip;
            
            networking.MakeSocket("localhost", out socket, out ip);
            SocketState ss = new SocketState(socket);
            ship sh = new ship(1,ss, 5, new Vector2D(8,8), "JoCee", 0, new Vector2D(8, 9), 7);
            
            sh.hit();
            Assert.AreEqual(4, sh.hp);
        }
        [TestMethod]
        public void decreaseHp()
        {
            World world = new World();
            Socket socket;
            IPAddress ip;

            networking.MakeSocket("localhost", out socket, out ip);
            SocketState ss = new SocketState(socket);
            ship sh = new ship(1, ss, 5, new Vector2D(8, 8), "JoCee", 0, new Vector2D(8, 9), 7);

            sh.decreaseHP(6);
            Assert.AreEqual(4, sh.hp);
        }

        [TestMethod]
        public void kill()
        {
            World world = new World();
            Socket socket;
            IPAddress ip;

            networking.MakeSocket("localhost", out socket, out ip);
            SocketState ss = new SocketState(socket);
            ship sh = new ship(1, ss, 5, new Vector2D(8, 8), "JoCee", 0, new Vector2D(8, 9), 7);
            sh.kill(30);
            Assert.AreEqual(true, sh.dead);
        }

        [TestMethod]
        public void frankenstein()
        {
            World world = new World();
            Socket socket;
            IPAddress ip;

            networking.MakeSocket("localhost", out socket, out ip);
            SocketState ss = new SocketState(socket);
            ship sh = new ship(1, ss, 5, new Vector2D(8, 8), "JoCee", 0, new Vector2D(8, 9), 7);
            sh.kill(30);
            sh.frankenstein(750);
            Assert.AreEqual(false, sh.dead);
        }

        [TestMethod]
        public void updateShipLocation()
        {
            //myServe ser = new myServe();
            
            World world = new World();
            Socket socket;
            IPAddress ip;

            networking.MakeSocket("localhost", out socket, out ip);
            SocketState ss = new SocketState(socket);
            ship sh = new ship(1, ss, 5, new Vector2D(8, 8), "JoCee", 0, new Vector2D(8, 9), 7);
            sh.kill(30);
            Assert.AreEqual(true, sh.dead);
        }

        [TestMethod]
        public void collisionStarTesting()
        {
            World world = new World();
            Socket socket;
            IPAddress ip;

            networking.MakeSocket("localhost", out socket, out ip);
            SocketState ss = new SocketState(socket);
            ship sh = new ship(1, ss, 5, new Vector2D(8, 8), "JoCee", 0, new Vector2D(8, 9), 7);
             world.addStar(30, 7, 7);
            world.detectCollisions(30, 30, 30);
            Assert.AreEqual(false, sh.dead == true);
        }
        [TestMethod]
        public void projectileFiringTesterTesting()
        {
            World world = new World();
            Socket socket;
            IPAddress ip;

            networking.MakeSocket("localhost", out socket, out ip);
            SocketState ss = new SocketState(socket);
            ship sh = new ship(1, ss, 5, new Vector2D(8, 8), "JoCee", 0, new Vector2D(8, 9), 7);
            world.AddShip(ss, sh);
            sh.decreaseProjectileDelay();
            world.addStar(30, 7, 7);
            world.detectCollisions(30, 30, 30);
            Assert.AreEqual(6, sh.projectileFiringDelayTimer);
        }
        [TestMethod]
        public void creatingAProjectile()
        {
            World world = new World();
            Socket socket;
            IPAddress ip;

            networking.MakeSocket("localhost", out socket, out ip);
            SocketState ss = new SocketState(socket);
            ship sh = new ship(1, ss, 5, new Vector2D(8, 8), "JoCee", 0, new Vector2D(8, 9), 7);
            world.AddShip(ss, sh);
            sh.decreaseProjectileDelay();
            world.addStar(30, 7, 7);
            world.detectCollisions(30, 30, 30);
            myServe serving = new myServe();
            world.createProjectile(sh);
            world.createProjectile(sh);
            ship shCrash = new ship(1, ss, 5, new Vector2D(8, 8), "JoCee", 0, new Vector2D(8, 9), 7);
            serving.checkShipProjCollision();

        }
        [TestMethod]
        public void multipleProjectiles()
        {
            
                World world = new World();
                Socket socket;
                IPAddress ip;

                networking.MakeSocket("localhost", out socket, out ip);
                SocketState ss = new SocketState(socket);
                ship sh = new ship(1, ss, 5, new Vector2D(8, 8), "JoCee", 0, new Vector2D(8, 9), 7);
                world.AddShip(ss, sh);
                sh.decreaseProjectileDelay();
                world.addStar(30, 7, 7);
                world.detectCollisions(30, 30, 30);
                myServe serving = new myServe();
                world.createProjectile(sh);
                world.createProjectile(sh);
                ship shCrash = new ship(1, ss, 5, new Vector2D(8, 8), "JoCee", 0, new Vector2D(8, 9), 7);
                serving.checkShipProjCollision();

            Assert.AreEqual(2, world.projectilesInWorld.Count);

        }
        [TestMethod]
        public void GetDirection()
        {

            World world = new World();
            Socket socket;
            IPAddress ip;

            networking.MakeSocket("localhost", out socket, out ip);
            SocketState ss = new SocketState(socket);
            ship sh = new ship(1, ss, 5, new Vector2D(8, 8), "JoCee", 0, new Vector2D(8, 9), 7);
            
            Assert.AreEqual(sh.GetDirections().GetX(), 8);

        }
        [TestMethod]
        public void GetDirectionY()
        {

            World world = new World();
            Socket socket;
            IPAddress ip;

            networking.MakeSocket("localhost", out socket, out ip);
            SocketState ss = new SocketState(socket);
            ship sh = new ship(1, ss, 5, new Vector2D(8, 8), "JoCee", 0, new Vector2D(8, 9), 7);

            Assert.AreEqual(sh.GetDirections().GetY(), 9);

        }
        [TestMethod]
        public void GetLocationX()
        {

            World world = new World();
            Socket socket;
            IPAddress ip;

            networking.MakeSocket("localhost", out socket, out ip);
            SocketState ss = new SocketState(socket);
            ship sh = new ship(1, ss, 5, new Vector2D(8, 8), "JoCee", 0, new Vector2D(9, 9), 7);

            Assert.AreEqual(sh.GetLocation().GetX(), 8);

        }
        [TestMethod]
        public void GetLocationY()
        {

            World world = new World();
            Socket socket;
            IPAddress ip;

            networking.MakeSocket("localhost", out socket, out ip);
            SocketState ss = new SocketState(socket);
            ship sh = new ship(1, ss, 5, new Vector2D(8, 8), "JoCee", 0, new Vector2D(9, 9), 7);

            Assert.AreEqual(sh.GetLocation().GetY(), 8);

        }

        [TestMethod]
        public void ShipThrust()
        {
            World world = new World();
            Socket socket;
            IPAddress ip;

            networking.MakeSocket("localhost", out socket, out ip);
            SocketState ss = new SocketState(socket);
            ship sh = new ship(1, ss, 5, new Vector2D(8, 8), "JoCee", 0, new Vector2D(9, 9), 7);
            sh.getThrust();
            Assert.AreEqual(false, sh.getThrust());
        }

        [TestMethod]
        public void ShipThrusting()
        {
            World world = new World();
            Socket socket;
            IPAddress ip;

            networking.MakeSocket("localhost", out socket, out ip);
            SocketState ss = new SocketState(socket);
            ship sh = new ship(1, ss, 5, new Vector2D(8, 8), "JoCee", 0, new Vector2D(9, 9), 7);
            sh.Thrust();

            Assert.AreNotEqual(8, sh.loc.GetX());
        }

        [TestMethod]
        public void ShipThrustingTrue()
        {
            World world = new World();
            Socket socket;
            IPAddress ip;

            networking.MakeSocket("localhost", out socket, out ip);
            SocketState ss = new SocketState(socket);
            ship sh = new ship(1, ss, 5, new Vector2D(8, 8), "JoCee", 0, new Vector2D(9, 9), 7);
            sh.Thrust();

        }


        [TestMethod]
        public void StarID()
        {
            World world = new World();
            Socket socket;
            IPAddress ip;

            networking.MakeSocket("localhost", out socket, out ip);
            SocketState ss = new SocketState(socket);
            star sh = new star( 5.0, 8, 8,1);
            

            Assert.AreEqual(1, sh.ID);
        }

        [TestMethod]
        public void starMass()
        {
            World world = new World();
            Socket socket;
            IPAddress ip;

            networking.MakeSocket("localhost", out socket, out ip);
            SocketState ss = new SocketState(socket);
            star sh = new star(5.0, 8, 8, 1);


            Assert.AreEqual(5.0, sh.mass);
        }


        [TestMethod]
        public void starXLocation()
        {
            World world = new World();
            Socket socket;
            IPAddress ip;

            networking.MakeSocket("localhost", out socket, out ip);
            SocketState ss = new SocketState(socket);
            star sh = new star(5.0, 8, 8, 1);


            Assert.AreEqual(8, sh.loc.GetX());
        }

        [TestMethod]
        public void starYLocation()
        {
            World world = new World();
            Socket socket;
            IPAddress ip;

            networking.MakeSocket("localhost", out socket, out ip);
            SocketState ss = new SocketState(socket);
            star sh = new star(5.0, 8, 8, 1);


            Assert.AreEqual(8, sh.loc.GetY());
        }

        [TestMethod]
        public void  ResetProjectileDelay()
        {
            World world = new World();
            Socket socket;
            IPAddress ip;

            networking.MakeSocket("localhost", out socket, out ip);
            SocketState ss = new SocketState(socket);
            ship sh = new ship(1, ss, 5, new Vector2D(8, 8), "JoCee", 0, new Vector2D(9, 9), 7);
            sh.resetProjectileDelay(8);
            sh.decreaseProjectileDelay();
            Assert.AreEqual(7, sh.projectileFiringDelayTimer);
        }

        [TestMethod]
        public void RotateRight()
        {
            World world = new World();
            Socket socket;
            IPAddress ip;

            networking.MakeSocket("localhost", out socket, out ip);
            SocketState ss = new SocketState(socket);
            ship sh = new ship(1, ss, 5, new Vector2D(8, 8), "JoCee", 0, new Vector2D(9, 9), 7);
            sh.resetProjectileDelay(8);
            sh.decreaseProjectileDelay();
            sh.rotateRight(9);
            Assert.AreNotEqual(9, sh.dir.GetX());
        }


        [TestMethod]
        public void RotateLeft()
        {
            World world = new World();
            Socket socket;
            IPAddress ip;

            networking.MakeSocket("localhost", out socket, out ip);
            SocketState ss = new SocketState(socket);
            ship sh = new ship(1, ss, 5, new Vector2D(8, 8), "JoCee", 0, new Vector2D(9, 9), 7);
            sh.resetProjectileDelay(8);
            sh.decreaseProjectileDelay();
            sh.rotateLeft(9);
            Assert.AreNotEqual(9, sh.dir.GetX());
        }

        [TestMethod]
        public void GetScore()
        {
            World world = new World();
            Socket socket;
            IPAddress ip;

            networking.MakeSocket("localhost", out socket, out ip);
            SocketState ss = new SocketState(socket);
            ship sh = new ship(1, ss, 5, new Vector2D(8, 8), "JoCee", 0, new Vector2D(8, 9), 7);
            world.AddShip(ss, sh);
            sh.setColor(1);
            sh.decreaseProjectileDelay();
            sh.increaseScore();
            Assert.AreEqual(1, sh.score);
        }
        [TestMethod]
        public void SetColor()
        {
            World world = new World();
            Socket socket;
            IPAddress ip;

            networking.MakeSocket("localhost", out socket, out ip);
            SocketState ss = new SocketState(socket);
            ship sh = new ship(1, ss, 5, new Vector2D(8, 8), "JoCee", 0, new Vector2D(8, 9), 7);
            world.AddShip(ss, sh);
            sh.setColor(1);
            sh.decreaseProjectileDelay();
            Assert.AreEqual(0, sh.deadTimer);
        }


    }
}
