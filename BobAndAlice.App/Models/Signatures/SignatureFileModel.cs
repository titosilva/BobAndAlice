using Newtonsoft.Json;

namespace BobAndAlice.App.Models.Signatures
{
    public class SignatureFileModel
    {
        [JsonProperty("Arquivo")]
        public string OriginalFileName { get; set; }

        [JsonProperty("Cifra")]
        public string EncryptedData { get; set; }

        [JsonProperty("Assinatura")]
        public string Signature { get; set; }

        [JsonProperty("ChavePublica")]
        public string PublicKeyValue { get; set; }

        [JsonProperty("Modulo")]
        public string PublicKeyModulus { get; set; }

        public string Serialize()
            => JsonConvert.SerializeObject(this, Formatting.Indented);

        public static bool TryDeserialize(string s, out SignatureFileModel deserialized)
        {
            deserialized = null;
            try
            {
                deserialized = JsonConvert.DeserializeObject<SignatureFileModel>(s);
                return true;
            } catch
            {
                return false;
            }
        }
    }
}
