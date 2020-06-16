﻿using SmartBulbColor.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SmartBulbColor.PluginApplication.AoolicationDTOs
{
    public class BulbCommandDTO
    {
        public TargetType Target;
        public CommandType Mode;
        public int TargetId;
        public string Method;
        public ArrayList Parameters;
    }
}