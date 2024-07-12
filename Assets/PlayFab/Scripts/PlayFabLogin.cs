using System.Collections;
using System.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using Sequence.Authentication;
using Sequence.WaaS;
using TMPro;
using UnityEngine;

namespace PlayFab.Scripts
{
    public class PlayFabLogin : MonoBehaviour
    {
        private enum LoginMethod
        {
            Guest,
            EmailPassword,
            UsernamePassword
        }

        [SerializeField] private string _email;
        [SerializeField] private string _password;
        [SerializeField] private string _username;
        [SerializeField] private string _titleId = "A1B62";
        private LoginMethod _loginMethod;

        private int _signOuts = 0;
        private IWallet _wallet;

        public void Start()
        {
            WaaSWallet.OnWaaSWalletCreated += OnWaaSWalletCreated;

            if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
            {
                PlayFabSettings.staticSettings.TitleId = _titleId;
            }

            PlayFabSettings.staticSettings.TitleId = "8F854"; // Todo remove this dev env credential
            
            SignIn(LoginMethod.Guest);
        }

        private void SignIn(LoginMethod loginMethod)
        {
            _loginMethod = loginMethod;
            switch (loginMethod)
            {
                case LoginMethod.Guest:
                    GuestLogin();
                    break;
                case LoginMethod.EmailPassword:
                    EmailPasswordLogin();
                    break;
                case LoginMethod.UsernamePassword:
                    UsernamePasswordLogin();
                    break;
                default:
                    Debug.Log("Done");
                    break;
            }
        }
    

        private void GuestLogin()
        {
            var request = new LoginWithCustomIDRequest { CustomId = "GettingStartedGuide", CreateAccount = true};
            PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
        }
        
        private void EmailPasswordLogin()
        {
            if (string.IsNullOrWhiteSpace(_email) || string.IsNullOrWhiteSpace(_password))
            {
                Debug.LogError("Email or password is empty");
                return;
            }
            var request = new LoginWithEmailAddressRequest { Email = _email, Password = _password };
            PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
        }
        
        private void UsernamePasswordLogin()
        {
            if (string.IsNullOrWhiteSpace(_username) || string.IsNullOrWhiteSpace(_password))
            {
                Debug.LogError("Username or password is empty");
                return;
            }
            var request = new LoginWithPlayFabRequest { Username = _username, Password = _password };
            PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnLoginFailure);
        }

        private void OnLoginSuccess(LoginResult result)
        {
            Debug.Log($"Logged in with PlayFab via {_loginMethod.ToString()} - connecting to WaaS now");

            string sessionTicket = result.SessionTicket;
            string titleId = PlayFabSettings.staticSettings.TitleId;
            
            ILogin login = WaaSLogin.GetInstance();
            login.OnLoginFailed += (error, method, email) =>
            {
                Debug.LogError(error);
            };
            login.PlayFabLogin(titleId, sessionTicket, GetEmail());
        }

        private string GetEmail()
        {
            string email;
            if (_loginMethod == LoginMethod.EmailPassword)
            {
                email = _email;
            }
            else if (_loginMethod == LoginMethod.UsernamePassword)
            {
                email = _username;
            }
            else
            {
                email = "";
            }

            return email;
        }

        private void OnLoginFailure(PlayFabError error)
        {
            string errorMessage = error.GenerateErrorReport();
            if (errorMessage.Contains("User not found"))
            {
                Debug.Log("User not found, creating new user");
                RegisterUser();
                return;
            }
            Debug.LogError(errorMessage);
        }
        
        private void RegisterUser()
        {
            RegisterPlayFabUserRequest request = null;
            if (_loginMethod == LoginMethod.EmailPassword)
            {
                request = new RegisterPlayFabUserRequest { Email = _email, Password = _password, RequireBothUsernameAndEmail = false };
            }
            else if (_loginMethod == LoginMethod.UsernamePassword)
            {
                request = new RegisterPlayFabUserRequest { Password = _password, Username = _username, Email = _email };
            }
            else
            {
                Debug.LogError("Cannot register user with Guest login");
                return;
            }
            PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFailure);
        }
        
        private void OnRegisterSuccess(RegisterPlayFabUserResult result)
        {
            Debug.Log("Registered new user with PlayFab - connecting to WaaS now");

            string sessionTicket = result.SessionTicket;
            string titleId = PlayFabSettings.staticSettings.TitleId;
            
            ILogin login = WaaSLogin.GetInstance();
            login.OnLoginFailed += (error, method, email) =>
            {
                Debug.LogError(error);
            };
            login.PlayFabLogin(titleId, sessionTicket, GetEmail());
        }
        
        private void OnRegisterFailure(PlayFabError error)
        {
            string errorMessage = error.GenerateErrorReport();
            Debug.LogError(errorMessage);
        }

        private void OnWaaSWalletCreated(WaaSWallet wallet)
        {
            TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();
            string email = PlayerPrefs.GetString(OpenIdAuthenticator.LoginEmail);
            if (string.IsNullOrWhiteSpace(email))
            {
                text.text = "Logged in as: " + wallet.GetWalletAddress();
            }
            else
            {
                text.text = "Logged in as: " + email;
            }
            _wallet = wallet;
            
            StartCoroutine(SignOutThenSignBackIn(_loginMethod));
        }

        private IEnumerator SignOutThenSignBackIn(LoginMethod loginMethod)
        {
            yield return new WaitForSeconds(3f);
            
            TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();
            text.text = "Logged out";

            Task signOutTask = _wallet.DropThisSession();
            yield return new WaitUntil(() => signOutTask.IsCompleted);
            
            if (_signOuts > 1)
            {
                _signOuts = 0;
                SignIn(loginMethod + 1);
                yield return null;
            }
            else
            {
                _signOuts++;
                _wallet = null;
                
                SignIn(loginMethod);
            }
        }
    }
}