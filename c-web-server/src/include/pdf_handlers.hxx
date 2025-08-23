#include <cstdio>

#include <functional>
#include "hpdf.h"

#include "utils.hxx"

#ifndef PDF_HANDLERS_H
#define PDF_HANDLERS_H

#ifndef BUFFER_CHUNK_SIZE
#define BUFFER_CHUNK_SIZE 1024
#endif

const char* get_harhu_pdf_error_message(int error_number) {
    switch(error_number) {
    case 0x1073: return "HPDF_ANNOT_INVALID_BORDER_STYLE";           
    case 0x1072: return "HPDF_ANNOT_INVALID_ICON";                   
    case 0x1001: return "HPDF_ARRAY_COUNT_ERR";                      
    case 0x1002: return "HPDF_ARRAY_ITEM_NOT_FOUND";                 
    case 0x1003: return "HPDF_ARRAY_ITEM_UNEXPECTED_TYPE";           
    case 0x1004: return "HPDF_BINARY_LENGTH_ERR";                    
    case 0x1005: return "HPDF_CANNOT_GET_PALLET";                    
    case 0x1007: return "HPDF_DICT_COUNT_ERR";                       
    case 0x1008: return "HPDF_DICT_ITEM_NOT_FOUND";                  
    case 0x1009: return "HPDF_DICT_ITEM_UNEXPECTED_TYPE";            
    case 0x100A: return "HPDF_DICT_STREAM_LENGTH_NOT_FOUND";         
    case 0x100B: return "HPDF_DOC_ENCRYPTDICT_NOT_FOUND";            
    case 0x100C: return "HPDF_DOC_INVALID_OBJECT";                   
    case 0x100E: return "HPDF_DUPLICATE_REGISTRATION";               
    case 0x1011: return "HPDF_ENCRYPT_INVALID_PASSWORD";             
    case 0x1013: return "HPDF_ERR_UNKNOWN_CLASS";                    
    case 0x1014: return "HPDF_EXCEED_GSTATE_LIMIT";                  
    case 0x100F: return "HPDF_EXCEED_JWW_CODE_NUM_LIMIT";            
    case 0x1080: return "HPDF_EXT_GSTATE_OUT_OF_RANGE";              
    case 0x1082: return "HPDF_EXT_GSTATE_READ_ONLY";                 
    case 0x1015: return "HPDF_FAILED_TO_ALLOC_MEM";                  
    case 0x1016: return "HPDF_FILE_IO_ERROR";                        
    case 0x1017: return "HPDF_FILE_OPEN_ERROR";                      
    case 0x1019: return "HPDF_FONT_EXISTS";                          
    case 0x101A: return "HPDF_FONT_INVALID_WIDTHS_TABLE";            
    case 0x101B: return "HPDF_INVALID_AFM_HEADER";                   
    case 0x101C: return "HPDF_INVALID_ANNOTATION";                   
    case 0x101E: return "HPDF_INVALID_BIT_PER_COMPONENT";            
    case 0x101F: return "HPDF_INVALID_CHAR_MATRICS_DATA";            
    case 0x1020: return "HPDF_INVALID_COLOR_SPACE";                  
    case 0x1021: return "HPDF_INVALID_COMPRESSION_MODE";             
    case 0x1022: return "HPDF_INVALID_DATE_TIME";                    
    case 0x1023: return "HPDF_INVALID_DESTINATION";                  
    case 0x1025: return "HPDF_INVALID_DOCUMENT";                     
    case 0x1026: return "HPDF_INVALID_DOCUMENT_STATE";               
    case 0x1027: return "HPDF_INVALID_ENCODER";                      
    case 0x1028: return "HPDF_INVALID_ENCODER_TYPE";                 
    case 0x102B: return "HPDF_INVALID_ENCODING_NAME";                
    case 0x102C: return "HPDF_INVALID_ENCRYPT_KEY_LEN";              
    case 0x1081: return "HPDF_INVALID_EXT_GSTATE";                   
    case 0x1075: return "HPDF_INVALID_FONT";                         
    case 0x102D: return "HPDF_INVALID_FONTDEF_DATA";                 
    case 0x102E: return "HPDF_INVALID_FONTDEF_TYPE";                 
    case 0x102F: return "HPDF_INVALID_FONT_NAME";                    
    case 0x1085: return "HPDF_INVALID_ICC_COMPONENT_NUM";            
    case 0x1030: return "HPDF_INVALID_IMAGE";                        
    case 0x1031: return "HPDF_INVALID_JPEG_DATA";                    
    case 0x1032: return "HPDF_INVALID_N_DATA";                       
    case 0x1033: return "HPDF_INVALID_OBJECT";                       
    case 0x1034: return "HPDF_INVALID_OBJ_ID";                       
    case 0x1035: return "HPDF_INVALID_OPERATION";                    
    case 0x1036: return "HPDF_INVALID_OUTLINE";                      
    case 0x1037: return "HPDF_INVALID_PAGE";                         
    case 0x1038: return "HPDF_INVALID_PAGES";                        
    case 0x1067: return "HPDF_INVALID_PAGE_INDEX";                   
    case 0x1079: return "HPDF_INVALID_PAGE_SLIDESHOW_TYPE";          
    case 0x1039: return "HPDF_INVALID_PARAMETER";                    
    case 0x103B: return "HPDF_INVALID_PNG_IMAGE";                    
    case 0x103C: return "HPDF_INVALID_STREAM";                       
    case 0x103F: return "HPDF_INVALID_TTC_FILE";                     
    case 0x1040: return "HPDF_INVALID_TTC_INDEX";                    
    case 0x1083: return "HPDF_INVALID_U3D_DATA";                     
    case 0x1068: return "HPDF_INVALID_URI";                          
    case 0x1041: return "HPDF_INVALID_WX_DATA";                      
    case 0x1042: return "HPDF_ITEM_NOT_FOUND";                       
    case 0x1043: return "HPDF_LIBPNG_ERROR";                         
    case 0x103D: return "HPDF_MISSING_FILE_NAME_ENTRY";              
    case 0x1084: return "HPDF_NAME_CANNOT_GET_NAMES";                
    case 0x1044: return "HPDF_NAME_INVALID_VALUE";                   
    case 0x1045: return "HPDF_NAME_OUT_OF_RANGE";                    
    case 0x1049: return "HPDF_PAGES_MISSING_KIDS_ENTRY";             
    case 0x104A: return "HPDF_PAGE_CANNOT_FIND_OBJECT";              
    case 0x104B: return "HPDF_PAGE_CANNOT_GET_ROOT_PAGES";           
    case 0x104C: return "HPDF_PAGE_CANNOT_RESTORE_GSTATE";           
    case 0x104D: return "HPDF_PAGE_CANNOT_SET_PARENT";               
    case 0x104E: return "HPDF_PAGE_FONT_NOT_FOUND";                  
    case 0x1076: return "HPDF_PAGE_INSUFFICIENT_SPACE";              
    case 0x1086: return "HPDF_PAGE_INVALID_BOUNDARY";                
    case 0x1074: return "HPDF_PAGE_INVALID_DIRECTION";               
    case 0x1077: return "HPDF_PAGE_INVALID_DISPLAY_TIME";            
    case 0x104F: return "HPDF_PAGE_INVALID_FONT";                    
    case 0x1050: return "HPDF_PAGE_INVALID_FONT_SIZE";               
    case 0x1051: return "HPDF_PAGE_INVALID_GMODE";                   
    case 0x1052: return "HPDF_PAGE_INVALID_INDEX";                   
    case 0x1048: return "HPDF_PAGE_INVALID_PARAM_COUNT";             
    case 0x1053: return "HPDF_PAGE_INVALID_ROTATE_VALUE";            
    case 0x1054: return "HPDF_PAGE_INVALID_SIZE";                    
    case 0x1078: return "HPDF_PAGE_INVALID_TRANSITION_TIME";         
    case 0x1055: return "HPDF_PAGE_INVALID_XOBJECT";                 
    case 0x1069: return "HPDF_PAGE_LAYOUT_OUT_OF_RANGE";             
    case 0x1070: return "HPDF_PAGE_MODE_OUT_OF_RANGE";               
    case 0x1071: return "HPDF_PAGE_NUM_STYLE_OUT_OF_RANGE";          
    case 0x1056: return "HPDF_PAGE_OUT_OF_RANGE";                    
    case 0x1057: return "HPDF_REAL_OUT_OF_RANGE";                    
    case 0x1058: return "HPDF_STREAM_EOF";                           
    case 0x1059: return "HPDF_STREAM_READLN_CONTINUE";               
    case 0x105B: return "HPDF_STRING_OUT_OF_RANGE";                  
    case 0x105C: return "HPDF_THIS_FUNC_WAS_SKIPPED";                
    case 0x105D: return "HPDF_TTF_CANNOT_EMBEDDING_FONT";            
    case 0x105E: return "HPDF_TTF_INVALID_CMAP";                     
    case 0x105F: return "HPDF_TTF_INVALID_FOMAT";                    
    case 0x1060: return "HPDF_TTF_MISSING_TABLE";                    
    case 0x1061: return "HPDF_UNSUPPORTED_FONT_TYPE";                
    case 0x1062: return "HPDF_UNSUPPORTED_FUNC";                     
    case 0x1063: return "HPDF_UNSUPPORTED_JPEG_FORMAT";              
    case 0x1064: return "HPDF_UNSUPPORTED_TYPE1_FONT";               
    case 0x1065: return "HPDF_XREF_COUNT_ERR";                       
    case 0x1066: return "HPDF_ZLIB_ERROR";       
    }
    return "UNKNOWN";                    
}

void error_handler (HPDF_STATUS   error_no,
                    HPDF_STATUS   detail_no,
                    void         *user_data)
{
    printf ("%s[HARU PDF ERROR]: error_no=%04X, description=%s\n%s",
        KRED, 
        (HPDF_UINT)error_no, 
        get_harhu_pdf_error_message(error_no),
        KNRM);
}

enum PDF_Command_Type {
    RECTANGLE, CIRCLE, LINE, TEXT
};

typedef struct {
    float left;
    float top;
    float bottom;
    float right;
    unsigned int back_color;
    unsigned int front_color;
    unsigned int border_color;
    float botder_width;
    char* text;
    PDF_Command_Type command_type;
} PDF_Cmd;

enum PDF_PageLayout { P,L };
enum PDF_PageSize { A4,Legal };

typedef struct {
    PDF_PageLayout layout;
    PDF_PageSize size; 
    PDF_Cmd*    command;
} PDF_Page;

typedef struct {
    PDF_Page* pages;
} PDF_Document;

int generate_pdf(function<void(char*,int)> callback) {
//     PDF_Document pdf_document;
//     return _generate_pdf(pdf_document, callback);
// }

// int _generate_pdf(PDF_Document pdf_document, function<void(char*,int)> callback) {

    
	
    HPDF_Doc  pdf = HPDF_New (error_handler, NULL);
    //HPDF_SetCurrentEncoder(pdf, "WinAnsiEncoding");
    // auto font = HPDF_GetFont(pdf, HPDF_LoadTTFontFromFile(pdf, "./times.ttf", HPDF_TRUE), "StandardEncoding");



    if (!pdf) {
        printf ("%s[HPDF HARU ERROR] : cannot create PdfDoc object\n%s",KRED,KNRM);
        return 1;
    }

    // for(size_t i=0;i<ARRAY_LEN(pdf_document.pages);++i) {

    //     auto size = HPDF_PageSizes::HPDF_PAGE_SIZE_A4;
    //     auto layout = HPDF_PageDirection::HPDF_PAGE_PORTRAIT;
        HPDF_Page page = HPDF_AddPage (pdf);
        // HPDF_Page_SetSize(page,size,layout);
        HPDF_REAL height = HPDF_Page_GetHeight (page);
        HPDF_REAL width = HPDF_Page_GetWidth (page);
        HPDF_Page_SetLineWidth (page, 1);

        HPDF_Font font = HPDF_GetFont (pdf, "Times-Roman", NULL);
        HPDF_Page_SetFontAndSize (page, font, 24);
        const string page_title = concat("نص عربي Generated Using LibHaru, time is ", now("%D %T"));
        HPDF_REAL tw = HPDF_Page_TextWidth (page, page_title.c_str());
        HPDF_Page_BeginText (page);
        HPDF_Page_TextOut (page, (width - tw) / 2, height - 50, page_title.c_str());
        HPDF_Page_EndText (page);
    // }

    /* save the document to a stream */
    HPDF_SaveToStream (pdf);
    auto buffer_length = HPDF_GetStreamSize(pdf);
    //fprintf (stderr, "the size of data is %d\n", HPDF_GetStreamSize(pdf));

    /* rewind the stream. */
    HPDF_ResetStream (pdf);

    /* get the data from the stream and output it to stdout. */
    HPDF_BYTE buf[518];
    HPDF_UINT32 siz = 517;
    HPDF_STATUS status = 0;
    for (int i=0;i<20;++i) {
        status = HPDF_ReadFromStream (pdf, buf, &siz);
        buffer_length -= siz;
        cout << KCYN << "[---> ] " << KRED << siz << " / " << KNRM << status << " ' " << buffer_length << endl;
        callback((char*)buf,siz);

        if (status == HPDF_STREAM_EOF || siz == HPDF_OK) {
            break;
        }
    }

    HPDF_Free (pdf);

    return 0;
}


#endif