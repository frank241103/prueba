public class Producto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; }
    public string Tipo { get; set; }  
    public string Estado { get; set; } 
}



public interface IProductoRepository
{
    Task<IEnumerable<Producto>> GetProductos();
    Task AddProducto(Producto producto);
    Task DeleteProducto(Guid id);
    Task MarkAsDefective(Guid id);
}


public interface IProductoRepository
{
    Task<IEnumerable<Producto>> GetProductos();
    Task AddProducto(Producto producto);
    Task DeleteProducto(Guid id);
    Task MarkAsDefective(Guid id);
}


public class ProductoRepository : IProductoRepository
{
    private readonly InventarioContext _context;
    public ProductoRepository(InventarioContext context) => _context = context;

    public async Task<IEnumerable<Producto>> GetProductos() => await _context.Productos.ToListAsync();

    public async Task AddProducto(Producto producto) { await _context.Productos.AddAsync(producto); await _context.SaveChangesAsync(); }

    public async Task DeleteProducto(Guid id) { var prod = await _context.Productos.FindAsync(id); if (prod != null) { _context.Productos.Remove(prod); await _context.SaveChangesAsync(); }}

    public async Task MarkAsDefective(Guid id) { var prod = await _context.Productos.FindAsync(id); if (prod != null) { prod.Estado = "Defectuoso"; await _context.SaveChangesAsync(); }}
}


[Route("api/productos")]
[ApiController]
public class ProductoController : ControllerBase
{
    private readonly IProductoRepository _repo;
    public ProductoController(IProductoRepository repo) => _repo = repo;

    [HttpGet] public async Task<IActionResult> GetProductos() => Ok(await _repo.GetProductos());
    [HttpPost] public async Task<IActionResult> AddProducto([FromBody] Producto prod) { await _repo.AddProducto(prod); return Ok(); }
    [HttpDelete("{id}")] public async Task<IActionResult> DeleteProducto(Guid id) { await _repo.DeleteProducto(id); return Ok(); }
    [HttpPut("defectuoso/{id}")] public async Task<IActionResult> MarkAsDefective(Guid id) { await _repo.MarkAsDefective(id); return Ok(); }
}


public void ConfigureServices(IServiceCollection services)
{
    services.AddDbContext<InventarioContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
    services.AddScoped<IProductoRepository, ProductoRepository>();
    services.AddControllers();
}


import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ProductoService {
  private apiUrl = 'http://localhost:5000/api/productos';
  constructor(private http: HttpClient) {}

  getProductos(): Observable<any> { return this.http.get(this.apiUrl); }
  addProducto(producto: any): Observable<any> { return this.http.post(this.apiUrl, producto); }
  deleteProducto(id: string): Observable<any> { return this.http.delete(`${this.apiUrl}/${id}`); }
  markAsDefective(id: string): Observable<any> { return this.http.put(`${this.apiUrl}/defectuoso/${id}`, {}); }
}
