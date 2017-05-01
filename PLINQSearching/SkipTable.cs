using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RapidSearching
{
    internal class SkipTable
    {
        
        private byte _patternLength;
        private byte[] _default;
        private byte[][] _skipTable;
        private const int BlockSize = 0x100;


        public SkipTable(int patternLength)
        {
            _patternLength = (byte)patternLength;
            _default = new byte[BlockSize];
            InitializeBlock(_default);

            _skipTable = new byte[BlockSize][];
            for (var i = 0; i < BlockSize; i++)
            {
                _skipTable[i] = _default;
            }
        }


        public byte this[int index]
        {
            get
            {
                return _skipTable[index / BlockSize][index % BlockSize];
            }
            set
            {
             
                int i = (index / BlockSize);
             
                if (_skipTable[i] == _default)
                {
                    // Yes, value goes in a new table
                    _skipTable[i] = new byte[BlockSize];
                    InitializeBlock(_skipTable[i]);
                }
                // Set value
                _skipTable[i][index % BlockSize] = value;
            }
        }

        /// <summary>
        /// Initializes a block to hold the current "nomatch" value.
        /// </summary>
        /// <param name="block">Block to be initialized</param>
        private void InitializeBlock(byte[] block)
        {
            for (int i = 0; i < BlockSize; i++)
                block[i] = _patternLength;
        }
    }
}
