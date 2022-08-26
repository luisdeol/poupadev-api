using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PoupaDev.API.Entities;
using PoupaDev.API.InputModels;
using PoupaDev.API.Persistence;

namespace PoupaDev.API.Controllers
{
    [ApiController]
    [Route("api/objetivos-financeiros")]
    public class ObjetivosFinanceirosController : ControllerBase
    {
        private readonly PoupaDevDbContext _context;
        private readonly string _connectionString;
        public ObjetivosFinanceirosController(PoupaDevDbContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("PoupaDevCs");
        }

        [HttpGet]
        public async Task<IActionResult> GetTodos() {
            using (var sqlConnection = new SqlConnection(_connectionString)) {
                const string sql = "SELECT * FROM Objetivos";
                var objetivos = await sqlConnection.QueryAsync<ObjetivoFinanceiro>(sql);

                return Ok(objetivos);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetPorId(int id) {
            var objetivo = _context
                .Objetivos
                .Include(o => o.Operacoes)
                .SingleOrDefault(s => s.Id == id);

            if (objetivo == null)
                return NotFound();

            return Ok(objetivo);
        }

        [HttpPost]
        public async Task<IActionResult> Post(ObjetivoFinanceiroInputModel model) {
            var objetivo = new ObjetivoFinanceiro(model.Titulo, model.Descricao,model.ValorObjetivo);

            var parameters = new {
                Titulo = objetivo.Titulo,
                Descricao = objetivo.Descricao,
                ValorObjetivo = objetivo.ValorObjetivo,
                DataCriacao = objetivo.DataCriacao
            };

            using (var sqlConnection = new SqlConnection(_connectionString)) {
                const string sql = @"INSERT INTO Objetivos OUTPUT INSERTED.Id VALUES (@Titulo, @Descricao, @ValorObjetivo, @DataCriacao)";

                var id = await sqlConnection.ExecuteScalarAsync<int>(sql, parameters);

                return CreatedAtAction("GetPorId", new { id }, model);
            }
        }

        [HttpPost("{id}/operacoes")]
        public IActionResult PostOperacao(int id, OperacaoInputModel model) {
            var operacao = new Operacao(model.Valor, model.TipoOperacao, id);

            var objetivo = _context.Objetivos
                .Include(o => o.Operacoes)
                .SingleOrDefault(o => o.Id == id);

            if (objetivo == null)
                return NotFound();

            
            objetivo.RealizarOperacao(operacao);

            _context.SaveChanges();

            return NoContent();
        }

        [Route("/error")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Error()
            => Problem();
    }
}