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
      window.addEventListener("resize", handleResize);
      handleResize()
      return () => {
        removeEventListener("GoogleSignIn", handleGoogleSignIn);
        window.removeEventListener("resize", handleResize);
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

    const handleResize = () => {
      const container = document.querySelector('.container') as any;

      let w = window.innerWidth * 0.98;
      let h = window.innerHeight * 0.98;

      const r = 600 / 960;
      if (w * r > window.innerHeight) {
        w = Math.min(w, Math.ceil(h / r));
      }

      h = Math.floor(w * r);

      container.style.width = w + "px";
      container.style.height = h + "px";
    }

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
