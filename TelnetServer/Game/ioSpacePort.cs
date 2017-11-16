using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaxyE.Galaxy;
using System.IO;
using GalaxyE.Galaxy.Zones;

namespace TelnetServer.Game {
    public class ioSpacePort {

        public SpacePort SpacePort { get; private set; }
        public TextCanvas Canvas { get; private set; }

        private int screenWidth = 0;

        public ioSpacePort(SpacePort port) : this (port, 79) { }

        public ioSpacePort(SpacePort port, int screenWidth) {
            this.SpacePort = port;
            this.screenWidth = screenWidth;
        }

        public string GetInfo() {

            string baseDir = "./Messages/Citys/";
            string spfile = "p1.png.MII";
            TextCanvas canvas = new TextCanvas(screenWidth, 18, TextCanvas.NumOfColorsEnum.XTERM256);
            StringBuilder sBuilder = new StringBuilder();

            canvas.FillRectangle(0, 0, 44, 18, 234);

            string tileFilename = Path.Combine(baseDir, spfile);
            var img = Messages.Message.ReadMII2File(tileFilename);
            img.Write(2, 2, canvas);

            canvas.DrawString(45, 1, string.Format("Port: {0}", SpacePort.Name), (byte)ANSI.ANSIColor_16.Cyan, 0);
            canvas.DrawString(45, 2, string.Format("Planet: {0}", SpacePort.Body.PlanetName), (byte)ANSI.ANSIColor_16.Cyan, 0);
            canvas.DrawString(45, 3, string.Format("Planet Sector: {0}", SpacePort.PlanetSector), (byte)ANSI.ANSIColor_16.Cyan, 0);

            sBuilder.AppendLine(canvas.ToString());
            return sBuilder.ToString();

        }

    }
}
