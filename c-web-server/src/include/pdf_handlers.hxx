#include <cstdio>

#include <functional>
#include "hpdf.h"

#include "utils.hxx"

#ifndef PDF_HANDLERS_H
#define PDF_HANDLERS_H


#ifndef BUFFER_CHUNK_SIZE
#define BUFFER_CHUNK_SIZE 1024
#endif

void error_handler (HPDF_STATUS   error_no,
                    HPDF_STATUS   detail_no,
                    void         *user_data)
{
    printf ("ERROR: error_no=%04X, detail_no=%u\n", (HPDF_UINT)error_no,
                (HPDF_UINT)detail_no);
}

int generate_pdf(function<void(char*,int)> callback) {
	
    HPDF_Doc  pdf = HPDF_New (error_handler, NULL);

    if (!pdf) {
        printf ("error: cannot create PdfDoc object\n");
        return 1;
    }

    HPDF_Page page = HPDF_AddPage (pdf);
    HPDF_REAL height = HPDF_Page_GetHeight (page);
    HPDF_REAL width = HPDF_Page_GetWidth (page);
    HPDF_Page_SetLineWidth (page, 1);

    HPDF_Font font = HPDF_GetFont (pdf, "Times-Roman", NULL);
    HPDF_Page_SetFontAndSize (page, font, 24);
    const char *page_title = concat("Generated Using LibHaru, time is ", now("%D %T"));
    HPDF_REAL tw = HPDF_Page_TextWidth (page, page_title);
    HPDF_Page_BeginText (page);
    HPDF_Page_TextOut (page, (width - tw) / 2, height - 50, page_title);
    HPDF_Page_EndText (page);

    /* save the document to a stream */
    HPDF_SaveToStream (pdf);
    fprintf (stderr, "the size of data is %d\n", HPDF_GetStreamSize(pdf));

    /* rewind the stream. */
    HPDF_ResetStream (pdf);

    /* get the data from the stream and output it to stdout. */
    for (int i=0;i<20;++i) {
        HPDF_BYTE buf[4096];
        HPDF_UINT32 siz = 4096;
        HPDF_STATUS read = HPDF_ReadFromStream (pdf, buf, &siz);
        callback((char*)buf,read);

        printf("%ld\n", read);

       if (siz == 0) {
            break;
       }

        // if (fwrite (buf, siz, 1, stdout) != 1) {
        //     fprintf (stderr, "cannot write to stdout %d", siz);
        //     //break;
        // }
    }

    HPDF_Free (pdf);

    return 0;
}


#endif