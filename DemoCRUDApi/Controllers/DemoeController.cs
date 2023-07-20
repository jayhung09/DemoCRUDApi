using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DemoCRUDApi.Model;

namespace DemoCRUDApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DemoeController : ControllerBase
    {
        private readonly DemoDBContext _context;

        public DemoeController(DemoDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 取全部記錄
        /// </summary>
        /// <returns>全部紀錄</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CrudDemo>>> GetCrudDemo()
        {
            if (_context.CrudDemo == null)
            {
                return NotFound();
            }
            return await _context.CrudDemo.ToListAsync();
        }

        /// <summary>
        /// 根據ID取值
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>ID對應值(如果有)</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<CrudDemo>> GetCrudDemo(int id)
        {
            if (_context.CrudDemo == null)
            {
                return NotFound();
            }
            var crudDemo = await _context.CrudDemo.FindAsync(id);

            if (crudDemo == null)
            {
                return NotFound();
            }

            return crudDemo;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="id">要更新的目標ID</param>
        /// <param name="crudDemo">要更新成的結果</param>
        /// <returns>是否更新成功</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCrudDemo(int id, CrudDemo crudDemo)
        {
            if (id != crudDemo.Id)
            {
                return BadRequest();
            }

            var findResult = await _context.CrudDemo.FindAsync(id);
            if (findResult is null)
            {
                return NotFound();
            }

            findResult.DemoName = crudDemo.DemoName;
            _context.SetModified(findResult);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return NoContent();
        }

        /// <summary>
        /// 新增一筆
        /// </summary>
        /// <param name="crudDemo">新的一筆資料</param>
        /// <returns>是否新增成功</returns>
        [HttpPost]
        public async Task<ActionResult<CrudDemo>> PostCrudDemo([FromBody] CrudDemo crudDemo)
        {
            if (_context.CrudDemo == null)
            {
                return Problem("Entity set 'DemoDBContext.CrudDemo' is null.");
            }

            if (CrudDemoExists(crudDemo.Id))
            {
                return UnprocessableEntity($"{nameof(crudDemo.Id)} {crudDemo.Id} 已存在");
            }

            await _context.CrudDemo.AddAsync(crudDemo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCrudDemo", new { id = crudDemo.Id }, crudDemo);
        }

        /// <summary>
        /// 刪除一筆資料
        /// </summary>
        /// <param name="id">要刪除的資料ID</param>
        /// <returns>是否刪除成功</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCrudDemo(int id)
        {
            if (_context.CrudDemo == null)
            {
                return NotFound();
            }
            var crudDemo = await _context.CrudDemo.FindAsync(id);
            if (crudDemo == null)
            {
                return NotFound();
            }

            _context.CrudDemo.Remove(crudDemo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// 確認是否存在該 id
        /// </summary>
        /// <param name="id">ID</param>
        private bool CrudDemoExists(int id)
        {
            return (_context.CrudDemo?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
