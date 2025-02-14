#!/bin/bash
cp -r ./Assets/WebGLTemplates/SequenceReact/. ./Packages/Sequence-Unity/WebGLTemplates/SequenceReact
rm ./Packages/Sequence-Unity/WebGLTemplates/SequenceReact/**/*.meta
git checkout ./Packages/Sequence-Unity/WebGLTemplates/SequenceReact/**/*.meta
git checkout ./Packages/Sequence-Unity/WebGLTemplates/SequenceReact/*.meta
echo "Copied!"

