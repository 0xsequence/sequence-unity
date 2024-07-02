import { Unity, useUnityContext } from "react-unity-webgl";
import { useCallback, useEffect, useState } from "react";
import {
  GoogleOAuthProvider,
  GoogleLogin,
  CredentialResponse,
} from "@react-oauth/google";

import "./App.css";

  function App() {
    const {
      unityProvider,
      addEventListener,
      removeEventListener,
      sendMessage,
      isLoaded,
      loadingProgression,
    } = useUnityContext({
      loaderUrl: "Build/<ReplaceWithDirectoryName>.loader.js",
      dataUrl: "Build/<ReplaceWithDirectoryName>.data",
      frameworkUrl: "Build/<ReplaceWithDirectoryName>.framework.js",
      codeUrl: "Build/<ReplaceWithDirectoryName>.wasm",
    });

  const loadingPercentage = Math.round(loadingProgression * 100);

  const handleGoogleSignIn = useCallback((googleClientId: string, nonce: string) => {
    setGoogleClientId(googleClientId);
    setNonce(nonce);
    setShowLogin(true);
  }, []);

  const [googleClientIdState, setGoogleClientId] = useState("");
  const [nonce, setNonce] = useState("");

  const [messageToSend, setMessageToSend] = useState<
    | {
        functionName: string;
        value: string;
      }
    | undefined
  >();

  useEffect(() => {
    if (messageToSend) {
      const message = messageToSend;
      setMessageToSend(undefined);
      sendMessage("WebBrowserMessageReceiver", message.functionName, message.value);
    }
  }, [messageToSend]);

  useEffect(() => {
    addEventListener("GoogleSignIn", handleGoogleSignIn);
    return () => {
      removeEventListener("GoogleSignIn", handleGoogleSignIn);
    };
  }, []);

  const [showLogin, setShowLogin] = useState(false);

  const handleGoogleLogin = async (tokenResponse: CredentialResponse) => {
    setMessageToSend({
      functionName: "OnGoogleSignIn",
      value: tokenResponse.credential!,
    });

    setShowLogin(false);
  };

  return (
    <div className="outer-container">
      <div className="container">
        {isLoaded === false && (
          <div className="loading-overlay">
            <p>Loading... ({loadingPercentage}%)</p>
          </div>
        )}
        <Unity className="unity" unityProvider={unityProvider} />
      </div>

      {showLogin && (
        <div className="login-outer-container">
          <div className="login-container">
            <h2 className="login-title">Login with Google</h2>
            <div>
              <GoogleOAuthProvider clientId={googleClientIdState}>
                <GoogleLogin
                  onSuccess={(response) => {
                    handleGoogleLogin(response);
                  }}
                  shape="circle"
                  width={230}
                  nonce={nonce}
                />
              </GoogleOAuthProvider>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}

export default App;
