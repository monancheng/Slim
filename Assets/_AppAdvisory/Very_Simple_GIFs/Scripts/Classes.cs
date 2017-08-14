/*
 * Copyright (c) 2015 Thomas Hourdel
 *
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 *
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 *    1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 
 *    2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 
 *    3. This notice may not be removed or altered from any source
 *    distribution.
 */




using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using ThreadPriority = System.Threading.ThreadPriority;
using System.Linq.Expressions;
using System.Reflection;

namespace AppAdvisory.VSGIF
{
	public class GifFrame
	{
		public int Width;
		public int Height;
		public Color32[] Data; 
	}




	public class GifEncoder
	{
		protected int m_Width;
		protected int m_Height;
		protected int m_Repeat = -1;                  // -1: no repeat, 0: infinite, >0: repeat count
		protected int m_FrameDelay = 0;               // Frame delay (milliseconds)
		protected bool m_HasStarted = false;          // Ready to output frames
		protected FileStream m_FileStream;

		protected GifFrame m_CurrentFrame;
		protected byte[] m_Pixels;                    // BGR byte array from frame
		protected byte[] m_IndexedPixels;             // Converted frame indexed to palette
		protected int m_ColorDepth;                   // Number of bit planes
		protected byte[] m_ColorTab;                  // RGB palette
		protected bool[] m_UsedEntry = new bool[256]; // Active palette entries
		protected int m_PaletteSize = 7;              // Color table size (bits-1)
		protected int m_DisposalCode = -1;            // Disposal code (-1 = use default)
		protected bool m_ShouldCloseStream = false;   // Close stream when finished
		protected bool m_IsFirstFrame = true;
		protected bool m_IsSizeSet = false;           // If false, get size from first frame
		protected int m_SampleInterval = 10;          // Default sample interval for quantizer


		private static float[] m_TransparencyMap;

		public void InitTransparencyMap() 
		{
			m_TransparencyMap = new float[256];
			for (var i = 255 ; i >= 0; --i) 
			{
				m_TransparencyMap[i] = (float)i/255;
			}
		}

		/// <summary>
		/// Default constructor. Repeat will be set to -1 and Quality to 10.
		/// </summary>
		public GifEncoder() : this(-1, 10)
		{
		}

		/// <summary>
		/// Constructor with the number of times the set of GIF frames should be played.
		/// </summary>
		/// <param name="repeat">Default is -1 (no repeat); 0 means play indefinitely</param>
		/// <param name="quality">Sets quality of color quantization (conversion of images to
		/// the maximum 256 colors allowed by the GIF specification). Lower values (minimum = 1)
		/// produce better colors, but slow processing significantly. Higher values will speed
		/// up the quantization pass at the cost of lower image quality (maximum = 100).</param>
		public GifEncoder(int repeat, int quality)
		{
			if (repeat >= 0)
				m_Repeat = repeat;

			m_SampleInterval = (int)Mathf.Clamp(quality, 1, 100);

		}

		/// <summary>
		/// Sets the delay time between each frame, or changes it for subsequent frames (applies
		/// to last frame added).
		/// </summary>
		/// <param name="ms">Delay time in milliseconds</param>
		public void SetDelay(int ms)
		{
			m_FrameDelay = Mathf.RoundToInt(ms / 10f);
		}

		protected int m_WatermarkWidth;
		protected int m_WatermarkHeight;
		protected Color32[] m_WatermarkPixels;

		public void SetWaterMark(Texture2D watermark) {
			if (watermark == null)
				return;

			m_WatermarkHeight = watermark.height;
			m_WatermarkWidth = watermark.width;
			m_WatermarkPixels = watermark.GetPixels32 ();

			InitTransparencyMap ();
		}
		
		/// <summary>
		/// Sets frame rate in frames per second. Equivalent to <code>SetDelay(1000/fps)</code>.
		/// </summary>
		/// <param name="fps">Frame rate</param>
		public void SetFrameRate(float fps)
		{
			if (fps > 0f)
				m_FrameDelay = Mathf.RoundToInt(100f / fps);
		}

		/// <summary>
		/// Adds next GIF frame. The frame is not written immediately, but is actually deferred
		/// until the next frame is received so that timing data can be inserted. Invoking
		/// <code>Finish()</code> flushes all frames.
		/// </summary>
		/// <param name="frame">GifFrame containing frame to write.</param>
		public void AddFrame(GifFrame frame)
		{
			if ((frame == null))
				throw new ArgumentNullException("Can't add a null frame to the gif.");

			if (!m_HasStarted)
				throw new InvalidOperationException("Call Start() before adding frames to the gif.");

			// Use first frame's size
			if (!m_IsSizeSet)
				SetSize(frame.Width, frame.Height);

			m_CurrentFrame = frame;
			GetImagePixels();
			AnalyzePixels();

			if (m_IsFirstFrame)
			{
				WriteLSD();
				WritePalette();

				if (m_Repeat >= 0)
					WriteNetscapeExt();
			}

			WriteGraphicCtrlExt();
			WriteImageDesc();

			if (!m_IsFirstFrame)
				WritePalette();

			WritePixels();
			m_IsFirstFrame = false;
		}

//		public FileStream GetFileStream()
//		{
//			return m_FileStream;
//		}

		/// Initiates GIF file creation on the given stream. The stream is not closed automatically.
		/// </summary>
		/// <param name="os">OutputStream on which GIF images are written</param>
		public void Start(FileStream os)
		{
			if (os == null)
				throw new ArgumentNullException("Stream is null.");

			m_ShouldCloseStream = false;
			m_FileStream = os;

			try
			{
				WriteString("GIF89a"); // header
			}
			catch (IOException e)
			{
				throw e;
			}

			m_HasStarted = true;
		}

		/// <summary>
		/// Initiates writing of a GIF file with the specified name. The stream will be handled for you.
		/// </summary>
		/// <param name="file">String containing output file name</param>
		public void Start(String file)
		{
			try
			{
				

				m_FileStream = new FileStream(file, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
				Start(m_FileStream);
				m_ShouldCloseStream = true;
			}
			catch (IOException e)
			{
				throw e;
			}
		}

		/// <summary>
		/// Flushes any pending data and closes output file.
		/// If writing to an OutputStream, the stream is not closed.
		/// </summary>
		public void Finish()
		{
			if (!m_HasStarted)
				throw new InvalidOperationException("Can't finish a non-started gif.");

			m_HasStarted = false;

			try
			{
				m_FileStream.WriteByte(0x3b); // Gif trailer
				m_FileStream.Flush();

				if (m_ShouldCloseStream)
					m_FileStream.Close();
			}
			catch (IOException e)
			{
				throw e;
			}

			// Reset for subsequent use
			m_FileStream = null;
			m_CurrentFrame = null;
			m_Pixels = null;
			m_IndexedPixels = null;
			m_ColorTab = null;
			m_ShouldCloseStream = false;
			m_IsFirstFrame = true;
		}

		// Sets the GIF frame size.
		protected void SetSize(int w, int h)
		{
			m_Width = w;
			m_Height = h;
			m_IsSizeSet = true;
		}

		// Extracts image pixels into byte array "pixels".
		protected void GetImagePixels()
		{
			m_Pixels = new Byte[3 * m_CurrentFrame.Width * m_CurrentFrame.Height];
			Color32[] p = m_CurrentFrame.Data;
			int count = 0;

			int startX = m_CurrentFrame.Width - m_WatermarkWidth;
			int startY = m_CurrentFrame.Height - m_WatermarkHeight;

			// Texture data is layered down-top, so flip it
			for (int th = m_CurrentFrame.Height - 1; th >= 0; th--)
			{
				for (int tw = 0; tw < m_CurrentFrame.Width; tw++)
				{
					Color32 color = p[th * m_CurrentFrame.Width + tw];

					if (tw >= startX && th < m_CurrentFrame.Height - startY) 
					{
						Color32 originalPix = color;
						Color32 watermarkPix = m_WatermarkPixels [th * m_WatermarkWidth + tw - startX];
						float a = m_TransparencyMap [watermarkPix.a];
						color = Color32.Lerp (originalPix, watermarkPix, a);
					}
				
					m_Pixels[count] = color.r; count++;
					m_Pixels[count] = color.g; count++;
					m_Pixels[count] = color.b; count++;
				}
			}
		}
		
		// Analyzes image colors and creates color map.
		protected void AnalyzePixels()
		{
			int len = m_Pixels.Length;
			int nPix = len / 3;
			m_IndexedPixels = new byte[nPix];
			NeuQuant nq = new NeuQuant(m_Pixels, len, (int)m_SampleInterval);
			m_ColorTab = nq.Process(); // Create reduced palette

			// Map image pixels to new palette
			int k = 0;
			for (int i = 0; i < nPix; i++)
			{
				int index = nq.Map(m_Pixels[k++] & 0xff, m_Pixels[k++] & 0xff, m_Pixels[k++] & 0xff);
				m_UsedEntry[index] = true;
				m_IndexedPixels[i] = (byte)index;
			}

			m_Pixels = null;
			m_ColorDepth = 8;
			m_PaletteSize = 7;
		}

		// Writes Graphic Control Extension.
		protected void WriteGraphicCtrlExt()
		{
			m_FileStream.WriteByte(0x21); // Extension introducer
			m_FileStream.WriteByte(0xf9); // GCE label
			m_FileStream.WriteByte(4);    // Data block size

			// Packed fields
			m_FileStream.WriteByte(Convert.ToByte(0 |     // 1:3 reserved
												  0 |     // 4:6 disposal
												  0 |     // 7   user input - 0 = none
												  0));    // 8   transparency flag

			WriteShort(m_FrameDelay); // Delay x 1/100 sec
			m_FileStream.WriteByte(Convert.ToByte(0)); // Transparent color index
			m_FileStream.WriteByte(0); // Block terminator
		}

		// Writes Image Descriptor.
		protected void WriteImageDesc()
		{
			m_FileStream.WriteByte(0x2c); // Image separator
			WriteShort(0);                // Image position x,y = 0,0
			WriteShort(0);
			WriteShort(m_Width);          // image size
			WriteShort(m_Height);

			// Packed fields
			if (m_IsFirstFrame)
			{
				m_FileStream.WriteByte(0); // No LCT  - GCT is used for first (or only) frame
			}
			else
			{
				// Specify normal LCT
				m_FileStream.WriteByte(Convert.ToByte(0x80 |           // 1 local color table  1=yes
													  0 |              // 2 interlace - 0=no
													  0 |              // 3 sorted - 0=no
													  0 |              // 4-5 reserved
													  m_PaletteSize)); // 6-8 size of color table
			}
		}

		// Writes Logical Screen Descriptor.
		protected void WriteLSD()
		{
			// Logical screen size
			WriteShort(m_Width);
			WriteShort(m_Height);

			// Packed fields
			m_FileStream.WriteByte(Convert.ToByte(0x80 |           // 1   : global color table flag = 1 (gct used)
												  0x70 |           // 2-4 : color resolution = 7
												  0x00 |           // 5   : gct sort flag = 0
												  m_PaletteSize)); // 6-8 : gct size

			m_FileStream.WriteByte(0); // Background color index
			m_FileStream.WriteByte(0); // Pixel aspect ratio - assume 1:1
		}

		// Writes Netscape application extension to define repeat count.
		protected void WriteNetscapeExt()
		{
			m_FileStream.WriteByte(0x21);    // Extension introducer
			m_FileStream.WriteByte(0xff);    // App extension label
			m_FileStream.WriteByte(11);      // Block size
			WriteString("NETSCAPE" + "2.0"); // App id + auth code
			m_FileStream.WriteByte(3);       // Sub-block size
			m_FileStream.WriteByte(1);       // Loop sub-block id
			WriteShort(m_Repeat);            // Loop count (extra iterations, 0=repeat forever)
			m_FileStream.WriteByte(0);       // Block terminator
		}

		// Write color table.
		protected void WritePalette()
		{
			m_FileStream.Write(m_ColorTab, 0, m_ColorTab.Length);
			int n = (3 * 256) - m_ColorTab.Length;

			for (int i = 0; i < n; i++)
				m_FileStream.WriteByte(0);
		}

		// Encodes and writes pixel data.
		protected void WritePixels()
		{
			LzwEncoder encoder = new LzwEncoder(m_Width, m_Height, m_IndexedPixels, m_ColorDepth);
			encoder.Encode(m_FileStream);
		}

		// Write 16-bit value to output stream, LSB first.
		protected void WriteShort(int value)
		{
			m_FileStream.WriteByte(Convert.ToByte(value & 0xff));
			m_FileStream.WriteByte(Convert.ToByte((value >> 8) & 0xff));
		}

		// Writes string to output stream.
		protected void WriteString(String s)
		{
			char[] chars = s.ToCharArray();

			for (int i = 0; i < chars.Length; i++)
				m_FileStream.WriteByte((byte)chars[i]);
		}
	}

	public class LzwEncoder
	{
		private static readonly int EOF = -1;

		private byte[] pixAry;
		private int initCodeSize;
		private int curPixel;

		// GIFCOMPR.C       - GIF Image compression routines
		//
		// Lempel-Ziv compression based on 'compress'.  GIF modifications by
		// David Rowley (mgardi@watdcsu.waterloo.edu)

		// General DEFINEs

		static readonly int BITS = 12;

		static readonly int HSIZE = 5003; // 80% occupancy

		// GIF Image compression - modified 'compress'
		//
		// Based on: compress.c - File compression ala IEEE Computer, June 1984.
		//
		// By Authors:  Spencer W. Thomas      (decvax!harpo!utah-cs!utah-gr!thomas)
		//              Jim McKie              (decvax!mcvax!jim)
		//              Steve Davies           (decvax!vax135!petsd!peora!srd)
		//              Ken Turkowski          (decvax!decwrl!turtlevax!ken)
		//              James A. Woods         (decvax!ihnp4!ames!jaw)
		//              Joe Orost              (decvax!vax135!petsd!joe)

		int n_bits; // number of bits/code
		int maxbits = BITS; // user settable max # bits/code
		int maxcode; // maximum code, given n_bits
		int maxmaxcode = 1 << BITS; // should NEVER generate this code

		int[] htab = new int[HSIZE];
		int[] codetab = new int[HSIZE];

		int hsize = HSIZE; // for dynamic table sizing

		int free_ent = 0; // first unused entry

		// block compression parameters -- after all codes are used up,
		// and compression rate changes, start over.
		bool clear_flg = false;

		// Algorithm:  use open addressing double hashing (no chaining) on the
		// prefix code / next character combination.  We do a variant of Knuth's
		// algorithm D (vol. 3, sec. 6.4) along with G. Knott's relatively-prime
		// secondary probe.  Here, the modular division first probe is gives way
		// to a faster exclusive-or manipulation.  Also do block compression with
		// an adaptive reset, whereby the code table is cleared when the compression
		// ratio decreases, but after the table fills.  The variable-length output
		// codes are re-sized at this point, and a special CLEAR code is generated
		// for the decompressor.  Late addition:  construct the table according to
		// file size for noticeable speed improvement on small files.  Please direct
		// questions about this implementation to ames!jaw.

		int g_init_bits;

		int ClearCode;
		int EOFCode;

		// output
		//
		// Output the given code.
		// Inputs:
		//      code:   A n_bits-bit integer.  If == -1, then EOF.  This assumes
		//              that n_bits =< wordsize - 1.
		// Outputs:
		//      Outputs code to the file.
		// Assumptions:
		//      Chars are 8 bits long.
		// Algorithm:
		//      Maintain a BITS character long buffer (so that 8 codes will
		// fit in it exactly).  Use the VAX insv instruction to insert each
		// code in turn.  When the buffer fills up empty it and start over.

		int cur_accum = 0;
		int cur_bits = 0;

		int [] masks =
		{
			0x0000,
			0x0001,
			0x0003,
			0x0007,
			0x000F,
			0x001F,
			0x003F,
			0x007F,
			0x00FF,
			0x01FF,
			0x03FF,
			0x07FF,
			0x0FFF,
			0x1FFF,
			0x3FFF,
			0x7FFF,
			0xFFFF };

		// Number of characters so far in this 'packet'
		int a_count;

		// Define the storage for the packet accumulator
		byte[] accum = new byte[256];

		//----------------------------------------------------------------------------
		public LzwEncoder(int width, int height, byte[] pixels, int color_depth) 
		{
			pixAry = pixels;
			initCodeSize = Math.Max(2, color_depth);
		}

		// Add a character to the end of the current packet, and if it is 254
		// characters, flush the packet to disk.
		void Add(byte c, Stream outs)
		{
			accum[a_count++] = c;
			if (a_count >= 254)
				Flush(outs);
		}

		// Clear out the hash table

		// table clear for block compress
		void ClearTable(Stream outs)
		{
			ResetCodeTable(hsize);
			free_ent = ClearCode + 2;
			clear_flg = true;

			Output(ClearCode, outs);
		}

		// reset code table
		void ResetCodeTable(int hsize) 
		{
			for (int i = 0; i < hsize; ++i)
				htab[i] = -1;
		}

		void Compress(int init_bits, Stream outs)
		{
			int fcode;
			int i /* = 0 */;
			int c;
			int ent;
			int disp;
			int hsize_reg;
			int hshift;

			// Set up the globals:  g_init_bits - initial number of bits
			g_init_bits = init_bits;

			// Set up the necessary values
			clear_flg = false;
			n_bits = g_init_bits;
			maxcode = MaxCode(n_bits);

			ClearCode = 1 << (init_bits - 1);
			EOFCode = ClearCode + 1;
			free_ent = ClearCode + 2;

			a_count = 0; // clear packet

			ent = NextPixel();

			hshift = 0;
			for (fcode = hsize; fcode < 65536; fcode *= 2)
				++hshift;
			hshift = 8 - hshift; // set hash code range bound

			hsize_reg = hsize;
			ResetCodeTable(hsize_reg); // clear hash table

			Output(ClearCode, outs);

			outer_loop : while ((c = NextPixel()) != EOF) 
			{
				fcode = (c << maxbits) + ent;
				i = (c << hshift) ^ ent; // xor hashing

				if (htab[i] == fcode) 
				{
					ent = codetab[i];
					continue;
				} 
				else if (htab[i] >= 0) // non-empty slot
				{
					disp = hsize_reg - i; // secondary hash (after G. Knott)
					if (i == 0)
						disp = 1;
					do 
					{
						if ((i -= disp) < 0)
							i += hsize_reg;

						if (htab[i] == fcode) 
						{
							ent = codetab[i];
							goto outer_loop;
						}
					} while (htab[i] >= 0);
				}
				Output(ent, outs);
				ent = c;
				if (free_ent < maxmaxcode) 
				{
					codetab[i] = free_ent++; // code -> hashtable
					htab[i] = fcode;
				} 
				else
					ClearTable(outs);
			}
			// Put out the final code.
			Output(ent, outs);
			Output(EOFCode, outs);
		}

		//----------------------------------------------------------------------------
		public void Encode( Stream os)
		{
			os.WriteByte( Convert.ToByte( initCodeSize) ); // write "initial code size" byte
			curPixel = 0;
			Compress(initCodeSize + 1, os); // compress and write the pixel data
			os.WriteByte(0); // write block terminator
		}

		// Flush the packet to disk, and reset the accumulator
		void Flush(Stream outs)
		{
			if (a_count > 0) 
			{
				outs.WriteByte( Convert.ToByte( a_count ));
				outs.Write(accum, 0, a_count);
				a_count = 0;
			}
		}

		int MaxCode(int n_bits) 
		{
			return (1 << n_bits) - 1;
		}

		//----------------------------------------------------------------------------
		// Return the next pixel from the image
		//----------------------------------------------------------------------------
		private int NextPixel() 
		{
			if (curPixel == pixAry.Length)
				return EOF;

			curPixel++;
			return pixAry[curPixel - 1] & 0xff;
		}

		void Output(int code, Stream outs)
		{
			cur_accum &= masks[cur_bits];

			if (cur_bits > 0)
				cur_accum |= (code << cur_bits);
			else
				cur_accum = code;

			cur_bits += n_bits;

			while (cur_bits >= 8) 
			{
				Add((byte) (cur_accum & 0xff), outs);
				cur_accum >>= 8;
				cur_bits -= 8;
			}

			// If the next entry is going to be too big for the code size,
			// then increase it, if possible.
			if (free_ent > maxcode || clear_flg) 
			{
				if (clear_flg) 
				{
					maxcode = MaxCode(n_bits = g_init_bits);
					clear_flg = false;
				} 
				else 
				{
					++n_bits;
					if (n_bits == maxbits)
						maxcode = maxmaxcode;
					else
						maxcode = MaxCode(n_bits);
				}
			}

			if (code == EOFCode) 
			{
				// At EOF, write the rest of the buffer.
				while (cur_bits > 0) 
				{
					Add((byte) (cur_accum & 0xff), outs);
					cur_accum >>= 8;
					cur_bits -= 8;
				}

				Flush(outs);
			}
		}
	}

	public class NeuQuant
	{
		protected static readonly int netsize = 256; // Number of colours used

		// Four primes near 500 - assume no image has a length so large that it is divisible by all four primes
		protected static readonly int prime1 = 499;
		protected static readonly int prime2 = 491;
		protected static readonly int prime3 = 487;
		protected static readonly int prime4 = 503;

		protected static readonly int minpicturebytes = (3 * prime4); // Minimum size for input image

		// Network Definitions
		protected static readonly int maxnetpos = (netsize - 1);
		protected static readonly int netbiasshift = 4; // Bias for colour values
		protected static readonly int ncycles = 100; // No. of learning cycles

		// Defs for freq and bias
		protected static readonly int intbiasshift = 16; // Bias for fractions
		protected static readonly int intbias = (((int)1) << intbiasshift);
		protected static readonly int gammashift = 10; // Gamma = 1024
		protected static readonly int gamma = (((int)1) << gammashift);
		protected static readonly int betashift = 10;
		protected static readonly int beta = (intbias >> betashift); // Beta = 1/1024
		protected static readonly int betagamma = (intbias << (gammashift - betashift));

		// Defs for decreasing radius factor
		protected static readonly int initrad = (netsize >> 3); // For 256 cols, radius starts
		protected static readonly int radiusbiasshift = 6; // At 32.0 biased by 6 bits
		protected static readonly int radiusbias = (((int)1) << radiusbiasshift);
		protected static readonly int initradius = (initrad * radiusbias); // And decreases by a
		protected static readonly int radiusdec = 30; // Factor of 1/30 each cycle

		// Defs for decreasing alpha factor
		protected static readonly int alphabiasshift = 10; /* alpha starts at 1.0 */
		protected static readonly int initalpha = (((int)1) << alphabiasshift);

		protected int alphadec; // Biased by 10 bits

		// Radbias and alpharadbias used for radpower calculation
		protected static readonly int radbiasshift = 8;
		protected static readonly int radbias = (((int)1) << radbiasshift);
		protected static readonly int alpharadbshift = (alphabiasshift + radbiasshift);
		protected static readonly int alpharadbias = (((int)1) << alpharadbshift);

		// Types and Global Variables
		protected byte[] thepicture; // The input image itself
		protected int lengthcount; // Lengthcount = H*W*3
		protected int samplefac; // Sampling factor 1..30
		protected int[][] network; // The network itself - [netsize][4]
		protected int[] netindex = new int[256]; // For network lookup - really 256
		protected int[] bias = new int[netsize]; // Bias and freq arrays for learning
		protected int[] freq = new int[netsize];
		protected int[] radpower = new int[initrad]; // Radpower for precomputation

		// Initialize network in range (0,0,0) to (255,255,255) and set parameters
		public NeuQuant(byte[] thepic, int len, int sample)
		{
			int i;
			int[] p;

			thepicture = thepic;
			lengthcount = len;
			samplefac = sample;

			network = new int[netsize][];
			for (i = 0; i < netsize; i++)
			{
				network[i] = new int[4];
				p = network[i];
				p[0] = p[1] = p[2] = (i << (netbiasshift + 8)) / netsize;
				freq[i] = intbias / netsize; // 1 / netsize
				bias[i] = 0;
			}
		}

		public byte[] ColorMap()
		{
			byte[] map = new byte[3 * netsize];
			int[] index = new int[netsize];

			for (int i = 0; i < netsize; i++)
				index[network[i][3]] = i;

			int k = 0;
			for (int i = 0; i < netsize; i++)
			{
				int j = index[i];
				map[k++] = (byte)(network[j][0]);
				map[k++] = (byte)(network[j][1]);
				map[k++] = (byte)(network[j][2]);
			}

			return map;
		}

		// Insertion sort of network and building of netindex[0..255] (to do after unbias)
		public void Inxbuild()
		{
			int i, j, smallpos, smallval;
			int[] p;
			int[] q;
			int previouscol, startpos;

			previouscol = 0;
			startpos = 0;

			for (i = 0; i < netsize; i++)
			{
				p = network[i];
				smallpos = i;
				smallval = p[1]; // Index on g

				// Find smallest in i..netsize-1
				for (j = i + 1; j < netsize; j++)
				{
					q = network[j];
					if (q[1] < smallval)
					{
						smallpos = j;
						smallval = q[1]; // Index on g
					}
				}

				q = network[smallpos];

				// Swap p (i) and q (smallpos) entries
				if (i != smallpos)
				{
					j = q[0];
					q[0] = p[0];
					p[0] = j;
					j = q[1];
					q[1] = p[1];
					p[1] = j;
					j = q[2];
					q[2] = p[2];
					p[2] = j;
					j = q[3];
					q[3] = p[3];
					p[3] = j;
				}

				// Smallval entry is now in position i
				if (smallval != previouscol)
				{
					netindex[previouscol] = (startpos + i) >> 1;

					for (j = previouscol + 1; j < smallval; j++)
						netindex[j] = i;

					previouscol = smallval;
					startpos = i;
				}
			}

			netindex[previouscol] = (startpos + maxnetpos) >> 1;

			for (j = previouscol + 1; j < 256; j++)
				netindex[j] = maxnetpos;
		}

		// Main Learning Loop
		public void Learn()
		{
			int i, j, b, g, r;
			int radius, rad, alpha, step, delta, samplepixels;
			byte[] p;
			int pix, lim;

			if (lengthcount < minpicturebytes)
				samplefac = 1;

			alphadec = 30 + ((samplefac - 1) / 3);
			p = thepicture;
			pix = 0;
			lim = lengthcount;
			samplepixels = lengthcount / (3 * samplefac);
			delta = samplepixels / ncycles;
			alpha = initalpha;
			radius = initradius;

			rad = radius >> radiusbiasshift;

			if (rad <= 1)
				rad = 0;

			for (i = 0; i < rad; i++)
				radpower[i] = alpha * (((rad * rad - i * i) * radbias) / (rad * rad));

			if (lengthcount < minpicturebytes)
			{
				step = 3;
			}
			else if ((lengthcount % prime1) != 0)
			{
				step = 3 * prime1;
			}
			else
			{
				if ((lengthcount % prime2) != 0)
				{
					step = 3 * prime2;
				}
				else
				{
					if ((lengthcount % prime3) != 0)
						step = 3 * prime3;
					else
						step = 3 * prime4;
				}
			}

			i = 0;
			while (i < samplepixels)
			{
				b = (p[pix + 0] & 0xff) << netbiasshift;
				g = (p[pix + 1] & 0xff) << netbiasshift;
				r = (p[pix + 2] & 0xff) << netbiasshift;
				j = Contest(b, g, r);

				Altersingle(alpha, j, b, g, r);

				if (rad != 0)
					Alterneigh(rad, j, b, g, r); // Alter neighbours

				pix += step;

				if (pix >= lim)
					pix -= lengthcount;

				i++;

				if (delta == 0)
					delta = 1;

				if (i % delta == 0)
				{
					alpha -= alpha / alphadec;
					radius -= radius / radiusdec;
					rad = radius >> radiusbiasshift;

					if (rad <= 1)
						rad = 0;

					for (j = 0; j < rad; j++)
						radpower[j] = alpha * (((rad * rad - j * j) * radbias) / (rad * rad));
				}
			}
		}

		// Search for BGR values 0..255 (after net is unbiased) and return colour index
		public int Map(int b, int g, int r)
		{
			int i, j, dist, a, bestd;
			int[] p;
			int best;

			bestd = 1000; // Biggest possible dist is 256*3
			best = -1;
			i = netindex[g]; // Index on g
			j = i - 1; // Start at netindex[g] and work outwards

			while ((i < netsize) || (j >= 0))
			{
				if (i < netsize)
				{
					p = network[i];
					dist = p[1] - g; // Inx key

					if (dist >= bestd)
					{
						i = netsize; // Stop iter
					}
					else
					{
						i++;

						if (dist < 0)
							dist = -dist;

						a = p[0] - b;

						if (a < 0)
							a = -a;

						dist += a;

						if (dist < bestd)
						{
							a = p[2] - r;

							if (a < 0)
								a = -a;

							dist += a;

							if (dist < bestd)
							{
								bestd = dist;
								best = p[3];
							}
						}
					}
				}

				if (j >= 0)
				{
					p = network[j];
					dist = g - p[1]; // Inx key - reverse dif

					if (dist >= bestd)
					{
						j = -1; // Stop iter
					}
					else
					{
						j--;

						if (dist < 0)
							dist = -dist;

						a = p[0] - b;

						if (a < 0)
							a = -a;

						dist += a;

						if (dist < bestd)
						{
							a = p[2] - r;

							if (a < 0)
								a = -a;

							dist += a;

							if (dist < bestd)
							{
								bestd = dist;
								best = p[3];
							}
						}
					}
				}
			}

			return best;
		}

		public byte[] Process()
		{
			Learn();
			Unbiasnet();
			Inxbuild();
			return ColorMap();
		}

		// Unbias network to give byte values 0..255 and record position i to prepare for sort
		public void Unbiasnet()
		{
			int i;

			for (i = 0; i < netsize; i++)
			{
				network[i][0] >>= netbiasshift;
				network[i][1] >>= netbiasshift;
				network[i][2] >>= netbiasshift;
				network[i][3] = i; // Record colour no
			}
		}

		// Move adjacent neurons by precomputed alpha*(1-((i-j)^2/[r]^2)) in radpower[|i-j|]
		protected void Alterneigh(int rad, int i, int b, int g, int r)
		{
			int j, k, lo, hi, a, m;
			int[] p;

			lo = i - rad;

			if (lo < -1)
				lo = -1;

			hi = i + rad;

			if (hi > netsize)
				hi = netsize;

			j = i + 1;
			k = i - 1;
			m = 1;

			while ((j < hi) || (k > lo))
			{
				a = radpower[m++];

				if (j < hi)
				{
					p = network[j++];
					p[0] -= (a * (p[0] - b)) / alpharadbias;
					p[1] -= (a * (p[1] - g)) / alpharadbias;
					p[2] -= (a * (p[2] - r)) / alpharadbias;
				}

				if (k > lo)
				{
					p = network[k--];
					p[0] -= (a * (p[0] - b)) / alpharadbias;
					p[1] -= (a * (p[1] - g)) / alpharadbias;
					p[2] -= (a * (p[2] - r)) / alpharadbias;
				}
			}
		}

		// Move neuron i towards biased (b,g,r) by factor alpha
		protected void Altersingle(int alpha, int i, int b, int g, int r)
		{
			/* Alter hit neuron */
			int[] n = network[i];
			n[0] -= (alpha * (n[0] - b)) / initalpha;
			n[1] -= (alpha * (n[1] - g)) / initalpha;
			n[2] -= (alpha * (n[2] - r)) / initalpha;
		}

		// Search for biased BGR values
		protected int Contest(int b, int g, int r)
		{
			// Finds closest neuron (min dist) and updates freq
			// Finds best neuron (min dist-bias) and returns position
			// For frequently chosen neurons, freq[i] is high and bias[i] is negative
			// bias[i] = gamma*((1/netsize)-freq[i])

			int i, dist, a, biasdist, betafreq;
			int bestpos, bestbiaspos, bestd, bestbiasd;
			int[] n;

			bestd = ~(((int)1) << 31);
			bestbiasd = bestd;
			bestpos = -1;
			bestbiaspos = bestpos;

			for (i = 0; i < netsize; i++)
			{
				n = network[i];
				dist = n[0] - b;

				if (dist < 0)
					dist = -dist;

				a = n[1] - g;

				if (a < 0)
					a = -a;

				dist += a;
				a = n[2] - r;

				if (a < 0)
					a = -a;

				dist += a;

				if (dist < bestd)
				{
					bestd = dist;
					bestpos = i;
				}

				biasdist = dist - ((bias[i]) >> (intbiasshift - netbiasshift));

				if (biasdist < bestbiasd)
				{
					bestbiasd = biasdist;
					bestbiaspos = i;
				}

				betafreq = (freq[i] >> betashift);
				freq[i] -= betafreq;
				bias[i] += (betafreq << gammashift);
			}

			freq[bestpos] += beta;
			bias[bestpos] -= betagamma;
			return bestbiaspos;
		}
	}

	public class ReflectionUtils<T> where T : class, new()
	{
		readonly T _Instance;

		public ReflectionUtils(T instance)
		{
			_Instance = instance;
		}

		public string GetFieldName<U>(Expression<Func<T, U>> fieldAccess)
		{
			MemberExpression memberExpression = fieldAccess.Body as MemberExpression;

			if (memberExpression != null)
				return memberExpression.Member.Name;

			throw new InvalidOperationException("Member expression expected");
		}

		public FieldInfo GetField(string fieldName)
		{
			return typeof(T).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
		}

		public A GetAttribute<A>(FieldInfo field) where A : Attribute
		{
			return (A)Attribute.GetCustomAttribute(field, typeof(A));
		}

		// MinAttribute
		public void ConstrainMin<U>(Expression<Func<T, U>> fieldAccess, float value)
		{
			FieldInfo fieldInfo = GetField(GetFieldName(fieldAccess));
			fieldInfo.SetValue(_Instance, Mathf.Max(value, GetAttribute<MinAttribute>(fieldInfo).min));
		}

		public void ConstrainMin<U>(Expression<Func<T, U>> fieldAccess, int value)
		{
			FieldInfo fieldInfo = GetField(GetFieldName(fieldAccess));
			fieldInfo.SetValue(_Instance, (int)Mathf.Max(value, GetAttribute<MinAttribute>(fieldInfo).min));
		}

		// RangeAttribute
		public void ConstrainRange<U>(Expression<Func<T, U>> fieldAccess, float value)
		{
			FieldInfo fieldInfo = GetField(GetFieldName(fieldAccess));
			RangeAttribute attr = GetAttribute<RangeAttribute>(fieldInfo);
			fieldInfo.SetValue(_Instance, Mathf.Clamp(value, attr.min, attr.max));
		}

		public void ConstrainRange<U>(Expression<Func<T, U>> fieldAccess, int value)
		{
			FieldInfo fieldInfo = GetField(GetFieldName(fieldAccess));
			RangeAttribute attr = GetAttribute<RangeAttribute>(fieldInfo);
			fieldInfo.SetValue(_Instance, (int)Mathf.Clamp(value, attr.min, attr.max));
		}
	}

	internal sealed class GIFMaker
	{
		static int workerId = 1;

		Thread m_Thread;
		int m_Id;

		internal List<GifFrame> m_Frames;
		internal GifEncoder m_Encoder;
		internal string m_FilePath;
		internal Action<int, string> m_OnFileSaved;
		internal Action<int, float> m_OnFileSaveProgress;

		internal GIFMaker(ThreadPriority priority)
		{
			m_Id = workerId++;
			m_Thread = new Thread(Run);
			m_Thread.Priority = priority;
		}

		internal void Start()
		{
			m_Thread.Start();
		}

		void Run()
		{


			m_Encoder.Start(m_FilePath);

			for (int i = 0; i < m_Frames.Count; i++)
			{
				GifFrame frame = m_Frames[i];
				m_Encoder.AddFrame(frame);

				if (m_OnFileSaveProgress != null)
				{
					float percent = (float)i / (float)m_Frames.Count;
					m_OnFileSaveProgress(m_Id, percent);
				}
			}

			m_Encoder.Finish();

			if (m_OnFileSaved != null)
				m_OnFileSaved(m_Id, m_FilePath);
		}
	}

	public sealed class MinAttribute : PropertyAttribute
	{
		public readonly float min;

		public MinAttribute(float min)
		{
			this.min = min;
		}
	}
}
