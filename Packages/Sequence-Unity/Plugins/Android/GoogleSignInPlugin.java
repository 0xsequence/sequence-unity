package xyz.sequence;

import android.content.Context;
import android.util.Log;

import androidx.annotation.NonNull;
import androidx.credentials.Credential;
import androidx.credentials.CredentialManager;
import androidx.credentials.CredentialManagerCallback;
import androidx.credentials.CustomCredential;
import androidx.credentials.GetCredentialRequest;
import androidx.credentials.GetCredentialResponse;
import androidx.credentials.exceptions.GetCredentialException;
import androidx.credentials.exceptions.NoCredentialException;
import androidx.credentials.exceptions.GetCredentialCustomException;

import com.google.android.libraries.identity.googleid.GetSignInWithGoogleOption;
import com.google.android.libraries.identity.googleid.GoogleIdTokenCredential;

import java.security.SecureRandom;
import java.util.Base64;
import java.util.concurrent.Executors;

import com.unity3d.player.UnityPlayer;

public class GoogleSignInPlugin {
    private static final String TAG = "SequenceGoogleSignIn";

    public static void signIn(Context context, String clientId)
    {
        getCredentialAsync(context, clientId);
    }

    private static void getCredentialAsync(Context context, String clientId)
    {
        GetSignInWithGoogleOption googleIdOption = new GetSignInWithGoogleOption
            .Builder(clientId)
            .setNonce(generateNonce())
            .build();

        GetCredentialRequest request = new GetCredentialRequest.Builder().addCredentialOption(googleIdOption).build();

        CredentialManager credentialManager = CredentialManager.create(context);
        credentialManager.getCredentialAsync(
                context,
                request,
                null,
                Executors.newSingleThreadExecutor(),
                new CredentialManagerCallback<GetCredentialResponse, GetCredentialException>() {
                    @Override
                    public void onResult(GetCredentialResponse getCredentialResponse) {
                        handleGetCredentialResponse(getCredentialResponse);
                    }

                    @Override
                    public void onError(@NonNull GetCredentialException e) {
                         Log.e(TAG, "Error getting credential", e);
                         if (e instanceof GetCredentialCustomException) {
                             GetCredentialCustomException customException = (GetCredentialCustomException) e;
                             Log.e(TAG, "Custom Exception Type: " + customException.getType());
                        }
                        
                        UnityPlayer.UnitySendMessage("NativeGoogleSignInReceiver", "HandleError", "Error");
                    }
                }
        );
    }

    private static void handleGetCredentialResponse(GetCredentialResponse getCredentialResponse) {
        Credential credential = getCredentialResponse.getCredential();
        if (credential instanceof CustomCredential && GoogleIdTokenCredential.TYPE_GOOGLE_ID_TOKEN_CREDENTIAL.equals(credential.getType())) {
            try {
                GoogleIdTokenCredential idTokenCredential = GoogleIdTokenCredential.createFrom(credential.getData());
                String idToken = idTokenCredential.getIdToken();
                
                UnityPlayer.UnitySendMessage("NativeGoogleSignInReceiver", "HandleIdToken", idToken);
            } catch (Exception e) {
                Log.e(TAG, "Failed to parse Google ID token response", e);
            }
        } else {
            Log.e(TAG, "Unexpected credential type");
        }
    }
    
    private static String generateNonce() {
        SecureRandom random = new SecureRandom();
        byte[] nonce = new byte[32];
        random.nextBytes(nonce);
        return Base64.getEncoder().encodeToString(nonce);
    }
}