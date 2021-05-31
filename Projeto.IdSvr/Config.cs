using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Projeto.IdSvr
{
    public class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResource()
        {
            return new List<IdentityResource>
            {
                ///Declaração dos IdentiResources quais dados o token pode carregar o openid é obrigatório
                ///por levar o id do assunto do token
                new IdentityResources.OpenId(),
                new IdentityResource
                {
                    Name = "profile",
                    UserClaims =
                    {
                                                              "name",
                                                              "family_name",
                                                              "given_name",
                                                              "middle_name",
                                                              "nickname",
                                                              "preferred_username",
                                                              "profile",
                                                              "picture",
                                                              "website",
                                                              "gender",
                                                              "birthdate",
                                                              "zoneinfo",
                                                              "locale",
                                                              "updated_at",
                                                              "role"

                    },


                }

            };
        }

        public static IEnumerable<ApiResource> GetAllApiResources()
        {
            return new List<ApiResource>()
            {
                ///Declaração do openid socpe para Api
                new ApiResource()
                {
                    Name = "openid",
                    DisplayName = "openid",
                    Description = "openid",
                    UserClaims = {"sub"},



                 },

                /// Declaração necessária para que a API possa compartilhar a sua informação com MVC ou uma outra API
                /// Desta forma a ApiUm pode acessar recursos da ApiUm e ApiDois e o MVC pode acessar dados da ApiDois através da ApiUm
                new ApiResource("resourceApiUm", "resourceApiUm"){
                    Scopes = { "scopeApiUm", "scopeApiDois", "scopeApiGateway" },


                },
                /// Compartilhar dados da ApiDois
                new ApiResource("resourceApiDois", "resourceApiDois"){
                    Scopes = { "scopeApiDois", "scopeApiGateway" },


                },

                 /// Consegue compartilhar dados da ApiUm, ApiDois e da ApiGateway
                // new ApiResource("resourceApiGateway", "resourceApiGateway"){
                //    Scopes = { "scopeApiGateway", "scopeApiUm", "scopeApiDois" },


                //},

            };

        }


        public static IEnumerable<ApiScope> GetAllApiScopes()
        {

            return new List<ApiScope>()
            {
                /// Declaração da Api scope para API UM
                new ApiScope(name: "scopeApiUm",
                displayName: "scopeApiUm",
                userClaims: new [] { JwtClaimTypes.Name,
                                     JwtClaimTypes.Email,
                                     JwtClaimTypes.Role }

                ),

                /// Declaração da Api scope para API Dois
                 new ApiScope(name: "scopeApiDois",
                displayName: "scopeApiDois",
                userClaims: new [] { JwtClaimTypes.Name,
                                     JwtClaimTypes.Email,
                                     JwtClaimTypes.Role }

                ),

                 /// Declaração da Api scope para API Dois
                 new ApiScope(name: "scopeApiGateway",
                displayName: "scopeApiGateway",
                userClaims: new [] { JwtClaimTypes.Name,
                                     JwtClaimTypes.Email,
                                     JwtClaimTypes.Role }

                ),

            };
        }

        public static IEnumerable<Client> GetClients()
        {

            return new List<Client>
            {
                /// Declaração do Client que para que Projeto MVC seja autenticado
                /// O MVC ao contrário da API, não declara o tipo ApiResource no AllowedScopes, pois é recusros entre API's
                /// tem que ser declarado direto o tipo ApiSocpe
                /// 
                /// O Secret é configurado para cada um dos clients sem que haja interferencia na comunicação entre projetos
                new Client()
                {
                    ClientId = "mvc.portal-oidc",
                    ClientName = "MVC Portal",
                    AllowedGrantTypes = { GrantType.AuthorizationCode },
                    AllowOfflineAccess = true,
                    ClientSecrets = { new Secret("mvc1".Sha256()) },
                    AllowedScopes =
                        {
                             IdentityServerConstants.StandardScopes.OpenId,
                             "profile",
                             "offline_access",
                             "scopeApiUm",
                             //"scopeApiGateway"
                             


                        },
                    RedirectUris = new[] { "https://localhost:44323/signin-oidc" },
                    RefreshTokenUsage = TokenUsage.ReUse,
                    RefreshTokenExpiration = TokenExpiration.Absolute,
                    AbsoluteRefreshTokenLifetime = 259200,
                    RequireConsent = false,
                    AllowAccessTokensViaBrowser = true,
                    AccessTokenLifetime = 3600,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    PostLogoutRedirectUris = { "https://localhost:44323/signout-callback-oidc" },
                    RequirePkce = true


                },

                /// Declaração do Client da ApiUm para que o Projeto da Api com mesmo nome seja atenticado
                /// obs para o auntenticação OAuth do Swagger funcionar declare diretamente os escopos scopeApiUm e scopeApiDois
                /// A não declaração não interfere na autenticação da Api
                
                /// Aqui pode ser declarado direto o ApiResource

                /// O Secret é configurado para cada um dos clients sem que haja interferencia na comunicação entre projetos
                new Client()
                {
                    ClientId = "projeto.api.um",
                    ClientSecrets = { new Secret("apium".Sha256()) },
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = { "resourceApiUm", "profile", "offline_access", "openid" /*/,"scopeApiUm", "scopeApiDois"*/ },
                    AllowOfflineAccess = true,


                },
                new Client()
                {
                    ClientId = "projeto.api.dois",
                    ClientSecrets = { new Secret("apidois".Sha256()) },
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = { "resourceApiDois", "profile", "offline_access", "openid" /*/,"scopeApiUm", "scopeApiDois"*/ },
                    AllowOfflineAccess = true,


                }
            };


        }

    }
}
