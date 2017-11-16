using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TelnetServer.Game {
	class FANSI {

		public static String[] UnicodeChars = new String[] { "", "☺", "", "♥", "♦", "♣", "♠", "", "", "", "", "", "", "", "♫", "☼", "►", 
            "◄", "↕", "‼", "¶", "§", "‗", "↨", "↑", "↓", "→", " ", "∟", "↔", "▲", "▼" ," ", "!","\"", "#","$","%","&","'","(",")","*",
            "+",",","-",".","/","0","1","2",
            "3","4","5","6","7","8","9",":",";","<","=",">","?","@","A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q",
            "R","S","T","U","V","W","X","Y","Z","[","\\",@"]","^","_","`","a","b","c","d","e","f","g","h","i","j","k","l","m","n","o","p",
            "q","r","s","t","u","v","w","x","y","z", "{", 
            "|", "}", "~", "⌂", "Ç", "ü", "é", "â", "ä", "à", "å", "ç", "ê", "ë", "è", "ï", "î", "ì", "Ä", "Å", "É", "æ", "Æ", "ô", "ö", 
            "ò", "û", "ù", "ÿ", "Ö", "Ü", "¢", "£", "¥", "₧", "ƒ", "á", "í", "ó", "ú", "ñ", "Ñ", "ª", "º", "¿", "⌐", "¬", "½", "¼", "¡", 
            "«", "»", "░", "▒", "▓", "│", "┤", "╡", "╢", "╖", "╕", "╣", "║", "╗", "╝", "╜", "╛", "┐", "└", "┴", "┬", "├", "─", "┼", "╞", 
            "╟", "╚", "╔", "╩", "╦", "╠", "═", "╬", "╧", "╨", "╤", "╥", "╙", "╘", "╒", "╓", "╫", "╪", "┘", "┌", "█", "▄", "▌", "▐", "▀", 
            "α", "β", "Γ", "π", "Σ", "δ", "μ", "τ", "Ф", "θ", "Ω", "δ", "∞", "ø", "Є", "∩", "≡", "±", "≥", "≤", "⌠", "⌡", "÷", "≈", "º", 
            "•", "·", "√", "ⁿ", "²", "■", "" };

		public static char[] CP437 = new char[] {
			'\u0000', '\u0001', '\u0002', '\u0003', '\u0004', '\u0005',
			'\u0006', '\u0007', '\u0008', '\u0009', '\u000A', '\u000B',
			'\u000C', '\u000D', '\u000E', '\u000F', '\u0010', '\u0011',
			'\u0012', '\u0013', '\u0014', '\u0015', '\u0016', '\u0017',
			'\u0018', '\u0019', '\u001A', '\u001B', '\u001C', '\u001D',
			'\u001E', '\u001F', '\u0020', '\u0021', '\u0022', '\u0023',
			'\u0024', '\u0025', '\u0026', '\u0027', '\u0028', '\u0029',
			'\u002A', '\u002B', '\u002C', '\u002D', '\u002E', '\u002F',
			'\u0030', '\u0031', '\u0032', '\u0033', '\u0034', '\u0035',
			'\u0036', '\u0037', '\u0038', '\u0039', '\u003A', '\u003B',
			'\u003C', '\u003D', '\u003E', '\u003F', '\u0040', '\u0041',
			'\u0042', '\u0043', '\u0044', '\u0045', '\u0046', '\u0047',
			'\u0048', '\u0049', '\u004A', '\u004B', '\u004C', '\u004D',
			'\u004E', '\u004F', '\u0050', '\u0051', '\u0052', '\u0053',
			'\u0054', '\u0055', '\u0056', '\u0057', '\u0058', '\u0059',
			'\u005A', '\u005B', '\u005C', '\u005D', '\u005E', '\u005F',
			'\u0060', '\u0061', '\u0062', '\u0063', '\u0064', '\u0065',
			'\u0066', '\u0067', '\u0068', '\u0069', '\u006A', '\u006B',
			'\u006C', '\u006D', '\u006E', '\u006F', '\u0070', '\u0071',
			'\u0072', '\u0073', '\u0074', '\u0075', '\u0076', '\u0077',
			'\u0078', '\u0079', '\u007A', '\u007B', '\u007C', '\u007D',
			'\u007E', '\u007F', '\u00C7', '\u00FC', '\u00E9', '\u00E2',
			'\u00E4', '\u00E0', '\u00E5', '\u00E7', '\u00EA', '\u00EB',
			'\u00E8', '\u00EF', '\u00EE', '\u00EC', '\u00C4', '\u00C5',
			'\u00C9', '\u00E6', '\u00C6', '\u00F4', '\u00F6', '\u00F2',
			'\u00FB', '\u00F9', '\u00FF', '\u00D6', '\u00DC', '\u00A2',
			'\u00A3', '\u00A5', '\u20A7', '\u0192', '\u00E1', '\u00ED',
			'\u00F3', '\u00FA', '\u00F1', '\u00D1', '\u00AA', '\u00BA',
			'\u00BF', '\u2310', '\u00AC', '\u00BD', '\u00BC', '\u00A1',
			'\u00AB', '\u00BB', '\u2591', '\u2592', '\u2593', '\u2502',
			'\u2524', '\u2561', '\u2562', '\u2556', '\u2555', '\u2563',
			'\u2551', '\u2557', '\u255D', '\u255C', '\u255B', '\u2510',
			'\u2514', '\u2534', '\u252C', '\u251C', '\u2500', '\u253C',
			'\u255E', '\u255F', '\u255A', '\u2554', '\u2569', '\u2566',
			'\u2560', '\u2550', '\u256C', '\u2567', '\u2568', '\u2564',
			'\u2565', '\u2559', '\u2558', '\u2552', '\u2553', '\u256B',
			'\u256A', '\u2518', '\u250C', '\u2588', '\u2584', '\u258C',
			'\u2590', '\u2580', '\u03B1', '\u00DF', '\u0393', '\u03C0',
			'\u03A3', '\u03C3', '\u00B5', '\u03C4', '\u03A6', '\u0398',
			'\u03A9', '\u03B4', '\u221E', '\u03C6', '\u03B5', '\u2229',
			'\u2261', '\u00B1', '\u2265', '\u2264', '\u2320', '\u2321',
			'\u00F7', '\u2248', '\u00B0', '\u2219', '\u00B7', '\u221A',
			'\u207F', '\u00B2', '\u25A0', '\u00A0'
		};

		public static string ConvertFromCP437ToUTF8(string strData) {
			byte[] data = Encoding.GetEncoding(437).GetBytes(strData);
			char[] utf8 = new char[data.Length];

			for (int i = 0; i < data.Length; i++)
				utf8[i] = CP437[data[i]];

			return new string(utf8);
		}


		static int[,] ColorValues = new int[256, 3] { 
			{ 0, 0, 0 }, { 128, 0, 0 }, { 0, 128, 0 }, { 128, 128, 0 }, { 0, 0, 128 }, { 128, 0, 128 },
			{ 0, 128, 128 }, { 192, 192, 192 }, { 128, 128, 128 }, { 255, 0, 0 }, { 0, 255, 0 }, { 255, 255, 0 },
			{ 0, 0, 255 }, { 255, 0, 255 }, { 0, 255, 255 }, { 255, 255, 255 }, { 0, 0, 0 }, { 0, 0, 95 },
			{ 0, 0, 135 }, { 0, 0, 175 }, { 0, 0, 215 }, { 0, 0, 255 }, { 0, 95, 0 }, { 0, 95, 95 }, 
			{ 0, 95, 135 }, { 0, 95, 175 }, { 0, 95, 215 }, { 0, 95, 255 }, { 0, 135, 0 }, { 0, 135, 95 }, 
			{ 0, 135, 135 }, { 0, 135, 175 }, { 0, 135, 215 }, { 0, 135, 255 }, { 0, 175, 0 }, { 0, 175, 95 }, { 0, 175, 135 }, 
			{ 0, 175, 175 }, { 0, 175, 215 }, { 0, 175, 255 }, { 0, 215, 0 }, { 0, 215, 95 }, { 0, 215, 135 }, { 0, 215, 175 }, 
			{ 0, 215, 215 }, { 0, 215, 255 }, { 0, 255, 0 }, { 0, 255, 95 }, { 0, 255, 135 }, { 0, 255, 175 }, { 0, 255, 215 }, 
			{ 0, 255, 255 }, { 95, 0, 0 }, { 95, 0, 95 }, { 95, 0, 135 }, { 95, 0, 175 }, { 95, 0, 215 }, { 95, 0, 255 }, 
			{ 95, 95, 0 }, { 95, 95, 95 }, { 95, 95, 135 }, { 95, 95, 175 }, { 95, 95, 215 }, { 95, 95, 255 }, { 95, 135, 0 }, 
			{ 95, 135, 95 }, { 95, 135, 135 }, { 95, 135, 175 }, { 95, 135, 215 }, { 95, 135, 255 }, { 95, 175, 0 }, 
			{ 95, 175, 95 }, { 95, 175, 135 }, { 95, 175, 175 }, { 95, 175, 215 }, { 95, 175, 255 }, { 95, 215, 0 }, 
			{ 95, 215, 95 }, { 95, 215, 135 }, { 95, 215, 175 }, { 95, 215, 215 }, { 95, 215, 255 }, { 95, 255, 0 }, 
			{ 95, 255, 95 }, { 95, 255, 135 }, { 95, 255, 175 }, { 95, 255, 215 }, { 95, 255, 255 }, { 135, 0, 0 }, 
			{ 135, 0, 95 }, { 135, 0, 135 }, { 135, 0, 175 }, { 135, 0, 215 }, { 135, 0, 255 }, { 135, 95, 0 }, 
			{ 135, 95, 95 }, { 135, 95, 135 }, { 135, 95, 175 }, { 135, 95, 215 }, { 135, 95, 255 }, { 135, 135, 0 }, 
			{ 135, 135, 95 }, { 135, 135, 135 }, { 135, 135, 175 }, { 135, 135, 215 }, { 135, 135, 255 }, { 135, 175, 0 }, 
			{ 135, 175, 95 }, { 135, 175, 135 }, { 135, 175, 175 }, { 135, 175, 215 }, { 135, 175, 255 }, { 135, 215, 0 }, 
			{ 135, 215, 95 }, { 135, 215, 135 }, { 135, 215, 175 }, { 135, 215, 215 }, { 135, 215, 255 }, { 135, 255, 0 }, 
			{ 135, 255, 95 }, { 135, 255, 135 }, { 135, 255, 175 }, { 135, 255, 215 }, { 135, 255, 255 }, { 175, 0, 0 }, 
			{ 175, 0, 95 }, { 175, 0, 135 }, { 175, 0, 175 }, { 175, 0, 215 }, { 175, 0, 255 }, { 175, 95, 0 }, { 175, 95, 95 }, 
			{ 175, 95, 135 }, { 175, 95, 175 }, { 175, 95, 215 }, { 175, 95, 255 }, { 175, 135, 0 }, { 175, 135, 95 }, 
			{ 175, 135, 135 }, { 175, 135, 175 }, { 175, 135, 215 }, { 175, 135, 255 }, { 175, 175, 0 }, { 175, 175, 95 }, 
			{ 175, 175, 135 }, { 175, 175, 175 }, { 175, 175, 215 }, { 175, 175, 255 }, { 175, 215, 0 }, { 175, 215, 95 }, 
			{ 175, 215, 135 }, { 175, 215, 175 }, { 175, 215, 215 }, { 175, 215, 255 }, { 175, 255, 0 }, { 175, 255, 95 }, 
			{ 175, 255, 135 }, { 175, 255, 175 }, { 175, 255, 215 }, { 175, 255, 255 }, { 215, 0, 0 }, { 215, 0, 95 }, 
			{ 215, 0, 135 }, { 215, 0, 175 }, { 215, 0, 215 }, { 215, 0, 255 }, { 215, 95, 0 }, { 215, 95, 95 }, { 215, 95, 135 }, 
			{ 215, 95, 175 }, { 215, 95, 215 }, { 215, 95, 255 }, { 215, 135, 0 }, { 215, 135, 95 }, { 215, 135, 135 }, 
			{ 215, 135, 175 }, { 215, 135, 215 }, { 215, 135, 255 }, { 215, 175, 0 }, { 215, 175, 95 }, { 215, 175, 135 }, 
			{ 215, 175, 175 }, { 215, 175, 215 }, { 215, 175, 255 }, { 215, 215, 0 }, { 215, 215, 95 }, { 215, 215, 135 }, 
			{ 215, 215, 175 }, { 215, 215, 215 }, { 215, 215, 255 }, { 215, 255, 0 }, { 215, 255, 95 }, { 215, 255, 135 }, 
			{ 215, 255, 175 }, { 215, 255, 215 }, { 215, 255, 255 }, { 255, 0, 0 }, { 255, 0, 95 }, { 255, 0, 135 }, 
			{ 255, 0, 175 }, { 255, 0, 215 }, { 255, 0, 255 }, { 255, 95, 0 }, { 255, 95, 95 }, { 255, 95, 135 }, 
			{ 255, 95, 175 }, { 255, 95, 215 }, { 255, 95, 255 }, { 255, 135, 0 }, { 255, 135, 95 }, { 255, 135, 135 }, 
			{ 255, 135, 175 }, { 255, 135, 215 }, { 255, 135, 255 }, { 255, 175, 0 }, { 255, 175, 95 }, { 255, 175, 135 }, 
			{ 255, 175, 175 }, { 255, 175, 215 }, { 255, 175, 255 }, { 255, 215, 0 }, { 255, 215, 95 }, { 255, 215, 135 },
			{ 255, 215, 175 }, { 255, 215, 215 }, { 255, 215, 255 }, { 255, 255, 0 }, { 255, 255, 95 }, { 255, 255, 135 }, 
			{ 255, 255, 175 }, { 255, 255, 215 }, { 255, 255, 255 }, { 0, 0, 0 }, { 18, 18, 18 }, { 28, 28, 28 }, { 38, 38, 38 }, 
			{ 48, 48, 48 }, { 58, 58, 58 }, { 68, 68, 68 }, { 78, 78, 78 }, { 88, 88, 88 }, { 98, 98, 98 }, { 108, 108, 108 }, 
			{ 118, 118, 118 }, { 128, 128, 128 }, { 138, 138, 138 }, { 148, 148, 148 }, { 158, 158, 158 }, { 168, 168, 168 }, 
			{ 178, 178, 178 }, { 188, 188, 188 }, { 198, 198, 198 }, { 208, 208, 208 }, { 218, 218, 218 }, { 228, 228, 228 }, 
			{ 238, 238, 238 } 
		};

		//static public Color[] Colors = new Color[] { 
		//    Color.FromArgb(0, 0, 0), Color.FromArgb(128, 0, 0), Color.FromArgb(0, 128, 0), 
		//    Color.FromArgb(128, 128, 0), Color.FromArgb(0, 0, 128), Color.FromArgb(128, 0, 128), Color.FromArgb(0, 128, 128), 
		//    Color.FromArgb(192, 192, 192), Color.FromArgb(128, 128, 128), Color.FromArgb(255, 0, 0), Color.FromArgb(0, 255, 0), 
		//    Color.FromArgb(255, 255, 0), Color.FromArgb(0, 0, 255), Color.FromArgb(255, 0, 255), Color.FromArgb(0, 255, 255), 
		//    Color.FromArgb(255, 255, 255), Color.FromArgb(0, 0, 0), Color.FromArgb(0, 0, 95), Color.FromArgb(0, 0, 135), 
		//    Color.FromArgb(0, 0, 175), Color.FromArgb(0, 0, 215), Color.FromArgb(0, 0, 255), Color.FromArgb(0, 95, 0), 
		//    Color.FromArgb(0, 95, 95), Color.FromArgb(0, 95, 135), Color.FromArgb(0, 95, 175), Color.FromArgb(0, 95, 215), 
		//    Color.FromArgb(0, 95, 255), Color.FromArgb(0, 135, 0), Color.FromArgb(0, 135, 95), Color.FromArgb(0, 135, 135), 
		//    Color.FromArgb(0, 135, 175), Color.FromArgb(0, 135, 215), Color.FromArgb(0, 135, 255), Color.FromArgb(0, 175, 0), 
		//    Color.FromArgb(0, 175, 95), Color.FromArgb(0, 175, 135), Color.FromArgb(0, 175, 175), Color.FromArgb(0, 175, 215), 
		//    Color.FromArgb(0, 175, 255), Color.FromArgb(0, 215, 0), Color.FromArgb(0, 215, 95), Color.FromArgb(0, 215, 135), 
		//    Color.FromArgb(0, 215, 175), Color.FromArgb(0, 215, 215), Color.FromArgb(0, 215, 255), Color.FromArgb(0, 255, 0), 
		//    Color.FromArgb(0, 255, 95), Color.FromArgb(0, 255, 135), Color.FromArgb(0, 255, 175), Color.FromArgb(0, 255, 215), 
		//    Color.FromArgb(0, 255, 255), Color.FromArgb(95, 0, 0), Color.FromArgb(95, 0, 95), Color.FromArgb(95, 0, 135), 
		//    Color.FromArgb(95, 0, 175), Color.FromArgb(95, 0, 215), Color.FromArgb(95, 0, 255), Color.FromArgb(95, 95, 0), 
		//    Color.FromArgb(95, 95, 95), Color.FromArgb(95, 95, 135), Color.FromArgb(95, 95, 175), Color.FromArgb(95, 95, 215), 
		//    Color.FromArgb(95, 95, 255), Color.FromArgb(95, 135, 0), Color.FromArgb(95, 135, 95), Color.FromArgb(95, 135, 135), 
		//    Color.FromArgb(95, 135, 175), Color.FromArgb(95, 135, 215), Color.FromArgb(95, 135, 255), Color.FromArgb(95, 175, 0), 
		//    Color.FromArgb(95, 175, 95), Color.FromArgb(95, 175, 135), Color.FromArgb(95, 175, 175), Color.FromArgb(95, 175, 215), 
		//    Color.FromArgb(95, 175, 255), Color.FromArgb(95, 215, 0), Color.FromArgb(95, 215, 95), Color.FromArgb(95, 215, 135), 
		//    Color.FromArgb(95, 215, 175), Color.FromArgb(95, 215, 215), Color.FromArgb(95, 215, 255), Color.FromArgb(95, 255, 0), 
		//    Color.FromArgb(95, 255, 95), Color.FromArgb(95, 255, 135), Color.FromArgb(95, 255, 175), Color.FromArgb(95, 255, 215), 
		//    Color.FromArgb(95, 255, 255), Color.FromArgb(135, 0, 0), Color.FromArgb(135, 0, 95), Color.FromArgb(135, 0, 135), 
		//    Color.FromArgb(135, 0, 175), Color.FromArgb(135, 0, 215), Color.FromArgb(135, 0, 255), Color.FromArgb(135, 95, 0), 
		//    Color.FromArgb(135, 95, 95), Color.FromArgb(135, 95, 135), Color.FromArgb(135, 95, 175), Color.FromArgb(135, 95, 215), 
		//    Color.FromArgb(135, 95, 255), Color.FromArgb(135, 135, 0), Color.FromArgb(135, 135, 95), Color.FromArgb(135, 135, 135), 
		//    Color.FromArgb(135, 135, 175), Color.FromArgb(135, 135, 215), Color.FromArgb(135, 135, 255), Color.FromArgb(135, 175, 0), 
		//    Color.FromArgb(135, 175, 95), Color.FromArgb(135, 175, 135), Color.FromArgb(135, 175, 175), Color.FromArgb(135, 175, 215), 
		//    Color.FromArgb(135, 175, 255), Color.FromArgb(135, 215, 0), Color.FromArgb(135, 215, 95), Color.FromArgb(135, 215, 135), 
		//    Color.FromArgb(135, 215, 175), Color.FromArgb(135, 215, 215), Color.FromArgb(135, 215, 255), Color.FromArgb(135, 255, 0), 
		//    Color.FromArgb(135, 255, 95), Color.FromArgb(135, 255, 135), Color.FromArgb(135, 255, 175), Color.FromArgb(135, 255, 215), 
		//    Color.FromArgb(135, 255, 255), Color.FromArgb(175, 0, 0), Color.FromArgb(175, 0, 95), Color.FromArgb(175, 0, 135), 
		//    Color.FromArgb(175, 0, 175), Color.FromArgb(175, 0, 215), Color.FromArgb(175, 0, 255), Color.FromArgb(175, 95, 0), 
		//    Color.FromArgb(175, 95, 95), Color.FromArgb(175, 95, 135), Color.FromArgb(175, 95, 175), Color.FromArgb(175, 95, 215), 
		//    Color.FromArgb(175, 95, 255), Color.FromArgb(175, 135, 0), Color.FromArgb(175, 135, 95), Color.FromArgb(175, 135, 135), 
		//    Color.FromArgb(175, 135, 175), Color.FromArgb(175, 135, 215), Color.FromArgb(175, 135, 255), Color.FromArgb(175, 175, 0), 
		//    Color.FromArgb(175, 175, 95), Color.FromArgb(175, 175, 135), Color.FromArgb(175, 175, 175), Color.FromArgb(175, 175, 215), 
		//    Color.FromArgb(175, 175, 255), Color.FromArgb(175, 215, 0), Color.FromArgb(175, 215, 95), Color.FromArgb(175, 215, 135), 
		//    Color.FromArgb(175, 215, 175), Color.FromArgb(175, 215, 215), Color.FromArgb(175, 215, 255), Color.FromArgb(175, 255, 0), 
		//    Color.FromArgb(175, 255, 95), Color.FromArgb(175, 255, 135), Color.FromArgb(175, 255, 175), Color.FromArgb(175, 255, 215), 
		//    Color.FromArgb(175, 255, 255), Color.FromArgb(215, 0, 0), Color.FromArgb(215, 0, 95), Color.FromArgb(215, 0, 135), 
		//    Color.FromArgb(215, 0, 175), Color.FromArgb(215, 0, 215), Color.FromArgb(215, 0, 255), Color.FromArgb(215, 95, 0), 
		//    Color.FromArgb(215, 95, 95), Color.FromArgb(215, 95, 135), Color.FromArgb(215, 95, 175), Color.FromArgb(215, 95, 215),
		//    Color.FromArgb(215, 95, 255), Color.FromArgb(215, 135, 0), Color.FromArgb(215, 135, 95), Color.FromArgb(215, 135, 135), 
		//    Color.FromArgb(215, 135, 175), Color.FromArgb(215, 135, 215), Color.FromArgb(215, 135, 255), Color.FromArgb(215, 175, 0), 
		//    Color.FromArgb(215, 175, 95), Color.FromArgb(215, 175, 135), Color.FromArgb(215, 175, 175), Color.FromArgb(215, 175, 215), 
		//    Color.FromArgb(215, 175, 255), Color.FromArgb(215, 215, 0), Color.FromArgb(215, 215, 95), Color.FromArgb(215, 215, 135), 
		//    Color.FromArgb(215, 215, 175), Color.FromArgb(215, 215, 215), Color.FromArgb(215, 215, 255), Color.FromArgb(215, 255, 0), 
		//    Color.FromArgb(215, 255, 95), Color.FromArgb(215, 255, 135), Color.FromArgb(215, 255, 175), Color.FromArgb(215, 255, 215), 
		//    Color.FromArgb(215, 255, 255), Color.FromArgb(255, 0, 0), Color.FromArgb(255, 0, 95), Color.FromArgb(255, 0, 135), 
		//    Color.FromArgb(255, 0, 175), Color.FromArgb(255, 0, 215), Color.FromArgb(255, 0, 255), Color.FromArgb(255, 95, 0), 
		//    Color.FromArgb(255, 95, 95), Color.FromArgb(255, 95, 135), Color.FromArgb(255, 95, 175), Color.FromArgb(255, 95, 215), 
		//    Color.FromArgb(255, 95, 255), Color.FromArgb(255, 135, 0), Color.FromArgb(255, 135, 95), Color.FromArgb(255, 135, 135), 
		//    Color.FromArgb(255, 135, 175), Color.FromArgb(255, 135, 215), Color.FromArgb(255, 135, 255), Color.FromArgb(255, 175, 0), 
		//    Color.FromArgb(255, 175, 95), Color.FromArgb(255, 175, 135), Color.FromArgb(255, 175, 175), Color.FromArgb(255, 175, 215), 
		//    Color.FromArgb(255, 175, 255), Color.FromArgb(255, 215, 0), Color.FromArgb(255, 215, 95), Color.FromArgb(255, 215, 135), 
		//    Color.FromArgb(255, 215, 175), Color.FromArgb(255, 215, 215), Color.FromArgb(255, 215, 255), Color.FromArgb(255, 255, 0), 
		//    Color.FromArgb(255, 255, 95), Color.FromArgb(255, 255, 135), Color.FromArgb(255, 255, 175), Color.FromArgb(255, 255, 215), 
		//    Color.FromArgb(255, 255, 255), Color.FromArgb(0, 0, 0), Color.FromArgb(18, 18, 18), Color.FromArgb(28, 28, 28), 
		//    Color.FromArgb(38, 38, 38), Color.FromArgb(48, 48, 48), Color.FromArgb(58, 58, 58), Color.FromArgb(68, 68, 68), 
		//    Color.FromArgb(78, 78, 78), Color.FromArgb(88, 88, 88), Color.FromArgb(98, 98, 98), Color.FromArgb(108, 108, 108), 
		//    Color.FromArgb(118, 118, 118), Color.FromArgb(128, 128, 128), Color.FromArgb(138, 138, 138), Color.FromArgb(148, 148, 148), 
		//    Color.FromArgb(158, 158, 158), Color.FromArgb(168, 168, 168), Color.FromArgb(178, 178, 178), Color.FromArgb(188, 188, 188), 
		//    Color.FromArgb(198, 198, 198), Color.FromArgb(208, 208, 208), Color.FromArgb(218, 218, 218), Color.FromArgb(228, 228, 228), 
		//    Color.FromArgb(238, 238, 238) 
		//};

		public static int ColorTo256(System.Drawing.Color color) {
			int c;
			int red = color.R;
			int green = color.G;
			int blue = color.B;

			int best_match = 0;
			double d, smallest_distance;

			smallest_distance = 10000000000.0;

			for (c = 0; c < ColorValues.GetLength(0); c++) {
				d = Math.Pow(ColorValues[c, 0] - red, 2.0) +
					Math.Pow(ColorValues[c, 1] - green, 2.0) +
					Math.Pow(ColorValues[c, 2] - blue, 2.0);
				if (d < smallest_distance) {
					smallest_distance = d;
					best_match = c;
				}
			}

			return best_match;
		}

		public static int Color256To16(int ColorNum, bool IsBack) {
			int c;
			int[,] a16rgbtable;
			int red = ColorValues[ColorNum, 0];
			int green = ColorValues[ColorNum, 1];
			int blue = ColorValues[ColorNum, 2];

			a16rgbtable = new int[16, 3] { { 0, 0, 0 }, { 128, 0, 0 }, { 0, 128, 0 },
                                           { 128, 128, 0 }, { 0, 0, 128 }, { 128, 0, 128 },
                                           { 0, 128, 128 }, { 192, 192, 192 }, { 128, 128, 128 },
                                           { 255, 0, 0 }, { 0, 255, 0 }, { 255, 255, 0 },
                                           { 0, 0, 255 }, { 255, 0, 255 }, { 0, 255, 255 },
                                           { 255, 255, 255 } };

			int best_match = 0;
			double d, smallest_distance;

			smallest_distance = 10000000000.0;

			for (c = 0; c < (IsBack ? 8 : 16); c++) {
				d = Math.Pow(a16rgbtable[c, 0] - red, 2.0) +
					Math.Pow(a16rgbtable[c, 1] - green, 2.0) +
					Math.Pow(a16rgbtable[c, 2] - blue, 2.0);
				if (d < smallest_distance) {
					smallest_distance = d;
					best_match = c;
				}
			}

			return best_match;
		}

	}
}
