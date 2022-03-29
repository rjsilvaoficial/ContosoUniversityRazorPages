using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using ContosoUniversity.Models;

namespace ContosoUniversity.Pages.Students
{
    public class IndexModel : PageModel
    {
        private readonly ContosoUniversity.Data.SchoolContext _context;

        public IndexModel(ContosoUniversity.Data.SchoolContext context)
        {
            _context = context;
        }

        public IList<Student> Student { get;set; }

        //Para adicionar a classificação são adicionadas variáveis correspondentes aos atributos sobre os quais ela sera aplicada
        //Em seguida são adicionadas variáveis para receber o filtro em vigor atualmente e a ordenação
        public string NameSort { get; set; }
        public string DateSort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }

        public IList<Student> Students { get; set; }

        //O método de listagem precisa receber um parametro string que conterá auxiliará com a info sobre a ordenação
        public async Task OnGetAsync(string sortOrder)
        {
            // using System;
            //NameSort = "name_desc" se sortOrder for nulo ou vazio, senão sera vazio
            NameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            //Se sortOrder == Date, o DateSort = date_desc, senão recebe Date
            DateSort = sortOrder == "Date" ? "date_desc" : "Date";

            //Aqui fazemos aquela busca no banco retornando todos os Students
            //IQueryable faz um objeto que só validará a efetivação da consulta quando se tornar uma coleção (ToListAsync())
            IQueryable<Student> studentsIQ = from s in _context.Students
                                             select s;

            //Aqui o swtich usa o sortOrder para ordenar a lista
            switch (sortOrder)
            {
                case "name_desc":
                    studentsIQ = studentsIQ.OrderByDescending(s => s.LastName);
                    break;
                case "Date":
                    studentsIQ = studentsIQ.OrderBy(s => s.EnrollmentDate);
                    break;
                case "date_desc":
                    studentsIQ = studentsIQ.OrderByDescending(s => s.EnrollmentDate);
                    break;
                default:
                    studentsIQ = studentsIQ.OrderBy(s => s.LastName);
                    break;
            }


                    Students = await studentsIQ.AsNoTracking().ToListAsync();

        }
    }
}
