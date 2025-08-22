#include <cstdio>
#include "core/SkCanvas.h"
#include "core/SkPaint.h"
#include "core/SkFont.h"
#include "core/SkScalar.h"
#include "core/SkFontStyle.h"
#include "core/SkFontMgr.h"
#include "core/SkTypeface.h"
#include "core/SkPath.h"
#include "include/core/SkStream.h"
#include "modules/skparagraph/include/FontCollection.h"
#include "include/docs/SkPDFDocument.h"
#include <functional>

#include "utils.hxx"

#ifndef PDF_HANDLERS_H
#define PDF_HANDLERS_H


#ifndef BUFFER_CHUNK_SIZE
#define BUFFER_CHUNK_SIZE 1024
#endif

void generate_pdf(function<void(char*,int)> callback) {
	
	SkPDF::Metadata metadata;
	// metadata.fTitle = "test pdf";
    // metadata.fCreator = "Example WritePDF() Function";
    // metadata.fCreation = {0, 2019, 1, 4, 31, 12, 34, 56};
    // metadata.fModified = {0, 2019, 1, 4, 31, 12, 34, 56};
    // See also SkPDF::JPEG::MetadataWithCallbacks()
    // metadata.jpegDecoder = SkPDF::JPEG::Decode;
    // metadata.jpegEncoder = SkPDF::JPEG::Encode;
	//SkFILEWStream buffer = SkFILEWStream("./test.pdf");
    auto stream = SkDynamicMemoryWStream();
	auto pdfDocument = SkPDF::MakeDocument(&stream, metadata);
	
	SkCanvas* canvas = pdfDocument->beginPage(500,800);
	canvas->drawColor(SK_ColorBLACK); // clear the canvas

	// const SkScalar R = 115.2f, C = 128.0f;
    // SkPath path;
    // path.moveTo(C + R, C);
    // for (int i = 1; i < 8; ++i) {
    //     SkScalar a = 2.6927937f * i;
    //     path.lineTo(C + R * cos(a), C + R * sin(a));
    // }
    SkPaint paint;
    paint.setAntiAlias(true);
    paint.setColor(SK_ColorRED);

    // auto c = sk_make_sp<skia::textlayout::FontCollection>();
    // sk_sp<SkFontMgr> fontMgr;
    // std::vector<SkString> defaultFamilyNames;
    // c->setDefaultFontManager(fontMgr,defaultFamilyNames);
    // auto typeface = fontMgr.release()->makeFromFile("./times.ttf"); //skFontManager.get().matchFamilyStyle("Arial", SkFontStyle(SkFontStyle::kBold_Weight, SkFontStyle::kCondensed_Width, SkFontStyle::kUpright_Slant));
    // SkFont font;
    // font.setSize(64);
    // font.setTypeface(typeface);

    // SkString text("Hello World"); //concat("Some text in pdf showing time ", now("%Y-%m-%d %H:%M:%S")));
    // canvas->save();
    
    // auto skFontMgr = SkFontMgr;
    // auto typeface = skFontMgr->matchFamilyStyle(nullptr, SkFontStyle::Bold());
    // auto typeface = skFontMgr.get()->makeFromFile("./times.ttf");
    // auto typeface = skFontMgr->legacyMakeTypeface("Times New Roman",  SkFontStyle::Bold()   );
    // SkFont font = SkFont(typeface, SkScalar(20));
    // canvas->drawSimpleText(text.c_str(), text.size(), SkTextEncoding::kUTF8,10,10, font,paint);
    // canvas->save();
	pdfDocument->endPage();

    pdfDocument->close();
	stream.flush();

	char buffer[BUFFER_CHUNK_SIZE];
	int length = stream.bytesWritten();
	int offset=0, remaining = std::min(length,BUFFER_CHUNK_SIZE);
	while(stream.read(&buffer,offset,remaining)){
		callback(buffer,remaining);
		offset+=remaining;
	}
}


#endif