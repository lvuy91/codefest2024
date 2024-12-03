using CodeFest24.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFest24
{
    public class EarthElement
    {
        public void Act(List<WeaponHammer> weaponWinds, Player player, Player opponent, MapInfoDto map)
        {
            if(player.HasTransform == true 
                && player.TransformType == 1)
            {
                if(isInRangeBeingAttack(player.CurrentPosition, opponent.CurrentPosition))
                {
                    //Play Safe
                    //GoExploreSpecialSpot();
                    //NearTheSpecialSpot();
                }
                else
                {
                    if(isPlayerHaveAdvantageToAct(player, opponent))
                    {

                    }
                    else
                    {
                        //GoBomb();
                    }
                }
            }
        }

        private bool isPlayerHaveAdvantageToAct(Player player, Player opponent)
        {
            bool isRun = true;

            if (player.TimeToUseSpecialWeapons > 0)
            {
                if (opponent.TimeToUseSpecialWeapons > 0
                    && opponent.TimeToUseSpecialWeapons < 3)
                {
                    if (player.EternalBadge > 0)
                    {
                        //Attack
                        isRun = false;
                        return isRun;
                    }
                    else
                    {
                        //Run
                        return isRun;
                    }
                }
                else
                {
                    //Run
                    return isRun;
                }
            }
            else
            {
                //Run
                return isRun;
            }
        }

        private bool isInRangeBeingAttack(CurrentPosition playerPosition, CurrentPosition opponentPosition)
        {
            if((playerPosition.Col == opponentPosition.Col
                || playerPosition.Row == opponentPosition.Row)
                && (opponentPosition.Col - playerPosition.Col > 7
                || opponentPosition.Row - playerPosition.Row > 7))
            {
                return true;
            }
            return false;
        }
    }
}
