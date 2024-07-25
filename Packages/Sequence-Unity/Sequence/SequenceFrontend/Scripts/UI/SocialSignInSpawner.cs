using System.Collections.Generic;
using UnityEngine;

namespace Sequence.Demo
{
    public class SocialSignInSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _googleSignInButton;
        [SerializeField] private GameObject _facebookSignInButton;
        [SerializeField] private GameObject _discordSignInButton;
        [SerializeField] private GameObject _appleSignInButton;

        private void Awake()
        {
            List<GameObject> toDisable = new List<GameObject>();
#if UNITY_IOS && !UNITY_EDITOR
            toDisable.Add(_facebookSignInButton);
            toDisable.Add(_discordSignInButton);
#elif UNITY_ANDROID && !UNITY_EDITOR
            toDisable.Add(_facebookSignInButton);
            toDisable.Add(_discordSignInButton);
#elif UNITY_WEBGL && !UNITY_EDITOR
            toDisable.Add(_facebookSignInButton);
            toDisable.Add(_discordSignInButton);
            toDisable.Add(_appleSignInButton);
#else
            toDisable.Add(_facebookSignInButton);
            toDisable.Add(_discordSignInButton);
#endif
            int items = toDisable.Count;
            for (int i = 0; i < items; i++)
            {
                toDisable[i].SetActive(false);
            }
        }
    }
}