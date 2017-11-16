using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace GalaxyE.Galaxy {

	#region System Enums

	public enum StarSpectralClass {
		O = 0,
		B = 1,
		A = 2,
		F = 3,
		G = 4,
		K = 5,
		M = 6
	}

	public enum StarLuminosityClass {
		_0 = -1,
		Ia = 0,
		Ib = 1,
		II = 2,
		III = 3,
		IV = 4,
		V = 5,
		VI = 6,
		VII = 7
	}

	public enum StarAgeClass {
		MainSequence = 0,
		MainEndSequence = 1,
		Giant = 2,
		Dead = 3
	}

	[Flags]
	public enum SystemComponent {
		none			= 0,
		jovian			= 1,
		terrestrial		= 2,
		ocean			= 4,
		ice				= 8,
		rocky			= 16,
		asteroid		= 32,
		inferno			= 64,
		nature			= terrestrial | ocean,
		minning			= asteroid | rocky | ice | inferno,
		industrial		= rocky | ice | terrestrial
	}

	#endregion

	[Serializable]
	public class StarSystem : SectorObject, ISerializable, IExtendableObject {

		#region Static Arrays (Generator Parameters)
		//Ia = 0, Ib = 1, II = 2, III = 3, IV = 4, V = 5, VI = 6, VII = 7
		private float[][] StarSize = new float[][] {
            new float[] { 25, 25, 17, 15, 12, 10, 9, 4 }, //O
            new float[] { 22, 22, 15, 12, 10,  8, 5, 3 }, //B
            new float[] { 30, 20, 12,  9,  6,  5, 4, 2 }, //A
            new float[] { 35, 30, 15,  9,  7,  4, 3, 1 }, //F
            new float[] { 40, 35, 15, 10,  8,  1, 0.8F, 0.6F }, //G
            new float[] { 45, 40, 20, 15,  9,  1, 0.6F, 0.4F }, //K
            new float[] { 50, 42, 30, 20, 10,  1, 0.4F, 0.2F }  //M
        };

		//Ia = 0, Ib = 1, II = 2, III = 3, IV = 4, V = 5, VI = 6, VII = 7
		private static int[][] StarMagnitude = new int[][] {
            new int[] { 0, 0, 1, 2, 3, 3, 5, 8 }, //O
            new int[] { 0, 0, 1, 2, 3, 3, 5, 8 }, //B
            new int[] { 0, 0, 1, 3, 3, 3, 6, 8 }, //A
            new int[] { 0, 0, 2, 3, 4, 4, 7, 8 }, //F
            new int[] { 0, 1, 2, 4, 5, 5, 7, 8 }, //G
            new int[] { 0, 1, 3, 4, 6, 7, 8, 9 }, //K
            new int[] { 1, 2, 3, 5, 8, 7, 9, 9 }  //M
        };

		//Ia = 0, Ib = 1, II = 2, III = 3, IV = 4, V = 5, VI = 6, VII = 7
		private float[][] StarMass = new float[][] {
            new float[] { 80, 70, 45,   40,   50,    60,    20,     10 }, //O
            new float[] { 30, 25, 20,    7,    6,  5.9F,     4,      2 }, //B
            new float[] { 18, 16, 10,    4,    3,     2,     1,      1 }, //A
            new float[] { 14, 12,  9,    2, 1.8F,  1.6F,  0.8F,   0.5F }, //F
            new float[] { 12, 10,  9,    1,    1,     1,  0.5F,  0.09F }, //G
            new float[] { 15, 13, 10, 1.2F, 1.1F,  0.7F,  0.1F,  0.05F }, //K
            new float[] { 17, 13, 25, 1.2F, 1.1F,  0.2F, 0.01F, 0.001F }  //M
        };

		private static int[] StarBaseAge = new int[] {
            10,         // main sequence at O 
            100,        // main sequence at B
            1000,       // main sequence at A
            5000,       // main sequence at F
            10000,      // main sequence at G
            50000,      // main sequence at K
            100000      // main sequence at M
        };

		// O = 0, B = 1, A = 2, F = 3, G = 4, K = 5, M = 6
		private static int[][] AgeSpectralEvolution = new int[][] {
            new int[] { 0, 0, 1, 2 }, //O as main sequence Spectral Type
            new int[] { 1, 1, 2, 2 }, //B as main sequence Spectral Type
            new int[] { 2, 3, 4, 2 }, //A as main sequence Spectral Type
            new int[] { 3, 4, 5, 2 }, //F as main sequence Spectral Type
            new int[] { 4, 6, 5, 2 }, //G as main sequence Spectral Type
            new int[] { 5, 5, 6, 2 }, //K as main sequence Spectral Type
            new int[] { 6, 6, 6, 2 }  //M as main sequence Spectral Type
        };

		// Ia = 0, Ib = 1, II = 2, III = 3, IV = 4, V = 5, VI = 6, VII = 7
		private static int[][] AgeLuminosityEvolution = new int[][] {
            new int[] { 5, 1, 0, 7 }, //O as main sequence Spectral Type
            new int[] { 5, 2, 0, 7 }, //B as main sequence Spectral Type
            new int[] { 5, 2, 1, 7 }, //A as main sequence Spectral Type
            new int[] { 5, 3, 1, 7 }, //F as main sequence Spectral Type
            new int[] { 5, 3, 2, 7 }, //G as main sequence Spectral Type
            new int[] { 5, 4, 2, 7 }, //K as main sequence Spectral Type
            new int[] { 5, 5, 3, 7 }, //M as main sequence Spectral Type
        };

		//Ia = 0, Ib = 1, II = 2, III = 3, IV = 4, V = 5, VI = 6, VII = 7
		private static float[][] StarHz = new float[][] {
            new float[] { 3500, 3300, 2000, 1000, 800, 700,  100,   0.5F },  //O
            new float[] { 1800, 1600, 900,  400,  300, 200,  50,    0.45F }, //B
            new float[] { 200,  190,  95,   11,   9,   7,    4,     0.4F },  //A
            new float[] { 180,  170,  85,   8,    5,   2,    1.1F,  0.3F },  //F
            new float[] { 170,  160,  80,   6,    3,   1,    0.6F,  0.25F }, //G
            new float[] { 170,  160,  80,   15,   10,  0.5F, 0.1F,  0.05F }, //K
            new float[] { 200,  190,  100,  30,   12,  0.1F, 0.1F,  0.05F }  //M
        };

		private static int[] AgeChance = new int[] {
            0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 1, 2, 2, 3,
        };

		// O = 0 0.00001%, B = 1 0,05%, A = 2 0.3%, F = 3 1.5%, G = 4 4%, K = 5 9%, M = 6 80%
		private static int[] BaseSpectralClassChance = new int[] {
            6, 6, 6, 6, 6, 6, 6, 6,
            6, 6, 6, 6, 6, 6, 5, 5,
            5, 5, 5, 5, 5, 5, 4, 4,
            4, 4, 3, 3, 3, 2, 1, 0,
        };
		#endregion

		#region StarChance
		/*
            0, 0, 0, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1,
            2, 2, 2, 2, 2, 3, 3, 3,
            3, 4, 4, 4, 5, 6, 7, 8 */

		static int[] StarChance_Type = new int[] {
            2, 2, 2, 2, 2, 2, 3, 3,
            3, 3, 3, 3, 3, 4, 4, 4,
            4, 5, 5, 5, 6, 6, 7, 7,
            8, 9, 9, 10, 11, 12, 13, 14,

            0, 0, 0, 0, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 2, 2, 2,


        };


		static int[] StarChanceA_Type = new int[] {
            0, 0, 0, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 14, 1, 1,
            1, 2, 2, 2, 2, 2, 2, 3,
            3, 3, 3, 3, 4, 4, 4, 5

        };

		static int[] StarChanceB_Type = new int[] {
            0, 1, 12, 1, 1, 2, 2, 2,
            1, 1, 3, 3, 3, 14, 1, 1,
            2, 3, 8, 12, 8, 9, 9, 9,
            9, 10, 10, 11, 11, 13, 13, 13


        };

		static int[] StarChanceC_Type = new int[] {
            0, 11, 1, 1, 1, 1, 1, 1,
            1, 1, 2, 2, 14, 2, 3, 3,
            3, 3, 4, 4, 4, 4, 5, 5,
            5, 6, 6, 6, 7, 7, 11, 11,
        };

		static int[] StarChanceD_Type = new int[] {
            0, 11, 1, 1, 1, 1, 1, 1,
            1, 1, 2, 2, 14, 14, 3, 3,
            3, 3, 4, 4, 4, 4, 5, 5,
            5, 6, 6, 6, 7, 7, 8, 9,
        };

		static int[] StarChance_Multiples = new int[] {
            0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 1, 1, 1, 1, 1,
            1, 1, 2, 2, 2, 3, 4, 5
        };
		#endregion

		#region Description Names
		string[] govnames = new string[] {
			"Anarchy","Feudal","Multi-gov","Dictatorship",
            "Communist","Confederacy","Democracy","Corporate State", "None"
		};

		string[] econnames = new string[] {
			"Rich Ind","Average Ind","Poor Ind","Mainly Ind",
			"Mainly Agri","Rich Agri","Average Agri","Poor Agri", "None"
		};

		string[] unitnames = new string[] {"t","kg","g"};
		#endregion

		#region Names Table


		/**********************************************************************************
            O 	bluish-violet 	Iota & Zeta (Alnitak) Orionis Aa
            B 	blue 	        Algol A, Regulus Aa
            A 	cyan 	        Sirius A, Vega
            F 	pale yellow 	Procyon A, Eta Cassiopeia A (F9-G0)
            G 	yellow 	        Sun, Alpha Centauri A
            K 	orange 	        Alpha Centauri B, Epsilon Eridani
            M 	red 	        Proxima Centauri, Barnard's Star
         **********************************************************************************
            0 	    Hypergiants 	        V810 or Omicron1 Centauri (?)
            Ia,b 	Supergiants 	        Antares Aa, Canopus
            IIa,b 	Bright Giants 	        Dubhe A, Tarazed
            IIIa,b 	Giants 	                Aldebaran Aa, Arcturus
            IVa,b 	Subgiants 	            Procyon A, Beta Hydri
            Va,b 	Main Sequence Dwarfs 	Sol, Sirius A
            VI/sd 	Subdwarfs 	            Kapteyn's Star
            VII/D 	"White" Dwarfs 	        Procyon B, Sirius B
         **********************************************************************************/

		static string[] StarLuminosity = new string[] {
            "Ia",	// 0 Very luminous supergiants
            "Ib",	// 1 Less luminous supergiants
            "II",	// 2 Luminous giants
            "III",	// 3 Giants
            "IV",	// 4 Subgiants
            "V",    // 5 Main sequence stars (dwarf stars)
            "VI",	// 6 Subdwarf
            "VII"	// 7 White Dwarf
        };

		static char[] StarTypeChar = new char[] {
            'O', 'B', 'A', 'F', 'G', 'K', 'M'
        };

		//              'O',  'B',  'A',  'F',  'G',  'K',  'M'
		static int[][] TypeTable = new int[][] {                            
            new int[] { 15,   15,   15,   15,   10,   12,   13 },     //Ia    
            new int[] { 15,   15,   15,   15,   10,   12,   13 },     //Ib    
            new int[] { 11,   11,   11,   10,   10,   9,    8  },     //II    
            new int[] { 11,   11,   11,   10,   10,   9,    8  },     //III   
            new int[] { 11,   11,   11,   10,   10,   9,    8  },     //IV    
            new int[] { 7,    6,    5,    4,    3,    2,    1  },     //V     
            new int[] { 7,    6,    5,    4,    3,    2,    1  },     //VI    
            new int[] { 15,   15,   15,   15,   15,   15,   15 }      //VII   
        };

		static string[] TypeDescSpectralClass = new string[] {
			"Blue",
 			"Blue",
			"Bright Blue",
			"Bright Yellow",
			"Yellow",
			"Orange",
			"Red"
		};

		static string[] TypeDescLuminosityClass = new string[] {
			"Hypergiant",
 			"Supergiant",
			"Hot Giant",
			"Giant",
			"Sub Giant",
			"Main Sequence",
			"Faint Drawf",
			"White Drawf"
		};

		string[] namePartStart_ = new string[] {"Ca","Arc","Ve","Ri","Pro","A","Be","Ha","Al","An","Pol","Fo","De","Mi","Re","Ad","Cas","Ga","El","Mir","Dub","We","Ka",
			"Sar","Men","Pea","Koo","She","Po","Di","Nun","Sai","Ko","Dha","Ra","Cih","Muh","Na","As","Su","Sa","Sche","Min","Wei","Me","I","E","Gir","Phe","Mar","Gi","Han",
			"Zos","Gra","Ar","Gie","Ghu","Zu","U","Pha","Kr","Ru","Mu","Ke","Has","Le","Ta","Pri","Por","Ce","Kur","Kor","Ras","Cor","Ni","Bo","Tu","Vin","He","Hy","Te",
			"Go","Ma","Sad","Zau","Ok","Pher","Phu","Se","Da","Meb","Aus","Per","Wa","Kw","Hoe","Er","Yed","Pos","Bra","Sk","Ed","Gor","Ter","Cho","Mus","Au","Ho","Mo","Dhe",
			"Kaf","Nek","Tse","Ba","Ain","Tau","Tay","At","Ple","Cen","Boö","Ly","O","Cru","Scor","Vir","Ge","Pis","Cy","Leo","Ep","Lam","Gam","Gru","Ze","Ur","The","Tri","Pa",
			"Oph","Pup","Io","Co","Dra","Lu","Pe","Pho","Kap","Li","Up","Pi","Eri","Her","Rho","Psi","In","Pic","Phoe","Omi","Can"
		};

		string[] namePartEnd_ = new string[] {"i","ri","us","no","pus","tu","rus","ga","pe","la","gel","cy","on","ch","er","nar","tel","ge","u","se","dar","ta","ir","cr","ux","de",
			"ba","ran","res","pi","ca","lux","ma","lha","ut","neb","mo","sa","gu","lus","ha","ra","tor","au","na","th","a","pla","ci","dus","ni","lam","tak","li","oth","fak","he",
			"gor","zen","ka","id","gas","vi","or","nan","tri","lhe","co","ck","zam","ris","gie","mal","ki","ke","nt","phe","tz","ph","ab","nab","sal","gue","gol","ne","bo","fa","in",
			"os","dis","il","zar","dr","nin","rak","nif","tab","kaa","bik","at","lu","dra","min","keb","e","nah","kab","kar","fia","rab","cel","la","be","nes","cha","nu","kal","hai",
			"tan","nel","bi","ct","az","bah","rid","uan","eh","dio","lis","zed","ed","dhi","ty","bal","rai","pho","ros","ban","ro","hal","ti","cus","re","nib","is","one","mia","trix",
			"di","ad","jat","mar","dah","mei","yat","reo","dal","su","ud","tar","go","rel","al","mel","ik","sed","sl","kad","jor","rad","gi","nus","bih","tra","si","an","zn","rin",
			"bit","te","va","phi","rk","chi","um","as","ich","thi","gon","ea","tia","grez","rt","mi","gin","gjeb","zi","da","ze","mam","thal","lah","fe","tha","nia","rea","ak","rd",
			"fal","jid","hma","sat","bra","en","ham","rf","lec","ia","las","io","pha","nis","jo","tau","tis","o","qui","cis","pii","rum","gni","lon","bda","nae","sei","lo","git","rii","vo",
			"rie","dro","me","dae","ppa","iu","sio","pei","ae","pis","pa","po","br","lum","cu","num","pri","cor","dri","psi","qua","bae","to","cri"
		};



		static string[] namepart_original = new string[] {
            "en" , "la" , "can", "be" ,
            "and", "phi", "eth", "ol" ,
            "ve" , "ho" , "a"  , "lia",
            "an" , "ar" , "ur" , "mi" ,
            "in" , "ti" , "qu" , "so" ,
            "ed" , "ess", "ex" , "io" ,
            "ce" , "ze" , "fa" , "ay" ,
            "wa" , "da" , "ack", "gre"
        };

		static string[] namepart = new string[] {
            "si", "ri", "us", "ca", "no", "gi", "ra", "tu", 
            "rus", "pus", "ve", "ga", "pe", "la", "gux", "pro", 
            "cyg", "on", "a", "az", "ch", "nar", "be", "tru", 
            "ge", "se", "ha", "dar", "al", "ta", "ir", "ux", 
            "al", "de", "ba", "ran", "an", "res", "spi", "po", 
            "lux", "ux", "de", "ne", "neb", "gu", "lus", "ha", 
            "ra", "sh", "u", "be", "lla", "ix", "th", "el", 
            "la", "al", "o", "li", "fa", "ka", "ku", "is", 
            "ki", "phe", "ch", "ab", "dha", "nab", "ol", "os", 
            "ky", "zar", "mi", "ke", "suh", "zo", "ne", "ar", 
            "pah", "ct", "ssa", "ra", "ta", "zeb", "ge", "nib", 
            "lhe", "phu", "ry", "az", "ma", "dih", "da", "ta", 
            "is", "rin", "sk", "at", "pus", "ez", "gy", "sa", 
            "au", "va", "am", "ho", "fe", "chi", "she", "ak", 
            "sat", "ba", "han", "ham", "ex", "sh", "eth", "oth",
            "th", "za", "xa", "dyz", "xy", "lly", "wa", "we"
        };
		#endregion

		#region Class Identification (Fields)

		private StarAgeClass _ageClass;
		private StarSpectralClass _mainSequenceSpectralClass;
		private StarSpectralClass _spectralClass;
		private StarLuminosityClass _luminosityClass;
		private int _magnitude;
		private float _mass;
		private float _hZone;
		private string _commonName = null;

		#endregion

		#region System Identification (Fields)

		private ulong _systemId;
		private int _numOfBodies;
		private int _x, _y, _z;
		private byte _type, _multiple;
		private string _starType;
		private int _sysNum;
		private SystemBody[] _systemBodies;
		private ulong[] _systemParams;
		private bool _generated;

		#endregion

		#region Star Identication (Properties)
		public int SysNum { get { return _sysNum; } }
		public byte Type { get { return _type; } }
		public byte NumStars { get { return _multiple; } }
		public string StarType { get { return _starType; } }
		public new ulong SystemId { get { return _systemId; } }
		public bool Generated { get { return _generated; } }
		public string CommonName { 
			get {
				if (_commonName != null)
					return _commonName;
				else
					return Name;
			}
		}

		public SystemComponent Components {
			get {
				SystemComponent comp = SystemComponent.none;
				foreach (var p in this.SystemBodies) {
					switch (p.BodyType) {
						case SystemBodyType.Venuzian: comp |= SystemComponent.inferno; break;
						case SystemBodyType.Inferno: comp |= SystemComponent.inferno; break;
						case SystemBodyType.Asteroid: comp |= SystemComponent.asteroid | SystemComponent.rocky; break;
						case SystemBodyType.RockyPlanetoid: comp |= SystemComponent.rocky; break;
						case SystemBodyType.RockyWorld: comp |= SystemComponent.rocky; break;
						case SystemBodyType.SubGasGiant: comp |= SystemComponent.jovian; break;
						case SystemBodyType.GasGiant: comp |= SystemComponent.jovian; break;
						case SystemBodyType.RingedGasGiant: comp |= SystemComponent.jovian; break;
						case SystemBodyType.WaterWorld: comp |= SystemComponent.terrestrial; break;
						case SystemBodyType.Terrestrial: comp |= SystemComponent.terrestrial; break;
						case SystemBodyType.IceWorld: comp |= SystemComponent.ice; break;
					}
				}

				return comp;
			}
		}

		//private StarSystemUtils _utils = null;

		//public uint TechLevel {
		//    get {
		//        if (this._utils == null)
		//            this._utils = new StarSystemUtils(this);

		//        return _utils.Information.techLevel;
		//    }
		//}

		//public uint EconomyBase {
		//    get {
		//        if (this._utils == null)
		//            this._utils = new StarSystemUtils(this);

		//        return _utils.Information.economy;
		//    }
		//}

		//public string Description {
		//    get {

		//        StringBuilder sBuilder = new StringBuilder();

		//        if (this._utils == null)
		//            this._utils = new StarSystemUtils(this);

		//        sBuilder.AppendFormat("{0} System. Coordinates: {1}", this.Name, this.Coords);
		//        sBuilder.AppendFormat("Stable System with {0} major bodies\r\n", this.NumberOfBodies + this.NumStars);
		//        sBuilder.AppendFormat("Main Star: {2} {0} ({1})\r\n", this.StarType, this._ageClass, this.Name);

		//        sBuilder.AppendFormat("Economy: {0}\r\n", econnames[_utils.Information.economy]);
		//        sBuilder.AppendFormat("Government: {0}\r\n", govnames[_utils.Information.govType]);

		//        sBuilder.AppendFormat("Tech Level: {0}\r\n", (_utils.Information.techLevel) + 1);
		//        sBuilder.AppendFormat("Turnover: {0}\r\n", (_utils.Information.Productivity));
		//        sBuilder.AppendFormat("Population: {0} Billion\r\n", (_utils.Information.Population) >> 3);

		//        sBuilder.AppendFormat("{0}\r\n", _utils.GetDescription());

		//        return sBuilder.ToString();
		//    }
		//}

		#endregion

		#region Class Identification (Properties)
		public StarSpectralClass SpectralClass { get { return _spectralClass; } }
		public StarLuminosityClass LuminosityClass { get { return _luminosityClass; } }

		public string Desc {
			get {
				return string.Format("{0} {1} Star", TypeDescSpectralClass[(int)SpectralClass],
					TypeDescLuminosityClass[(int)LuminosityClass]);
			}
		}

		public int Magnitude { get { return _magnitude; } }
		public float Size {
			get {
				return (StarSize[(int)_spectralClass][(int)_luminosityClass]);
			}
		}

		public float Mass { get { return _mass; } }
		public float HZone { get { return _hZone; } }

		public int NumberOfBodies { get { return _numOfBodies; } }

		public SystemBody[] SystemBodies {
			get {
				if (_systemBodies == null) {
					_systemBodies = new SystemBody[_numOfBodies];
					for (int i = 0; i < _systemBodies.Length; i++)
						_systemBodies[i] = new SystemBody(this, i);
				}
				return _systemBodies;
			}
		}
		#endregion

		#region Construct system from base information (for known systems)
		public StarSystem(Sector sector, int sysNum, string typeCode, string name, int sX, int sY, int sZ)
			: base(sector, sector.Coords) {

			string[] mTypeCode = typeCode.Split('+');

			this._generated = false;
			this._multiple = (byte)mTypeCode.Length;
			this.Name = name;
			this._sysNum = sysNum;
			this._x = sX;
			this._y = sY;
			this._systemId = (((ulong)sysNum) << 26) + (((ulong)sector.IndexX) << 13) + (ulong)sector.IndexY;
			this.Sector = sector;

			//if (sZ > 128 || sZ < -128)
			//    this._z = 128 * Math.Sign(sZ);
			//else
			this._z = sZ;

			Match m = Regex.Match(mTypeCode[0], "[IVXab]+");

			if (m.Success) {

				string value = m.Groups[0].Value;
				char stChar = mTypeCode[0][0];

				for (int i = 0; i < StarTypeChar.Length; i++) {
					if (stChar == StarTypeChar[i]) {
						for (int j = 0; j < StarLuminosity.Length; j++) {
							if (value == StarLuminosity[j]) {
								this._type = (byte)TypeTable[j][i];
								this._luminosityClass = (StarLuminosityClass)j;
								this._spectralClass = (StarSpectralClass)i;
								this._mainSequenceSpectralClass = (StarSpectralClass)i;

								this._magnitude = (int)Math.Round(float.Parse(
									Regex.Replace(mTypeCode[0], "[A-Za-z\\-]+", ""),
									System.Globalization.CultureInfo.InvariantCulture
								), 0);

								this._starType = string.Format("{0}{1}{2}",
									_spectralClass,
									_magnitude,
									_luminosityClass);
								break;
							}
						}
						break;
					}
				}
			}
			BuildCoords();

			if (name != "Sol")
				CalculateNumOfBodies();
			else {
				_numOfBodies = 9;
			}

			//Sector solSector = new Sector();

			//double distance = this.Coords.CalculateDistance(solSector.Coords);

			//if (distance > 100) {

			//    _utils = new StarSystemUtils(this);

			//    StarSystemUtils.PlanSys sysInfo = new StarSystemUtils.PlanSys();
			//    sysInfo.Population = (uint)(12 << 3);
			//    sysInfo.descOverride = "System explored. No registered settlements.";
			//    sysInfo.economy = 8;
			//    sysInfo.govType = 8;
			//    sysInfo.techLevel = 0;
			//    sysInfo.Productivity = 0;

			//    _utils.Information = sysInfo;

			//} else if (distance > 150) {

			//    _utils = new StarSystemUtils(this);

			//    StarSystemUtils.PlanSys sysInfo = new StarSystemUtils.PlanSys();
			//    sysInfo.Population = (uint)(12 << 3);
			//    sysInfo.descOverride = "System unexplored.";
			//    sysInfo.economy = 8;
			//    sysInfo.govType = 8;
			//    sysInfo.techLevel = 0;
			//    sysInfo.Productivity = 0;

			//    _utils.Information = sysInfo;
			//}

			this._mass = StarMass
				[(int)_mainSequenceSpectralClass]
				[(int)_luminosityClass];

			if (_generated)
				this._magnitude = StarMagnitude[(int)_spectralClass][(int)_luminosityClass];

			this._hZone = StarHz[(int)_spectralClass][(int)_luminosityClass];

		}
		#endregion

		#region Construct System

		public static StarSystem GetStarSystem(Sector sector, int sysNum) {
			if (sysNum > 0 && sysNum < sector.Systems.Count)
				return sector.Systems[sysNum];
			return null;
		}

		public static StarSystem GetStarSystem(uint sectorX, uint sectorY, int sysNum) {
			Sector sector = new Sector(sectorX, sectorY);
			return GetStarSystem(sector, sysNum);
		}

		public static StarSystem GetStarSystem(Coords coords) {
			Sector sector = coords.GetSector();
			foreach (var sys in sector.Systems)
				if (sys.Coords == coords) return sys;
			return null;
		}

		private StarSystem(Sector sector, int sysNum)
			: base(sector, sector.Coords) {

			this.Sector = sector;
			this._sysNum = sysNum;
			this.Name = "";
			this._starType = "";
			this._systemId = (((ulong)sysNum) << 26) + (((ulong)sector.IndexX) << 13) + (ulong)sector.IndexY;
			this._systemParams = sector.GetSystemParams(sysNum);
			this.Generate(_systemParams[0], _systemParams[1]);
			BuildCoords();
			CalculateNumOfBodies();

			Sector solSector = new Galaxy.Sector(); //Builds Sol Sector
			double distance = this.Coords.CalculateDistance(solSector.Coords);

			

		}
		#endregion

		#region Construct Generated System
		internal StarSystem(Sector sector, int sysNum, ulong SystemParam_0, ulong SystemParam_1)
			: base(sector, sector.Coords) {

			this.Sector = sector;
			this._sysNum = sysNum;
			this.Name = "";
			this._starType = "";
			this._systemParams = new ulong[] { SystemParam_0, SystemParam_1 };
			this._systemId = (((ulong)sysNum) << 26) + (((ulong)sector.IndexX) << 13) + (ulong)sector.IndexY;
			this.Generate(SystemParam_0, SystemParam_1);
			BuildCoords();
			CalculateNumOfBodies();
		}
		#endregion

		#region BuildCoords
		private void BuildCoords() {
			Coords = new Coords(Sector.IndexX, Sector.IndexY, this._x, this._y, this._z);
		}
		#endregion

		#region CorrectZ Hack for known Systems
		internal void CorrectZ(int range) {
			return;

			//_z = _z * 128 / range;
			//BuildCoords();
		}
		#endregion

		#region Generate System
		private void Generate(ulong SystemParam_0, ulong SystemParam_1) {
			this._generated = true;
			this._z = (((int)((SystemParam_0 & 0xFFFF0000) >> 16) & 0x3FF) - 0x1FF) << 1;
			this._y = ((int)(SystemParam_0 >> 8) & 0x7F) - 0x3F;
			this._x = ((int)((SystemParam_0 & 0x0001FE) >> 1) & 0x7F) - 0x3F;
			this._multiple = (byte)StarChance_Multiples[SystemParam_1 & 0x1f];

			//sth += (ulong)this._sector.SectorColor.R;
			//sth += (ulong)this._sector.SectorColor.G;
			//sth += (ulong)this._sector.SectorColor.B;
			//sth /= 3;

			this._ageClass =
				(StarAgeClass)(AgeChance[(int)((SystemParam_0 >> 2) & 0x1F)]);

			this._mainSequenceSpectralClass =
				(StarSpectralClass)(BaseSpectralClassChance[(int)((((SystemParam_0 >> 4) & 0x1F)) & 0x1F)]);

			this._spectralClass = (StarSpectralClass)AgeSpectralEvolution
				[(int)_mainSequenceSpectralClass]
				[(int)_ageClass];

			this._luminosityClass = (StarLuminosityClass)AgeLuminosityEvolution
				[(int)_mainSequenceSpectralClass]
				[(int)_ageClass];

			this._mass = StarMass
				[(int)_mainSequenceSpectralClass]
				[(int)_luminosityClass];

			this._magnitude = StarMagnitude[(int)_spectralClass][(int)_luminosityClass];
			this._hZone = StarHz[(int)_spectralClass][(int)_luminosityClass];

			//this._luminosityClass = (StarLuminosityClass)(LChance[(int)((SystemParam_1 >> 2) & 0x1F | sth)]);
			//this._spectralClass = (StarSpectralClass) SvsLChance[(int)this._luminosityClass & 0x1F][(SystemParam_1 >> 4) & 0x1F];

			this._starType = string.Format("{0}{1}{2}",
				_spectralClass,
				_magnitude,
				_luminosityClass);



			if ((int)this._luminosityClass <= 4)
				this.Name = GetCommonName_A(Sector.IndexX, Sector.IndexY, _sysNum, true);
			else if ((int)this._luminosityClass < 6)
				this.Name = GetCommonName_A(Sector.IndexX, Sector.IndexY, _sysNum, false);
			else
				this.Name = GetSystemName_C(Sector.IndexX, Sector.IndexY, _sysNum); 
				

			this._commonName = this.Name; //GetCommonName_A(Sector.IndexX, Sector.IndexY, _sysNum);
		}
		#endregion

		#region Generate Name
		private string GetSystemName(uint coordx, uint coordy, int Sysnum) {
			coordx += (uint)Sysnum;
			coordy += coordx;
			coordx = Utils._rotl(coordx, 3);
			coordx += coordy;
			coordy = Utils._rotl(coordy, 5);
			coordy += coordx;
			coordy = Utils._rotl(coordy, 4);
			coordx = Utils._rotl(coordx, Sysnum);
			coordx += coordy;

			string dest;
			ulong uniqueid;

			// First, generate a unique id for this system
			uniqueid = (((ulong)Sysnum) << 26) + (((ulong)coordx) << 13) + (ulong)coordy;
			// Jingle the numbers a bit
			uniqueid ^= (((uniqueid >> 11) ^ (uniqueid << (32 - 11))));
			uniqueid ^= (((uniqueid >> 4) ^ (uniqueid << (32 - 4))));
			uniqueid ^= (((uniqueid >> 11) ^ (uniqueid << (32 - 11))));

			dest = namepart[(coordx >> 2) & 127];
			coordx = Utils._rotr(coordx, 5);
			dest += namepart[(coordx >> 2) & 127];
			dest += '-';

			dest += (char)('A' + (uniqueid % 26));
			uniqueid /= 26;
			dest += (int)('A' + (uniqueid % 26));
			uniqueid /= 26;

			//coordx = Utils._rotr(coordx, 5);
			//dest += namepart[(coordx >> 2) & 127];
			dest = (char)(((byte)dest[0]) ^ 0x20) + dest.Substring(1);

			return dest;
		}

		public string GetCommonName_Elite(uint coordx, uint coordy, int Sysnum) {
			coordx += (uint)Sysnum;
			coordy += coordx;
			coordx = Utils._rotl(coordx, 3);
			coordx += coordy;
			coordy = Utils._rotl(coordy, 5);
			coordy += coordx;
			coordy = Utils._rotl(coordy, 4);
			coordx = Utils._rotl(coordx, Sysnum);
			coordx += coordy;

			string dest;
			ulong uniqueid;

			// First, generate a unique id for this system
			uniqueid = (((ulong)Sysnum) << 26) + (((ulong)coordx) << 13) + (ulong)coordy;
			// Jingle the numbers a bit
			uniqueid ^= (((uniqueid >> 11) ^ (uniqueid << (32 - 11))));
			uniqueid ^= (((uniqueid >> 4) ^ (uniqueid << (32 - 4))));
			uniqueid ^= (((uniqueid >> 11) ^ (uniqueid << (32 - 11))));

			dest = namepart[(coordx >> 2) & 0x1F];
			coordx = Utils._rotr(coordx, 5);
			dest += namepart[(coordx >> 2) & 0x1F];
			coordy = Utils._rotr(coordy, 5);
			dest += namepart[(coordy >> 2) & 0x1F];

			//coordx = Utils._rotr(coordx, 5);
			//dest += namepart[(coordx >> 2) & 127];
			dest = (char)(((byte)dest[0]) ^ 0x20) + dest.Substring(1);

			return dest;
		}

		public string GetCommonName_A(uint coordx, uint coordy, int Sysnum, bool _2Words) {
			coordx += (uint)Sysnum;
			coordy += coordx;
			coordx = Utils._rotl(coordx, 3);
			coordx += coordy;
			coordy = Utils._rotl(coordy, 5);
			coordy += coordx;
			coordy = Utils._rotl(coordy, 4);
			coordx = Utils._rotl(coordx, Sysnum);
			coordx += coordy;

			string dest;
			ulong uniqueid;

			// First, generate a unique id for this system
			uniqueid = (((ulong)Sysnum) << 26) + (((ulong)coordx) << 13) + (ulong)coordy;
			// Jingle the numbers a bit
			uniqueid ^= (((uniqueid >> 11) ^ (uniqueid << (32 - 11))));
			uniqueid ^= (((uniqueid >> 4) ^ (uniqueid << (32 - 4))));
			uniqueid ^= (((uniqueid >> 11) ^ (uniqueid << (32 - 11))));




			dest = namePartStart_[(coordx >> 2) & 127];
			coordx = Utils._rotr(coordx, 5);
			dest += namePartEnd_[(coordx >> 2) & 255];

			if (_2Words) {
				coordy = Utils._rotr(coordy, 5);
				dest += " " + namePartStart_[(coordy >> 2) & 127];
				coordy = Utils._rotr(coordy, 5);
				dest += namePartEnd_[(coordy >> 2) & 255];
			}

			//coordy = Utils._rotr(coordy, 5);
			//dest += namePartEnd_[(coordy >> 2) & 255];

			//coordx = Utils._rotr(coordx, 5);
			//dest += namepart[(coordx >> 2) & 127];
			//dest = (char)(((byte)dest[0]) ^ 0x20) + dest.Substring(1);

			return dest;
		}

		public string GetSystemName_C(uint xl, uint yl, int Sysnum) {
			ulong uniqueid;
			string dest = "";

			// First, generate a unique id for this system
			uniqueid = (((ulong)Sysnum) << 26) + (((ulong)xl) << 13) + (ulong)yl;
			// Jingle the numbers a bit
			uniqueid ^= (((uniqueid >> 11) ^ (uniqueid << (32 - 11))));
			uniqueid ^= (((uniqueid >> 4) ^ (uniqueid << (32 - 4))));
			uniqueid ^= (((uniqueid >> 11) ^ (uniqueid << (32 - 11))));

			// generate the 3 letters
			dest += (char)('A' + (uniqueid % 26));
			uniqueid /= 26;
			dest += (char)('A' + (uniqueid % 26));
			uniqueid /= 26;
			dest += (char)('A' + (uniqueid % 26));
			uniqueid /= 26;

			// What's left in decimal, anything up to 4 digits
			return string.Format("{0}-{1:0000}", dest, uniqueid & 8191); //sprintf(dest, "%04d", uniqueid);
		}
		#endregion

		private void CalculateNumOfBodies() {

			/** Pf **************************************
			 * TODO:
			 * This is a dummy implementation of the 
			 * algoritm to calculate the number of
			 * satelite bodies in the system
			 ********************************************/
			Random rand = new Random((int)this._systemId);
			this._numOfBodies = rand.Next(1, 14);

		}

		#region ISerializable Members

		public override void GetObjectData(SerializationInfo info, StreamingContext context) {
			base.GetObjectData(info, context);
			info.AddValue("sector", Sector);
			info.AddValue("sysNum", _sysNum);
		}

		//internal StartSystem(Sector sector, int sysNum) : base (sector, sector.Coord) {
		public StarSystem(SerializationInfo info, StreamingContext context)
			: base(info, context) {
				this.Sector = (Sector)info.GetValue("sector", typeof(Sector));
			this._sysNum = info.GetInt32("sysNum");
			this.Name = "";
			this._starType = "";
			this._systemId = (((ulong)_sysNum) << 26) + (((ulong)Sector.IndexX) << 13) + (ulong)Sector.IndexY;
			this._systemParams = Sector.GetSystemParams(_sysNum);
			this.Generate(_systemParams[0], _systemParams[1]);
			BuildCoords();
			CalculateNumOfBodies();
		}

		#endregion

		#region Equals
		public static bool operator ==(StarSystem sObj1, StarSystem sObj2) {

			if (Object.ReferenceEquals(sObj1, null) || Object.ReferenceEquals(sObj2, null))
				return Object.ReferenceEquals(sObj1, sObj2);
			else
				return sObj1.Equals(sObj2);

		}

		public static bool operator !=(StarSystem sObj1, StarSystem sObj2) {

			if (Object.ReferenceEquals(sObj1, null) || Object.ReferenceEquals(sObj2, null))
				return !Object.ReferenceEquals(sObj1, sObj2);
			else
				return !sObj1.Equals(sObj2);
		}

		public override bool Equals(object obj) {
			StarSystem sObj = obj as StarSystem;
			if (!Object.ReferenceEquals(sObj, null)) {
				return this.SystemId == sObj.SystemId;
			}
			return false;
		}

		public override int GetHashCode() {
			return (int)this.SystemId;
		}
		#endregion

		public override string ToString() {
			return string.Format("{0} System", this.Name);
		}

		#region IExtendableObject Members

		private Properties extendedProperties = null;
		public Properties ExtendedProperties {
			get {
				if (extendedProperties == null)
					extendedProperties = new Properties();

				return extendedProperties;
			}
		}

		#endregion
	}

}
