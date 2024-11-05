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
        this.context = context;

        if (masterKey == null) {

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
                Log.d("Exception", e.getMessage());
            } catch (IOException e){
                Log.d("Exception", e.getMessage());
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
        sharedPreferences.edit().putString(key, value).apply();
    }

    private String RunGetKeychainValue(String key)
    {
        return sharedPreferences.getString(key, "");
    }
}
