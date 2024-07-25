using System;
using System.Collections.Generic;
using Sequence.Authentication;
using Sequence.Config;
using Sequence.Utils;
using TMPro;
using UnityEngine;

namespace Sequence.Demo
{
    public class FederatedAuthExplanationPage : UIPage
    {
        [SerializeField] private GameObject _overrideAccountButton;
        [SerializeField] private TextMeshProUGUI _explanationText;
        
        private bool _enableMultipleAccountsPerEmail;
        private FederatedAuthPopupPanel _federatedAuthPopupPanel;

        public override void Open(params object[] args)
        {
            base.Open(args);
            
            _federatedAuthPopupPanel = args.GetObjectOfTypeIfExists<FederatedAuthPopupPanel>();
            if (_federatedAuthPopupPanel == default)
            {
                throw new SystemException(
                    $"Invalid use. {GetType().Name} must be opened with a {typeof(FederatedAuthPopupPanel)} as an argument");
            }
            LoginMethod loginMethod = args.GetObjectOfTypeIfExists<LoginMethod>();
            if (loginMethod == LoginMethod.None)
            {
                throw new SystemException(
                    $"Invalid use. {GetType().Name} must be opened with a {typeof(LoginMethod)} as an argument");
            }
            List<LoginMethod> loginMethods = args.GetObjectOfTypeIfExists<List<LoginMethod>>();
            if (loginMethods == default)
            {
                throw new SystemException(
                    $"Invalid use. {GetType().Name} must be opened with a {typeof(List<LoginMethod>)} as an argument");
            }

            _enableMultipleAccountsPerEmail = SequenceConfig.GetConfig().EnableMultipleAccountsPerEmail;

            
            _overrideAccountButton.SetActive(_enableMultipleAccountsPerEmail);
            string explanation =
                $"You've already used this email to sign in with another method. If you'd like to be able to sign in with {loginMethod} in the future, you'll need to login with ";
            int methods = loginMethods.Count;
            if (methods > 1)
            {
                explanation += " one of: ";
                for (int i = 0; i < methods - 1; i++)
                {
                    if (i > 0)
                    {
                        explanation += ", ";
                    }
                    explanation += loginMethods[i];
                }
                explanation += ", or " + loginMethods[methods - 1];
            }
            else if (methods == 0)
            {
                explanation += "that method";
            }
            else
            {
                explanation += loginMethods[0];
            }
            explanation += " first.";

            if (_enableMultipleAccountsPerEmail)
            {
                explanation += "\nAlternatively, we can the delete the account associated with this email and create a new one for you.";
            }
            
            _explanationText.text = explanation;
        }

        public void ReturnToLogin()
        {
            _federatedAuthPopupPanel.ReturnToLogin();
        }

        public void NewAccount()
        {
            if (!SequenceConfig.GetConfig().EnableMultipleAccountsPerEmail)
            {
                throw new SystemException("Creating accounts with the same email is not enabled");
            }
            _federatedAuthPopupPanel.OverrideAccount();
        }
    }
}