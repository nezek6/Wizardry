using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using System;

namespace WizardryShared
{
	public partial class Spell
	{
		public delegate void UpdateBehaviour(TimeSpan timeSpan, Spell spell);

		public delegate void HitBehaviour(TimeSpan timeSpan, Spell spell, Character target);

		public static Random randomizer = new Random();

		public struct SpellDetails
		{
			public readonly double maxDistance;

			public readonly double cooldown;

			public readonly int speed;

			public readonly double channelTime;

			public readonly bool ignoreDrawDepth;

			public readonly UpdateBehaviour updateBehaviour;

			public readonly HitBehaviour hitBehaviour;

			public SpellDetails(
				double maxDistance,
				double cooldown,
				int speed,
				double channelTime,
				bool ignoreDrawDepth,
				UpdateBehaviour updateBehaviour,
				HitBehaviour hitBehaviour
				)
			{
				this.maxDistance = maxDistance;
				this.cooldown = cooldown;
				this.speed = speed;
				this.channelTime = channelTime;
				this.ignoreDrawDepth = ignoreDrawDepth;
				this.updateBehaviour = updateBehaviour;
				this.hitBehaviour = hitBehaviour;
			}
		}


		public static readonly IList<SpellDetails> Details = new ReadOnlyCollection<SpellDetails>(new[]{
			
			// Spell ID 0 - Fireball
			new SpellDetails(
				400,
				0.4,
				5,
				0,
				false,
				( TimeSpan timeSpan, Spell spell) => 
				{
					Vector2 nextposition = spell.worldPosition + (spell.Direction * Details[0].speed);

					Vector2 distanceV = spell.worldPosition - spell.StartPoint;
					spell.Distance = distanceV.Length();

					int nextHeight = spell.provider.TileMap.GetHeightAtWorldPosition(nextposition);
					if (nextHeight >= HEIGHT_THRESHOLD)
					{
						spell.Active = false;
					}
				   spell.worldPosition = nextposition;
				},
				( TimeSpan timeSpan, Spell spell, Character target) => 
				{
					if (target.Team != spell.caster.Team)
					{
						target.CurrentHealth -= 10 * spell.caster.DamageModifier;
                        spell.Active = false;
					}
				}			
				
			),

			// Spell ID 1 - Holy Bolt
			new SpellDetails(
				400,
				0.4,
				5,
				0,
				false,
				( TimeSpan timeSpan, Spell spell) => 
				{
					Vector2 nextposition = spell.worldPosition + (spell.Direction * Details[0].speed);

					Vector2 distanceV = spell.worldPosition - spell.StartPoint;
					spell.Distance = distanceV.Length();

					int nextHeight = spell.provider.TileMap.GetHeightAtWorldPosition(nextposition);
					if (nextHeight >= HEIGHT_THRESHOLD)
					{
						spell.Active = false;
					}
				   spell.worldPosition = nextposition;
				},
				( TimeSpan timeSpan, Spell spell, Character target) => 
				{
					if (target.Team.Equals(spell.caster.Team))
					{
						target.CurrentHealth += 3 * spell.caster.DamageModifier;
					}
					else target.CurrentHealth -= 3 * spell.caster.DamageModifier;
					spell.Active = false;
				}		
			),

			// Spell ID 2 - Wind Lash
			new SpellDetails(
				400,
				0.4,
				5,
				0,
				false,
				( TimeSpan timeSpan, Spell spell) => 
				{
					Vector2 nextposition = spell.worldPosition + (spell.Direction * Details[0].speed);

					Vector2 distanceV = spell.worldPosition - spell.StartPoint;
					spell.Distance = distanceV.Length();

					int nextHeight = spell.provider.TileMap.GetHeightAtWorldPosition(nextposition);
					if (nextHeight >= HEIGHT_THRESHOLD)
					{
						spell.Active = false;
					}
				   spell.worldPosition = nextposition;
				},
				( TimeSpan timeSpan, Spell spell, Character target) => 
				{
					if (target.Team != spell.caster.Team)
					{
						target.CurrentHealth -= 5 * spell.caster.DamageModifier;
                        spell.Active = false;
					}
				}	
			),

			// Spell ID 3 - Explosion field
			new SpellDetails(
				10,
				5f,
				5,
				0,
				false,
				( TimeSpan timeSpan, Spell spell) => 
				{
						Vector2 position = spell.StartPoint;
						if ((spell.tickTracker) > 0.15)
						{
						spell.tickTracker = 0;
						position.X += randomizer.Next(-300,300);
						position.Y += randomizer.Next(-300,300);
						spell.provider.Shoot(4, position, new Vector2(1, 1), new Vector2(1,1), 0, spell.caster);
						}
						spell.Distance += (float)timeSpan.TotalSeconds;
						spell.tickTracker += (float)timeSpan.TotalSeconds;

				},
				( TimeSpan timeSpan, Spell spell, Character target) => 
				{
					
				}			
			),

			// Spell ID 4 - Explosion
			new SpellDetails(
				30*12,
				7,
				5,
				0,
				true,
				( TimeSpan timeSpan, Spell spell) => 
				{
					spell.Distance += (float)timeSpan.TotalMilliseconds;
				},
				( TimeSpan timeSpan, Spell spell, Character target) => 
				{
					if (target.Team != spell.caster.Team)
					{
						target.CurrentHealth = (target.CurrentHealth - (7f / 12f)) * spell.caster.DamageModifier;
					}
				}		
			),

			// Spell ID 5 - Holy Nova
			new SpellDetails(
				1,
				10f,
				5,
				0,
				false,
				( TimeSpan timeSpan, Spell spell) => 
				{
					float angle = 0;
					Matrix rotMatrix = Matrix.CreateRotationZ(angle);
					Vector2 direction = new Vector2();
					direction = Vector2.Transform(new Vector2(1, 0), rotMatrix);

					if (spell.tickTracker > 0.2)
					{
						Vector2 position = spell.StartPoint;
						for (int i = 0; i < 21; i++){
							spell.provider.Shoot(6, position, spell.startPoint + direction, direction, angle,spell.caster);
							angle = ((MathHelper.Pi * 2) / 20)*i;
							rotMatrix = Matrix.CreateRotationZ(angle);
							direction = Vector2.Transform(new Vector2(1, 0), rotMatrix);
							spell.tickTracker = 0;
					}
					spell.provider.Shoot(12, spell.startPoint, spell.startPoint + direction, direction,0f, spell.caster);
					}
					spell.Distance += (float)timeSpan.TotalSeconds;
					spell.tickTracker += (float)timeSpan.TotalSeconds;

				},
				( TimeSpan timeSpan, Spell spell, Character target) => 
				{
					spell.Active = false;
				}		
			),

			// Spell ID 6 - Holy Nova Bolt
			new SpellDetails(
				400,
				1,
				5,
				0,
				false,
				( TimeSpan timeSpan, Spell spell) => 
				{
					Vector2 nextposition = spell.worldPosition + (spell.Direction * Details[0].speed);

					Vector2 distanceV = spell.worldPosition - spell.StartPoint;
					spell.Distance = distanceV.Length();

					int nextHeight = spell.provider.TileMap.GetHeightAtWorldPosition(nextposition);
					if (nextHeight >= HEIGHT_THRESHOLD)
					{
						spell.Active = false;
					}
				   spell.worldPosition = nextposition;
				},
				( TimeSpan timeSpan, Spell spell, Character target) => 
				{
					if ( target.Team == spell.Caster.Team )
					{
						target.CurrentHealth += 2 * spell.caster.DamageModifier;
					}
					else target.CurrentHealth -= 1 * spell.caster.DamageModifier;
					spell.Active = false;
				}		
			),

			// Spell ID 7 - Claw
			new SpellDetails(
				75*4,
				0.4f,
				10,
				0,
				true,
				( TimeSpan timeSpan, Spell spell) => 
				{
					float scaledFrame = spell.caster.CurrentAnimation.FrameHeight * spell.caster.CurrentAnimation.Scale;
						spell.worldPosition = spell.caster.WorldPosition + new Vector2(0, -scaledFrame/2) + (spell.Direction * 50);


					spell.Distance += (float)timeSpan.TotalMilliseconds;
				},
				( TimeSpan timeSpan, Spell spell, Character target) => 
				{
					if (target.Team != spell.caster.Team)
						target.CurrentHealth -= 1 * spell.caster.DamageModifier;
				}		
			),

			// Spell ID 8 - Stampede
			new SpellDetails(
				10,
				5f,
				5,
				0,
				false,
				( TimeSpan timeSpan, Spell spell) => 
				{
					Random randomizer = new Random();
						Vector2 position = spell.StartPoint;
						if ((spell.tickTracker) > 0.5)
						{
						spell.tickTracker = 0;
						position.X += -400;
						position.Y += randomizer.Next(-300,300);
						spell.provider.Shoot(9, position, new Vector2(1, 1), new Vector2(1,1), 0,spell.caster);
						}
						spell.Distance += (float)timeSpan.TotalSeconds;
						spell.tickTracker += (float)timeSpan.TotalSeconds;

				},
				( TimeSpan timeSpan, Spell spell, Character target) => 
				{

				}		
			),

			// Spell ID 9 - Stampede Unit
			new SpellDetails(
				800,
				7,
				5,
				0,
				false,
				( TimeSpan timeSpan, Spell spell) => 
				{
					Vector2 nextposition = spell.worldPosition + (new Vector2(1,0) * 5);

					Vector2 distanceV = spell.worldPosition - spell.StartPoint;
					spell.Distance = distanceV.Length();

					int nextHeight = spell.provider.TileMap.GetHeightAtWorldPosition(nextposition);
					if (nextHeight >= HEIGHT_THRESHOLD)
					{
						spell.Active = false;
					}
				   spell.worldPosition = nextposition;
				},
				( TimeSpan timeSpan, Spell spell, Character target) => 
				{
					if (target.Team != spell.caster.Team)
					{
						target.CurrentHealth -= 20 * spell.caster.DamageModifier;
					}
					spell.Active = false;
				}		
			),

			// Spell ID 10 - Fire Shift
			new SpellDetails(
				800,
				25,
				10,
				0,
				false,
				( TimeSpan timeSpan, Spell spell) => 
				{
					//spell.userData = (int)spell.caster.Status;
					spell.caster.Status = Character.CharStatus.OFFSCREEN;
					spell.caster.PositionOverride = true;
					
					Vector2 nextposition = spell.worldPosition + (spell.Direction * Details[10].speed);

					Vector2 distanceV = spell.worldPosition - spell.StartPoint;
					spell.Distance = distanceV.Length();

					int nextHeight = spell.provider.TileMap.GetHeightAtWorldPosition(nextposition);
					if ((nextHeight-spell.height) >= HEIGHT_THRESHOLD 
						|| spell.worldPosition.X > spell.provider.TileMap.GetWidthInPixels()
						|| spell.worldPosition.X < 0
						|| spell.worldPosition.Y > spell.provider.TileMap.GetHeightInPixels()
						|| spell.worldPosition.Y < 0
						|| spell.distance > Details[10].maxDistance - Details[10].speed
						|| spell.worldPosition == spell.clickPosition
						|| spell.distance > spell.clickDistance)
					{

						spell.Active = false;
						spell.caster.PositionOverride = false;
					   spell.caster.Status = Character.CharStatus.NORMAL;
					  // spell.caster.Status = (Character.CharStatus)spell.userData;
					}
				   spell.worldPosition = nextposition;
				   spell.height = nextHeight;
				   spell.caster.WorldPosition = spell.worldPosition;

				   spell.provider.Shoot(4, spell.worldPosition, Vector2.Zero, Vector2.Zero, 0f, spell.caster);
				},
				( TimeSpan timeSpan, Spell spell, Character target) => 
				{
					if (target.Team != spell.caster.Team)
					{
						target.CurrentHealth -= 0.2f * spell.caster.DamageModifier;
					}
				}		
			),

			// Spell ID 11 - Claw Leap
			new SpellDetails(
				400,
				25,
				10,
				0,
				true,
				( TimeSpan timeSpan, Spell spell) => 
				{
					//spell.userData = (int)spell.caster.Status;
					spell.caster.Status = Character.CharStatus.OFFSCREEN;
					spell.caster.PositionOverride = true;
					
					Vector2 nextposition = spell.worldPosition + (spell.Direction * Details[10].speed);

					Vector2 distanceV = spell.worldPosition - spell.StartPoint;
					spell.Distance = distanceV.Length();

					int nextHeight = spell.provider.TileMap.GetHeightAtWorldPosition(nextposition);
					if ((nextHeight-spell.height) >= HEIGHT_THRESHOLD 
						|| spell.worldPosition.X > spell.provider.TileMap.GetWidthInPixels()
						|| spell.worldPosition.X < 0
						|| spell.worldPosition.Y > spell.provider.TileMap.GetHeightInPixels()
						|| spell.worldPosition.Y < 0
						|| spell.distance > Details[11].maxDistance - Details[11].speed
						|| spell.worldPosition == spell.clickPosition
						|| spell.distance > spell.clickDistance)
					{

						spell.Active = false;
						spell.caster.PositionOverride = false;
					   spell.caster.Status = Character.CharStatus.NORMAL;
					  // spell.caster.Status = (Character.CharStatus)spell.userData;
					}
				   spell.worldPosition = nextposition;
				   spell.height = nextHeight;
				   spell.caster.WorldPosition = spell.worldPosition;

				},
				( TimeSpan timeSpan, Spell spell, Character target) => 
				{
					if (target.Team != spell.caster.Team)
					{
						target.CurrentHealth -= 0.05f * spell.caster.DamageModifier;
					}
				}		
			),

			//ID 12 : Holy Wings
			new SpellDetails(
				1,
				7,
				5,
				0,
				true,
				( TimeSpan timeSpan, Spell spell) => 
				{
					spell.Distance += (float)timeSpan.TotalSeconds;
					spell.WorldPosition = spell.caster.WorldPosition + new Vector2(0, -60);
				},
				( TimeSpan timeSpan, Spell spell, Character target) => 
				{
					
				}		
			),

			// Spell ID 13 - Wind Spin
			new SpellDetails(
				800,
				25,
				10,
				0,
				true,
				( TimeSpan timeSpan, Spell spell) => 
				{
					//spell.userData = (int)spell.caster.Status;
					spell.caster.Status = Character.CharStatus.OFFSCREEN;
					spell.caster.PositionOverride = true;
					
					Vector2 nextposition = spell.worldPosition + (spell.Direction * Details[10].speed);

					Vector2 distanceV = spell.worldPosition - spell.StartPoint;
					spell.Distance = distanceV.Length();

					int nextHeight = spell.provider.TileMap.GetHeightAtWorldPosition(nextposition);
					if ((nextHeight-spell.height) >= HEIGHT_THRESHOLD 
						|| spell.worldPosition.X > spell.provider.TileMap.GetWidthInPixels()
						|| spell.worldPosition.X < 0
						|| spell.worldPosition.Y > spell.provider.TileMap.GetHeightInPixels()
						|| spell.worldPosition.Y < 0
						|| spell.distance > Details[13].maxDistance - Details[13].speed
						|| spell.worldPosition == spell.clickPosition
						|| spell.distance > spell.clickDistance)
					{

						spell.Active = false;
						spell.caster.PositionOverride = false;
					   spell.caster.Status = Character.CharStatus.NORMAL;
					  // spell.caster.Status = (Character.CharStatus)spell.userData;
					}
				   spell.worldPosition = nextposition;
				   spell.height = nextHeight;
				   spell.caster.WorldPosition = spell.worldPosition;

					if ((spell.distance - spell.tickTracker) > Details[13].maxDistance/10)
						{
							spell.tickTracker = spell.distance ;
							spell.provider.Shoot(14, spell.worldPosition, new Vector2(1,1), new Vector2(spell.Direction.Y * tornadoFlip, -spell.Direction.X * tornadoFlip), 0, spell.caster);
						tornadoFlip *= -1;
						}
						spell.Distance += (float)timeSpan.TotalSeconds;

				},
				( TimeSpan timeSpan, Spell spell, Character target) => 
				{
					if (target.Team != spell.caster.Team)
					{
						target.CurrentHealth -= 1f * spell.caster.DamageModifier;
					}
				}		
			),
			
			//ID 14: Tornado
			new SpellDetails(
				200,
				0.4,
				2,
				0,
				false,
				( TimeSpan timeSpan, Spell spell) => 
				{
					Vector2 nextposition = spell.worldPosition + (spell.Direction * Details[14].speed);

					Vector2 distanceV = spell.worldPosition - spell.StartPoint;
					spell.Distance = distanceV.Length();

					int nextHeight = spell.provider.TileMap.GetHeightAtWorldPosition(nextposition);
					if (nextHeight >= HEIGHT_THRESHOLD)
					{
						spell.Active = false;
					}
				   spell.worldPosition = nextposition;



				},
				( TimeSpan timeSpan, Spell spell, Character target) => 
				{
					target.CurrentHealth -= 15 * spell.caster.DamageModifier;
					spell.Active = false;
				}			
				
			),

			//ID 15: Ninja Storm
			new SpellDetails(
				225*10,
				5f,
				10,
				0,
				true,
				( TimeSpan timeSpan, Spell spell) => 
				{
					/*
					if ((spell.tickTracker) < 0)
					{
						spell.caster.CurrentFocus -= 100;
						spell.tickTracker = 0;
					}
					*/
					spell.caster.Status = Character.CharStatus.OFFSCREEN;
					spell.rotation = 0;
					float scaledFrame = spell.caster.CurrentAnimation.FrameHeight * spell.caster.CurrentAnimation.Scale;
						spell.worldPosition = spell.caster.WorldPosition;


					spell.Distance += (float)timeSpan.TotalMilliseconds;

					if (spell.distance > spell.maxDistance)
					{
						spell.provider.Shoot(16, spell.worldPosition , spell.worldPosition + new Vector2(0, 62), new Vector2(1, 1), 0, spell.caster);
						spell.provider.Shoot(16, spell.worldPosition , spell.worldPosition + new Vector2(65, 42),new Vector2(1, 1), 0, spell.caster);
						spell.provider.Shoot(16, spell.worldPosition,  spell.worldPosition + new Vector2(-65, 42), new Vector2(1, 1), 0, spell.caster);
						spell.provider.Shoot(16, spell.worldPosition, spell.worldPosition + new Vector2(50, -30), new Vector2(1, 1), 0, spell.caster);
						spell.provider.Shoot(16, spell.worldPosition,spell.worldPosition + new Vector2(-50, -30), new Vector2(1, 1), 0, spell.caster);
						spell.provider.Shoot(17, spell.worldPosition,spell.worldPosition + new Vector2(0, -50), new Vector2(1, 1), 0, spell.caster);
					}

				},
				( TimeSpan timeSpan, Spell spell, Character target) => 
				{
					if (target.Team != spell.caster.Team)
					{
						target.CurrentHealth -= 0.1f * spell.caster.DamageModifier;
					}
				}		
				
			),

			
			//ID 16: Ninja Storm Spin
			new SpellDetails(
				12,
				0.4f,
				10,
				0,
				true,
				( TimeSpan timeSpan, Spell spell) => 
				{
					spell.Distance += (float)timeSpan.TotalSeconds;
					 if (spell.distance >= spell.maxDistance)
					{
						spell.caster.Status = Character.CharStatus.NORMAL;
					 }
					 spell.worldPosition = spell.caster.WorldPosition - (spell.startPoint - spell.ClickPosition);
				},
				( TimeSpan timeSpan, Spell spell, Character target) => 
				{
					if (target.Team != spell.caster.Team)
					{
						target.CurrentHealth -= 0.1f * spell.caster.DamageModifier;
					}
				}		
				

			),

			
			//ID 17: Ninja Storm Tornado
			new SpellDetails(
				9,
				25f,
				10,
				0,
				true,
				( TimeSpan timeSpan, Spell spell) => 
				{
					spell.rotation = 0;
					float scaledFrame = spell.caster.CurrentAnimation.FrameHeight * spell.caster.CurrentAnimation.Scale;
						spell.worldPosition = spell.caster.WorldPosition - new Vector2(0,75);

					 if ((spell.distance - spell.tickTracker) > Details[17].maxDistance/24)
						{
							spell.tickTracker = spell.distance ;
							Vector2 direction = new Vector2(randomizer.Next(-20, 20), randomizer.Next(-20, 20));
							direction.Normalize();
							spell.provider.Shoot(14, spell.worldPosition, new Vector2(1,1), direction, 0, spell.caster);
						}
						spell.Distance += (float)timeSpan.TotalSeconds;

				},
				( TimeSpan timeSpan, Spell spell, Character target) => 
				{
					if (target.Team != spell.caster.Team)
						target.CurrentHealth -= 1f * spell.caster.DamageModifier;
				}		
				
			),
			
			//ID 18: Holy Shield
			new SpellDetails(
				3,
				25f,
				5,
				0,
				true,
				( TimeSpan timeSpan, Spell spell) => 
				{
					if (spell.tickTracker != 1234)
					{
						spell.tickTracker = 1234;
						spell.caster.CurrentHealth += 10;
						spell.userData = spell.caster.CurrentHealth;
						spell.caster.Speed = 7;
					}

					if (spell.caster.CurrentHealth != spell.userData)
					{
					spell.caster.CurrentHealth = (float)spell.userData;
					}

					spell.Distance += (float)timeSpan.TotalSeconds;
					spell.WorldPosition = spell.caster.WorldPosition + new Vector2(0, -spell.caster.CurrentAnimation.FrameHeight/2 - 10);
					if (spell.distance > spell.maxDistance)
					{
						spell.caster.Speed = 4;
					}
				},
				( TimeSpan timeSpan, Spell spell, Character target) => 
				{

				}   
			),

			//ID 19: Respawner
			new SpellDetails(
				GameSettings.RESPAWN_TIME * 1000,
				0,
				0,
				0,
				false,
				(TimeSpan elapsedTime, Spell s) =>
					{
						s.distance += (float)elapsedTime.TotalMilliseconds;
						if ( s.distance >= s.maxDistance )
						{
							s.Caster.WorldPosition = s.WorldPosition;
							s.Caster.PositionOverride = true;
							s.Caster.Status = Character.CharStatus.NORMAL;
							s.Caster.CurrentHealth = s.Caster.MaxHealth;
							s.Caster.CurrentFocus = 0;
							s.Caster.FocusOverride = true;
							s.Caster.CrystalCount = 0;
							s.Active = false;
						}
					},
				(TimeSpan elapsedTime, Spell s, Character target)=>
					{
						// Do nothing - this spell shouldn't cause collisions
					}
			),
		});


		//STATIC INFORMATION THAT CAN BE USED BY SPELL BEHAVIOURS

		static int tornadoFlip = 1;

	}

}
