using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UIManager.Controls
{
	/// <summary>
	/// Delegate type for Callback methods assigned to controls.
	/// </summary>
	/// <param name="sender">Reference to the control invoking the callback method.</param>
	public delegate void Callback( Control sender );

	/// <summary>
	/// Abstract base class for UI Controls.
	/// </summary>
	public abstract class Control
	{
		#region Data Members

		// List holding the callbacks assigned to the Control
		protected List<Callback> callbacks;

		// The position of the Control on screen
		protected Point position;

		// The width of the Control
		protected int width;

		// The Height of the Control
		protected int height;

		// Whether or not the control is active
		protected bool active = true;

		// The GameScreen that the Control belongs to
		protected GameScreen screen;

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the On-Screen position of the Control.
		/// </summary>
		public virtual Point Position
		{
			get
			{
				return position;
			}
			set
			{
				position = value;
			}
		}

		/// <summary>
		/// Gets or sets the Control's width.
		/// </summary>
		public virtual int Width
		{
			get
			{
				return width;
			}
			set
			{
				width = value;
			}
		}

		/// <summary>
		/// Gets or sets the Control's height.
		/// </summary>
		public virtual int Height
		{
			get
			{
				return height;
			}
			set
			{
				height = value;
			}
		}

		/// <summary>
		/// Gets or sets whether the Control is active.
		/// </summary>
		public virtual bool Active
		{
			get
			{
				return active;
			}
			set
			{
				active = value;
			}
		}

		#endregion

		#region Initialization

		/// <summary>
		/// Control class constructor.
		/// </summary>
		public Control( Point position, int width, int height, GameScreen screen )
		{
			callbacks = new List<Callback>();
			this.position = position;
			this.width = width;
			this.height = height;
			this.screen = screen;
			InitInputs();
		}

		/// <summary>
		/// Template method to be customized for each Control type's initialization of inputs.
		/// </summary>
		protected abstract void InitInputs();

		#endregion

		#region Methods

		/// <summary>
		/// Registers a Callback to be called when the Control is activated. The same Callback method
		/// cannot be added to the same Control twice.
		/// </summary>
		/// <param name="callback">The Callback method to add.</param>
		/// <returns>True if the Callback was registered and false otherwise.</returns>
		public bool RegisterCallback( Callback callback )
		{
			if ( ! callbacks.Contains( callback ) )
			{
				callbacks.Add( callback );
				return true;
			}
			return false;
		}

		/// <summary>
		/// Clears all Callbacks registered to the Control.
		/// </summary>
		public void ClearCallbacks()
		{
			callbacks.Clear();
		}

		/// <summary>
		/// Unregisters a Callback from the Control.
		/// </summary>
		/// <param name="callback">The Callback to unregister.</param>
		/// <returns>True if the Callback was unregistered and false otherwise (ie. it was not registered).</returns>
		public bool RemoveCallback( Callback callback )
		{
			if ( callbacks.Contains( callback ) )
			{
				callbacks.Remove( callback );
				return true;
			}
			return false;
		}

		/// <summary>
		/// Triggers the Control to call its callbacks.
		/// </summary>
		public void Trigger()
		{
			foreach ( Callback cb in callbacks )
			{
				cb( this );
			}
		}

		/// <summary>
		/// Called each frame for the Control to check user input.
		/// </summary>
		public void HandleInputs( GameTime gameTime, InputState input )
		{
			if ( Active )
			{
				Update( gameTime, input );
			}
		}

		/// <summary>
		/// Template method to customize the behavior of Controls. This method will
		/// be called automatically every frame that the Control is active.
		/// </summary>
		protected abstract void Update( GameTime gameTime, InputState input );

		/// <summary>
		/// Template method to customize how a Control is drawn.
		/// </summary>
		public abstract void Draw( GameTime gameTime, float alpha, SpriteEffects effect );

		/// <summary>
		/// Draws the Control with full Alpha and no Effects.
		/// </summary>
		public void Draw( GameTime gameTime )
		{
			Draw( gameTime, 1f, SpriteEffects.None );
		}

		/// <summary>
		/// Draws the Control with the specified Alpha and no Effects.
		/// </summary>
		public void Draw( GameTime gameTime, float alpha )
		{
			Draw( gameTime, alpha, SpriteEffects.None );
		}

		#endregion
	}
}
