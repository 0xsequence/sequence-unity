#!/bin/bash

# Check if the input argument is provided
if [ -z "$1" ]; then
  echo "Usage: $0 <hex_string>"
  exit 1
fi

# Function to split hex string into lines of 32 bytes
function split_hex_string {
  local hex_string=$1
  local line_length=64
  local offset=0

  while [ $offset -lt ${#hex_string} ]; do
    echo "${hex_string:$offset:$line_length}"
    offset=$((offset + line_length))
  done
}

# Get the input hex string from the command-line argument
input_hex_string="$1"

# Call the function and redirect output to a txt file
split_hex_string "$input_hex_string" > output.txt

echo "Splitting complete. The result is saved in 'output.txt'."

