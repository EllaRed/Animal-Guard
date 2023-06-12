#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("D+RqG2mwi5OcuaXKwT3r6rzA/Vu2Ynoeo/VUunt1tUzTQrXkeVXW5KrvfAffF4HmLouxWBG7xJHdKFEgF43iK2H9+f8LhY9u6xCLxmgr7BxL25YeYHaeX2LU5snmuqyVgsMnwRdwTCAfWDG/24cvyVaoiZ423KobA4COgbEDgIuDA4CAgUDTnPQUSGlMwXGftMkyQZ9KbtTmUnKC53+VJbEDgKOxjIeIqwfJB3aMgICAhIGCBQ8nNgmSCs3mjre+v5p6oI8K6hdAY/5CCGLkLZp0cOIQKArg+W2+78g0yzI55S4oNBfTkdwF0Ln87bn0R4pndOKJcASphBstuiJRBFb6LqTjK262NRzT7qFH9E4EnFvw4qpmzGbfXOAEBymAHIOCgIGA");
        private static int[] order = new int[] { 7,11,3,9,12,5,11,10,10,13,10,11,13,13,14 };
        private static int key = 129;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
