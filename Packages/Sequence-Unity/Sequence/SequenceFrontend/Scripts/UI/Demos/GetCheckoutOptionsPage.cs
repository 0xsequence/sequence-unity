using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Sequence.Demo
{
    public class GetCheckoutOptionsPage : UIPage
    {
        [SerializeField] Button [] checkoutOptionButton;
        public override void Open(params object[] args)
        {
            base.Open(args);

            GetCheckoutOptions(0);
        }
       
        public void GetCheckoutOptions(int randomOptions) 
        {
            var options = new GetCheckoutOptionsMock().GetCheckoutOptions(randomOptions);

            foreach (var option in options)
            {
                checkoutOptionButton[(int)option].gameObject.SetActive(true);   
            }
        }

        
    }

}

