using System.Collections;
using UnityEngine;

namespace Sequence.Demo
{
    public class UIPanel : UIPage
    {
        public UIPage InitialPage;
        
        public IEnumerator OpenInitialPage()
        {
            yield return new WaitForSeconds(base._openAnimationDurationInSeconds);
            InitialPage.Open();
        }
    }
}