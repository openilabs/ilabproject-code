/*
 * Copyright (c) 2005 The Massachusetts Institute of Technology.  All
 * rights reserved.  Please see license.txt in top level directory for
 * full license.
 */

package weblab.toolkit.util;

/**
 * Decodes from Base64 Content-Transfer-Encoding
 *
 * See section 6.8 of RFC 2045 (http://www.ietf.org/rfc/rfc2045.txt)
 * for the specification of Base64.
 */
public class Base64
{
  /**
   * Decodes data from Base64 notation.
   *
   * @param base64string the string to decode
   * @return the decoded data
   */
  public static byte[] decode(String base64string)
  {
    // general idea: work in groups of four characters ("quads").
    // Each character gets decoded to a 6-bit number.  A quad of four
    // 6-bit numbers contains 24 bits of relevant data, which get
    // rearranged into three 8-bit numbers and output as three bytes.

    // the decoding table
    byte[] table = createDecodingTable();

    // holds the output as we go (note: might be longer than we need)
    byte[] output = new byte[3*((base64string.length()+3)/4)];

    // holds the current group of four 6-bit values (each decoded from
    // a character of the string).
    byte[] quad = new byte[4];

    // counters to keep track of where we are in the output and in the
    // current quad
    int output_ctr = 0, quad_ctr = 0;

    for(int i = 0; i < base64string.length(); i++)
    {
      int c = (int) base64string.charAt(i);

      // if we have a valid base64 encoding char (or '=' which is also >0)
      if (c >= 0 && c<128 && table[c] >= 0)
      {
	// decode it to a 6-bit number, and append to the current quad
        quad[quad_ctr++] = table[c];

	// when we have a full quad of four 6-bit numbers, rearrange
	// the relevant 24 bits into three 8-bit numbers, append these
	// to the output as bytes, and reset for a new quad
        if (quad_ctr == 4)
	{
          output[output_ctr++] = (byte)((quad[0]<<2) + (quad[1]>>4));
	  output[output_ctr++] = (byte)((quad[1]<<4) + (quad[2]>>2));
	  output[output_ctr++] = (byte)((quad[2]<<6) + quad[3]);
          quad_ctr = 0;
        }
      }
    }
    // check our last quad for padding characters ('='); we may have
    // written one or two too many bytes to the decoded output.  If
    // we have, decrement output_ctr accordingly.
    if (quad[3] == table[(int)'='])
    {
      output_ctr--;
      if(quad[2]==table[(int)'='])
        output_ctr--;
    }

    // copy the output to a fresh array that's exactly the right length
    byte[] result = new byte[output_ctr];
    for(int i = 0; i < output_ctr; i++)
      result[i] = output[i];
    return result;
  }



  /**
   * Creates the Base64 decoding table, which maps the integer value
   * of an encoding character (between 0 and 127) to a byte containing
   * either 6 bits of useful data (represented as a number between 0
   * and 63) or a number outside this range indicating some other meaning.
   *
   *   A-Z decodes to 0-25
   *   a-z decodes to 26-51
   *   0-9 decodes to 52-61
   *   + decodes to 62
   *   / decodes to 63
   *   = is used for padding (represented by 64)
   *   all other characters are -1 and should be ignored by decoding software
   */
  private static byte[] createDecodingTable()
  {
    byte[] table = new byte[128];

    // initialize to -1 (invalid characters)
    for(int i = 0; i < 128; i++)
      table[i] = -1;

    byte value = 0;

    for(int c = (int)'A'; c <= (int)'Z'; c++)
      table[c] = value++;

    for(int c = (int)'a'; c <= (int)'z'; c++)
      table[c] = value++;

    for(int c = (int)'0'; c <= (int)'9'; c++)
      table[c] = value++;

    table[(int)'+'] = 62;
    table[(int)'/'] = 63;
    table[(int)'='] = 64;

    return table;
  }
    
} // end class Base64
