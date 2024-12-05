using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace UsuariosApi.Autorization
{
    public class IdadeAutorization : AuthorizationHandler<IdadeMinima>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IdadeMinima requirement)
        {
            var dataNascimentoClaim = context.User.FindFirst(claim => claim.Type == ClaimTypes.DateOfBirth);

            if (dataNascimentoClaim is null){
                return Task.CompletedTask;
            }

            var dataNascimento = Convert.ToDateTime(dataNascimentoClaim.Value); //converte para datetime

            var idade = DateTime.Today.Year - dataNascimento.Year;

            if(dataNascimento > DateTime.Today.AddYears(-idade)) {
                idade--;
            }

            if(idade >= requirement.Idade){
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}