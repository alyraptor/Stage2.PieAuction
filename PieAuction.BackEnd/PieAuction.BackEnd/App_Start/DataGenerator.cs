﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json.Linq;
using PieAuction.BackEnd.Data_Access;
using PieAuction.BackEnd.Models;

namespace PieAuction.BackEnd.App_Start
{
    public class DataGenerator
    {
        public static Random rand = new Random();

        public Task StartGenerating()
        {
            return Task.Run(() =>
            {
                Task.Run((Action) KeepAddingUsers);
                Task.Run((Action) KeepAddingPies);
                Task.Run((Action) KeepAddingBids);
            });
        }

        public void KeepAddingBids()
        {
            while (true)
            {
                AddNewBid();
                Thread.Sleep(TimeSpan.FromSeconds(rand.Next(0, 6)));
            }
        }

        public void AddNewBid()
        {
            try
            {
                var httpClient = new HttpClient()
                {
                    BaseAddress = new Uri(@"http://localhost:15665/api/")
                };

                var userDao = new AuctionUserDao();
                var pieDao = new PieDao();

                var possibleUsers = userDao.GetFilteredListOfUsers(u => u.IsStudent == false).ToArray();
                var randomUser = possibleUsers[rand.Next(0, possibleUsers.Length)].Id;

                var possiblePies = pieDao.GetPies(p => !p.MadeByUserIds.Contains(randomUser) &&
                                                       !p.SoldToUserId.HasValue &&
                                                       p.EndDateTime > DateTime.Now).ToArray();
                var randomPie = possiblePies[rand.Next(0, possiblePies.Length)].Id;

                var getBids = JArray.Parse(httpClient.GetAsync("bids?pieId=" + randomPie).Result.Content
                    .ReadAsStringAsync().Result);
                var maxBid = getBids.Any() 
                    ? getBids.Max(b => b["Amount"].Value<decimal>())
                    : rand.Next(5, 200);

                var res = httpClient.PostAsync("bids", new StringContent(new JObject()
                {
                    ["AuctionUserId"] = randomUser.ToString(),
                    ["PieId"] = randomPie.ToString(),
                    ["Amount"] = (int)(maxBid + rand.Next(1, 250))
                }.ToString(), Encoding.UTF8, "application/json")).Result;
                Console.WriteLine(res.ToString());

            }
            catch { }
        }

        public void KeepAddingPies()
        {
            while (true)
            {
                AddNewPie();

                var pieDao = new PieDao();
                var pies = pieDao.GetPies();
                Thread.Sleep(TimeSpan.FromSeconds(rand.Next(5, pies.Length < 100 ? 10 : 45)));
            }
        }

        public void AddNewPie()
        {
            try
            {
                var userDao = new AuctionUserDao();
                var pieDao = new PieDao();

                var possibleUsers = userDao.GetFilteredListOfUsers();
                while (possibleUsers.Count(u => u.IsStudent == false) < 4 ||
                       possibleUsers.Count(u => u.IsStudent == true) < 4)
                {
                    AddNewUser();
                    possibleUsers = userDao.GetFilteredListOfUsers();
                }

                var randomPieFlavor = PieFlavorsList[rand.Next(0, PieFlavorsList.Length)];
                var madeByUsers = possibleUsers.OrderBy(u => rand.NextDouble()).Take(rand.Next(1, 4)).ToArray();

                var newPie = new Pie()
                {
                    Flavor = randomPieFlavor,
                    Name =
                        $"{madeByUsers[rand.Next(0, madeByUsers.Length)].FirstName}'s {PieNameAttributes[rand.Next(0, PieNameAttributes.Length)]} {randomPieFlavor} Pie",
                    MadeByUserIds = madeByUsers.Select(u => u.Id).ToArray(),
                    IsGlutenFree = rand.NextDouble() < 0.2 ? true : false,
                    IsVegan = rand.NextDouble() < 0.2 ? true : false,
                    SoldToUserId = rand.NextDouble() < 0.3
                        ? possibleUsers.Where(u => u.IsStudent == false).ToArray()[
                            rand.Next(0, possibleUsers.Count(u => u.IsStudent == false))].Id
                        : (Guid?) null,
                    ImageAddress = PieImagesList[rand.Next(0, PieImagesList.Length)]
                };
                pieDao.InsertNewPie(newPie);
            }
            catch
            {
            }
        }

        public void KeepAddingUsers()
        {
            while (true)
            {
                AddNewUser();
                Thread.Sleep(TimeSpan.FromSeconds(rand.Next(30, 90)));
            }
        }

        public void AddNewUser()
        {
            try
            {
                var userDao = new AuctionUserDao();
                var user = new AuctionUser()
                {
                    FirstName = FirstNameList[rand.Next(0, FirstNameList.Length)],
                    LastName = LastNamesList[rand.Next(0, LastNamesList.Length)],
                    IsStudent = rand.NextDouble() < 0.5 ? true : false
                };
                userDao.NewUser(user);
            }
            catch
            {
            }
        }

        public static string[] FirstNameList = new string[]
        {
            "Randy",
            "Bendick",
            "Inness",
            "Charlotte",
            "Nicholas",
            "Meier",
            "Kale",
            "Francisca",
            "Clark",
            "Delmer",
            "Jessey",
            "Dotty",
            "Sigmund",
            "Elnora",
            "Nanni",
            "Ephraim",
            "Stavros",
            "Avril",
            "Egan",
            "Jan",
            "Jaymie",
            "Nikolaos",
            "Olvan",
            "Francisca",
            "Adham",
            "Payton",
            "Candace",
            "Fidel",
            "Edith",
            "Aaren",
            "Gerardo",
            "Shawn",
            "Field", "Dyann",
            "Loree",
            "Remington",
            "Marybeth",
            "Laryssa",
            "Garrett",
            "Rogerio",
            "Nettle",
            "Conny",
            "Isaak",
            "Dene",
            "Modesty",
            "Dorotea",
            "Gayler",
            "Cindie",
            "Mala",
            "Eugenie",
            "Jaquith",
            "Rodrick",
            "Doretta",
            "Eben",
            "Alayne",
            "Audy",
            "Pepita",
            "Malissa",
            "Gerhardine",
            "Marisa",
            "Matthew",
            "Carma",
            "Lorraine",
            "Phylis",
            "Enoch",
            "Editha",
            "Maddi",
            "Sydney",
            "Dorothea",
            "Gusti",
            "Atlante",
            "Tomasina",
            "Koo",
            "Bordy",
            "Jada",
            "Adelle",
            "Lucinda",
            "Emmery",
            "Amargo",
            "Raeann",
            "Hillard",
            "Clementius",
            "Kippar",
            "Olenolin",
            "Jarred",
            "Theo",
            "Billye",
            "Raffaello",
            "Cally",
            "Lynnell",
            "Anallise",
            "Steffie",
            "Verney",
            "Wilhelmina",
            "Allis",
            "Marcello",
            "Gilli",
            "Morgan",
            "Tremayne",
            "Jefferson",
            "Jeannine",
            "Hermann",
            "Brendon",
            "Manolo",
            "Cari",
            "Merridie",
            "Sheffy",
            "Christel",
            "Karine",
            "Rey",
            "Carroll",
            "Tiphany",
            "Louis",
            "Lia",
            "Keary",
            "Robinia",
            "Mable",
            "Dee",
            "Charla",
            "Bobine",
            "Denny",
            "Atalanta",
            "Devy",
            "Stephani",
            "Austin",
            "Illa",
            "Yuma",
            "Gard",
            "Rochelle",
            "Tyler",
            "Sianna",
            "Stephan",
            "Kip",
            "Kendell",
            "Silvan",
            "Kristel",
            "Gaspar",
            "Kaile",
            "Terrence",
            "Caralie",
            "Neall",
            "Kelley",
            "Kerby",
            "Bab",
            "Darbie",
            "Berna",
            "Filberte",
            "Jannelle",
            "Janelle",
            "George",
            "Hale",
            "Yvonne",
            "Meggy",
            "Karoly",
            "Delilah",
            "Gennie",
            "Westbrooke",
            "Gasparo",
            "Dacia",
            "Roxy",
            "Garold",
            "Theodore",
            "Shaun",
            "Tera",
            "Marjie",
            "Creigh",
            "Madel",
            "Abner",
            "Meyer",
            "Ursulina",
            "Flossi",
            "Nickie",
            "Zenia",
            "Cassandre",
            "Silvano",
            "Lev",
            "Melisande",
            "Pernell",
            "Prue",
            "Parke",
            "Sumner",
            "Mack",
            "Remington",
            "Hal",
            "Philomena",
            "Heddi",
            "Elke",
            "Pearle",
            "Dorris",
            "Tanya",
            "Hercule",
            "Caterina",
            "Rourke",
            "Hildy",
            "Angel",
            "Reena",
            "Koren",
            "Celka",
            "Rudy",
            "Cary",
            "Kristen",
            "Ike",
            "Amity",
            "Uta",
            "Pepita",
            "Allx",
            "Linnea",
            "Eugene",
            "Kory",
            "Dru",
            "Rafa",
            "Kaylee",
            "Merridie",
            "Elinor",
            "Baird",
            "Adolpho",
            "Annissa",
            "Hasty",
            "Edgar",
            "Lelia",
            "Faye",
            "Amble",
            "Lodovico",
            "Luise",
            "Linette",
            "Lanette",
            "Mylo",
            "Ernaline",
            "Paul",
            "Skip",
            "Flss",
            "Karisa",
            "Tucker",
            "Sophey",
            "Hurleigh",
            "Hoyt",
            "Jaimie",
            "Staffard",
            "Geoffry",
            "Avram",
            "Mariele",
            "Knox",
            "Maurene",
            "Mariel",
            "Lorry",
            "Burnaby",
            "Jerrilyn",
            "Else",
            "Corrie",
            "Stanislaus",
            "Fabian",
            "Tanney",
            "Joana",
            "Lanna",
            "Johnathan",
            "Krystle",
            "Camel",
            "Holden",
            "Ronica",
            "Corbet",
            "Leonore",
            "Natividad",
            "Randi",
            "Clayborn",
            "Elnora",
            "Mercie",
            "Kissie",
            "Jeanie",
            "Emlyn",
            "Debee",
            "Cedric",
            "Gussie",
            "Linus",
            "Kettie",
            "Doti",
            "Bernie",
            "Verne",
            "Evy",
            "Aaren",
            "Oren",
            "Josee",
            "Irwinn",
            "Jacquetta",
            "Remus",
            "Bert",
            "Pamela",
            "Adrienne",
            "Perl",
            "Shanda",
            "Chucho",
            "Raviv",
            "Ranna",
            "Piggy",
            "Marlin",
            "Erl",
            "Tessie",
            "Shantee",
            "Ryann",
            "Dasi",
            "Adlai",
            "Cy",
            "Maynord",
            "Joseph",
            "Jessalin",
            "Barbara-anne",
            "Ingra",
            "Mead",
            "Janaye",
            "Norman",
            "Brina",
            "Delmor",
            "Kitti",
            "Edita",
            "Creighton",
            "Malanie",
            "Jack",
            "Serene",
            "Kenon",
            "Gordy",
            "Sam",
            "Travers",
            "Di",
            "Olympe",
            "Lebbie",
            "Rose",
            "Morna",
            "Chaddy",
            "Emelen",
            "Hugues",
            "Valentine",
            "Delinda",
            "Dominic",
            "Conrado",
            "Terrye",
            "Herbie",
            "Welch",
            "Ruth",
            "Kermy",
            "Richardo",
            "Ashien",
            "Ingunna",
            "Susanna",
            "Shaw",
            "Harley",
            "Sandor",
            "Barbara-anne",
            "Leah",
            "Kendricks",
            "Regina",
            "Ardith",
            "Devy",
            "Crista",
            "Ofella",
            "Stavros",
            "Stacy",
            "Chrissie",
            "Sharona",
            "Mattias",
            "Carlee",
            "Dalton",
            "Caleb",
            "Alex",
            "Else",
            "Aurie",
            "Evvy",
            "Aleksandr",
            "Jayson",
            "Hugo",
            "Jocko",
            "Felipa",
            "Kimbell",
            "Chucho",
            "Dayle",
            "Karen",
            "Hadlee",
            "Marcelia",
            "Wolfie",
            "Kelila",
            "Lauretta",
            "Inge",
            "Dene",
            "Virgil",
            "Townsend",
            "Irwinn",
            "Aubrey",
            "Geordie",
            "Langston",
            "Randie",
            "Wittie",
            "Jo-anne",
            "Alexis",
            "Carl",
            "Clark",
            "Kellie",
            "Shaughn",
            "Peadar",
            "Roddy",
            "Itch",
            "Jabez",
            "Clayson",
            "Suzi",
            "Damiano",
            "Celeste",
            "Dian",
            "Willette",
            "Kristofor",
            "Isaak",
            "Nikki",
            "Conny",
            "Nobe",
            "Lorri",
            "Roanna",
            "Gael",
            "Marillin",
            "Sherwood",
            "Jacky",
            "Vivian",
            "Emeline",
            "Sharona",
            "Buckie",
            "Caz",
            "Solomon",
            "Shannah",
            "Hugibert",
            "Malena",
            "Winne",
            "Wain",
            "Gray",
            "Nonna",
            "Carole",
            "Georgeta",
            "Orren",
            "Kevon",
            "Isidro",
            "Quintin",
            "Lissie",
            "Antone",
            "Coralie",
            "Pearle",
            "Hammad",
            "Darcie",
            "Gerome",
            "Lonnard",
            "Aili",
            "Ray",
            "Ronnie",
            "Lilah",
            "Maia",
            "Salim",
            "Kellsie",
            "Julee",
            "Kirstin",
            "Birgitta",
            "Fabio",
            "Veriee",
            "Hale",
            "Herschel",
            "Emilee",
            "Lil",
            "Archibaldo",
            "Butch",
            "Bordy",
            "Ryley",
            "Diana",
            "Prisca",
            "Saunder",
            "Silas",
            "Beauregard",
            "Leon",
            "Lanny",
            "Lettie",
            "Georgy",
            "Bertina",
            "Terrie",
            "Phillida",
            "Katerine",
            "Boris",
            "Enoch",
            "Carie",
            "Kora",
            "Corine",
            "Swen",
            "Magdalene",
            "Paddie",
            "Claudine",
            "Nicola",
            "Rudie",
            "Mommy",
            "Tabatha",
            "Serena",
            "Corie",
            "Salem",
            "Mireielle",
            "Lyndy",
            "Michel",
            "Vinnie",
            "Roch",
            "Klement",
            "Skip",
            "Herrick",
            "Phebe",
            "Abel",
            "Brynna",
            "Giacomo",
            "Filip",
            "Vassily",
            "Drew",
            "Torey",
            "Leontyne",
            "Zechariah",
            "Athena",
            "Vevay",
            "Sandy",
            "Dorian",
            "Clarance",
            "Marge",
            "Saba",
            "Rivi",
            "Jolee",
            "Arlin",
            "Pattie",
            "Antonie",
            "Stefa",
            "Leontyne",
            "Davis",
            "Nancee",
            "Eduardo",
            "Ambrose",
            "Arte",
            "Irv",
            "Cullan",
            "Eal",
            "Gabriella"
        };

        public static string[] LastNamesList = new string[]
        {
            "Skirvane",
            "Aspling",
            "Rendell",
            "Boost",
            "McSherry",
            "Guppy",
            "Simic",
            "Dany",
            "Masedon",
            "Barthod",
            "Thompson",
            "Wethered",
            "Lesek",
            "Monkley",
            "McAvey",
            "Geistbeck",
            "Leipelt",
            "Edleston",
            "Lauder",
            "Portchmouth",
            "Andreolli",
            "Betjes",
            "Holstein",
            "Pocklington",
            "Origin",
            "Burgot",
            "King",
            "Elkin",
            "Downs",
            "Corwood",
            "Arter",
            "Belhome", "Larkby",
            "Monks",
            "Blann",
            "Howels",
            "Worpole",
            "Patry",
            "Valens-Smith",
            "Bullers",
            "Alderton",
            "Garmston",
            "Mougenel",
            "Filipowicz",
            "Bassom",
            "Claricoates",
            "Tabor",
            "Dewer",
            "Beseke",
            "Roizin",
            "Haile",
            "Lucia",
            "Tinmouth",
            "Runchman",
            "Hamflett",
            "Labeuil",
            "Nys",
            "Grimm",
            "Wyndham",
            "Kondratenko",
            "Croizier",
            "Stirton",
            "Peealess",
            "Urling",
            "Dreher",
            "Andreopolos",
            "Benedetti",
            "Wickens",
            "Penylton",
            "Darton",
            "Crockford",
            "Elie",
            "Skerm",
            "Kubatsch",
            "Doubleday",
            "Giacobbo",
            "Morratt",
            "Beric",
            "Giotto",
            "Parrington",
            "Eilers",
            "Kinnin",
            "Brennand",
            "Dillway",
            "Monk",
            "Rubenchik",
            "Fowgies",
            "Paolino",
            "Mozzi",
            "Ceeley",
            "McNirlin",
            "Banbrook",
            "Featherstonhaugh",
            "Hanlin",
            "Goad",
            "McAughtry",
            "Leathe",
            "De Luna",
            "Hannent",
            "McGookin",
            "Midden",
            "Brellin",
            "Scamaden",
            "Dobney",
            "Brunnen",
            "De la Perrelle",
            "Lutwidge",
            "Laugheran",
            "Eddowis",
            "Pautard",
            "Fanton",
            "Baston",
            "Spoure",
            "Stollenbeck",
            "McDill",
            "Langcaster",
            "Khristoforov",
            "Tottle",
            "Blatchford",
            "Gerritsma",
            "Dunphie",
            "Heimes",
            "Gregoraci",
            "Pashan",
            "Picot",
            "Overel",
            "Dobbie",
            "Wipper",
            "Bourgourd",
            "Najera",
            "Zorzoni",
            "Colombier",
            "Harrap",
            "Erat",
            "Plastow",
            "Wooland",
            "Witton",
            "Frankton",
            "Varren",
            "Kienzle",
            "Frostdyke",
            "Merigon",
            "Abbot",
            "Girardengo",
            "Girardey",
            "Etridge",
            "Peiro",
            "Zotto",
            "Nother",
            "Gerauld",
            "Rubinsaft",
            "Sherewood",
            "Housley",
            "Barbrick",
            "Pears",
            "Dankersley",
            "Balston",
            "Veregan",
            "Duffit",
            "Blondelle",
            "Buckner",
            "Dunsmuir",
            "Hartnup",
            "de Werk",
            "Mattiuzzi",
            "Scarman",
            "Weaben",
            "Dessaur",
            "Stegers",
            "Sickling",
            "Palombi",
            "Cudiff",
            "Grier",
            "Hundy",
            "Huett",
            "Oldfield-Cherry",
            "Parnaby",
            "Heasley",
            "Epton",
            "Hagergham",
            "Aaron",
            "Gillet",
            "Northedge",
            "Duiguid",
            "Senyard",
            "Kerton",
            "Crothers",
            "Carriage",
            "Pasley",
            "Dally",
            "Dunleavy",
            "Mellor",
            "Sly",
            "Condy",
            "Deeley",
            "Falvey",
            "Meharry",
            "Pryde",
            "Olrenshaw",
            "Challenger",
            "McLean",
            "Goodhall",
            "Hurich",
            "Fike",
            "Pardew",
            "Jeffers",
            "Pembry",
            "Manicomb",
            "Heller",
            "Gaddie",
            "Woodburne",
            "Royl",
            "Szymanski",
            "Dollin",
            "Comerford",
            "Warrilow",
            "Garnar",
            "Dacca",
            "McMahon",
            "Olivetti",
            "Pratte",
            "Lorente",
            "Noyce",
            "Hadden",
            "Pharo",
            "Brimson",
            "Jacke",
            "Hane",
            "Youell",
            "Hodgins",
            "Ortelt",
            "Pickworth",
            "Aldwich",
            "De Maria",
            "Bunney",
            "Buddles",
            "Escritt",
            "Jellis",
            "McGarel",
            "Hughf",
            "MacCoveney",
            "Linnard",
            "Banfield",
            "Hupe",
            "Isherwood",
            "Lenden",
            "Wastie",
            "MacArthur",
            "Kedward",
            "Wookey",
            "Wadmore",
            "Holdron",
            "Sharple",
            "Fearnsides",
            "Bees",
            "Biernacki",
            "Nairne",
            "Madgewick",
            "Josephi",
            "Guesford",
            "Dillet",
            "Baudesson",
            "Aggott",
            "Coupar",
            "Pee",
            "Kassman",
            "Edmonson",
            "Escoffier",
            "Srawley",
            "Ivens",
            "Dunnan",
            "Ducarne",
            "Ledgister",
            "Langhor",
            "Coady",
            "Baynham",
            "Mylan",
            "Ellings",
            "Dehn",
            "Dorrell",
            "Moles",
            "Degli Abbati",
            "Toffoletto",
            "Bolitho",
            "Stamp",
            "Scatchar",
            "Thunderchief",
            "Gherardini",
            "Stevani",
            "Trubshawe",
            "McCudden",
            "Enston",
            "Lodovichi",
            "Domerc",
            "Sivier",
            "Parken",
            "Joney",
            "Landsbury",
            "Skittles",
            "Roaf",
            "Sabater",
            "Tandy",
            "O'Rourke",
            "Fido",
            "Thairs",
            "Gudyer",
            "Tidcombe",
            "Presswell",
            "Frohock",
            "Chasson",
            "Divis",
            "Anderbrugge",
            "Ollive",
            "Windridge",
            "Philbin",
            "Ilewicz",
            "Richin",
            "Gallihaulk",
            "Pinor",
            "Cumo",
            "Ornelas",
            "Edgeley",
            "Tzar",
            "Mapstone",
            "Brown",
            "Indruch",
            "Suttle",
            "Pitcher",
            "Clementson",
            "Clissett",
            "Topliss",
            "Bortolazzi",
            "Trouncer",
            "Jacox",
            "Empleton",
            "Brunelleschi",
            "Farrans",
            "McKeon",
            "Spurnier",
            "Dive",
            "Teck",
            "O'Corr",
            "Gadsden",
            "Shacklady",
            "Askem",
            "Verbeke",
            "Innis",
            "Gaunt",
            "Murdoch",
            "Noe",
            "Gregan",
            "Baunton",
            "Pettko",
            "Shasnan",
            "Spendlove",
            "Hazeldene",
            "Farney",
            "Maria",
            "Bostick",
            "Stroton",
            "Lamas",
            "Disbrey",
            "McClurg",
            "Doxey",
            "Scaysbrook",
            "McArley",
            "Lorinez",
            "Paireman",
            "Bastard",
            "Rusbridge",
            "Pogosian",
            "Dunckley",
            "Kwietak",
            "Matts",
            "Helbeck",
            "Thayre",
            "Cheverton",
            "Frost",
            "Crebott",
            "Gregoretti",
            "Tanswell",
            "Cunnell",
            "Lundon",
            "Fairhall",
            "Worg",
            "Vaudre",
            "Pristnor",
            "Levecque",
            "Redwin",
            "Woodburne",
            "Yakolev",
            "Bayly",
            "Mannion",
            "McKinstry",
            "Dagworthy",
            "Imore",
            "Fullom",
            "Casterou",
            "Card",
            "Hirjak",
            "Acarson",
            "Rilston",
            "Sommer",
            "Buckham",
            "Cardello",
            "Wasmer",
            "Wendover",
            "McCrudden",
            "O'Hanlon",
            "Raggitt",
            "Ternouth",
            "Pantin",
            "de Aguirre",
            "Hand",
            "Dixey",
            "Rosekilly",
            "Denyakin",
            "Cubitt",
            "Swadon",
            "Hefner",
            "McDonnell",
            "Blazdell",
            "Ventham",
            "Billyard",
            "Fellenor",
            "Sessions",
            "Lewsie",
            "Hurch",
            "Bee",
            "Eary",
            "Gaskoin",
            "Bernardotte",
            "Capewell",
            "Ginnally",
            "Hartfield",
            "Millbank",
            "Leadstone",
            "Arnell",
            "Wrates",
            "Mussettini",
            "Cridge",
            "Barkes",
            "Hayhoe",
            "Joslyn",
            "Melendez",
            "Chinn",
            "McAulay",
            "Spohr",
            "Vale",
            "Byram",
            "Dorbon",
            "Gerrets",
            "Aubrey",
            "Saggers",
            "Tillman",
            "Fronek",
            "Doole",
            "Satterley",
            "Corneille",
            "Bycraft",
            "Simenon",
            "Lurcock",
            "Dumberell",
            "Dedrick",
            "Antonietti",
            "Elstob",
            "Bisgrove",
            "Figgess",
            "Took",
            "Brazur",
            "Prise",
            "Sloy",
            "Huster",
            "Pinsent",
            "Bullent",
            "Sundin",
            "De la Yglesia",
            "Cristoforetti",
            "Seakings",
            "Adey",
            "Frostick",
            "McVity",
            "Norcock",
            "Rennick",
            "Mealham",
            "Dohms",
            "Gaitskill",
            "Sarah",
            "Saura",
            "Everingham",
            "Nettleship",
            "Wittke",
            "MacGow",
            "Leslie",
            "Jagiela",
            "Springford",
            "Katt",
            "Yesipov",
            "Lornsen",
            "Ballefant",
            "Shearston",
            "Chesnay",
            "Scouler",
            "Beebee",
            "Scopes",
            "Neild",
            "Millam",
            "Tatersale",
            "Keavy",
            "Caldecutt",
            "Rubinsky",
            "Cleaveland",
            "Shreeve",
            "Hicklingbottom",
            "Bucky",
            "Bonifant",
            "Yanele",
            "Yellowley",
            "Unthank",
            "Brazenor",
            "Atlay",
            "Kiossel",
            "Keirl",
            "Kettlestring",
            "Vasiltsov",
            "Jamme",
            "East",
            "Couthard",
            "Leimster",
            "Varga",
            "Kennaway",
            "Duxfield",
        };

        public static string[] PieFlavorsList = new string[]
        {
            "Key Lime",
            "Lemon Meringue",
            "Banana Cream",
            "Pecan",
            "Blueberry",
            "Pumpkin",
            "Apple",
            "Strawberry",
            "Cherry",
            "Peach",
            "Blackberry",
            "Mississippi Mud",
            "Coconut Cream",
            "Chocolate Cream"
        };

        public static string[] PieNameAttributes = new string[]
        {
            "Best",
            "Favorite",
            "Special",
            "One-Of-A-Kind",
            "Family Recipe",
            "Delicious",
            "World Famous",
            "Superior",
            "Hy-Vee",
            "Bakery",
            "Double-Crust",
            "Twice-Baked",
            "No Bake"
        };

        public static string[] PieImagesList = new string[]
        {
            "https://i.imgur.com/AE40CAG.jpg",
            "https://i.imgur.com/9jPeh1p.jpg",
            "https://i.imgur.com/7tMxdvl.jpg",
            "https://i.imgur.com/tli7Dvg.jpg",
            "https://i.imgur.com/C1yOAGN.jpg",
            "https://i.imgur.com/lX5L6Vq.jpg",
            "https://i.imgur.com/69lelke.jpg",
            "https://i.imgur.com/LmBfcqW.jpg",
            "https://i.imgur.com/sEUlGOw.jpg",
            "https://i.imgur.com/dlaC3l8.jpg",
            "https://i.imgur.com/jwGjwTt.jpg"
        };
    }
}