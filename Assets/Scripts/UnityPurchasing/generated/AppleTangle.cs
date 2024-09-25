#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class AppleTangle
    {
        private static byte[] data = System.Convert.FromBase64String("Bl8eDAwKEhoMXx4cHBoPCx4RHBoICFEeDw8TGlEcEBJQHg8PExocHsGLDOSRrRtwtAYwS6fdQYYHgBS3YO6kYTgvlHqSIQb7UpRJ3SgzKpP/a1SvFjjrCXaBixTyUT/ZiDgyAL8cTAiIRXhTKZSlcF5xpcUMZjDK1NwO7TgsKr7QUD7Mh4ScD7KZ3DPIZMLsPVttVbhwYsky4yEctzT/aFX5N/mIcn5+enp/Tx1OdE92eXwqen98/X5wf0/9fnV9/X5+f5vu1nYFT/1+CU9xeXwqYnB+foB7e3x9fhtKXGo0aiZizOuIiePhsC/FvicvXzw+T/1+XU9yeXZV+Tf5iHJ+fn5ZT1t5fCp7dGxiPg8PExpfPBoNC3t5bH0qLE5sT255fCp7dWx1Pg8PaU9reXwqe3xscj4PDxMaXy0QEAtQT/68eXdUeX56enh9fU/+yWX+zPAM/h+5ZCR2UO3Nhzs3jx9H4WqKd1R5fnp6eH1+aWEXCwsPDEVQUAgtGhMWHhEcGl8QEV8LFxYMXxwaDQ8TGl88Gg0LFhkWHB4LFhARXz4Kzk8nkyV7TfMXzPBioRoMgBghGsN5fCpicXtpe2tUrxY46wl2gYsU8h0TGl8MCx4RGx4NG18LGg0SDF8eCxcQDRYLBk5pT2t5fCp7fGxyPg9MSSVPHU50T3Z5fCp7eWx9KixObE9ueXwqe3VsdT4PDxMaXzYRHFFOAD7X54autRnjWxRur9zEm2RVvGC2Zg2KInGqACDkjVp8xSrwMiJyjqZJAL74KqbY5sZNPYSnqg7hAd4tFhkWHB4LFhARXz4KCxcQDRYLBk54kwJG/PQsX6xHu87A5TB1FIBUgxMaXzYRHFFOWU9beXwqe3RsYj4PeU9weXwqYmx+foB7ek98fn6AT2LXowFdSrVaqqZwqRSr3Vtcboje0/1+f3l2Vfk3+YgcG3p+T/6NT1V5Xx4RG18cGg0LFhkWHB4LFhARXw/KRdKLcHF/7XTOXmlRC6pDcqQdaVNfHBoNCxYZFhweCxpfDxATFhwGUT/ZiDgyAHchT2B5fCpiXHtnT2n0ZvahhjQTinjUXU99l2dBhy92rEJZGF/1TBWIcv2woZTcUIYsFSQbW52UrsgPoHA6nli1jhIHkpjKaGhg+vz6ZOZCOEiN1uQ/8VOrzu9tpxEbXxwQERsWCxYQEQxfEBlfCgwaGPB3y1+ItNNTXxAPyUB+T/PIPLA2pwngTGsa3gjrtlJ9fH5/ftz9fg8TGl8tEBALXzw+T2Fock9JT0tNSeYzUgfIkvPko4wI5I0JrQhPML4m2Hp2A2g/KW5hC6zI9FxEONyqEEpNTktPTEklaHJMSk9NT0ZNTktPT/17xE/9fNzffH1+fX1+fU9yeXZ3IU/9fm55fCpiX3v9fndP/X57T3DiQoxUNldlt4GxysZxpiFjqbRCDR4cCxYcGl8MCx4LGhIaEQsMUU9yeXZV+Tf5iHJ+fnp6f3z9fn5/I+rhBXPbOPQkq2lITLS7cDKxaxauXxAZXwsXGl8LFxoRXx4PDxMWHB4LFhkWHB4LGl8dBl8eEQZfDx4NCzoBYDMUL+k+9rsLHXRv/D74TPX+L9X1qqWbg692eEjPCgpe");
        private static int[] order = new int[] { 40,36,50,15,32,57,24,59,36,14,28,20,13,28,50,28,37,55,35,38,32,26,30,42,57,36,50,55,38,58,32,51,38,58,54,54,45,42,44,51,42,56,53,49,49,50,55,57,48,52,59,53,58,58,55,57,57,57,58,59,60 };
        private static int key = 127;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
