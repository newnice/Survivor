#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("ectIa3lET0BjzwHPvkRISEhMSUr9OFMFltnRRKHM7FvtIO/nqhrbhKpbUQ3OQl/cdqpKHrrfjSPLK0hSWgZLJRpm6MgcK8AOblaa1L0A4BK1+1GRCJx4Hc3939vP4DtFJE2uKhxjMcGnUYJSnza4E7jM2S3kVOify0hGSXnLSENLy0hISYmx/4IWP906J2xE2EWLJs9Wvp3SpZV9T2ps47FT7YHK5x4x9l4M0OUh7fih2KZxliITmHuZe2Hxaiyh9KKQxfvb2FSRb7iLDf3EAMirqS5iaHoXEPSH+8eo7Mrpj/Jbj9lc3MLHxwMoza1t+ScGHt+AJ6oAAVsDRnJIaXKIeFjscEGfYKsA+3nilMVWQ+uw08pghCfbfuijA2CzREtKSElI");
        private static int[] order = new int[] { 0,2,12,6,6,7,12,10,8,12,12,13,12,13,14 };
        private static int key = 73;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
