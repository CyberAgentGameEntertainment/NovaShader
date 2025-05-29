#!/bin/bash

# Script to remove the com.apple.quarantine attribute
# from a file specified with a relative path
#
# Note: This script uses the 'flip' tool which is licensed under the BSD 3-Clause License.
# The full license text can be found in the tool's documentation.

# Target file relative path
TARGET_RELATIVE_PATH="Assets/Tests/bin/Mac/flip"

# Convert to absolute path based on the script's directory
SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
TARGET_PATH="$SCRIPT_DIR/$TARGET_RELATIVE_PATH"

# Check: Does the file exist?
if [ ! -e "$TARGET_PATH" ]; then
  echo "Target file does not exist: $TARGET_PATH"
  exit 1
fi

# Remove quarantine attribute
echo "Removing quarantine attribute: $TARGET_PATH"
xattr -d com.apple.quarantine "$TARGET_PATH" 2>/dev/null

# Verify result
if xattr "$TARGET_PATH" | grep -q "com.apple.quarantine"; then
  echo "Quarantine attribute still exists (may need elevated privileges)"
  echo "Please try again with sudo:"
  echo "sudo $0"
  exit 1
else
  echo "Quarantine attribute was successfully removed"
fi