using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalaxyE.Galaxy {
	public class StarSystemUtils {

		public struct sysSeed {
			public uint w0;
			public uint w1;
			public uint w2;
		};

		public struct sysFastSeed {
			public int a,b,c,d;
		};

		public struct PlanSys {
			public uint x, y;
			public uint economy; /* These two are actually only 0-7  */
			public uint govType;
			public uint techLevel; /* 0-16 i think */
			public uint Population;   /* One byte */
			public uint Productivity; /* Two byte */
			public uint Radius; /* Two byte (not used by game at all) */
			public sysFastSeed goatsoupseed;
			public string descOverride;
		}

		private sysFastSeed rnd_seed;
		private int seed;

		#region static arrays

		static string pairs0 = "..LEXEGEZACEBISOUSESARMAINDIREA.ERATENBERALAVETIEDORQUANTEISRION";

		static string[][] desc_list = new string[][]
			{
/* 81 */	new string[] {"known", "notable", "well known", "famous", "noted"}, //Conhecido por
/* 82 */	new string[] {"very", "", "most", "reasonably", ""}, //Muito Conhecido por
/* 83 */	new string[] {"ancient", "\x95", "great", "vast", "\xA5"}, 
/* 84 */	new string[] {"\x9E \x9D fields", "mountains", "\x9C", "\x94 forests", "breathtaking landspaces"},
/* 85 */	new string[] {"\x8E", "hospitality", "intolerance", "cult of \xA6", "passion for \x86"},
/* 86 */	new string[] {"nature", "art", "others", "life", "success"},
/* 87 */	new string[] {"devil's lisard", "monster", "bat", "snake", "\xB2"},
/* 88 */	new string[] {"beset", "plagued", "ravaged", "cursed", "scourged"},
/* 89 */	new string[] {"\x96 bursts of cosmic radiation", "\x9B \x98 \x99s", "a \x9B disease", "\x96 earthquakes", "\x96 solar activity"},
/* 8A */	new string[] {"its \x83 \x84", "the \xB1 \x98 \x99","its inhabitants' \x9A \x85", "\xA1", "its \x8D \x8E"},
/* 8B */	new string[] {"juice", "brandy", "water", "brew", "beer"},
/* 8C */	new string[] {"\xB2", "\xB1 \x99", "\xB1 \xB2", "\xB1 \x9B", "\x9B \xB2"},
/* 8D */	new string[] {"fabulous", "exotic", "dangerous", "rare", "very rare"},
/* 8E */	new string[] {"cities", "temples", "underground markets", "culture", " \xA1 "},
/* 8F */	new string[] {"\xB0", "The \xB0 System", "The \xB0 zone", "This System", "The \xB0 System"},
/* 90 */	new string[] {"a somehow unexplored", "a mostly desert", "a mainly arid", "a somehow unknown", "an arid"},
/* 91 */	new string[] {"System", "Region", "Sector", "Star System", "Zone"},
/* 92 */	new string[] {"wasp", "moth", "grub", "ant", "\xB2"},
/* 93 */	new string[] {"lisard", "snake", "dragon", "bird", "worm"},
/* 94 */	new string[] {"tropical", "dense", "rain", "impenetrable", "exuberant"},
/* 95 */	new string[] {"funny", "wierd", "unusual", "strange", "peculiar"},
/* 96 */	new string[] {"frequent", "occasional", "unpredictable", "dreadful", "deadly"},
/* 97 */	new string[] {"\x82 \x81 for \x8A", "\x82 \x81 for \x8A and \x8A", "\x88 by \x89 but also \x81 for \x8A", "\x82 \x81 for \x8A but \x88 by \x89","\x90 \x91"},
/* 98 */	new string[] {"\x9A", "\xA5", "unusual", "rare", "dark"},
/* 99 */	new string[] {"\x9F", "\xA0", "\x87oid", "\x93", "\x92"},
/* 9A */	new string[] {"ancient", "exceptional", "eccentric", "ingrained", "\x95"},
/* 9B */	new string[] {"killer", "deadly", "evil", "lethal", "vicious"},
/* 9C */	new string[] {"tornados", "electromagnetic storms", "lava channels", "rocky formations", "volcanoes"},
/* 9D */	new string[] {"plant", "flower", "fruit", "seed", "\xB2weed"},
/* 9E */	new string[] {"\xB2", "\xB1 \xB2", "\xB1 \x9B", "inhabitant", "\xB1 \xB2"},
/* 9F */	new string[] {"shrew", "beast", "bison", "snake", "wolf"},
/* A0 */	new string[] {"\xB2", "cat", "monkey", "goat", "fish"},
/* A1 */	new string[] {"\x8C \x8B", "\xB1 \x9F \xA2","its \x8D \xA0 \xA2", "\xA3 \xA4", "\x8C \x8B"},
/* A2 */	new string[] {"meat", "cutlet", "steak", "burgers", "soup"},
/* A3 */	new string[] {"ice", "plasma", "zero-G", "vacuum", "\xB1 ultra"},
/* A4 */	new string[] {"hockey", "jump", "fight", "futebol", "tennis"},
/* A5 */	new string[] {"purple", "blue", "red", "pink", "green"},
/* A5 */	new string[] {"\xB2", "the sun", "the \xA5 \xB2", "the dead", "nature"}
			};

		#endregion

		private StarSystem starSystem;
		private PlanSys planSys;

		public PlanSys Information { get { return planSys; } internal set { this.planSys = value; } }

		public StarSystemUtils(StarSystem starSystem) {
			this.starSystem = starSystem;

			//need to actually force the creation of the planets so we can calculate some params

			int habitability = 0;

			foreach (SystemBody body in starSystem.SystemBodies) {
				switch (body.BodyType) {
					case SystemBodyType.Asteroid:
						if (body.Temperature < 70 && body.Temperature > -150) habitability += 1; 
						break;
					case SystemBodyType.GasGiant:
						break;
					case SystemBodyType.IceWorld:
						habitability += 50; 
						break;
					case SystemBodyType.Inferno:
						break;
					case SystemBodyType.RingedGasGiant:
						break;
					case SystemBodyType.RockyPlanetoid:
						if (body.Temperature < 70 && body.Temperature > -150) habitability += 5; 
						break;
					case SystemBodyType.RockyWorld:
						if (body.Temperature < 70 && body.Temperature > -150) habitability += 10; 
						break;
					case SystemBodyType.SubGasGiant:
						break;
					case SystemBodyType.Terrestrial:
						habitability += 100; break;
					case SystemBodyType.Venuzian:
						break;
					case SystemBodyType.WaterWorld:
						habitability += 80; break;
				}
			}

			sysSeed systemSeed = new sysSeed();

			systemSeed.w0 = (uint)(starSystem.SystemId);
			systemSeed.w1 = (uint)(starSystem.Coords.Sector.X << 10 | (int)starSystem.SystemId);
			systemSeed.w2 = (uint)(starSystem.Coords.Sector.Y << 10 | (int)starSystem.SystemId);

			tweakSeed(ref systemSeed);
			tweakSeed(ref systemSeed);
			tweakSeed(ref systemSeed);
			tweakSeed(ref systemSeed);

			planSys = this.MakeSystemBaseInfo(systemSeed, habitability);
			rnd_seed = planSys.goatsoupseed;
			seed = (int)(systemSeed.w0 << 16 & systemSeed.w1);
		}

		public string GetDescription() {
			if (this.planSys.descOverride != null)
				return this.planSys.descOverride;
			else {
				StringBuilder sbuilder = new StringBuilder();
				goat_soup("\x8F is \x97.", this.starSystem, sbuilder);
				return sbuilder.ToString();
			}
		}

		private void tweakSeed(ref sysSeed s) {
			uint temp;
			temp = (s.w0 + s.w1 + s.w2); /* 2 byte aritmetic */
			s.w0 = s.w1;
			s.w1 = s.w2;
			s.w2 = temp;
		}

		private int gen_rnd_number() {
			int a,x;
			x = (rnd_seed.a * 2) & 0xFF;
			a = x + rnd_seed.c;
			if (rnd_seed.a > 127) a++;
			rnd_seed.a = a & 0xFF;
			rnd_seed.c = x;

			a = a / 256;	/* a = any carry left from above */
			x = rnd_seed.b;
			a = (a + x + rnd_seed.d) & 0xFF;
			rnd_seed.b = a;
			rnd_seed.d = x;

			return a; // | rand.Next() ;
			
		}

		private void goat_soup(string source, StarSystem sys, StringBuilder sbuilder) {

			for (int n = 0; n < source.Length; n++) {

				uint c = (uint)(source[n]) & 0xFF;

				if (c == '\0') break;

				if (c < 0x80)
					sbuilder.Append((char)c);

				else {

					if (c <= 0xAF) {
						int rnd = gen_rnd_number();
						goat_soup(
							desc_list[c - 0x81][
								((rnd >= 0x33) ? 0 : 1) +
								((rnd >= 0x66) ? 0 : 1) +
								((rnd >= 0x99) ? 0 : 1) +
								((rnd >= 0xCC) ? 0 : 1)
							], sys, sbuilder);
					} else
						switch (c) {
							case 0xB0: /* system name */
								sbuilder.Append(sys.Name.ToLower());
								break;
							case 0xB1: /* <system name>ian */
								sbuilder.AppendFormat("{0}ian", sys.CommonName.ToLower());
								break;
							case 0xB2: /* random name */
								int i;
								int len = gen_rnd_number() & 3;
								for (i = 0; i <= len; i++) {
									int x = gen_rnd_number() & 0x3e;

									if (pairs0[x] != '.')
										sbuilder.Append(pairs0[x].ToString().ToLower());

									if (i != 0 && (pairs0[x + 1] != '.'))
										sbuilder.Append(pairs0[x + 1].ToString().ToLower());
								}
								break;
							default: sbuilder.AppendFormat("<bad char in data {0:X}>", (char)c);
								return;
						}	/* endswitch */
				}	/* endelse */
			}	/* endwhile */
		}	/* endfunc */

		string[] govnames = new string[] {
			"Anarchy","Feudal","Dictatorship","Communist",
			"Multi-gov","Confederacy","Democracy","Corporate State", "None"
		};

		string[] econnames = new string[] {
			"Rich Ind","Average Ind","Poor Ind","Mainly Ind",
			"Mainly Agri","Rich Agri","Average Agri","Poor Agri", "None"
		};

		private PlanSys MakeSystemBaseInfo(sysSeed s, int habitability) {

			PlanSys thisSys = new PlanSys();
			uint longnameflag= s.w0 & 64;

			thisSys.descOverride = null;
			Random rand = new Random((int)(s.w1 >> 8));

			

			thisSys.x = (s.w1 >> 8);
			thisSys.y = (s.w0 >> 8);
				
			int[] possibleEconomies = null;

			if (habitability > 0 && habitability <= 10) {
				thisSys.descOverride = "Mainly arid system with some small scale mining operations";
				thisSys.govType = (uint)rand.Next(0, 2);
				possibleEconomies = new int[] { 2, 3 };
			} else if (habitability > 10 && habitability <= 20) {
				thisSys.govType = (uint)rand.Next(0, 4);
				possibleEconomies = new int[] { 1, 2, 3 };
			} else if (habitability > 20 && habitability <= 40) {
				thisSys.govType = (uint)rand.Next(1, 5);
				possibleEconomies = new int[] { 0, 1, 2, 3 };
			} else if (habitability > 40 && habitability <= 80) {
				thisSys.govType = (uint)rand.Next(2, 5);
				possibleEconomies = new int[] { 0, 1, 2, 3 };
			} else {
				possibleEconomies = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };
				thisSys.govType = (uint)rand.Next(2, 7);
			}

			if (habitability == 0) {
				thisSys.govType = 0;
				thisSys.economy = 0;
				thisSys.Population = 0;
				thisSys.Productivity = 0;
				thisSys.techLevel = 0;
			} else {
				thisSys.economy = (uint)rand.Next(0, possibleEconomies.Length - 1);

				if (thisSys.govType <= 1) {
					thisSys.economy = ((thisSys.economy) | 2);
				}


				thisSys.techLevel = ((s.w1 >> 8) & 3) + (thisSys.economy ^ 7);
				thisSys.techLevel += (thisSys.govType >> 1);
				if (((thisSys.govType) & 1) == 1) thisSys.techLevel += 1;

				/* simulation of 6502's LSR then ADC */
				thisSys.Population = 4 * (uint)((thisSys.techLevel + thisSys.economy) * (habitability / 100.0));
				thisSys.Population += (thisSys.govType) + 1;

				thisSys.Productivity = (((thisSys.economy) ^ 7) + 3) * ((thisSys.govType) + 4);
				thisSys.Productivity *= (thisSys.Population) * 8;

			}

			thisSys.Radius = 256 * (((s.w2 >> 8) & 15) + 11) + thisSys.x;

			thisSys.goatsoupseed.a = (int)(s.w1 & 0xFF);
			thisSys.goatsoupseed.b = (int)(s.w1 >> 8);
			thisSys.goatsoupseed.c = (int)(s.w2 & 0xFF);
			thisSys.goatsoupseed.d = (int)(s.w2 >> 8);

			return thisSys;

		}

	}
}
