using System;
using Sequence.Authentication;
using Sequence.Utils;

namespace Sequence.Demo
{
    public class NewAccountConfirmationPage :UIPage
    {
        private FederatedAuthPopupPanel _federatedAuthPopupPanel;
        private ILogin _login;

        public override void Open(params object[] args)
        {
            base.Open(args);

            _federatedAuthPopupPanel = args.GetObjectOfTypeIfExists<FederatedAuthPopupPanel>();
            if (_federatedAuthPopupPanel == default)
            {
                throw new SystemException(
                    $"Invalid use. {GetType().Name} must be opened with a {typeof(FederatedAuthPopupPanel)} as an argument");
            }
            
            _login = args.GetObjectOfTypeIfExists<ILogin>();
            if (_login == default)
            {
                throw new SystemException(
                    $"Invalid use. {GetType().Name} must be opened with a {typeof(ILogin)} as an argument");
            }
        }

        public void NewAccount()
        {
            _login.ForceCreateAccount();
            _federatedAuthPopupPanel.Close();
        }

        public void Cancel()
        {
            _federatedAuthPopupPanel.ReturnToLogin();
        }
    }
}