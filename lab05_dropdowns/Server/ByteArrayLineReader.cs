
using System.IO;
using System;
using System.Linq;
using System.Collections.Generic;

namespace WebApp.Http{

public sealed class ByteArrayLineReader {
    private int p;
    private Stream _stream;
    private byte[] _buffer;
    int _contentLength;
    int _read;
    public ByteArrayLineReader(Stream stream, int contentLength, byte[] initial) {
        _stream = stream;
        _buffer = initial;
        _contentLength = contentLength;
        _read = initial.Length;
        p = 0;
    }

    public int ContentLengthRead { 
        get { return _read; }
    }

    public bool Done {
        get { 
            return p>=_buffer.Length && (_contentLength<1 || _read>=_contentLength);
        }
    }

    public byte[] NextLine() {
        if (Done) return Array.Empty<byte>();        
 
        for(int i=p;i<_buffer.Length;i++) {
            if ((char)_buffer[i] == '\n') {
                var ret = Slice(_buffer,p,++i);
                p = i;
                return ret;
            }
        }
    
        //getting here means partial read,
        if(_read<_contentLength) {
            var sofar = p<_buffer.Length ? Slice(_buffer,p,_buffer.Length-1) : Array.Empty<byte>();
            int r = _stream.Read(_buffer,0,_buffer.Length);
            if(r>0) {
                _read+=r;
                _buffer = Concatenate(sofar,Slice(_buffer,0,r));
                p = 0;
                return NextLine();
            }
        }

        return Array.Empty<byte>();
    }

    private static byte[] Slice(byte[] buffer, int start, int end) {
        byte[] copy = new byte[end-start];
        int p = 0;
        for(int i=start;i<end;++i) {
            copy[p++] = buffer[i];
        }
        return copy;
    }

    // static void showLine(
    // [CallerLineNumber] int lineNumber = 0,
    // [CallerMemberName] string? caller = null)
    // {
    //     //System.Console.WriteLine(" at line " + lineNumber + " (" + caller + ")");
    // }
}
}