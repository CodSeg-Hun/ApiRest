
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualBasic;

namespace ApiRest.Clases
{
    public class OAuthBase
    {
        const string HMACSHA256SignatureType = "HMAC-SHA256";
        const string OAuthVersion = "1.0";
        const string OAuthParameterPrefix = "oauth_";
        const string OAuthConsumerKeyKey = "oauth_consumer_key";
        const string OAuthVersionKey = "oauth_version";
        const string OAuthSignatureMethodKey = "oauth_signature_method";
        const string OAuthTimestampKey = "oauth_timestamp";
        const string OAuthNonceKey = "oauth_nonce";
        const string OAuthTokenKey = "oauth_token";



        public class QueryParameter
        {
            public QueryParameter(string name, string value)
            {
                this.name = name;
                this.value = value;
            }

            // Public ReadOnly Property Name As String
            // Public ReadOnly Property Value As String

            private string _name;
            public string name
            {
                get
                {
                    return _name;
                }
                set
                {
                    _name = value;
                }
            }

            private string _value;
            public string value
            {
                get
                {
                    return _value;
                }
                set
                {
                    _value = value;
                }
            }
        }



        protected class QueryParameterComparer : IComparer<QueryParameter>
        {
            public int Compare(QueryParameter x, QueryParameter y)
            {
                if (x.name == y.name)
                    return string.Compare(x.value, y.value);
                else
                    return string.Compare(x.name, y.name);
            }
        }



        public static string ComputeHash(HashAlgorithm hashAlgorithm, string data)
        {
            byte[] dataBuffer = Encoding.ASCII.GetBytes(data);
            byte[] hashBytes = hashAlgorithm.ComputeHash(dataBuffer);
            return Convert.ToBase64String(hashBytes);
        }





        public static List<QueryParameter> GetQueryParameters(string parameters)
        {
            if (parameters.StartsWith("?"))
                parameters = parameters.Remove(0, 1);

            List<QueryParameter> result = new List<QueryParameter>();

            if (!string.IsNullOrEmpty(parameters))
            {
                string[] p = parameters.Split("&");

                foreach (string s in p)
                {
                    if (!string.IsNullOrEmpty(s) && !s.StartsWith(OAuthParameterPrefix))
                    {
                        if (s.IndexOf("=") > -1)
                        {
                            string[] temp = s.Split("=");
                            result.Add(new QueryParameter(temp[0], temp[1]));
                        }
                        else
                            result.Add(new QueryParameter(s, string.Empty));
                    }
                }
            }
            return result;
        }


        public static string UrlEncode(string value)
        {
            StringBuilder result = new StringBuilder();

            var unreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

            foreach (char symbol in value)
            {
                if (unreservedChars.IndexOf(symbol) != -1)
                    result.Append(symbol);
                else
                    // result.Append("%"c & String.Format("{0:X2}", CInt(symbol)))
                    result.Append("%" + string.Format("{0:X2}", Strings.Asc(symbol)));
            }

            return result.ToString();
        }

        public static string NormalizeRequestParameters(IList<QueryParameter> parameters)
        {
            StringBuilder sb = new StringBuilder();
            QueryParameter p = null;

            for (int i = 0; i <= parameters.Count - 1; i++)
            {
                p = parameters[i];
                sb.AppendFormat("{0}={1}", p.name, p.value);

                if (i < parameters.Count - 1)
                    sb.Append("&");
            }

            return sb.ToString();
        }


        public static string GenerateSignatureBase(Uri url, string consumerKey, string token, string httpMethod, string timeStamp, string nonce, string signatureType)      // If token Is Nothing Then
        {
            List<QueryParameter> parameters = GetQueryParameters(url.Query);
            parameters.Add(new QueryParameter(OAuthVersionKey, OAuthVersion));
            parameters.Add(new QueryParameter(OAuthNonceKey, nonce));
            parameters.Add(new QueryParameter(OAuthTimestampKey, timeStamp));
            parameters.Add(new QueryParameter(OAuthSignatureMethodKey, signatureType));
            parameters.Add(new QueryParameter(OAuthConsumerKeyKey, consumerKey));
            parameters.Add(new QueryParameter(OAuthTokenKey, token));
            parameters.Sort(new QueryParameterComparer());
            var normalizedUrl = string.Format("{0}://{1}", url.Scheme, url.Host);

            if (!((url.Scheme == "http" && url.Port == 80) || (url.Scheme == "https" && url.Port == 443)))
                normalizedUrl += ":" + url.Port;

            normalizedUrl += url.AbsolutePath;
            var normalizedRequestParameters = NormalizeRequestParameters(parameters);

            StringBuilder signatureBase = new StringBuilder();
            signatureBase.AppendFormat("{0}&", httpMethod.ToUpper());
            signatureBase.AppendFormat("{0}&", UrlEncode(normalizedUrl));
            signatureBase.AppendFormat("{0}", UrlEncode(normalizedRequestParameters));
            return signatureBase.ToString();
        }


        public static string GenerateSignatureUsingHash(string signatureBase, HashAlgorithm hash)
        {
            return ComputeHash(hash, signatureBase);
        }


        public static string GenerateSignatureSHA256(Uri url, string consumerKey, string consumerSecret, string token, string tokenSecret, string httpMethod, string timeStamp, string nonce)
        {
            string sha256base = GenerateSignatureBase(url, consumerKey, token, httpMethod, timeStamp, nonce, HMACSHA256SignatureType);

            HMACSHA256 hmacsha256 = new HMACSHA256();

            hmacsha256.Key = Encoding.ASCII.GetBytes(string.Format("{0}&{1}", UrlEncode(consumerSecret), string.IsNullOrEmpty(tokenSecret) ? "" : UrlEncode(tokenSecret)));


            return GenerateSignatureUsingHash(sha256base, hmacsha256);
        }


        public static string GenerateSignature(Uri url, string consumerKey, string consumerSecret, string token, string tokenSecret, string httpMethod, string timeStamp, string nonce)
        {
            var signature = GenerateSignatureSHA256(url, consumerKey, consumerSecret, token, tokenSecret, httpMethod, timeStamp, nonce);

            if (signature.Contains("+"))
                signature = signature.Replace("+", "%2B");

            if (signature.Contains("="))
                signature = signature.Replace("=", "%3D");

            return signature;
        }

        public static string GenerateTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        public static string GenerateNonce()
        {
            var random = new Random();
            return random.Next(123400, 9999999).ToString();
        }
    }
}