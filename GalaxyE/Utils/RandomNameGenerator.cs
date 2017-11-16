using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalaxyE.Utils {
	public class RandomNameGenerator {

		public static string GetRandomName() {
			Random rand = new Random();
			return names[rand.Next(0, names.Length - 1)];
		}


		private static string[] names = new string[] { 
			"Ezekiel Mctaggart","Otto Giroir","Berry Rines","Freddy Mclead","Eugenio Wolfenbarger","Alonso Mang",
			"Junior Lowrance","Lenard Gilkison","Daniel Fertig","Eusebio Hachey","Reyes Ankrom","Matt Chunn",
			"Rodolfo Kirwin","Ian Riehl","Lyman Beller","Myles Martinek","John Reavis","Roland Collings",
			"Kirby Wegener","Rashad Sappington","Marcel Everette","Colin Herold","Johnnie Claiborne","Tobias Steel",
			"Isaac Gafford","Broderick Gagnon","Clark Foulds","Heath Ritz","Erin Craghead","Gary Paterno",
			"Seymour Racine","Chance Wince","Trey Kessel","Dannie Sever","Pasquale Spargo","Justin Gorden",
			"Hollis Morant","Mel Broomfield","Lynn Brimmer","Kendrick Kulinski","Shaun Relyea","German Hellmann",
			"Alphonso Upshur","Mohammad Broaddus","Noah Beighley","Hershel Ivery","Dante Sandy","Christian Marano",
			"Mitch Mcmorrow","Anibal Margerum","Courtney Calles", "Damion Oviedo", "Marshall Wotring", "Dion Coppage", 
			"Shad Henton", "Son Ansell", "Pierre Valletta", "Harris Ang", "Otha Seibel", "Kyle Spicher", "Craig Linzey", 
			"Colin Schoenberg", "Nathaniel Stringer", "Danny Gillen", "Alonso Linnell", "Keven Sims", "Darrel Fling", 
			"Alan Dockstader", "Heath Nellum", "Marcus Brookes", "Morris Saad", "Werner Eifert", "Rayford Gower", 
			"Wiley Colquitt", "Richard Tetzlaff", "Hans Shand", "Clay Sanson", "Heriberto Freire", "Sonny Eggen", "Cameron Spiker", 
			"Stewart Smidt", "Manuel Azar", "Herschel Delawder", "Gerald Serafino", "Paris Betty", "Ken Sclafani", 
			"Robert Pardini", "Albert Ambriz", "Chong Fikes", "Rosario Mccrum", "Raymond Curcio", "Agustin Blust", 
			"Preston Reindl", "Hal Tabor", "Sammie Montez", "Carlos Drennon", "Junior Hartwick", "Boyd Riggenbach", 
			"Ryan Narcisse", "Antoine Pickney", "Carey Weiskopf", "Hosea Poch", "Geoffrey Mcbrayer", "Maurice Cutts", 
			"Connie Pasha", "Cary Hilt", "Jose Killion", "Solomon Munsterman", "Omer Empson", "Buford Dieterich", 
			"Anderson Drescher", "Zachariah Moone", "Duncan Eatmon", "Tuan Bragg", "Ramon Kellam", "Peter Legendre", 
			"Korey Citizen", "Raleigh Ptacek", "Nathaniel Pfeifer", "Ted Hornback", "Jack Biermann", "Carmen Vanduyn", 
			"Christian Brasher", "Chas Averett", "Rubin Puma", "Tod Drouin", "Palmer Grimley", "Colby Turvey", 
			"Santos Arispe", "Chris Bradfield", "Werner Zanders", "Mickey Kaminski", "Floyd Weatherly", "Antonia Ogles", 
			"Howard Vanscoy", "Alden Zygmont", "Kristopher Murrell", "Alex Maravilla", "Cliff Fullington", "Florentino Machado", 
			"Claude Dalrymple", "Joseph Kulpa", "Johnathan Deason", "Mauricio Hammock", "Rey Wolfram", "Fritz Current", 
			"Tony Plumer", "Chet Marbury", "Devin Brundige", "Barrett Gratton", "Cletus Persaud", "Pat Lewallen", 
			"Terence Luby", "Saul Tate", "Curt Beckman", "Bryon Awong", "Haywood Branham", "Dewayne Sampley", "Kory Meyerson", 
			"Hugh Sibrian", "Bertram Balsamo", "Kermit Phelan", "Richie Amundson", "Willy Paterno", "Delmer Ku", 
			"Roderick Stangle", "Emerson Asaro", "Guadalupe Lichtman", "Duncan Basquez", "Conrad Both", "Cory Elgin", 
			"Leroy Yow", "Bernard Veasley", "Ronny Terry", "Gustavo Johanson", "Pierre Preslar", "Issac Vandusen", 
			"Miquel Gravelle", "Jamison Hacker", "Sebastian Boor", "Darron Witte", "Harlan Ganey", "Antonio Flohr", 
			"Brooks Finke", "Donnie Tineo", "Alexander France", "Nathaniel Webb", "Vicente Reid", "Eugene Labadie", 
			"Foster Vanderford", "Chadwick Bordeaux", "Errol Pettie", "Ramon Cabell", "Ervin Brann", "Gale Tsui", 
			"Ken Popek", "Lawerence Sacco", "Logan Goers", "Jamal Wooster", "Manual Mcilvain", "Deon Abbey", "Hugh Reece", 
			"Byron Linz", "Teodoro Spafford", "Kermit Palomo", "Enoch Mcfee", "Laurence Gressett", "Noe Gutierez", 
			"Hung Gent", "Thad Brighton", "Erwin Mcphee", "Raleigh Hammers", "Levi Gallop", "Colton Fleck", "Lincoln Knowlton", 
			"Sammy Haworth", "Jewel Soltis", "Guadalupe Clardy", "Darius Radigan", "Jonathon Hobbs", "Charles Vandeventer", 
			"Maximo Phillippe", "Jarred Pucci", "Lon Moynihan", "Stan Kaya", "Davis Sommer", "Tobias Maire", "Tyrone Seidell", 
			"Erich Byrns", "Long Mckeehan", "David Branscum", "Augustine Millen", "Courtney Mcconico", "Jermaine Jetter", 
			"Elvis Beckel", "Graham Yule", "Bobby Brabant", "Adam Tune", "Garland Stockwell", "Tom Gulyas", "Mervin Traver", 
			"Jimmie Laubscher", "Benny Mcgray", "Lou Santi", "Anibal Sailor", "Gavin Dupler", "Joshua Nedd", "Rodrick Lindberg", 
			"Alexander Ruvolo", "Eldridge Toft"
		};

	}
}
