using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using UsuariosApi.Models;

namespace UsuariosApi.Services
{
    public class TokenService
    {
        public string GenerateToken(Usuario usuario)
        {
            Claim[] claims = new Claim[] {
                new Claim("username", usuario.UserName),
                new Claim("id", usuario.Id),
                new Claim(ClaimTypes.DateOfBirth, usuario.DataNascimento.ToString()),
            };


            var chave = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("9ASHDA98H9ah9ha9H9A89n0f")
            );

            var signingCredentials = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken ( 
                expires: DateTime.Now.AddMinutes(10), 
                claims: claims, 
                signingCredentials: signingCredentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}