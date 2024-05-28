using Sequence.Authentication;
using UnityEngine;
using UnityEngine.UI;

namespace Sequence.Demo
{
    public class LoginButtonHighlighter : MonoBehaviour
    {
        [SerializeField] private Outline[] _emailLoginOutlines;
        [SerializeField] private Outline[] _googleLoginOutlines;
        [SerializeField] private Outline[] _discordLoginOutlines;
        [SerializeField] private Outline[] _facebookLoginOutlines;
        [SerializeField] private Outline[] _appleLoginOutlines;

        public void HighlightAppropriateButton(LoginMethod method)
        {
            switch (method)
            {
                case LoginMethod.Email:
                    HighlightButton(_emailLoginOutlines);
                    break;
                case LoginMethod.Google:
                    HighlightButton(_googleLoginOutlines);
                    break;
                case LoginMethod.Discord:
                    HighlightButton(_discordLoginOutlines);
                    break;
                case LoginMethod.Facebook:
                    HighlightButton(_facebookLoginOutlines);
                    break;
                case LoginMethod.Apple:
                    HighlightButton(_appleLoginOutlines);
                    break;
            }
        }

        private void HighlightButton(Outline[] outlines)
        {
            int length = outlines.Length;
            for (int i = 0; i < length; i++)
            {
                outlines[i].enabled = true;
            }
        }
    }
}