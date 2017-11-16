using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TelnetServer.Game;

namespace TelnetServer.SocketServer {
    class xSend {

        private xConnection conn;
        private static ANSI.ANSIColor_16 messageColor = ANSI.ANSIColor_16.Cyan;

        private void SendLine(string line, params object[] parameters) {

            if (conn.player != null) {
                conn.player.AcquireSendDataLock();
                try {
                    conn.player.SendLine(line, parameters);
                } finally {
                    conn.player.ReleaseSendDataLock();
                }
            } else
                conn.Send(string.Format(line, parameters) + "\n");

        }

        private void SendFormat(string line, params object[] parameters) {

            if (conn.player != null) {
                conn.player.AcquireSendDataLock();
                try {
                    conn.player.SendFormat(line, parameters);
                } finally {
                    conn.player.ReleaseSendDataLock();
                }
            } else
                conn.Send(string.Format(line, parameters));

        }

        public xSend(xConnection conn) {
            this.conn = conn;
        }

        public void SendMessage(string message) {
            this.SendLine("{0}{1}{0} {2}",
                ANSI.SetColorANSI16("-", ANSI.ANSIColor_16.BrightYellow),
                ANSI.SetColorANSI16("♦", ANSI.ANSIColor_16.White),
                ANSI.SetColorANSI16(message, messageColor));
        }

        public void SendSucessMessage(string message, string status) {
            this.SendFormat("{0}{1}{0} {2}",
                ANSI.SetColorANSI16("-", ANSI.ANSIColor_16.BrightCyan),
                ANSI.SetColorANSI16("♦", ANSI.ANSIColor_16.White),
                ANSI.SetColorANSI16(message.PadRight(60), messageColor));

            this.SendFormat("{2}[ {0}{1}{2} ]\r\n",
                ANSI.ColorANSI16(ANSI.ANSIColor_16.Green),
                status,
                ANSI.ClearFormat()
            );
        }

        public void SendFailMessage(string message, string status) {
            this.SendFormat("{0}{1}{0} {2}",
                ANSI.SetColorANSI16("-", ANSI.ANSIColor_16.BrightCyan),
                ANSI.SetColorANSI16("♦", ANSI.ANSIColor_16.White),
                ANSI.SetColorANSI16(message.PadRight(60), messageColor));

                
            this.SendFormat("{2}[ {0}{1}{2} ]\r\n",
                ANSI.ColorANSI16(ANSI.ANSIColor_16.Red),
                status,
                ANSI.ClearFormat()
            );
        }

    }
}
