using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace InternetGame
{
    //Denna klass skickar inputs till servern så att servern kan avgöra vad som ska göras med dem
    class InputManager
    {
        private NetworkConnection networkConnection;

        public InputManager(NetworkConnection _networkConnection)
        {
            networkConnection = _networkConnection;
        }

        public void Update(double gameTime)
        {
            var state = Keyboard.GetState();
            CheckKeyState(Keys.Down, state);
            CheckKeyState(Keys.Up, state);
            CheckKeyState(Keys.Left, state);
            CheckKeyState(Keys.Right, state);
    
        }
        //Om en key är nere så skickas det till serven via SendInput i NetworkConnection, en egen skapad klass.
        private void CheckKeyState(Keys key, KeyboardState state)
        {
            if(state.IsKeyDown(key))
            {
                networkConnection.SendInput(key);
            }
        }
    }
}
