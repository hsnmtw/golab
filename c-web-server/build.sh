rm -rf bin/app
export LD_PRELOAD=../../libharu/src/libhpdf.so.2.4
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
 -I ./../../libharu/include \
 -L./../../libharu/src \
 -l:libhpdf.so \
 -o ./bin/app \
 -fsanitize=address -static-libasan
 
 bin/app