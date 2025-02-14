using System;
using Sequence.EmbeddedWallet;
using SequenceSDK.Samples;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Sequence.Boilerplates
{
    public static class BoilerplateFactory
    {
        public static void OpenSequenceLoginWindow(Transform parent)
        {
            GameObject loginWindowGameObject = (GameObject)Resources.Load("Prefabs/Login/SequenceLoginWindow");
            
            if (loginWindowGameObject != null)
            {
                GameObject window = Object.Instantiate(loginWindowGameObject, parent);
                window.GetComponent<SequenceLoginWindow>().Show(SequenceLogin.GetInstance());
            }
            else
            {
                throw new Exception("Prefab not found in Resources folder");
            }
        }
    }
}