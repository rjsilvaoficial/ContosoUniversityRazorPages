using ContosoUniversity.Data;
using ContosoUniversity.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity.Pages.Students
{
    public class IndexModel : PageModel
    {
        private readonly SchoolContext _context;

        public IndexModel(SchoolContext context)
        {
            _context = context;
        }

        public string NameSort { get; set; }
        public string DateSort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }
        public IList<Student> Students { get; set; }



        public int QuantidadeItens { get; set; }

        private const int _itensPorPagina = 3;  
        public int QuantidadePaginas { get; set; }
        public int PaginaAtual { get; set; }





        //começando a paginação
        public async Task OnGetAsync(string sortOrder, string searchString, int? pagina = 1)
        {
            NameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            DateSort = sortOrder == "Date" ? "date_desc" : "Date";

            CurrentFilter = searchString;
            CurrentSort = sortOrder;

            IQueryable<Student> studentsIQ = from s in _context.Students
                                             select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                studentsIQ = studentsIQ.Where(s => s.LastName.Contains(searchString)
                                       || s.FirstMidName.Contains(searchString));
            }

            //O switch incluso aqui, gera a classificação de TODA a lista de resultados, não somente a página atual
            switch (sortOrder)
            {
                case "name_desc":
                    studentsIQ = studentsIQ.OrderByDescending(s => s.LastName.ToUpper());
                    break;
                case "Date":
                    studentsIQ = studentsIQ.OrderBy(s => s.EnrollmentDate);
                    break;
                case "date_desc":
                    studentsIQ = studentsIQ.OrderByDescending(s => s.EnrollmentDate);
                    break;
                default:
                    studentsIQ = studentsIQ.OrderBy(s => s.LastName.ToUpper());
                    break;
            }



            //Esta linha gera uma referência à stundentsIQ
            var studentsIQQuery = studentsIQ;

            //Esta linha usa a var com referência  a stundentsIQ para gerar um resultado particular
            //(a quantidade de itens do tipo student no db)
            QuantidadeItens = await studentsIQQuery.CountAsync();

            //Aqui teremos o valor 1 (default) ou o valor passado na url (prop Value garante isso)
            PaginaAtual = pagina.Value;

            QuantidadePaginas = Convert.ToInt32(Math.Ceiling(QuantidadeItens * 1M / _itensPorPagina));

            studentsIQ = studentsIQ.Skip(PaginaAtual * _itensPorPagina - _itensPorPagina).Take(_itensPorPagina);


            //Neste local o switch ordena o conteúdo da página em exibição
            //switch (sortOrder)
            //{
            //    case "name_desc":
            //        studentsIQ = studentsIQ.OrderByDescending(s => s.LastName.ToUpper());
            //        break;
            //    case "Date":
            //        studentsIQ = studentsIQ.OrderBy(s => s.EnrollmentDate);
            //        break;
            //    case "date_desc":
            //        studentsIQ = studentsIQ.OrderByDescending(s => s.EnrollmentDate);
            //        break;
            //    default:
            //        studentsIQ = studentsIQ.OrderBy(s => s.LastName.ToUpper());
            //        break;
            //}

            Students = await studentsIQ.AsNoTracking().ToListAsync();
        }
    }
}