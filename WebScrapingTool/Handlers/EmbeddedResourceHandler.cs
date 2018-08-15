using System.IO;
using System.Reflection;
using System.Text;
using static WebScrapingTool.Constants.Constants;

namespace WebScrapingTool.Handlers
{
    //This class will be responsible for interactions with embedded resources
    public static class EmbeddedResourceHandler
    {
        private static string m_currentDomainFriendlyName = Properties.AppSettings.Default.ApplicationName;

        //Will return an assembly as a byte array
        public static byte[] GetAssemblyResource(string embededResourceName)
        {
            byte[] rawBytes = null;
            using (Stream assemblyStream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{m_currentDomainFriendlyName}{embededResourceName}"))
            {
                rawBytes = new byte[assemblyStream.Length];
                for (int Index = 0; Index < assemblyStream.Length; Index++)
                {
                    rawBytes[Index] = (byte)assemblyStream.ReadByte();
                }
            }

            return rawBytes;
        }

        //Will return  assembly contents as a string
        public static string GetAssemblyResourceAsString(string embededResourceName) => Encoding.Default.GetString(GetAssemblyResource(embededResourceName)).Replace(EncodingBom.Utf8Bom, string.Empty);
    }
}
