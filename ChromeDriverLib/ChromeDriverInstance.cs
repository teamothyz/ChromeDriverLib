using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools;
using SeleniumUndetectedChromeDriver;
using System;
using System.Diagnostics;
using System.Drawing;

namespace ChromeDriverLib
{
    public class ChromeDriverInstance
    {
        private static readonly object _lockUserDir = new();
        private static readonly object _lockChrome = new();
        public static readonly string HttpPrefix = "http://";
        public static readonly string HttpsPrefix = "https://";
        public static readonly string Socks5Prefix = "socks5://";
        private static readonly string _defaultUserDir = "Default";

        private static readonly Random _random = new();
        private static readonly List<string> timeZones;
        private static readonly string _timezonesStr = "Pacific/Midway,Pacific/Niue,Pacific/Pago_Pago," +
            "America/Adak,Pacific/Honolulu,Pacific/Rarotonga,Pacific/Tahiti,Pacific/Marquesas," +
            "America/Anchorage,America/Juneau,America/Metlakatla,America/Nome,America/Sitka,America/Yakutat," +
            "Pacific/Gambier,America/Los_Angeles,America/Tijuana,America/Vancouver,Pacific/Pitcairn," +
            "America/Boise,America/Cambridge_Bay,America/Ciudad_Juarez,America/Creston,America/Dawson," +
            "America/Dawson_Creek,America/Denver,America/Edmonton,America/Fort_Nelson,America/Hermosillo," +
            "America/Inuvik,America/Mazatlan,America/Phoenix,America/Whitehorse,America/Yellowknife," +
            "America/Bahia_Banderas,America/Belize,America/Chicago,America/Chihuahua,America/Costa_Rica," +
            "America/El_Salvador,America/Guatemala,America/Indiana/Knox,America/Indiana/Tell_City," +
            "America/Managua,America/Matamoros,America/Menominee,America/Merida,America/Mexico_City," +
            "America/Monterrey,America/North_Dakota/Beulah,America/North_Dakota/Center," +
            "America/North_Dakota/New_Salem,America/Ojinaga,America/Rankin_Inlet,America/Regina," +
            "America/Resolute,America/Swift_Current,America/Tegucigalpa,America/Winnipeg,Pacific/Easter," +
            "Pacific/Galapagos,America/Atikokan,America/Bogota,America/Cancun,America/Cayman," +
            "America/Detroit,America/Eirunepe,America/Grand_Turk,America/Guayaquil,America/Havana," +
            "America/Indiana/Indianapolis,America/Indiana/Marengo,America/Indiana/Petersburg," +
            "America/Indiana/Vevay,America/Indiana/Vincennes,America/Indiana/Winamac,America/Iqaluit," +
            "America/Jamaica,America/Kentucky/Louisville,America/Kentucky/Monticello,America/Lima," +
            "America/Nassau,America/New_York,America/Panama,America/Port-au-Prince,America/Rio_Branco," +
            "America/Toronto,America/Anguilla,America/Antigua,America/Aruba,America/Asuncion,America/Barbados," +
            "America/Blanc-Sablon,America/Boa_Vista,America/Campo_Grande,America/Caracas,America/Cuiaba," +
            "America/Curacao,America/Dominica,America/Glace_Bay,America/Goose_Bay,America/Grenada," +
            "America/Guadeloupe,America/Guyana,America/Halifax,America/Kralendijk,America/La_Paz," +
            "America/Lower_Princes,America/Manaus,America/Marigot,America/Martinique,America/Moncton," +
            "America/Montserrat,America/Porto_Velho,America/Port_of_Spain,America/Puerto_Rico,America/Santiago," +
            "America/Santo_Domingo,America/St_Barthelemy,America/St_Kitts,America/St_Lucia,America/St_Thomas," +
            "America/St_Vincent,America/Thule,America/Tortola,Atlantic/Bermuda,America/St_Johns," +
            "America/Araguaina,America/Argentina/Buenos_Aires,America/Argentina/Catamarca,America/Argentina/Cordoba," +
            "America/Argentina/Jujuy,America/Argentina/La_Rioja,America/Argentina/Mendoza,America/Argentina/Rio_Gallegos," +
            "America/Argentina/Salta,America/Argentina/San_Juan,America/Argentina/San_Luis,America/Argentina/Tucuman," +
            "America/Argentina/Ushuaia,America/Bahia,America/Belem,America/Cayenne,America/Fortaleza,America/Maceio," +
            "America/Miquelon,America/Montevideo,America/Nuuk,America/Paramaribo,America/Punta_Arenas,America/Recife," +
            "America/Santarem,America/Sao_Paulo,Antarctica/Palmer,Antarctica/Rothera,Atlantic/Stanley,America/Noronha," +
            "Atlantic/South_Georgia,America/Scoresbysund,Atlantic/Azores,Atlantic/Cape_Verde,Africa/Abidjan,Africa/Accra," +
            "Africa/Bamako,Africa/Banjul,Africa/Bissau,Africa/Casablanca,Africa/Conakry,Africa/Dakar,Africa/El_Aaiun," +
            "Africa/Freetown,Africa/Lome,Africa/Monrovia,Africa/Nouakchott,Africa/Ouagadougou,Africa/Sao_Tome," +
            "America/Danmarkshavn,Antarctica/Troll,Atlantic/Canary,Atlantic/Faroe,Atlantic/Madeira,Atlantic/Reykjavik," +
            "Atlantic/St_Helena,Europe/Dublin,Europe/Guernsey,Europe/Isle_of_Man,Europe/Jersey,Europe/Lisbon,Europe/London," +
            "Africa/Algiers,Africa/Bangui,Africa/Brazzaville,Africa/Ceuta,Africa/Douala,Africa/Kinshasa,Africa/Lagos," +
            "Africa/Libreville,Africa/Luanda,Africa/Malabo,Africa/Ndjamena,Africa/Niamey,Africa/Porto-Novo,Africa/Tunis," +
            "Africa/Windhoek,Arctic/Longyearbyen,Europe/Amsterdam,Europe/Andorra,Europe/Belgrade,Europe/Berlin," +
            "Europe/Bratislava,Europe/Brussels,Europe/Budapest,Europe/Copenhagen,Europe/Gibraltar,Europe/Ljubljana," +
            "Europe/Luxembourg,Europe/Madrid,Europe/Malta,Europe/Monaco,Europe/Oslo,Europe/Paris,Europe/Podgorica," +
            "Europe/Prague,Europe/Rome,Europe/San_Marino,Europe/Sarajevo,Europe/Skopje,Europe/Stockholm,Europe/Tirane," +
            "Europe/Vaduz,Europe/Vatican,Europe/Vienna,Europe/Warsaw,Europe/Zagreb,Europe/Zurich,Africa/Blantyre," +
            "Africa/Bujumbura,Africa/Cairo,Africa/Gaborone,Africa/Harare,Africa/Johannesburg,Africa/Juba,Africa/Khartoum," +
            "Africa/Kigali,Africa/Lubumbashi,Africa/Lusaka,Africa/Maputo,Africa/Maseru,Africa/Mbabane,Africa/Tripoli," +
            "Asia/Beirut,Asia/Famagusta,Asia/Gaza,Asia/Hebron,Asia/Jerusalem,Asia/Nicosia,Europe/Athens,Europe/Bucharest," +
            "Europe/Chisinau,Europe/Helsinki,Europe/Kaliningrad,Europe/Kyiv,Europe/Mariehamn,Europe/Riga,Europe/Sofia," +
            "Europe/Tallinn,Europe/Vilnius,Africa/Addis_Ababa,Africa/Asmara,Africa/Dar_es_Salaam,Africa/Djibouti," +
            "Africa/Kampala,Africa/Mogadishu,Africa/Nairobi,Antarctica/Syowa,Asia/Aden,Asia/Amman,Asia/Baghdad,Asia/Bahrain," +
            "Asia/Damascus,Asia/Kuwait,Asia/Qatar,Asia/Riyadh,Europe/Istanbul,Europe/Kirov,Europe/Minsk,Europe/Moscow," +
            "Europe/Simferopol,Europe/Volgograd,Indian/Antananarivo,Indian/Comoro,Indian/Mayotte,Asia/Tehran,Asia/Baku," +
            "Asia/Dubai,Asia/Muscat,Asia/Tbilisi,Asia/Yerevan,Europe/Astrakhan,Europe/Samara,Europe/Saratov,Europe/Ulyanovsk," +
            "Indian/Mahe,Indian/Mauritius,Indian/Reunion,Asia/Kabul,Antarctica/Mawson,Asia/Aqtau,Asia/Aqtobe,Asia/Ashgabat," +
            "Asia/Atyrau,Asia/Dushanbe,Asia/Karachi,Asia/Oral,Asia/Qyzylorda,Asia/Samarkand,Asia/Tashkent,Asia/Yekaterinburg," +
            "Indian/Kerguelen,Indian/Maldives,Asia/Colombo,Asia/Kolkata,Asia/Kathmandu,Antarctica/Vostok,Asia/Almaty," +
            "Asia/Bishkek,Asia/Dhaka,Asia/Omsk,Asia/Qostanay,Asia/Thimphu,Asia/Urumqi,Indian/Chagos,Asia/Yangon,Indian/Cocos," +
            "Antarctica/Davis,Asia/Bangkok,Asia/Barnaul,Asia/Hovd,Asia/Ho_Chi_Minh,Asia/Jakarta,Asia/Krasnoyarsk," +
            "Asia/Novokuznetsk,Asia/Novosibirsk,Asia/Phnom_Penh,Asia/Pontianak,Asia/Tomsk,Asia/Vientiane,Indian/Christmas," +
            "Asia/Brunei,Asia/Choibalsan,Asia/Hong_Kong,Asia/Irkutsk,Asia/Kuala_Lumpur,Asia/Kuching,Asia/Macau,Asia/Makassar," +
            "Asia/Manila,Asia/Shanghai,Asia/Singapore,Asia/Taipei,Asia/Ulaanbaatar,Australia/Perth,Australia/Eucla,Asia/Chita," +
            "Asia/Dili,Asia/Jayapura,Asia/Khandyga,Asia/Pyongyang,Asia/Seoul,Asia/Tokyo,Asia/Yakutsk,Pacific/Palau," +
            "Australia/Adelaide,Australia/Broken_Hill,Australia/Darwin,Antarctica/DumontDUrville,Antarctica/Macquarie," +
            "Asia/Ust-Nera,Asia/Vladivostok,Australia/Brisbane,Australia/Hobart,Australia/Lindeman,Australia/Melbourne," +
            "Australia/Sydney,Pacific/Chuuk,Pacific/Guam,Pacific/Port_Moresby,Pacific/Saipan,Australia/Lord_Howe," +
            "Antarctica/Casey,Asia/Magadan,Asia/Sakhalin,Asia/Srednekolymsk,Pacific/Bougainville,Pacific/Efate," +
            "Pacific/Guadalcanal,Pacific/Kosrae,Pacific/Norfolk,Pacific/Noumea,Pacific/Pohnpei,Antarctica/McMurdo," +
            "Asia/Anadyr,Asia/Kamchatka,Pacific/Auckland,Pacific/Fiji,Pacific/Funafuti,Pacific/Kwajalein,Pacific/Majuro," +
            "Pacific/Nauru,Pacific/Tarawa,Pacific/Wake,Pacific/Wallis,Pacific/Chatham,Pacific/Apia,Pacific/Fakaofo," +
            "Pacific/Kanton,Pacific/Tongatapu,Pacific/Kiritimati";

        static ChromeDriverInstance()
        {
            timeZones = _timezonesStr.Split(',')
                .Where(s => !string.IsNullOrWhiteSpace(s.Trim()))
                .Select(s => s.Trim()).ToList();
        }

        public static MyChromeDriver GetInstance(int positionX,
            int positionY,
            bool isMaximize = false,
            string? proxy = null,
            bool isHeadless = true,
            List<string>? extensionPaths = null,
            bool disableImg = true,
            bool privateMode = true,
            string? userDataDir = null,
            string? profile = null,
            bool isDeleteProfile = true,
            bool keepOneWindow = false,
            int? height = null,
            int? width = null,
            Dictionary<string, object>? prefs = null,
            CancellationToken? token = null)
        {
            MyChromeDriver myDriver = new();
            try
            {
                if (userDataDir == null)
                {
                    myDriver.ProfileDir = GetUserDir();
                    userDataDir = myDriver.ProfileDir;
                    myDriver.IsDeleteProfile = isDeleteProfile;
                }
                else
                {
                    myDriver.ProfileDir = Path.Combine(userDataDir, profile ?? _defaultUserDir);
                    myDriver.IsDeleteProfile = isDeleteProfile;
                }
                token ??= CancellationToken.None;

                var options = new ChromeOptions();
                var proxyInfo = new List<string>();
                if (!string.IsNullOrEmpty(proxy))
                {
                    var prefix = string.Empty;
                    if (proxy.Contains(HttpPrefix) || proxy.Contains(HttpsPrefix))
                    {
                        proxy = proxy.Replace(HttpPrefix, "").Replace(HttpsPrefix, "");
                        prefix = HttpPrefix;
                    }
                    else if (proxy.Contains(Socks5Prefix))
                    {
                        proxy = proxy.Replace(Socks5Prefix, "");
                        prefix = Socks5Prefix;
                    }
                    else throw new Exception("unsupported proxy type");

                    proxyInfo = [.. proxy.Split(":")];
                    if (proxyInfo.Count != 2 && proxyInfo.Count != 4) throw new Exception("invalid proxy format");
                    options.AddArgument($"--proxy-server={prefix}{proxyInfo[0]}:{proxyInfo[1]}");
                }

                var extensions = new List<string>();
                extensions.AddRange(extensionPaths ?? []);
                if (extensions.Count > 0) options.AddArguments($"--load-extension={string.Join(",", extensions)}");

                if (privateMode) options.AddArgument("--incognito");
                if (disableImg) options.AddArgument("--blink-settings=imagesEnabled=false");
                if (!string.IsNullOrWhiteSpace(profile)) options.AddArgument($"--profile-directory={profile}");

                var chromeDriverPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "chromedriver.exe");
                lock (_lockChrome)
                {
                    myDriver.Driver = UndetectedChromeDriver.Create(userDataDir: userDataDir,
                    driverExecutablePath: chromeDriverPath,
                    headless: isHeadless,
                    hideCommandPromptWindow: true,
                    prefs: prefs,
                    options: options);
                }

                if (!isMaximize)
                {
                    myDriver.Driver.Manage().Window.Position = new Point(positionX, positionY);
                    myDriver.Driver.Manage().Window.Size = new Size(width ?? 300, height ?? 300);
                }
                else
                {
                    myDriver.Driver.Manage().Window.Maximize();
                }
                Thread.Sleep(1000);
                if (proxyInfo.Count == 4)
                {
                    myDriver.Driver.GetDevToolsSession(new DevToolsOptions
                    {
                        ProtocolVersion = 101
                    });
                    var networkManager = new NetworkManager(myDriver.Driver);
                    networkManager.AddAuthenticationHandler(new NetworkAuthenticationHandler
                    {
                        Credentials = new PasswordCredentials(proxyInfo[2], proxyInfo[3]),
                        UriMatcher = _ => true
                    });
                    networkManager.StartMonitoring().Wait(token.Value);
                }

                if (keepOneWindow)
                {
                    while (myDriver.Driver.WindowHandles.Count > 1)
                    {
                        myDriver.Driver.Close();
                        Thread.Sleep(1000);
                        myDriver.Driver.SwitchTo().Window(myDriver.Driver.WindowHandles.First());
                    }
                }
            }
            catch (Exception)
            {
                Serilog.Log.Error("LỖI HỆ THỐNG. HÃY KIỂM TRA CẬP NHẬT");
                Close(myDriver).Wait();
                throw new Exception("LỖI HỆ THỐNG. HÃY KIỂM TRA CẬP NHẬT");
            }
            return myDriver;
        }

        public static void ChangeTimeZone(MyChromeDriver myDriver)
        {
            myDriver.Driver.ExecuteCdpCommand("Emulation.setTimezoneOverride", new Dictionary<string, object>
            {
                { "timezoneId", timeZones[_random.Next(timeZones.Count)] }
            });
        }

        private static string GetUserDir()
        {
            var folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "profiles");
            var container = Path.Combine(folder, Guid.NewGuid().ToString());
            if (!Directory.Exists(folder))
            {
                lock (_lockUserDir)
                {
                    if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                }
            }
            Directory.CreateDirectory(container);
            return container;
        }

        public static async Task Close(MyChromeDriver myDriver)
        {
            if (myDriver == null) return;
            try
            {
                myDriver.Driver?.Close();
                await Task.Delay(1000).ConfigureAwait(false);
                myDriver.Driver?.Quit();
                await Task.Delay(1000).ConfigureAwait(false);
                myDriver.Driver?.Dispose();
                await Task.Delay(1000).ConfigureAwait(false);
            }
            catch { }

            if (myDriver.IsDeleteProfile && Directory.Exists(myDriver.ProfileDir))
            {
                try
                {
                    Directory.Delete(myDriver.ProfileDir, true);
                }
                catch { }
            }
        }

        public static void KillAllChromes(bool deleteTempFolder = true)
        {
            try
            {
                string cmdScript = @"@echo off
                    setlocal
                    echo Killing all chrome.exe processes...
                    taskkill /F /IM chrome.exe /T >nul
                    echo Killing all chromedriver.exe processes...
                    taskkill /F /IM chromedriver.exe /T >nul
                    echo Processes killed successfully.
                    endlocal";

                var psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    RedirectStandardInput = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };

                var p = Process.Start(psi);
                p?.StandardInput.WriteLine(cmdScript);
                p?.StandardInput.Flush();
                p?.StandardInput.Close();
                p?.WaitForExit(10000);
            }
            catch { }

            if (deleteTempFolder)
            {
                var folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "profiles");
                try
                {
                    if (!Directory.Exists(folder)) return;
                    Directory.Delete(folder, true);
                }
                catch { }
            }
        }
    }
}
