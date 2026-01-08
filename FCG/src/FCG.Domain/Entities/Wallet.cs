using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCG.Domain.Entities
{
    internal class Wallet
    {
        public int Id { get; set; }
        public required string Username { get; set; }

        [Precision(18, 2)]
        public required decimal Funds { get; set; }
        public DateTime LastRecharge { get; set; }

    }
}
