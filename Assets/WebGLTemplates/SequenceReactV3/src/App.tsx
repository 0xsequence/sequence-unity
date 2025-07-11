import {ReactUnityEventParameter} from "react-unity-webgl/distribution/types/react-unity-event-parameters";
import { Unity, useUnityContext } from "react-unity-webgl";
import { useCallback, useEffect, useState } from "react";

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
    loaderUrl: "Build/webgl.loader.js",
    dataUrl: "Build/webgl.data.unityweb",
    frameworkUrl: "Build/webgl.framework.js.unityweb",
    codeUrl: "Build/webgl.wasm.unityweb",
  });

  const loadingPercentage = Math.round(loadingProgression * 100);

  const handleSequenceWalletAuth = useCallback((...parameters: ReactUnityEventParameter[]): ReactUnityEventParameter => {
    const inputJson = parameters[0] as string;
    const input = JSON.parse(inputJson);

    const walletWindow = window.open(input.url, "Wallet", 'width=600,height=400,left=200,top=200');
    if (!walletWindow) {
      throw new Error("Unable to find wallet");
    }

    const message = {
      id: 'id-123',
      type: 'request',
      action: input.action,
      payload: input.payload
    }

    walletWindow.postMessage(JSON.stringify(message));

    /*setMessageToSend({
      functionName: "HandleResponse",
      value: walletUrl,
    });*/
    return '';
  }, []);

  const [messageToSend, setMessageToSend] = useState<{ functionName: string; value: string; } | undefined>(undefined);

  useEffect(() => {
    if (messageToSend) {
      const message = messageToSend;
      setMessageToSend(undefined);
      sendMessage("SequenceNativeReceiver", message.functionName, message.value);
    }
  }, [messageToSend]);

  useEffect(() => {
    addEventListener("OpenWalletApp", handleSequenceWalletAuth);
    window.addEventListener("message", handleMessage);
    window.addEventListener("resize", handleResize);
    handleResize()
    return () => {
      removeEventListener("OpenWalletApp", handleSequenceWalletAuth);
      window.removeEventListener("message", handleMessage);
      window.removeEventListener("resize", handleResize);
    };
  }, []);

  const handleMessage = (e) => {
    console.log(e);
  }

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
      </div>
  );
}

export default App;
