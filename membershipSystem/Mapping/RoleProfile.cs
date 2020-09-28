using AutoMapper;
using membershipSystem.Models;
using membershipSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace membershipSystem.Mapping
{
    public class RoleProfile:Profile
    {
        public RoleProfile()
        {
            CreateMap<AppRole, RoleViewModel>().ReverseMap();
        }
    }
}
