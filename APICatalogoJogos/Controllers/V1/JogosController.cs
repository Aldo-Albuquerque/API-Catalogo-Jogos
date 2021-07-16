using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using APICatalogoJogos.Exceptions;
using APICatalogoJogos.InputModel;
using APICatalogoJogos.Services;
using APICatalogoJogos.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APICatalogoJogos.Controllers.V1
{
    [Route("api/V1/[controller]")]
    [ApiController]
    public class JogosController : ControllerBase
    {
        private readonly IJogoService _jogoService;

        public JogosController(IJogoService jogoService)
        {
            _jogoService = jogoService;
        }

        /// <summary>
        /// Obter jogos por páginas;
        /// </summary>
        /// <remarks>
        /// Impossível obter os jogos sem paginação;
        /// </remarks>
        /// <param name="pagina">Indicar página a ser consultada. Min 1;</param>
        /// <param name="quantidade">Indicar número de jogos a serem exibidos por página. Min 1, Max 50;</param>
        /// <response code="200">Exibir os jogos;</response>
        /// <response code="204">Caso não haja jogos;</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JogoViewModel>>> Obter([FromQuery, Range(1, int.MaxValue)] int pagina = 1, [FromQuery, Range(1, 50)] int quantidade = 5)
        {
            var jogos = await _jogoService.Obter(pagina, quantidade);

            if (jogos.Count == 0)
                return NoContent();

            return Ok(jogos);
        }

        /// <summary>
        /// Obter jogo por Id;
        /// </summary>
        /// <param name="idJogo">Indicar Id do jogo a ser consultado;</param>
        /// <response code="200">Exibir jogo indicado;</response>
        [HttpGet("{idJogo:guid}")]
        public async Task<ActionResult<JogoViewModel>> Obter([FromRoute] Guid idJogo)
        {
            var jogo = await _jogoService.Obter(idJogo);

            if (jogo == null)
                return NoContent();

            return Ok();
        }

        /// <summary>
        /// Adicionar Jogo;
        /// </summary>
        /// <param name="jogoInputModel">Adicionar jogo com Título, Produtora e Preço;</param>
        /// <response code="200">Jogo adicionado com sucesso;</response>
        [HttpPost]
        public async Task<ActionResult<JogoViewModel>> AdicionarJogo([FromBody] JogoInputModel jogoInputModel)
        {
            try
            {
                var jogo = await _jogoService.Inserir(jogoInputModel);
                return Ok(jogo);
            }
            catch (JogoJaCadastradoException ex)
            {
                return UnprocessableEntity("Já existe um jogo com este título para esta produtora");
            }

        }

        /// <summary>
        /// Alterar jogo por Id;
        /// </summary>
        /// <param name="idJogo">Indicar Id do jogo a ser alterado;</param>
        /// <param name="jogoInputModel">Alterar jogo nos parâmetros indicados: Título/Produtora/Preço;</param>
        /// <response code="200">Jogo alterado com sucesso;</response>
        [HttpPut("{idJogo:guid}")]
        public async Task<ActionResult> AtualizarJogo([FromRoute] Guid idJogo, [FromBody] JogoInputModel jogoInputModel)
        {
            try
            {
                await _jogoService.Atualizar(idJogo, jogoInputModel);

                return Ok();
            }
            catch (JogoNaoCadastradoException ex)
            {
                return NotFound("Jogo inexistente");
            }
        }

        /// <summary>
        /// Alterar preço de jogo por Id;
        /// </summary>
        /// <param name="idJogo">Indicar Id do jogo a ser alterado;</param>
        /// <param name="preco">Definir novo preço para o jogo indicado;</param>
        /// <response code="200">Jogo alterado com sucesso;</response>
        [HttpPatch("{idJogo:guid}/preco/{preco:double}")]
        public async Task<ActionResult> AtualizarJogo([FromRoute] Guid idJogo, [FromRoute] double preco)
        {
            try
            {
                await _jogoService.Atualizar(idJogo, preco);

                return Ok();
            }
            catch (JogoNaoCadastradoException ex)
            {
                return NotFound("Jogo inexistente");
            }
        }

        /// <summary>
        /// Deletar jogo por Id;
        /// </summary>
        /// <param name="idJogo">Indicar Id do jogo a ser deletado;</param>
        /// <response code="200">Jogo deletado com sucesso;</response>
        [HttpDelete("{idJogo:guid}")]
        public async Task<ActionResult> DeletarJogo([FromRoute] Guid idJogo)
        {
            try
            {
                await _jogoService.Remover(idJogo);

                return Ok();
            }
            catch (JogoNaoCadastradoException ex)
            {
                return NotFound("Jogo inexistente");
            }
        }

    }
}
