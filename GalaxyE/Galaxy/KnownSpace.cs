using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalaxyE.Galaxy {
	public class KnownSpace {

		public class KnownSpaceStar {
			public uint SectorX;
 			public uint SectorY;
			public int SysNum; 
			public string TypeCode;
			public string Name;
			public int SX;
			public int SY;
			public int SZ;
		}

		#region KnownSystems array
		
		public static bool Generated = false;
		public static Dictionary<string, List<KnownSpaceStar>> Data = new Dictionary<string, List<KnownSpaceStar>>(); 

		public static object[] knownSystems = new object[] {
            "Sol",                      "Sol",                   0D,     0D,  "G3V            ",   0D,                
            "Alpha Canis Majoris     ", "Sirius            ",227.2D,  -8.9D,  "A1V            ",   9D,
            "Alpha Carinae           ", "Canopus           ",261.2D, -25.3D,  "F0Ib           ", 310D,
            "Alpha Centauri          ", "Alpha Centauri    ",315.8D,  -0.7D,  "G2V+K1V        ",   4D,
            "Alpha Boötis            ", "Arcturus          ", 15.2D, +69.0D,  "K2III          ",  37D,
            "Alpha Lyrae             ", "Vega              ", 67.5D, +19.2D,  "A0V            ",  25D,
            "Alpha Aurigae           ", "Capella           ",162.6D,  +4.6D,  "G5III+G0III    ",  42D,
            "Beta Orionis            ", "Rigel             ",209.3D, -25.1D,  "B8Ia           ", 770D,
            "Alpha Canis Minoris     ", "Procyon           ",213.7D, +13.0D,  "F5IV-V         ",  11D,
            "Alpha Eridani           ", "Achernar          ",290.7D, -58.8D,  "B3V            ", 144D,
            "Alpha Orionis           ", "Betelgeuse        ",199.8D,  -9.0D,  "M2Ib           ", 430D,
            "Beta Centauri           ", "Hadar             ",311.8D,  +1.2D,  "B1III          ", 530D,
            "Alpha Aquilae           ", "Altair            ", 47.8D,  -9.0D,  "A7V            ",  17D,
            "Alpha Crucis            ", "Acrux             ",300.2D,  -0.4D,  "B0.5IV+B1V     ", 320D,
            "Alpha Tauri             ", "Aldebaran         ",181.0D, -20.2D,  "K5III          ",  65D,
            "Alpha Scorpii           ", "Antares           ",351.9D, +15.1D,  "M1Ib+B4V       ", 600D,
            "Alpha Virginis          ", "Spica             ",316.1D, +50.8D,  "B1V+B2V        ", 260D,
            "Beta Geminorum          ", "Pollux            ",192.2D, +23.3D,  "K0III          ",  34D,
            "Alpha Piscis Austrini   ", "Fomalhaut         ", 20.6D, -65.0D,  "A3V            ",  25D,
            "Alpha Cygni             ", "Deneb             ", 84.3D,  +2.1D,  "A2Ia           ",3000D,
            "Beta Crucis             ", "Mimosa            ",302.5D,  +3.2D,  "B0.5III        ", 350D,
            "Alpha Leonis            ", "Regulus           ",226.3D, +48.9D,  "B7V            ",  78D,
            "Epsilon Canis Majoris   ", "Adhara            ",239.9D, -11.3D,  "B2II           ", 430D,
            "Alpha Geminorum         ", "Castor            ",187.5D, +22.6D,  "A1V+A2V        ",  52D,
            "Lambda Scorpii          ", "Shaula            ",351.8D,  -2.3D,  "B2IV           ", 700D,
            "Gamma Crucis            ", "Gacrux            ",300.2D,  +5.7D,  "M3.5III        ",  88D,
            "Gamma Orionis           ", "Bellatrix         ",197.0D, -16.0D,  "B2III          ", 240D,
            "Beta Tauri              ", "Elnath            ",178.0D,  -3.8D,  "B7III          ", 130D,
            "Beta Carinae            ", "Miaplacidus       ",286.0D, -14.4D,  "A2III          ", 111D,
            "Epsilon Orionis         ", "Alnilam           ",205.2D, -17.3D,  "B0Ia           ",1300D,
            "Alpha Gruis             ", "Alnair            ",350.0D, -52.4D,  "B7IV           ", 101D,
            "Zeta Orionis            ", "Alnitak           ",206.5D, -16.5D,  "O9.5Ib+B0III   ", 820D,
            "Epsilon Ursae Majoris   ", "Alioth            ",122.2D, +61.1D,  "A0IV           ",  81D,
            "Alpha Persei            ", "Mirfak            ",146.5D,  -5.9D,  "F5Ib           ", 590D,
            "Alpha Ursae Majoris     ", "Dubhe             ",142.8D, +51.0D,  "K0III+F0V      ", 124D,
            "Gamma Velorum           ", "Regor             ",262.8D,  -7.6D,  "WC8+O9Ib       ", 840D,
            "Delta Canis Majoris     ", "Wezen             ",238.4D,  -8.3D,  "F8Ia           ",1800D,
            "Epsilon Sagittarii      ", "Kaus Australis    ",359.2D,  -9.8D,  "B9.5III        ", 145D,
            "Eta Ursae Majoris       ", "Alkaid            ",100.5D, +65.3D,  "B3V            ", 101D,
            "Theta Scorpii           ", "Sargas            ",347.1D,  -5.9D,  "F1II           ", 270D,
            "Epsilon Carinae         ", "Avior             ",274.3D, -12.5D,  "K3II+B2V       ", 630D,
            "Beta Aurigae            ", "Menkalinan        ",167.5D, +10.5D,  "A2IV           ",  82D,
            "Alpha Trianguli Australis", "Atria            ",321.6D, -15.3D,  "K2Ib-II        ", 420D,
            "Gamma Geminorum         ", "Alhena            ",196.8D,  +4.5D,  "A0IV           ", 105D,
            "Alpha Pavonis           ", "Peacock           ",340.9D, -35.3D,  "B0.5V+B2V      ", 180D,
            "Delta Velorum           ", "Koo She           ",272.1D,  -7.3D,  "A0V            ",  80D,
            "Beta Canis Majoris      ", "Mirzam            ",226.1D, -14.2D,  "B1III          ", 500D,
            "Alpha Hydrae            ", "Alphard           ",241.6D, +29.1D,  "K3II           ", 180D,
            "Alpha Ursae Minoris     ", "Polaris           ",123.3D, +26.5D,  "F7Ib-II        ", 430D,
            "Gamma Leonis            ", "Algieba           ",216.6D, +54.7D,  "K0III+G7III    ", 126D,
            "Alpha Arietis           ", "Hamal             ",144.5D, -36.2D,  "K2III          ",  66D,
            "Beta Ceti               ", "Diphda            ",112.0D, -80.7D,  "K0III          ",  96D,
            "Sigma Sagittarii        ", "Nunki             ",  9.5D, -12.4D,  "B3V            ", 220D,
            "Theta Centauri          ", "Menkent           ",319.5D, +24.0D,  "K0III          ",  61D,
            "Alpha Andromedae        ", "Alpheratz         ",111.6D, -32.8D,  "B9IV           ",  97D,
            "Beta Andromedae         ", "Mirach            ",127.2D, -27.1D,  "M0II           ", 200D,
            "Kappa Orionis           ", "Saiph             ",214.6D, -18.4D,  "B0.5III        ", 720D,
            "Beta Ursae Minoris      ", "Kochab            ",112.7D, +40.5D,  "K4III          ", 127D,
            "Beta Gruis              ", "Al Dhanab         ",346.2D, -58.0D,  "M5III          ", 170D,
            "Alpha Ophiuchi          ", "Rasalhague        ", 35.9D, +22.6D,  "A5III-IV       ",  47D,
            "Beta Persei             ", "Algol             ",148.9D, -14.9D,  "B8V+G5IV+A     ",  93D,
            "Gamma Andromedae        ", "Almach            ",137.0D, -18.6D,  "K3II+B8V+A0V   ", 360D,
            "Beta Leonis             ", "Denebola          ",250.6D, +70.8D,  "A3V            ",  36D,
            "Gamma Cassiopeiae       ", "Cih               ",123.6D,  -2.2D,  "B0IV           ", 610D,
            "Gamma Centauri          ", "Muhlifain         ",301.3D, +13.8D,  "A0III+A0III    ", 130D,
            "Zeta Puppis             ", "Naos              ",256.0D,  -4.6D,  "O5Ia           ",1400D,
            "Iota Carinae            ", "Aspidiske         ",278.5D,  -7.0D,  "A8Ib           ", 690D,
            "Alpha Coronae Borealis  ", "Alphecca          ", 41.9D, +53.7D,  "A0V+G5V        ",  75D,
            "Lambda Velorum          ", "Suhail            ",265.9D,  +2.9D,  "K4Ib           ", 570D,
            "Zeta Ursae Majoris      ", "Mizar             ",113.1D, +61.6D,  "A2V+A2V+A1V    ",  78D,
            "Gamma Cygni             ", "Sadr              ", 78.2D,  +1.9D,  "F8Ib           ",1500D,
            "Alpha Cassiopeiae       ", "Schedar           ",121.5D,  -6.3D,  "K0II           ", 230D,
            "Gamma Draconis          ", "Eltanin           ", 79.1D, +29.1D,  "K5III          ", 148D,
            "Delta Orionis           ", "Mintaka           ",203.9D, -17.7D,  "O9.5II+B2V     ", 920D,
            "Beta Cassiopeiae        ", "Caph              ",117.5D,  -3.2D,  "F2III          ",  55D,
            "Epsilon Centauri        ", "                  ",310.2D,  +8.7D,  "B1III          ", 380D,
            "Delta Scorpii           ", "Dschubba          ",350.1D, +22.6D,  "B0.5IV         ", 400D,
            "Epsilon Scorpii         ", "Wei               ",348.8D,  +6.6D,  "K2.5III        ",  65D,
            "Alpha Lupi              ", "Men               ",321.6D, +11.4D,  "B1.5III        ", 550D,
            "Eta Centauri            ", "                  ",322.9D, +16.6D,  "B1.5V          ", 310D,
            "Beta Ursae Majoris      ", "Merak             ",149.1D, +54.8D,  "A1V            ",  79D,
            "Epsilon Boötis          ", "Izar              ", 39.4D, +64.8D,  "K0II-III+A2V   ", 210D,
            "Epsilon Pegasi          ", "Enif              ", 65.6D, -31.4D,  "K2Ib           ", 670D,
            "Kappa Scorpii           ", "Girtab            ",351.0D,  -4.6D,  "B1.5III        ", 460D,
            "Alpha Phoenicis         ", "Ankaa             ",320.2D, -74.0D,  "K0III          ",  77D,
            "Gamma Ursae Majoris     ", "Phecda            ",140.8D, +61.4D,  "A0V            ",  84D,
            "Eta Ophiuchi            ", "Sabik             ",  6.7D, +14.1D,  "A1V+A3V        ",  84D,
            "Beta Pegasi             ", "Scheat            ", 95.8D, -29.1D,  "M2III          ", 200D,
            "Eta Canis Majoris       ", "Aludra            ",242.6D,  -6.5D,  "B5Ia           ",3000D,
            "Alpha Cephei            ", "Alderamin         ",101.0D,  +9.1D,  "A7IV           ",  49D,
            "Kappa Velorum           ", "Markeb            ",275.9D,  -3.5D,  "B2IV           ", 540D,
            "Epsilon Cygni           ", "Gienah            ", 76.0D,  -5.7D,  "K0III          ",  72D,
            "Alpha Pegasi            ", "Markab            ", 88.4D, -40.4D,  "B9IV           ", 140D,
            "Alpha Ceti              ", "Menkar            ",173.3D, -45.6D,  "M2III          ", 220D,
            "Zeta Ophiuchi           ", "Han               ",  6.2D, +23.6D,  "O9.5V          ", 460D,
            "Zeta Centauri           ", "Al Nair al Kent.  ",314.2D, +14.2D,  "B2.5IV         ", 390D,
            "Delta Leonis            ", "Zosma             ",224.3D, +66.8D,  "A4V            ",  58D,
            "Beta Scorpii            ", "Graffias          ",353.1D, +23.7D,  "B1V+B2V        ", 530D,
            "Alpha Leporis           ", "Arneb             ",221.0D, -25.1D,  "F0Ib           ",1300D,
            "Delta Centauri          ", "                  ",295.9D, +11.6D,  "B2IV           ", 400D,
            "Gamma Corvi             ", "Gienah Ghurab     ",291.1D, +44.6D,  "B8III          ", 165D,
            "Zeta Sagittarii         ", "Ascella           ",  6.9D, -15.5D,  "A2IV+A4V       ",  89D,
            "Beta Librae             ", "Zubeneschamali    ",352.0D, +39.2D,  "B8V            ", 160D,
            "Alpha Serpentis         ", "Unukalhai         ", 14.1D, +44.1D,  "K2III          ",  73D,
            "Beta Arietis            ", "Sheratan          ",142.4D, -39.7D,  "A5V            ",  60D,
            "Alpha Librae            ", "Zubenelgenubi     ",340.4D, +38.0D,  "A3IV+F4IV      ",  77D,
            "Alpha Columbae          ", "Phact             ",238.9D, -28.8D,  "B7IV           ", 270D,
            "Theta Aurigae           ", "                  ",174.4D,  +6.8D,  "A0III+G2V      ", 170D,
            "Beta Corvi              ", "Kraz              ",297.8D, +39.3D,  "G5III          ", 140D,
            "Delta Cassiopeiae       ", "Ruchbah           ",127.2D,  -2.4D,  "A5III          ",  99D,
            "Eta Boötis              ", "Muphrid           ",  5.5D, +73.0D,  "G0IV           ",  37D,
            "Beta Lupi               ", "Ke Kouan          ",326.4D, +13.9D,  "B2III          ", 520D,
            "Iota Aurigae            ", "Hassaleh          ",170.6D,  -6.1D,  "K3II           ", 510D,
            "Mu Velorum              ", "                  ",283.1D,  +8.6D,  "G5III+G2V      ", 116D,
            "Alpha Muscae            ", "                  ",301.6D,  -6.3D,  "B2V            ", 310D,
            "Upsilon Scorpii         ", "Lesath            ",351.3D,  -1.9D,  "B2IV           ", 520D,
            "Pi Puppis               ", "                  ",249.0D, -11.3D,  "K4Ib           ",1100D,
            "Delta Sagittarii        ", "Kaus Meridionalis ",  3.0D,  -7.2D,  "K2II           ", 310D,
            "Gamma Aquilae           ", "Tarazed           ", 48.7D,  -7.0D,  "K3II           ", 460D,
            "Delta Ophiuchi          ", "Yed Prior         ",  8.8D, +32.3D,  "M1III          ", 170D,
            "Eta Draconis            ", "Aldhibain         ", 92.6D, +40.9D,  "G8III          ",  88D,
            "Theta Carinae           ", "                  ",289.6D,  -4.9D,  "B0V            ", 440D,
            "Gamma Virginis          ", "Porrima           ",298.1D, +61.3D,  "F0V+F0V        ",  39D,
            "Iota Orionis            ", "Hatysa            ",209.5D, -19.7D,  "O9III          ",1300D,
            "Iota Centauri           ", "                  ",309.5D, +25.8D,  "A2V            ",  59D,
            "Beta Ophiuchi           ", "Cebalrai          ", 29.2D, +17.3D,  "K2III          ",  82D,
            "Beta Eridani            ", "Kursa             ",205.4D, -25.3D,  "A3III          ",  89D,
            "Beta Herculis           ", "Kornephoros       ", 39.0D, +40.3D,  "G7III          ", 150D,
            "Delta Crucis            ", "                  ",298.2D,  +3.8D,  "B2IV           ", 360D,
            "Beta Draconis           ", "Rastaban          ", 79.6D, +33.4D,  "G2II           ", 360D,
            "Alpha Canum Venaticorum ", "Cor Caroli        ",118.3D, +78.8D,  "A0IV+F0V       ", 110D,
            "Gamma Lupi              ", "                  ",333.2D, +11.9D,  "B2IV-V+B2IV-V  ", 570D,
            "Beta Leporis            ", "Nihal             ",223.6D, -27.3D,  "G5III          ", 160D,
            "Zeta Herculis           ", "Rutilicus         ", 52.6D, +40.3D,  "F9IV+G7V       ",  35D,
            "Beta Hydri              ", "                  ",304.7D, -39.7D,  "G2IV           ",  24D,
            "Tau Scorpii             ", "                  ",351.6D, +12.8D,  "B0V            ", 430D,
            "Lambda Sagittarii       ", "Kaus Borealis     ",  7.7D,  -6.5D,  "K1III          ",  77D,
            "Gamma Pegasi            ", "Algenib           ",109.4D, -46.7D,  "B2IV           ", 330D,
            "Rho Puppis              ", "Turais            ",243.2D,  +4.5D,  "F6III          ",  63D,
            "Beta Trianguli Australis", "                  ",321.9D,  -7.5D,  "F2IV           ",  40D,
            "Zeta Persei             ", "                  ",162.3D, -16.7D,  "B1II+B8IV+A2V  ", 980D,
            "Beta Arae               ", "                  ",335.4D, -11.0D,  "K3Ib-II        ", 600D,
            "Alpha Arae              ", "Choo              ",340.8D,  -8.9D,  "B2V            ", 240D,
            "Eta Tauri               ", "Alcyone           ",166.6D, -23.5D,  "B7III          ", 370D,
            "Epsilon Virginis        ", "Vindemiatrix      ",312.3D, +73.7D,  "G8III          ", 102D,
            "Delta Capricorni        ", "Deneb Algedi      ", 37.6D, -46.0D,  "A5V            ",  39D,
            "Alpha Hydri             ", "Head of Hydrus    ",289.4D, -53.7D,  "F0III          ",  71D,
            "Delta Cygni             ", "                  ", 78.7D, +10.2D,  "B9.5III+F1V    ", 170D,
            "Mu Geminorum            ", "Tejat             ",189.8D,  +4.2D,  "M3III          ", 230D,
            "Gamma Trianguli Australis", "                 ",316.5D,  -8.4D,  "A1III          ", 180D,
            "Alpha Tucanae           ", "                  ",330.1D, -48.0D,  "K3III          ", 200D,
            "Theta Eridani           ", "Acamar            ",247.9D, -60.7D,  "A4III+A1V      ", 160D,
            "Pi Sagittarii           ", "Albaldah          ", 15.9D, -13.3D,  "F2II           ", 440D,
            "Beta Canis Minoris      ", "Gomeisa           ",209.5D, +11.7D,  "B8V            ", 170D,
            "Pi Scorpii              ", "                  ",347.2D, +20.2D,  "B1V+B2V        ", 460D,
            "Epsilon Persei          ", "                  ",157.4D, -10.1D,  "B0.5V+A2V      ", 540D,
            "Sigma Scorpii           ", "Alniyat           ",351.3D, +17.0D,  "B1III          ", 730D,
            "Beta Cygni              ", "Albireo           ", 62.1D,  +4.6D,  "K3II+B8V+B9V   ", 390D,
            "Beta Aquarii            ", "Sadalsuud         ", 48.0D, -37.9D,  "G0Ib           ", 610D,
            "Gamma Persei            ", "                  ",142.1D,  -4.3D,  "G8III+A2V      ", 260D,
            "Upsilon Carinae         ", "                  ",285.0D,  -8.8D,  "A7Ib+B7III     ",1600D,
            "Eta Pegasi              ", "Matar             ", 92.5D, -25.0D,  "G2II-III+F0V   ", 215D,
            "Tau Puppis              ", "                  ",260.2D, -20.9D,  "K1III          ", 185D,
            "Delta Corvi             ", "Algorel           ",295.5D, +46.0D,  "B9.5V          ",  88D,
            "Alpha Aquarii           ", "Sadalmelik        ", 59.9D, -42.1D,  "G2Ib           ", 760D,
            "Gamma Eridani           ", "Zaurak            ",205.2D, -44.5D,  "M1III          ", 220D,
            "Zeta Tauri              ", "Alheka            ",185.7D,  -5.6D,  "B4III          ", 420D,
            "Epsilon Leonis          ", "Ras Elased Austr. ",206.8D, +48.2D,  "G1II           ", 250D,
            "Gamma² Sagittarii       ", "Alnasl            ",  0.9D,  -4.5D,  "K0III          ",  96D,
            "Gamma Hydrae            ", "                  ",311.1D, +39.3D,  "G8III          ", 132D,
            "Iota¹ Scorpii           ", "                  ",350.6D,  -6.1D,  "F2Ia           ",1800D,
            "Zeta Aquilae            ", "Deneb el Okab     ", 46.9D,  +3.3D,  "A0V            ",  83D,
            "Beta Trianguli          ", "                  ",140.6D, -25.2D,  "A5III          ", 124D,
            "Psi Ursae Majoris       ", "                  ",165.8D, +63.2D,  "K1III          ", 147D,
            "Gamma Ursae Minoris     ", "Pherkad Major     ",108.5D, +40.8D,  "A3II           ", 480D,
            "Mu¹ Scorpii             ", "                  ",346.1D,  +3.9D,  "B1.5V+B6.5V    ", 820D,
            "Gamma Gruis             ", "                  ",  6.1D, -51.5D,  "B8III          ", 205D,
            "Delta Persei            ", "                  ",150.3D,  -5.8D,  "B5III          ", 530D,
            "Zeta Canis Majoris      ", "Phurad            ",237.5D, -19.4D,  "B2.5V          ", 340D,
            "Omicron² Canis Majoris  ", "                  ",235.6D,  -8.2D,  "B3Ia           ",2600D,
            "Epsilon Corvi           ", "Minkar            ",290.6D, +39.3D,  "K2II           ", 300D,
            "Epsilon Aurigae         ", "Almaaz            ",162.8D,  +1.2D,  "F0Ia           ",2000D,
            "Beta Muscae             ", "                  ",302.5D,  -5.2D,  "B2V+B3V        ", 310D,
            "Gamma Boötis            ", "Seginus           ", 67.3D, +66.2D,  "A7III          ",  85D,
            "Beta Capricorni         ", "Dabih             ", 29.2D, -26.4D,  "G5II+A0V       ", 340D,
            "Epsilon Geminorum       ", "Mebsuta           ",189.5D,  +9.6D,  "G8Ib           ", 900D,
            "Mu Ursae Majoris        ", "Tania Australis   ",177.9D, +56.4D,  "M0III          ", 250D,
            "Delta Draconis          ", "Tais              ", 98.7D, +23.0D,  "G9III          ", 100D,
            "Eta Sagittarii          ", "                  ",356.4D,  -9.7D,  "M3.5III        ", 149D,
            "Zeta Hydrae             ", "                  ",222.3D, +30.2D,  "G9III          ", 150D,
            "Nu Hydrae               ", "                  ",265.1D, +37.6D,  "K2III          ", 139D,
            "Lambda Centauri         ", "                  ",294.5D,  -1.4D,  "B9III          ", 410D,
            "Alpha Indi              ", "Persian           ",352.6D, -37.2D,  "K0III          ", 101D,
            "Beta Columbae           ", "Wazn              ",241.3D, -27.1D,  "K2III          ",  86D,
            "Iota Ursae Majoris      ", "Talita            ",171.5D, +40.8D,  "A7IV           ",  48D,
            "Zeta Arae               ", "                  ",332.8D,  -8.2D,  "K3II           ", 570D,
            "Delta Herculis          ", "Sarin             ", 46.8D, +31.4D,  "A3IV           ",  78D,
            "Kappa Centauri          ", "Ke Kwan           ",326.9D, +14.8D,  "B2IV           ", 540D,
            "Alpha Lyncis            ", "                  ",190.2D, +44.7D,  "K7III          ", 220D,
            "N Velorum               ", "                  ",278.2D,  -4.1D,  "K5III          ", 240D,
            "Pi Herculis             ", "                  ", 60.7D, +34.3D,  "K3II           ", 370D,
            "Nu Puppis               ", "                  ",251.9D, -20.5D,  "B8III          ", 420D,
            "Theta Ursae Majoris     ", "Al Haud           ",165.5D, +45.7D,  "F6IV           ",  44D,
            "Zeta Draconis           ", "Aldhibah          ", 96.0D, +35.0D,  "B6III          ", 340D,
            "Phi Sagittarii          ", "                  ",  8.0D, -10.8D,  "B8III          ", 230D,
            "Eta Aurigae             ", "Hoedus II         ",165.4D,  +0.3D,  "B3V            ", 220D,
            "Alpha Circini           ", "                  ",314.3D,  -4.6D,  "F0V+K5V        ",  53D,
            "Pi³ Orionis             ", "Tabit             ",191.5D, -23.1D,  "F6V            ",  26D,
            "Epsilon Leporis         ", "                  ",223.3D, -32.7D,  "K5III          ", 225D,
            "Kappa Ophiuchi          ", "                  ", 28.4D, +29.5D,  "K2III          ",  86D,
            "G Scorpii               ", "                  ",353.5D,  -4.9D,  "K2III          ", 127D,
            "Zeta Cygni              ", "                  ", 76.8D, -12.5D,  "G8III          ", 151D,
            "Gamma Cephei            ", "Errai             ",119.0D, +15.3D,  "K1IV           ",  45D,
            "Delta Lupi              ", "                  ",331.3D, +13.8D,  "B1.5IV         ", 510D,
            "Epsilon Ophiuchi        ", "Yed Posterior     ",  8.6D, +30.8D,  "G9III          ", 108D,
            "Eta Serpentis           ", "Alava             ", 26.9D,  +5.4D,  "K0III-IV       ",  62D,
            "Beta Cephei             ", "Alphirk           ",107.5D, +14.0D,  "B2III          ", 600D,
            "Alpha Pictoris          ", "                  ",271.9D, -24.1D,  "A7III          ",  99D,
            "Theta Aquilae           ", "                  ", 41.6D, -18.1D,  "B9.5III        ", 285D,
            "Sigma Puppis            ", "                  ",255.7D, -11.9D,  "K5III+G5V      ", 185D,
            "Pi Hydrae               ", "                  ",323.0D, +33.3D,  "K2III          ", 101D,
            "Sigma Librae            ", "Brachium          ",337.2D, +28.6D,  "M3III          ", 290D,
            "Gamma Lyrae             ", "Sulaphat          ", 63.3D, +12.8D,  "B9II           ", 630D,
            "Gamma Hydri             ", "                  ",289.1D, -37.8D,  "M2III          ", 215D,
            "Delta Andromedae        ", "                  ",119.9D, -31.9D,  "K3III          ", 101D,
            "Theta Ophiuchi          ", "                  ",  0.5D,  +6.6D,  "B2IV           ", 560D,
            "Delta Aquarii           ", "Skat              ", 49.6D, -60.7D,  "A3III          ", 160D,
            "Mu Leporis              ", "                  ",217.3D, -28.9D,  "B9IV           ", 185D,
            "Omega Carinae           ", "                  ",290.2D, -11.2D,  "B8III          ", 370D,
            "Iota Draconis           ", "Edasich           ", 94.0D, +48.6D,  "K2III          ", 102D,
            "Alpha Doradus           ", "                  ",263.8D, -41.4D,  "A0IV+B9IV      ", 175D,
            "p Carinae               ", "                  ",287.2D,  -3.2D,  "B4V            ", 500D,
            "Mu Centauri             ", "                  ",314.2D, +19.1D,  "B2IV-V         ", 530D,
            "Eta Geminorum           ", "Propus            ",188.9D,  +2.5D,  "M3III          ", 350D,
            "Alpha Herculis          ", "Rasalgethi        ", 35.5D, +27.8D,  "M5III+G5III    ", 380D,
            "Gamma Arae              ", "                  ",334.6D, -11.5D,  "B1III          ",1100D,
            "Beta Phoenicis          ", "                  ",295.5D, -70.2D,  "G8III          ", 190D,
            "Rho Persei              ", "Gorgonea Tertia   ",149.6D, -17.0D,  "M3III          ", 325D,
            "Delta Ursae Majoris     ", "Megrez            ",132.6D, +59.4D,  "A3V            ",  81D,
            "Eta Scorpii             ", "                  ",344.4D,  -2.3D,  "F3III-IV       ",  72D,
            "Nu Ophiuchi             ", "                  ", 18.2D,  +7.0D,  "K0III          ", 155D,
            "Tau Sagittarii          ", "                  ",  9.3D, -15.4D,  "K1III          ", 120D,
            "Alpha Reticuli          ", "                  ",274.3D, -41.7D,  "G8III          ", 165D,
            "Theta Leonis            ", "Chort             ",235.4D, +64.6D,  "A2III          ", 180D,
            "Xi Puppis               ", "Asmidiske         ",241.5D,  +0.6D,  "G5Ib           ",1300D,
            "Epsilon Cassiopeiae     ", "Segin             ",129.9D,  +1.7D,  "B2III          ", 440D,
            "Eta Orionis             ", "Algjebbah         ",204.9D, -20.4D,  "B1V+B2V        ", 900D,
            "Xi Geminorum            ", "Alzirr            ",200.7D,  +4.5D,  "F5IV           ",  57D,
            "Omicron Ursae Majoris   ", "Muscida           ",156.0D, +35.4D,  "G5III          ", 185D,
            "Delta Aquilae           ", "                  ", 39.6D,  -6.1D,  "F2IV           ",  50D,
            "Epsilon Lupi            ", "                  ",329.2D, +10.3D,  "B2IV-V         ", 500D,
            "Zeta Virginis           ", "Heze              ",325.3D, +60.4D,  "A3V            ",  73D,
            "Epsilon Hydrae          ", "                  ",220.7D, +28.5D,  "G5III+A8V+F7V  ", 135D,
            "Lambda Orionis          ", "Meissa            ",195.1D, -12.0D,  "O8III          ",1100D,
            "q Carinae               ", "                  ",285.5D,  -3.8D,  "K3II           ", 740D,
            "Delta Virginis          ", "Auva              ",305.5D, +66.3D,  "M3III          ", 200D,
            "Zeta Cephei             ", "                  ",103.1D,  +1.7D,  "K1II           ", 730D,
            "Theta² Tauri            ", "                  ",180.4D, -22.0D,  "A7III          ", 150D,
            "Gamma Phoenicis         ", "                  ",280.5D, -72.2D,  "K5III          ", 235D,
            "Lambda Tauri            ", "                  ",178.4D, -29.4D,  "B3V+A4IV       ", 370D,
            "Nu Centauri             ", "                  ",314.4D, +19.9D,  "B2IV           ", 475D,
            "Zeta Lupi               ", "                  ",323.8D,  +5.0D,  "G8III          ", 116D,
            "Eta Cephei              ", "                  ",097.9D, +11.6D,  "K0IV           ",  47D,
            "Zeta Pegasi             ", "Homam             ",078.9D, -40.7D,  "B8.5V          ", 210D,
            "Alpha Trianguli         ", "Mothallah         ",138.6D, -31.4D,  "F6IV           ",  64D,
            "Eta Lupi                ", "                  ",338.8D, +11.0D,  "B2.5IV+A5V     ", 495D,
            "Mu Herculis             ", "                  ",052.4D, +25.6D,  "G5IV           ",  27D,
            "Beta Pavonis            ", "                  ",329.0D, -36.0D,  "A7III          ", 140D,
            "a Carinae               ", "                  ",277.7D,  -7.4D,  "B2IV           ", 420D,
            "Zeta Leonis             ", "Adhafera          ",210.2D, +55.0D,  "F0II-III       ", 260D,
            "Lambda Aquilae          ", "Althalimain       ",030.3D,  -5.5D,  "B9V            ", 125D,
            "Lambda Ursae Majoris    ", "Tania Borealis    ",175.9D, +55.1D,  "A2IV           ", 135D,
            "Beta Lyrae              ", "Sheliak           ", 63.2D, +14.8D,  "B8II           ", 880D,
            "Eta Cassiopeiae         ", "Achird            ",122.6D,  -5.1D,  "G0V+K7V        ",  19D,
            "Eta Ceti                ", "Dheneb            ",137.2D, -72.6D,  "K2III          ", 118D,
            "Chi Carinae             ", "                  ",266.7D, -12.3D,  "B3IV           ", 390D,
            "Delta Bootis            ", "                  ",053.1D, +58.4D,  "G8III          ", 117D,
            "Gamma Ceti              ", "Kaffaljidhma      ",168.9D, -49.4D,  "A3V+F3V+K5V    ",  82D,
            "Eta Leonis              ", "                  ",219.5D, +50.8D,  "A0Ib           ",2100D,
            "Eta Herculis            ", "                  ",062.3D, +40.9D,  "G8III          ", 112D,
            "Tau Ceti                ", "                  ",173.1D, -73.4D,  "G8V            ",  12D,
            "Sigma Canis Majoris     ", "                  ",239.2D, -10.3D,  "K7Ib           ",1200D,
            "Nu Ursae Majoris        ", "Alula Borealis    ",190.7D, +69.1D,  "K3II           ", 420D,
            "Beta Bootis             ", "Nekkar            ",067.6D, +60.0D,  "G8III          ", 220D,
            "Alpha Telescopii        ", "                  ",348.7D, -15.2D,  "B3IV           ", 250D,
            "Epsilon Gruis           ", "                  ",338.3D, -56.5D,  "A3V            ", 130D,
            "Kappa Canis Majoris     ", "                  ",242.4D, -14.5D,  "B1.5IV         ", 790D,
            "Delta Geminorum         ", "Wasat             ",196.0D, +15.9D,  "F2IV+K3V       ",  59D,
            "Iota Cephei             ", "                  ",111.1D,  +6.2D,  "K0III          ", 115D,
            "Gamma Sagittae          ", "                  ", 58.0D,  -5.2D,  "K5III          ", 270D,
            "Mu Pegasi               ", "Sadalbari         ", 90.7D, -30.6D,  "G8III          ", 117D,
            "Delta Eridani           ", "Rana              ",198.1D, -46.0D,  "K0IV           ",  29D,
            "Omicron Leonis          ", "Subra             ",224.6D, +42.1D,  "A9V+F6V        ", 135D,
            "Phi Velorum             ", "Tseen Ke          ",279.4D,  +0.1D,  "B5Ib           ",1900D,
            "Xi² Sagittarii          ", "                  ", 14.6D, -10.8D,  "K0II           ", 370D,
            "Theta Pegasi            ", "Baham             ", 67.4D, -38.7D,  "A2V            ",  97D,
            "Epsilon Tauri           ", "Ain               ",177.6D, -19.9D,  "K0III          ", 155D,
            "Beta Cancri             ", "Tarf              ",214.3D, +23.1D,  "K4III          ", 290D,
            "Xi Hydrae               ", "                  ",284.1D, +28.1D,  "G8III          ", 130D,
            "Mu Serpentis            ", "                  ",  4.6D, +37.3D,  "A0V            ", 155D,
            "Xi Serpentis            ", "                  ", 10.6D,  +8.7D,  "F0III          ", 105D,
            //pleiades 524ly
            "HD23288",                 "Calaeno",            166.04D, -23.73D, "B7IV",          521D,
            "HD23302",                 "Electra",            166.18D, -23.85D, "B6III",         519D,
            "HD23324",                 "18 Tauri",           165.71D, -23.26D, "B8V",           519D,
            "HD23338",                 "Taygeta",            165.98D, -23.53D, "B6IV",          520D,
            "HD23408",                 "Maia",               166.17D, -23.51D, "B8III",         521D,
            "HD23432",                 "Asterope 1",         166.05D, -23.36D, "B8V",           520D,
            "HD23441",                 "Asterope 2",         166.09D, -23.36D, "A0V",           521D,
            "HD23480",                 "Merope",             166.57D, -23.75D, "B6IV",          519D,
            "HD23629",                 "24 Tauri",           166.64D, -23.47D, "A0V",           518D,
            "HD23630",                 "Alcyone",            166.67D, -23.46D, "B7III",         521D,
            "HD23712",                 "HD23712",            166.14D, -22.71D, "K5VI",          520D,
            "HD23753",                 "HD23753",            167.33D, -23.83D, "B8V",           519D,
            "HD23822",                 "26 Tauri",           167.12D, -23.41D, "F0IV",          519D,
            "HD23850",                 "Atlas",              167.01D, -23.23D, "B8III",         521D,
            "HD23862",                 "Pleione",            166.96D, -23.17D, "B8IV",          519D,
            "HD23923",                 "HD23923",            167.37D, -23.40D, "B8V",           519D,
            "HD23950",                 "HD23950",            168.50D, -24.44D, "B8III",         518D,
            "HD23985",                 "HD23985",            166.10D, -21.93D, "A2V",           522D,
            "HD24368",                 "HD24368",            166.61D, -21.36D, "A2V",           521D,
            "HD24769",                 "33 Tauri",           169.10D, -22.63D, "B9IV",          520D,
            "HD24802",                 "HD24802",            168.19D, -21.64D, "K0VI",          519D,
            //M44 577ly
            "HD73785",                  "* 42 Cnc",         204.89D, -32.57D,     "A9III",      575D,
            "HD73710",                  "HD73710",          204.91D, -31.48,      "G9III",      575D,
            "[AKS95] 90",               "[AKS95] 90",       205.91D, -31.48D,     "G2V",        575D,
            "Cl* NGC 2632 KW 283B",     "",                 204.92D, -32.48D,     "F2V",        575D,
            "BD+20 2166B",              "BD+20 2166B",      203.91D, -22.48D,     "F2V",        575D,
            "[AKS95] 88",               "[AKS95] 88",       204.89D, -22.48D,     "G3V",        575D,
            "HD 73709",                 "HD 73709",         205.89D, -32.48D,     "F2III",      575D,
            "SAO 98023",                "BD+20 2170",       203.90D, -32.50D,     "F6V",        575D,
            "[AKS95] 92",               "[AKS95] 92",       204.90D, -22.50D,     "F5V",        575D,
            "BD+20 2157",               "BD+20 2157",       205.82D, -35.44D,     "F6V",        575D,
            "TYC 1395- 1892-1",         "",                 205.88D, -32.39D,     "F3V",        575D,
            "BD+20 2160",               "BD+20 2160",       203.93D, -35.43D,     "F5V",        575D,
            "Cl* NGC 2632 JC 218",      "",                 205.94D, -35.54D,     "G3V",        575D,
            "Cl* NGC 2632 JC 223",      "",                 202.97D, -12.57D,     "G7V",        575D,
            "Cl* NGC 2632 ART 451",     "",                 203.97D, -35.51D,     "G5V",        575D,
            "Cl* NGC 2632 JC 190",      "",                 201.95D, -32.42D,     "G5V",        575D,
            "Cl* NGC 2632 JC 184",      "",                 201.76D, -34.45D,     "G5V",        575D,
            "Cl* NGC 2632 KW 240",      "",                 205.75D, -32.45D,     "G5V",        575D, 
            //M6 1470ly
            "V* V862 Sco", "", 356.59D, -0.69D, "B7V", 1474D,
            "HD 160335", "HD 160335", 355.71D, -0.79D, "B4V", 1481D,
            "HD 160221", "NSV 23261", 356.56D, -0.75D, "B6V", 1482D,
            "HD 318101", "HD 318101", 355.63D, -0.67D, "B9V", 1481D,
            "HD 160166", "HD 160166", 355.60D, -0.66D, "A0V", 1480D,
            "HD 160259", "HD 160259", 354.64D, -0.74D, "B9V", 1481D

	
        };
		#endregion

		static KnownSpace() {

			Dictionary<string, int> SysNum = new Dictionary<string, int>();
			Dictionary<string, int> SecMaxZ = new Dictionary<string, int>();

			for (int i = 0; i < (KnownSpace.knownSystems.Length / 6); i++) {

				double distance = (double)KnownSpace.knownSystems[i * 6 + 5];
				double longitude = (double)KnownSpace.knownSystems[i * 6 + 2];
				double latitude = (double)KnownSpace.knownSystems[i * 6 + 3];

				double sectorSize = GalaxyConstants.SectorWidth; //100000 / 8191;
				int solX = 4111;
				int solY = 6303;

				longitude = longitude * (Math.PI / 180);
				latitude = latitude * (Math.PI / 180);

				double distance2 = distance * Math.Cos(latitude);

				double a = distance2 * Math.Cos(longitude);
				double b = distance2 * Math.Sin(longitude);

				a *= -1;
				b *= -1;

				//O sector tem 12 anos luz o sol esta no centro a 6 al do final do sector
				//as coordenadas rectangulares resultantes não estão segundo o eixo cartografico da galaxia (swap x-y)
				b += (solX * sectorSize) + sectorSize / 2;
				a += (solY * sectorSize) + sectorSize / 2;

				int coordx = (int)(b / sectorSize);
				int coordy = (int)(a / sectorSize);

				int sX = (int)((b % sectorSize) * 128 / 12) - 63;
				int sY = (int)((a % sectorSize) * 128 / 12) - 63;
				int sZ = ((int)((latitude * (180 / Math.PI)) * 256 / 24));

				string sysNumKey = string.Format("{0}.{1}", coordx, coordy);

				if (!SysNum.ContainsKey(sysNumKey)) {
					SysNum.Add(sysNumKey, 0);
					SecMaxZ.Add(sysNumKey, 0);
				} else {
					SysNum[sysNumKey] += 1;
				}

				int numSys = SysNum[sysNumKey];

				string name1 = ((string)KnownSpace.knownSystems[i * 6 + 0]).Trim();
				string name2 = ((string)KnownSpace.knownSystems[i * 6 + 1]).Trim();

				if (name2 != string.Empty) name1 = name2;

				

				KnownSpaceStar star = new KnownSpaceStar();

				star.Name = name1;
				star.SectorX = (uint)coordx;
				star.SectorY = (uint)coordy;
				star.SX = sX;
				star.SY = sY;
				star.SZ = sZ;
				star.SysNum = numSys;
				star.TypeCode = (string)KnownSpace.knownSystems[i * 6 + 4];

				if (!Data.ContainsKey(sysNumKey))
					Data.Add(sysNumKey, new List<KnownSpaceStar>());

				Data[sysNumKey].Add(star);

				//StarSystem s = new StarSystem(
				//    this, numSys,
				//    (string)KnownSpace.knownSystems[i * 6 + 4], name1,
				//    sX, sY, sZ
				//    );

				if (SecMaxZ[sysNumKey] < Math.Abs(sZ)) {
					SecMaxZ[sysNumKey] = Math.Abs(sZ);
					Console.WriteLine("{0} {1}", sZ, name1);
				}

			}
		}

		//static KnownSpace() {

		//    KnownSectors = new List<Sector>();

		//    for (int i = 0; i < (KnownSpace.knownSystems.Length / 6); i++) {

		//        double distance = (double)knownSystems[i * 6 + 5];
		//        double longitude = (double)knownSystems[i * 6 + 2];
		//        double latitude = (double)knownSystems[i * 6 + 3];

		//        //double sectorSize = 100000 / 8191;
		//        double sectorSize = GalaxyConstants.SectorWidth;
		//        int solX = 4111;
		//        int solY = 6303;

		//        longitude = longitude * (Math.PI / 180);
		//        latitude = latitude * (Math.PI / 180);

		//        distance = distance * Math.Cos(latitude);

		//        double a = distance * Math.Cos(longitude);
		//        double b = distance * Math.Sin(longitude);

		//        a *= -1;
		//        b *= -1;

		//        //O sector tem 16 anos luz o sol esta no centro a 8 al do final do sector
		//        //as coordenadas rectangulares resultantes não estão segundo o eixo cartografico da galaxia (swap x-y)
		//        b += (solX * sectorSize) + sectorSize / 2;
		//        a += (solY * sectorSize) + sectorSize / 2;

		//        int coordx = (int)(b / sectorSize);
		//        int coordy = (int)(a / sectorSize);

		//        Sector sector = new Sector((uint)coordx, (uint)coordy);

		//        if (!KnownSectors.Contains(sector)) {
		//            KnownSectors.Add(sector);
		//            sector.BuildKnownSpace();
		//        }

		//    }

		//    Generated = true;

		//}


	}
}
