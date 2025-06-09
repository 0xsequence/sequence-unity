package xyz.sequence;

import android.content.Intent;
import android.os.Bundle;
import com.unity3d.player.UnityPlayerActivity;

public class SequenceUnityActivity extends UnityPlayerActivity {
    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);
        GoogleSignInPlugin.onActivityResult(requestCode, resultCode, data);
    }
}