/* This game/project is a test for the Tile Engine */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using DebugAssist;

// Add the namespace for the tile engine
using TileEngine;

namespace XnaTestGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;		// ewww public? well, just for testing purposes :P
		KeyboardState prevState;
		SpriteFont fnt;

		float mapScale = 1f;
		float camSpeed = 20f;
        
        // The tile engine objects
        public Camera camera;
        public IsoTileMap tileMap;
		//SquareTileMap tileMap;
		//HexTileMap tileMap;

		// Test character!
		TheDude theDude;
        PlayerSprite theDude2;

		// Test spell!
		Spell spell = new Spell();

		// Debug assistor
		DbgAssistor dbg;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

			graphics.PreferredBackBufferHeight = 600;
			graphics.PreferredBackBufferWidth = 800;
			//graphics.PreferredBackBufferHeight = 800;
			//graphics.PreferredBackBufferWidth = 1150;
			//graphics.IsFullScreen = true;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch( GraphicsDevice );

			IsMouseVisible = true;   // Makes the mouse cursor visible
			prevState = Keyboard.GetState();

			// Create the character
			theDude = new TheDude( this );
            theDude2 = new PlayerSprite( this );
            Animation playerAnimation = new Animation();
            Texture2D playerTexture = Content.Load<Texture2D>("Windlasher/walkside");
            playerAnimation.Initialize(playerTexture, Vector2.Zero, 6, 30, Color.White, 2f, true);
            theDude2.Initialize("WindLasher", new Vector2(298,212));
			Components.Add( theDude2 );

			// Create the debug helper as a service
			dbg = new DbgAssistor( this );
			dbg.SetSpritebatch( spriteBatch );
			Services.AddService( typeof(IDbgAssistor), dbg );
			Components.Add( dbg );

			// Add some debugging info
			SetupDebugTexts();

			base.Initialize();
        }

        protected override void LoadContent()
        {
			// Load the font for drawing text
			dbg.DbgFont = Content.Load<SpriteFont>( "Fonts/font" );
			

			////////////////////////////////////////////
			// Init the tile engine components
			////////////////////////////////////////////

			// Camera
			camera = new Camera(this);
			camera.Speed = camSpeed;
			camera.Autonomous = false;
			Components.Add( camera );

			// TileMap
			tileMap = new IsoTileMap( camera );
			//tileMap = new SquareTileMap( camera );
			//tileMap = new HexTileMap( camera );
			tileMap.Scale = mapScale;

			// Cursor - Cursors are currently only implemented for iso maps!
			tileMap.Cursor.Active = true;
			tileMap.Cursor.Colour = Color.Blue;

            // Load the tile map
			tileMap.LoadFromFile( Content, "Content/Maps/IsoTestMap.map" );
			//tileMap.LoadFromFile( Content, "Content/Maps/WallsIsoTestMap.map" );
			//tileMap.LoadFromFile( Content, "Content/Maps/SnowIsoTestMap.map" );
			//tileMap.LoadFromFile( Content, "Content/Maps/SmallIsoTestMap.map" );
			//tileMap.LoadFromFile( Content, "Content/Maps/SquareTestMap.map" );
			//tileMap.LoadFromFile( Content, "Content/Maps/HexTestMap.map" );

			// Set the camera to fit the world
			tileMap.SetCameraToWorldBounds();
			//camera.SetBounds( Point.Zero, new Point( 5000, 5000 ) ); // Ok I lied :P let the camera be for now!


			////////////////////////////////////////////
			// Spells!
			////////////////////////////////////////////

			// Fun with spells!
			spell = new Spell();
			spell.Initialize( GraphicsDevice.Viewport, Content.Load<Texture2D>( "Spells/spell1" ) );
        }

        protected override void Update(GameTime gameTime)
        {
			KeyboardState newState = Keyboard.GetState();
			
            spell.Update(new Vector2(Mouse.GetState().X, Mouse.GetState().Y), gameTime, newState);

            // Allows the game to exit
            if ( GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
				 newState.IsKeyDown( Keys.Escape ) )
                this.Exit();

			// Control map scaling
			if ( newState.IsKeyDown( Keys.Add ) && !prevState.IsKeyDown( Keys.Add ) )
			{
				mapScale = MathHelper.Clamp( mapScale + 0.05f, 0.05f, 5f );
				camSpeed += 5f;
			}
			else if ( newState.IsKeyDown( Keys.Subtract ) && !prevState.IsKeyDown( Keys.Subtract ) )
			{
				mapScale = MathHelper.Clamp( mapScale - 0.05f, 0.05f, 5f );
				camSpeed -= 5f;
			}
			if ( tileMap.Scale != mapScale )
			{
				tileMap.Scale = mapScale;
				tileMap.SetCameraToWorldBounds();
			}

			// Setup debug ability to toggle camera autonomy
			DbgAction action = new DbgAction();
			action.Activator = new DbgKey( Keys.C, Buttons.RightShoulder );
			action.Action = new DbgActionDel( () =>
				{
					camera.Autonomous = !camera.Autonomous;
					theDude.camBound = !theDude.camBound;
				} );
			dbg.AddDbgAction( action );

			prevState = newState;
			
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // Open the sprite batch to the correct mode.
			//  NOTE: The Iso engine needs the modes set, while setting these modes
			//        breaks the drawing for the other two engines.
			spriteBatch.Begin( SpriteSortMode.BackToFront, BlendState.AlphaBlend );
			//spriteBatch.Begin();
            spell.Draw(spriteBatch);
			// Let the map draw itself
			tileMap.Draw( spriteBatch );


			// Let the game draw itself - other components can therefore use the same depth buffer
			base.Draw( gameTime );

			
			// Lock and load
			spriteBatch.End();

        }

		private void SetupDebugTexts()
		{
			dbg.Title = "TileEngine Test Game";

			// Add info about the map's current scale
			dbg.AddDbgMsg( () => 
				{ return "Map Scale: " + mapScale.ToString(); } );

			// Add info about the camera
			dbg.AddDbgMsg( () =>
				{ return "Camera: (" + (int)camera.Position.X + ", " + (int)camera.Position.Y + ")"; } );

			// Add info about the Mouse position
			dbg.AddDbgMsg( () =>
				{ return "Mouse: (" + (int)Mouse.GetState().X + ", " + (int)Mouse.GetState().Y + ")"; } );

			dbg.AddDbgMsg( () =>
				{
					Vector2 mouseWorld = camera.ScreenToWorldPosition( new Vector2( Mouse.GetState().X, Mouse.GetState().Y ) );
					return "Mouse World Pos: (" + (int)mouseWorld.X + ", " + (int)mouseWorld.Y + ")";
				} );

			dbg.AddDbgMsg( () =>
				{
					Vector2 mouseWorld = camera.ScreenToWorldPosition( new Vector2( Mouse.GetState().X, Mouse.GetState().Y ) );
					Point mouseCell = tileMap.WorldLocationToTileRowCol( mouseWorld );
					return "Mouse On Tile: [" + mouseCell.Y.ToString() + ", " + mouseCell.X.ToString() + "]";
				} );
		}
    }
}