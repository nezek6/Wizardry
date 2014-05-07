using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using System;

namespace WizardryShared
{
    public partial class PickUp
    {
        static Random randomizer = new Random();

		public delegate void SpawnBehaviour( TimeSpan elapsedTime, PickUp pickUp );

		public delegate void HitBehaviour( TimeSpan elapsedTime, PickUp pickUp, Character target );

        public struct PickUpDetails
        {

            public readonly double respawnTime;

            public readonly SpawnBehaviour spawnBehaviour;

            public readonly HitBehaviour hitBehaviour;

            public PickUpDetails(
                double respawnTime,
                SpawnBehaviour spawnBehaviour,
                HitBehaviour hitBehaviour
                )
            {
                this.respawnTime = respawnTime;
                this.spawnBehaviour = spawnBehaviour;
                this.hitBehaviour = hitBehaviour;
            }
        }


        public static readonly IList<PickUpDetails> Details = new ReadOnlyCollection<PickUpDetails>(new[]{
			
			// PickUp ID 0 - Crystal
			new PickUpDetails(
				30,
				( TimeSpan elapsedTime, PickUp pickUp ) => 
				{
                    if ( pickUp.fixedLocation.X == -1 )
                    {
						int mapWidth = pickUp.Provider.TileMap.WidthInPixels;
						int mapHeight = pickUp.Provider.TileMap.HeightInPixels;
                        do
                        {
							int xPos = randomizer.Next( 0, mapWidth );
							int yPos = randomizer.Next( 0, mapHeight );
                            pickUp.worldPosition = new Vector2(xPos, yPos);
							pickUp.fixedLocation = pickUp.worldPosition;
                        } while (pickUp.provider.TileMap.GetHeightAtWorldPosition(pickUp.worldPosition) != 0);
                    }
                    else
                    {
                        pickUp.worldPosition = pickUp.fixedLocation;
                    }
				},
                ( TimeSpan elapsedTime, PickUp pickUp, Character target ) => 
				{
					// Guard to make sure we don't process collisions while respawning
					if ( pickUp.CurrentStatus == PickUpStatus.RESPAWNING )
					{
						return;
					}

                    target.CrystalCount += 1;
                    pickUp.currentStatus = PickUpStatus.RESPAWNING;
                    pickUp.respawnTimer = Details[0].respawnTime;
                    //pickUp.worldPosition = new Vector2(-1, -1);
                    //pickUp.currentAnimation.BoundingBox = new Rectangle();
				}			
				
			),
			
		});
    }
}
