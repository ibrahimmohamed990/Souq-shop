﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Souq.DataAccessLayer.DbInitializer
{
    public interface IDbInitializer
    {
        Task Initialize();
    }
}
