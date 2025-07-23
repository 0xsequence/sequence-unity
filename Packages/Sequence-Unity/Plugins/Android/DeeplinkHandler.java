package xyz.sequence;

import android.app.Activity;
import android.content.Intent;
import android.net.Uri;
import com.unity3d.player.UnityPlayer;

public class DeeplinkHandler {
    public static void checkIntent(Activity activity) {
        Intent intent = activity.getIntent();
        Uri data = intent.getData();
        if (data != null) {
            String url = data.toString();
            UnityPlayer.UnitySendMessage("SequenceNativeReceiver", "HandleResponse", url);
        }
    }
}