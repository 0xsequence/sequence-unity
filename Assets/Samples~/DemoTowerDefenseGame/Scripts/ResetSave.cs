using UnityEngine;

namespace DefaultNamespace
{
    public class ResetSave : CheatCode
    {
        public void DoResetSave()
        {
            GameStateSaver saver = new GameStateSaver();
            saver.ResetSave();
        }

        protected override bool HandleCheatCode()
        {
            if (_cheatCode == "reset")
            {
                DoResetSave();
                return true;
            }

            return false;
        }
    }
}