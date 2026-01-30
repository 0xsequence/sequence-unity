import {ReactUnityEventParameter} from "react-unity-webgl/distribution/types/react-unity-event-parameters";
import { Unity, useUnityContext } from "react-unity-webgl";
import { useCallback, useEffect, useState } from "react";
import {
    GoogleOAuthProvider,
    GoogleLogin,
    CredentialResponse,
} from "@react-oauth/google";

import "./App.css";

let walletWindow: Window | null = null;
let authInput: AuthInput | null = null;

interface AuthInput {
    url: string;
    action: string;
    payload: string;
}

function App() {
    const {
        unityProvider,
        addEventListener,
        removeEventListener,
        sendMessage,
        isLoaded,
        loadingProgression,
    } = useUnityContext({
        loaderUrl: "Build/build.loader.js",
        dataUrl: "Build/build.data.unityweb",
        frameworkUrl: "Build/build.framework.js.unityweb",
        codeUrl: "Build/build.wasm.unityweb",
    });

    const loadingPercentage = Math.round(loadingProgression * 100);

    const handleSequenceWalletAuth = useCallback((...parameters: ReactUnityEventParameter[]): ReactUnityEventParameter => {
        const inputJson = parameters[0] as string;
        authInput = JSON.parse(inputJson) as AuthInput;

        const sessionId = generateId();
        walletWindow = window.open(
            `${authInput?.url}?dappOrigin=${window.location.origin}&sessionId=${sessionId}`,
            "Wallet",
            'width=600,height=600,left=300,top=300');

        return '';
    }, []);

    const [messageToSend, setMessageToSend] = useState<{ gameObject: string; functionName: string; value: string; } | undefined>(undefined);

    useEffect(() => {
        if (messageToSend) {
            const message = messageToSend;
            setMessageToSend(undefined);
            sendMessage(message.gameObject, message.functionName, message.value);
        }
    }, [messageToSend]);

    useEffect(() => {
        addEventListener("GoogleSignIn", handleGoogleSignIn);
        addEventListener("OpenWalletApp", handleSequenceWalletAuth);
        window.addEventListener("message", handleMessage);
        window.addEventListener("resize", handleResize);
        handleResize()
        return () => {
            removeEventListener("GoogleSignIn", handleGoogleSignIn);
            removeEventListener("OpenWalletApp", handleSequenceWalletAuth);
            window.removeEventListener("message", handleMessage);
            window.removeEventListener("resize", handleResize);
        };
    }, []);

    const handleGoogleSignIn = useCallback((...parameters: ReactUnityEventParameter[]): ReactUnityEventParameter => {
        const googleClientId = parameters[0] as string;
        const nonce = parameters[1] as string;
        setGoogleClientId(googleClientId);
        setNonce(nonce);
        setShowLogin(true);
        return '';
    }, []);

    const handleGoogleLogin = async (tokenResponse: CredentialResponse) => {
        setMessageToSend({
            gameObject: "WebBrowserMessageReceiver",
            functionName: "OnGoogleSignIn",
            value: tokenResponse.credential!,
        });

        setShowLogin(false);
    };

    const [googleClientIdState, setGoogleClientId] = useState("");
    const [nonce, setNonce] = useState("");
    const [showLogin, setShowLogin] = useState(false);

    const handleMessage = async (event: MessageEvent) => {
        if (!walletWindow) {
            return;
        }

        switch (event.data.type) {
            case "WALLET_OPENED":
                postMessageToWallet({
                    id: generateId(),
                    type: 'INIT',
                    sessionId: 'mcyc0abl-8q11zpb',
                });

                console.log(authInput)
                postMessageToWallet({
                    id: generateId(),
                    type: 'REQUEST',
                    action: authInput?.action,
                    payload: authInput?.payload
                });

                console.log('sent init message')
                break;
            case "RESPONSE":
                let data = event.data;
                if (data.payload) {
                    const parsedPayload = JSON.stringify(data.payload, (_, v) => {
                        if (typeof v === 'bigint') {
                            return {_isBigInt: true, data: v.toString()};
                        } else if (v instanceof Uint8Array) {
                            return {_isUint8Array: true, data: bytesToHex(v)};
                        } else {
                            return v;
                        }
                    });

                    data = {...data, payload: btoa(parsedPayload)};
                }

                console.log(data);

                setMessageToSend({
                    gameObject: "SequenceNativeReceiver",
                    functionName: "HandleResponse",
                    value: JSON.stringify(data)
                });

                walletWindow.close();
                break;
        }
    }

    function bytesToHex(bytes: Uint8Array): string {
        return '0x' + Array.from(bytes)
            .map(b => b.toString(16).padStart(2, "0"))
            .join("");
    }

    const postMessageToWallet = (message: any) => {
        try {
            if (!walletWindow) {
                throw new Error("Unable to find wallet");
            }

            const walletOrigin = new URL(authInput?.url || '').origin;
            walletWindow.postMessage(message, walletOrigin);
        } catch (e) {
            console.error(e);
        }
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

    const generateId = (): string =>  {
        return `${Date.now().toString(36)}-${Math.random()
            .toString(36)
            .substring(2, 9)}`;
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
