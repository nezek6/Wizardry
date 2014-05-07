using System;
using Microsoft.Xna.Framework;
using MouseButtons = UIManager.InputState.MouseButtons;

namespace UIManager
{
	/// <summary>
	/// Defines an specialized action type that the user performs with the mouse.
	/// <see cref="UIManager.InputAction"/>
	/// </summary>
	public class MouseInputAction
	{
		/// <summary>
		/// Specifies the possible cursor actions over a given area.
		/// </summary>
		public enum MouseCursorAction
		{
			Enter,
			Hover,
			Leave
		}

		// Tracks whether the action is a button action (true) or a cursor action (false).
		private bool isBtnAction;

		// Whether to only consider new button presses. Only applies if the action is a button action.
		private bool newPressOnly;

		// The button tied to the action (applies to button press actions only).
		private MouseButtons button;

		// The cursor action tied to the action (applies to cursor actions only).
		private MouseCursorAction cursorAction;

		// The hotzone (i.e. active area) for the cursor action (applies to cursor actions only).
		private Rectangle hotZone;

		/// <summary>
		/// Constructor which creates an input action for when a mouse button is pressed.
		/// </summary>
		/// <param name="button">The button to tie the action to.</param>
		/// <param name="newPressOnly">Whether or not to flag the action only when the button is first pressed.</param>
		public MouseInputAction( MouseButtons button, bool newPressOnly )
		{
			isBtnAction = true;
			this.button = button;
			this.newPressOnly = newPressOnly;
		}


		/// <summary>
		/// Constructor which creates an input action for mouse cursor movement.
		/// </summary>
		/// <param name="cursorAction">The type of cursor activity to tie the action to.</param>
		/// <param name="hotZone">The area of interest for the cursor action.</param>
		public MouseInputAction( MouseCursorAction cursorAction, Rectangle hotZone )
		{
			isBtnAction = false;
			this.cursorAction = cursorAction;
			this.hotZone = hotZone;
		}


		/// <summary>
		/// Evaluates the action against a given InputState.
		/// </summary>
		/// <param name="inState">The InputState to test for the action.</param>
		/// <returns>True if the action occurred and false otherwise.</returns>
		public bool Evaluate( InputState inState )
		{
			bool result = false;

			if ( isBtnAction )
			{
				// The action corresponds to a mouse button press
				if ( newPressOnly )
				{
					result = inState.IsNewMousePress( button );
				}
				else
				{
					result = inState.IsMousePressed( button );
				}
			}
			else
			{
				// The action corresponds to a mouse cursor action
				Point curCursor = inState.CurrentCursorPosition();	// Position of the cursor on current frame
				Point prevCursor = inState.LastCursorPosition();	// Position of the cursor on the last frame

				switch ( cursorAction )
				{
					case MouseCursorAction.Enter:
						result = ( !hotZone.Contains( prevCursor ) && hotZone.Contains( curCursor ) );
						break;
					case MouseCursorAction.Hover:
						result = hotZone.Contains( curCursor );
						break;
					case MouseCursorAction.Leave:
						result = ( hotZone.Contains( prevCursor ) && !hotZone.Contains( curCursor ) );
						break;
				}
			}

			return result;
		}
	}
}
