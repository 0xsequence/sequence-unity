using UnityEngine;

namespace DefaultNamespace
{
    public abstract class CheatCode : MonoBehaviour
    {
        protected string _cheatCode = "";
        private float _cheatCodeTimeRemaining = 0f;
        private float _cheatCodeTime = 3f;
        
        public void Update()
        {
            EnterCheatCode();
            if (HandleCheatCode())
            {
                _cheatCodeTimeRemaining = 0f;
                _cheatCode = "";
            }
            DecrementCheatCodeTimer();
        }

        private void EnterCheatCode()
        {
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                _cheatCode = "";
                _cheatCodeTimeRemaining = 0f;
            }
            else if (Input.anyKeyDown)
            {
                if (_cheatCodeTimeRemaining <= 0f)
                {
                    _cheatCodeTimeRemaining = _cheatCodeTime;
                }
                _cheatCode += Input.inputString;
            }
        }
        
        /// <summary>
        /// Handle the cheat code and return true if the cheat code was handled, false otherwise
        /// </summary>
        /// <returns></returns>
        protected abstract bool HandleCheatCode();
        
        private void DecrementCheatCodeTimer()
        {
            if (_cheatCodeTimeRemaining > 0f)
            {
                _cheatCodeTimeRemaining -= Time.deltaTime;
            }
            if (_cheatCodeTimeRemaining <= 0f)
            {
                _cheatCode = "";
            }
        }
    }
}