#!/bin/bash

set -e

echo "=== TestFlight Upload Script Starting ==="

# Only upload if enabled
if [ "$ENABLE_TESTFLIGHT_UPLOAD" != "TRUE" ]; then
    echo "ENABLE_TESTFLIGHT_UPLOAD != TRUE, skipping upload."
    exit 0
fi

# Workspace (Unity Cloud Build sets this)
WORKSPACE="${WORKSPACE:-}"

if [ -z "$WORKSPACE" ]; then
    echo "ERROR: WORKSPACE environment variable is not set."
    exit 1
fi

echo "Workspace: $WORKSPACE"


# ----------------------------
# Locate IPA file
# ----------------------------

IPA_DIR="$WORKSPACE/.build/last/ios-production"
IPA_PATH="$IPA_DIR/build.ipa"

echo "Looking for IPA at: $IPA_PATH"

if [ ! -f "$IPA_PATH" ]; then
    echo "IPA not found directly. Searching recursively under workspace..."
    IPA_PATH=$(find "$WORKSPACE" -type f -name "*.ipa" | head -n 1)
fi

if [ -z "$IPA_PATH" ]; then
    echo "ERROR: No IPA file found anywhere under $WORKSPACE"
    exit 1
fi

echo "Using IPA file: $IPA_PATH"


# ----------------------------
# Write the .p8 key to disk
# ----------------------------

if [ -z "$APPSTORE_CONNECT_P8" ]; then
    echo "ERROR: APPSTORE_CONNECT_P8 environment variable missing."
    exit 1
fi

if [ -z "$APPSTORE_CONNECT_KEY_ID" ]; then
    echo "ERROR: APPSTORE_CONNECT_KEY_ID missing."
    exit 1
fi

if [ -z "$APPSTORE_CONNECT_ISSUER_ID" ]; then
    echo "ERROR: APPSTORE_CONNECT_ISSUER_ID missing."
    exit 1
fi

# Replace literal "\n" with real newlines
P8_CONTENT=$(printf "%b" "$APPSTORE_CONNECT_P8")

KEY_DIR="$HOME/.appstoreconnect/private_keys"
KEY_PATH="$KEY_DIR/AuthKey_${APPSTORE_CONNECT_KEY_ID}.p8"

mkdir -p "$KEY_DIR"

echo "Writing App Store Connect private key to:"
echo "$KEY_PATH"

printf "%b" "$P8_CONTENT" > "$KEY_PATH"


# ----------------------------
# Upload using altool
# ----------------------------

echo "Uploading IPA to TestFlight via altool…"

UPLOAD_CMD="xcrun altool --upload-app \
    -f \"$IPA_PATH\" \
    -t ios \
    --apiKey $APPSTORE_CONNECT_KEY_ID \
    --apiIssuer $APPSTORE_CONNECT_ISSUER_ID"

echo "Running: $UPLOAD_CMD"

if eval $UPLOAD_CMD; then
    echo "=== Upload IPA to Appstore Connect finished successfully ==="
else
    echo "=== Upload IPA to Appstore Connect FAILED ==="
    exit 1
fi

echo "=== TestFlight Upload Script Completed ==="