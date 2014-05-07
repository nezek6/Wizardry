using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UIManager.Controls
{
	public class Button : Control
	{
		#region Data Members

		// The button state:
		//   0 = inert
		//   1 = hover  (mouse over)
		//   2 = primed (mouse over + button down)
		private int state;

		// The area covered by the button
		private Rectangle area;

		// Input action for when the left button is pressed down (new presses only)
		private MouseInputAction mouseButtonDown;

		// Input action denoting whether the mouse button is held down
		private MouseInputAction mouseButtonHeld;

		// Input action for when the cursor is over the button area
		private MouseInputAction cursorOver;

		// The texure of the Button
		private Texture2D texture = null;

		// The width of a frame of the texture
		private int texFrameWidth;

		// The height of a frame of the texture
		private int texFrameHeight;

		// The button's text
		private string text = "";

		// The text's font
		private SpriteFont textFont = null;

		// The font colours for the different states
		private Color[] textColours;

		#endregion

		#region Initialization

		/// <summary>
		/// Button class constructor.
		/// </summary>
		/// <param name="position">The top-left corner of the Button.</param>
		/// <param name="width">The Button's width.</param>
		/// <param name="height">The Button's height.</param>
		/// <param name="screen">Reference to the screen on which the Button exists.</param>
		public Button( Point position, int width, int height, GameScreen screen )
			: base( position, width, height, screen )
		{
			// Starting state is inert
			state = 0;
			textColours = new Color[4];
		}

		/// <summary>
		/// Sets up the Buttons user input actions. This is called automatically when
		/// the Button is created.
		/// </summary>
		protected override void InitInputs()
		{
			area = new Rectangle( Position.X, Position.Y, width, height );
			mouseButtonDown = new MouseInputAction( InputState.MouseButtons.Left, true );
			mouseButtonHeld = new MouseInputAction( InputState.MouseButtons.Left, false );
			cursorOver = new MouseInputAction( MouseInputAction.MouseCursorAction.Hover, area );
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the Button's On-Screen position.
		/// </summary>
		public override Point Position
		{
			set
			{
				base.Position = value;
				InitInputs();
			}
		}

		/// <summary>
		/// Gets or sets the Button's height.
		/// </summary>
		public override int Height
		{
			set
			{
				base.Height = value;
				InitInputs();
			}
		}
		
		/// <summary>
		/// Gets or sets the Button's width.
		/// </summary>
		public override int Width
		{
			set
			{
				base.Width = value;
				InitInputs();
			}
		}

		/// <summary>
		/// Gets or sets the Button's texture.
		/// </summary>
		public Texture2D Texture
		{
			get
			{
				return texture;
			}
			set
			{
				texture = value;
				texFrameHeight = texture.Height;
				texFrameWidth = (texture.Width >> 2);	// Since there are 4 frames (one for each state)
			}
		}

		/// <summary>
		/// Gets or sets the Button's text.
		/// </summary>
		public string Text
		{
			get
			{
				return text;
			}
			set
			{
				text = value;
			}
		}

		/// <summary>
		/// Gets the Button's font.
		/// </summary>
		public SpriteFont Font
		{
			get
			{
				return textFont;
			}
			internal set
			{
				textFont = value;
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Sets the Button text's font.
		/// </summary>
		/// <param name="font">The font to set to.</param>
		/// <param name="inertColour">Text colour when the Button is in the inert (ie. the cursor is not on it).</param>
		/// <param name="hoverColour">Text colour when the Button is in the hover state (ie. the cursor over it).</param>
		/// <param name="primedColour">Text colour the the Button is in the primed state (ie. the cursor over it and mouse button down).</param>
		/// <param name="inactiveColour">Text colour when the Button is inactive</param>
		public void SetFont( SpriteFont font, Color inertColour, Color hoverColour, Color primedColour, Color inactiveColour )
		{
			Font = font;
			textColours[0] = inertColour;
			textColours[1] = hoverColour;
			textColours[2] = primedColour;
			textColours[3] = inactiveColour;
		}

		#endregion

		#region Update/Draw Overrides

		/// <summary>
		/// Called automatically every frame if the Button is active.
		/// </summary>
		protected override void Update( GameTime gameTime, InputState input )
		{
			// Behavior depends on the state the button's in
			switch ( state )
			{
				case 0:		// Inert
					if ( cursorOver.Evaluate( input ) )
					{
						state = 1;
					}
					break;
				case 1:		// Hover
					if ( ! cursorOver.Evaluate( input ) )
					{
						state = 0;
					}
					else if ( mouseButtonDown.Evaluate( input ) )
					{
						state = 2;
					}
					break;
				case 2:		// Primed
					if ( ! cursorOver.Evaluate( input ) )
					{
						state = 0;
					}
					else if ( ! mouseButtonHeld.Evaluate( input ) )
					{
						state = 1;
						Trigger();
					}
					break;
			}
		}

		/// <summary>
		/// Called automatically every frame so that the Button can draw itself.
		/// </summary>
		public override void Draw( GameTime gameTime, float alpha, SpriteEffects effect )
		{
			int frame = Active ? state : 3;

			// Open the drawing buffer
			SpriteBatch sb = screen.ScreenManager.SpriteBatch;
			sb.Begin( SpriteSortMode.Deferred, BlendState.AlphaBlend );

			// Draw the texture (if set)
			if ( Texture != null )
			{
				sb.Draw( Texture, area, new Rectangle( frame * texFrameWidth, 0, texFrameWidth, texFrameHeight ), Color.White * alpha, 0f, Vector2.Zero, effect, 0 );
			}

			// Draw button text (if set)
			if ( Text != "" && Font != null )
			{
				Vector2 textSize = Font.MeasureString( Text );
				
				int textXOffset = (int)Math.Ceiling( ( Width - textSize.X ) / 2 );
				int textYOffset = (int)Math.Ceiling( ( Height - textSize.Y ) / 2 );
				//Color textColour = textColours[frame];
				//textColour.A = (byte)MathHelper.Clamp( alpha, 0, 255 );

				sb.DrawString( Font, Text, new Vector2( Position.X + textXOffset, Position.Y + textYOffset ), textColours[frame] * alpha );
			}

			// Close the drawing buffer
			sb.End();
		}

		#endregion
	}
}
