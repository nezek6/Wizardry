using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WizardryShared;
using Microsoft.Xna.Framework;

namespace WizardryClient
{
    class NPC : Character
    {

        double previousHealth = 100;

        double stunDuration = 0.2;

        double stunCounter = 0;

        enum AttackType {RANGE,MELEE};

        AttackType type = AttackType.RANGE;

        Random randomizer = new Random();

        public NPC(ResourceProvider provider): base(provider)
        {
            Name = "NPC";
        }

        public override void Update(TimeSpan elapsedTime)
        {
            if (Status == CharStatus.DEAD)
            {
                return;
            }
            if (CurrentHealth < 0)
            {
                worldPosition = new Vector2(-1000,-1000);
                Status = CharStatus.DEAD;
                Active = false;
                provider.GameState.Characters[0].CurrentHealth += 10;
                provider.GameState.Characters[0].CrystalCount += 10;
                provider.GameState.Characters[0].Score += 1;
                return;
            }

            if (previousHealth != CurrentHealth)
            {
                stunCounter = stunDuration;
            }

            base.Update(elapsedTime);

            if (RoleID == 3)
            {
                type = AttackType.MELEE;
            }

            DamageModifier = 1 + (provider.GameState.Characters[0].Score / 100);

           // SeparateNPCs();

            Vector2 toPlayer = (provider.GameState.Characters[0].WorldPosition - WorldPosition);
            float distance = toPlayer.Length();

            if (type == AttackType.MELEE)
            {
                if (distance > 80 && distance < 400 && stunCounter <= 0)
                {
                    toPlayer.Normalize();
                    toPlayer *= 4;
                    WorldPosition += toPlayer;
                }
                Shoot(0, provider.GameState.Characters[0].WorldPosition);
            }
            else if (type == AttackType.RANGE)
            {
                /*
                if (distance > 300 && distance < 400 )
                {
                    toPlayer.Normalize();
                    toPlayer *= 4;
                    WorldPosition += toPlayer;
                }
                 */
                if (distance < 500)
                {
                    Shoot(0, new Vector2(provider.GameState.Characters[0].WorldPosition.X + randomizer.Next(-70, 70), provider.GameState.Characters[0].WorldPosition.Y + randomizer.Next(-70, 70)));
                }

            }

            previousHealth = CurrentHealth;
            if (stunCounter >= 0)
            {
                stunCounter -= elapsedTime.TotalSeconds;
            }
        }

        public void SeparateNPCs()
        {
            Vector2 displacement = Vector2.Zero;

            foreach (Character c in provider.GameState.Characters)
            {
                if (c == provider.GameState.Characters[0]) continue;
                Vector2 toNPC = (c.WorldPosition - WorldPosition);
                float distance = toNPC.Length();

                if (distance < 20)
                {
                    if (toNPC == Vector2.Zero)
                    {
                        Random randomizer = new Random();
                        toNPC = new Vector2(randomizer.Next(1, 100), randomizer.Next(1, 100));
                        toNPC.Normalize();
                        displacement += toNPC * -1;
                    }
                    toNPC.Normalize();
                    displacement += toNPC * -1;
                }

            }

            if (displacement != Vector2.Zero)
            {
                displacement.Normalize();
                displacement *= 1;
            }
            WorldPosition += displacement;
        }

    }
}
