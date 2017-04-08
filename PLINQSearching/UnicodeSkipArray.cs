using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLINQSearching
{
    /// <summary>
    /// Implements a multi-stage byte array. Uses less memory than a byte
    /// array large enough to hold an offset for each Unicode character.
    /// </summary>
    class UnicodeSkipArray
    {
        // Pattern length used for default byte value
        private byte _patternLength;
        // Default byte array (filled with default value)
        private byte[] _default;
        // Array to hold byte arrays
        private byte[][] _skipTable;
        // Size of each block
        private const int BlockSize = 0x100;

        /// <summary>
        /// Initializes this UnicodeSkipTable instance
        /// </summary>
        /// <param name="patternLength">Length of BM pattern</param>
        public UnicodeSkipArray(int patternLength)
        {
            // Default value (length of pattern being searched)
            _patternLength = (byte)patternLength;
            // Default table (filled with default value)
            _default = new byte[BlockSize];
            InitializeBlock(_default);
            // Master table (array of arrays)
            _skipTable = new byte[BlockSize][];
            for (int i = 0; i < BlockSize; i++)
                _skipTable[i] = _default;
        }

        /// <summary>
        /// Sets/gets a value in the multi-stage tables.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public byte this[int index]
        {
            get
            {
                // Return value
                return _skipTable[index / BlockSize][index % BlockSize];
            }
            set
            {
                // Get array that contains value to set
                int i = (index / BlockSize);
                // Does it reference the default table?
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
