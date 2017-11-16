using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TelnetServer.Game {
	class ioShipComputer {

		private ioPlayer player;
		private static ANSI.ANSIColor_16 messageColor = ANSI.ANSIColor_16.Cyan;

		public ioShipComputer(ioPlayer player) {
			this.player = player;
		}

		public void SendMessage(string message) {
			player.AcquireSendDataLock();
			try {

				player.SendLine("{0}{1}{0} {2}",
					ANSI.SetColorANSI16("-", ANSI.ANSIColor_16.BrightYellow),
					ANSI.SetColorANSI16("♦", ANSI.ANSIColor_16.White),
					ANSI.SetColorANSI16(message, messageColor));

				//player.SendLine(string.Format("{0}", message));
				System.Threading.Thread.Sleep(500);
			} finally {
				player.ReleaseSendDataLock();
			}
		}

		public void SendSucessMessage(string message, string status) {
			player.AcquireSendDataLock();
			try {

				player.SendFormat("{0}{1}{0} {2}",
					ANSI.SetColorANSI16("-", ANSI.ANSIColor_16.BrightCyan),
					ANSI.SetColorANSI16("♦", ANSI.ANSIColor_16.White),
					ANSI.SetColorANSI16(message.PadRight(60), messageColor));

				System.Threading.Thread.Sleep(100);
				player.SendFormat("{2}[ {0}{1}{2} ]\r\n",
					ANSI.ColorANSI16(ANSI.ANSIColor_16.Green),
					status,
					ANSI.ClearFormat()
				);
				System.Threading.Thread.Sleep(500);
			} finally {
				player.ReleaseSendDataLock();
			}
		}

		public void SendFailMessage(string message, string status) {
			player.AcquireSendDataLock();
			try {

				player.SendFormat("{0}{1}{0} {2}",
					ANSI.SetColorANSI16("-", ANSI.ANSIColor_16.BrightCyan),
					ANSI.SetColorANSI16("♦", ANSI.ANSIColor_16.White),
					ANSI.SetColorANSI16(message.PadRight(60), messageColor));

				System.Threading.Thread.Sleep(100);
				player.SendFormat("{2}[ {0}{1}{2} ]\r\n",
					ANSI.ColorANSI16(ANSI.ANSIColor_16.Red),
					status,
					ANSI.ClearFormat()
				);
				System.Threading.Thread.Sleep(500);
			} finally {
				player.ReleaseSendDataLock();
			}

		}

		public void SendCountDownMessage(string message, string status, int steps, int stepTime, bool showNumbers) {

			player.AcquireSendDataLock();
			try {
				int messageLenght = message.Length;

				player.SendFormat("{0}{1}{0} {2}",
					ANSI.SetColorANSI16("-", ANSI.ANSIColor_16.BrightRed),
					ANSI.SetColorANSI16("♦", ANSI.ANSIColor_16.White),
					ANSI.SetColorANSI16(message, messageColor));

				for (int i = 0; i <= steps; i++) {
					System.Threading.Thread.Sleep(stepTime);
					if (showNumbers) {
						player.SendFormat("{0} ", 
							ANSI.SetColorANSI16((steps - i).ToString(), ANSI.ANSIColor_16.BrightCyan));
						messageLenght += 2;
					} else {
						player.SendFormat(ANSI.SetColorANSI16(".", ANSI.ANSIColor_16.BrightCyan));
						messageLenght++;
					}
				}

				player.SendFormat("{3}{2}[ {0}{1}{2} ]\r\n",
					ANSI.ColorANSI16(ANSI.ANSIColor_16.Green),
					status,
					ANSI.ClearFormat(),
					"".PadRight(60 - messageLenght)
				);
				System.Threading.Thread.Sleep(500);
			} finally {
				player.ReleaseSendDataLock();
			}
		}


	}
}
