using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PoupaDev.API.Entities;

namespace PoupaDev.API.Persistence
{
    public class PoupaDevDbContext
   {
       public List<ObjetivoFinanceiro> Objetivos { get; set; }
 
       public PoupaDevDbContext()
       {
           Objetivos = new List<ObjetivoFinanceiro>();
       }
   }

}