using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RapidSearching
{
    internal class SkipTable
    {
        private readonly byte[] _standardByteArray;
        private readonly byte[][] _alteredSkipArray;
        private const int BlockSize = 0x100;

        public byte StringToSearchLength { get; set; }

        public SkipTable(int patternLength)
        {
            StringToSearchLength = (byte)patternLength;
            _standardByteArray = new byte[BlockSize];
            InitializeBlock(_standardByteArray);

            _alteredSkipArray = new byte[BlockSize][];
            for (var i = 0; i < BlockSize; i++)
            {
                _alteredSkipArray[i] = _standardByteArray;
            }
        }


        public byte this[int index]
        {
            get
            {
                return _alteredSkipArray[index / BlockSize][index % BlockSize];
            }
            set
            {
             
                var i = (index / BlockSize);
             
                if (_alteredSkipArray[i] == _standardByteArray)
                {
                   
                    _alteredSkipArray[i] = new byte[BlockSize];
                    InitializeBlock(_alteredSkipArray[i]);
                }
              
                _alteredSkipArray[i][index % BlockSize] = value;
            }
        }


        private void InitializeBlock(IList<byte> block)
        {
            for (var i = 0; i < BlockSize; i++)
            {
                block[i] = StringToSearchLength;
            }
        }
    }
}
