package xyz.sequence;

import android.app.Activity;
import android.content.Intent;
import android.util.Log;

import androidx.annotation.Nullable;

import com.google.android.gms.auth.api.signin.*;
import com.google.android.gms.common.api.ApiException;
import com.google.android.gms.tasks.Task;

import com.unity3d.player.UnityPlayer;

public class GoogleSignInPlugin {
    private static final String TAG = "GoogleSignInPlugin";
    private static final int RC_SIGN_IN = 9001;
    private static GoogleSignInClient mGoogleSignInClient;
    private static Activity unityActivity;

    public static void initialize(Activity activity, String clientId) {
        unityActivity = activity;
        GoogleSignInOptions gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DEFAULT_SIGN_IN)
            .requestIdToken(clientId)
            .requestEmail()
            .build();

        mGoogleSignInClient = GoogleSignIn.getClient(activity, gso);
    }

    public static void signIn() {
        if (mGoogleSignInClient == null) {
            Log.e(TAG, "GoogleSignInClient is null. Did you call initialize()?");
            return;
        }
    
        Log.d(TAG, "Starting Google Sign-In intent");
    
        mGoogleSignInClient.revokeAccess().addOnCompleteListener(task -> {
            mGoogleSignInClient.signOut().addOnCompleteListener(signOutTask -> {
                Intent signInIntent = mGoogleSignInClient.getSignInIntent();
                unityActivity.startActivityForResult(signInIntent, RC_SIGN_IN);
            });
        });
    }

    public static void onActivityResult(int requestCode, int resultCode, @Nullable Intent data) {
        Log.d(TAG, "onActivityResult called with requestCode: " + requestCode + ", resultCode: " + resultCode);

        if (requestCode == RC_SIGN_IN) {
            Log.d(TAG, "Handling Google Sign-In result");
            Task<GoogleSignInAccount> task = GoogleSignIn.getSignedInAccountFromIntent(data);
            handleSignInResult(task);
        }
    }

    private static void handleSignInResult(Task<GoogleSignInAccount> completedTask) {
        try {
            GoogleSignInAccount account = completedTask.getResult(ApiException.class);
            String token = account.getIdToken();
            
            Log.d(TAG, "Sign-In success: ID token = " + token);
            UnityPlayer.UnitySendMessage("NativeGoogleSignInReceiver", "HandleIdToken", token);
        } catch (ApiException e) {
            Log.e(TAG, "Sign-In failed: code=" + e.getStatusCode() + " message=" + e.getMessage(), e);
            UnityPlayer.UnitySendMessage("NativeGoogleSignInReceiver", "HandleError", e.getMessage());
        }
    }
}