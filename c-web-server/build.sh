#!/bin/sh

rm -rf bin/app

g++ src/main.cxx \
 -std=c++17 \
 -ffast-math \
 -Wall \
 -Wextra \
 -Winit-self \
 -Wpointer-arith \
 -Wsign-compare \
 -Wvla \
 -Wno-deprecated-declarations \
 -Wno-uninitialized \
 -Wno-psabi \
 -Wno-switch-default \
 -Wno-unused-parameter \
 -Wno-unreachable-code \
 -Wunused \
 -fvisibility-inlines-hidden \
 -fno-rtti \
 -fPIC \
 -g \
 -Wnon-virtual-dtor \
 -Wno-noexcept-type \
 -o ./bin/app \
 -fsanitize=address,undefined -static-libasan
 bin/app
