using System.Collections;
using UnityEngine;

namespace Sequence.Demo
{
    public class UIPanel : UIPage
    {
        public UIPage InitialPage;
        
        public virtual IEnumerator OpenInitialPage(params object[] openArgs)
        {
            yield return new WaitForSeconds(base._openAnimationDurationInSeconds);
            InitialPage.Open(openArgs);
        }
    }
}