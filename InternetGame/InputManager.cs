using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace InternetGame
{
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
        private void CheckKeyState(Keys key, KeyboardState state)
        {
            if(state.IsKeyDown(key))
            {
                networkConnection.SendInput(key);
            }
        }
    }
}
