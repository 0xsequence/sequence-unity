using System;
using TowerDefense.Game;
using TowerDefense.Level;
using UnityEngine;

namespace DefaultNamespace
{
    public class DebugEndLevel : CheatCode
    {
        protected override bool HandleCheatCode()
        {
            if (_cheatCode == "one")
            {
                EndLevel(1);
                return true;
            }
            else if (_cheatCode == "two")
            {
                EndLevel(2);
                return true;
            }
            else if (_cheatCode == "three")
            {
                EndLevel(3);
                return true;
            }

            return false;
        }

        public void EndLevel(int stars)
        {
            LevelManager.SafelyCallLevelCompleted(overrideStars: stars);
        }
    }
}