using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JGUZDV.Extensions.Json.Converters
{
    /// <summary>
    /// Converts a <see cref="ClaimsPrincipal"/> to and from JSON.
    /// </summary>
    public class ClaimsPrincipalConverter : JsonConverter<ClaimsPrincipal>
    {
        /// <inheritdoc/>
        public override ClaimsPrincipal? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType is JsonTokenType.Null)
                return null;

            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException();

            var identities = new List<ClaimsIdentity>();

            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if(reader.TokenType == JsonTokenType.PropertyName)
                {
                    switch (reader.GetString())
                    {
                        case nameof(ClaimsPrincipal.Identities):
                            reader.Read();
                            identities.AddRange(ReadClaimsIdentityArray(ref reader)); 
                            break;

                        default:
                            throw new JsonException();
                    }
                }
            }

            return new ClaimsPrincipal(identities);
        }


        private static List<ClaimsIdentity> ReadClaimsIdentityArray(ref Utf8JsonReader reader)
        {
            var result = new List<ClaimsIdentity>();

            if (reader.TokenType != JsonTokenType.StartArray)
                throw new JsonException();

            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
            {
                if(reader.TokenType == JsonTokenType.StartObject)
                {
                    var identity = ReadClaimsIdentity(ref reader);
                    result.Add(identity);
                }
                else
                {
                    throw new JsonException();
                }
            }

            return result;
        }


        private static ClaimsIdentity ReadClaimsIdentity(ref Utf8JsonReader reader)
        {
            string? authenticationType = null;
            string? nameType = ClaimsIdentity.DefaultNameClaimType;
            string? roleType = ClaimsIdentity.DefaultRoleClaimType;

            var claims = new List<Claim>();

            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    switch (reader.GetString())
                    {
                        case nameof(ClaimsIdentity.AuthenticationType):
                            authenticationType = reader.GetString();
                            break;

                        case nameof(ClaimsIdentity.NameClaimType): 
                            nameType = reader.GetString();
                            break;
                        
                        case nameof(ClaimsIdentity.RoleClaimType): 
                            roleType = reader.GetString();
                            break;

                        case nameof(ClaimsIdentity.Claims):
                            reader.Read();
                            claims.AddRange(ReadClaimsArray(ref reader));
                            break;

                        default:
                            throw new JsonException();
                    }
                }
            }

            return new ClaimsIdentity(claims, authenticationType, nameType ?? ClaimsIdentity.DefaultNameClaimType, roleType ?? ClaimsIdentity.DefaultRoleClaimType);
        }


        private static List<Claim> ReadClaimsArray(ref Utf8JsonReader reader)
        {
            var result = new List<Claim>();

            if (reader.TokenType != JsonTokenType.StartArray)
                throw new JsonException();

            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
            {
                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    var claim = ReadClaim(ref reader);
                    result.Add(claim);
                }
                else
                {
                    throw new JsonException();
                }
            }

            return result;
        }


        private static Claim ReadClaim(ref Utf8JsonReader reader)
        {
            string? type = null;
            string? value = null;

            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    switch (reader.GetString())
                    {
                        case nameof(Claim.Type):
                            type = reader.GetString();
                            break;

                        case nameof(Claim.Value):
                            value = reader.GetString();
                            break;

                        default:
                            throw new JsonException();
                    }
                }
            }

            return type != null && value != null
                ? new Claim(type, value)
                : throw new JsonException();
        }


        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, ClaimsPrincipal value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteStartArray(nameof(value.Identities));

            foreach(var identity in value.Identities)
            {
                writer.WriteStartObject();

                writer.WriteString(nameof(identity.AuthenticationType), identity.AuthenticationType);
                writer.WriteString(nameof(identity.NameClaimType), identity.NameClaimType);
                writer.WriteString(nameof(identity.RoleClaimType), identity.RoleClaimType);

                writer.WriteStartArray(nameof(identity.Claims));

                foreach(var claim in identity.Claims)
                {
                    writer.WriteStartObject();

                    writer.WriteString(nameof(claim.Type), claim.Type);
                    writer.WriteString(nameof(claim.Value), claim.Value);

                    writer.WriteEndObject();
                }

                writer.WriteEndArray();

                writer.WriteEndObject();
            }

            writer.WriteEndArray();
            writer.WriteEndObject();
        }
    }
}
