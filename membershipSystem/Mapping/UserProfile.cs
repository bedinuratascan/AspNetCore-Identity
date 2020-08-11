using AutoMapper;
using membershipSystem.Models;
using membershipSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace membershipSystem.Mapping
{
    public class UserProfile:Profile
    {
        public UserProfile()
        {
            CreateMap<AppUser, UserViewModel>().ReverseMap();
        }
    }
}
