using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TelnetServer.Game {
	public class TextCanvas {

		public enum NumOfColorsEnum {
			None,
			ANSI,
			XTERM256
		}

		public int Width { get; private set; }
		public int Height { get; private set; }
		public char FillChar { get; set; }
		public NumOfColorsEnum NumOfColors { get; set; }

		public class bufferData {
			public byte Foreground { get; set; }
			public byte Background { get; set; }
			public char Char { get; set; }
		}

		private bufferData[,] buffer;

		public TextCanvas(int width, int height, NumOfColorsEnum numOfColors) {
			this.Width = width;
			this.Height = height;
			this.FillChar = ' ';
			this.NumOfColors = numOfColors;
			this.buffer = new bufferData[width, height];
			this.FillRectangle(0, 0, width, height, 0);
		}

		public void DrawString(int x, int y, string data) {
			DrawString(x, y, data, 250, 0);
		}

		public void DrawString(int x, int y, string data, byte fgColor, byte bgColor) {
			if (y < 0 || y >= buffer.GetLength(1)) return;

			int offset = 0;
			if (x < 0) { offset = Math.Abs(x); x = 0; }

			if (offset > data.Length - 1 || x >= buffer.GetLength(0)) return;

			data = data.Substring(offset);

			for (int i = 0; i < data.Length; i++) {
				if (x + i < buffer.GetLength(0)) {
					bufferData d = new bufferData();
					d.Char = data[i];
					d.Background = bgColor;
					d.Foreground = fgColor;
					buffer[x + i, y] = d;
				}
			}

		}

		public bufferData[,] GetRawData() {
			return buffer;
		}

		public void DrawRawData(int x, int y, bufferData[,] data) {
			for (int dy = 0; dy < data.GetLength(1); dy++) {
				for (int dx = 0; dx < data.GetLength(0); dx++) {
					if (dx + x < this.buffer.GetLength(0) &&
						dy + y < this.buffer.GetLength(1)) 
					{
						this.buffer[dx + x, dy + y] = data[dx, dy];
					}
				}
			}
		}

		public void DrawGrid(int x, int y, int width, int height, byte color) {

			string topDef = "───┬";
			string lineDef = "───┼";
			string bottomDef = "───┴";

			string topLine = "", line = "", bottomLine = "";

			for (int i = 1; i < width - 1; i += 4) {
				topLine += topDef;
				line += lineDef;
				bottomLine += bottomDef;
			}

			topLine = topLine.Substring(0, width - 2);
			bottomLine = bottomLine.Substring(0, width - 2);
			line = line.Substring(0, width - 2);

			DrawString(x, y, string.Format("┌{0}┐", topLine), color, 0);
			for (int i = 1; i < height - 1; i++, y++) {
				DrawString(x, y + 1, string.Format("│{0}│", line), color, 0);
			}
			DrawString(x, y + 1, string.Format("└{0}┘", bottomLine), color, 0);
		}
		
		public void DrawRectangle(int x, int y, int width, int height, byte borderColor) {
			DrawString(x, y, string.Format("┌{0}┐", string.Empty.PadLeft(width - 2, '─')), borderColor, 0);
			for (int i = 1; i < height - 1; i++, y++) {
				DrawString(x, y + 1, "│", borderColor, 0);
				DrawString(width - 1, y + 1, "│", borderColor, 0);
			}
			DrawString(x, y + 1, string.Format("└{0}┘", string.Empty.PadLeft(width - 2, '─')), borderColor, 0);
		}

		public void DrawHorizontalLine(int x, int y, int width, byte color) {
			DrawString(x, y, string.Format("{0}", string.Empty.PadLeft(width, '─')), color, 0);
		}

		public void FillRectangle(int x, int y, int width, int height, byte Color) {
			for (int i = 1; i < height - 1; i++) {
				DrawString(x, y + i, string.Empty.PadLeft(width, ' '), Color, Color);
			}
		}

		public override string ToString() {
			StringBuilder sBuilder = new StringBuilder();
			for (int y = 0; y < buffer.GetLength(1); y++) {
				byte lastBgColor = 0;
				byte lastFgColor = 0;
				for (int x = 0; x < buffer.GetLength(0); x++) {
					var d = buffer[x, y];

					if (d != null) {
						bool printProperties = (x == 0 || lastBgColor != d.Background || lastFgColor != d.Foreground);

						if (printProperties) {

							lastBgColor = d.Background;
							lastFgColor = d.Foreground;

							string escSequence = "";
							if (NumOfColors == NumOfColorsEnum.XTERM256) {
								escSequence = ANSI.SetColor256(d.Foreground, d.Background);
							} else if (NumOfColors == NumOfColorsEnum.ANSI) {
								escSequence = ANSI.ColorANSI16(
									(ANSI.ANSIColor_16)FANSI.Color256To16(d.Foreground, false),
									(ANSI.ANSIColor_8)FANSI.Color256To16(d.Background, true)
								);
							}
							sBuilder.Append(escSequence);
						}

						sBuilder.Append(d.Char);
					} else
						sBuilder.Append(this.FillChar);
				}

				sBuilder.AppendFormat("{0}\r\n", ANSI.ClearFormat());
			}

			sBuilder.Append(ANSI.ClearFormat());

			return sBuilder.ToString();
		}


	}
}
