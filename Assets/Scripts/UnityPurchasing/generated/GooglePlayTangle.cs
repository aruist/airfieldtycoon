// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("I62zRj4VmjHGWwLn0PQ1Jgv+UTXdTd7yY+eZBgyBcTkfIJ+Q0nfSV1jR2Mn6zsW/oK4robZPr0nh1EP75ws0CiLN9BJJcu32NVzdSUqFxish9wtMlERPysiKj5OXLbo9pmKKTHJVlWP0Piqh3QNg3iS9GPMC9ND3ZsWZsPs0vSXiZYrYtrqoh6bko/nZDFUPeuGdz61GaEOKsS6tgar7nWPTabRxrJ0/z/zFUhtXeCfKJOs1mSuoi5mkr6CDL+EvXqSoqKisqappbyBzklNDj4tXOwMuv5Er7a7wdyuopqmZK6ijqyuoqKksWNj6RTS4Z64w8Xtb/9FR8mqIfUK2/rE/amUeOP5JEMw1Jw+guRTw26uoyoF4SRAR8T8hiDlklKuqqKmo");
        private static int[] order = new int[] { 6,5,13,7,12,13,7,10,11,10,12,13,13,13,14 };
        private static int key = 169;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
