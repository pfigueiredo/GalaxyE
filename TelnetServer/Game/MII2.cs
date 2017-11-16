using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TelnetServer.Game {
	public class MII2 {

		public string Header { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
		public bool Error { get; set; }
		public int ErrorLine { get; set; }
		private TextCanvas.bufferData[,] buffer;

		public MII2() { }

		public void ReadData(TextReader tr) {

			Error = false;
			ErrorLine = 0;

			int nLine = 0;

			string head = tr.ReadLine(); nLine++;
			string strW = tr.ReadLine(); nLine++;
			string strH = tr.ReadLine(); nLine++;

			bool ok = head != null && strH != null && strW != null;
			if (ok) {

				int w = 0, h = 0;

				ok &= int.TryParse(strH, out h);
				ok &= int.TryParse(strW, out w);

				this.Height = h;
				this.Width = w;
				this.Header = head;
			}

			if (ok) {

				this.buffer = new TextCanvas.bufferData[Width, Height];

				for (int y = 0; y < this.Height; y++) {
					for (int x = 0; x < this.Width; x++) {
						string line = tr.ReadLine(); nLine++;
						if (line != null) {
							string[] tokens = line.Split(',');
							if (ok &= (tokens.Length == 4)) {
								string strFG = tokens[0];
								string strBG = tokens[1];
								string strChar = tokens[2];

								TextCanvas.bufferData data = new TextCanvas.bufferData();

								byte fg, bg;
								int iChar;

								ok &= byte.TryParse(strFG, out fg);
								ok &= byte.TryParse(strBG, out bg);
								ok &= int.TryParse(strChar, out iChar);

								data.Background = bg;
								data.Foreground = fg;

								if (ok &= (iChar <= FANSI.CP437.Length))
									data.Char = FANSI.CP437[iChar];

								if (ok) {
									buffer[x, y] = data;
								} else
									break;
							}
						}
					}

					if (!ok) break;
				}
			}

			Error = !ok;
			if (Error)
				ErrorLine = nLine;
		}

		public void WriteTile(TextCanvas canvas) {
			for (int y = 0; y < canvas.Height; y += this.buffer.GetLength(1))
				for (int x = 0; x < canvas.Width; x += this.buffer.GetLength(0))
					canvas.DrawRawData(x, y, buffer);
		}

		public void Write(int x, int y, TextCanvas canvas) {
			canvas.DrawRawData(x, y, buffer);
		}

		public override string ToString() {
			TextCanvas canvas = new TextCanvas(this.Width, this.Height, TextCanvas.NumOfColorsEnum.XTERM256);
			canvas.DrawRawData(0, 0, buffer);
			return canvas.ToString();
		}

	}
}
