#libicudata.a
#libicui18n.a
#libicuio.a
#libicutu.a
#libicuuc.a
#libuuid.a
#libexpat.a
#libpkgconf.a
#libsharpyuv.a
#libturbojpeg.a
#libbentleyottmann.a
#libjsonreader.a
#libskottie.a
#libskparagraph.a
#libsksg.a
#libskshaper.a
#libskunicode_core.a
#libskunicode_icu.a
#libsvg.a

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
 -I ./skia/include/skia \
 -L./skia/lib \
 -l:libskia.a \
 -l:libfontconfig.a \
 -l:libfreetype.a \
 -l:libharfbuzz-subset.a \
 -l:libharfbuzz.a \
 -l:libpng16.a \
 -l:libbz2.a \
 -l:libjpeg.a \
 -l:libz.a \
 -l:libwebp.a \
 -l:libwebpdecoder.a \
 -l:libwebpdemux.a \
 -l:libwebpmux.a \
 -l:libbrotlidec.a \
 -l:libbrotlicommon.a \
 -l:libbrotlienc.a \
 -o ./bin/main \
 -fexceptions
