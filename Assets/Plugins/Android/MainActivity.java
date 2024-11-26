package xzy.sequence.app;

import android.content.Context;
import android.content.Intent;
import android.graphics.Color;
import android.graphics.Rect;
import android.net.Uri;
import android.os.Bundle;
import android.util.Log;

import com.unity3d.player.UnityPlayerActivity;


import xyz.sequence.AndroidKeyBridge;



public class MainActivity extends UnityPlayerActivity
{

    @Override
    protected void onResume() {
        super.onResume();
        AndroidKeyBridge.getInstance().init(this);
    }

    public void onDestroy() {
        super.onDestroy();
        AndroidKeyBridge.getInstance().destroy();
    }

}
