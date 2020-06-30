using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MultiplePasswordsEncrypter
{
    // Multiple Passwords Encrypted file reader.
    public class MPEHeaderReader
    {
        private readonly byte[] _buf;
        private int _pos = 0;

        public MPEHeaderReader(byte[] buf)
        {
            _buf = buf;
        }

        public int Position { get { return _pos; } }


        public string ReadString(int len)
        {
            string s = Encoding.UTF8.GetString(_buf, _pos, len);
            _pos += len;
            return s;
        }

        public byte ReadByte()
        {
            byte val = _buf[_pos];
            _pos += 1;
            return val;
        }

        public int  ReadInt()
        {
            int val = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(_buf, _pos));
            _pos += 4;
            return val;
        }

        public long ReadLong()
        {
            long val = IPAddress.NetworkToHostOrder(BitConverter.ToInt64(_buf, _pos));
            _pos += 8;
            return val;
        }

        public byte[] ReadBytes(int len)
        {
            byte[] val = new byte[len];
            Buffer.BlockCopy(_buf, _pos, val, 0, len);
            _pos += len;
            return val;
        }
    }
}
