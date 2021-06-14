#!/bin/sh

cd ./UnityGame/
set -x

export FIREBASE_BUILD="$(find -E . -regex '.*\.(ipa|apk)' -print -quit)"
if [ -z "$FIREBASE_BUILD" ]; then
  echo "Could not find .ipa/.apk file"
  exit 1
fi

export RELEASE_NOTES="$(git log -1 --pretty=%B)"

npm install
npm run distribute