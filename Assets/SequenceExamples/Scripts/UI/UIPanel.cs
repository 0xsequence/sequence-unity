using System.Collections;
using UnityEngine;

namespace Sequence.Demo
{
    public class UIPanel : UIPage
    {
        public IEnumerator OpenInitialPage(UIPage page)
        {
            yield return new WaitForSeconds(base._openAnimationDurationInSeconds);
            page.Open();
        }
    }
}