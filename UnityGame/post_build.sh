#!/bin/sh

cd ./GameClient
set -x

export FIREBASE_BUILD="$(find -E . -regex '.*\.(ipa|apk)' -print -quit)"
if [ -z "$FIREBASE_BUILD" ]; then
  echo "Could not find .ipa/.apk file"
  exit 1
fi

npm install
npm run distribute