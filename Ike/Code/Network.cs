using System.Net.Sockets;
using System.Net;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Ike
{
	/// <summary>
	/// 网络相关操作
	/// </summary>
	public class Network
	{
		#region 浏览器Agents
		/// <summary>
		/// 浏览器Agents
		/// </summary>
		private readonly static string[] agents = new string[] {
		 "Mozilla/4.0 (compatible; MSIE 7.0; windows NT 5.1; NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022; Infopath.2)",
		 "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705",
		"Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.1 (KHTML, like Gecko) Chrome/22.0.1207.1 Safari/537.1",
		"Mozilla/5.0 (X11; CrOS i686 2268.111.0) AppleWebKit/536.11 (KHTML, like Gecko) Chrome/20.0.1132.57 Safari/536.11",
		"Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/536.6 (KHTML, like Gecko) Chrome/20.0.1092.0 Safari/536.6",
		"Mozilla/5.0 (Windows NT 6.2) AppleWebKit/536.6 (KHTML, like Gecko) Chrome/20.0.1090.0 Safari/536.6",
		"Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.1 (KHTML, like Gecko) Chrome/19.77.34.5 Safari/537.1",
		"Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/536.5 (KHTML, like Gecko) Chrome/19.0.1084.9 Safari/536.5",
		"Mozilla/5.0 (Windows NT 6.0) AppleWebKit/536.5 (KHTML, like Gecko) Chrome/19.0.1084.36 Safari/536.5",
		"Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/536.3 (KHTML, like Gecko) Chrome/19.0.1063.0 Safari/536.3",
		"Mozilla/5.0 (Windows NT 5.1) AppleWebKit/536.3 (KHTML, like Gecko) Chrome/19.0.1063.0 Safari/536.3",
		"Mozilla/5.0 (Macintosh; Intel Mac OS X 10_8_0) AppleWebKit/536.3 (KHTML, like Gecko) Chrome/19.0.1063.0 Safari/536.3",
		"Mozilla/5.0 (Windows NT 6.2) AppleWebKit/536.3 (KHTML, like Gecko) Chrome/19.0.1062.0 Safari/536.3",
		"Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/536.3 (KHTML, like Gecko) Chrome/19.0.1062.0 Safari/536.3",
		"Mozilla/5.0 (Windows NT 6.2) AppleWebKit/536.3 (KHTML, like Gecko) Chrome/19.0.1061.1 Safari/536.3",
		"Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/536.3 (KHTML, like Gecko) Chrome/19.0.1061.1 Safari/536.3",
		"Mozilla/5.0 (Windows NT 6.1) AppleWebKit/536.3 (KHTML, like Gecko) Chrome/19.0.1061.1 Safari/536.3",
		"Mozilla/5.0 (Windows NT 6.2) AppleWebKit/536.3 (KHTML, like Gecko) Chrome/19.0.1061.0 Safari/536.3",
		"Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/535.24 (KHTML, like Gecko) Chrome/19.0.1055.1 Safari/535.24",
		"Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/535.24 (KHTML, like Gecko) Chrome/19.0.1055.1 Safari/535.24",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/532.5 (KHTML, like Gecko) Chrome/4.0.249.0 Safari/532.5",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/534.14 (KHTML, like Gecko) Chrome/10.0.601.0 Safari/534.14 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/534.20 (KHTML, like Gecko) Chrome/11.0.672.2 Safari/534.20 ",
		"Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/534.27 (KHTML, like Gecko) Chrome/12.0.712.0 Safari/534.27 ",
		"Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/13.0.782.24 Safari/535.1 ",
		"Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.7 (KHTML, like Gecko) Chrome/16.0.912.36 Safari/535.7 ",
		"Mozilla/5.0 (Windows NT 6.1; rv:2.0.1) Gecko/20100101 Firefox/4.0.1 ",
		"Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:2.0.1) Gecko/20100101 Firefox/4.0.1 ",
		"Mozilla/5.0 (Windows NT 6.1; WOW64; rv:6.0a2) Gecko/20110622 Firefox/6.0a2 ",
		"Mozilla/5.0 (Windows NT 6.1; WOW64; rv:7.0.1) Gecko/20100101 Firefox/7.0.1 ",
		"Mozilla/5.0 (Windows NT 6.1; WOW64; rv:2.0b4pre) Gecko/20100815 Minefield/4.0b4pre ",
		"Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.8 (KHTML, like Gecko) Beamrise/17.2.0.9 Chrome/17.0.939.0 Safari/535.8 ",
		"Mozilla/5.0 (Windows NT 6.1) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/18.6.872.0 Safari/535.2 UNTRUSTED/1.0 3gpp-gba UNTRUSTED/1.0 ",
		"Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/536.6 (KHTML, like Gecko) Chrome/20.0.1092.0 Safari/536.6 ",
		"Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.1 (KHTML, like Gecko) Chrome/22.0.1207.1 Safari/537.1 ",
		"Mozilla/5.0 (Windows NT 6.1; WOW64; rv:10.0.1) Gecko/20100101 Firefox/10.0.1 ",
		"Mozilla/5.0 (Windows NT 6.1; rv:12.0) Gecko/20120403211507 Firefox/12.0 ",
		"Mozilla/5.0 (Windows NT 6.1; WOW64; rv:15.0) Gecko/20120427 Firefox/15.0a1 ",
		"Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; Trident/4.0) ",
		"Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0) ",
		"Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; WOW64; Trident/6.0) ",
		"Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; Trident/6.0) ",
		"Mozilla/5.0 (compatible; MSIE 10.6; Windows NT 6.1; Trident/5.0; InfoPath.2; SLCC1; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729; .NET CLR 2.0.50727) 3gpp-gba UNTRUSTED/1.0 ",
		"Opera/9.80 (Windows NT 6.1; U; en) Presto/2.7.62 Version/11.01 " ,
		"Opera/9.80 (Windows NT 6.1; U; es-ES) Presto/2.9.181 Version/12.00 " ,
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/533.19.4 (KHTML, like Gecko) Version/5.0.2 Safari/533.18.5 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-GB; rv:1.9.1.17) Gecko/20110123 (like Firefox/3.x) SeaMonkey/2.0.12 ",
		"Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0) ",
		"Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0; Sleipnir/2.9.8) ",
		"Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; Trident/6.0) ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/534.7 (KHTML, like Gecko) Chrome/7.0.517.43 Safari/534.7 ",
		"Mozilla/5.0 (Windows NT 6.1) AppleWebKit/534.24 (KHTML, like Gecko) Chrome/11.0.696.65 Safari/534.24 ",
		"Mozilla/5.0 (Windows NT 6.1) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/13.0.782.107 Safari/535.1 ",
		"Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/14.0.835.202 Safari/535.1 ",
		"Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.7 (KHTML, like Gecko) Chrome/16.0.912.75 Safari/535.7 ",
		"Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; MAGW; .NET4.0C; Lunascape 6.5.8.24780) ",
		"Mozilla/5.0 (Windows NT 6.1; WOW64; rv:7.0a1) Gecko/20110623 Firefox/7.0a1 Fennec/7.0a1 ",
		"Mozilla/5.0 (Windows NT 6.1; WOW64; rv:2.2a1pre) Gecko/20110331 Firefox/4.2a1pre Fennec/4.1a1pre ",
		"Mozilla/5.0 (Windows NT 6.1; WOW64; rv:2.0b8) Gecko/20101221 Firefox/4.0b8 Fennec/4.0b3 ",
		"Mozilla/5.0 (Windows NT 6.1; WOW64; rv:2.1.1) Gecko/20110415 Firefox/4.0.2pre Fennec/4.0.1 ",
		"Mozilla/5.0 (Windows NT 6.1; rv:2.1.1) Gecko/20110415 Firefox/4.0.2pre Fennec/4.0.1 ",
		"Mozilla/5.0 (Windows NT 6.1; WOW64; rv:2.1) Gecko/20110318 Firefox/4.0b13pre Fennec/4.0 ",
		"Mozilla/5.0 (Windows NT 6.1; rv:2.1) Gecko/20110318 Firefox/4.0b13pre Fennec/4.0 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US; rv:1.9.2a1pre) Gecko/20090317 Fennec/1.0b1 ",
		"Opera/9.80 (J2ME/MIDP; Opera Mini/5.0 (Windows; U; Windows NT 6.1; sv-SE) AppleWebKit/23.411; U; en) Presto/2.5.25 Version/10.54 " ,
		"Opera/9.80 (J2ME/MIDP; Opera Mini/5.0 (Windows; U; Windows NT 6.1; rv:2.2) Gecko/24.838; U; id) Presto/2.5.25 Version/10.54 " ,
		"Opera/9.80 (J2ME/MIDP; Opera Mini/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/23.411; U; en) Presto/2.5.25 Version/10.54 " ,
		"Opera/9.80 (J2ME/MIDP; Opera Mini/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/22.478; U; en) Presto/2.5.25 Version/10.54 " ,
		"Opera/9.80 (J2ME/MIDP; Opera Mini/5.0 (Windows NT 6.1; WOW64) AppleWebKit/23.411; U; en) Presto/2.5.25 Version/10.54 " ,
		"Opera/9.80 (J2ME/MIDP; Opera Mini/4.3.24214 (Windows; U; Windows NT 6.1) AppleWebKit/24.838; U; id) Presto/2.5.25 Version/10.54 " ,
		"Opera/9.80 (Windows NT 6.1; Opera Mobi/49; U; en) Presto/2.4.18 Version/10.00 ",
		"Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.1623.0 Safari/537.36 ",
		"Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/29.0.1547.62 Safari/537.36 ",
		"Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/28.0.1468.0 Safari/537.36 ",
		"Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.93 Safari/537.36 ",
		"Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.93 Safari/537.36 ",
		"Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.17 (KHTML, like Gecko) Chrome/24.0.1312.60 Safari/537.17 ",
		"Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.13 (KHTML, like Gecko) Chrome/24.0.1284.0 Safari/537.13 ",
		"Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.2 (KHTML, like Gecko) Chrome/22.0.1216.0 Safari/537.2 ",
		"Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/536.3 (KHTML, like Gecko) Chrome/19.0.1063.0 Safari/536.3 ",
		"Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/536.3 (KHTML, like Gecko) Chrome/19.0.1062.0 Safari/536.3 ",
		"Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/536.3 (KHTML, like Gecko) Chrome/19.0.1061.1 Safari/536.3 ",
		"Mozilla/5.0 (Windows NT 6.1) AppleWebKit/536.3 (KHTML, like Gecko) Chrome/19.0.1061.1 Safari/536.3 ",
		"Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.11 (KHTML, like Gecko) Chrome/17.0.963.66 Safari/535.11 ",
		"Mozilla/5.0 (Windows NT 6.1) AppleWebKit/535.11 (KHTML, like Gecko) Chrome/17.0.963.66 Safari/535.11 ",
		"Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.11 (KHTML, like Gecko) Chrome/17.0.963.56 Safari/535.11 ",
		"Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.8 (KHTML, like Gecko) Chrome/17.0.940.0 Safari/535.8 ",
		"Mozilla/5.0 (Windows NT 6.1) AppleWebKit/535.7 (KHTML, like Gecko) Chrome/16.0.912.77 Safari/535.7ad-imcjapan-syosyaman-xkgi3lqg03!wgz ",
		"Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.7 (KHTML, like Gecko) Chrome/16.0.912.63 Safari/535.7xs5D9rRDFpg2g ",
		"Mozilla/5.0 (Windows NT 6.1) AppleWebKit/535.8 (KHTML, like Gecko) Chrome/16.0.912.63 Safari/535.8 ",
		"Mozilla/5.0 (Windows NT 6.1) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.861.0 Safari/535.2 ",
		"Mozilla/5.0 (Windows NT 6.1) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/14.0.815.10913 Safari/535.1 ",
		"Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/14.0.814.0 Safari/535.1 ",
		"Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/14.0.813.0 Safari/535.1 ",
		"Mozilla/5.0 (Windows NT 6.1) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/14.0.812.0 Safari/535.1 ",
		"Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/14.0.811.0 Safari/535.1 ",
		"Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/14.0.810.0 Safari/535.1 ",
		"Mozilla/5.0 (Windows NT 6.1) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/14.0.801.0 Safari/535.1 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1) AppleWebKit/526.3 (KHTML, like Gecko) Chrome/14.0.564.21 Safari/526.3 ",
		"Mozilla/5.0 (Windows NT 6.1) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/13.0.782.215 Safari/535.1 ",
		"Mozilla/5.0 (Windows NT 6.1; en-US) AppleWebKit/534.30 (KHTML, like Gecko) Chrome/12.0.750.0 Safari/534.30 ",
		"Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/534.30 (KHTML, like Gecko) Chrome/12.0.742.53 Safari/534.30 ",
		"Mozilla/5.0 (Windows NT 6.1) AppleWebKit/534.30 (KHTML, like Gecko) Chrome/12.0.742.113 Safari/534.30 ",
		"Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/534.24 (KHTML, like Gecko) Chrome/12.0.702.0 Safari/534.24 ",
		"Mozilla/5.0 (Windows NT 6.1) AppleWebKit/534.24 (KHTML, like Gecko) Chrome/12.0.702.0 Safari/534.24 ",
		"Mozilla/5.0 (Windows NT 6.1) AppleWebKit/534.24 (KHTML, like Gecko) Chrome/11.0.699.0 Safari/534.24 ",
		"Mozilla/5.0 (Windows NT 6.1) AppleWebKit/534.24 (KHTML, like Gecko) Chrome/11.0.697.0 Safari/534.24 ",
		"Mozilla/5.0 (Windows NT 6.1) AppleWebKit/534.24 (KHTML, like Gecko) Chrome/11.0.696.68 Safari/534.24 ",
		"Mozilla/5.0 (Windows NT 6.1) AppleWebKit/534.24 (KHTML, like Gecko) Chrome/11.0.696.3 Safari/534.24 ",
		"Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/534.24 (KHTML, like Gecko) Chrome/11.0.696.12 Safari/534.24 ",
		"Mozilla/5.0 (Windows NT 6.1) AppleWebKit/534.24 (KHTML, like Gecko) Chrome/11.0.694.0 Safari/534.24 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/534.20 (KHTML, like Gecko) Chrome/11.0.669.0 Safari/534.20 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/534.17 (KHTML, like Gecko) Chrome/11.0.655.0 Safari/534.17 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/534.17 (KHTML, like Gecko) Chrome/11.0.654.0 Safari/534.17 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/534.17 (KHTML, like Gecko) Chrome/10.0.649.0 Safari/534.17 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; de-DE) AppleWebKit/534.17 (KHTML, like Gecko) Chrome/10.0.649.0 Safari/534.17 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/534.16 (KHTML, like Gecko) Chrome/10.0.648.134 Safari/534.16 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; ru-RU; AppleWebKit/534.16; KHTML; like Gecko; Chrome/10.0.648.11;Safari/534.16) ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; ru-RU) AppleWebKit/534.16 (KHTML, like Gecko) Chrome/10.0.648.11 Safari/534.16 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/534.16 (KHTML, like Gecko) Chrome/10.0.648.11 Safari/534.16 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/534.16 (KHTML, like Gecko) Chrome/10.0.638.0 Safari/534.16 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/534.13 (KHTML, like Gecko) Chrome/9.0.597.19 Safari/534.13 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/534.13 (KHTML, like Gecko) Chrome/9.0.597.0 Safari/534.13 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/534.13 (KHTML, like Gecko) Chrome/9.0.596.0 Safari/534.13 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; de-DE) AppleWebKit/534.10 (KHTML, like Gecko) Chrome/8.0.552.224 Safari/534.10 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/534.10 (KHTML, like Gecko) Chrome/8.0.552.215 Safari/534.10 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/534.10 (KHTML, like Gecko) Chrome/7.0.540.0 Safari/534.10 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; de-DE) AppleWebKit/534.10 (KHTML, like Gecko) Chrome/7.0.540.0 Safari/534.10 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/534.6 (KHTML, like Gecko) Chrome/7.0.498.0 Safari/534.6 ",
		"Mozilla/5.0 (ipad Windows; U; Windows NT 6.1; en-US) AppleWebKit/534.6 (KHTML, like Gecko) Chrome/7.0.498.0 Safari/534.6 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/534.3 (KHTML, like Gecko) Chrome/6.0.464.0 Safari/534.3 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/534.3 (KHTML, like Gecko) Chrome/6.0.461.0 Safari/534.3 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/534.3 (KHTML, like Gecko) Chrome/6.0.460.0 Safari/534.3 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/534.3 (KHTML, like Gecko) Chrome/6.0.459.0 Safari/534.3 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/534.3 (KHTML, like Gecko) Chrome/6.0.458.1 Safari/534.3 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/534.2 (KHTML, like Gecko) Chrome/6.0.454.0 Safari/534.2 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/534.1 (KHTML, like Gecko) Chrome/6.0.428.0 Safari/534.1 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-GB) AppleWebKit/534.1 (KHTML, like Gecko) Chrome/6.0.428.0 Safari/534.1 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/533.9 (KHTML, like Gecko) Chrome/6.0.400.0 Safari/533.9 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/533.2 (KHTML, like Gecko) Chrome/6.0 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/533.4 (KHTML, like Gecko) Chrome/5.0.375.999 Safari/533.4 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/533.4 (KHTML, like Gecko) Chrome/5.0.370.0 Safari/533.4 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/533.3 (KHTML, like Gecko) Chrome/5.0.354.0 Safari/533.3 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/533.2 (KHTML, like Gecko) Chrome/5.0.342.3 Safari/533.2 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/532.9 (KHTML, like Gecko) Chrome/5.0.307.1 Safari/532.9 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/532.5 (KHTML, like Gecko) Chrome/4.1.249.1025 Safari/532.5 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; it-IT) AppleWebKit/532.5 (KHTML, like Gecko) Chrome/4.0.249.25 Safari/532.5 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/532.5 (KHTML, like Gecko) Chrome/4.0.246.0 Safari/532.5 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/532.3 (KHTML, like Gecko) Chrome/4.0.227.0 Safari/532.3 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/532.3 (KHTML, like Gecko) Chrome/4.0.223.5 Safari/532.3 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/532.2 (KHTML, like Gecko) Chrome/4.0.223.1 Safari/532.2 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/532.2 (KHTML, like Gecko) Chrome/4.0.222.3 Safari/532.2 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/532.2 (KHTML, like Gecko) Chrome/4.0.222.12 Safari/532.2 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/532.1 (KHTML, like Gecko) Chrome/4.0.213.1 Safari/532.1 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/532.0 (KHTML, like Gecko) Chrome/4.0.212.0 Safari/532.0 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/532.0 (KHTML, like Gecko) Chrome/4.0.211.4 Safari/532.0 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/532.0 (KHTML, like Gecko) Chrome/4.0.211.0 Safari/532.0 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/532.0 (KHTML, like Gecko) Chrome/4.0.208.0 Safari/532.0 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/532.0 (KHTML, like Gecko) Chrome/4.0.206.1 Safari/532.0 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/532.0 (KHTML, like Gecko) Chrome/4.0.206.0 Safari/532.0 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/532.0 (KHTML, like Gecko) Chrome/4.0.204.0 Safari/532.0 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/532.0 (KHTML, like Gecko) Chrome/4.0.203.2 Safari/532.0 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/532.0 (KHTML, like Gecko) Chrome/4.0.203.0 Safari/532.0 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/532.0 (KHTML, like Gecko) Chrome/4.0.202.0 Safari/532.0 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/532.0 (KHTML, like Gecko) Chrome/4.0.201.1 Safari/532.0 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/532.0 (KHTML, like Gecko) Chrome/3.0.197.11 Safari/532.0 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/532.0 (KHTML, like Gecko) Chrome/3.0.197.0 Safari/532.0 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/532.0 (KHTML, like Gecko) Chrome/3.0.196.2 Safari/532.0 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/532.0 (KHTML, like Gecko) Chrome/3.0.195.6 Safari/532.0 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/532.0 (KHTML, like Gecko) Chrome/3.0.195.4 Safari/532.0 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/532.0 (KHTML, like Gecko) Chrome/3.0.195.3 Safari/532.0 ",
		"Mozilla/6.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/532.0 (KHTML, like Gecko) Chrome/3.0.195.27 Safari/532.0 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/532.0 (KHTML, like Gecko) Chrome/3.0.195.27 Safari/532.0 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/532.0 (KHTML, like Gecko) Chrome/3.0.195.21 Safari/532.0 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/532.0 (KHTML, like Gecko) Chrome/3.0.195.10 Safari/532.0 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/532.0 (KHTML, like Gecko) Chrome/3.0.195.1 Safari/532.0 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/531.4 (KHTML, like Gecko) Chrome/3.0.194.0 Safari/531.4 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/531.3 (KHTML, like Gecko) Chrome/3.0.193.2 Safari/531.3 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/531.0 (KHTML, like Gecko) Chrome/3.0.191.0 Safari/531.0 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/531.0 (KHTML, like Gecko) Chrome/2.0.182.0 Safari/532.0 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/531.0 (KHTML, like Gecko) Chrome/2.0.182.0 Safari/531.0 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/530.0 (KHTML, like Gecko) Chrome/2.0.182.0 Safari/531.0 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/530.6 (KHTML, like Gecko) Chrome/2.0.174.0 Safari/530.6 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/530.5 (KHTML, like Gecko) Chrome/2.0.172.43 Safari/530.5 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/530.4 (KHTML, like Gecko) Chrome/2.0.172.0 Safari/530.4 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/528.8 (KHTML, like Gecko) Chrome/2.0.156.1 Safari/528.8 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/528.8 (KHTML, like Gecko) Chrome/1.0.156.0 Safari/528.8 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/525.19 (KHTML, like Gecko) Chrome/1.0.154.53 Safari/525.19 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/525.19 (KHTML, like Gecko) Chrome/1.0.154.43 Safari/525.19 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/525.19 (KHTML, like Gecko) Chrome/0.3.154.9 Safari/525.19",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/534.14 (KHTML, like Gecko) Chrome/9.0.601.0 Safari/534.14 ",
		"Mozilla/5.0 (Windows NT 6.0) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.120 Safari/535.2 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0 x64; en-US; rv:1.9pre) Gecko/2008072421 Minefield/3.0.2pre ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-GB; rv:1.9.0.11) Gecko/2009060215 Firefox/3.0.11 (.NET CLR 3.5.30729) ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US; rv:1.9.1.6) Gecko/20091201 Firefox/3.5.6 GTB5 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/527 (KHTML, like Gecko, Safari/419.3) Arora/0.6 (Change: ) ",
		"Mozilla/5.0 (Windows NT 6.0; rv:14.0) Gecko/20100101 Firefox/14.0.1 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/533.1 (KHTML, like Gecko) Maxthon/3.0.8.2 Safari/533.1 ",
		"Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.0) ",
		"Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.0; Trident/4.0) ",
		"Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.0; Trident/4.0) ",
		"Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.0; Trident/5.0) ",
		"Opera/9.25 (Windows NT 6.0; U; en) " ,
		"Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.0; SLCC1; .NET CLR 2.0.50727; Media Center PC 5.0; .NET CLR 3.0.04506) ",
		"Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.0; SLCC1; .NET CLR 2.0.50727; Media Center PC 5.0; .NET CLR 3.0.04506; InfoPath.1) ",
		"Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.0; Trident/5.0) ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/525.13 (KHTML, like Gecko) Chrome/0.2.149.27 Safari/525.13 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/532.0 (KHTML, like Gecko) Chrome/3.0.195.38 Safari/532.0 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/534.10 (KHTML, like Gecko) Chrome/8.0.552.224 Safari/534.10 ChromePlus/1.5.2.0 ",
		"Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.0; SLCC1; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30618; Lunascape 4.7.3) ",
		"Mozilla/5.0 (Windows NT 6.0; rv:2.1.1) Gecko/20110415 Firefox/4.0.2pre Fennec/4.0.1 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US; rv:1.9.2a1pre) Gecko/20081222 Fennec/1.0a2 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US; rv:1.9.1b2pre) Gecko/20081015 Fennec/1.0a1 ",
		"Opera/9.80 (Windows NT 6.0; Opera Mobi/49; U; en) Presto/2.4.18 Version/10.00 " ,
		"Mozilla/5.0 (Windows NT 6.0) AppleWebKit/536.5 (KHTML, like Gecko) Chrome/19.0.1084.36 Safari/536.5 ",
		"Mozilla/5.0 (Windows NT 6.0; WOW64) AppleWebKit/535.19 (KHTML, like Gecko) Chrome/18.0.1025.45 Safari/535.19 ",
		"Mozilla/5.0 (Windows NT 6.0; WOW64) AppleWebKit/535.11 (KHTML, like Gecko) Chrome/17.0.963.66 Safari/535.11 ",
		"Mozilla/5.0 (Windows NT 6.0) AppleWebKit/535.11 (KHTML, like Gecko) Chrome/17.0.963.66 Safari/535.11 ",
		"Mozilla/5.0 (Windows NT 6.0; WOW64) AppleWebKit/535.11 (KHTML, like Gecko) Chrome/17.0.963.56 Safari/535.11 ",
		"Mozilla/5.0 (Windows NT 6.0; WOW64) AppleWebKit/535.7 (KHTML, like Gecko) Chrome/16.0.912.75 Safari/535.7 ",
		"Mozilla/5.0 (Windows NT 6.0) AppleWebKit/535.7 (KHTML, like Gecko) Chrome/16.0.912.75 Safari/535.7 ",
		"Mozilla/5.0 (Windows NT 6.0; WOW64) AppleWebKit/535.7 (KHTML, like Gecko) Chrome/16.0.912.36 Safari/535.7 ",
		"Mozilla/5.0 (Windows NT 6.0) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/14.0.792.0 Safari/535.1 ",
		"Mozilla/5.0 (Windows NT 6.0; WOW64) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/13.0.782.41 Safari/535.1 ",
		"Mozilla/5.0 (Windows NT 6.0) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/13.0.782.41 Safari/535.1 ",
		"Mozilla/5.0 (Windows NT 6.0; WOW64) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/13.0.782.220 Safari/535.1 ",
		"Mozilla/5.0 (Windows NT 6.0) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/13.0.782.220 Safari/535.1 ",
		"Mozilla/5.0 (Windows NT 6.0) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/13.0.782.20 Safari/535.1 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/13.0.782.107 Safari/535.1 ",
		"Mozilla/5.0 (Windows NT 6.0) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/13.0.782.1 Safari/535.1 ",
		"Mozilla/5.0 (Windows NT 6.0) AppleWebKit/534.30 (KHTML, like Gecko) Chrome/12.0.742.100 Safari/534.30 ",
		"Mozilla/5.0 (Windows NT 6.0; WOW64) AppleWebKit/534.24 (KHTML, like Gecko) Chrome/11.0.699.0 Safari/534.24 ",
		"Mozilla/5.0 (Windows NT 6.0; WOW64) AppleWebKit/534.24 (KHTML, like Gecko) Chrome/11.0.696.34 Safari/534.24 ",
		"Mozilla/5.0 (Windows NT 6.0) AppleWebKit/534.24 (KHTML, like Gecko) Chrome/11.0.696.3 Safari/534.24 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/534.20 (KHTML, like Gecko) Chrome/11.0.672.2 Safari/534.20 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/534.16 (KHTML, like Gecko) Chrome/10.0.648.133 Safari/534.16 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/534.13 (KHTML, like Gecko) Chrome/9.0.597.0 Safari/534.13 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/533.3 (KHTML, like Gecko) Chrome/8.0.552.224 Safari/533.3 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/534.8 (KHTML, like Gecko) Chrome/7.0.521.0 Safari/534.8 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/534.3 (KHTML, like Gecko) Chrome/6.0.458.1 Safari/534.3 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/533.4 (KHTML, like Gecko) Chrome/5.0.375.127 Safari/533.4 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/533.2 (KHTML, like Gecko) Chrome/5.0.342.5 Safari/533.2 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/533.2 (KHTML, like Gecko) Chrome/5.0.342.1 Safari/533.2 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/532.4 (KHTML, like Gecko) Chrome/4.0.241.0 Safari/532.4 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/532.3 (KHTML, like Gecko) Chrome/4.0.224.2 Safari/532.3 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/532.2 (KHTML, like Gecko) Chrome/4.0.223.0 Safari/532.2 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/532.2 (KHTML, like Gecko) Chrome/4.0.222.12 Safari/532.2 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/532.2 (KHTML, like Gecko) Chrome/4.0.221.6 Safari/532.2 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/532.1 (KHTML, like Gecko) Chrome/4.0.220.1 Safari/532.1 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/532.1 (KHTML, like Gecko) Chrome/4.0.213.1 Safari/532.1 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/532.0 (KHTML, like Gecko) Chrome/4.0.211.7 Safari/532.0 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/532.0 (KHTML, like Gecko) Chrome/4.0.211.4 Safari/532.0 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/532.0 (KHTML, like Gecko) Chrome/4.0.211.2 Safari/532.0 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/532.0 (KHTML, like Gecko) Chrome/4.0.208.0 Safari/532.0 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/532.0 (KHTML, like Gecko) Chrome/4.0.207.0 Safari/532.0 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/532.0 (KHTML, like Gecko) Chrome/4.0.206.1 Safari/532.0 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/532.0 (KHTML, like Gecko) Chrome/4.0.203.2 Safari/532.0 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0 (x86_64); de-DE) AppleWebKit/532.0 (KHTML, like Gecko) Chrome/4.0.202.2 Safari/532.0 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/532.0 (KHTML, like Gecko) Chrome/4.0.202.0 Safari/532.0 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/532.0 (KHTML, like Gecko) Chrome/4.0.201.1 Safari/532.0 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/532.0 (KHTML, like Gecko) Chrome/3.0.198.0 Safari/532.0 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/532.0 (KHTML, like Gecko) Chrome/3.0.197.11 Safari/532.0 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/532.0 (KHTML, like Gecko) Chrome/3.0.196.2 Safari/532.0 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/532.0 (KHTML, like Gecko) Chrome/3.0.195.6 Safari/532.0 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/532.0 (KHTML, like Gecko) Chrome/3.0.195.3 Safari/532.0 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/532.0 (KHTML, like Gecko) Chrome/3.0.195.27 Safari/532.0 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/532.0 (KHTML, like Gecko) Chrome/3.0.195.21 Safari/532.0 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/532.0 (KHTML, like Gecko) Chrome/3.0.195.20 Safari/532.0 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/532.0 (KHTML, like Gecko) Chrome/3.0.195.17 Safari/532.0 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/532.0 (KHTML, like Gecko) Chrome/3.0.195.10 Safari/532.0 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/532.0 (KHTML, like Gecko) Chrome/3.0.195.1 Safari/532.0 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/531.3 (KHTML, like Gecko) Chrome/3.0.193.2 Safari/531.3 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/531.3 (KHTML, like Gecko) Chrome/3.0.193.0 Safari/531.3 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/530.7 (KHTML, like Gecko) Chrome/2.0.176.0 Safari/530.7 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/530.6 (KHTML, like Gecko) Chrome/2.0.174.0 Safari/530.6 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/530.5 (KHTML, like Gecko) Chrome/2.0.173.1 Safari/530.5 ",
		"Mozilla/6.0 (Windows; U; Windows NT 6.0; en-US) Gecko/2009032609 Chrome/2.0.172.6 Safari/530.7 ",
		"Mozilla/6.0 (Windows; U; Windows NT 6.0; en-US) Gecko/2009032609 (KHTML, like Gecko) Chrome/2.0.172.6 Safari/530.7 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/530.5 (KHTML, like Gecko) Chrome/2.0.172.6 Safari/530.5 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/530.5 (KHTML, like Gecko) Chrome/2.0.172.43 Safari/530.5 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/530.5 (KHTML, like Gecko) Chrome/2.0.172.40 Safari/530.5 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/530.5 (KHTML, like Gecko) Chrome/2.0.172.39 Safari/530.5 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/530.5 (KHTML, like Gecko) Chrome/2.0.172.23 Safari/530.5 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/530.5 (KHTML, like Gecko) Chrome/2.0.172.2 Safari/530.5 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/530.4 (KHTML, like Gecko) Chrome/2.0.171.0 Safari/530.4 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/530.1 (KHTML, like Gecko) Chrome/2.0.168.0 Safari/530.1 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/530.1 (KHTML, like Gecko) Chrome/2.0.164.0 Safari/530.1 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/530.0 (KHTML, like Gecko) Chrome/2.0.162.0 Safari/530.0 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/530.0 (KHTML, like Gecko) Chrome/2.0.160.0 Safari/530.0 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/528.10 (KHTML, like Gecko) Chrome/2.0.157.2 Safari/528.10 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/528.11 (KHTML, like Gecko) Chrome/2.0.157.0 Safari/528.11 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/528.8 (KHTML, like Gecko) Chrome/2.0.156.1 Safari/528.8 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/525.19 (KHTML, like Gecko) Chrome/1.0.154.59 Safari/525.19 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/525.19 (KHTML, like Gecko) Chrome/1.0.154.53 Safari/525.19 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/525.19 (KHTML, like Gecko) Chrome/1.0.154.50 Safari/525.19 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/525.19 (KHTML, like Gecko) Chrome/1.0.154.46 Safari/525.19 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/525.19 (KHTML, like Gecko) Chrome/1.0.154.43 Safari/525.19 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/525.19 (KHTML, like Gecko) Chrome/1.0.154.42 Safari/525.19 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/525.19 (KHTML, like Gecko) Chrome/0.4.154.31 Safari/525.19 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/525.19 (KHTML, like Gecko) Chrome/0.2.153.0 Safari/525.19 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/525.19 (KHTML, like Gecko) Chrome/0.2.152.0 Safari/525.19 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/525.19 (KHTML, like Gecko) Chrome/0.2.151.0 Safari/525.19 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/525.13 (KHTML, like Gecko) Chrome/0.2.149.6 Safari/525.13 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/525.13 (KHTML, like Gecko) Chrome/0.2.149.30 Safari/525.13 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/525.13 (KHTML, like Gecko) Chrome/0.2.149.29 Safari/525.13 ",
		"Mozilla/5.0 (Windows; U; Windows NT 6.0; de) AppleWebKit/525.13 (KHTML, like Gecko) Chrome/0.2.149.27 Safari/525.13 ",
		"Mozilla/5.0 (Windows NT 6.2) AppleWebKit/536.3 (KHTML, like Gecko) Chrome/19.0.1061.1 Safari/536.3 ",
		"Mozilla/5.0 (Windows NT 6.2) AppleWebKit/536.6 (KHTML, like Gecko) Chrome/20.0.1090.0 Safari/536.6 ",
		"Mozilla/5.0 (Windows NT 6.2; Win64; x64; rv:16.0) Gecko/16.0 Firefox/16.0 ",
		"Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.2; Trident/5.0) ",
		"Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.2; WOW64; Trident/5.0) ",
		"Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; Win64; x64; Trident/6.0) ",
		"Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; Trident/6.0) ",
		"Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0) ",
		"Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; ARM; Trident/6.0) ",
		"Mozilla/5.0 (Windows NT 6.2; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/32.0.1667.0 Safari/537.36 ",
		"Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/30.0.1599.17 Safari/537.36 ",
		"Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/29.0.1547.2 Safari/537.36 ",
		"Mozilla/5.0 (Windows NT 6.2) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/28.0.1467.0 Safari/537.36 ",
		"Mozilla/5.0 (Windows NT 6.2) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/28.0.1464.0 Safari/537.36 ",
		"Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.93 Safari/537.36 ",
		"Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.15 (KHTML, like Gecko) Chrome/24.0.1295.0 Safari/537.15 ",
		"Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.14 (KHTML, like Gecko) Chrome/24.0.1292.0 Safari/537.14 ",
		"Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.13 (KHTML, like Gecko) Chrome/24.0.1290.1 Safari/537.13 ",
		"Mozilla/5.0 (Windows NT 6.2) AppleWebKit/537.13 (KHTML, like Gecko) Chrome/24.0.1290.1 Safari/537.13 ",
		"Mozilla/5.0 (Windows NT 6.2) AppleWebKit/537.11 (KHTML, like Gecko) Chrome/23.0.1271.26 Safari/537.11 ",
		"Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.11 (KHTML, like Gecko) Chrome/23.0.1271.17 Safari/537.11 ",
		"Mozilla/5.0 (Windows NT 6.2) AppleWebKit/537.4 (KHTML, like Gecko) Chrome/22.0.1229.94 Safari/537.4 ",
		"Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.1 (KHTML, like Gecko) Chrome/19.77.34.5 Safari/537.1 ",
		"Mozilla/5.0 (Windows NT 6.2) AppleWebKit/536.3 (KHTML, like Gecko) Chrome/19.0.1062.0 Safari/536.3 ",
		"Mozilla/5.0 (Windows NT 6.2) AppleWebKit/536.3 (KHTML, like Gecko) Chrome/19.0.1061.0 Safari/536.3 ",
		"Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/535.24 (KHTML, like Gecko) Chrome/19.0.1055.1 Safari/535.24 ",
		"Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/535.11 (KHTML, like Gecko) Chrome/17.0.963.66 Safari/535.11 ",
		"Mozilla/5.0 (Windows NT 6.2) AppleWebKit/535.11 (KHTML, like Gecko) Chrome/17.0.963.66 Safari/535.11 ",
		"Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/535.11 (KHTML, like Gecko) Chrome/17.0.963.65 Safari/535.11"};

		#endregion

		/// <summary>
		/// 浏览器Agents
		/// </summary>
		public static string[] Agents { get { return agents; } }

		/// <summary>
		/// 检测URL是否有效,使用<see langword="GET"/> 方法请求
		/// </summary>
		/// <param name="url">URL链接</param>
		/// <returns>状态码在200-299则为<see langword="true"></see>,反之为<see langword="false"/></returns>
		public static async Task<bool> CheckUrlAsync(string url)
		{
			try
			{
				using HttpClient client = new();
				HttpResponseMessage response = await client.GetAsync(url);
				return response.IsSuccessStatusCode;
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// 发送消息到<see cref="Socket"/>服务端,接收数据后会与服务端断开连接
		/// </summary>
		/// <param name="socketIp">服务端IP地址</param>
		/// <param name="socketPort">服务端<see cref="Socket"/>端口号</param>
		/// <param name="message">消息</param>
		/// <returns>服务端接收到发送的数据后返回的数据</returns>
		public static string SendMessageToSocket(string socketIp, int socketPort, string message)
		{
			IPEndPoint serverEndPoint = new(IPAddress.Parse(socketIp), socketPort);
			using Socket clientSocket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			clientSocket.Connect(serverEndPoint);
			byte[] data = Encoding.UTF8.GetBytes(message);
			clientSocket.Send(data);
			byte[] buffer = new byte[1024];
			int bytesRead = clientSocket.Receive(buffer);
			string read = Encoding.UTF8.GetString(buffer, 0, bytesRead);
			return read;
		}

		/// <summary>
		/// 异步调用API上传文件
		/// </summary>
		/// <param name="url">上传地址</param>
		/// <param name="filePath">文件路径</param>
		/// <param name="fileParName">服务端接收文件的参数名称</param>
		/// <param name="parameters">额外的参数,<see langword="Key"/>=参数名,<see langword="Value"/>=参数值</param>
		/// <param name="renameFile">重命名上传的文件名称(如<paramref name="renameFile"/>=test.excel,上传C:\Temp\data.excel,服务端接收到文件名称为test.excel)</param>
		/// <returns>服务器返回的结果</returns>
		public static async Task<string> PostUploadFileAsync(string url, string filePath, string fileParName, Dictionary<string, object>? parameters, string? renameFile = null)
		{
			using HttpClient client = new();
			using MultipartFormDataContent formData = [];
			byte[] fileBytes = File.ReadAllBytes(filePath);
			var fileContent = new ByteArrayContent(fileBytes);
			string fileName = Path.GetFileName(filePath);
			if (renameFile != null)
			{
				fileName = renameFile;
			}
			formData.Add(fileContent, fileParName, fileName);
			if (parameters != null)
			{
				foreach (var item in parameters)
				{
					if (item.Value == null)
					{
						parameters[item.Key] = "null";
					}
					formData.Add(new StringContent(item.Value!.ToString()!), item.Key);
				}
			}
			var response = await client.PostAsync(url, formData);
			return await response.Content.ReadAsStringAsync();
		}
		/// <summary>
		/// 异步调用API上传文件
		/// </summary>
		/// <param name="url">API接口链接</param>
		/// <param name="filePath">文件路径</param>
		/// <param name="fileParName">重命名文件</param>
		/// <returns></returns>
		public static async Task<string> PostUploadFileAsync(string url, string filePath, string fileParName)
		{
			return await PostUploadFileAsync(url, filePath, fileParName, null, null);
		}

		/// <summary>
		/// 检测是否连通指定IP
		/// </summary>
		/// <param name="ip">IP地址如<see langword="127.0.0.1"/> 格式</param>
		/// <returns>返回结果</returns>
		public static bool PingIp(string ip)
		{
			using var ping = new System.Net.NetworkInformation.Ping();
			System.Net.NetworkInformation.PingReply pr = ping.Send(ip);
			return pr.Status == System.Net.NetworkInformation.IPStatus.Success;
		}

		/// <summary>
		/// 检测是否连通的ip地址
		/// </summary>
		/// <param name="ip">IP地址,格式如<see langword="127.0.0.1"/></param>
		/// <returns>返回结果</returns>
		public static long IpRoundtripTime(string ip)
		{
			using var ping = new System.Net.NetworkInformation.Ping();
			System.Net.NetworkInformation.PingReply pr = ping.Send(ip);
			if (pr.Status == System.Net.NetworkInformation.IPStatus.Success)
			{
				return pr.RoundtripTime;
			}
			return -1;
		}

		/// <summary>
		/// 获取随机Agent
		/// </summary>
		/// <returns></returns>
		public static string GetRandomAgent()
		{
			return Agents[new Random().Next(0, Agents.Length - 1)];
		}



		/// <summary>
		/// 视频链接是否有效
		/// </summary>
		/// <param name="videoUrl">连接地址</param>
		/// <param name="timeout">超时时间,单位是<seealso langword="Millisecond" /></param>
		/// <returns>
		/// <list type="table">
		/// <item><b>Item1</b>表示是否正常(<see cref="bool"/>)</item>
		/// <item><b>Item2</b>表示说明信息(<see cref="string"/>)</item>
		/// </list>
		/// </returns>
		public static async Task<Tuple<bool, string>> IsVideoPlayableAsync(string videoUrl, int timeout)
		{
			try
			{
				using (HttpClient client = new HttpClient())
				{
					client.Timeout = TimeSpan.FromMilliseconds(timeout);
					string[] suffixs = new string[] { ".mp4", ".flv", ".webm", ".ogg", ".mkv", ".3gp", ".avi", ".mov" };
					if (suffixs.Any(suffix => videoUrl.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)))
					{
						HttpResponseMessage response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, videoUrl));
						if (response.IsSuccessStatusCode)
						{
							return new Tuple<bool, string>(true, "Get the response header is normal");
						}
						else
						{
							return new Tuple<bool, string>(false, $"Get the response header is error ({response.StatusCode} - {response.ReasonPhrase})");
						}
					}
					else
					{
						HttpResponseMessage response = await client.GetAsync(videoUrl);
						if (response.IsSuccessStatusCode)
						{
							if (response.Content.Headers.ContentType != null && response.Content.Headers.ContentType.MediaType!.StartsWith("video/"))
							{
								return new Tuple<bool, string>(true, "The response content is obtained in video format");
							}
							else
							{
								string responseBody = await response.Content.ReadAsStringAsync();
								if (!string.IsNullOrWhiteSpace(responseBody))
								{
									return new Tuple<bool, string>(true, "The response body was successfully");
								}
								else
								{
									return new Tuple<bool, string>(false, "The response body was failure");
								}
							}
						}
						else
						{
							return new Tuple<bool, string>(false, $"Get the response header is error ({response.StatusCode} - {response.ReasonPhrase})");
						}
					}
				}
			}
			catch (Exception ex)
			{
				return new Tuple<bool, string>(false, $"Exception ({ex.Message})");
			}
		}

		/// <summary>
		/// 判断IPV4格式IP是否为私有地址
		/// </summary>
		/// <param name="ipAddress">IP</param>
		/// <returns></returns>
		public static bool IsPrivateIPAddress(string ipAddress)
		{
			if (IPAddress.TryParse(ipAddress, out IPAddress? parsedIpAddress) && parsedIpAddress != null)
			{
				byte[] bytes = parsedIpAddress.GetAddressBytes();
				if (bytes.Length == 4 &&
					(bytes[0] == 10 ||
					 (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31) ||
					 (bytes[0] == 192 && bytes[1] == 168)))
				{
					return true;
				}
			}
			return false;
		}



		/// <summary>
		/// 绑定接收套接字数据,接收后立即关闭连接,不会返回如何内容,只用于接收数据
		/// </summary>
		/// <param name="port">端口号</param>
		/// <param name="dataReceivedCallback">委托接收事件,委托中<see langword="int"/>值对应:<list type="table">
		/// <item><see langword="int"/> = 1  [接收到数据]</item>
		/// <item><see langword="int"/> = 0  [运行信息]</item>
		/// <item><see langword="int"/> = -1  [捕获异常]</item>
		/// </list>
		/// </param>
		/// <param name="maxListen">最大监听数</param>
		/// <param name="encoding">解码编码</param>
		/// <param name="cancelToken">指示标记调用处取消任务,实际断开需要等待下一次客户端连接时才会生效</param>
		/// <param name="bufferSize">缓存区大小</param>
		/// <returns></returns>
		public static async Task ReceiveSocketData(int port, Action<string?, int> dataReceivedCallback, int maxListen, Encoding encoding, Common.CancelToken cancelToken, int bufferSize = 1024)
		{
			await Task.Run(() =>
			{
				using (Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
				{
					IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, port);
					listener.Bind(localEndPoint);
					listener.Listen(maxListen);
					dataReceivedCallback?.Invoke($"服务正在侦听 {localEndPoint}", 0);
					while (true)
					{
						try
						{
							dataReceivedCallback?.Invoke("等待连接", 0);
							using (Socket clientSocket = listener.Accept())
							{
								if (cancelToken.IsCancellationRequested)
								{
									break;
								}
								EndPoint? endPoint = clientSocket.LocalEndPoint;
								string? connectIP = "";
								if (endPoint != null)
								{
									connectIP = endPoint.ToString();
								}
								byte[] buffer = new byte[bufferSize];
								int bytesRead = clientSocket.Receive(buffer);
								string message = encoding.GetString(buffer, 0, bytesRead).Trim();
								dataReceivedCallback?.Invoke(message, 1);
								clientSocket.Close();
								dataReceivedCallback?.Invoke($"断开连接 {connectIP}", 0);
							}
						}
						catch (Exception ex) 
						{
							dataReceivedCallback?.Invoke(ex.ToString(), -1);
						}
					}
					dataReceivedCallback?.Invoke("终止侦听", 0);
				}
			});
		}

		/// <summary>
		/// 异步获取网页Html内容
		/// </summary>
		/// <param name="url">网址</param>
		/// <param name="encoding">编码</param>
		/// <returns></returns>
		public static async Task<string> GetHtmlAsync(string url, Encoding encoding)
		{
			using (HttpClient client = new HttpClient())
			{
				HttpResponseMessage response = await client.GetAsync(url);
				response.EnsureSuccessStatusCode();
				byte[] bytes = await response.Content.ReadAsByteArrayAsync();
				string html = encoding.GetString(bytes);
				return html;
			}
		}


		/// <summary>
		/// 检测指定端口号是否打开
		/// </summary>
		/// <param name="ip">IP</param>
		/// <param name="port">端口号</param>
		/// <param name="timeoutMilliseconds">超时时间,毫秒</param>
		/// <returns></returns>
		public static bool IsPortOpen(string ip, int port, int timeoutMilliseconds)
		{
			try
			{
				using (TcpClient client = new TcpClient())
				{
					client.ReceiveTimeout = timeoutMilliseconds;
					client.SendTimeout = timeoutMilliseconds;
					client.Connect(ip, port);
					return true;
				}
			}
			catch (SocketException)
			{
				return false;
			}
		}


		/// <summary>
		/// 异步检测指定端口号是否打开
		/// </summary>
		/// <param name="ip">IP</param>
		/// <param name="port">端口号</param>
		/// <param name="timeoutMilliseconds">超时时间,毫秒</param>
		/// <returns></returns>
		public static async Task<bool> IsPortOpenAsync(string ip, int port, int timeoutMilliseconds)
		{
			try
			{
				using (var client = new TcpClient())
				{
					client.ReceiveTimeout = timeoutMilliseconds + 1000;
					client.SendTimeout = timeoutMilliseconds + 1000;
					var connectTask = client.ConnectAsync(ip, port);
					if (await Task.WhenAny(connectTask, Task.Delay(timeoutMilliseconds)) == connectTask && client.Connected)
					{
						return true;
					}
					return false;
				}
			}
			catch (SocketException)
			{
				return false;
			}
		}

	}
}
