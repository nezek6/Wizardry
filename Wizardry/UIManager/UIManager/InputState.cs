#region File Description
//-----------------------------------------------------------------------------
// InputState.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace UIManager
{
    /// <summary>
    /// Helper for reading input from keyboard, gamepad, and touch input. This class 
    /// tracks both the current and previous state of the input devices, and implements 
    /// query methods for high level input actions such as "move up through the menu"
    /// or "pause the game".
    /// </summary>
    public class InputState
    {
		/// <summary>
		/// Specifies a particular button found on a mouse.
		/// </summary>
		public enum MouseButtons
		{
			Left,
			Right,
			Middle,
			Side1,
			Side2
		}

		public const int MAX_INPUTS = 4;

		#region Fields

		public readonly KeyboardState[] CurrentKeyboardStates;
        public readonly GamePadState[] CurrentGamePadStates;
		private MouseState currentMouseState;

        public readonly KeyboardState[] LastKeyboardStates;
        public readonly GamePadState[] LastGamePadStates;
		private MouseState lastMouseState;

        public readonly bool[] GamePadWasConnected;

        public TouchCollection TouchState;

        public readonly List<GestureSample> Gestures = new List<GestureSample>();

		#endregion

		#region Accessors

		/// <summary>
		/// Gets the current state of the mouse.
		/// </summary>
		public MouseState CurrentMouseState
		{
			get
			{
				return currentMouseState;
			}
		}


		/// <summary>
		/// Gets the state of the mouse from the previous frame.
		/// </summary>
		public MouseState LastMouseState
		{
			get
			{
				return lastMouseState;
			}
		}

		#endregion

		#region Constructors and Gameloop Methods

		/// <summary>
        /// Constructs a new input state.
        /// </summary>
        public InputState()
        {
            CurrentKeyboardStates = new KeyboardState[MAX_INPUTS];
            CurrentGamePadStates = new GamePadState[MAX_INPUTS];

            LastKeyboardStates = new KeyboardState[MAX_INPUTS];
            LastGamePadStates = new GamePadState[MAX_INPUTS];

            GamePadWasConnected = new bool[MAX_INPUTS];
        }


        /// <summary>
        /// Reads the latest state user input.
        /// </summary>
        public void Update()
        {
			// Track Mouse state
			lastMouseState = currentMouseState;
			currentMouseState = Mouse.GetState();

            for (int i = 0; i < MAX_INPUTS; i++)
            {
                LastKeyboardStates[i] = CurrentKeyboardStates[i];
                LastGamePadStates[i] = CurrentGamePadStates[i];

                CurrentKeyboardStates[i] = Keyboard.GetState((PlayerIndex)i);
                CurrentGamePadStates[i] = GamePad.GetState((PlayerIndex)i);

                // Keep track of whether a gamepad has ever been
                // connected, so we can detect if it is unplugged.
                if (CurrentGamePadStates[i].IsConnected)
                {
                    GamePadWasConnected[i] = true;
                }
            }

            // Get the raw touch state from the TouchPanel
            TouchState = TouchPanel.GetState();

            // Read in any detected gestures into our list for the screens to later process
            Gestures.Clear();
            while (TouchPanel.IsGestureAvailable)
            {
                Gestures.Add(TouchPanel.ReadGesture());
            }
        }

		#endregion

		#region Keyboard Methods

		/// <summary>
        /// Helper for checking if a key was pressed during this update. The
        /// controllingPlayer parameter specifies which player to read input for.
        /// If this is null, it will accept input from any player. When a keypress
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsKeyPressed(Keys key, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;

                return CurrentKeyboardStates[i].IsKeyDown(key);
            }
            else
            {
                // Accept input from any player.
                return (IsKeyPressed(key, PlayerIndex.One, out playerIndex) ||
                        IsKeyPressed(key, PlayerIndex.Two, out playerIndex) ||
                        IsKeyPressed(key, PlayerIndex.Three, out playerIndex) ||
                        IsKeyPressed(key, PlayerIndex.Four, out playerIndex));
            }
        }


		/// <summary>
		/// Helper for checking if a key was newly pressed during this update. The
		/// controllingPlayer parameter specifies which player to read input for.
		/// If this is null, it will accept input from any player. When a keypress
		/// is detected, the output playerIndex reports which player pressed it.
		/// </summary>
		public bool IsNewKeyPress( Keys key, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex )
		{
			if ( controllingPlayer.HasValue )
			{
				// Read input from the specified player.
				playerIndex = controllingPlayer.Value;

				int i = (int)playerIndex;

				return ( CurrentKeyboardStates[i].IsKeyDown( key ) &&
						LastKeyboardStates[i].IsKeyUp( key ) );
			}
			else
			{
				// Accept input from any player.
				return ( IsNewKeyPress( key, PlayerIndex.One, out playerIndex ) ||
						IsNewKeyPress( key, PlayerIndex.Two, out playerIndex ) ||
						IsNewKeyPress( key, PlayerIndex.Three, out playerIndex ) ||
						IsNewKeyPress( key, PlayerIndex.Four, out playerIndex ) );
			}
		}

		#endregion

		#region Gamepad Methods

		/// <summary>
        /// Helper for checking if a button was pressed during this update.
        /// The controllingPlayer parameter specifies which player to read input for.
        /// If this is null, it will accept input from any player. When a button press
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsButtonPressed(Buttons button, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;

                return CurrentGamePadStates[i].IsButtonDown(button);
            }
            else
            {
                // Accept input from any player.
                return (IsButtonPressed(button, PlayerIndex.One, out playerIndex) ||
                        IsButtonPressed(button, PlayerIndex.Two, out playerIndex) ||
                        IsButtonPressed(button, PlayerIndex.Three, out playerIndex) ||
                        IsButtonPressed(button, PlayerIndex.Four, out playerIndex));
            }
        }


		/// <summary>
		/// Helper for checking if a button was newly pressed during this update.
		/// The controllingPlayer parameter specifies which player to read input for.
		/// If this is null, it will accept input from any player. When a button press
		/// is detected, the output playerIndex reports which player pressed it.
		/// </summary>
		public bool IsNewButtonPress( Buttons button, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex )
		{
			if ( controllingPlayer.HasValue )
			{
				// Read input from the specified player.
				playerIndex = controllingPlayer.Value;

				int i = (int)playerIndex;

				return ( CurrentGamePadStates[i].IsButtonDown( button ) &&
						LastGamePadStates[i].IsButtonUp( button ) );
			}
			else
			{
				// Accept input from any player.
				return ( IsNewButtonPress( button, PlayerIndex.One, out playerIndex ) ||
						IsNewButtonPress( button, PlayerIndex.Two, out playerIndex ) ||
						IsNewButtonPress( button, PlayerIndex.Three, out playerIndex ) ||
						IsNewButtonPress( button, PlayerIndex.Four, out playerIndex ) );
			}
		}

		#endregion

		#region Mouse Methods

		/// <summary>
		/// Helper for checking if a given Mouse Button is pressed during this update.
		/// </summary>
		/// <param name="button">The button the check.</param>
		/// <returns>True if the button is pressed and false otherwise.</returns>
		public bool IsMousePressed( MouseButtons button )
		{
			bool result = false;

			switch ( button )
			{
				case MouseButtons.Left:
					result = ( currentMouseState.LeftButton == ButtonState.Pressed );
					break;
				case MouseButtons.Right:
					result = ( currentMouseState.RightButton == ButtonState.Pressed );
					break;
				case MouseButtons.Middle:
					result = ( currentMouseState.MiddleButton == ButtonState.Pressed );
					break;
				case MouseButtons.Side1:
					result = ( currentMouseState.XButton1 == ButtonState.Pressed );
					break;
				case MouseButtons.Side2:
					result = ( currentMouseState.XButton2 == ButtonState.Pressed );
					break;
			}

			return result;
		}


		/// <summary>
		/// Helper for checking if a mouse button is newly pressed within this update.
		/// </summary>
		/// <param name="button">The button the check.</param>
		/// <returns>True if the given mouse button is newly pressed and false otherwise.</returns>
		public bool IsNewMousePress( MouseButtons button )
		{
			bool result = false;

			switch ( button )
			{
				case MouseButtons.Left:
					result = ( ( currentMouseState.LeftButton == ButtonState.Pressed ) &&
						       ( lastMouseState.LeftButton == ButtonState.Released ) );
					break;
				case MouseButtons.Right:
					result = ( ( currentMouseState.RightButton == ButtonState.Pressed ) &&
							   ( lastMouseState.RightButton == ButtonState.Released ) );
					break;
				case MouseButtons.Middle:
					result = ( ( currentMouseState.MiddleButton == ButtonState.Pressed ) &&
							   ( lastMouseState.MiddleButton == ButtonState.Released ) );
					break;
				case MouseButtons.Side1:
					result = ( ( currentMouseState.XButton1 == ButtonState.Pressed ) &&
							   ( lastMouseState.XButton1 == ButtonState.Released ) );
					break;
				case MouseButtons.Side2:
					result = ( ( currentMouseState.XButton2 == ButtonState.Pressed ) &&
							   ( lastMouseState.XButton2 == ButtonState.Released ) );
					break;
			}

			return result;
		}


		/// <summary>
		/// Retrieves the position of the cursor in the previous frame.
		/// </summary>
		/// <returns>The position of the cursor in the previous frame.</returns>
		public Point LastCursorPosition()
		{
			return new Point( lastMouseState.X, lastMouseState.Y );
		}


		/// <summary>
		/// Retrieves the current position of the cursor.
		/// </summary>
		/// <returns>The position of the cursor.</returns>
		public Point CurrentCursorPosition()
		{
			return new Point( currentMouseState.X, currentMouseState.Y );
		}


		/// <summary>
		/// Helper which determines how much the cursor moved between the current frame and the previous frame.
		/// </summary>
		/// <returns>The amount the cursor moved between the current and previous frame.</returns>
		public Point CursorPositionDelta()
		{
			int x = currentMouseState.X - lastMouseState.X;
			int y = currentMouseState.Y - lastMouseState.Y;
			return new Point( x, y );
		}

		#endregion
    }
}
