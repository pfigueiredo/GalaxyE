using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TelnetServer.Game {
	class ANSI {

		public enum ANSIAttribute {
			Off = 0,
			Bold = 1,
			Underscore = 4,
			Blink = 5,
			Reverse = 7,
			Concealed = 8
		}

		public enum ANSIColor_8 {
			Black = 0,
			Red = 1,
			Green = 2,
			BrownYellow = 3,
			Blue = 4,
			Magenta = 5,
			Cyan = 6,
			Gray = 7
		}

		public enum ANSIColor_16 {
			Black = 0,
			Red = 1,
			Green = 2,
			DarkYellow = 3,
			Blue = 4,
			Magenta = 5,
			Cyan = 6,
			Gray = 7,

			Darkgray = 10,
			BrightRed = 11,
			BrightGreen = 12,
			BrightYellow = 13,
			BrightBlue = 14,
			BrightMagenta = 15,
			BrightCyan = 16,
			White = 17
		}


        


        public static string EscapeFormatingCodes(string data) {
            Regex r = new Regex("&([a-zA-Z;]+)\\((.+?)\\)");
            return r.Replace(data, new MatchEvaluator(m => {
                if (m.Success) {
                    var codes = m.Groups[1].Value;
                    var text = m.Groups[2].Value;
                    var cTokens = codes.Split(';');
                    var color = ANSI.ANSIColor_16.Gray;
                    var attr = ANSI.ANSIAttribute.Off;
                    switch (cTokens[0]) {
                        case "R": color = ANSIColor_16.BrightRed; break;
                        case "G": color = ANSIColor_16.BrightGreen; break;
                        case "Y": color = ANSIColor_16.BrightYellow; break;
                        case "B": color = ANSIColor_16.BrightBlue; break;
                        case "M": color = ANSIColor_16.BrightMagenta; break;
                        case "C": color = ANSIColor_16.BrightCyan; break;
                        case "W": color = ANSIColor_16.White; break;
                        case "r": color = ANSIColor_16.Red; break;
                        case "g": color = ANSIColor_16.Green; break;
                        case "y": color = ANSIColor_16.DarkYellow; break;
                        case "b": color = ANSIColor_16.Blue; break;
                        case "m": color = ANSIColor_16.Magenta; break;
                        case "c": color = ANSIColor_16.Cyan; break;
                        case "w": color = ANSIColor_16.Gray; break;
                    }

                    if (cTokens.Length > 1) {
                        switch (cTokens[1]) {
                            case "b": attr = ANSIAttribute.Bold; break;
                            case "u": attr = ANSIAttribute.Underscore; break;
                            case "B": attr = ANSIAttribute.Blink; break;
                            case "r": attr = ANSIAttribute.Reverse; break;
                            case "x": attr = ANSIAttribute.Concealed; break;
                        }
                        return string.Format("{0}{1}{2}{3}", ColorANSI16(color), Attribute(attr), text, ClearFormat());
                    }
                    else
                        return string.Format("{0}{1}{2}", ColorANSI16(color), text, ClearFormat());


                }
                return m.Value;
            }));
        }

		public static string ClearFormat() {
			return string.Format("{0}[0m", (char)27);
		}

		public static string Bold() {
			return string.Format("{0}[{1}m", (char)27, (int)ANSIAttribute.Bold);
		}

		public static string Blink() {
			return string.Format("{0}[{1}m", (char)27, (int)ANSIAttribute.Blink);
		}

		public static string Attribute(ANSIAttribute attribute) {
			return string.Format("{0}[{1}m", (char)27, (int)attribute);
		}

		public static string Bold(string text) {
			return string.Format("{0}[{1}m{2}{3}", (char)27, (int)ANSIAttribute.Bold, text, ClearFormat());
		}

		public static string Blink(string text) {
			return string.Format("{0}[{1}m{2}{3}", (char)27, (int)ANSIAttribute.Blink, text, ClearFormat());
		}

		public static string Attribute(string text, ANSIAttribute attribute) {
			return string.Format("{0}[{1}m{2}{3}", (char)27, (int)attribute, text, ClearFormat());
		}

		public static string ColorANSI16(ANSIColor_16 foreground, ANSIColor_8 background) {

			int bgColor = (int)background + 40;
			int fgColor = (int)foreground + 30;
			int bold = 0;

			if (fgColor >= 40) {
				bold = 1; fgColor -= 10;
			}

			return string.Format("{0}[{1};{2};{3}m", (char)27, bold, fgColor, bgColor);
		}

		public static string ColorANSI16(ANSIColor_16 foreground) {

			int fgColor = (int)foreground + 30;
			int bold = 0;

			if (fgColor >= 40) {
				bold = 1; fgColor -= 10;
			}

			return string.Format("{0}[{2};{1}m", (char)27, fgColor, bold);
		}

		public static string SetColorANSI16(string text, ANSIColor_16 foreground, ANSIColor_8 background) {
			return string.Format("{0}{1}{2}", ColorANSI16(foreground, background), text, ClearFormat());
		}

		public static string SetColorANSI16(string text, ANSIColor_16 foreground) {
			return string.Format("{0}{1}{2}", ColorANSI16(foreground), text, ClearFormat());
		}

		//\033[38;5;${N}m //\033[48;5;${M}m
		public static string SetColor256(byte foreground, byte background) {
			return string.Format("{0}[38;5;{1}m{0}[48;5;{2}m", (char)27, (int)foreground, (int)background);
		}

		public static string SetColor256(string text, byte foreground) {
			return string.Format("{0}[38;5;{1}m", (char)27, (int)foreground, text);
		}

		public static string AtPosition(string text, int line, int column, bool returnToCurrentPos) {
			return string.Format("{5}{0}[{1};{2}H{3}{4}", (char)27, line, column, text,
				returnToCurrentPos ? string.Format("{0}[u", (char)27) : "",
				returnToCurrentPos ? string.Format("{0}[s", (char)27) : ""
			);
		}

		public static string ClearScreen() {
			return string.Format("{0}[2J", (char)27);
		}

		public static string ClearLine() {
			return string.Format("{0}[K", (char)27);
		}

	}
}
