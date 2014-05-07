using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using UIManager;
using WizardryShared;

namespace WizardryClient
{
	/// <summary>
	/// This is the main type of the game
	/// </summary>
	public class WizardryGameClient : Microsoft.Xna.Framework.Game
	{
		GraphicsDeviceManager graphics;
		ScreenManager screenManager;
		ScreenFactory screenFactory;
		TextureProvider textureProvider;

		// Texture for the cursor
		Texture2D cursorTex;

		public TextureProvider TextureProvider
		{
			get { return textureProvider; }
		}

		public WizardryGameClient()
		{
			// Setup Content manager and the GraphicsDeviceManager
			Content.RootDirectory = "Content";
			graphics = new GraphicsDeviceManager( this );
			textureProvider = new TextureProvider( Content );

			// Set the Window title, window size and turn on the mouse cursor
			Window.Title = "Wizardry";
			graphics.PreferredBackBufferWidth = 800;
			graphics.PreferredBackBufferHeight = 600;
			//IsMouseVisible = true;

			// Set up the Screen Manager
			screenFactory = new ScreenFactory();
			Services.AddService( typeof(IScreenFactory), screenFactory );
			screenManager = new ScreenManager( this );
			Components.Add( screenManager );
	
			// Attach the initial screens
			screenManager.AddScreen( new BackgroundScreen(), null );
			screenManager.AddScreen( new MainMenuScreen(), null );

			// Give appropriate references to the GameManager
			GameManager.Instance.Game = this;
		}

		/// <summary>
		/// Initializes the game.
		/// </summary>
		protected override void Initialize()
		{
			base.Initialize();
			GameManager.Instance.SpriteBatch = screenManager.SpriteBatch;
			DrawingHelpers.Initialize( GraphicsDevice );
			cursorTex = Content.Load<Texture2D>( "Images/cursor" );
		}

		/// <summary>
		/// Loads content required by the game.
		/// </summary>
		protected override void LoadContent()
		{
			Spell.LoadContent( TextureProvider );
			Character.LoadContent( TextureProvider );
            PickUp.LoadContent(textureProvider);
			base.LoadContent();
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update( GameTime gameTime )
		{
			GameManager.Instance.HandleIncomingPackets();

			// The real magic happens in the Screens
			base.Update( gameTime );
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw( GameTime gameTime )
		{
			// The real magic happens in the Screens
			GraphicsDevice.Clear( Color.Black );
			base.Draw( gameTime );

			// Draw the cursor
			MouseState mouse = Mouse.GetState();
			Vector2 mousePos = new Vector2( mouse.X, mouse.Y );
			if ( GraphicsDevice.Viewport.Bounds.Contains( new Point( mouse.X, mouse.Y ) ) )
			{
				SpriteBatch sb = GameManager.Instance.SpriteBatch;
				sb.Begin();
				sb.Draw( cursorTex, mousePos, Color.White );
				sb.End();
			}
		}

		/// <summary>
		/// Called when the game is shutting down.
		/// </summary>
		protected override void OnExiting( object sender, System.EventArgs args )
		{
			GameManager.Instance.Client.Disconnect( "o/" );
			base.OnExiting( sender, args );
		}
	}
}
