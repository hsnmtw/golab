#cc main.c -Wextra -Wall -Wunused -O3 -Os -o ./bin/main
rm -rf bin/main
g++ src/*.cxx \
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
 -O3 \
 -Os \
 -fvisibility-inlines-hidden \
 -fno-exceptions \
 -fno-rtti \
 -fPIC \
 -s \
 -g0 \
 -Wnon-virtual-dtor \
 -Wno-noexcept-type \
 -I ./../../libharu/include \
 -L./../../libharu/src \
 -l:libhpdf.so \
 -o ./bin/main \
 -fexceptions

export LD_LIBRARY_PATH=$LD_LIBRARY_PATH:../../libharu/src/
bin/main