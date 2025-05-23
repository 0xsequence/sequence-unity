package xyz.sequence;

import android.content.SharedPreferences;
import android.util.Log;
import android.content.Context;
import android.widget.Toast;
import androidx.security.crypto.EncryptedSharedPreferences;
import androidx.security.crypto.MasterKey;

import java.io.IOException;
import java.security.GeneralSecurityException;


public class AndroidKeyBridge {

    private static AndroidKeyBridge instance = null;
    private Context context = null;
    private MasterKey masterKey = null;
    private SharedPreferences sharedPreferences;

    private AndroidKeyBridge()
    { 
        
    }

    public void init(Context context)
    {
        if (context == null) {
            Log.w("AndroidKeyBridge", "Provided context is null, trying to get UnityPlayer.currentActivity");
            try {
                context = com.unity3d.player.UnityPlayer.currentActivity;
            } catch (Exception e) {
                Log.e("AndroidKeyBridge", "Failed to get UnityPlayer.currentActivity", e);
            }
        }
    
        if (context == null) {
            Log.e("AndroidKeyBridge", "Context is still null, cannot initialize AndroidKeyBridge");
            return;
        }
    
        this.context = context;

        if (masterKey == null || sharedPreferences == null) {

            try {
            masterKey = new MasterKey.Builder(context)
                    .setKeyScheme(MasterKey.KeyScheme.AES256_GCM)
                    .build();
            sharedPreferences = EncryptedSharedPreferences.create(
                    context,
                    "secret_shared_prefs",
                    masterKey,
                    EncryptedSharedPreferences.PrefKeyEncryptionScheme.AES256_SIV,
                    EncryptedSharedPreferences.PrefValueEncryptionScheme.AES256_GCM
                );
            } catch (GeneralSecurityException e){
                Log.d("AndroidKeyBridge", "Encountered error when initializing AndroidKeyBridge: " + e.getMessage());
            } catch (IOException e){
                Log.d("AndroidKeyBridge", "Encountered error when initializing AndroidKeyBridge: " + e.getMessage());
            }
        }
    }

    public void destroy()
    {
        this.context = null;
    }

    public static AndroidKeyBridge getInstance()
    {
        if (instance == null)
            instance = new AndroidKeyBridge();

        return instance;
    }

    public static void SaveKeychainValue(String key, String value)
    {
        getInstance().RunSaveKeychainValue(key, value);
    }

    public static String GetKeychainValue(String key)
    {
        return getInstance().RunGetKeychainValue(key);
    }

    private void RunSaveKeychainValue(String key, String value)
    {
        if (masterKey == null || sharedPreferences == null) {
            init(context);
        }
        sharedPreferences.edit().putString(key, value).apply();
    }

    private String RunGetKeychainValue(String key)
    {
        if (masterKey == null || sharedPreferences == null) {
            init(context);
        }
        return sharedPreferences.getString(key, "");
    }
}
