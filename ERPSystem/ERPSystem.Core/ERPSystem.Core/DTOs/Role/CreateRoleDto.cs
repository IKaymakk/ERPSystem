﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Core.DTOs.Role
{
    public class CreateRoleDto
    {
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}
